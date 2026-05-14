GO
PRINT N'Creating [dbo].[BvDialerFeatures]...';


GO
CREATE TABLE [dbo].[BvDialerFeatures] (
    [DialerId] INT            NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [Value]    BIT            NOT NULL,
    CONSTRAINT [PK_BvDialerFeatures] PRIMARY KEY CLUSTERED ([DialerId] ASC, [Name] ASC)
);


GO
PRINT N'Update complete.';


GO
