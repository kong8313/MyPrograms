GO
PRINT N'Altering [dbo].[BvSpAttemptsByDispositionReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpAttemptsByDispositionReport]
   @SurveySid INT,
   @Itses NVARCHAR(MAX),
   @HideEmpty BIT,
   @StartDateTime DATETIME,
   @EndDateTime DATETIME,
   @CallCenterId INTEGER = NULL

   WITH RECOMPILE
AS
    DECLARE @StateGroupId INT,
	@MaxAttempts int = 11
    SELECT @StateGroupId = s.StateGroupID
    FROM BvSurvey s
    WHERE s.Sid = @SurveySid;
    
    IF(@StartDateTime IS NULL) SET @StartDateTime = '01-01-1753 00:00:00'
    IF(@EndDateTime IS NULL) SET @EndDateTime = '12-31-9999 23:59:59.997'

    ;WITH NecessaryItsList AS
    (
       SELECT s.StateID AS Its,
              s.Name AS [Name]
       FROM dbo.utilSplitNumbers( ISNULL(@Itses, ''), ',') i
       INNER JOIN BvState s ON (s.StateGroupID = @StateGroupId AND
                                s.StateID = i.Item)
       
       UNION 
       
       SELECT s.StateID AS Its,
              s.Name AS [Name]
       FROM BvState s
       WHERE @Itses IS NULL AND
             s.StateGroupID = @StateGroupId
    ),
	AllAttempts AS
	(
	   SELECT ( ROW_NUMBER() over(partition by InterviewID order by FiredTime)) AS NumberAttempts,
	          h.InterviewID AS InterviewId,
	          s.StateId AS Its,
	          s.Name AS ItsName
	   FROM BvState s
	   LEFT JOIN BvHistory h ON s.StateId = h.ITS AND
	                            h.SurveyId = @SurveySid AND
	                            h.FiredTime >= @StartDateTime AND
	                            h.FiredTime <= @EndDateTime AND
	                            h.InterviewId IS NOT NULL AND
	                            h.RoleID = 2
	   WHERE s.StateGroupID = @StateGroupId AND (h.CallCenterID = @CallCenterId OR @CallCenterId IS NULL OR h.ID is NULL) 
	),
	Attempts AS
	(
	   SELECT IIF(NumberAttempts > @MaxAttempts , @MaxAttempts, NumberAttempts ) AS NumberAttempts,
	          InterviewId,
	          Its,
	          ItsName
	   FROM AllAttempts
	),
	AttemptsByDesposition AS
	(
	   SELECT Its AS  Code,
	          ItsName AS Disposition,
              [1] AS Attempts1,
              [2] AS Attempts2,
              [3] AS Attempts3,
              [4] AS Attempts4,
              [5] AS Attempts5,
              [6] AS Attempts6,
              [7] AS Attempts7,
              [8] AS Attempts8,
              [9] AS Attempts9,
              [10] AS Attempts10,
			  [11] AS Attempts11AndMore
       FROM Attempts a
       PIVOT
       (
          COUNT(a.InterviewId) 
          FOR a.NumberAttempts in ( [1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11])
       ) AS p
       WHERE (@HideEmpty = 0 OR
              [1]+[2]+[3]+[4]+[5]+[6]+[7]+[8]+[9]+[10]+[11] > 0)
    )
    SELECT abd.*
    FROM AttemptsByDesposition abd
    INNER JOIN NecessaryItsList il ON il.Its = abd.Code
GO
PRINT N'Altering [dbo].[BvSpNumberOfAttemptsReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpNumberOfAttemptsReport]
   @SurveySid INT,
   @StartDateTime DATETIME,
   @EndDateTime DATETIME,
   @TotalSampleSize INT OUTPUT,
   @CallCenterId INTEGER = NULL
