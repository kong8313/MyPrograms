GO
PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonDeferredMonitoring]
    ADD [IsOldInterface] BIT CONSTRAINT [DF_BvPersonDeferredMonitoring_IsOldInterface] DEFAULT 0 NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpCleanDeferredMonitoring]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCleanDeferredMonitoring]';


GO
PRINT N'Refreshing [dbo].[BvSpGetDeferredMonitoringStartFile]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetDeferredMonitoringStartFile]';


GO
PRINT N'Update complete.';


GO
