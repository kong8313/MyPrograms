UPDATE BvPerson
SET CallCenterID = 0
FROM BvPerson p
INNER JOIN BvMembership ms
ON p.SID = ms.ObjectSID 
INNER JOIN BvPersonGroup pg
ON ms.ContainerSID = pg.SID
WHERE pg.RoleID = 64

GO
PRINT N'Update complete.';


GO
