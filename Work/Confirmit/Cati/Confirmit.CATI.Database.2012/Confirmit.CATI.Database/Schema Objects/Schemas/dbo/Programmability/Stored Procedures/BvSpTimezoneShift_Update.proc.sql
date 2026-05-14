CREATE PROCEDURE [dbo].[BvSpTimezoneShift_Update]
        @OwnerSID int,
        @ShiftID int,
        @TimezoneID int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime,
        @Mode int

AS

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvShift
    WHERE   ID = @ShiftID
    AND OwnerSID = @OwnerSID
    
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
    
UPDATE  BvTimezoneShift
    SET StartDayOfWeek = @StartDayOfWeek, 
        StartTime = @StartTime,
        FinishDayOfWeek = @FinishDayOfWeek,
        FinishTime = @FinishTime
    WHERE   OwnerSID = @OwnerSID
    AND ShiftID = @ShiftID
    AND TimezoneID = @TimezoneID
return 0