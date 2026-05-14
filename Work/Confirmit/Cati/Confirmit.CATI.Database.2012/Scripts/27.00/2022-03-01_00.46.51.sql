GO
PRINT N'Altering [dbo].[BvConversations]...';


GO
ALTER TABLE [dbo].[BvConversations]
    ADD [Resolved] BIT CONSTRAINT [DF_Conversation_Resolved] DEFAULT 0 NOT NULL;


GO
PRINT N'Refreshing [dbo].[BvSpCleanMessages]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpCleanMessages]';


GO
PRINT N'Update complete.';


GO
