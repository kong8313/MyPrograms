GO
PRINT N'Dropping [dbo].[BvAppointment].[IX_app_SurveySID_InterviewSID_State]...';


GO
DROP INDEX [IX_app_SurveySID_InterviewSID_State]
    ON [dbo].[BvAppointment];


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_SurveySID_InterviewSID_State]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_SurveySID_InterviewSID_State]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [InterviewSID] ASC);


GO
PRINT N'Creating [dbo].[BvSampleStatusSummary].[IX_BvSampleStatusSummary_ITS]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSampleStatusSummary_ITS]
    ON [dbo].[BvSampleStatusSummary]([ITS] ASC);


GO
PRINT N'Creating [dbo].[GetMinSurveyAppTime]...';


GO
CREATE FUNCTION [dbo].[GetMinSurveyAppTime]
(
	@SurveySID INT
)
RETURNS TABLE AS RETURN
( 
	SELECT MIN(Time) as minTime FROM BvAppointment WHERE @SurveySID = SurveySID AND State=1
)
GO
PRINT N'Creating [dbo].[GetSurveyAlertAppointments]...';


GO
CREATE FUNCTION GetSurveyAlertAppointments
	(
		@SurveySID INT,
		@Top INT,
		@Amber INT,
		@Red INT,
		@Now DATETIME
	)
	RETURNS TABLE AS RETURN
	(
		WITH a as
		(
			SELECT *, 2 as AlertStatus FROM BvAppointment a
			WHERE a.SurveySID = @SurveySID AND  a.State = 1 /*with call*/ AND a.Time < DATEADD( second, -@Red, @NOW )
			UNION ALL 
			SELECT *, 1 as AlertStatus FROM BvAppointment a
			WHERE a.SurveySID = @SurveySID AND  a.State = 1 /*with call*/ AND a.Time BETWEEN  @Now AND DATEADD( second, -@Amber, @NOW )
		)
		SELECT TOP(100) * FROM a ORDER BY a.Time
		

	)
GO
PRINT N'Altering [dbo].[BvSpAlert_RecalculateAll]...';


GO
ALTER PROCEDURE [dbo].[BvSpAlert_RecalculateAll]
   @Now DATETIME
AS 

    CREATE TABLE #tempTable(SurveySID int NOT NULL,
              StrikeRate int NOT NULL DEFAULT(0),
              CountCalls int NOT NULL DEFAULT(0),
              AvgDuration float NOT NULL DEFAULT(0))


    DECLARE @needTime DATETIME;
    SET @needTime = DATEADD(minute, -15, @Now);


    INSERT INTO #tempTable
    SELECT BvSurvey.SID, 
            4*ISNULL(sum(case when h.ITS = 13 then 1 else 0 end), 0), 4*count(h.SurveyId), ISNULL(avg(Duration), 0)
    FROM BvSurvey 
    left join BvHistory h on h.FiredTime >= @needTime AND
                          h.SurveyId = BvSurvey.SID AND
                          h.RoleID = 2
	WHERE State <> 2
    group by SID


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
	SELECT sss.SurveySID, Cnt, ases.AlertStatus INTO #SampleStatusSummary FROM  BvSampleStatusSummary sss
	LEFT JOIN AlertStatuses as ases ON sss.SurveySID = ases.SurveySID
	WHERE sss.ITS = 16
    
    
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
            BvAggregateSurveyAlertStatus.StrikeRate = tt.StrikeRate,
            BvAggregateSurveyAlertStatus.CountCalls = tt.CountCalls,
            BvAggregateSurveyAlertStatus.AvgDuration = tt.AvgDuration,
            
            AlertStatusOfInterviewersLoggedCount = ilg.val,
            AlertStatusOfNextAppointmentTime = nat.val,
            AlertStatusOfTotalSampleSize = tss.val,
            AlertStatusOfScheduledCallsCount = scc.val,
            AlertStatusOfSuspendedCallsCount = succ.val,
            AlertStatusOfMinutesSpentWorkingOnSurvey = mswos.val,
            AlertStatusOfAssignedInterviewersCount = aic.val,
            AlertStatusOfStrikeRate = sr.val,
            AlertStatusOfCountCalls = cc.val,
            MaxStatusOfITSAlerts = BvSampleStatusSummary.AlertStatus
        FROM BvAggregateSurveyAlertStatus
        
        INNER JOIN #AlertStatuses BvSampleStatusSummary ON ( BvSampleStatusSummary.SurveySID = BvAggregateSurveyAlertStatus.SID )
                                              
        INNER JOIN #tempTable tt ON tt.SurveySID = BvAggregateSurveyAlertStatus.SID 
            
        INNER JOIN BvAggregateSurvey WITH(ROWLOCK) 
            ON (tt.SurveySID=BvAggregateSurvey.SID)
            
        LEFT JOIN (SELECT SurveySID, COUNT(*) as cnt
                   FROM BvTasks
                   WHERE SurveySID > 0
                   GROUP BY SurveySID) logs ON (tt.SurveySID = logs.SurveySID)
                   
        LEFT JOIN (SELECT COUNT(*) cnt, BvPersonrel.ObjectSid SurveySID
				   FROM BvPersonrel WHERE BvPersonrel.Type = 2
				   GROUP BY BvPersonrel.ObjectSid) AS AssignedInterviewers ON AssignedInterviewers.SurveySID = BvAggregateSurveyAlertStatus.SID
                   
        OUTER APPLY GetMinSurveyAppTime( BvAggregateSurveyAlertStatus.SID ) as Appointment
                   
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT(ISNULL(logs.cnt, 0), @AmberOfInterviewersLoggedCount, @RedOfInterviewersLoggedCount ) as ilg
        CROSS APPLY dbo.udf_AlertStatus_TAB_DATETIME(
          DATEADD(millisecond, 
                  -DATEPART(millisecond, Appointment.minTime),
                  Appointment.minTime), 
          @Now, 
          @AmberOfNextAppointmentTime, 
          @RedOfNextAppointmentTime ) as nat
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT(BvSampleStatusSummary.Cnt, @AmberOfTotalSampleSize, @RedOfTotalSampleSize ) as tss
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.ScheduledCallsCount, @AmberOfScheduledCallsCount, @RedOfScheduledCallsCount ) as scc
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.SuspendedCallsCount-BvAggregateSurvey.ScheduledCallsCount, @AmberOfSuspendedCallsCount, @RedOfSuspendedCallsCount ) as succ
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( BvAggregateSurvey.MinutesSpentWorkingOnSurvey, @AmberOfMinutesSpentWorkingOnSurvey, @RedOfMinutesSpentWorkingOnSurvey ) as mswos
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( AssignedInterviewers.cnt, @AmberOfAssignedInterviewersCount, @RedOfAssignedInterviewersCount ) as aic
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.StrikeRate, @AmberOfStrikeRate, @RedOfStrikeRate ) as sr
        CROSS APPLY dbo.udf_AlertStatus_TAB_INT( tt.CountCalls, @AmberOfCountCalls, @RedOfCountCalls ) as cc	
