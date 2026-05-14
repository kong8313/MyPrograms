CREATE  PROCEDURE [dbo].[BvSpSurvey_AssignToCallCenter]
        @SurveyId INT,
        @CallCenterId INT
AS
SET NOCOUNT ON

	INSERT INTO BvSurveyAssignmentOnCallCenter(SurveyId, CallCenterId) 
		SELECT @SurveyId, @CallCenterId
			WHERE NOT EXISTS( SELECT 1 FROM BvSurveyAssignmentOnCallCenter WHERE SurveyId = @SurveyId AND CallCenterId = @CallCenterId )

	IF @@ROWCOUNT = 0 
	BEGIN
		RETURN (0)
	END
		
RETURN (0)
