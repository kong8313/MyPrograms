GO
PRINT N'Altering [dbo].[AudioMonitoring]...';


GO
ALTER TABLE [dbo].[AudioMonitoring]
    ADD [MonitorMode] INT CONSTRAINT [DF_AudioMonitoring_MonitorMode] DEFAULT 0 NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpInsertUpdateAudioMonitoringSession]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpInsertUpdateAudioMonitoringSession]';


GO
PRINT N'Update complete.';


GO