RETURN (0)
GO
PRINT N'Altering [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
ALTER PROCEDURE BvSpAlert_RecalculateAppointment
	@AppointmentAlert_ShortInterval INT,
	@AppointmentAlert_LongInterval INT,
	@defaultTimeZone INT
AS
   DECLARE @Now DATETIME = GETUTCDATE()

   DECLARE @Red INT
   DECLARE @Amber INT 

   SELECT @Red = Red, @Amber = Amber
   FROM BvThresholds
   WHERE ObjectSID = 0 AND
         ThresholdsTypeID = 15

   DECLARE @StartDate DATETIME

   SET @StartDate = DATEADD(millisecond, -DATEPART(millisecond, @Now), @Now)
   SET @StartDate = DATEADD(second, -DATEPART(second, @StartDate), @StartDate)
   SET @StartDate = DATEADD(minute, -DATEPART(minute, @StartDate), @StartDate)
   SET @StartDate = DATEADD(Hour, -DATEPART(hour, @StartDate), @StartDate)

   DECLARE @ShortIntervalStart DATETIME = @Now
   DECLARE @ShortIntervalFinish DATETIME = DateAdd(second, @AppointmentAlert_ShortInterval, @Now)

   DECLARE @LongIntervalStart DATETIME = (CASE WHEN @AppointmentAlert_LongInterval >= 0
                                                THEN @Now
                                                ELSE @StartDate
                                                END)
   DECLARE @LongIntervalFinish DATETIME = (CASE WHEN @AppointmentAlert_LongInterval >= 0
                                                THEN DateAdd(hour, @AppointmentAlert_LongInterval, @Now)
                                                ELSE DateAdd(day, -@AppointmentAlert_LongInterval, @StartDate)
                                                END)
   ----------------------BvAppointmentCounters----------------------
   UPDATE BvAppointmentCounters
   SET CountForShortInterval = (SELECT COUNT(*)
                                FROM BvAppointment a
                                WHERE a.State = 1 AND/*with call*/
                                      a.SurveySID = BvAppointmentCounters.SurveySID AND
                                      a.Time BETWEEN @ShortIntervalStart AND @ShortIntervalFinish),
       CountForLongInterval = (SELECT COUNT(*)
                               FROM BvAppointment a
                               WHERE a.State = 1 AND/*with call*/
                                     a.SurveySID = BvAppointmentCounters.SurveySID AND
                                     a.Time between @LongIntervalStart AND @LongIntervalFinish )
   ----------------------BvAppointmentsAlertStatus----------------------
   TRUNCATE TABLE BvAppointmentsAlertStatus
  
   INSERT INTO BvAppointmentsAlertStatus( 
     [ID],
     [SurveySID],
     [SurveyName],
     [ProjectID],
     [InterviewID],
     [AppointmentTime],
     [TZID],
     [Resource],
     [Contact],
     [AlertStatus],
     [CallID])
   SELECT a.ID,
          a.SurveySID,
          s.Description,
          s.Name,
          a.InterviewSID,
          a.Time,
          ISNULL(a.TZID, @defaultTimeZone),
          NULL,
          a.ContactName,
          a.AlertStatus,
          0
   FROM BvSurvey s 
   CROSS APPLY GetSurveyAlertAppointments( s.SID, 100, @Amber, @Red, @Now ) a
   WHERE s.State = 1 
   
   UPDATE BvAppointmentsAlertStatus
		SET [Resource] = pag.Name,
			[CallID] = ss.ID
		FROM BvSvySchedule ss
		LEFT JOIN BvViewPersonAndGroup pag ON(ss.ExplicitType = 2 AND
                                         pag.SID = ss.ExplicitSID)
        WHERE  BvAppointmentsAlertStatus.SurveySID = ss.SurveySID AND
                                  BvAppointmentsAlertStatus.[InterviewID] = ss.InterviewID
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
