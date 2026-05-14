CREATE VIEW BvViewPersonAndGroup AS
    SELECT  SID, 
		CallCenterID,
        Name, 
        0           IsGroup,
        FullName,
        Description
        FROM    BvPerson
    UNION
    SELECT  BvPersonGroup.SID, 
		0			CallCenterID,
        Name, 
        1           IsGroup,
        ''          FullName,
        ''          Description
    FROM    BvPersonGroup
	UNION
    SELECT  BvAssignmentResource.Id, 
		0			CallCenterID,
        Name, 
        1           IsGroup,
        ''          FullName,
        ''          Description
    FROM    BvAssignmentResource