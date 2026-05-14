CREATE PROCEDURE [dbo].[BvSpCallCenter_ListOfAssignedToSurvey]
	@SurveyId INT
AS
	SELECT cs.* FROM BvSurveyAssignmentOnCallCenter a 
		INNER JOIN BvCallCenter cs
		ON a.CallCenterId = cs.ID
		WHERE a.SurveyId = @SurveyId

	RETURN(0)