CREATE PROCEDURE [dbo].[BvSpAssignmentResource_Insert]
@Qualifier VARCHAR(900)
AS
SET NOCOUNT ON

DECLARE @Persons TABLE( ID INT)
DECLARE @ID INT = NULL

SELECT @ID = ID FROM BvAssignmentResource WHERE Qualifier = @Qualifier

IF @ID IS NOT NULL 
BEGIN
	SELECT * FROM @Persons
	RETURN @ID
END

DECLARE @Resources TABLE( ID INT) 
INSERT INTO @Resources SELECT Item FROM dbo.utilSplitNumbers(@Qualifier, ',')

DECLARE @Name NVARCHAR(MAX) = ''

SELECT @Name = @Name + Name + ',' FROM @Resources r LEFT JOIN BvViewPersonAndGroup pg ON r.ID = pg.SID ORDER BY r.ID

SET @Name = SUBSTRING(@Name, 0, LEN(@Name))

EXEC @ID = BvSpGetNewSID

INSERT INTO BvAssignmentResource( ID, Name, Qualifier ) VALUES( @ID, @Name, @Qualifier )

IF @@ROWCOUNT > 0
BEGIN 
	INSERT INTO BvAssignmentResourceItem(AssignmentID, ResourceID) SELECT @ID, ID FROM @Resources 
	DECLARE @Size INT = @@ROWCOUNT
	INSERT INTO @Persons 
		SELECT pr.PersonSID FROM BvAssignmentResourceItem ari LEFT JOIN BvPersonRel pr ON pr.ObjectSID = ari.ResourceID 
		WHERE ari.AssignmentID = @ID
		GROUP BY pr.PersonSID HAVING COUNT(*) = @Size

	INSERT INTO BvPersonRel( PersonSID, ObjectSID, RoleID, Type ) SELECT ID, @ID, 2, 1 FROM @Persons
END
ELSE
BEGIN
    SELECT @ID = ID FROM BvAssignmentResource WHERE Qualifier = @Qualifier
END

SELECT * FROM @Persons

RETURN @ID