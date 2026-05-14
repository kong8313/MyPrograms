CREATE PROCEDURE [dbo].[BvSpFilter_Insert]
@Name          NVARCHAR(255),
@Description   NVARCHAR(255),
@AndOrOperator INTEGER,
@SurveySID     INTEGER,
@Hidden        INTEGER
AS
DECLARE @SID INTEGER

    IF NOT EXISTS( SELECT * FROM BvFilters WHERE [Name] = @Name )
    BEGIN
        EXEC @SID = BvSpGetNewSID

        INSERT INTO BvFilters( [SID],
           [Name],
           [Description],
           [AndOrOperator],
           [SurveySID],
           [Hidden])
        VALUES( @SID, @Name, @Description, @AndOrOperator, @SurveySID, @Hidden )
    END
    ELSE
    BEGIN
        -- GetSurveyName
        DECLARE @SurveyName NVARCHAR(255)
        
        SELECT @SurveyName = BvSurvey.[Name]
         FROM BvSurvey, BvFilters
         WHERE BvFilters.[Name] = @Name AND
           BvSurvey.SID = BvFilters.SurveySID

        IF @SurveyName IS NULL
          RAISERROR( N'Filter with such name already exists', 12, 1 )
        ELSE
          RAISERROR( N'The name you entered reserved for "%s" survey', 12, 1, @SurveyName )

        RETURN (-1)
    END

RETURN (@SID)