AS
	SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
	SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

   IF @SurveySid IS NULL AND @StartDateTime IS NULL AND @EndDateTime IS NULL AND @TotalSampleSize IS NULL
   BEGIN
      SELECT 0 as Attempts, 0 as Records, 0 as [SampleSize]
    
      RETURN 0
   END

   --1) should we check state here?
   --2) should we that time is necessary for sample here?
   SELECT @TotalSampleSize = COUNT(*)
   FROM BvInterview
   WHERE SurveySID = @SurveySid;
   
   CREATE TABLE #temp( Attempts INT, Records INT);

   WITH NotEmptyAttempts AS
   (
      SELECT COUNT(*) AS Attempts, 
             1 AS InterviewCount 
      FROM BvHistory h
      WHERE h.SurveyId = @SurveySid AND
            h.RoleID = 2 AND --don't calc sample calls
            h.FiredTime BETWEEN @StartDateTime AND @EndDateTime AND
			h.ITS <> 15 AND h.ITS <> 25 AND		-- 15 returned not dialed, 25-expired
            h.InterviewId IS NOT NULL AND 
            (h.CallCenterID = @CallCenterId OR @CallCenterId IS NULL)  
      GROUP BY h.InterviewId
   ),
   NotEmptyOutputList AS
   (
	   SELECT nea.Attempts AS Attempts,
			  COUNT(nea.InterviewCount) AS Records
	   FROM NotEmptyAttempts nea
	   GROUP BY nea.Attempts
   )
   INSERT INTO #temp
   SELECT neol.Attempts Attempts,
          neol.Records Records
   FROM NotEmptyOutputList neol;
   
   WITH AllAttempts AS
   (
      SELECT MAX(Attempts) AS Attempts
      FROM #temp
      
      UNION ALL
      
      SELECT Attempts-1
      FROM AllAttempts
      WHERE Attempts > 1
   )
   SELECT aa.Attempts,
          ISNULL(t.Records, 0) Records,
		  @TotalSampleSize as [SampleSize]
   FROM AllAttempts aa
   LEFT JOIN #temp t ON t.Attempts = aa.Attempts
   WHERE aa.Attempts IS NOT NULL
   ORDER BY aa.Attempts
   OPTION (MAXRECURSION 500)
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
 @EndShiftTime DATETIME,
 @IncludeOpenEndReviewTimeInInterviewDuration BIT = 1,
 @CallCenterId INTEGER = NULL
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS SurveyId,
		    '' AS ProjectId,
		    '' AS Title,
		    0 AS LogOnTime,
			0 AS OnBreakTimePaid,
			0 AS OnBreakTimeUnpaid,
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

 create table #Surveys(SurveyId  int primary key, ProjectId NVARCHAR (255), Title NVARCHAR (255), DurationPaid int, DurationUnpaid int)
 insert into #Surveys
 SELECT 
	s.SID AS SurveyId ,
	s.Name AS ProjectId,
	s.Description AS Title,
	NULL,
	NULL
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

 
 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(ISNULL(
	   	    	CASE WHEN CAST( '00:00:00' - (DATEADD ( SECOND, Duration, StartTime ) - @EndShiftTime)  AS TIME )  < @diff
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END, Duration)
	), 0) Duration, 
	SurveyId,
	ISNULL(bt.IsPaid, 1) as IsPaid
    FROM BvTimeBreaksHistory h
	LEFT JOIN BvBreakType bt on bt.Id = h.BreakTypeId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY SurveyId, IsPaid
 ),
 AggregatedBreaksHistory AS (
	SELECT tbh.SurveyId,
	DurationPaid = sum(CASE WHEN tbh.IsPaid = 1 THEN tbh.Duration ELSE 0 END),
	DurationUnpaid = sum(CASE WHEN tbh.IsPaid = 0 THEN tbh.Duration ELSE 0 END)
	FROM TimeBreaksHistory tbh
	GROUP BY tbh.SurveyId
 )
 update #Surveys
 SET 
 DurationPaid = AggregatedBreaksHistory.DurationPaid,
 DurationUnpaid = AggregatedBreaksHistory.DurationUnpaid
 FROM AggregatedBreaksHistory
 WHERE #Surveys.SurveyId = AggregatedBreaksHistory.SurveyId


