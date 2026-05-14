CREATE TRIGGER [dbo].[TrShiftDelete] ON [dbo].[BvShift] 
AFTER DELETE 
AS
    delete from BvTzPeriodicalShifts
        from BvTzPeriodicalShifts t, deleted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
 
    delete from BvTzUnPeriodicalShifts
        from BvTzUnPeriodicalShifts t, deleted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID