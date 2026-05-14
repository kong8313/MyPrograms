GO
PRINT N'Creating [dbo].[BvInterviewerProductivityReportTemplate]...';


GO
CREATE TABLE [dbo].[BvInterviewerProductivityReportTemplate] (
    [Id]                             INT            IDENTITY (1, 1) NOT NULL,
    [Name]                           NVARCHAR (255) NOT NULL,
    [DateCreated]                    DATETIME       NOT NULL,
    [CreatedBy]                      NVARCHAR (255) NOT NULL,
    [LastModified]                   DATETIME       NOT NULL,
    [IsPortrait]                     BIT            NOT NULL,
    [IsDefault]                      BIT            NOT NULL,
    [IncludeZeroValues]              BIT            NOT NULL,
    [IsPublic]                       BIT            NOT NULL,
    [ShowDialerAttempts]             BIT            NOT NULL,
    [IncludeBreakTimeInCalculations] BIT            NOT NULL,
    [ColumnData]                     XML            NOT NULL,
    CONSTRAINT [PK_BvInterviewerProductivityReportTemplate_Id] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BvInterviewerProductivityReportTemplate].[UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]...';

GO
CREATE UNIQUE NONCLUSTERED INDEX [UIXF_BvInterviewerProductivityReportTemplate_IsDefault_filtered]
    ON [dbo].[BvInterviewerProductivityReportTemplate]([IsDefault] ASC) WHERE [IsDefault] = 1;

GO
PRINT N'Creating default template';

GO
INSERT INTO [dbo].[BvInterviewerProductivityReportTemplate] VALUES (
 'Default template',
 '2019-01-01 00:00:00.000',
 'Administrator',
 '2019-01-01 00:00:00.000',
 0,
 1,
 0,
 1,
 0,
 0,
 '<Columns xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>UserId</DisplayName>
    <StandardColumnId>0</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>UserName</DisplayName>
    <StandardColumnId>1</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>LogOnTime</DisplayName>
    <StandardColumnId>2</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>WaitingTimeInHours</DisplayName>
    <StandardColumnId>3</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>PaidBreakTimeInHours</DisplayName>
    <StandardColumnId>4</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>UnpaidBreakTimeInHours</DisplayName>
    <StandardColumnId>5</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>ReviewTimeInHours</DisplayName>
    <StandardColumnId>6</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>Interviews</DisplayName>
    <StandardColumnId>7</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>InterviewsPerLogOnHour</DisplayName>
    <StandardColumnId>8</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>Completes</DisplayName>
    <StandardColumnId>9</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>CompletesPerLogOnHour</DisplayName>
    <StandardColumnId>10</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>InterviewsPerComplete</DisplayName>
    <StandardColumnId>11</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
  <ProductivityReportTemplateBaseColumn xsi:type="ProductivityReportTemplateStandardColumn">
    <DisplayName>AverageCompletedInterviewLengthInMinutes</DisplayName>
    <StandardColumnId>12</StandardColumnId>
  </ProductivityReportTemplateBaseColumn>  
</Columns>')

GO
PRINT N'Update complete.';


GO
