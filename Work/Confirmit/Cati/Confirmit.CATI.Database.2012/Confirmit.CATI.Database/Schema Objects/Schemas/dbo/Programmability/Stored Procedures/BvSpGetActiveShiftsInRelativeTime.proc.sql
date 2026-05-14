CREATE PROCEDURE [dbo].[BvSpGetActiveShiftsInRelativeTime]
@dtStart DATETIME, @dtFinish DATETIME, @DefaultTZ INT
AS

    DECLARE @TimeSpanInMin INT = DATEDIFF( "n", @dtStart, @dtFinish );
    
    DECLARE @TimeInWeek INT = ( DATEPART( dw, @dtStart ) - 1 ) * 1440 + 
                            DATEPART( hour, @dtStart ) * 60 + 
                            DATEPART( minute, @dtStart )
                            
    CREATE TABLE #tzshift (
        [ID] [int] NOT NULL,
        [OwnerID] [int] NOT NULL,
        [ShiftTypeID] [int] NOT NULL,
        [TimeZoneID] [int] NOT NULL
    )

    DECLARE @WeekSizeInMinutes INT = 7 * 24 * 60
		
    ;WITH 
    -- select using shifts
    ShiftByTZ( owner_id, shift_id, type_id, tz_id, start_wo, finish_wo ) AS
    (
        SELECT owner_id, shift_id, type_id, tz_id, start_dt, finish_dt from BvTzPeriodicalShifts where start_dt != finish_dt /*ignore fictitious shifts*/
    ),
    --calc first future time for shift by TZ
    MatchingShiftByTZ( owner_id, shift_id, type_id, tz_id) as
    (
        SELECT s.owner_id, s.shift_id, s.type_id, s.tz_id
            FROM ShiftByTZ as s
           WHERE ( s.start_wo <= @TimeInWeek AND s.finish_wo >= (@TimeInWeek + @TimeSpanInMin) ) 
               OR ( s.finish_wo > 10080/*60*24*7*/ AND	(s.start_wo - 10080/*60*24*7*/) <= @TimeInWeek AND
														(s.finish_wo - 10080/*60*24*7*/) >= (@TimeInWeek + @TimeSpanInMin) )
    )
    INSERT INTO #tzshift
    SELECT	shift_id,
			owner_id,
			type_id, 
			tz_id
		FROM MatchingShiftByTZ ms WHERE NOT EXISTS( 
                SELECT 1 FROM BvTzUnPeriodicalShifts ex 
					WHERE ex.owner_id = ms.owner_id AND ex.tz_id = ms.tz_id AND
						ex.start_dt <= @dtStart AND ex.finish_dt > @dtStart )

	INSERT INTO #tzshift 
		SELECT ID, OwnerID, ShiftTypeID, 0 FROM  #tzshift WHERE TimeZoneID = @DefaultTZ;
		
	SELECT	[ID] as [ID],
			[OwnerID] as [OwnerID],
			[ShiftTypeID] as [ShiftTypeID],
			[TimeZoneID] as [TimeZoneID]
		FROM #tzshift
	
	DROP TABLE #tzshift
	
RETURN (0)