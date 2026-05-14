GO
PRINT N'Creating [dbo].[BvConversationMessages]...';


GO
CREATE TABLE [dbo].[BvConversationMessages] (
    [Id]             INT             IDENTITY (1, 1) NOT NULL,
    [ConversationId] INT             NOT NULL,
    [Date]           DATETIME        NOT NULL,
    [Body]           NVARCHAR (1024) NOT NULL,
    [SenderType]     BIT             NOT NULL,
    [SenderId]       VARCHAR (64)    NOT NULL,
    [SenderFullName] VARCHAR (130)   NOT NULL,
    CONSTRAINT [PK_ConversationMessages] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[BvConversationMessages].[IX_BvConversationMessages_ConversationId_SenderType]...';


GO
CREATE NONCLUSTERED INDEX [IX_BvConversationMessages_ConversationId_SenderType]
    ON [dbo].[BvConversationMessages]([ConversationId] ASC, [SenderType] ASC)
    ON [PRIMARY];


GO
PRINT N'Creating [dbo].[BvConversations]...';


GO
CREATE TABLE [dbo].[BvConversations] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [TitleForSupervisor]  NVARCHAR (255) NOT NULL,
    [TitleForInterviewer] NVARCHAR (255) NOT NULL,
    [CallCenterId]        INT            NOT NULL,
    CONSTRAINT [PK_BvConversations] PRIMARY KEY CLUSTERED ([Id] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[BvConversationToInterviewer]...';


GO
CREATE TABLE [dbo].[BvConversationToInterviewer] (
    [InterviewerId]     INT NOT NULL,
    [ConversationId]    INT NOT NULL,
    [LastReadMessageId] INT NULL,
    CONSTRAINT [PK_BvConversationToInterviewer] PRIMARY KEY CLUSTERED ([InterviewerId] ASC, [ConversationId] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[BvConversationToSupervisor]...';


GO
CREATE TABLE [dbo].[BvConversationToSupervisor] (
    [SupervisorName]    VARCHAR (64) NOT NULL,
    [ConversationId]    INT          NOT NULL,
    [LastReadMessageId] INT          NULL,
    [IsSubscribed]      BIT          NOT NULL,
    CONSTRAINT [PK_BvConversationToSupervisor] PRIMARY KEY CLUSTERED ([SupervisorName] ASC, [ConversationId] ASC) ON [PRIMARY]
);


GO
PRINT N'Creating [dbo].[FK_BvConversationMessage_BvConversation]...';


GO
ALTER TABLE [dbo].[BvConversationMessages] WITH NOCHECK
    ADD CONSTRAINT [FK_BvConversationMessage_BvConversation] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[BvConversations] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvConversation_BvCallCenter]...';


GO
ALTER TABLE [dbo].[BvConversations] WITH NOCHECK
    ADD CONSTRAINT [FK_BvConversation_BvCallCenter] FOREIGN KEY ([CallCenterId]) REFERENCES [dbo].[BvCallCenter] ([ID]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvConversationToInterviewer_BvConversations]...';


GO
ALTER TABLE [dbo].[BvConversationToInterviewer] WITH NOCHECK
    ADD CONSTRAINT [FK_BvConversationToInterviewer_BvConversations] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[BvConversations] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Creating [dbo].[FK_BvConversationToInterviewer_BvMessages]...';


GO
ALTER TABLE [dbo].[BvConversationToInterviewer] WITH NOCHECK
    ADD CONSTRAINT [FK_BvConversationToInterviewer_BvMessages] FOREIGN KEY ([LastReadMessageId]) REFERENCES [dbo].[BvConversationMessages] ([Id]);


GO
PRINT N'Creating [dbo].[FK_BvConversationToSupervisor_BvMessages]...';


GO
ALTER TABLE [dbo].[BvConversationToSupervisor] WITH NOCHECK
    ADD CONSTRAINT [FK_BvConversationToSupervisor_BvMessages] FOREIGN KEY ([LastReadMessageId]) REFERENCES [dbo].[BvConversationMessages] ([Id]);


GO
PRINT N'Creating [dbo].[FK_BvConversationToSupervisor_BvConversations]...';


GO
ALTER TABLE [dbo].[BvConversationToSupervisor] WITH NOCHECK
    ADD CONSTRAINT [FK_BvConversationToSupervisor_BvConversations] FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[BvConversations] ([Id]) ON DELETE CASCADE;


GO
PRINT N'Altering [dbo].[BvSpCleanMessages]...';


GO
ALTER PROCEDURE [dbo].[BvSpCleanMessages]
@ExpirationPeriod INT
AS
BEGIN

	DELETE from bvMessages
	WHERE DateAdd(day, @ExpirationPeriod, bvMessages.CreateTime) < GETUTCDATE();
 
	WITH LastMessagesInChat AS (
		SELECT [ConversationId], MAX([Date]) as [Date]
		FROM BvConversationMessages
        GROUP BY [ConversationId])
		
	DELETE 
	FROM BvConversations 
	WHERE Id IN (									 
		SELECT ConversationId
		FROM LastMessagesInChat
		WHERE DateAdd(day, @ExpirationPeriod, [Date]) < GETUTCDATE());

END
GO
PRINT N'Checking existing data against newly created constraints';


GO



GO
ALTER TABLE [dbo].[BvConversationMessages] WITH CHECK CHECK CONSTRAINT [FK_BvConversationMessage_BvConversation];

ALTER TABLE [dbo].[BvConversations] WITH CHECK CHECK CONSTRAINT [FK_BvConversation_BvCallCenter];

ALTER TABLE [dbo].[BvConversationToInterviewer] WITH CHECK CHECK CONSTRAINT [FK_BvConversationToInterviewer_BvConversations];

ALTER TABLE [dbo].[BvConversationToInterviewer] WITH CHECK CHECK CONSTRAINT [FK_BvConversationToInterviewer_BvMessages];

ALTER TABLE [dbo].[BvConversationToSupervisor] WITH CHECK CHECK CONSTRAINT [FK_BvConversationToSupervisor_BvMessages];

ALTER TABLE [dbo].[BvConversationToSupervisor] WITH CHECK CHECK CONSTRAINT [FK_BvConversationToSupervisor_BvConversations];


GO
PRINT N'Update complete.';


GO
