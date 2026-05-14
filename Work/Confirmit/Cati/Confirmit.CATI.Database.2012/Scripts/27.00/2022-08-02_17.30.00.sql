GO
PRINT N'Altering [dbo].[BvSpSurveyProductivityReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyProductivityReport]
	@SurveySids NVARCHAR(MAX), 
    @PersonSIDs NVARCHAR (MAX), 
    @ITS NVARCHAR (MAX), 
    @StartDate DATETIME, 
    @EndDate DATETIME,
	@SurveyDataFilter NVARCHAR(MAX),
    @StartShiftTime DATETIME,
    @EndShiftTime DATETIME

WITH RECOMPILE

AS
IF(@SurveySids IS NULL AND 
   @PersonSIDs IS NULL AND
   @ITS IS NULL AND
   @StartDate IS NULL AND
   @EndDate IS NULL)
BEGIN
   SELECT 0 AS [PersonSID],
          '' AS [PersonCode],           
          '' AS [PersonName],
          0 AS [SurveySID],
          '' AS [SurveyCode],
          '' AS [SurveyName],
          cast(0 as SMALLINT)  AS [StateID],
          '' AS [StateName],
          0 AS [InterviewCount],
          0 AS [TotalInterviewCount],
          0 AS [InterviewTime],
          cast(0 as decimal) AS [InterviewTimePercentage]
          
   RETURN 0;
END
    
          
          
DECLARE @DefaultStateGroupID INTEGER
SELECT top(1) @DefaultStateGroupID = ID 
FROM BvStateGroup 
ORDER BY [Order] ASC;
 
CREATE TABLE #surveySids([SurveyId] int primary key, [SurveyCode] nvarchar(max), [Description] nvarchar(max))
insert into #surveySids 
SELECT [SID] AS [SurveyId],
          [Name] AS [SurveyCode],
          [Description]
FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')
INNER JOIN BvSurvey ON SID = Item

CREATE TABLE #SelectedStatuses([StateID] SMALLINT primary key, [StateName] nvarchar(max))
insert into #SelectedStatuses
SELECT [s].[StateID],
       [s].[Name] [StateName]
FROM [BvState] [s]
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@ITS, ''), ',') ON [s].[StateID] = [Item]
WHERE (@ITS IS NULL OR [Item] IS NOT NULL) AND [s].[StateGroupID] = @DefaultStateGroupID 
         
create table #persons(sid int primary key, [PersonCode] nvarchar(max), [PersonName] nvarchar(max))
insert into #persons
SELECT SID, 
       CAST([SID] AS NVARCHAR(MAX)) [PersonCode],
       [Name] [PersonName]
FROM BvPerson
LEFT JOIN dbo.utilSplitNumbers(ISNULL(@PersonSIDs, ''), ',') ON [SID] = [Item]
WHERE @PersonSIDs IS NULL OR [Item] IS NOT NULL AND EXISTS
  (
     SELECT * 
     FROM [BvPersonRel] 
     WHERE SID = PersonSid AND RoleId = 2
  )


CREATE TABLE #respids 
( 
	surveyid INT,
	respid int,
	PRIMARY KEY CLUSTERED 
	(
		[SurveyId] ASC,
		[respid] ASC
	)
)

IF (@SurveyDataFilter IS NOT NULL)
BEGIN
	DECLARE @sql NVARCHAR(MAX)
	SET @sql = N'INSERT INTO #respids SELECT ' +  @SurveySids + ', respid from [dbo].[BvReplicatedData_' + @SurveySids + '] AS CFInterview WHERE ' + @SurveyDataFilter 
	EXEC (@sql)
END


DECLARE @diff TIME

IF CAST(@StartShiftTime AS TIME) > CAST(@EndShiftTime AS TIME)
	select @diff = CAST('00:00:00' - (@StartShiftTime-@EndShiftTime) AS TIME )
else
	select @diff = CAST(@EndShiftTime-@StartShiftTime AS TIME)

;WITH BvHistory_CTE AS
(
   SELECT [history].SurveyId, [history].ITS, [history].Duration, [history].PersonSid,
   COUNT([history].its) OVER(partition by [history].[SurveyId], [history].[PersonsId]) as TotalInterviewCount,
   SUM([history].[Duration]) OVER(partition by [history].[SurveyId], [history].[PersonsId]) as TotalInterviewTime
   FROM #surveySids [survey] 
   INNER JOIN [BvHistory] [history] ON [survey].[SurveyId] = [history].[SurveyId]
   LEFT JOIN #respids i on i.respid = [history].InterviewId AND i.surveyid = [history].SurveyId
   WHERE (i.respid IS NOT NULL OR @SurveyDataFilter IS NULL) AND [history].[FiredTime] BETWEEN @StartDate AND @EndDate AND
         [history].[RoleID] = 2 AND
		 (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
),
BvHistoryWithStates_CTE AS
(
   SELECT [history].*, [state].*
   FROM BvHistory_CTE [history]
   INNER JOIN #SelectedStatuses [state] ON [state].[StateID] = [history].[ITS]
), GroupedByPersonCounts AS
(
SELECT 
	 [person].[SID] AS [PersonSID],
	 [person].[PersonCode],           
	 [person].[PersonName],
                
	 [history].[SurveyId] AS [SurveySID],
	 [history].[StateID] AS [StateID],
	 [history].[StateName],
 
	 COUNT(*) AS [InterviewCount], /* Interview count for status. */
    
	 /* Total calls count for the selected person and survey (regardless to status). */
	 MAX(TotalInterviewCount) AS [TotalInterviewCount],

	 ISNULL(SUM([history].[Duration]), 0) AS [InterviewTime], /* Interview time in seconds. */

	 /* If TotalInterviewTime is null or zero then SUM([history].[Duration]) is null or zero so the result will be 0 */
	 100.0 *  ISNULL(SUM([history].[Duration]), 0) / ISNULL(NULLIF(MAX(TotalInterviewTime), 0), 1) AS [InterviewTimePercentage] /* Interview time percentage */

	 FROM #persons [person]
	 INNER JOIN BvHistoryWithStates_CTE [history] ON [history].[PersonSID] = [person].[SID]

	 GROUP BY   [history].[SurveyId],
				[history].[StateId], 
				[history].[StateName],
				[person].[SID], 
				[person].[PersonCode], 
				[person].[PersonName]
 )
 SELECT 
	c.*,
	s.[SurveyCode] AS [SurveyCode],
	s.[Description] AS [SurveyName]
 FROM GroupedByPersonCounts c
 JOIN #SurveySids s
	ON c.SurveySID = s.SurveyId
 ORDER BY [PersonCode], [StateId]

RETURN 0
GO
PRINT N'Update complete.';


GO
