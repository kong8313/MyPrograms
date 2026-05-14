PRINT N'Altering [dbo].[BvSpAlertsHistoryAggregatedReport]...';


GO
ALTER PROCEDURE BvSpAlertsHistoryAggregatedReport
    @PersonIds NVARCHAR(MAX),
    @SurveyIds NVARCHAR(MAX),
    @StartDate DATETIME,
    @EndDate   DATETIME,
    @InterviewState TINYINT
 AS
	;WITH Persons AS
	(
		SELECT p.SID AS PersonId,
			   p.Name AS PersonName
		FROM dbo.utilSplitNumbers( ISNULL(@PersonIds, ''), ',') s
		INNER JOIN BvPerson p ON p.SID = s.Item
		
		UNION 

		SELECT p.SID AS PersonId,
		       p.Name AS PersonName
		FROM BvPerson p
		WHERE @PersonIds IS NULL
	),
	Surveys AS
	(
		SELECT s.Item AS SurveyId
		FROM dbo.utilSplitNumbers( ISNULL(@SurveyIds, ''), ',') s
	)
	SELECT p.PersonId,
		   p.PersonName,
           ISNULL(SUM(h.AnswerSubmissionAlert^1), 0) AnswerSubmissionAmberCounts,
           ISNULL(SUM(h.AnswerSubmissionAlert^0), 0) AnswerSubmissionRedCounts,
           ISNULL(SUM(h.QuickAnswerSubmissionAlert^1), 0) QuickAnswerSubmissionAmberCounts,
           ISNULL(SUM(h.QuickAnswerSubmissionAlert^0), 0) QuickAnswerSubmissionRedCounts
    FROM BvAnswerSubmissionAlertHistory h
    INNER JOIN Persons p ON p.PersonId = h.PersonId
    INNER JOIN Surveys s ON s.SurveyID = h.SurveyId
    WHERE SubmissionTime >= @startDate AND
          SubmissionTime <= @endDate AND
          (InterviewState = @InterviewState OR @InterviewState IS NULL)
    GROUP BY p.PersonId, p.PersonName
GO
PRINT N'Altering [dbo].[BvSpAlertsHistoryReport]...';


