GO
PRINT N'Dropping table [dbo].[BvInterviewerProductivityReportTemplate]...';


GO
DROP TABLE [dbo].[BvInterviewerProductivityReportTemplate];


GO
PRINT N'Creating table [dbo].[BvInterviewerProductivityReportTemplate]...';


GO
CREATE TABLE [dbo].[BvInterviewerProductivityReportTemplate] (
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
    [ColumnData]                     XML            NOT NULL,
    CONSTRAINT [PK_BvInterviewerProductivityReportTemplate_Id] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT UC_BvInterviewerProductivityReportTemplate UNIQUE NONCLUSTERED ([Name] ASC)
);


GO
PRINT N'Creating [dbo].[BvInterviewerProductivityReportTemplate].[UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]
    ON [dbo].[BvInterviewerProductivityReportTemplate]([IsDefault] ASC) WHERE [IsDefault] = 1;


GO
PRINT N'Adding base template ';


INSERT INTO [dbo].[BvInterviewerProductivityReportTemplate] VALUES 
('System template', '2019-01-01 00:00:00.000', 'System', 'system', '2019-01-01 00:00:00.000', 2, 0, 1, 0, 0, 0,
'<Columns xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ProductivityReportTemplateColumn>
    <DisplayName>User ID</DisplayName>
    <StandardColumnName>PersonId</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>User name</DisplayName>
    <StandardColumnName>PersonName</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Log on time (hours)</DisplayName>
    <StandardColumnName>LogOnHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Waiting time (hours)</DisplayName>
    <StandardColumnName>WaitingHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Paid break time (hours)</DisplayName>
    <StandardColumnName>BreakHoursPaid</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Unpaid break time (hours)</DisplayName>
    <StandardColumnName>BreakHoursUnpaid</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Review Time (hours)</DisplayName>
    <StandardColumnName>OpenEndReviewHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn xsi:type="ProductivityReportTemplateColumnWithStatuses">
    <DisplayName>Interviews</DisplayName>
    <StandardColumnName>DialingsCount</StandardColumnName>
    <Visible>true</Visible>
    <IsIncludeStatuses>false</IsIncludeStatuses>
    <ExtendedStatuses />
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Interviews per log on hour</DisplayName>
    <StandardColumnName>DialingsPerLogOnHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn xsi:type="ProductivityReportTemplateColumnWithStatuses">
    <DisplayName>Completes</DisplayName>
    <StandardColumnName>Completes</StandardColumnName>
    <Visible>true</Visible>
    <IsIncludeStatuses>true</IsIncludeStatuses>
    <ExtendedStatuses>
      <int>13</int>
    </ExtendedStatuses>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Completes per log on hour</DisplayName>
    <StandardColumnName>CompletesPerLogOnHours</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Interviews per complete</DisplayName>
    <StandardColumnName>DialingsPerComplete</StandardColumnName>
  </ProductivityReportTemplateColumn>
  <ProductivityReportTemplateColumn>
    <DisplayName>Average completed interview length (min)</DisplayName>
    <StandardColumnName>AverageDuration</StandardColumnName>
  </ProductivityReportTemplateColumn>
</Columns>')


GO
PRINT N'Update complete.';


GO
