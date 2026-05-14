GO
PRINT N'Creating [dbo].[BvIntArrayType]...';


GO
CREATE TYPE [dbo].[BvIntArrayType] AS TABLE (
    [Value] INT NOT NULL);


GO
PRINT N'Creating [dbo].[BvStringArrayType]...';


GO
CREATE TYPE [dbo].[BvStringArrayType] AS TABLE (
    [Value] NVARCHAR (MAX) NULL);


GO
PRINT N'Update complete.';


GO
