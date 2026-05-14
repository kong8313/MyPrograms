PRINT N'Altering [dbo].[BvSpSchedule_Delete]...';


GO
ALTER PROCEDURE [dbo].[BvSpSchedule_Delete]
       @ScheduleID int 
AS
DECLARE @rows INT

DECLARE @allHourSID INT
SELECT @allHourSID = MIN( ScheduleID ) FROM BvSchedule

/* Don't allow to delete 'All hours' schedule */
IF @allHourSID = @ScheduleID
BEGIN
	RAISERROR( 'Could not delete default scheduling script.', 12, 1)
    RETURN -1
END

IF EXISTS ( SELECT SID FROM BvSurvey WHERE ScheduleID = @ScheduleID AND State <> 2 )
BEGIN
	RAISERROR( 'Could not delete scheduling script that used by survey(s)', 12, 1)
	RETURN -1
END

BEGIN TRAN

    --should we update calls with none shift type?
    UPDATE BvSvySchedule SET ShiftTypeID = -z.TimeZoneID
    FROM BvSvySchedule c
    INNER JOIN BvShiftZones z ON c.ShiftTypeID = z.[ID] 
    INNER JOIN BvShiftType t ON t.OwnerSID = @ScheduleID AND z.ShiftTypeID = t.ObjectID


    DELETE FROM BvScheduleParam WHERE ScheduleID = @ScheduleID

    DELETE FROM BvShiftZones
        WHERE ShiftTypeID IN ( 
            SELECT ObjectID FROM BvShiftType
            WHERE OwnerSID = @ScheduleID )

    DELETE  BvShift
        WHERE OwnerSID = @ScheduleID

    DELETE  BvShiftType
        WHERE OwnerSID = @ScheduleID

    DELETE  BvTimezoneShift
        WHERE OwnerSID = @ScheduleID

    IF EXISTS ( SELECT SID FROM BvSurvey WHERE ScheduleID = @ScheduleID AND State = 2 )
    BEGIN
        UPDATE BvSurvey 
            SET ScheduleID = @allHourSID 
        WHERE ScheduleID = @ScheduleID AND State = 2
    END

    DELETE FROM BvSchedule 
        WHERE   ScheduleID = @ScheduleID

COMMIT

RETURN (0)
GO
PRINT N'Update complete.';


GO
