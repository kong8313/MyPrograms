CREATE PROCEDURE [dbo].[BvSpQueueUpSheduleTask3]
    @NowUTC           datetime,
    @DefaultTZ        INT,
	@TzBalancingThreshold INT=0
as
set nocount on

declare @rows int
 
    -- temp table for determine active shifts/survey
    create table #temp
    (
        [ID] int not null,
        SurveySID int not null,
		ShiftPriority int not null
    )
 
    -- calculate live shifts 
    insert into #temp exec BvSpGetLiveShifts @NowUTC, @DefaultTZ, @TzBalancingThreshold
 
        -- copy new shifts information
     delete BvActiveShiftTypeZone
     insert into BvActiveShiftTypeZone
     select [ID], SurveySID, ShiftPriority from #temp
 
     drop table #temp
return (0)