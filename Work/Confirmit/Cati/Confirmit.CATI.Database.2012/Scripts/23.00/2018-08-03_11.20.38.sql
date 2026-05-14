
UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-06-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-06-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-06-03 02:00:00.000', 0, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+09:00) Pyongyang',
			Bias = -540,
			DaylightType = 2,
			--StandardName = 'North Korea Standard Time', 
			StandardStart = '2000-05-01 23:30:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'North Korea Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = 30
		WHERE StandardName = 'North Korea Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+09:00) Pyongyang',
			Bias = -540,
			DaylightType = 2,
			--StandardName = 'North Korea Standard Time', 
			StandardStart = '2000-05-01 23:30:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'North Korea Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = 30
		WHERE StandardName = 'North Korea Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 118, '(GMT+09:00) Pyongyang', -540, 2, 'North Korea Standard Time', '2000-05-01 23:30:00.000', 5, 0, 'North Korea Daylight Time', '2000-01-01 00:00:00.000', 1, 30
END

GO
PRINT N'Update complete.'

GO
