GO
PRINT N'Dropping PK_BvTelephoneBlacklist...';


GO
ALTER TABLE [dbo].[BvTelephoneBlacklist] DROP CONSTRAINT [PK_BvTelephoneBlacklist];


GO
PRINT N'Starting rebuilding table [dbo].[BvTelephoneBlacklist]...';


GO
BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvTelephoneBlacklist] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [TelephoneNumber] VARCHAR (255) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvTelephoneBlacklist] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvTelephoneBlacklist])
    BEGIN
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvTelephoneBlacklist] ON;
        INSERT INTO [dbo].[tmp_ms_xx_BvTelephoneBlacklist] ([Id], [TelephoneNumber])
        SELECT   [Id],
                 [TelephoneNumber]
        FROM     [dbo].[BvTelephoneBlacklist]
        ORDER BY [Id] ASC;
        SET IDENTITY_INSERT [dbo].[tmp_ms_xx_BvTelephoneBlacklist] OFF;
    END

DROP TABLE [dbo].[BvTelephoneBlacklist];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvTelephoneBlacklist]', N'BvTelephoneBlacklist';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvTelephoneBlacklist]', N'PK_BvTelephoneBlacklist', N'OBJECT';

COMMIT TRANSACTION;

SET TRANSACTION ISOLATION LEVEL READ COMMITTED;


GO
PRINT N'Creating [dbo].[BvTelephoneBlacklist].[IX_BvTelephoneBlacklist]...';


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_BvTelephoneBlacklist]
    ON [dbo].[BvTelephoneBlacklist]([TelephoneNumber] ASC) WITH (IGNORE_DUP_KEY = ON)
    ON [PRIMARY];


GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
