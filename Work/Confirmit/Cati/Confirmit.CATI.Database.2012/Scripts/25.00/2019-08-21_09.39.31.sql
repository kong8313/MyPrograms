GO
PRINT N'Creating [dbo].[DF_BvInterviewerProductivityReportTemplate_ReadOnly]...';


GO
ALTER TABLE [dbo].[BvInterviewerProductivityReportTemplate]
    ADD [ReadOnly] BIT NOT NULL CONSTRAINT [DF_BvInterviewerProductivityReportTemplate_ReadOnly] DEFAULT 0;


PRINT N'Creating [dbo].[UC_BvInterviewerProductivityReportTemplate]...';


GO
ALTER TABLE [dbo].[BvInterviewerProductivityReportTemplate]
    ADD CONSTRAINT [UC_BvInterviewerProductivityReportTemplate] UNIQUE NONCLUSTERED ([Name] ASC);



PRINT N'Updating default template...';


GO
UPDATE [dbo].[BvInterviewerProductivityReportTemplate]
SET 
[ReadOnly] = 1,
ColumnData = '<Columns xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
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
</Columns>'
WHERE [Name] = 'Default template'

GO
PRINT N'Update complete.';


GO
