PRINT N'Dropping [dbo].[FK_BvInterviewQuotaCell_SurveyQuotaCell]...';


GO
ALTER TABLE [dbo].[BvInterviewQuotaCell] DROP CONSTRAINT [FK_BvInterviewQuotaCell_SurveyQuotaCell];


GO
PRINT N'Dropping [dbo].[FK_BvSurveyQuotaCell_SurveyQuota]...';


GO
ALTER TABLE [dbo].[BvSurveyQuotaCell] DROP CONSTRAINT [FK_BvSurveyQuotaCell_SurveyQuota];


GO
PRINT N'Starting rebuilding table [dbo].[BvSurveyQuotaCell]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvSurveyQuotaCell] (
    [SurveyID]    INT NOT NULL,
    [QuotaID]     INT NOT NULL,
    [CellID]      INT NOT NULL,
    [Counter]     INT NOT NULL,
    [Limit]       INT NOT NULL,
    [LiveCounter] INT NOT NULL,
    [LiveLimit]   INT NOT NULL,
    [IsDisabled]  BIT NOT NULL,
    [IsOpen]      BIT NOT NULL CONSTRAINT DF_BvSurveyQuotaCell_IsOpen DEFAULT 1,
    [XmlData]     XML NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvSurveyQuotaCell1] PRIMARY KEY CLUSTERED ([SurveyID] ASC, [QuotaID] ASC, [CellID] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvSurveyQuotaCell])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvSurveyQuotaCell] ([SurveyID], [QuotaID], [CellID], [Counter], [Limit], [LiveCounter], [LiveLimit], [IsDisabled], [XmlData])
        SELECT   [SurveyID],
                 [QuotaID],
                 [CellID],
                 [Counter],
                 [Limit],
                 [LiveCounter],
                 [LiveLimit],
                 [IsDisabled],
                 [XmlData]
        FROM     [dbo].[BvSurveyQuotaCell]
        ORDER BY [SurveyID] ASC, [QuotaID] ASC, [CellID] ASC;
    END

DROP TABLE [dbo].[BvSurveyQuotaCell];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvSurveyQuotaCell]', N'BvSurveyQuotaCell';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvSurveyQuotaCell1]', N'PK_BvSurveyQuotaCell', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[FK_BvInterviewQuotaCell_SurveyQuotaCell]...';


GO
ALTER TABLE [dbo].[BvInterviewQuotaCell] WITH NOCHECK
    ADD CONSTRAINT [FK_BvInterviewQuotaCell_SurveyQuotaCell] FOREIGN KEY ([SurveyID], [QuotaID], [CellID]) REFERENCES [dbo].[BvSurveyQuotaCell] ([SurveyID], [QuotaID], [CellID]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvSurveyQuotaCell_SurveyQuota]...';


GO
ALTER TABLE [dbo].[BvSurveyQuotaCell] WITH NOCHECK
    ADD CONSTRAINT [FK_BvSurveyQuotaCell_SurveyQuota] FOREIGN KEY ([SurveyID], [QuotaID]) REFERENCES [dbo].[BvSurveyQuota] ([SurveyID], [QuotaID]) ON DELETE CASCADE;


GO
PRINT N'Refreshing [dbo].[BvSpCall_Activate]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Activate]';


GO
PRINT N'Refreshing [dbo].[BvSpSvySch_Insert]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSvySch_Insert]';


GO
PRINT N'Refreshing [dbo].[BvSpCall_Enable]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCall_Enable]';


GO
PRINT N'Refreshing [dbo].[BvSpPromoteCalls]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPromoteCalls]';


GO
PRINT N'Refreshing [dbo].[BvSpSetCallDeliveryMode]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpSetCallDeliveryMode]';


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [dbo].[BvInterviewQuotaCell] WITH CHECK CHECK CONSTRAINT [FK_BvInterviewQuotaCell_SurveyQuotaCell];

ALTER TABLE [dbo].[BvSurveyQuotaCell] WITH CHECK CHECK CONSTRAINT [FK_BvSurveyQuotaCell_SurveyQuota];


GO
PRINT N'Update complete.';


GO
