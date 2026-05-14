CREATE FUNCTION BvFnPerson_Get( @CallCenterId INT )
RETURNS TABLE
AS
RETURN 
(
	SELECT  * FROM BvPerson  a with( nolock ) WHERE @CallCenterId = 0 OR CallCenterID = @CallCenterId
)
