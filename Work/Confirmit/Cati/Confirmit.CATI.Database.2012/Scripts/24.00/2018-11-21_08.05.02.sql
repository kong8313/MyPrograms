GO
PRINT N'Altering [dbo].[BvSpGetListSurveyTasks]...';


GO
ALTER PROCEDURE BvSpGetListSurveyTasks
   @surveysBatchID int,
   @interviewersBatchID int,   
   @TimeZoneID INT,
   @CallCenterID INT,
   @UserName NVARCHAR(MAX)

AS
   DECLARE @currTime DATETIME
   EXEC @currTime = GetUtcNow
   DECLARE @AmberOfLastSubmission INT
   DECLARE @RedOfLastSubmission INT
   DECLARE @AmberOfLastKeepAliveTime INT
   DECLARE @RedOfLastKeepAliveTime INT
   DECLARE @AmberOfNoActivity INT
   DECLARE @RedOfNoActivity INT
   DECLARE @AmberOfInterviewDuration INT
   DECLARE @RedOfInterviewDuration INT

   SELECT @AmberOfLastSubmission = Amber, @RedOfLastSubmission = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 1/*Task alert*/

   SELECT @AmberOfLastKeepAliveTime = Amber, @RedOfLastKeepAliveTime = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 16/*Last keep alive alert*/
   
   SELECT @AmberOfNoActivity = Amber, @RedOfNoActivity = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 18/*No activity alert*/

   SELECT @AmberOfInterviewDuration = Amber, @RedOfInterviewDuration = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 19/*Interview duration alert*/
   
   SELECT tsk.InterviewID, 
          tsk.PersonSID, 
          p.Name as InterviewerName, 
          tsk.SurveySID, 
          tsk.ProjectID, 
          tsk.SurveyName,
          tsk.TimeCallDelivered, 
          tsk.State, 
          tsk.SecondsSinceLastSubmission, 
          tsk.LastSubmissionAlert, 
          tsk.LastKeepAliveTime,
          tsk.LastKeepAliveTimeAlert,
          tsk.EndOfLastActivityAlert,
		  tsk.InterviewDurationAlert,
          tsk.InterviewState,
          tsk.LoggedInToDialerState,
          tsk.TzID, 
          tsk.DiallingMode, -- if no survey assigned to task - use manual dialing mode
          tsk.CallOutcome, 
          tsk.StatusLogout,
          tsk.ProblemId,
          tz.Bias, 
          pm.supervisorName,
          pm.MonitoringSessionID,
          tsk.StationId,
		  tsk.DialType,
		  tsk.OpenEndReviewInSeconds,
		  tsk.DialerId,
		  p.Type,
		  tsk.CallType,
		  tsk.LinkedChain,
		  tsk.CallConnectionState,
		  tsk.BreakTypeName
   FROM
   (SELECT t.InterviewID, 
          t.PersonSID, 
          t.SurveySID, 
          ISNULL(s.Name, '') as ProjectID, 
          ISNULL(s.Description, '') as SurveyName,
          (CASE WHEN t.StatusLogout != 6 /*BREAK*/ THEN t.TimeCallDelivered 
                ELSE lb.StartTime
           END) as TimeCallDelivered, 
          t.State, 
          (CASE WHEN t.InterviewID = 0 THEN NULL ELSE ISNULL(DATEDIFF(second, TimeStateChanged, @currTime), 0) END) as SecondsSinceLastSubmission, 
          (CASE WHEN InterviewID > 0 
				THEN tsc.val
				ELSE 0
			END) LastSubmissionAlert, 
          t.LastKeepAliveTime,
          (CASE WHEN LastKeepAliveTime IS NULL 
				THEN 2 
				ELSE lkat.val
			END) LastKeepAliveTimeAlert,
          (CASE WHEN TimeCallDelivered IS NULL AND t.StartTime IS NOT NULL
                 THEN  naa.val 
                 ELSE 0 
          END) EndOfLastActivityAlert,
		  (CASE WHEN TimeCallDelivered IS NULL
                 THEN 0 
                 ELSE ida.val  
          END) InterviewDurationAlert,
          t.InterviewState,
          t.LoggedInToDialerState,
          t.TzID, 
          t.DiallingMode, 
          t.CallOutcome, 
          t.StatusLogout,
          t.ProblemId,
          t.StationId,
		  dt.Name as DialType,
		  CASE WHEN t.OpenEndReviewStartTime IS NOT NULL THEN DATEDIFF(ss, t.OpenEndReviewStartTime, GETUTCDATE()) ELSE NULL END AS OpenEndReviewInSeconds,
		  t.DialerId,
		  t.CallType,
		  t.LinkedChain,
		  t.CallConnectionState,
		  bt.Name as BreakTypeName
   FROM BvTasks t
   LEFT JOIN BvFnSurvey_GetByTransferBatch( @surveysBatchID ) s ON (t.SurveySID = s.SID)
   LEFT JOIN BvUserSurveyPermission up ON t.SurveySID = up.SurveySID AND up.UserName = @UserName
   LEFT JOIN BvBreakType bt on bt.Id = t.BreakTypeId
   INNER JOIN BvDialType dt ON t.DialTypeId = dt.Id
   INNER JOIN dbo.BvFnPerson_GetByTransferBatch(@interviewersBatchID) pta ON pta.Id = t.PersonSID
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, @currTime), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime) as lkat
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, @currTime), @AmberOfLastSubmission, @RedOfLastSubmission ) as tsc
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, t.StartTime, @currTime), @AmberOfNoActivity, @RedOfNoActivity ) as naa
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeCallDelivered, @currTime), @AmberOfInterviewDuration, @RedOfInterviewDuration ) as ida
   WHERE (s.SID IS NOT NULL AND up.SurveySID IS NOT NULL) OR t.SurveySID = 0) as tsk
   INNER JOIN BvFnPerson_Get(@CallCenterID) p ON (tsk.PersonSID = p.SID)
   INNER JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)
