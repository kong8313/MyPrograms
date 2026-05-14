PRINT N'Altering [dbo].[BvSpInterviewerProductivityReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewerProductivityReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @CalcAllBreakHistory BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME, 
 @SurveyDataFilter NVARCHAR(MAX),
 @StartShiftTime DATETIME,
 @EndShiftTime DATETIME
 
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @PersonSids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS PersonId,
		    '' AS PersonName,
		    0 AS LogOnTime,
			0 AS WaitingTime,
			0 AS OnBreakTime,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end

 DECLARE @DiallerName NVARCHAR(20)
 SET  @DiallerName = N'Dialer';
 
 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 CREATE TABLE #Persons(PersonSid int primary key, Name NVARCHAR (255), Duration int)
 INSERT INTO #Persons
 SELECT p.SID AS PersonSid,
        p.Name AS Name,
		NULL
 FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
 INNER JOIN BvPerson p ON p.SID = s.Item
 UNION 
 SELECT p.Sid AS PersonSid,
        p.Name AS Name,
		NULL
 FROM BvPerson p
 WHERE @PersonSids IS NULL
 UNION
 SELECT DialerSid AS PersonSid,
        @DiallerName AS Name,
		NULL
 FROM (SELECT 0 AS DialerSid) dailerSids
 WHERE @UseDialer = 1

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #SurveyIdsList(SurveyId  int primary key)
 insert into #SurveyIdsList
 SELECT Item AS SurveyId 
 FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',')

 IF ( @CalcAllBreakHistory = 1 )
	INSERT INTO #SurveyIdsList VALUES(0)

DECLARE @diff TIME

IF CAST(@StartShiftTime AS TIME) > CAST(@EndShiftTime AS TIME)
	select @diff = CAST('00:00:00' - (@StartShiftTime-@EndShiftTime) AS TIME )
else
	select @diff = CAST(@EndShiftTime-@StartShiftTime AS TIME)


 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(
	   	    CASE WHEN CAST( DATEADD ( SECOND, Duration, StartTime ) AS TIME ) <= ISNULL(CAST(@EndShiftTime AS TIME), CAST('23:59:59' AS TIME ))
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END
	), 0) Duration, InterviewerId
    FROM BvTimeBreaksHistory h
	LEFT JOIN #SurveyIdsList s
	ON h.SurveyId = s.SurveyId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime AND ( s.SurveyId IS NOT NULL )
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY InterviewerId
 )
 update #persons
 set duration = TimeBreaksHistory.Duration
 from TimeBreaksHistory
 where #persons.PersonSid = TimeBreaksHistory.interviewerId

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

