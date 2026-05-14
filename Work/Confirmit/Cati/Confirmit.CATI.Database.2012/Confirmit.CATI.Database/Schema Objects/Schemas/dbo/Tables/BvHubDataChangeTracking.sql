CREATE TABLE [dbo].[BvHubDataChangeTracking]
(
    [HubId] INT NOT NULL, 
    [LastPublishedCatiChangeTrackingVersion] BIGINT NOT NULL CONSTRAINT DF_BvHubDataChangeTracking_LastPublishedChangeTrackingVersion DEFAULT(0),
    [LastPublishedSessionsChangeTrackingVersion] BIGINT NOT NULL CONSTRAINT DF_BvHubDataChangeTracking_LastPublishedSessionsChangeTrackingVersion DEFAULT(0),
    [LastPublishedTime] DATETIME2(3) NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_LastPublishedTime] DEFAULT ('1970-01-01T00:00:00.000'), 
    [ShouldSyncSessions] BIT NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_ShouldSyncSessions] DEFAULT (0),
	[ShouldSyncBreaks] BIT NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_ShouldSyncBreaks] DEFAULT (0), 
    [CallHistorySyncType] SMALLINT NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_CallHistorySyncType] DEFAULT (0), 
    [ForceSyncSessions] BIT NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_ForceSyncSessions] DEFAULT (0), 
    [ForceSyncBreaks] BIT NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_ForceSyncBreaks] DEFAULT (0), 
    [ForceSyncCallHistory] BIT NOT NULL CONSTRAINT [DF_BvHubDataChangeTracking_ForceSyncCallHistory] DEFAULT (0), 
    CONSTRAINT [PK_BvHubDataChangeTracking] PRIMARY KEY ([HubId])
)
