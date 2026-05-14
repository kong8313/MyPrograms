PRINT N'Creating Table [dbo].[BvSearchableFieldsOrdered]...';


GO
CREATE TABLE [dbo].[BvSearchableFieldsOrdered] (
    [SurveyId]    INT            NOT NULL,
    [FieldName]   NVARCHAR (128) NOT NULL,
    [IsSystem]    BIT            NOT NULL,
    [IsEnabled]   BIT            NOT NULL,
    [OrderNumber] INT            NOT NULL,
    CONSTRAINT [PK_BvSearchableFieldsOrdered] PRIMARY KEY CLUSTERED ([SurveyId] ASC, [FieldName] ASC)
);


GO
PRINT N'Creating Foreign Key [dbo].[FK_BvSearchableFieldsOrdered_SurveyId]...';


GO
ALTER TABLE [dbo].[BvSearchableFieldsOrdered] WITH NOCHECK
    ADD CONSTRAINT [FK_BvSearchableFieldsOrdered_SurveyId] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[BvSurvey] ([SID]) ON DELETE CASCADE;


GO
PRINT N'Checking existing data against newly created constraints';


GO
ALTER TABLE [dbo].[BvSearchableFieldsOrdered] WITH CHECK CHECK CONSTRAINT [FK_BvSearchableFieldsOrdered_SurveyId];


GO
PRINT N'Fill BvSearchableFieldsOrdered table for each survey';


GO
-- Temporary table to store Survey IDs
DECLARE @SurveyIds TABLE (Id INT PRIMARY KEY CLUSTERED);
INSERT INTO @SurveyIds (Id)
SELECT DISTINCT [SID] FROM [dbo].[BvSurvey];

-- Temporary table to store system field names
DECLARE @SystemFields TABLE ([Name] NVARCHAR(30) PRIMARY KEY CLUSTERED);
INSERT INTO @SystemFields VALUES 
    ('TelephoneNumber'), ('ExtensionNumber'), ('TimeZoneId'), 
    ('DialType'), ('RespondentName'), ('CallAttemptCount'), ('ITSName'), ('TimeToCall');

-- Main processing loop for each SurveyId
WHILE EXISTS (SELECT 1 FROM @SurveyIds)
BEGIN
    DECLARE @SurveyId INT;
    DECLARE @HiddenSearchableFields NVARCHAR(256);

    -- Get the next SurveyId from the temporary table
    SELECT TOP 1 @SurveyId = Id FROM @SurveyIds ORDER BY Id;

    -- Retrieve HiddenSearchableFields for the current SurveyId
    SELECT TOP 1 @HiddenSearchableFields = HiddenSearchableFields
    FROM [dbo].[BvSurvey]
    WHERE [SID] = @SurveyId;

    -- Determine values for IsItsNameEnabled and IsTimeToCallEnabled
    DECLARE @IsItsNameEnabled INT = CASE WHEN CHARINDEX('ITSName', @HiddenSearchableFields) = 0 THEN 1 ELSE 0 END;
    DECLARE @IsTimeToCallEnabled INT = CASE WHEN CHARINDEX('TimeToCall', @HiddenSearchableFields) = 0 THEN 1 ELSE 0 END;

    -- Insert system fields into BvSearchableFieldsOrdered
    INSERT INTO BvSearchableFieldsOrdered ([SurveyId], [FieldName], [IsSystem], [IsEnabled], [OrderNumber])
    SELECT @SurveyId, 'RespondentName', 1, 1, 0
    UNION ALL
    SELECT @SurveyId, 'TelephoneNumber', 1, 1, 1
	UNION ALL
	SELECT @SurveyId, 'ITSName', 1, @IsItsNameEnabled, 2
    UNION ALL
    SELECT @SurveyId, 'TimeToCall', 1, @IsTimeToCallEnabled, 3;

    -- Insert variable fields into BvSearchableFieldsOrdered
    INSERT INTO [dbo].[BvSearchableFieldsOrdered] ([SurveyId], [FieldName], [IsSystem], [IsEnabled], [OrderNumber])
    SELECT 
        @SurveyId, 
        vf.Name, 
        0, 
        CASE WHEN EXISTS (
            SELECT 1 
            FROM [dbo].[BvSearchableFields]
            WHERE [UseMode] = 0 
              AND [SurveyId] = @SurveyId 
              AND [ColumnId] = vf.ColumnId 
              AND [TableId] = vf.TableId
        ) THEN 1 ELSE 0 END,
        ROW_NUMBER() OVER (ORDER BY vf.Name) + 3 -- Start order number after system fields
    FROM (
        -- Retrieve variable fields that are not system fields
        SELECT DISTINCT rc.ColumnName AS Name, rc.ColumnID AS ColumnId, rc.TableID AS TableId
        FROM [dbo].[BvReplicationColumns] rc
        JOIN [dbo].[BvReplicationTables] rt ON rt.ID = rc.TableID
        WHERE rt.SurveySid = @SurveyId
          AND NOT EXISTS (SELECT 1 FROM @SystemFields WHERE [Name] = rc.ColumnName)
    ) vf;

    -- Remove the processed SurveyId from the temporary table
    DELETE FROM @SurveyIds WHERE Id = @SurveyId;
END;


GO
PRINT N'Update complete.';


GO
