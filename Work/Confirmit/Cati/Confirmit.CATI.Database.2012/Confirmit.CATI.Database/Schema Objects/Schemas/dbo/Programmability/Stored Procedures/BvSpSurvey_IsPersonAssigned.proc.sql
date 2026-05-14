CREATE PROCEDURE [dbo].[BvSpSurvey_IsPersonAssigned]
    @SurveySID INT,
 @PersonSID INT
AS
  
SELECT a.Id from dbo.BvPersonOrGroupAssignmentOnSurvey a 
 WHERE PersonOrGroupId = @PersonSID and SurveyId = @SurveySID

return (0)