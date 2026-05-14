GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_CallHistorySyncType]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_CallHistorySyncType] DEFAULT (0) FOR [CallHistorySyncType];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_ForceSyncBreaks]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_ForceSyncBreaks] DEFAULT (0) FOR [ForceSyncBreaks];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_ForceSyncCallHistory]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_ForceSyncCallHistory] DEFAULT (0) FOR [ForceSyncCallHistory];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_ForceSyncSessions]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_ForceSyncSessions] DEFAULT (0) FOR [ForceSyncSessions];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_LastPublishedTime]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_LastPublishedTime] DEFAULT ('1970-01-01T00:00:00.000') FOR [LastPublishedTime];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_ShouldSyncBreaks]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_ShouldSyncBreaks] DEFAULT (0) FOR [ShouldSyncBreaks];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_ShouldSyncSessions]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_ShouldSyncSessions] DEFAULT (0) FOR [ShouldSyncSessions];


GO
PRINT N'Update complete.';


GO
