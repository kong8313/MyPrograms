CREATE TRIGGER dbo.TrTimezoneDelete ON dbo.BvTimezone 
AFTER DELETE 
AS
    DELETE FROM BvTzPeriodicalShifts WHERE tz_id IN
        ( SELECT ID FROM deleted )
 
    DELETE FROM BvTzUnPeriodicalShifts WHERE tz_id IN
        ( SELECT ID FROM deleted )

    DELETE FROM BvShiftZones WHERE TimeZoneID IN
        ( SELECT ID FROM deleted )