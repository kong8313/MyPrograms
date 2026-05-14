CREATE FUNCTION BvFnPersonOrGroupAssignmentOnSurvey_Get( @CallCenterId INT )
RETURNS TABLE
AS
RETURN 
(
	SELECT  * FROM BvPersonOrGroupAssignmentOnSurvey  a with( nolock ) WHERE CallCenterID = @CallCenterId
)
