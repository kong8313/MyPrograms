CREATE TRIGGER [dbo].[TrShiftUpdate] ON [dbo].[BvShift] 
AFTER UPDATE
AS
    delete from BvTzPeriodicalShifts
        from BvTzPeriodicalShifts t, inserted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
 
    delete from BvTzUnPeriodicalShifts
        from BvTzUnPeriodicalShifts t, inserted d
        where t.shift_id    = d.[ID]
            and t.owner_id  = d.OwnerSID
 
-- insert un periodical shifts
    insert into BvTzUnPeriodicalShifts
        select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           tz.[ID] as timezone,
           s.StartTime as Start,
           s.FinishTime as Finish
        from inserted s
        cross join dbo.BvTimezone tz
        where s.CycleType = 2
 
-- insert periodical shifts
    insert into BvTzPeriodicalShifts
        select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           tz.[ID] as timezone,
           dbo.udfShiftStart( s.StartDayOfWeek, s.StartTime ),
           dbo.udfShiftFinish( s.StartDayOfWeek, s.FinishDayOfWeek, s.StartTime, s.FinishTime )
    from inserted s
    cross join dbo.BvTimezone tz
    where s.CycleType = 1