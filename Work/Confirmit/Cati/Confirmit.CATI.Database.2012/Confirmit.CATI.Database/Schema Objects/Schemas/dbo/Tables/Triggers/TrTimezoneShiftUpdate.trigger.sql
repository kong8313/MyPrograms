CREATE TRIGGER [dbo].[TrTimezoneShiftUpdate] ON [dbo].[BvTimezoneShift] 
AFTER UPDATE 
AS
 
-- first delete
    delete from BvTzUnPeriodicalShifts
    from BvTzUnPeriodicalShifts t, inserted d
    where t.shift_id    = d.ShiftID
        and t.owner_id  = d.OwnerSID
        and t.tz_id     = d.TimezoneID
 
    delete from BvTzPeriodicalShifts
    from BvTzPeriodicalShifts t, inserted d
    where t.shift_id    = d.ShiftID
        and t.owner_id  = d.OwnerSID
        and t.tz_id     = d.TimezoneID
 
-- second insert
insert into BvTzUnPeriodicalShifts
    select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           t.TimeZoneID as timezone,
           t.StartTime  as Start,
           t.FinishTime as Finish
    from dbo.BvShift s
    inner join inserted t on s.[ID] = t.ShiftID 
        and s.OwnerSID = t.OwnerSID
    where s.CycleType = 2
 
insert into BvTzPeriodicalShifts
    select s.[ID] as ShiftID,
           s.ShiftTypeID,
           s.OwnerSID,
           t.TimeZoneID as timezone,
           dbo.udfShiftStart( t.StartDayOfWeek, t.StartTime ) as StartInMins,
           dbo.udfShiftFinish( t.StartDayOfWeek, t.FinishDayOfWeek, t.StartTime, t.FinishTime ) as FinishInMins
    from dbo.BvShift s
    inner join inserted t on s.[ID] = t.ShiftID and s.OwnerSID = t.OwnerSID
    where s.CycleType = 1