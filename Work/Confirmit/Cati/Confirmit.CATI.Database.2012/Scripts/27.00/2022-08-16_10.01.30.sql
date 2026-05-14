
GO
PRINT N'Dropping [dbo].[DF_BvSampleStatusSummary_AlertStatus]...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummary] DROP CONSTRAINT [DF_BvSampleStatusSummary_AlertStatus];


GO
PRINT N'Dropping [dbo].[DF_BvSampleStatusSummary_Cnt]...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummary] DROP CONSTRAINT [DF_BvSampleStatusSummary_Cnt];

GO
PRINT N'Dropping [dbo].[DF_BvSampleStatusSummary_IsCati]...';


GO
PRINT N'Starting rebuilding table [dbo].[BvSampleStatusSummary]...';



GO
PRINT N'Altering [dbo].[BvSampleStatusSummaryDelta]...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummaryDelta]
    ADD [IsCati] BIT CONSTRAINT [DF_BvSampleStatusSummaryDelta_IsCati] DEFAULT (0) NOT NULL;




GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvSampleStatusSummary] (
    [SurveySID]   INT NOT NULL,
    [ITS]         INT NOT NULL,
    [Cnt]         INT CONSTRAINT [DF_BvSampleStatusSummary_Cnt] DEFAULT (0) NOT NULL,
    [AlertStatus] INT CONSTRAINT [DF_BvSampleStatusSummary_AlertStatus] DEFAULT (0) NOT NULL,
    [IsCati]      BIT CONSTRAINT [DF_BvSampleStatusSummary_IsCati] DEFAULT (0) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_Pk_BvSampleStatusSummary1] PRIMARY KEY CLUSTERED ([SurveySID] ASC, [ITS] ASC, [IsCati] ASC)
);


DROP TABLE [dbo].[BvSampleStatusSummary];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvSampleStatusSummary]', N'BvSampleStatusSummary';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_Pk_BvSampleStatusSummary1]', N'Pk_BvSampleStatusSummary', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvSampleStatusSummary].[IX_BvSampleStatusSummary_ITS]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvSampleStatusSummary_ITS]
    ON [dbo].[BvSampleStatusSummary]([ITS] ASC);




GO
PRINT N'Altering [dbo].[BvTrBvInterview_InterviewsDelete]...';


GO
ALTER TRIGGER [BvTrBvInterview_InterviewsDelete] ON [dbo].[BvInterview] 
AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON

	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
		    /*[SID]*/ SurveySID, 
			/*[ScheduledCallsCount]*/ 0, 
			/*[SuspendedCallsCount]*/ -COUNT(*) SuspendedCallsCount, 
			/*[MinutesSpentWorkingOnSurvey]*/ 0
        FROM deleted
        GROUP BY SurveySID

    INSERT INTO [BvSampleStatusSummaryDelta]
		SELECT 
    	    /*[SurveySID]*/ SurveySID,
	        /*[ITS]*/ TransientState,
	        /*[Cnt]*/ -COUNT(ID),
			/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
	    FROM DELETED
	    GROUP BY SurveySID, TransientState, CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
END
GO
PRINT N'Altering [dbo].[BvTrBvInterview_InterviewsInsert]...';


GO
ALTER TRIGGER [BvTrBvInterview_InterviewsInsert] ON [dbo].[BvInterview] 
AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON
    
	INSERT INTO [BvAggregateSurveyDelta]
		SELECT 
		    /*[SID]*/ SurveySID, 
			/*[ScheduledCallsCount]*/ 0, 
			/*[SuspendedCallsCount]*/ COUNT(*) SuspendedCallsCount, 
			/*[MinutesSpentWorkingOnSurvey]*/ 0
        FROM inserted
        GROUP BY SurveySID

    INSERT INTO [BvSampleStatusSummaryDelta]
		SELECT 
    	    /*[SurveySID]*/ SurveySID,
	        /*[ITS]*/ TransientState,
	        /*[Cnt]*/ COUNT(ID),
			/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
	    FROM INSERTED
	    GROUP BY SurveySID, TransientState, CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
END
GO
PRINT N'Altering [dbo].[BvTrBvInterview_InterviewsUpdate]...';


