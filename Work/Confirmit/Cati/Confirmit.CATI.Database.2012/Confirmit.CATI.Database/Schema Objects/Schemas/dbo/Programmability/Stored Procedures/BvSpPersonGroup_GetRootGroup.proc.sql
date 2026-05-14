CREATE PROCEDURE [dbo].[BvSpPersonGroup_GetRootGroup]
AS
	SELECT pg.SID
	FROM BvPersonGroup pg
	LEFT JOIN BvMembership m ON pg.Sid = m.ObjectSID
	WHERE m.ObjectSID IS NULL
RETURN 0