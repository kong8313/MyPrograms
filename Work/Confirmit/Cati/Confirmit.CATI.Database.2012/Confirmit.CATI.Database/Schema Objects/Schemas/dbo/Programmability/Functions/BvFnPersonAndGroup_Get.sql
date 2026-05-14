CREATE FUNCTION [dbo].[BvFnPersonAndGroup_Get]
(
	@CallCenterId int
)
RETURNS TABLE
AS
RETURN
(
	SELECT  
	    SID, 
		CallCenterID,
        Name, 
        0 as IsGroup
    FROM BvPerson
    WHERE CallCenterID = @CallCenterId
    UNION
    SELECT  
	    BvPersonGroup.SID, 
		0 as CallCenterID,
        Name, 
        1 as IsGroup
    FROM BvPersonGroup
	UNION
    SELECT  
	    BvAssignmentResource.Id, 
		0 as CallCenterID,
        Name, 
        1 as IsGroup
    FROM BvAssignmentResource
)