GO
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
			0 AS OnBreakTimeUnpaid,
			0 AS OnBreakTimePaid,
		    0 AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
		    0 AS Completes,
		    0 AverageCompletedInterviewDuration,
			0 AS OpenEndReviewDuration
    return
 end

 DECLARE @DiallerName NVARCHAR(20)
 SET  @DiallerName = N'Dialer';
 
 SET @StartDateTime = ISNULL(@StartDateTime, '1753-01-01T00:00:00.000')
 SET @EndDateTime = ISNULL(@EndDateTime, '9999-12-31T23:59:59.997');

 CREATE TABLE #Persons(PersonSid int primary key, Name NVARCHAR (255), DurationPaid int, DurationUnpaid int)
 INSERT INTO #Persons
 SELECT p.SID AS PersonSid,
        p.Name AS Name,
		NULL,
		NULL
 FROM dbo.utilSplitNumbers( ISNULL(@PersonSids, ''), ',') s
 INNER JOIN BvPerson p ON p.SID = s.Item
 UNION 
 SELECT p.Sid AS PersonSid,
        p.Name AS Name,
		NULL,
		NULL
 FROM BvPerson p
 WHERE @PersonSids IS NULL
 UNION
 SELECT DialerSid AS PersonSid,
        @DiallerName AS Name,
		NULL,
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
    SELECT ISNULL(SUM(ISNULL(
	   	    	CASE WHEN CAST( '00:00:00' - (DATEADD ( SECOND, Duration, StartTime ) - @endShiftTime)  AS TIME )  < @diff
				THEN
					Duration 
				ELSE
				     DATEDIFF(SECOND,  CAST(StartTime AS TIME), CAST ( @EndShiftTime AS TIME) )
			END, Duration)
	), 0) Duration, 
	InterviewerId,
	bt.IsPaid as IsPaid
    FROM BvTimeBreaksHistory h
	LEFT JOIN #SurveyIdsList s
	ON h.SurveyId = s.SurveyId
	LEFT JOIN BvBreakType bt on bt.Id = h.BreakTypeId
    WHERE StartTime BETWEEN @StartDateTime AND @EndDateTime AND ( s.SurveyId IS NOT NULL )
	      AND (@StartShiftTime IS NULL OR CAST( StartTime - @StartShiftTime  AS TIME ) <= @diff)
    GROUP BY InterviewerId, IsPaid
 )
 update #persons
 set DurationPaid = (select TimeBreaksHistory.Duration
 from TimeBreaksHistory
 where #persons.PersonSid = TimeBreaksHistory.interviewerId and (TimeBreaksHistory.IsPaid = 1 OR TimeBreaksHistory.IsPaid IS NULL)),
 DurationUnpaid = (select TimeBreaksHistory.Duration
 from TimeBreaksHistory
 where #persons.PersonSid = TimeBreaksHistory.interviewerId and TimeBreaksHistory.IsPaid = 0)

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
  (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0) + ISNULL(p.DurationPaid, 0) + ISNULL(p.DurationUnpaid, 0)) AS LogOnTime,
  ISNULL(SUM(h.WaitingTime), 0) AS WaitingTime,
  ISNULL(p.DurationPaid, 0) AS OnBreakTimePaid,
  ISNULL(p.DurationUnpaid, 0) AS OnBreakTimeUnpaid,
  COUNT(h.InterviewId) AS DialingsCount, --there is can be dummy record where Hst_Path1 is null (it is used when user was logged in but didn't get any calls)
  COUNT(cil.CompletedIts) AS Completes,
  ISNULL(AVG(CASE WHEN cil.CompletedIts IS NOT NULL THEN h.Duration ELSE NULL END), 0) AS AverageCompletedInterviewDuration,
  ISNULL(SUM(h.OpenEndReviewDuration), 0) AS OpenEndReviewDuration
 FROM #Persons p
 LEFT JOIN FilteredHistory h ON p.PersonSid = h.PersonSid AND
        h.FiredTime >= @StartDateTime AND
        h.FiredTime <= @EndDateTime AND
        h.RoleID = 2 AND --we should not calced calls which were added during sample addition
        h.SurveyId IN (SELECT sil.SurveyId FROM #SurveyIdsList sil)
 LEFT JOIN #CompletedItsList cil ON cil.CompletedIts = h.ITS
	 LEFT JOIN #respids i on i.respid = h.InterviewId AND i.surveyid = h.SurveyId
	 WHERE i.respid IS NOT NULL OR @SurveyDataFilter IS NULL

 GROUP BY p.PersonSid, p.Name, p.DurationPaid, p.DurationUnpaid
 HAVING (@HideEmpty = 0 OR (ISNULL(SUM(ISNULL(h.Duration, h.ConfirmitDuration)), 0) + ISNULL(SUM(h.WaitingTime), 0)) > 0 OR p.PersonSid = 0)
GO
PRINT N'Altering [dbo].[BvSpInterviewerSessionsReport]...';


GO
ALTER PROCEDURE [dbo].[BvSpInterviewerSessionsReport]
    @personIds NVARCHAR(MAX),
	@SearchCondition NVARCHAR(MAX),
	@PageIndex INT,
	@PageSize INT,
	@OrderField NVARCHAR(64),
	@IsOrderASC BIT,
	@DatabaseName nvarchar(max),
	@CallCenterId int,
	@CompanyId int,
	@EventType int
AS
	IF @personIds IS NULL AND @PageIndex IS NULL AND @PageSize IS NULL
	BEGIN
		SELECT  '' AS PersonName,
				CAST(NULL AS DATETIME) AS StartTime,
				CAST(NULL AS DATETIME) AS FinishTime,
				0 AS Duration,
				0 as Event,
				'' as Note
		WHERE 1 = 0
		RETURN 0;
	END
	
	DECLARE @loginTotalCount int = 0

	IF @EventType != 0
		BEGIN
		CREATE TABLE #CatiPersonSessionHistory
		(
			PersonName nvarchar(max),
			StartTime datetime,
			FinishTime datetime,
			Duration int,
			Event int
		)

		INSERT INTO #CatiPersonSessionHistory
		EXEC @loginTotalCount = BvSpGetCatiPersonSessionHistory @personIds, @SearchCondition, @PageIndex, @PageSize, @OrderField, @IsOrderASC, @DatabaseName, @CallCenterId, @CompanyId
	END
	
	DECLARE @Query NVARCHAR(MAX) = ''
	DECLARE @loginQuery NVARCHAR(MAX) = 'SELECT PersonName, StartTime, FinishTime, Duration, Event, NULL as Note FROM #CatiPersonSessionHistory'
	DECLARE @breakQuery NVARCHAR(MAX)= '
	SELECT BvPerson.Name PersonName,
	       StartTime,
		   DATEADD(second, Duration, StartTime) as FinishTime,
		   Duration,
		   0 as Event,
		   bt.Name as Note
	FROM BvTimeBreaksHistory
	INNER JOIN dbo.utilSplitNumbers( ''' + ISNULL(@PersonIds, '') + ''', '','') s1 ON s1.Item = InterviewerId
	INNER JOIN BvPerson ON SID = InterviewerId
	LEFT JOIN bvBreakType bt on bt.Id = BvTimeBreaksHistory.BreakTypeId'

	SET @Query = 
        CASE
            WHEN @EventType = -1
                THEN @breakQuery + ' UNION ALL ' + @loginQuery
            WHEN @EventType = 0
                THEN @breakQuery
            WHEN @EventType = 1
                THEN @loginQuery
        END;
	
	DECLARE @TotalCount INT

    EXEC @TotalCount = BvSpGetObjectsPage @PageIndex, @PageSize, @OrderField, @IsOrderASC, @Query, @OrderField, @SearchCondition
	IF @loginTotalCount != 0
	BEGIN
		SET @TotalCount = @TotalCount + (@loginTotalCount - (SELECT COUNT(*) FROM #CatiPersonSessionHistory))
	END
    RETURN @TotalCount
RETURN 0
GO
PRINT N'Update complete.';


GO