GO
ALTER TRIGGER [BvTrBvInterview_InterviewsUpdate] ON [dbo].[BvInterview] 
AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON

    IF UPDATE( TransientState )
    BEGIN
		INSERT INTO [BvSampleStatusSummaryDelta]
			SELECT 
    			/*[SurveySID]*/ SurveySID,
				/*[ITS]*/ TransientState,
				/*[Cnt]*/ -COUNT(ID),
				/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
			FROM DELETED
			GROUP BY SurveySID, TransientState,CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END

		INSERT INTO [BvSampleStatusSummaryDelta]
			SELECT 
    			/*[SurveySID]*/ SurveySID,
				/*[ITS]*/ TransientState,
				/*[Cnt]*/ COUNT(ID),
				/*[IsCati]*/ CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
			FROM INSERTED
			GROUP BY SurveySID, TransientState, CASE WHEN [LastChannelID] = 1 THEN 0 ELSE 1 END
    END
END
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
GO
PRINT N'Altering [dbo].[BvSpGetSurveyActivityWithAlerts]...';


GO
ALTER PROCEDURE [dbo].[BvSpGetSurveyActivityWithAlerts]
   @BatchID int, @onlyActiveSurveys bit, @CustomITS1 INT, @CustomITS2 INT, 
   @CustomITS3 INT, @CustomITS4 INT, @CustomITS5 INT, @onlyCatiInterviews BIT = 0
AS  
	;WITH CustomITSes as (
		SELECT 
			SurveySID,
			SUM( CASE WHEN ITS = @CustomITS1 THEN Cnt ELSE NULL END ) as CustomITS1_Cnt,
			MAX( CASE WHEN ITS = @CustomITS1 THEN AlertStatus ELSE 0 END ) as CustomITS1_Alert,
			SUM( CASE WHEN ITS = @CustomITS2 THEN Cnt ELSE NULL END ) as CustomITS2_Cnt,
			MAX( CASE WHEN ITS = @CustomITS2 THEN AlertStatus ELSE 0 END ) as CustomITS2_Alert,
			SUM( CASE WHEN ITS = @CustomITS3 THEN Cnt ELSE NULL END ) as CustomITS3_Cnt,
			MAX( CASE WHEN ITS = @CustomITS3 THEN AlertStatus ELSE 0 END ) as CustomITS3_Alert,
			SUM( CASE WHEN ITS = @CustomITS4 THEN Cnt ELSE NULL END ) as CustomITS4_Cnt,
			MAX( CASE WHEN ITS = @CustomITS4 THEN AlertStatus ELSE 0 END ) as CustomITS4_Alert,
			SUM( CASE WHEN ITS = @CustomITS5 THEN Cnt ELSE NULL END ) as CustomITS5_Cnt,
			MAX( CASE WHEN ITS = @CustomITS5 THEN AlertStatus ELSE 0 END ) as CustomITS5_Alert
		FROM BvSampleStatusSummary
        WHERE @onlyCatiInterviews = 0 OR IsCati = 1
		GROUP BY SurveySID
	)
    SELECT asas.[SID] as SurveySID,
           asas.[Name] as ProjectID,
           asas.[Description]  as SurveyName,
           asas.[InterviewersLoggedCount],
           asas.[InterviewersLoggedCountPrev],
           asas.[NextAppointmentTime],
           asas.[TotalSampleSize], -- count of interview with 'fresh sample' its
           asas.[ScheduledCallsCount],
           asas.[ScheduledCallsCountPrev],
           asas.[SuspendedCallsCount],
           asas.[SuspendedCallsCountPrev],
           asas.[MinutesSpentWorkingOnSurvey],
           asas.[AssignedInterviewersCount],
           asas.[StrikeRate],
           asas.[CountCalls],
           asas.[AvgDuration],
           asas.[AlertStatusOfInterviewersLoggedCount],
           asas.[AlertStatusOfNextAppointmentTime],
           asas.[AlertStatusOfTotalSampleSize],
           asas.[AlertStatusOfScheduledCallsCount],
           asas.[AlertStatusOfSuspendedCallsCount],
           asas.[AlertStatusOfMinutesSpentWorkingOnSurvey],
           asas.[AlertStatusOfAssignedInterviewersCount],
           asas.[AlertStatusOfStrikeRate],
           asas.[AlertStatusOfCountCalls],
           asas.[MaxStatusOfITSAlerts],
           BvSurvey.[Target],
           asas.[StrikeRate1h],
           asas.[CountCalls1h],
           asas.[AvgDuration1h],
           asas.[MinutesSpentWorkingOnSurveyInDay],
           asas.[AlertStatusOfStrikeRate1h],
           asas.[AlertStatusOfCountCalls1h],
           CustomITSes.[CustomITS1_Cnt],
           CustomITSes.[CustomITS1_Alert],
           CustomITSes.[CustomITS2_Cnt],
           CustomITSes.[CustomITS2_Alert],
           CustomITSes.[CustomITS3_Cnt],
           CustomITSes.[CustomITS3_Alert],
           CustomITSes.[CustomITS4_Cnt],
           CustomITSes.[CustomITS4_Alert],
           CustomITSes.[CustomITS5_Cnt],
           CustomITSes.[CustomITS5_Alert]
    FROM BvTransferArrays ta
    INNER JOIN BvAggregateSurveyAlertStatus asas
        ON ta.ItemID = asas.SID
    INNER JOIN BvSurvey 
        ON (BvSurvey.SID = asas.SID)
	LEFT JOIN CustomITSes 
		ON asas.SID = CustomITSes.SurveySID
    WHERE ta.BatchID = @BatchID
	AND BvSurvey.State <> 2
	AND	InterviewersLoggedCount >= @onlyActiveSurveys
