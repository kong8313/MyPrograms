CREATE function dbo.udfShiftStart
-- return count of minutes from start of week
(
@week_day    int,
@time        smalldatetime
)
returns integer 
as
begin
 
    return @week_day * 1440 + datepart( hour, @time ) * 60 +
        datepart( minute, @time )
end