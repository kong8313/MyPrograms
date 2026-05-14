PRINT N'Altering [dbo].[BvCallHistory]...';


GO
ALTER TABLE [dbo].[BvCallHistory] ALTER COLUMN [FiredTime] DATETIME NOT NULL;


GO
PRINT N'Update complete.';


GO
