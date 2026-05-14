CREATE TABLE [dbo].[BvVersionHistory]
(
    [Id] INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_BvVersionHistory_Id PRIMARY KEY, 
    [Major] INT NOT NULL, 
    [Minor] INT NOT NULL, 
    [BranchName] NVARCHAR(MAX) NOT NULL, 
    [ScriptNumber] INT NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [ScriptAppliedDate] DATETIME NOT NULL, 
    [Duration] INT NOT NULL, 
    [ScriptText] NVARCHAR(MAX) NOT NULL, 
    [ScriptOutput] NVARCHAR(MAX) NOT NULL, 
    [IsAppliedDuringDBCreation] BIT NOT NULL,
    [DbUpateUtilityVersion] NVARCHAR(MAX) NOT NULL,
    [ActiveUser]  NVARCHAR(MAX) NOT NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Time in milliseconds took to apply the script',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'BvVersionHistory',
    @level2type = N'COLUMN',
    @level2name = N'Duration'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The description from ScriptDefinitionFile',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'BvVersionHistory',
    @level2type = N'COLUMN',
    @level2name = N'Description'