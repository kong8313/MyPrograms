PRINT N'Altering [dbo].[BvBreakType]...';


GO
ALTER TABLE [dbo].[BvBreakType]
    ADD [YellowThreshold] INT NULL,
        [RedThreshold]     INT NULL;


GO
PRINT N'Refreshing [dbo].[RestView_BreakHistory]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[RestView_BreakHistory]';


GO
PRINT N'Creating [dbo].[udf_AlertDurationStatus]...';


GO
ALTER FUNCTION [dbo].[udf_AlertStatus_TAB_INT]
(
    @Value INT,
    @Amber INT,
    @Red INT,
    @Type TINYINT
)
returns table
as return(
    SELECT ( 
        CASE
            WHEN @Type IS NULL /*Old*/ THEN
                CASE 
                    WHEN ((@Amber IS NULL) OR (@Red IS NULL)) THEN 0
                    WHEN ((@Red = @Amber) AND (@Value = @Red)) THEN 2
                    WHEN (@Red > @Amber) THEN 
                        CASE 
                            WHEN (@Value >= @Red) THEN 2
                            WHEN (@Value >= @Amber) THEN 1
                            ELSE 0
                        END
                    WHEN (@Red < @Amber) THEN  
                        CASE 
                            WHEN (@Value <= @RED) THEN 2
                            WHEN (@Value <= @Amber) THEN 1
                            ELSE 0
                        END
                    ELSE 0
                END
            WHEN @Red IS NULL AND @Amber IS NULL THEN NULL            
            WHEN @Type = 1 /*Ascending*/ THEN
                CASE 
                    WHEN @Red IS NOT NULL AND @Red < @Value THEN 2
                    WHEN @Amber IS NOT NULL AND @Amber < @Value THEN 1
                    ELSE 0
                END
            WHEN @Type = 2 /*Descending*/ THEN
                CASE 
                    WHEN @Red IS NOT NULL AND @Red > @Value THEN 2
                    WHEN @Amber IS NOT NULL AND @Amber > @Value THEN 1
                    ELSE 0
                END
        END) AS val
)


GO
PRINT N'Altering [dbo].[BvSpGetListSurveyTasks]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetListSurveyTasks]
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
   DECLARE @AmberOfBreakDuration INT
   DECLARE @RedOfBreakDuration INT

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

   SELECT @AmberOfBreakDuration = Amber, @RedOfBreakDuration = Red
   FROM BvThresholds 
   WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 20/*Break duration alert*/
   
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
		  tsk.BreakDurationAlert,
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
		  tsk.BreakTypeName,
		  tsk.JsonContext,
		  tsk.InterviewScreenRecording,
		  tsk.IsLiveMonitoringEnabled
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
		  (CASE WHEN t.StatusLogout != 6
                 THEN 0 
                 ELSE ISNULL( bdaNew.val, bda.val) 
          END) BreakDurationAlert,
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
		  bt.Name as BreakTypeName,
		  t.JsonContext,
		  s.InterviewScreenRecording,
		  s.IsLiveMonitoringEnabled
   FROM BvTasks t
   LEFT JOIN BvFnSurvey_GetByTransferBatch( @surveysBatchID ) s ON (t.SurveySID = s.SID)
   LEFT JOIN BvUserSurveyPermission up ON t.SurveySID = up.SurveySID AND up.UserName = @UserName
   LEFT JOIN BvBreakType bt on bt.Id = t.BreakTypeId
   INNER JOIN BvDialType dt ON t.DialTypeId = dt.Id
   INNER JOIN dbo.BvFnPerson_GetByTransferBatch(@interviewersBatchID) pta ON pta.Id = t.PersonSID
   OUTER APPLY dbo.GetLastTimeBreak(t.PersonSID) lb
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, LastKeepAliveTime, @currTime), @AmberOfLastKeepAliveTime, @RedOfLastKeepAliveTime, NULL) as lkat
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeStateChanged, @currTime), @AmberOfLastSubmission, @RedOfLastSubmission, NULL) as tsc
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, t.StartTime, @currTime), @AmberOfNoActivity, @RedOfNoActivity, NULL) as naa
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, TimeCallDelivered, @currTime), @AmberOfInterviewDuration, @RedOfInterviewDuration, NULL) as ida
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, lb.StartTime, @currTime), @AmberOfBreakDuration, @RedOfBreakDuration, NULL) as bda
   CROSS APPLY dbo.udf_AlertStatus_TAB_INT(DATEDIFF(second, lb.StartTime, @currTime), bt.YellowThreshold, bt.RedThreshold, 1) as bdaNew
   WHERE (s.SID IS NOT NULL AND up.SurveySID IS NOT NULL) OR t.SurveySID = 0) as tsk
   INNER JOIN BvFnPerson_Get(@CallCenterID) p ON (tsk.PersonSID = p.SID)
   LEFT JOIN BvTimezone tz ON ((CASE WHEN TzID = 0 THEN @TimeZoneID ELSE TzId END) = tz.ID)
   LEFT JOIN BvPersonMonitoring pm ON (pm.PersonSID = tsk.PersonSID)


