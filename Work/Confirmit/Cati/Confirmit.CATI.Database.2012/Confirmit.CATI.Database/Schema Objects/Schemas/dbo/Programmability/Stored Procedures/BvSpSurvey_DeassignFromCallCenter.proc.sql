CREATE  PROCEDURE [dbo].[BvSpSurvey_DeassignFromCallCenter]
        @SurveyId INT,
        @CallCenterId INT
AS
SET NOCOUNT ON

	SELECT SID INTO #DeasiggnedExplicitSIDs FROM BvPerson WHERE CallCenterID = @CallCenterId

	DELETE FROM BvSurveyAssignmentOnCallCenter 
		WHERE SurveyId = @SurveyId AND CallCenterId = @CallCenterId

	IF @@ROWCOUNT = 0 
	BEGIN
		RETURN (0)
	END

	DELETE FROM BvPersonOrGroupAssignmentOnSurvey
		WHERE SurveyId = @SurveyId AND CallCenterID = @CallCenterId

	DELETE BvPersonRel 
		WHERE ObjectSID = @SurveyId AND Type = 2 AND PersonSID IN ( SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterId )

	DELETE BvLoginGroup 
		WHERE SurveySID = @SurveyId AND PersonSID IN ( SELECT SID FROM BvPerson WHERE CallCenterID = @CallCenterId )

	UPDATE BvSvySchedule 
		SET ExplicitSID = @SurveyId
		FROM #DeasiggnedExplicitSIDs d
		WHERE SurveySID = @SurveyId AND ExplicitSID = d.SID

RETURN (0)
