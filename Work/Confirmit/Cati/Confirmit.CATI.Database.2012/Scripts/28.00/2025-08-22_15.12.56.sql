GO
PRINT N'Starting rebuilding table [dbo].[BvHistoryCustomFields]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvHistoryCustomFields] (
    [Id]              INT            NOT NULL,
    [SourceTable]     INT            NOT NULL,
    [SourceFieldName] NVARCHAR (50)  NOT NULL,
    [DisplayName]     NVARCHAR (50)  NULL,
    [Description]     NVARCHAR (255) NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvHistoryCustomFields1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvHistoryCustomFields])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvHistoryCustomFields] ([Id], [SourceTable], [SourceFieldName], [DisplayName], [Description])
        SELECT   [Id],
                 [SourceTable],
                 [SourceFieldName],
                 [DisplayName],
                 [Description]
        FROM     [dbo].[BvHistoryCustomFields]
        ORDER BY [Id] ASC;
    END

DROP TABLE [dbo].[BvHistoryCustomFields];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvHistoryCustomFields]', N'BvHistoryCustomFields';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvHistoryCustomFields1]', N'PK_BvHistoryCustomFields', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Update complete.';


GO
