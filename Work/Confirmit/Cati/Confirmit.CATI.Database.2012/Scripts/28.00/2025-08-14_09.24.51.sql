GO
PRINT N'Creating Table [dbo].[BvHistoryCustomFields]...';


GO
CREATE TABLE [dbo].[BvHistoryCustomFields] (
    [Id]              INT            NOT NULL,
    [SourceTable]     INT            NOT NULL,
    [SourceFieldName] NVARCHAR (50)  NOT NULL,
    [DisplayName]     NVARCHAR (50)  NULL,
    [Description]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Update complete.';


GO
