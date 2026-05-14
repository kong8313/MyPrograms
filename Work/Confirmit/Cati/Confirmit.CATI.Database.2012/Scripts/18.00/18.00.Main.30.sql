GO
PRINT N'Dropping [dbo].[BvAppointment].[IX_app_BatchID]...';


GO
DROP INDEX [IX_app_BatchID]
    ON [dbo].[BvAppointment];


GO
PRINT N'Dropping FkBvAppointmentsAlertStatus_Appointment...';


GO
ALTER TABLE [dbo].[BvAppointmentsAlertStatus] DROP CONSTRAINT [FkBvAppointmentsAlertStatus_Appointment];


GO
PRINT N'Dropping UQ_BvAppointment_Id...';


GO
ALTER TABLE [dbo].[BvAppointment] DROP CONSTRAINT [UQ_BvAppointment_Id];


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
