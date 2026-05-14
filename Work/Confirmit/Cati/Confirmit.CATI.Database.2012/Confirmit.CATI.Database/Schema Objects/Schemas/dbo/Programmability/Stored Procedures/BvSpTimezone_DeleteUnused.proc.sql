CREATE PROCEDURE [dbo].[BvSpTimezone_DeleteUnused]
AS
	DELETE FROM [BvTimezone]
	WHERE [id] NOT IN (SELECT LocalTimezoneId FROM BvCallCenter)
	AND [id] NOT IN 
		( SELECT [TimezoneID] FROM [BvInterview] WHERE [TimezoneID] IS NOT NULL GROUP BY [TimezoneID] )
	AND [id] NOT IN ( SELECT z.[TimeZoneID] FROM [BvSvySchedule] sh
						JOIN [BvShiftZones] z
						ON sh.[ShiftTypeID] = z.[id] 
						GROUP BY z.[TimeZoneID] )
	AND [id] NOT IN ( SELECT [TimezoneID] FROM [BvTimezoneShift]
					 GROUP BY [TimezoneID] )