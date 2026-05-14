CREATE TABLE [dbo].[BvPersonMonitoring] (
    [PersonSID]           INT            NOT NULL,
    [supervisorName]      NVARCHAR (255) NOT NULL,
    [MonitoringSessionID] BIGINT         NOT NULL,
    [TelephoneNumber] NVARCHAR(256)         NULL,
	[IsWebMonitoring] BIT NOT NULL CONSTRAINT DF_BvPersonMonitoring_IsWebMonitoring DEFAULT(0),
	[IsLiveMonitoringEnabled] BIT NOT NULL CONSTRAINT DF_BvPersonMonitoring_IsLiveMonitoringEnabled DEFAULT(0)
);

