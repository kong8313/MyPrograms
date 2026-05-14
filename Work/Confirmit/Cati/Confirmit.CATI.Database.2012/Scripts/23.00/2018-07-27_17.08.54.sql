GO
PRINT N'Dropping [dbo].[DF_BvQuotaBalancing_priority]...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing] DROP CONSTRAINT [DF_BvQuotaBalancing_priority];


GO
PRINT N'Dropping [dbo].[DF_BvQuotaBalancing_promotionCoefficient]...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing] DROP CONSTRAINT [DF_BvQuotaBalancing_promotionCoefficient];


GO
PRINT N'Dropping [dbo].[FK_BvQuotaBalancing_surveyId]...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing] DROP CONSTRAINT [FK_BvQuotaBalancing_surveyId];


GO
PRINT N'Starting rebuilding table [dbo].[BvQuotaBalancing]...';

GO

CREATE TABLE [dbo].[tmp_ms_xx_BvQuotaBalancing] (
    [surveyId]             INT  NOT NULL,
    [quotaId]		   INT NOT NULL,
    [quotaName]            NVARCHAR(256)  NOT NULL,
    [priority]             INT  CONSTRAINT [DF_BvQuotaBalancing_priority] DEFAULT (500) NOT NULL,
    [promotionThreshold]   INT  NOT NULL,
    [promotionCoefficient] REAL CONSTRAINT [DF_BvQuotaBalancing_promotionCoefficient] DEFAULT (0.8) NOT NULL
);

ALTER TABLE [tmp_ms_xx_BvQuotaBalancing] ADD CONSTRAINT [tmp_ms_xx_constraint_PK_BvQuotaBalancing1] PRIMARY KEY CLUSTERED ([surveyId] ASC, [quotaId] ASC)

DROP TABLE [dbo].[BvQuotaBalancing];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvQuotaBalancing]', N'BvQuotaBalancing';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvQuotaBalancing1]', N'PK_BvQuotaBalancing', N'OBJECT';


GO
PRINT N'Creating [dbo].[FK_BvQuotaBalancing_surveyId]...';


GO
ALTER TABLE [dbo].[BvQuotaBalancing] WITH NOCHECK
    ADD CONSTRAINT [FK_BvQuotaBalancing_surveyId] FOREIGN KEY ([surveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Checking existing data against newly created constraints';

GO

GO
ALTER TABLE [dbo].[BvQuotaBalancing] WITH CHECK CHECK CONSTRAINT [FK_BvQuotaBalancing_surveyId];

PRINT N'ALTER TABLE [BvPromotionHistory] DROP COLUMN [quotaId]...';
GO
ALTER TABLE [BvPromotionHistory] DROP COLUMN [quotaId]
GO
PRINT N'ALTER TABLE [BvPromotionHistory] ADD [quotaName] NVARCHAR(256) NOT NULL...';
GO
ALTER TABLE [BvPromotionHistory] ADD [quotaName] NVARCHAR(256) NOT NULL CONSTRAINT DF_BvPromotionHistory_quotaName  DEFAULT ('')
GO
ALTER TABLE [BvPromotionHistory] DROP CONSTRAINT DF_BvPromotionHistory_quotaName
GO

PRINT N'Updating system settings...';
GO
DELETE FROM BvSystemSettings WHERE SystemName = 'QuotaBalancing.TotalPeriodIsSec' OR SystemName = 'QuotaBalancing.MinDelayInSec'

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF ( @DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
  (
      SELECT 'QuotaBalancing.TotalPeriod', 'Promotion period', 'Quotas', 'Total time allotted for running promotion procedure for all ''quota balanced'' surveys (opened surveys with the quota chosen for balancing).', 4, 0, '0.00:15:00.000'
      UNION ALL
      SELECT 'QuotaBalancing.MinDelay', 'Min delay between calls of promotion process', 'Quotas', 'Minimal delay between calls of promotion process.', 4, 0, '0.00:00:10.000'
  )
  INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
  	SELECT * FROM Data
END
GO

GO
PRINT N'Update complete.';


GO
