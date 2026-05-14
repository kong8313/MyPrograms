CREATE PROCEDURE [dbo].[BvSpSurvey_Shutdown]
    @SurveyId INT
AS
    UPDATE BvSvySchedule 
		SET CallState = 2 
		WHERE SurveySID = @SurveyId AND CallState BETWEEN -2 AND -1