;WITH FilteredHistory AS 
(
	SELECT * FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
 SELECT
  p.PersonSid AS PersonId,
  p.Name AS PersonName,
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(p.Duration, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(p.Duration, 0) AS OnBreakTime,
  COUNT(h.InterviewId) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  COUNT(cil.CompletedIts) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
 FROM #Persons p
 LEFT JOIN FilteredHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls which were added during sample addition
        h.SurveyId IN (SELECT sil.SurveyId FROM #SurveyIdsList sil)
 LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
	 LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
	 WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL

 GROUP BY p.PersonSid, p.Name, p.Duration
 HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)
GO
PRINT N'Altering [dbo].[BvSpSurveyOverviewReportForAllPersons]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyOverviewReportForAllPersons]
 @SurveySids NVARCHAR (MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME,
 @SurveyDataFilter NVARCHAR(MAX),
 @StartShiftTime DATETIME,
 @EndShiftTime DATETIME

 WITH RECOMPILE
AS 

 if(@SurveySids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS SurveyId,
		    '' AS ProjectId,
		    '' AS Title,
		    0 AS LogOnTime,
		    0 AS WaitingTime,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end


 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #Surveys(SurveyId  int primary key, ProjectId NVARCHAR (255), Title NVARCHAR (255))
 insert into #Surveys
 SELECT 
	s.SID AS SurveyId ,
	s.Name AS ProjectId,
	s.Description AS Title
  FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',') ss
  INNER JOIN BvSurvey s ON s.SID = ss.Item
 
 
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


;WITH FilteredHistory AS 
(
	SELECT * FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  COUNT(h.InterviewID) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  COUNT(cil.CompletedIts) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration

  FROM #Surveys s 
  LEFT JOIN FilteredHistory h ON s.SurveyId = h.SurveyId AND
                           h.FiredTime >= @StartDateTime AND
                           h.FiredTime <= @EndDateTime AND
                           h.RoleID = 2 --we should not calced calls whuch were added during sample addition
  LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
  LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
  WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL
  GROUP BY s.SurveyId, s.ProjectId, s.Title
  HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0)
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
		  cast(0 as tinyint)  AS [StateID],
		  '' AS [StateName],
		  0 AS [InterviewCount],
		  0 AS [TotalInterviewCount],
          0 AS [InterviewTime]
          
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

CREATE TABLE #SelectedStatuses([StateID] tinyint primary key, [StateName] nvarchar(max))
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
   SELECT [history].*, [SurveyCode], [Description]
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
)
 
SELECT 
 [person].[SID] AS [PersonSID],
 [person].[PersonCode],           
 [person].[PersonName],
                
 [history].[SurveyId] AS [SurveySID],
 [history].[SurveyCode] AS [SurveyCode],
 [history].[Description] AS [SurveyName],

 [history].[StateID] AS [StateID],
 [history].[StateName],
 
 COUNT(*) AS [InterviewCount], /* Interview count for status. */
    
 /* Total calls count for the selected person and survey (regardless to status). */
 (SELECT COUNT(*) 
  FROM [BvHistory_CTE] [h1] 
  WHERE [person].[SID] = [h1].[PersonSID] AND
        [history].[SurveyId] = [h1].[SurveyId] AND
        [h1].[ITS] IS NOT NULL ) AS [TotalInterviewCount],

 
 ISNULL(SUM([history].[Duration]), 0) AS [InterviewTime] /* Interview time in seconds. */

 FROM #persons [person]
 INNER JOIN BvHistoryWithStates_CTE [history] ON [history].[PersonSID] = [person].[SID]

 GROUP BY   [history].[SurveyId],
            [history].[SurveyCode],
            [history].[Description],
            [history].[StateId], 
            [history].[StateName],
            [person].[SID], 
            [person].[PersonCode], 
            [person].[PersonName]
 
 ORDER BY [person].[PersonCode], [history].[StateId]

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpSurveyOverviewReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpSurveyOverviewReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME,
 @SurveyDataFilter NVARCHAR(MAX),
 @StartShiftTime DATETIME,
 @EndShiftTime DATETIME

 
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @PersonSids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS SurveyId,
		    '' AS ProjectId,
		    '' AS Title,
		    0 AS LogOnTime,
		    0 AS WaitingTime,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end

 if( @PersonSids is null)
 BEGIN
    exec BvSpSurveyOverviewReportForAllPersons @SurveySids, @CompletedItses, @UseDialer, @HideEmpty, @StartDateTime, @EndDateTime, @SurveyDataFilter, @StartShiftTime, @EndShiftTime
	return
 END

 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #Surveys(SurveyId  int primary key, ProjectId NVARCHAR (255), Title NVARCHAR (255))
 insert into #Surveys
 SELECT s.SID AS SurveyId ,
	    s.Name AS ProjectId,
	    s.Description AS Title
 FROM dbo.utilSplitNumbers( ISNULL(@SurveySids, ''), ',') ss
 INNER JOIN BvSurvey s ON s.SID = ss.Item

 create table #Persons(PersonSid int primary key)
 insert into #Persons
 SELECT p.SID AS PersonSid
  FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
  INNER JOIN BvPerson p ON p.SID = s.Item
 UNION
 SELECT DialerSid AS PersonSid
 FROM (SELECT 0 AS DialerSid) dailerSids
 WHERE @UseDialer = 1
 

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


;WITH FilteredHistory AS 
(
	SELECT * FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  COUNT(h.InterviewID) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  ISNULL(SUM(cil.CompletedIts/cil.CompletedIts), 0) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration
  FROM #Surveys s 
  LEFT JOIN FilteredHistory h 
        ON s.SurveyId = h.SurveyId AND
           h.FiredTime >= @StartDateTime AND
           h.FiredTime <= @EndDateTime AND
           h.RoleID = 2 --we should not calced calls whuch were added during sample addition
        AND h.PersonSID IN (SELECT p.PersonSid FROM #Persons p)
    LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
	LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
	WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL
	GROUP BY s.SurveyId, s.ProjectId, s.Title
    HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0)
GO
PRINT N'Update complete.';


GO