GO
PRINT N'Altering [dbo].[BvSpSampleStatusSummaryProcessDelta]...';


GO
ALTER PROCEDURE [dbo].[BvSpSampleStatusSummaryProcessDelta]
AS

	DECLARE @BvSampleStatusSummaryDelta TABLE
	(
		[ID]			BIGINT,
		[SurveySID]		INT NOT NULL,
		[ITS]			INT NOT NULL,
		[Cnt]			INT NOT NULL,
		[IsCati]		BIT NOT NULL
	);

	DELETE FROM [BvSampleStatusSummaryDelta] WITH (READPAST)
	OUTPUT DELETED.* INTO @BvSampleStatusSummaryDelta

	UPDATE aggrTbl
		SET aggrTbl.Cnt = aggrTbl.Cnt + data.Dif,
			alertStatus = dbo.udf_AlertStatus_INT( aggrTbl.Cnt + data.Dif, ThresholdDef.Amber, ThresholdDef.Red )
	FROM BvSampleStatusSummary aggrTbl
		INNER JOIN ( 
			SELECT SurveySID, ITS, SUM(Cnt) as Dif, IsCati FROM @BvSampleStatusSummaryDelta GROUP BY SurveySID, ITS, IsCati
				) as data
		ON aggrTbl.SurveySID = data.SurveySID AND aggrTbl.ITS = data.ITS AND aggrTbl.IsCati = data.IsCati
	LEFT JOIN BvThresholdITS as ThresholdDef
		ON ThresholdDef.SurveySID = 0 /*Use default thresholds, survey specific thresholds are not supported now*/ AND ThresholdDef.ITS = data.ITS 

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpSurvey_Insert]...';


