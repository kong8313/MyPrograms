CREATE  PROCEDURE [dbo].[BvSpSurvey_GetCountOfLoggedPerson]
        @SurveyId INT,
        @CallCenterId INT,
		@TaskChoiceMode INT
AS
SET NOCOUNT ON

	SELECT COUNT(*) FROM BvTasks t
		INNER JOIN BvPerson p
		ON t.PersonSID = p.SID
		WHERE (t.SurveySID = @SurveyId OR @SurveyId = 0 ) AND t.CallCenterID = @CallCenterId AND (p.ManualSelection = @TaskChoiceMode OR @TaskChoiceMode = -1)
RETURN (0)
