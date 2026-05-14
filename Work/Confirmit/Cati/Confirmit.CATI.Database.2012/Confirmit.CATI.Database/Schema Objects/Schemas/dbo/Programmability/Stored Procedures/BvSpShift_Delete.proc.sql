CREATE PROCEDURE [dbo].[BvSpShift_Delete]
        @OwnerSID int,
        @ID int,
        @Mode int

AS
DECLARE @Rows int

SELECT  @Rows = COUNT( * )
    FROM    BvShift
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID
    
IF @Rows = 0
BEGIN
    RAISERROR( 'Shift with ID = %i and OwnerSID = %i not found', 16, 1, @ID, @OwnerSID)
    RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR( 'Multiple shifts with ID = %i and OwnerSID = %i found', 16, 1, @ID,@OwnerSID )
    RETURN -1
END
    
DELETE  BvTimezoneShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ID
DELETE  BvShift
    WHERE   OwnerSID = @OwnerSID
    AND ID = @ID
return 0