CREATE TABLE [dbo].[BvConversationMessages]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [ConversationId] INT NOT NULL, 
    [Date] DATETIME NOT NULL, 
    [Body] NVARCHAR(1024) NOT NULL, 
    [SenderType] BIT NOT NULL, 
    [SenderId] VARCHAR(64) NOT NULL, 
    [SenderFullName] VARCHAR(130) NOT NULL, 
	CONSTRAINT [PK_ConversationMessages] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC 
	) ON [PRIMARY],
    CONSTRAINT [FK_BvConversationMessage_BvConversation] FOREIGN KEY ([ConversationId]) REFERENCES [BvConversations]([Id]) ON DELETE CASCADE 
)