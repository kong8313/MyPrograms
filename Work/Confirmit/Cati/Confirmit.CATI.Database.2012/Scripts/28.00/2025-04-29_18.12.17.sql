PRINT N'Dropping Default Constraint [dbo].[DF_BvSearchableFields_UseMode]...';


GO
ALTER TABLE [dbo].[BvSearchableFields] DROP CONSTRAINT [DF_BvSearchableFields_UseMode];


GO
PRINT N'Starting rebuilding table [dbo].[BvSearchableFields]...';


GO
CREATE TABLE [dbo].[tmp_ms_xx_BvSearchableFields] (
    [SurveyId] INT NOT NULL,
    [ColumnId] INT NOT NULL,
    [TableId]  INT NOT NULL,
    [UseMode]  INT NULL,
    CONSTRAINT [tmp_ms_xx_constraint_PK_BvSearchableFields1] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [ColumnId] ASC, [TableId] ASC)
);

IF EXISTS (SELECT TOP 1 1 
           FROM   [dbo].[BvSearchableFields])
    BEGIN
        INSERT INTO [dbo].[tmp_ms_xx_BvSearchableFields] ([SurveyId], [ColumnId], [TableId], [UseMode])
        SELECT   [SurveyId],
                 [ColumnId],
                 [TableId],
                 [UseMode]
        FROM     [dbo].[BvSearchableFields]
        ORDER BY [SurveyId] ASC, [ColumnId] ASC, [TableId] ASC;
    END

DROP TABLE [dbo].[BvSearchableFields];

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_BvSearchableFields]', N'BvSearchableFields';

EXECUTE sp_rename N'[dbo].[tmp_ms_xx_constraint_PK_BvSearchableFields1]', N'PK_BvSearchableFields', N'OBJECT';


GO
PRINT N'Update complete.';
