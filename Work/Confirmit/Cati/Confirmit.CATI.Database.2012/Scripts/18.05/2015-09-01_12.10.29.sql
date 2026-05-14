
PRINT N'Dropping [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring]...';
GO

DROP INDEX [IX_BvPersonDeferredMonitoring]
    ON [dbo].[BvPersonDeferredMonitoring];
GO

PRINT N'Altering [dbo].[BvPersonDeferredMonitoring]...';
GO

ALTER TABLE [dbo].[BvPersonDeferredMonitoring] DROP COLUMN [MonitoringSessionID];
GO

PRINT N'Creating [dbo].[BvPersonDeferredMonitoring].[IX_BvPersonDeferredMonitoring]...';
GO

CREATE NONCLUSTERED INDEX [IX_BvPersonDeferredMonitoring]
    ON [dbo].[BvPersonDeferredMonitoring]([PersonSID] ASC, [IsRecording] ASC, [IsComplete] ASC)
    INCLUDE([ID])
    ON [PRIMARY];
GO

PRINT N'Update complete.';
GO