GO
ALTER PROCEDURE BvSpAlertsHistoryReport
	@personIds NVARCHAR(MAX),
	@surveyIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT
 AS
 
	IF @personIds IS NULL AND @surveyIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
	/* Looks like we're generating code using FMTONLY. So lets return metadata*/
	SELECT  0 AS PersonId,
			'' AS PersonName,
			0 AS SurveyId,
			'' AS ProjectId,
			'' AS SurveyName,
			0 AlertType,
			cast(0 as bit) Alert,
			0 AS AnswerDuration,
			'' AS QuestionId,
			CAST(NULL AS DATETIME) AS SubmissionTime,
			0 AS InterviewId,
			CAST(0 AS TINYINT) AS InterviewState
     WHERE 1 = 0
	 RETURN 0;
	END
 
    DECLARE @query NVARCHAR(MAX) = '
    SELECT p.Sid AS PersonId,
           p.Name AS PersonName,
           s.SID AS SurveyId,
           s.Name AS ProjectId,
           s.Description AS SurveyName,
           (CASE WHEN h.AnswerSubmissionAlert IS NULL THEN 2 ELSE 1 END) AlertType,
           (CASE WHEN h.AnswerSubmissionAlert IS NULL THEN h.QuickAnswerSubmissionAlert ELSE h.AnswerSubmissionAlert END) Alert,
           h.AnswerDuration,
           h.QuestionId,
           h.SubmissionTime,
           h.InterviewId,
           h.InterviewState
    FROM BvAnswerSubmissionAlertHistory h
    LEFT JOIN dbo.utilSplitNumbers( ''' + ISNULL(@PersonIds, '') + ''', '','') s1 ON s1.Item = h.PersonId
    INNER JOIN BvPerson p ON p.Sid = h.PersonId
    INNER JOIN dbo.utilSplitNumbers( ISNULL(''' + @SurveyIds + ''', ''''), '','') s2 ON s2.Item = h.SurveyId
    INNER JOIN BvSurvey s ON s.SID = h.SurveyId
    WHERE '''' = ''' + ISNULL(@PersonIds, '') + ''' OR s1.Item IS NOT NULL'

    DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
    RETURN @TotalCount
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpInterviewerBreaksReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewerBreaksReport]
    @personIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT
AS
	IF @personIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
		SELECT  '' AS PersonName,
				CAST(NULL AS DATETIME) AS StartTime,
				0 AS Duration
		WHERE 1 = 0
		RETURN 0;
	END
	
	DECLARE @query NVARCHAR(MAX) = '
	SELECT Name PersonName,
	       StartTime,
		   Duration
	FROM BvTimeBreaksHistory
	INNER JOIN dbo.utilSplitNumbers( ''' + ISNULL(@PersonIds, '') + ''', '','') s1 ON s1.Item = InterviewerId
	INNER JOIN BvPerson ON SID = InterviewerId'
	      
	DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
    RETURN @TotalCount
RETURN 0
GO
PRINT N'Altering [dbo].[BvSpInterviewerProductivityReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewerProductivityReport]
 @SurveySids NVARCHAR (MAX),
 @PersonSids NVARCHAR(MAX),
 @CompletedItses NVARCHAR(MAX),
 @UseDialer BIT,
 @HideEmpty BIT,
 @StartDateTime DATETIME,
 @EndDateTime DATETIME
 
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

 ;WITH TimeBreaksHistory AS
 (
    SELECT ISNULL(SUM(Duration), 0) Duration, InterviewerId
    FROM BvTimeBreaksHistory
    WHERE StartTime >= @StartDateTime AND
          StartTime <= @EndDateTime
    GROUP BY InterviewerId
 )
 update #persons
 set duration = TimeBreaksHistory.Duration
 from TimeBreaksHistory
 where #persons.PersonSid = TimeBreaksHistory.interviewerId

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
 LEFT JOIN BvHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls which were added during sample addition
        h.SurveyId IN (SELECT sil.SurveyId FROM #SurveyIdsList sil)
 LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
 GROUP BY p.PersonSid, p.Name, p.Duration
 HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)
GO
PRINT N'Altering [dbo].[BvSpReportSampleStatusSummary]...';


GO
ALTER PROCEDURE [dbo].[BvSpReportSampleStatusSummary]
@SurveySID INT, 
@PersonsSIDs NVARCHAR (2000), 
@ITSIDs NVARCHAR (1000)
AS
IF @SurveySID IS NULL AND @PersonsSIDs IS NULL AND @ITSIDs IS NULL
BEGIN
    SELECT 
    0 as [StateID],
    '' as [StateName],
    0 as [Count],
    '' as [SurveyName],
    0 as [SampleSize],
    0 as [Calls],
    '' as [Person]
    
    RETURN 0
END

DECLARE @StrSurveySID NVARCHAR (16)
SET @StrSurveySID = CAST(@SurveySID AS NVARCHAR(16))

DECLARE @SurveyQreName NVARCHAR (255), @SurveyDescription NVARCHAR (255)
SELECT @SurveyQreName = ISNULL(Name, '''') FROM BvSurvey WHERE SID = @SurveySID AND State <> 2
SELECT @SurveyDescription = ISNULL(Description, '''') FROM BvSurvey WHERE SID = @SurveySID AND State <> 2
Set @SurveyDescription = REPLACE(@SurveyDescription,'''','''''') --escape single apostrophe

SET @SurveyQreName = @SurveyDescription + ' (' + @SurveyQreName + ')'

DECLARE @PersonsStatement NVARCHAR (1000)
DECLARE @PersonsFilter NVARCHAR (4000)
DECLARE @PersonsGroup NVARCHAR (255)
IF @PersonsSIDs IS NULL OR @PersonsSIDs = '' BEGIN
 SET @PersonsStatement = ' ''ALL_PERSONS'' '
 SET @PersonsFilter = ''
 SET @PersonsGroup = ''
 SET @PersonsSIDs = ''
END
ELSE BEGIN
 SET @PersonsStatement = 
  ' IsNull((SELECT Name FROM BvPerson WHERE SID = 
  BvInterview.LastCallPersonSID), ''NO_CALLS'') '
 SET @PersonsFilter = 
  ' AND BvInterview.LastCallPersonSID in (' +
  @PersonsSIDs + ') '
 SET @PersonsGroup = ', BvInterview.LastCallPersonSID '
END

DECLARE @ITSFilter NVARCHAR (2000)
IF @ITSIDs = ''
 SET @ITSFilter = ''
ELSE
 SET @ITSFilter = ' AND bvstate.stateid IN (' + @ITSIDs + ') '

DECLARE @Query NVARCHAR (4000)
SET @Query=
 'SELECT
  bvstate.stateid ''StateID'',
  bvstate.name ''StateName'',
  count( BvInterview.transientstate ) ''Count'',
  ''' + @SurveyQreName + ''' ''SurveyName'',
  (SELECT count(*) 
   FROM BvInterview 
   WHERE (SurveySID = ' + @StrSurveySID + ') ' +
   ') ''SampleSize'',
  0 ''Calls'',
   ' + @PersonsStatement + ' ''Person''
 FROM bvstate LEFT JOIN BvInterview 
 ON (bvstate.stateid = BvInterview.transientstate) 
 AND (SurveySID = ' + @StrSurveySID + ') ' +
 'LEFT JOIN BvSurvey ON
 bvsurvey.SID = ' + @StrSurveySID + '
 WHERE bvstate.StateGroupID = bvsurvey.StateGroupID 
  ' + @PersonsFilter + ' 
  ' + @ITSFilter + ' 
 GROUP BY bvstate.stateid, bvstate.name ' + @PersonsGroup + ' 
 ORDER BY BvState.StateID'
/*print @Query*/
exec sp_executesql @Query
GO
PRINT N'Update complete.';


GO
