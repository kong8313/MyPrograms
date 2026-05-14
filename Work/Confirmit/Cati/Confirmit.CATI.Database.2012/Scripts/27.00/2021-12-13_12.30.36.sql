PRINT N'Creating [dbo].[RemoveNonNumericCharacters]...';


GO
CREATE Function [dbo].[RemoveNonNumericCharacters] (@strText VARCHAR(1000))
RETURNS VARCHAR(1000)
AS
BEGIN
    WHILE PATINDEX('%[^0-9]%', @strText) > 0
    BEGIN
        SET @strText = STUFF(@strText, PATINDEX('%[^0-9]%', @strText), 1, '')
    END
    RETURN @strText
END
GO
PRINT N'Altering [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]...';


GO
ALTER FUNCTION [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]( @SurveyId INT, @FromId INT, @ToId INT)
RETURNS TABLE
AS
	RETURN (
	SELECT i.ID FROM BvTelephoneBlacklist p 
			INNER JOIN BvInterview i 
				ON p.Type = 0 AND p.TelephoneNumber = [dbo].RemoveNonNumericCharacters(i.TelephoneNumber) 
			WHERE i.SurveySID = @SurveyId AND p.ID BETWEEN @FromId AND @ToId
	UNION 
	SELECT i.ID FROM BvTelephoneBlacklist p 
			INNER JOIN BvInterview i 
				ON p.Type = 1 AND [dbo].RemoveNonNumericCharacters(i.TelephoneNumber) LIKE p.TelephoneNumber + '%'
				WHERE i.SurveySID = @SurveyId AND p.ID BETWEEN @FromId AND @ToId
	)
GO
PRINT N'Altering [dbo].[BvSpTelephoneBlacklist_Filter]...';


GO
ALTER PROCEDURE [dbo].[BvSpTelephoneBlacklist_Filter]
  @TelephoneNumbers BvStringArrayType READONLY
AS
SELECT t.Value AS TelephoneNumber, f.IsFiltered as IsFiltered 
	FROM @TelephoneNumbers AS t 
	CROSS APPLY [dbo].[BvFnBlacklist_IsTelephoneNumberFiltered]([dbo].RemoveNonNumericCharacters(Value)) AS f
GO
PRINT N'Altering [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]...';


GO
ALTER PROCEDURE [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]
	@FromId INT,
	@ToId INT
AS
BEGIN
	SELECT i.SurveySID FROM BvTelephoneBlacklist p 
		INNER JOIN BvInterview i 
			ON p.Type = 0 AND p.TelephoneNumber = [dbo].RemoveNonNumericCharacters(i.TelephoneNumber) 
		INNER JOIN BvSurvey s 
			ON s.SID = i.SurveySID
		WHERE s.[IsTelephoneBlacklistSupported] = 1 AND p.ID BETWEEN @FromId AND @ToId
	UNION
	SELECT i.SurveySID FROM BvTelephoneBlacklist p 
		INNER JOIN BvInterview i 
			ON p.Type = 1 AND [dbo].RemoveNonNumericCharacters(i.TelephoneNumber) LIKE p.TelephoneNumber + '%'
		INNER JOIN BvSurvey s 
			ON s.SID = i.SurveySID
		WHERE s.[IsTelephoneBlacklistSupported] = 1 AND p.ID BETWEEN @FromId AND @ToId
END
GO
PRINT N'Update complete.';


GO
