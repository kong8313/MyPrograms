create procedure [dbo].[BvSpGetLiveShifts]
@utc smalldatetime,    -- in utc time
@tz_local INT,
@TzBalancingThreshold INT=0
as
set nocount on
declare @date1 int
DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID
 
set @date1 = @@DATEFIRST
set DATEFIRST 7
 
    create table #temp_tz ( 
        tz_id    int,
        ltStart  smalldatetime,				--local time in a specific TZ
        minStart int						--offset in minutes withing a week (for ltStart)
    )
 
    create table #active
    (
        [ID] int not null,
        ScheduleID int not null,
        tz_id int not null,
		[ShiftPriority] int not null
    )
 
    -- check in future
    set @utc = dateadd( minute, 1, @utc )
 
    -- insert into temp normalize date by all timezone
    -- normalize date - time in minute from start of week
    -- = day_of_week * 24 * 60 + hour * 60 + minute
    insert into #temp_tz 
	select	[ID]										AS TzID, 
														ltStart,
            (DATEPART( dw, ltStart ) - 1 ) * 1440 + 
             DATEPART( hour, ltStart ) * 60 + 
             DATEPART( minute, ltStart )				AS minStart

    from ( select  [ID], dbo.UTC2LT( @utc, Bias, DaylightType,
            StandardDayOfWeek, StandardStart, StandardBias,
            DaylightDayOfWeek, DaylightStart, DaylightBias ) as ltStart
            from BvTimezone ) s1
 
        --select * from #temp_tz
  
    -- insert periodical active shifts to 
    insert into #active
        select distinct 
			z.[ID], 
			tzs.owner_id, 
			tzs.tz_id,
			case 
				when (tzs.finish_dt - #temp_tz.minStart) < @TzBalancingThreshold
					then 1
					else 0
			end	as [ShiftPriority]
        from #temp_tz
        inner join BvTzPeriodicalShifts tzs on
            #temp_tz.tz_id = tzs.tz_id
              and ( #temp_tz.minStart >= tzs.start_dt 
              and #temp_tz.minStart < tzs.finish_dt OR 
              #temp_tz.minStart + 10080/*week*/ >= tzs.start_dt 
              and #temp_tz.minStart + 10080/*week*/ < tzs.finish_dt)
        inner join BvShiftZones z on
              ( z.TimeZoneID = tzs.tz_id or
              ( z.TimeZoneID = 0 and tzs.tz_id = @tz_local ) )
              and z.ShiftTypeID = tzs.type_id
 
    -- delete shifts which fits exclusions
        delete from #active 
        from  #active a 
                        join BvTzUnPeriodicalShifts utzs on
                                a.tz_id = utzs.tz_id
                                 and a.ScheduleID = utzs.owner_id
                        join #temp_tz on #temp_tz.tz_id = utzs.tz_id
                        
                        where 
                                #temp_tz.ltStart >= utzs.start_dt and #temp_tz.ltStart < utzs.finish_dt
                        
    set DATEFIRST @date1
    drop table #temp_tz

    -- insert timezones for [AnyValid] calls
    insert into #active
        select distinct -z.TimeZoneID, a.ScheduleID, a.tz_id, a.[ShiftPriority]
        from #active a, BvShiftZones z
        where a.[ID] = z.[ID]
    -- insert fictive shift for [None] calls
    insert into #active
        select 
			@ShiftTypeNone, 
			ScheduleID, 
			0,
			0				-- for simplicity we won't prioritise such call in tz balancing mechanism
		FROM BvSchedule
 
    select a.[ID], b.SID, a.[ShiftPriority]
        from  BvSurvey b
		inner join #active a
		on a.ScheduleID = b.ScheduleID
        WHERE b.State = 1 /* survey opened */
			  AND EXISTS( SELECT 1 FROM BvSvyScheduleRuntimeStatistics srs WHERE b.SID = srs.SurveyId AND srs.ShiftTypeID = a.ID AND srs.FreeCount > 0)

return (0)