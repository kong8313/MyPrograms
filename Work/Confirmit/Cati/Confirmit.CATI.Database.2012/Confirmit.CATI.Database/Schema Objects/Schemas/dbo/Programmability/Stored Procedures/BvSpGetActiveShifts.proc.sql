CREATE PROCEDURE [dbo].[BvSpGetActiveShifts]
@dtStart    DATETIME,--UTC
@dtFinish   DATETIME,--UTC
@SelectType INT = 1, -- 1 - ShiftID, OwnerID, ShiftType, TimeZoneID
                     -- 2 - ShiftType, TimeZone
                     -- 3 - BvShiftZones.ID, ShiftTypeID
@DefaultTZID INT
--WITH ENCRYPTION
AS
    DECLARE @ShiftTypeNone INT = -2147483648; --None constant for bvSvySchedule.ShiftTypeID

    CREATE TABLE #tzshift (
        [ID] [int] NOT NULL,
        [OwnerID] [int] NOT NULL,
        [ShiftTypeID] [int] NOT NULL,
        [TimeZoneID] [int] NOT NULL
    )

    DECLARE @TimeSpanInMin INT
    --select @dtStart, @dtFinish
    
    SET @TimeSpanInMin = DATEDIFF( "n", @dtStart, @dtFinish );

    WITH 
    -- select using shifts
    ShiftByTZ( owner_id, shift_id, type_id, tz_id, start_wo, finish_wo ) AS
    (
        SELECT owner_id, shift_id, type_id, tz_id, start_dt, finish_dt from BvTzPeriodicalShifts where start_dt != finish_dt /*ignore fictitious shifts*/
    ),
    -- select offset from begin week in minutes by TZ for current time
    TimeAndWeekOffsetByTZ( tz_id, cur_ut, cur_lt, cur_week_start_lt, cur_tz_wo ) AS
    (
        SELECT ID, cur_ut, cur_lt, DATEADD( minute, -cur_tz_wo, cur_lt), cur_tz_wo FROM (
        SELECT [ID], @dtStart as cur_ut, cur_lt, ( DATEPART( dw, cur_lt ) - 1 ) * 1440 + 
                            DATEPART( hour, cur_lt ) * 60 + 
                            DATEPART( minute, cur_lt ) as cur_tz_wo                      
                                from ( select  [ID], dbo.UTC2LT( @dtStart, Bias, DaylightType,
                                    StandardDayOfWeek, StandardStart, StandardBias,
                                    DaylightDayOfWeek, DaylightStart, DaylightBias ) as cur_lt
                                 from BvTimezone ) s1 ) s2
    ),
    --calc first future time for shift by TZ
    MatchingShiftByTZ( owner_id, shift_id, type_id, tz_id, start_lt, finish_lt) as
    (
        SELECT s.owner_id, s.shift_id, s.type_id, s.tz_id, 
            cur_lt,--trim shift time
            DATEADD( minute, @TimeSpanInMin , cur_lt )--trim shift time
            FROM ShiftByTZ as s
            INNER JOIN TimeAndWeekOffsetByTZ two
            ON s.tz_id = two.tz_id
           WHERE ( s.start_wo <= two.cur_tz_wo AND s.finish_wo >= (two.cur_tz_wo + @TimeSpanInMin) ) 
               OR ( s.finish_wo > 10080/*60*24*7*/ AND (s.start_wo - 10080/*60*24*7*/) <= two.cur_tz_wo AND (s.finish_wo - 10080/*60*24*7*/) >= (two.cur_tz_wo + @TimeSpanInMin) )
    )
    INSERT INTO #tzshift --OUTPUT INSERTED.* 
    SELECT shift_id, owner_id, type_id, tz_id FROM MatchingShiftByTZ ms where NOT EXISTS( 
                SELECT 1 FROM BvTzUnPeriodicalShifts ex WHERE ms.start_lt >= ex.start_dt and ms.finish_lt < ex.finish_dt and ex.owner_id = ms.owner_id and ex.tz_id = ms.tz_id )

  -- Prepare default timezone
  INSERT INTO #tzshift 
    SELECT [ID], OwnerID, ShiftTypeID, 0
    FROM #tzshift WHERE TimeZoneID = @DefaultTZID

  IF @SelectType = 1
      SELECT [ID], OwnerID, ShiftTypeID, TimeZoneID FROM #tzshift
  ELSE IF @SelectType = 2
      SELECT DISTINCT ShiftTypeID, TimeZoneID FROM #tzshift
  ELSE IF @SelectType = 3
      SELECT DISTINCT BvShiftZones.[ID] , #tzshift.OwnerID
        FROM BvShiftZones, #tzshift
        WHERE BvShiftZones.TimeZoneID = #tzshift.TimeZoneID
          AND BvShiftZones.ShiftTypeID = #tzshift.ShiftTypeID
      UNION ALL
      SELECT DISTINCT -TimeZoneID, OwnerID FROM #tzshift
      UNION ALL
      SELECT @ShiftTypeNone, ScheduleID FROM BvSchedule
    drop table #tzshift
RETURN (0)