CREATE PROCEDURE [dbo].[BvSpShiftType_Update]
        @OwnerSID int,
        @OldID int,
        @NewID int,
        @Name nvarchar(255),
        @Color int,
        @Mode int,
        @ObjectID int
AS
DECLARE @Rows int

SELECT  @Rows = COUNT(*), @ObjectID = MIN( ObjectID )
    FROM    BvShiftType
    WHERE   ID = @OldID
    AND OwnerSID = @OwnerSID
    
IF @Rows = 0
BEGIN
	RAISERROR('Shift type with ID = %i and OwnerSID = %i not found', 16, 1, @OldID, @OwnerSID)
	RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR('Multiple shift types with ID = %i and OwnerSID = %i found', 16, 1, @OldID, @OwnerSID)
    RETURN -1
END
    
IF ( @OldID <> @NewID ) 
BEGIN

    SELECT  @Rows = COUNT(*)
        FROM    BvShiftType
        WHERE   ID = @NewID
        AND OwnerSID = @OwnerSID

    IF @Rows <> 0
	BEGIN
		RAISERROR('Shift type with ID = %i and OwnerSID = %i  already exists', 16, 1, @NewID, @OwnerSID)
		RETURN -1
	END

    UPDATE  BvShiftType
        SET ID = @NewID,
            Name = @Name,
            Color = @Color
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END
ELSE BEGIN
    UPDATE  BvShiftType
        SET Name = @Name,
            Color = @Color
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END
RETURN ( 0 )