CREATE TABLE [dbo].[BvInterviewerProductivityReportTemplate]
(
	[Id] INT IDENTITY (1, 1) NOT NULL CONSTRAINT PK_BvInterviewerProductivityReportTemplate_Id PRIMARY KEY,
	[Name] NVARCHAR (255) NOT NULL CONSTRAINT UC_BvInterviewerProductivityReportTemplate UNIQUE NONCLUSTERED ([Name] ASC), 
    [DateCreated] DATETIME NOT NULL, 
    [CreatorName] NVARCHAR(255) NOT NULL, 
	[CreatorLogin] NVARCHAR(255) NOT NULL,
    [LastModified] DATETIME NOT NULL, 
	[AccessType] TINYINT NOT NULL, 
    [IsPortrait] BIT NOT NULL, 
    [IsDefault] BIT NOT NULL, 
    [IncludeZeroValues] BIT NOT NULL, 
    [ShowDialerAttempts] BIT NOT NULL, 
    [IncludeBreakTimeInCalculations] BIT NOT NULL, 
    [SplitBySurvey] BIT NOT NULL CONSTRAINT DF_BvInterviewerProductivityReportTemplate_SplitBySurvey DEFAULT 0,
    [TimeRepresentationType] INT NOT NULL CONSTRAINT DF_BvInterviewerProductivityReportTemplate_TimeRepresentationType DEFAULT 0,
    [ColumnData] XML NOT NULL
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]
ON [dbo].[BvInterviewerProductivityReportTemplate] ([IsDefault])
WHERE [IsDefault] = 1