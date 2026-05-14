GO
PRINT N'Dropping DF_BvAppointment_BatchID...';


GO
ALTER TABLE [dbo].[BvAppointment] DROP CONSTRAINT [DF_BvAppointment_BatchID];


GO
PRINT N'Dropping DF_BvAppointment_TempID...';


GO
ALTER TABLE [dbo].[BvAppointment] DROP CONSTRAINT [DF_BvAppointment_TempID];


GO
PRINT N'Dropping FkBvAppointmentsAlertStatus_Appointment...';


GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus] DROP CONSTRAINT [FkBvAppointmentsAlertStatus_Appointment];


GO
PRINT N'Starting rebuilding table [dbo].[BvAppointment]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvAppointment] (
    [SurveySID]      INT            NOT NULL,
    [InterviewSID]   INT            NOT NULL,
    [Time]           DATETIME       NOT NULL,
    [ExpTime]        DATETIME       NULL,
    [RespondentName] NVARCHAR (255) NULL,
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [State]          INT            NOT NULL,
    [ContactName]    NVARCHAR (255) NOT NULL,
    [BatchID]        INT            CONSTRAINT [DF_BvAppointment_BatchID] DEFAULT (0) NOT NULL,
    [TempID]         INT            CONSTRAINT [DF_BvAppointment_TempID] DEFAULT (0) NOT NULL,
    [TZID]           INT            NULL,
    CONSTRAINT [tmp_ms_xx_constraint_Pk_app] PRIMARY KEY CLUSTERED ([ID] ASC, [SurveySID] ASC),
    CONSTRAINT [tmp_ms_xx_constraint_UQ_BvAppointment_Id] UNIQUE NONCLUSTERED ([ID] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvAppointment])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvAppointment] ON;
        INSERT INTO [dbo].[tmp_ms_xx_BvAppointment] ([ID], [SurveySID], [InterviewSID], [Time], [ExpTime], [RespondentName], [State], [ContactName], [BatchID], [TempID], [TZID])
        SELECT   [ID],
                 [SurveySID],
                 [InterviewSID],
                 [Time],
                 [ExpTime],
                 [RespondentName],
                 [State],
                 [ContactName],
                 [BatchID],
                 [TempID],
                 [TZID]
        FROM     [dbo].[BvAppointment]
        ORDER BY [ID] ASC, [SurveySID] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvAppointment] OFF;
    END

DROP TABLE [dbo].[BvAppointment];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvAppointment]', N'BvAppointment';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_Pk_app]', N'Pk_app', N'OBJECT';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_UQ_BvAppointment_Id]', N'UQ_BvAppointment_Id', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_alert]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_alert]
    ON [dbo].[BvAppointment]([State] ASC, [Time] DESC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_BatchID]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_BatchID]
    ON [dbo].[BvAppointment]([BatchID] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_app_SurveySID_InterviewSID_State]...';


GO
CREATE NONCLUSTERED INDEX [IX_app_SurveySID_InterviewSID_State]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [InterviewSID] ASC, [State] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_BvAppointment_ExpTime]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAppointment_ExpTime]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [ExpTime] ASC);


GO
PRINT N'Creating [dbo].[BvAppointment].[IX_BvAppointment_Time]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvAppointment_Time]
    ON [dbo].[BvAppointment]([SurveySID] ASC, [Time] ASC);


GO
PRINT N'Creating FkBvAppointmentsAlertStatus_Appointment...';


GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus] WITH NOCHECK
    ADD CONSTRAINT [FkBvAppointmentsAlertStatus_Appointment] FOREIGN KEY ([ID]) REFERENCES [dbo].[BvAppointment] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
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
PRINT N'Refreshing [dbo].[BvSpRemoveExpiredCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'dbo.BvSpRemoveExpiredCalls';


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
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus] WITH CHECK CHECK CONSTRAINT [FkBvAppointmentsAlertStatus_Appointment];


GO
PRINT N'Update complete.';


GO
