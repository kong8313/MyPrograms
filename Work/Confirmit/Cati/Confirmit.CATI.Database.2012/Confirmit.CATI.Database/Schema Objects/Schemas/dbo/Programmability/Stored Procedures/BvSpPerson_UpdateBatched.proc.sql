CREATE PROCEDURE [dbo].[BvSpPerson_UpdateBatched]
    @Qualifier VARCHAR(900),
    @DialTypeId tinyint
AS

SET NOCOUNT ON

DECLARE @Persons TABLE( ID INT) 
INSERT INTO @Persons SELECT Item FROM dbo.utilSplitNumbers(@Qualifier, ',')

UPDATE BvPerson
SET DialTypeId = @DialTypeId
WHERE SID IN
(
	SELECT pg.SID FROM @Persons persons inner JOIN BvViewPersonAndGroup pg ON persons.ID = pg.SID
	WHERE IsGroup = 0
	UNION ALL
	SELECT p.SID FROM @Persons persons 
	INNER JOIN BvViewPersonAndGroup pg ON persons.ID = pg.SID
	INNER JOIN BvMembership on pg.SID = BvMembership.ContainerSID
	INNER join BvPerson p on p.SID = BvMembership.ObjectSID
	WHERE IsGroup = 1
)