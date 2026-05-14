GO
PRINT N'Dropping Default Constraint [dbo].[DF_BvInterviewerProductivityReportTemplate_SplitBySurvey]...';


GO
ALTER TABLE [dbo].[BvInterviewerProductivityReportTemplate] DROP CONSTRAINT [DF_BvInterviewerProductivityReportTemplate_SplitBySurvey];


GO
PRINT N'Starting rebuilding table [dbo].[BvInterviewerProductivityReportTemplate]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvInterviewerProductivityReportTemplate] (
    [Id]                             INT            IDENTITY (1, 1) NOT NULL,
    [Name]                           NVARCHAR (255) NOT NULL,
    [DateCreated]                    DATETIME       NOT NULL,
    [CreatorName]                    NVARCHAR (255) NOT NULL,
    [CreatorLogin]                   NVARCHAR (255) NOT NULL,
    [LastModified]                   DATETIME       NOT NULL,
    [AccessType]                     TINYINT        NOT NULL,
    [IsPortrait]                     BIT            NOT NULL,
    [IsDefault]                      BIT            NOT NULL,
    [IncludeZeroValues]              BIT            NOT NULL,
    [ShowDialerAttempts]             BIT            NOT NULL,
    [IncludeBreakTimeInCalculations] BIT            NOT NULL,
    [SplitBySurvey]                  BIT            CONSTRAINT [DF_BvInterviewerProductivityReportTemplate_SplitBySurvey] DEFAULT 0 NOT NULL,
    [TimeRepresentationType]         INT            CONSTRAINT [DF_BvInterviewerProductivityReportTemplate_TimeRepresentationType] DEFAULT 0 NOT NULL,
    [ColumnData]                     XML            NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvInterviewerProductivityReportTemplate_Id1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [tmp_ms_xx_constraint_UC_BvInterviewerProductivityReportTemplate1] UNIQUE NONCLUSTERED ([Name] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvInterviewerProductivityReportTemplate])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvInterviewerProductivityReportTemplate] ON;
        INSERT INTO [dbo].[tmp_ms_xx_BvInterviewerProductivityReportTemplate] ([Id], [Name], [DateCreated], [CreatorName], [CreatorLogin], [LastModified], [AccessType], [IsPortrait], [IsDefault], [IncludeZeroValues], [ShowDialerAttempts], [IncludeBreakTimeInCalculations], [SplitBySurvey], [ColumnData])
        SELECT   [Id],
                 [Name],
                 [DateCreated],
                 [CreatorName],
                 [CreatorLogin],
                 [LastModified],
                 [AccessType],
                 [IsPortrait],
                 [IsDefault],
                 [IncludeZeroValues],
                 [ShowDialerAttempts],
                 [IncludeBreakTimeInCalculations],
                 [SplitBySurvey],
                 [ColumnData]
        FROM     [dbo].[BvInterviewerProductivityReportTemplate]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvInterviewerProductivityReportTemplate] OFF;
    END

DROP TABLE [dbo].[BvInterviewerProductivityReportTemplate];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvInterviewerProductivityReportTemplate]', N'BvInterviewerProductivityReportTemplate';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvInterviewerProductivityReportTemplate_Id1]', N'PK_BvInterviewerProductivityReportTemplate_Id', N'OBJECT';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_UC_BvInterviewerProductivityReportTemplate1]', N'UC_BvInterviewerProductivityReportTemplate', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating Index [dbo].[BvInterviewerProductivityReportTemplate].[UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]
    ON [dbo].[BvInterviewerProductivityReportTemplate]([IsDefault] ASC) WHERE [IsDefault] = 1;


GO
PRINT N'Update complete.';


GO
