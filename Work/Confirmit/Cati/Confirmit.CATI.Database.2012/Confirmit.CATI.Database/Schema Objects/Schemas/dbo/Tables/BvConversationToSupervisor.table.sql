CREATE TABLE [dbo].[BvConversationToSupervisor]
(
	[SupervisorName] VARCHAR(64) NOT NULL , 
    [ConversationId] INT NOT NULL, 
    [LastReadMessageId] INT NULL, 
    [IsSubscribed] BIT NOT NULL, 
    CONSTRAINT [PK_BvConversationToSupervisor] PRIMARY KEY CLUSTERED 
	(
		[SupervisorName] ASC,
		[ConversationId] ASC	
	) ON [PRIMARY],
    CONSTRAINT [FK_BvConversationToSupervisor_BvConversations] FOREIGN KEY ([ConversationId]) REFERENCES [BvConversations]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_BvConversationToSupervisor_BvMessages] FOREIGN KEY ([LastReadMessageId]) REFERENCES [BvConversationMessages]([Id])
)
