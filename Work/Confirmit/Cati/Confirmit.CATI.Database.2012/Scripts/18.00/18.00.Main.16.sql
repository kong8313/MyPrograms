GO
PRINT N'Dropping DF_BvInterview_ConfirmitSid...';


GO
ALTER TABLE [dbo].[BvInterview] DROP CONSTRAINT [DF_BvInterview_ConfirmitSid];


GO
PRINT N'Dropping DF_BvInterview_DialerId...';


GO
ALTER TABLE [dbo].[BvInterview] DROP CONSTRAINT [DF_BvInterview_DialerId];


GO
PRINT N'Dropping DF_BvInterview_DialingMode...';


GO
ALTER TABLE [dbo].[BvInterview] DROP CONSTRAINT [DF_BvInterview_DialingMode];


GO
PRINT N'Altering [dbo].[BvCallExpired]...';


GO
ALTER TABLE [dbo].[BvCallExpired] ALTER COLUMN [interviewID] INT NOT NULL;

ALTER TABLE [dbo].[BvCallExpired] ALTER COLUMN [surveyID] INT NOT NULL;


GO
PRINT N'Creating PK_BvCallExpired...';


GO
ALTER TABLE [dbo].[BvCallExpired]
    ADD CONSTRAINT [PK_BvCallExpired] PRIMARY KEY CLUSTERED ([surveyID] ASC, [interviewID] ASC);

GO
PRINT N'Starting rebuilding table [dbo].[BvInterview]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvInterview] (
    [ID]                INT            NOT NULL,
    [SurveySID]         INT            NOT NULL,
    [TelephoneNumber]   VARCHAR (255)  NULL,
    [RespondentName]    NVARCHAR (255) NULL,
    [TimezoneID]        INT            NULL,
    [TransientState]    INT            NOT NULL,
    [LastCallTime]      DATETIME       NULL,
    [LastCallPersonSID] INT            NULL,
    [Duration]          INT            NULL,
    [ExtensionNumber]   VARCHAR (255)  NULL,
    [ConfirmitSid]      VARCHAR (64)   CONSTRAINT [DF_BvInterview_ConfirmitSid] DEFAULT ('') NOT NULL,
    [BatchID]           INT            NOT NULL,
    [LastChannelID]     TINYINT        NOT NULL,
    [DialingMode]       TINYINT        CONSTRAINT [DF_BvInterview_DialingMode] DEFAULT (0) NOT NULL,
    [DialerId]          INT            CONSTRAINT [DF_BvInterview_DialerId] DEFAULT (0) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_BvPk_int] PRIMARY KEY CLUSTERED ([SurveySID] ASC, [ID] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvInterview])
    BEGIN
        
        INSERT INTO [dbo].[tmp_ms_xx_BvInterview] ([SurveySID], [ID], [TelephoneNumber], [RespondentName], [TimezoneID], [TransientState], [LastCallTime], [LastCallPersonSID], [Duration], [ExtensionNumber], [ConfirmitSid], [BatchID], [LastChannelID], [DialingMode], [DialerId])
        SELECT   [SurveySID],
                 [ID],
                 [TelephoneNumber],
                 [RespondentName],
                 [TimezoneID],
                 [TransientState],
                 [LastCallTime],
                 [LastCallPersonSID],
                 [Duration],
                 [ExtensionNumber],
                 [ConfirmitSid],
                 [BatchID],
                 [LastChannelID],
                 [DialingMode],
                 [DialerId]
        FROM     [dbo].[BvInterview]
        ORDER BY [SurveySID] ASC, [ID] ASC;
        
    END

DROP TABLE [dbo].[BvInterview];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvInterview]', N'BvInterview';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_BvPk_int]', N'BvPk_int', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvInterview].[BvIx_int_Batch]...';


GO
CREATE NONCLUSTERED INDEX [BvIx_int_Batch]
    ON [dbo].[BvInterview]([BatchID] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[BvIx_int_State]...';


GO
CREATE NONCLUSTERED INDEX [BvIx_int_State]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TransientState] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_DialingMode]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_DialingMode]
    ON [dbo].[BvInterview]([SurveySID] ASC, [DialingMode] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_LastCallTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_LastCallTime]
    ON [dbo].[BvInterview]([SurveySID] ASC, [LastCallTime] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_RespondentName]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_RespondentName]
    ON [dbo].[BvInterview]([SurveySID] ASC, [RespondentName] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_TelephoneNumber]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_TelephoneNumber]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TelephoneNumber] ASC);


