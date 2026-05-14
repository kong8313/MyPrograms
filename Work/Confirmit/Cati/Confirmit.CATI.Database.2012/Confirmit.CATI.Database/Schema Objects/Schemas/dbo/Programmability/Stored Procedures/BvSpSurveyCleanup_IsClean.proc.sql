CREATE PROCEDURE [dbo].[BvSpSurveyCleanup_IsClean]
    @SurveyId INT
AS
     DECLARE @Cnt INT
     SELECT @Cnt = COUNT(*) FROM BvPersonOrGroupAssignmentOnSurvey WHERE SurveyId = @SurveyId
     
     IF @Cnt <> 0 
     BEGIN
         RETURN 0
     END
      
     SELECT @Cnt = COUNT(*) FROM BvSvySchedule WHERE SurveySid = @SurveyId
 
     IF @Cnt <> 0
     BEGIN
         RETURN 0
     END
     
     RETURN 1