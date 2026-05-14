CREATE TRIGGER [dbo].[TrTimezoneShiftDelete] ON [dbo].[BvTimezoneShift] 
AFTER DELETE 
AS
declare @site_tz int
 
    delete from BvTzPeriodicalShifts
        from BvTzPeriodicalShifts t, deleted d
        where t.shift_id    = d.ShiftID
            and t.owner_id  = d.OwnerSID
            and t.tz_id     = d.TimezoneID
 
    delete from BvTzUnPeriodicalShifts
        from BvTzUnPeriodicalShifts t, deleted d
        where t.shift_id    = d.ShiftID
            and t.owner_id  = d.OwnerSID
            and t.tz_id     = d.TimezoneID