GO
PRINT N'Creating [dbo].[BvInterview].[IX_BvInterview_TimezoneID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvInterview_TimezoneID]
    ON [dbo].[BvInterview]([SurveySID] ASC, [TimezoneID] ASC);


GO
PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring_SurveySID]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring_SurveySID]
    ON [dbo].[BvPersonDeferredMonitoring]([SurveySID] ASC);


GO
PRINT N'Creating [dbo].[BvTrBvInterview_InterviewsDelete]...';


GO
CREATE TRIGGER [BvTrBvInterview_InterviewsDelete] ON [dbo].[BvInterview] 
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
PRINT N'Creating [dbo].[BvTrBvInterview_InterviewsInsert]...';


GO
CREATE TRIGGER [BvTrBvInterview_InterviewsInsert] ON [dbo].[BvInterview] 
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
PRINT N'Creating [dbo].[BvTrBvInterview_InterviewsUpdate]...';


GO
CREATE TRIGGER [BvTrBvInterview_InterviewsUpdate] ON [dbo].[BvInterview] 
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
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Refreshing [dbo].[BvSpGetExpiredCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetExpiredCalls';


GO
PRINT N'Refreshing [dbo].[BvSpRemoveExpiredCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpRemoveExpiredCalls';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAll]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAlert_RecalculateAll';


GO
PRINT N'Refreshing [dbo].[BvSpAlert_RecalculateAppointment]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAlert_RecalculateAppointment';


GO
PRINT N'Refreshing [dbo].[BvSpAppointmentGet2]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAppointmentGet2';


GO
PRINT N'Refreshing [dbo].[BvSpAppointmentUpdate]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpAppointmentUpdate';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Get]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_Get';


GO
PRINT N'Refreshing [dbo].[BvSpCallHistory_List]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCallHistory_List';


GO
PRINT N'Refreshing [dbo].[BvSpGetAllAppointmentsForUser]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetAllAppointmentsForUser';


GO
PRINT N'Refreshing [dbo].[BvSpHistory_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpHistory_CfData_Insert';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_UpdateRespondentFields]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpInterview_UpdateRespondentFields';


GO
PRINT N'Refreshing [dbo].[BvSpInterviewsAndAppointments_Delete_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpInterviewsAndAppointments_Delete_Batch';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpLookUpByPerson';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForAssignmentMode]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpLookUpByPerson_ForAssignmentMode';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForCallGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpLookUpByPerson_ForCallGroup';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForManualMode]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpLookUpByPerson_ForManualMode';


GO
PRINT N'Refreshing [dbo].[BvSpLookUpByPerson_ForSurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpLookUpByPerson_ForSurvey';


GO
PRINT N'Refreshing [dbo].[BvSpShiftType_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpShiftType_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_DeleteFiltered]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_DeleteFiltered';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSvySch_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSvySch_Insert';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_Activate';


GO
PRINT N'Refreshing [dbo].[BvSpCall_ChangeShiftType]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_ChangeShiftType';


GO
PRINT N'Refreshing [dbo].[BvSpCall_MoveToITS]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpCall_MoveToITS';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnly';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyByPersonGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetCachedCallsForPredictiveSurveyByPersonGroup';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyBySurvey]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetCachedCallsForPredictiveSurveyBySurvey';


GO
PRINT N'Refreshing [dbo].[BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssigned';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_CfData_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpInterview_CfData_Insert';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpInterview_Insert';


GO
PRINT N'Refreshing [dbo].[BvSpInterview_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpInterview_Update';


GO
PRINT N'Refreshing [dbo].[BvSpInterviews_UpdateState_Batch]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpInterviews_UpdateState_Batch';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyModifyStateGroup]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurveyModifyStateGroup';


GO
PRINT N'Refreshing [dbo].[BvSpSurveyState_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurveyState_Update';


GO
PRINT N'Refreshing [dbo].[BvSpTasks_Update_2]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpTasks_Update_2';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_Delete]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpTimezone_Delete';


GO
PRINT N'Refreshing [dbo].[BvSpTimezone_DeleteUnused]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpTimezone_DeleteUnused';


GO
PRINT N'Refreshing [dbo].[BvSpSurvey_Update]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpSurvey_Update';


GO
PRINT N'Checking existing data against newly created constraints';


GO

GO
PRINT N'Update complete.';


GO
