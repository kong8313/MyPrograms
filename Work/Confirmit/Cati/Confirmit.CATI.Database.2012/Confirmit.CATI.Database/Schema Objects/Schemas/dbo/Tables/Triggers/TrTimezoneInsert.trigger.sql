CREATE TRIGGER dbo.TrTimezoneInsert ON dbo.BvTimezone 
AFTER INSERT 
AS
DECLARE @site_tz    int = ISNULL( ( SELECT LocalTimezoneId FROM BvCallCenter WHERE IsDefault = 1 ), 1 )
 
    INSERT INTO BvTzPeriodicalShifts
        SELECT  ts.shift_id,
                ts.type_id,
                ts.owner_id,
                inserted.ID,
                ts.start_dt,
                ts.finish_dt
    FROM BvTzPeriodicalShifts ts, inserted
    WHERE ts.tz_id = @site_tz
 
    INSERT INTO BvTzUnPeriodicalShifts
        SELECT  ts.shift_id,
                ts.type_id,
                ts.owner_id,
                inserted.ID,
                ts.start_dt,
                ts.finish_dt
    FROM BvTzUnPeriodicalShifts ts, inserted
    WHERE ts.tz_id = @site_tz

    -- Insert shift type time zones
    INSERT INTO BvShiftZones 
      SELECT i.ID, BvShiftType.[ObjectID]
      FROM inserted i, BvShiftType