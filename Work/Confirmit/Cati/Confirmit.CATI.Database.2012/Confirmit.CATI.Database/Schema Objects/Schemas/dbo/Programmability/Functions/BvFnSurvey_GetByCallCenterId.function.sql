CREATE FUNCTION [dbo].[BvFnSurvey_GetByCallCenterId]
(
	@CallCenterId int
)
RETURNS TABLE
AS
RETURN
(	
	SELECT s.* 
	FROM [BvSurvey]  s LEFT JOIN [BvSurveyAssignmentOnCallCenter] sa ON s.SID = sa.SurveyId AND sa.CallCenterId = @CallCenterID 
	WHERE @CallCenterID IS NULL OR sa.CallCenterId IS NOT NULL
)
