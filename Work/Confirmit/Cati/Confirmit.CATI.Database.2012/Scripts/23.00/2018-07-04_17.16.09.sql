PRINT N'Creating [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]...';


GO
CREATE PROCEDURE [dbo].[BvSpTelephoneBlacklist_GetSurveysToDeleteCalls]
	@FromId INT,
	@ToId INT
AS
BEGIN
	SELECT i.SurveySID FROM BvTelephoneBlacklist p 
		INNER JOIN BvInterview i 
			ON p.Type = 0 AND p.TelephoneNumber = i.TelephoneNumber 
		INNER JOIN BvSurvey s 
			ON s.SID = i.SurveySID
		WHERE s.[IsTelephoneBlacklistSupported] = 1 AND p.ID BETWEEN @FromId AND @ToId
	UNION
	SELECT i.SurveySID FROM BvTelephoneBlacklist p 
		INNER JOIN BvInterview i 
			ON p.Type = 1 AND i.TelephoneNumber LIKE p.TelephoneNumber + '%'
		INNER JOIN BvSurvey s 
			ON s.SID = i.SurveySID
		WHERE s.[IsTelephoneBlacklistSupported] = 1 AND p.ID BETWEEN @FromId AND @ToId
END
GO
PRINT N'Update complete.';


GO
