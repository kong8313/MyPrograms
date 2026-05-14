PRINT N'Creating [dbo].[BvCallTransferSessions]...';


GO
CREATE TABLE [dbo].[BvCallTransferSessions] (
    [CallId]     INT NOT NULL,    
    [TransferId] NVARCHAR(512) NOT NULL,
    CONSTRAINT [PK_BvCallTransferSessions_CallId] PRIMARY KEY CLUSTERED ([CallId])
);

GO
CREATE INDEX [IX_BvCallTransferSessions_TransferId] ON [dbo].[BvCallTransferSessions]([TransferId])

GO
PRINT N'Update complete.';

