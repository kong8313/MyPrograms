GO
PRINT N'Altering [dbo].[BvConversations]...';


GO
ALTER TABLE [dbo].[BvConversations] DROP COLUMN [TitleForInterviewer];


GO
PRINT N'Refreshing [dbo].[BvSpCleanMessages]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCleanMessages]';


GO
PRINT N'Update complete.';


GO
