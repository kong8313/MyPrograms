GO
PRINT N'Dropping [dbo].[BvSpSvyShedule_DeleteCallsByBlacklist]...';


GO
DROP PROCEDURE [dbo].[BvSpSvyShedule_DeleteCallsByBlacklist];


GO
PRINT N'Creating [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]...';


GO
CREATE FUNCTION [dbo].[BvFnBlacklist_GetInterviewIdsForBlacklistRange]( @SurveyId INT, @FromId INT, @ToId INT)
RETURNS TABLE
AS
	RETURN (
	SELECT i.ID FROM BvTelephoneBlacklist p 
			INNER JOIN BvInterview i 
				ON p.Type = 0 AND p.TelephoneNumber = i.TelephoneNumber 
			WHERE i.SurveySID = @SurveyId AND p.ID BETWEEN @FromId AND @ToId
	UNION 
	SELECT i.ID FROM BvTelephoneBlacklist p 
			INNER JOIN BvInterview i 
				ON p.Type = 1 AND i.TelephoneNumber LIKE p.TelephoneNumber + '%'
				WHERE i.SurveySID = @SurveyId AND p.ID BETWEEN @FromId AND @ToId
	)
GO
PRINT N'Altering [dbo].[BvSpTelephoneBlacklist_Insert]...';


GO
ALTER PROCEDURE [dbo].[BvSpTelephoneBlacklist_Insert]
@Type TINYINT,
@TelephoneNumber varchar(255)
AS
SET NOCOUNT ON

DECLARE @ID TABLE( ID INT )

INSERT INTO [dbo].[BvTelephoneBlacklist]([Type], [TelephoneNumber])
	OUTPUT inserted.ID INTO @ID
    VALUES (@Type, @TelephoneNumber)

RETURN ISNULL(( SELECT ID FROM @ID ), 0)
GO
PRINT N'Update complete.';


GO
