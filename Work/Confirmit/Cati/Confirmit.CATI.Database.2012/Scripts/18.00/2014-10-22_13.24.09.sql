GO
PRINT N'Creating [dbo].[BvTelephoneBlacklistIdSequence]...';


GO
CREATE SEQUENCE [dbo].[BvTelephoneBlacklistIdSequence]
    AS INT
    START WITH 1
    INCREMENT BY 1;


GO

PRINT N'Starting altering sequence [dbo].[BvTelephoneBlacklistIdSequence]...';


GO

DECLARE @MaxValue INT = ISNULL( ( SELECT MAX(ID) FROM [dbo].[BvTelephoneBlacklist] ), 0 )
DECLARE @Query NVARCHAR(MAX) = 'ALTER SEQUENCE [dbo].[BvTelephoneBlacklistIdSequence]
RESTART WITH ' + CAST( @MaxValue + 1 AS NVARCHAR(MAX))
EXEC (@Query)
GO

PRINT N'Starting rebuilding table [dbo].[BvTelephoneBlacklist]...';


GO

BEGIN TRANSACTION;

SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

SET XACT_ABORT ON;

CREATE TABLE [dbo].[tmp_ms_xx_BvTelephoneBlacklist] (
    [Id]              INT           CONSTRAINT [DF_BvTelephoneBlacklist_Id] DEFAULT  NEXT VALUE FOR [dbo].[BvTelephoneBlacklistIdSequence] NOT NULL,
    [TelephoneNumber] VARCHAR (255) NOT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvTelephoneBlacklist] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvTelephoneBlacklist])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvTelephoneBlacklist] ([Id], [TelephoneNumber])
        SELECT   [Id],
                 [TelephoneNumber]
        FROM     [dbo].[BvTelephoneBlacklist]
        ORDER BY [Id] ASC;
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
PRINT N'Creating [dbo].[BvSpTelephoneBlacklist_Insert]...';


GO
CREATE PROCEDURE [dbo].[BvSpTelephoneBlacklist_Insert]
@TelephoneNumber varchar(255)
AS
SET NOCOUNT ON

INSERT INTO [dbo].[BvTelephoneBlacklist]
([TelephoneNumber])
VALUES
(@TelephoneNumber)
GO
PRINT N'Update complete.';


GO
