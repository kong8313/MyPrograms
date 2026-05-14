CREATE TABLE [dbo].[BvScheduledEmailReports]
(
    [ReportType] INT NOT NULL, 
    [LastSent] DATETIME NULL 
	CONSTRAINT [PK_BvScheduledEmailReports_ReportType] PRIMARY KEY CLUSTERED ([ReportType] ASC)
)