GO
PRINT N'Altering [dbo].[BvSpAlert_RecalculateAll]...';


GO
ALTER PROCEDURE [dbo].[BvSpAlert_RecalculateAll]
   @Now DATETIME
AS 

    CREATE TABLE #tempTable(SurveySID int NOT NULL,
              StrikeRate15min int NOT NULL DEFAULT(0),
              CountCalls15min int NOT NULL DEFAULT(0),
              AvgDuration15min float NOT NULL DEFAULT(0),
			  StrikeRate1h int NOT NULL DEFAULT(0),
              CountCalls1h int NOT NULL DEFAULT(0),
              AvgDuration1h float NOT NULL DEFAULT(0))


    DECLARE @needTime15min DATETIME;
    SET @needTime15min = DATEADD(minute, -15, @Now);
    DECLARE @needTime1h DATETIME;
    SET @needTime1h = DATEADD(hour, -1, @Now);

	;WITH historyInfo AS (
		SELECT	s.SID, 
				case when h.ITS = 13 AND h.FiredTime >= @needTime15min then 1 else 0 end as completeCall15min,
				case when h.FiredTime >= @needTime15min then 1 else 0 end as call15min,
				case when h.FiredTime >= @needTime15min then h.Duration else 0 end as duration15min,
				case when h.ITS = 13 then 1 else 0 end as completeCall1h,
				h.Duration
		FROM BvSurvey s
		left join BvHistory h on h.FiredTime >= @needTime1h AND
							  h.SurveyId = s.SID AND
							  h.RoleID = 2 
		WHERE State <> 2
	)
    INSERT INTO #tempTable
    SELECT  SID, 
            4 * ISNULL(sum(completeCall15min), 0), 
			4 * ISNULL(sum(call15min), 0), 
			CASE WHEN sum(call15min) > 0 THEN sum(duration15min) / sum(call15min) ELSE 0 END,
            ISNULL(sum(completeCall1h), 0 ), 
			count(SID), 
			ISNULL(avg(duration), 0)
    FROM historyInfo
	GROUP BY SID


    --2. InterviewersLoggedCount thresholds
    DECLARE @AmberOfInterviewersLoggedCount INT
    DECLARE @RedOfInterviewersLoggedCount INT
    SELECT @AmberOfInterviewersLoggedCount = Amber, @RedOfInterviewersLoggedCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 2/*SurveyActivityView.InterviewersLoggedCount alert*/


    --3. NextAppointmentTime thresholds
    DECLARE @AmberOfNextAppointmentTime INT
    DECLARE @RedOfNextAppointmentTime INT
    SELECT @AmberOfNextAppointmentTime = Amber, @RedOfNextAppointmentTime = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 3/*SurveyActivityView.NextAppointmentTime alert*/


    --4. NextAppointmentTime thresholds
    DECLARE @AmberOfTotalSampleSize INT
    DECLARE @RedOfTotalSampleSize INT
    SELECT @AmberOfTotalSampleSize = Amber, @RedOfTotalSampleSize = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 4/*SurveyActivityView.TotalSampleSize alert*/


    --6. Scheduled thresholds
    DECLARE @AmberOfScheduledCallsCount INT
    DECLARE @RedOfScheduledCallsCount INT
    SELECT @AmberOfScheduledCallsCount = Amber, @RedOfScheduledCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 6/*SurveyActivityView.ScheduledCallsCount alert*/


    --7. SuspendedCallsCount thresholds
    DECLARE @AmberOfSuspendedCallsCount INT
    DECLARE @RedOfSuspendedCallsCount INT
    SELECT @AmberOfSuspendedCallsCount = Amber, @RedOfSuspendedCallsCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 7/*SurveyActivityView.SuspendedCallsCount alert*/


    --8. MinutesSpentWorkingOnSurvey thresholds
    DECLARE @AmberOfMinutesSpentWorkingOnSurvey INT
    DECLARE @RedOfMinutesSpentWorkingOnSurvey INT
    SELECT @AmberOfMinutesSpentWorkingOnSurvey = Amber, @RedOfMinutesSpentWorkingOnSurvey = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 8/*SurveyActivityView.SuspendedCallsCount alert*/


    --9. AssignedInterviewersCount thresholds
    DECLARE @AmberOfAssignedInterviewersCount INT
    DECLARE @RedOfAssignedInterviewersCount INT
    SELECT @AmberOfAssignedInterviewersCount = Amber, @RedOfAssignedInterviewersCount = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 9/*SurveyActivityView.AssignedInterviewersCount alert*/


    --10. StrikeRate thresholds
    DECLARE @AmberOfStrikeRate INT
    DECLARE @RedOfStrikeRate INT
    SELECT @AmberOfStrikeRate = Amber, @RedOfStrikeRate = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 10/*SurveyActivityView.StrikeRate alert*/


    --11. CountCalls thresholds
    DECLARE @AmberOfCountCalls INT
    DECLARE @RedOfCountCalls INT
    SELECT @AmberOfCountCalls = Amber, @RedOfCountCalls = Red
        FROM BvThresholds 
        WHERE ObjectSID = 0 /*Default value*/ AND ThresholdsTypeID = 11/*SurveyActivityView.CountCalls alert*/
    
    CREATE TABLE #AlertStatuses
    (
		SurveySID INT NOT NULL PRIMARY KEY,
		Cnt INT NOT NULL,
		AlertStatus INT NOT NULL
    )
    
    ;WITH AlertStatuses AS
    (
		SELECT SurveySID, MAX( AlertStatus ) as AlertStatus FROM BvSampleStatusSummary GROUP BY SurveySID
	)
	INSERT INTO #AlertStatuses SELECT sss.SurveySID, Cnt, ases.AlertStatus FROM  BvSampleStatusSummary sss
	LEFT JOIN AlertStatuses as ases ON sss.SurveySID = ases.SurveySID
	WHERE sss.ITS = 16
    
	CREATE TABLE #SpendTime(
		SurveyId INT NOT NULL PRIMARY KEY,
		MinutesSpentWorkingOnSurveyInDay INT NOT NULL
	)

	INSERT INTO #SpendTime(SurveyId, MinutesSpentWorkingOnSurveyInDay ) 
		SELECT ip.SurveyId, ISNULL( SUM(ip.InterviewingTime), 0 ) 
			FROM BvInterviewerPerformance ip GROUP BY SurveyId
    
    SET @Now = DATEADD(millisecond, -DATEPART(millisecond, @Now), @Now)
	
    UPDATE BvAggregateSurveyAlertStatus
        SET BvAggregateSurveyAlertStatus.InterviewersLoggedCount = ISNULL(logs.cnt, 0),
            BvAggregateSurveyAlertStatus.InterviewersLoggedCountPrev = BvAggregateSurveyAlertStatus.InterviewersLoggedCount,
            BvAggregateSurveyAlertStatus.NextAppointmentTime = Appointment.minTime,
            BvAggregateSurveyAlertStatus.TotalSampleSize = BvSampleStatusSummary.Cnt,
            BvAggregateSurveyAlertStatus.ScheduledCallsCount = BvAggregateSurvey.ScheduledCallsCount,
            BvAggregateSurveyAlertStatus.ScheduledCallsCountPrev = BvAggregateSurveyAlertStatus.ScheduledCallsCount,
            BvAggregateSurveyAlertStatus.SuspendedCallsCount = BvAggregateSurvey.SuspendedCallsCount-BvAggregateSurvey.ScheduledCallsCount,
            BvAggregateSurveyAlertStatus.SuspendedCallsCountPrev = BvAggregateSurveyAlertStatus.SuspendedCallsCount,
            BvAggregateSurveyAlertStatus.MinutesSpentWorkingOnSurvey = BvAggregateSurvey.MinutesSpentWorkingOnSurvey,
            BvAggregateSurveyAlertStatus.AssignedInterviewersCount = ISNULL(AssignedInterviewers.cnt, 0),
            BvAggregateSurveyAlertStatus.StrikeRate = tt.StrikeRate15min,
            BvAggregateSurveyAlertStatus.CountCalls = tt.CountCalls15min,
            BvAggregateSurveyAlertStatus.AvgDuration = tt.AvgDuration15min,
            
            AlertStatusOfInterviewersLoggedCount = ilg.val,
            AlertStatusOfNextAppointmentTime = nat.val,
            AlertStatusOfTotalSampleSize = tss.val,
            AlertStatusOfScheduledCallsCount = scc.val,
            AlertStatusOfSuspendedCallsCount = succ.val,
            AlertStatusOfMinutesSpentWorkingOnSurvey = mswos.val,
            AlertStatusOfAssignedInterviewersCount = aic.val,
            AlertStatusOfStrikeRate = sr15min.val,
            AlertStatusOfCountCalls = cc15min.val,
            MaxStatusOfITSAlerts = BvSampleStatusSummary.AlertStatus,

            BvAggregateSurveyAlertStatus.StrikeRate1h = tt.StrikeRate1h,
            BvAggregateSurveyAlertStatus.CountCalls1h = tt.CountCalls1h,
            BvAggregateSurveyAlertStatus.AvgDuration1h = tt.AvgDuration1h,
			BvAggregateSurveyAlertStatus.MinutesSpentWorkingOnSurveyInDay = ISNULL( ss.MinutesSpentWorkingOnSurveyInDay, 0 ),
            BvAggregateSurveyAlertStatus.AlertStatusOfStrikeRate1h = sr1h.val,
            BvAggregateSurveyAlertStatus.AlertStatusOfCountCalls1h = cc1h.val

        FROM BvAggregateSurveyAlertStatus
        
        INNER JOIN #AlertStatuses BvSampleStatusSummary ON ( BvSampleStatusSummary.SurveySID = BvAggregateSurveyAlertStatus.SID )
                                              
        INNER JOIN #tempTable tt ON tt.SurveySID = BvAggregateSurveyAlertStatus.SID 
            
        INNER JOIN BvAggregateSurvey WITH(ROWLOCK) 
            ON (tt.SurveySID=BvAggregateSurvey.SID)
		LEFT JOIN #SpendTime ss ON ss.SurveyId = tt.SurveySID
            
        LEFT JOIN (SELECT SurveySID, COUNT(*) as cnt
                   FROM BvTasks
                   WHERE SurveySID > 0
                   GROUP BY SurveySID) logs ON (tt.SurveySID = logs.SurveySID)
                   
        LEFT JOIN (SELECT COUNT(*) cnt, BvPersonrel.ObjectSid SurveySID
				   FROM BvPersonrel WHERE BvPersonrel.Type = 2
				   GROUP BY BvPersonrel.ObjectSid) AS AssignedInterviewers ON AssignedInterviewers.SurveySID = BvAggregateSurveyAlertStatus.SID
                   
        OUTER APPLY GetMinSurveyAppTime( BvAggregateSurveyAlertStatus.SID ) as Appointment
                   
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT(ISNULL(logs.cnt, 0), @AmberOfInterviewersLoggedCount, @RedOfInterviewersLoggedCount, NULL) as ilg
        CROSS APPLY dbo.udf_AlertStatus_TAB_DATETIME(
          DATEADD(millisecond, 
                  -DATEPART(millisecond, Appointment.minTime),
                  Appointment.minTime), 
          @Now, 
          @AmberOfNextAppointmentTime, 
          @RedOfNextAppointmentTime ) as nat
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT(BvSampleStatusSummary.Cnt, @AmberOfTotalSampleSize, @RedOfTotalSampleSize, NULL) as tss
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.ScheduledCallsCount, @AmberOfScheduledCallsCount, @RedOfScheduledCallsCount, NULL) as scc
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.SuspendedCallsCount-BvAggregateSurvey.ScheduledCallsCount, @AmberOfSuspendedCallsCount, @RedOfSuspendedCallsCount, NULL) as succ
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.MinutesSpentWorkingOnSurvey, @AmberOfMinutesSpentWorkingOnSurvey, @RedOfMinutesSpentWorkingOnSurvey, NULL) as mswos
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( AssignedInterviewers.cnt, @AmberOfAssignedInterviewersCount, @RedOfAssignedInterviewersCount, NULL) as aic
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.StrikeRate15min, @AmberOfStrikeRate, @RedOfStrikeRate, NULL) as sr15min
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.CountCalls15min, @AmberOfCountCalls, @RedOfCountCalls, NULL) as cc15min	
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.StrikeRate1h, @AmberOfStrikeRate, @RedOfStrikeRate, NULL) as sr1h
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.CountCalls1h, @AmberOfCountCalls, @RedOfCountCalls, NULL) as cc1h
RETURN (0)


GO
PRINT N'Refreshing [dbo].[BvSpGetInterviewerBreaks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetInterviewerBreaks]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewerProductivityReport]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewerProductivityReport]';


GO
PRINT N'Update complete.';


GO
