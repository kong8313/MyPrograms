CREATE TABLE [dbo].[BvPersonMonitoringEvents] (
    [ID]                  BIGINT          IDENTITY (1, 1) NOT NULL,
    [PersonSID]           INT             NOT NULL,
    [MonitoringSessionID] BIGINT          NOT NULL,
    [TimeStamp]           DATETIME        NOT NULL,
    [MessageType]         INT             NOT NULL,
    [EventObject]         VARBINARY (MAX) NULL
);