GO
ALTER  PROCEDURE [dbo].[BvSpSurvey_Insert]
        @SID int,
        @Name nvarchar( 255 ),
        @Description nvarchar( 255 ),
        @QuotaType tinyint,
		@DialMode tinyint,
        @State int,
        @forceOpnRev int,
        @StateGroupID int,
        @RecWholeInt int,
		@InterviewScreenRecording bit,
        @RouteAddress NVARCHAR(255),
        @CfDbSchemaPath NVARCHAR(255),
        @DestinationTableName NVARCHAR (255), 
		@ReplicationStatus BIT,
		@ScheduleID INT,
		@DialerParameters NVARCHAR(MAX),
		@IsTelephoneBlacklistSupported BIT,
		@NotificationEmail NVARCHAR(MAX),
		@EnforceHttps BIT,
		@SurveySchedulingMode SMALLINT,
		@SurveySqlServerName NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON


	IF @StateGroupID = 0
	BEGIN
		DECLARE @MinOrder INTEGER
		SELECT @MinOrder = MIN([Order]) FROM BvStateGroup
		SELECT @StateGroupID = [ID] FROM BvStateGroup WHERE [Order] = @MinOrder
	END


	IF ISNULL( @ScheduleID, 0 ) = 0
	BEGIN
		SELECT @ScheduleID = MIN( ScheduleID ) FROM BvSchedule
	END

	INSERT  BvSurvey( 
			SID, 
			[Name], 
			[Description],
			QuotaType,
			DialMode,
			State,
			ForceOpnRev,
			StateGroupID,
			RecWholeInt,
			InterviewScreenRecording,
			CfDbSchemaPath,
			DestinationTableName, 
			ReplicationStatus,
			ScheduleID,
			DialerParameters,
			IsTelephoneBlacklistSupported,
			[NotificationEmail],
			[EnforceHttps],
			SurveySchedulingMode,
			SurveySqlServerName
			)
		VALUES
		(
			@SID,
			@Name,
			@Description,
			@QuotaType,
			@DialMode,
			@State,
			@forceOpnRev,
			@StateGroupID,
			@RecWholeInt,
			@InterviewScreenRecording,
			@CfDbSchemaPath,
			@DestinationTableName, 
			@ReplicationStatus,
			@ScheduleID,
			@DialerParameters,
			@IsTelephoneBlacklistSupported,
			@NotificationEmail,
			@EnforceHttps,
			@SurveySchedulingMode,
			@SurveySqlServerName	
		)
        
	INSERT BvAggregateSurvey (SID) VALUES(@SID)
	INSERT BvAggregateSurveyAlertStatus (SID, Name, Description) VALUES(@SID, @Name, @Description)

	INSERT BvAppointmentCounters (SurveySID, SurveyName, ProjectID, CountForShortInterval, CountForLongInterval)
	VALUES(@SID, @Description, @Name, 0, 0)

	INSERT INTO BvSampleStatusSummary( SurveySID, ITS, IsCati ) 
			SELECT @SID, StateID, 0 FROM BvState WHERE StateGroupID = @StateGroupID
	
	INSERT INTO BvSampleStatusSummary( SurveySID, ITS, IsCati ) 
			SELECT @SID, StateID, 1 FROM BvState WHERE StateGroupID = @StateGroupID

	-- Add default schedule param of current scheduling script to BvScheduleParam table
	INSERT INTO BvScheduleParam( ScheduleID, SurveySID, ParamID, [Name], Description, Type, Value ) 
		SELECT sp.ScheduleID, @SID, sp.ParamID, sp.Name, sp.Description, sp.Type, sp.Value
					 FROM BvScheduleParam sp 
							WHERE sp.SurveySID = 0 AND sp.ScheduleID = @ScheduleID

	RETURN (0)
END


GO
PRINT N'Migration to new BvSampleStatusSummary table design';
GO

DELETE FROM BvSampleStatusSummary;
DELETE FROM BvSampleStatusSummaryDelta;

INSERT INTO BvSampleStatusSummary( SurveySID, ITS, IsCati ) 
		SELECT SID, StateID, 0 FROM BvSurvey LEFT JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID;

INSERT INTO BvSampleStatusSummary( SurveySID, ITS, IsCati ) 
		SELECT SID, StateID, 1 FROM BvSurvey LEFT JOIN BvState ON BvState.StateGroupID = BvSurvey.StateGroupID;

INSERT INTO BvSampleStatusSummaryDelta
		SELECT 
		/*[SurveySID]*/ SurveySID,
		/*[ITS]*/       TransientState,
		/*[Cnt]*/       COUNT(ID),
		/*[IsCati]*/    CASE WHEN LastChannelID = 1 THEN 0 ELSE 1 END
		FROM BvInterview
		GROUP BY SurveySID, TransientState, CASE WHEN LastChannelID = 1 THEN 0 ELSE 1 END;

EXEC BvSpSampleStatusSummaryProcessDelta;


GO
PRINT N'Refreshing [dbo].[BvSpSampleStatusSummary_Get]...';

GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSampleStatusSummary_Get]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpThresholdITS_Set]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpThresholdITS_Set]';


GO
PRINT N'Refreshing [dbo].[BvSpThresholdITS_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpThresholdITS_Delete]';


GO
PRINT N'Update complete.';


GO
