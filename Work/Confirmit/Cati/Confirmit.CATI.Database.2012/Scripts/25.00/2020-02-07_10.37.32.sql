PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD [RecordCreationTime] DATETIME CONSTRAINT [DF_BvPersonDeferredMonitoring_RecordCreationTime] DEFAULT GETUTCDATE() NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpCleanDeferredMonitoring]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCleanDeferredMonitoring]';


GO
PRINT N'Refreshing [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDeferredMonitoringStartFile]';


GO
PRINT N'Update wrong RecordCreationTime values for existing records';


GO
UPDATE [dbo].[BvPersonDeferredMonitoring]
SET [RecordCreationTime] = [TimeStamp]
WHERE [TimeStamp] < [RecordCreationTime]

GO
PRINT N'Update complete.';


GO
