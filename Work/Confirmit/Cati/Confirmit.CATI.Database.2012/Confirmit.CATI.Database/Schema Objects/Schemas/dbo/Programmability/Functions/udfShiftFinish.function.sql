create function dbo.udfShiftFinish
-- return count of minutes from start of week
(
@sweek_day    int,
@fweek_day    int,
@stime        smalldatetime,
@ftime        smalldatetime
)
returns integer 
as
begin
declare @week_day int
 
    set @week_day = @fweek_day
    
    if ( @fweek_day < @sweek_day ) or
        ( @fweek_day = @sweek_day and @sweek_day > @fweek_day )
        set @week_day = @week_day + 7
 
    return @week_day * 1440 + datepart( hour, @ftime ) * 60 +
        datepart( minute, @ftime )
end