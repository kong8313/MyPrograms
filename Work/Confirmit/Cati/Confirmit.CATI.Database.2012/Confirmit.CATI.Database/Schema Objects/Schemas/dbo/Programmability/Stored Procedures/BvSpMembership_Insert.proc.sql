CREATE PROCEDURE [dbo].[BvSpMembership_Insert]
        @ContainerSID int,
        @ObjectSID int
AS

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvMembership
    WHERE   ContainerSID = @ContainerSID
    AND ObjectSID = @ObjectSID

IF @Rows <> 0
BEGIN
	RAISERROR( 'Duplicated object', 16, 1)
    RETURN -1
END

INSERT  BvMembership( 
        ContainerSID, 
        ObjectSID ) 
    VALUES( 
        @ContainerSID, 
        @ObjectSID )