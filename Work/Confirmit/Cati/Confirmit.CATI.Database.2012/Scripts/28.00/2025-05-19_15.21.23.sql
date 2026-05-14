GO
PRINT N'Creating Table [dbo].[BvHubDataChangeTracking]...';


GO
CREATE TABLE [dbo].[BvHubDataChangeTracking] (
    [HubId]                                      INT           NOT NULL,
    [LastPublishedCatiChangeTrackingVersion]     BIGINT        NOT NULL,
    [LastPublishedSessionsChangeTrackingVersion] BIGINT        NOT NULL,
    [LastPublishedTime]                          DATETIME2 (3) NOT NULL,
    [ShouldSyncSessions]                         BIT           NOT NULL,
    [ShouldSyncBreaks]                           BIT           NOT NULL,
    [CallHistorySyncType]                        SMALLINT      NOT NULL,
    [ForceSyncSessions]                           BIT           NOT NULL,
    [ForceSyncBreaks]                            BIT           NOT NULL,
    [ForceSyncCallHistory]                       BIT           NOT NULL,
    CONSTRAINT [PK_BvHubDataChangeTracking] PRIMARY KEY CLUSTERED ([HubId] ASC)
);


GO
PRINT N'Creating Primary Key [dbo].[PK_BvHistory_Id]...';


GO
ALTER TABLE [dbo].[BvHistory]
    ADD CONSTRAINT [PK_BvHistory_Id] PRIMARY KEY NONCLUSTERED ([ID] ASC);


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_LastPublishedChangeTrackingVersion]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_LastPublishedChangeTrackingVersion] DEFAULT (0) FOR [LastPublishedCatiChangeTrackingVersion];


GO
PRINT N'Creating Default Constraint [dbo].[DF_BvHubDataChangeTracking_LastPublishedSessionsChangeTrackingVersion]...';


GO
ALTER TABLE [dbo].[BvHubDataChangeTracking]
    ADD CONSTRAINT [DF_BvHubDataChangeTracking_LastPublishedSessionsChangeTrackingVersion] DEFAULT (0) FOR [LastPublishedSessionsChangeTrackingVersion];


GO
PRINT N'Update complete.';


GO
