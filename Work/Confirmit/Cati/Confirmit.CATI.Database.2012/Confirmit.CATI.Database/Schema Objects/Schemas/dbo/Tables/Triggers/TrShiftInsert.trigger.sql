CREATE TRIGGER [dbo].[TrShiftInsert] ON [dbo].[BvShift] 
AFTER INSERT
AS
 
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