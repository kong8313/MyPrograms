CREATE PROCEDURE [dbo].[BvSpPerson_ListWithTasksByType]
	@Type INT
AS
	SELECT * FROM BvPerson p
	LEFT JOIN BvTasks t ON p.SID = t.PersonSID
	WHERE p.Type = @Type