CREATE PROCEDURE [dbo].[BvSpShift_Update]
        @OwnerSID int,
        @OldID int,
        @NewID int,
        @CycleType int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime,
        @ShiftTypeID int,
        @Mode int

AS

IF ( @OldID <> @NewID ) BEGIN
    UPDATE  BvTimezoneShift
        SET ShiftID = @NewID
        WHERE   OwnerSID = @OwnerSID
        AND ShiftID = @OldID
    UPDATE  BvShift
        SET ID = @NewID,
            CycleType = @CycleType,
            StartDayOfWeek = @StartDayOfWeek, 
            StartTime = @StartTime,
            FinishDayOfWeek = @FinishDayOfWeek,
            FinishTime = @FinishTime,
            ShiftTypeID = @ShiftTypeID
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END
ELSE BEGIN
    UPDATE  BvShift
        SET CycleType = @CycleType,
            StartDayOfWeek = @StartDayOfWeek, 
            StartTime = @StartTime,
            FinishDayOfWeek = @FinishDayOfWeek,
            FinishTime = @FinishTime,
            ShiftTypeID = @ShiftTypeID
        WHERE   OwnerSID = @OwnerSID
        AND ID = @OldID
END

return 0