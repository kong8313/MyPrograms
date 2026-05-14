GO
PRINT N'Altering Table [dbo].[BvDialers]...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD [Features] NVARCHAR (MAX) NULL;


GO
PRINT N'Update complete.';


GO
