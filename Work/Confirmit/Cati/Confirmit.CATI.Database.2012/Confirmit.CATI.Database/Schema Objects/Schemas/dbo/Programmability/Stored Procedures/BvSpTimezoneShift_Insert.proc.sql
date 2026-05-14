CREATE PROCEDURE [dbo].[BvSpTimezoneShift_Insert]
        @OwnerSID int,
        @ShiftID int,
        @TimezoneID int,
        @StartDayOfWeek int,
        @StartTime datetime,
        @FinishDayOfWeek int,
        @FinishTime datetime

AS

DECLARE @Rows int

SELECT  @Rows = COUNT(*)
    FROM    BvTimezoneShift
    WHERE   TimezoneID = @TimezoneID
    AND ShiftID = @ShiftID
    AND OwnerSID = @OwnerSID
IF @Rows <> 0
--  return 50001    /* BVDBS_STORED_PROCEDURE_DUPLICATED_OBJECT */
    return 0

INSERT  BvTimezoneShift( 
        OwnerSID, 
        ShiftID, 
        StartDayOfWeek,
        StartTime,
        FinishDayOfWeek,
        FinishTime,
        TimezoneID )
    VALUES( @OwnerSID, 
        @ShiftID, 
        @StartDayOfWeek,
        @StartTime,
        @FinishDayOfWeek,
        @FinishTime,
        @TimezoneID )
return  @TimezoneID