GO
PRINT N'Creating [dbo].[BvSupervisorsActive]...';


GO
CREATE TABLE [dbo].[BvSupervisorsActive] (
    [UserName]       NVARCHAR (255) NOT NULL,
    [LastActiveTime] DATETIME       NOT NULL,
    [Connections]    INT            NOT NULL,
    [CallCenterId]   INT            NOT NULL,
    CONSTRAINT [PK_BvSupervisorsActive] PRIMARY KEY CLUSTERED ([UserName] ASC)
);


GO
PRINT N'Update complete.';


GO
