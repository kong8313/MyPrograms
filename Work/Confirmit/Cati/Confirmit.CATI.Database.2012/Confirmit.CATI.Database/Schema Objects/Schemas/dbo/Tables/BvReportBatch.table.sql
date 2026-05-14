CREATE TABLE [dbo].[BvReportBatch] (
    [ID]          INT      IDENTITY (1, 1) NOT NULL,
    [PersonSID]   INT      NOT NULL,
    [ReportID]    INT      NOT NULL,
    [TimeCreated] DATETIME NOT NULL
);

