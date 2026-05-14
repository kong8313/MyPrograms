
UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-04 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-03-04 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-04 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-03-04 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-04 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-03-04 02:00:00.000', 0, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Gaza, Hebron',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'West Bank Gaza Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'West Bank Gaza Daylight Time',
			DaylightStart = '2000-03-04 01:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'West Bank Gaza Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Gaza, Hebron',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'West Bank Gaza Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'West Bank Gaza Daylight Time',
			DaylightStart = '2000-03-04 01:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'West Bank Gaza Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 113, '(GMT+02:00) Gaza, Hebron', -120, 2, 'West Bank Gaza Standard Time', '2000-10-05 01:00:00.000', 6, 0, 'West Bank Gaza Daylight Time', '2000-03-04 01:00:00.000', 6, -60
END

GO
PRINT N'Update complete.'

GO
