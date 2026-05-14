CREATE TABLE [dbo].[BvConversationToInterviewer]
(
    [InterviewerId] INT NOT NULL, 
    [ConversationId] INT NOT NULL, 
    [LastReadMessageId] INT NULL,
    CONSTRAINT [PK_BvConversationToInterviewer] PRIMARY KEY CLUSTERED 
	(
		[InterviewerId] ASC,
		[ConversationId] ASC	
	) ON [PRIMARY],
    CONSTRAINT [FK_BvConversationToInterviewer_BvConversations] FOREIGN KEY ([ConversationId]) REFERENCES [BvConversations]([Id]) ON DELETE CASCADE, 
    CONSTRAINT [FK_BvConversationToInterviewer_BvMessages] FOREIGN KEY ([LastReadMessageId]) REFERENCES [BvConversationMessages]([Id])
)
