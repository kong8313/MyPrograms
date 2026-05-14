CREATE TABLE [dbo].[BvConversations]
(
	[Id] INT IDENTITY(1,1) NOT NULL , 
    [TitleForSupervisor] NVARCHAR(255) NOT NULL, 
	[CallCenterId] INT NOT NULL, 
    [Resolved] BIT NOT NULL CONSTRAINT DF_Conversation_Resolved DEFAULT 0, 
    CONSTRAINT [PK_BvConversations] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC 
	) ON [PRIMARY],
    CONSTRAINT [FK_BvConversation_BvCallCenter] FOREIGN KEY ([CallCenterId]) REFERENCES [BvCallCenter]([ID]) ON DELETE CASCADE 	
)