;WITH FilteredHistory AS 
(
	SELECT 
	FiredTime,
	ConfirmitDuration,
	WaitingTime,
	SurveyId,
	InterviewID,
	RoleID,
	PersonSID,
	ITS,
	CallCenterId,
	IIF(@IncludeOpenEndReviewTimeInInterviewDuration = 1, Duration,  Duration + ISNULL(OpenEndReviewDuration, 0)) AS Duration
	FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(s.DurationPaid, 0) + ISNULL(s.DurationUnpaid, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(s.DurationPaid, 0) AS OnBreakTimePaid,
  ISNULL(s.DurationUnpaid, 0) AS OnBreakTimeUnpaid,
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
  WHERE (i.respid IS NOT NULL OR @SurveyDataFilter IS NULL) AND (h.CallCenterID = @CallCenterId OR @CallCenterId IS NULL OR h.CallCenterID IS NULL) 
  GROUP BY s.SurveyId, s.ProjectId, s.Title, s.DurationPaid, s.DurationUnpaid
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
    @EndShiftTime DATETIME,
    @CallCenterId INTEGER = NULL

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
          cast(0 as SMALLINT) AS [StateID],
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
WHERE ((@CallCenterId IS NULL OR CallCenterID = @CallCenterId) AND @PersonSIDs IS NULL) OR [Item] IS NOT NULL AND EXISTS
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
 @EndShiftTime DATETIME,
 @IncludeOpenEndReviewTimeInInterviewDuration BIT = 1,
 @CallCenterId INTEGER = NULL

 
 WITH RECOMPILE
AS 

 if(@SurveySids is null and @PersonSids is null and @CompletedItses is null and @UseDialer is null and @HideEmpty is null and @StartDateTime is null and @EndDateTime is null)
 begin
    select  0 AS SurveyId,
		    '' AS ProjectId,
		    '' AS Title,
		    0 AS LogOnTime,
		    0 AS WaitingTime,
			0 AS OnBreakTimePaid,
			0 AS OnBreakTimeUnpaid,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration
    return
 end

 if( @PersonSids is null)
 BEGIN
    exec BvSpSurveyOverviewReportForAllPersons @SurveySids, @CompletedItses, @UseDialer, @HideEmpty, @StartDateTime, @EndDateTime, @SurveyDataFilter, @StartShiftTime, @EndShiftTime, @IncludeOpenEndReviewTimeInInterviewDuration, @CallCenterId
	return
 END

 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 create table #CompletedItsList(CompletedIts  int primary key)
 insert into #CompletedItsList
 select * from dbo.utilSplitNumbers( ISNULL(@CompletedItses, ''), ',');

 create table #Surveys(SurveyId  int primary key, ProjectId NVARCHAR (255), Title NVARCHAR (255),  DurationPaid int, DurationUnpaid int)
 insert into #Surveys
 SELECT s.SID AS SurveyId ,
	    s.Name AS ProjectId,
	    s.Description AS Title,
		NULL,
		NULL
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


 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(ISNULL(
	   	    	CASE WHEN CAST( '00:00:00' - (DATEADD ( SECOND, Duration, StartTime ) - @EndShiftTime)  AS TIME )  < @diff
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END, Duration)
	), 0) Duration, 
	SurveyId,
	ISNULL(bt.IsPaid, 1) as IsPaid
    FROM BvTimeBreaksHistory h
	LEFT JOIN #Persons p
	ON h.InterviewerId = p.PersonSid
	LEFT JOIN BvBreakType bt on bt.Id = h.BreakTypeId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime AND ( p.PersonSid IS NOT NULL )
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY SurveyId, IsPaid
 ),
 AggregatedBreaksHistory AS (
	SELECT tbh.SurveyId,
	DurationPaid = sum(CASE WHEN tbh.IsPaid = 1 THEN tbh.Duration ELSE 0 END),
	DurationUnpaid = sum(CASE WHEN tbh.IsPaid = 0 THEN tbh.Duration ELSE 0 END)
	FROM TimeBreaksHistory tbh
	GROUP BY tbh.SurveyId
 )
 update #Surveys
 SET 
 DurationPaid = AggregatedBreaksHistory.DurationPaid,
 DurationUnpaid = AggregatedBreaksHistory.DurationUnpaid
 FROM AggregatedBreaksHistory
 WHERE #Surveys.SurveyId = AggregatedBreaksHistory.SurveyId


;WITH FilteredHistory AS 
(
	SELECT 
	FiredTime,
	ConfirmitDuration,
	WaitingTime,
	SurveyId,
	InterviewID,
	RoleID,
	PersonSID,
	ITS,
	IIF(@IncludeOpenEndReviewTimeInInterviewDuration = 1, Duration,  Duration + ISNULL(OpenEndReviewDuration, 0)) AS Duration
	FROM BvHistory 
	WHERE (@StartShiftTime IS NULL OR CAST( firedTime - @StartShiftTime  AS TIME ) <= @diff)
)
SELECT
  s.SurveyId AS SurveyId,
  s.ProjectId AS ProjectId,
  s.Title AS Title,
  
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(s.DurationPaid, 0) + ISNULL(s.DurationUnpaid, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(s.DurationPaid, 0) AS OnBreakTimePaid,
  ISNULL(s.DurationUnpaid, 0) AS OnBreakTimeUnpaid,
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
	GROUP BY s.SurveyId, s.ProjectId, s.Title, s.DurationPaid, s.DurationUnpaid
    HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0)
GO
PRINT N'Update complete.';


GO
