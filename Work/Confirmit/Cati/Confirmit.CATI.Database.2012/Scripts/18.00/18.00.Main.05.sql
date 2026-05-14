GO
PRINT N'Dropping [dbo].[BvInterview].[BvIx_int_State]...';


GO
DROP INDEX [BvIx_int_State]
    ON [dbo].[BvInterview];


GO
PRINT N'Altering [dbo].[BvInterview]...';


GO
ALTER TABLE [dbo].[BvInterview] ALTER COLUMN [TransientState] INT NOT NULL;


GO
PRINT N'Creating [dbo].[BvInterview].[BvIx_int_State]...';


GO
CREATE NONCLUSTERED INDEX [BvIx_int_State]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TransientState] ASC);


GO
PRINT N'Creating [dbo].[BvSampleStatusSummaryDelta]...';


GO
CREATE TABLE [dbo].[BvSampleStatusSummaryDelta] (
    [ID]        BIGINT IDENTITY (1, 1) NOT NULL,
    [SurveySID] INT    NOT NULL,
    [ITS]       INT    NOT NULL,
    [Cnt]       INT    NOT NULL,
    CONSTRAINT [BvSampleStatusSummaryDelta_PK_ID] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
ALTER TABLE [dbo].[BvSampleStatusSummaryDelta] SET (LOCK_ESCALATION = DISABLE);


GO
PRINT N'Creating DF_BvSampleStatusSummaryDelta_Cnt...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummaryDelta]
    ADD CONSTRAINT [DF_BvSampleStatusSummaryDelta_Cnt] DEFAULT (0) FOR [Cnt];


GO
PRINT N'Creating DF_BvSampleStatusSummaryDelta_Its...';


GO
ALTER TABLE [dbo].[BvSampleStatusSummaryDelta]
    ADD CONSTRAINT [DF_BvSampleStatusSummaryDelta_Its] DEFAULT (0) FOR [ITS];


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
	        /*[Cnt]*/ -COUNT(ID)
	    FROM DELETED
	    GROUP BY SurveySID, TransientState 
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
	        /*[Cnt]*/ COUNT(ID)
	    FROM INSERTED
	    GROUP BY SurveySID, TransientState 
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
				/*[Cnt]*/ -COUNT(ID)
			FROM DELETED
			GROUP BY SurveySID, TransientState 

		INSERT INTO [BvSampleStatusSummaryDelta]
			SELECT 
    			/*[SurveySID]*/ SurveySID,
				/*[ITS]*/ TransientState,
				/*[Cnt]*/ COUNT(ID)
			FROM INSERTED
			GROUP BY SurveySID, TransientState 
    END
END
GO
PRINT N'Altering [dbo].[BvSpAggregateSurveyProcessDelta]...';


GO
ALTER PROCEDURE [dbo].[BvSpAggregateSurveyProcessDelta]
AS
    DECLARE @BvAggregateSurveyDelta TABLE
	(
		[ID]                          BIGINT,
		[SID]                         INT NOT NULL,
		[ScheduledCallsCount]         INT NOT NULL,
		[SuspendedCallsCount]         INT NOT NULL,
		[MinutesSpentWorkingOnSurvey] INT NOT NULL
	);

	DELETE FROM BvAggregateSurveyDelta WITH (READPAST)
	OUTPUT DELETED.* INTO @BvAggregateSurveyDelta

	UPDATE 
	    BvAggregateSurvey 
	SET 
	    BvAggregateSurvey.MinutesSpentWorkingOnSurvey += AggregatedDelta.MinutesSpentWorkingOnSurvey,
	    BvAggregateSurvey.ScheduledCallsCount += AggregatedDelta.ScheduledCallsCount,
		BvAggregateSurvey.SuspendedCallsCount += AggregatedDelta.SuspendedCallsCount
	FROM
	    BvAggregateSurvey
	INNER JOIN
	(
	    SELECT [SID],
		       SUM([ScheduledCallsCount]) ScheduledCallsCount,
		       SUM([SuspendedCallsCount]) SuspendedCallsCount,
			   SUM([MinutesSpentWorkingOnSurvey]) MinutesSpentWorkingOnSurvey
		FROM @BvAggregateSurveyDelta
		GROUP BY [SID]
	) AggregatedDelta
	ON BvAggregateSurvey.SID = AggregatedDelta.SID

RETURN 0
GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Creating [dbo].[BvSpSampleStatusSummaryProcessDelta]...';


GO
CREATE PROCEDURE [dbo].[BvSpSampleStatusSummaryProcessDelta]
AS

	DECLARE @BvSampleStatusSummaryDelta TABLE
	(
		[ID]			BIGINT,
		[SurveySID]		INT NOT NULL,
		[ITS]			INT NOT NULL,
		[Cnt]			INT NOT NULL
	);

	DELETE FROM [BvSampleStatusSummaryDelta] WITH (READPAST)
	OUTPUT DELETED.* INTO @BvSampleStatusSummaryDelta

	UPDATE aggrTbl
		SET aggrTbl.Cnt = aggrTbl.Cnt + data.Dif,
			alertStatus = dbo.udf_AlertStatus_INT( aggrTbl.Cnt + data.Dif, ThresholdDef.Amber, ThresholdDef.Red )
	FROM BvSampleStatusSummary aggrTbl
		INNER JOIN ( 
			SELECT SurveySID, ITS, SUM(Cnt) as Dif FROM @BvSampleStatusSummaryDelta GROUP BY SurveySID, ITS
				) as data
		ON aggrTbl.SurveySID = data.SurveySID AND aggrTbl.ITS = data.ITS 
	LEFT JOIN BvThresholdITS as ThresholdDef
		ON ThresholdDef.SurveySID = 0 /*Use default thresholds, survey specific thresholds are not supported now*/ AND ThresholdDef.ITS = data.ITS 

RETURN 0
GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangeShiftType]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_ChangeShiftType]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_MoveToITS]';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCallHistory_List]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_CfData_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterview_UpdateRespondentFields]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviews_UpdateState_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviews_UpdateState_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInterviewsAndAppointments_Delete_Batch]';


GO
PRINT N'Refreshing [dbo].[BvSpSample_Abandon]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSample_Abandon]';


GO
PRINT N'Refreshing [dbo].[BvSpSample_Finalize]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSample_Finalize]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyModifyStateGroup]';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurveyState_Update]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_Update_2]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTasks_Update_2]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_Delete]';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpTimezone_DeleteUnused]';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSurvey_Update]';


GO
PRINT N'Update complete.';


GO
