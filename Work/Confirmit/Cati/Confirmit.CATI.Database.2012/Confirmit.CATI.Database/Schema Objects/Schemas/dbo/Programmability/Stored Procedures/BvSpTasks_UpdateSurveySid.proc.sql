CREATE PROCEDURE [dbo].[BvSpTasks_UpdateSurveySid]
 @PersonSID int,
 @SurveySID int
AS

UPDATE [dbo].[BvTasks]
    SET SurveySID = @SurveySID,
		SelectedSurveyId = @SurveySID
WHERE PersonSID = @PersonSID

RETURN 0