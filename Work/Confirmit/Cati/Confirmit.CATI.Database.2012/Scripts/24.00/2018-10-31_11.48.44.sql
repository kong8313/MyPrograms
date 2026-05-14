PRINT N'Creating [dbo].[BvBreakType]...';


GO
CREATE TABLE [dbo].[BvBreakType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (256) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [IsPaid]      BIT            NOT NULL,
    CONSTRAINT [PK_BvBreakType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_BvBreakType] UNIQUE([Name])
);


GO
INSERT INTO BvBreakType (Name, Description, IsPaid) VALUES( 'Break', 'Default break', 1)


GO
PRINT N'Update complete.';

