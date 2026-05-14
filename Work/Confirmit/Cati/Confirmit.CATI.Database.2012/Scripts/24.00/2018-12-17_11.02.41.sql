
UPDATE BvTimezone SET  
			Name = '(GMT+03:00) Moscow, St. Petersburg',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Russia TZ 2 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 2 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 2 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Moscow, St. Petersburg',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Russia TZ 2 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 2 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 2 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 16, '(GMT+03:00) Moscow, St. Petersburg', -180, 1, 'Russia TZ 2 Standard Time', NULL, NULL, 0, 'Russia TZ 2 Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+01:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-01-01 00:00:00.000',
			StandardDayOfWeek = 1, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-06-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+01:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-01-01 00:00:00.000',
			StandardDayOfWeek = 1, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-06-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+01:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-01-01 00:00:00.000', 1, 0, 'Morocco Daylight Time', '2000-06-03 02:00:00.000', 0, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Volgograd',
			Bias = -240,
			DaylightType = 2,
			--StandardName = 'Volgograd Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Volgograd Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = 60
		WHERE StandardName = 'Volgograd Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Volgograd',
			Bias = -240,
			DaylightType = 2,
			--StandardName = 'Volgograd Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Volgograd Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = 60
		WHERE StandardName = 'Volgograd Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 143, '(GMT+04:00) Volgograd', -240, 2, 'Volgograd Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'Volgograd Daylight Time', '2000-01-01 00:00:00.000', 1, 60
END

GO
PRINT N'Update complete.'

GO
