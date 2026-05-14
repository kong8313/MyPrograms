CREATE PROCEDURE [dbo].[BvSpAlert_RecalculateAll]
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
				case when h.FiredTime IS NOT NULL then 1 else 0 end as call1h,
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
			ISNULL(sum(call1h), 0), 
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
	INSERT INTO #AlertStatuses SELECT sss.SurveySID, SUM(sss.Cnt) AS Cnt, MAX(ases.AlertStatus) AS AlertStatus FROM  BvSampleStatusSummary sss
	LEFT JOIN AlertStatuses as ases ON sss.SurveySID = ases.SurveySID
	WHERE sss.ITS = 16
    GROUP BY sss.SurveySID

    
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
