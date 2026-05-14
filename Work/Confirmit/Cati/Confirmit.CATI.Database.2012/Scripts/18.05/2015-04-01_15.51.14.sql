GO
PRINT N'Altering [dbo].[BvDialers]...';


GO
ALTER TABLE [dbo].[BvDialers]
    ADD [WhiteList] NVARCHAR (MAX) CONSTRAINT [DF_BvDialers_WhiteList] DEFAULT (NULL) NULL;


GO
PRINT N'Update complete.';


GO
