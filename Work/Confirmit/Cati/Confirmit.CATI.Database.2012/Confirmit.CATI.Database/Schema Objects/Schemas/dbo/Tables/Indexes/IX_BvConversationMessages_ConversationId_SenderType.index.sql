CREATE INDEX [IX_BvConversationMessages_ConversationId_SenderType] ON [dbo].[BvConversationMessages] (
	[ConversationId] ASC,
	[SenderType] ASC
) ON [PRIMARY]