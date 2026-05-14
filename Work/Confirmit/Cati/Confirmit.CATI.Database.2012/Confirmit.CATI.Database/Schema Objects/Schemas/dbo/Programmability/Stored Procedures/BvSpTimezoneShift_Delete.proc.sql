CREATE PROCEDURE [dbo].[BvSpTimezoneShift_Delete]
        @OwnerSID int,
        @ShiftID int,
        @TimezoneID int,
        @Mode int

AS
DECLARE @Rows int
SELECT  @Rows = COUNT( * )
    FROM    BvTimezoneShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ShiftID
    AND TimezoneID = @TimezoneID
    
IF @Rows = 0
BEGIN
    RAISERROR('Shift with ShiftID = %i and OwnerSID = %i not found', 16, 1, @ShiftID, @OwnerSID)
    RETURN -1
END
IF @Rows <> 1
BEGIN
    RAISERROR('Multiple shifts with ShiftID = %i and OwnerSID = %i found', 16, 1, @ShiftID, @OwnerSID)
    RETURN -1
END
    
DELETE  BvTimezoneShift
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ShiftID
    AND     TimezoneID = @TimezoneID
return 0