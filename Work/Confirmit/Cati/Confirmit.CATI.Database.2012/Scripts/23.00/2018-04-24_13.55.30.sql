
UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Brasilia',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'E. South America Standard Time', 
			StandardStart = '2000-02-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'E. South America Daylight Time',
			DaylightStart = '2000-11-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'E. South America Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Brasilia',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'E. South America Standard Time', 
			StandardStart = '2000-02-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'E. South America Daylight Time',
			DaylightStart = '2000-11-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'E. South America Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 54, '(GMT-03:00) Brasilia', 180, 2, 'E. South America Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'E. South America Daylight Time', '2000-11-01 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Cuiaba',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Central Brazilian Standard Time', 
			StandardStart = '2000-02-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Central Brazilian Daylight Time',
			DaylightStart = '2000-11-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Central Brazilian Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Cuiaba',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Central Brazilian Standard Time', 
			StandardStart = '2000-02-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Central Brazilian Daylight Time',
			DaylightStart = '2000-11-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Central Brazilian Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 79, '(GMT-04:00) Cuiaba', 240, 2, 'Central Brazilian Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'Central Brazilian Daylight Time', '2000-11-01 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-05-02 03:00:00.000',
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
			StandardStart = '2000-05-02 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-03-04 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-05-02 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-03-04 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+01:00) Sao Tome',
			Bias = -60,
			DaylightType = 2,
			--StandardName = 'Sao Tome Standard Time', 
			StandardStart = '2000-01-01 01:00:00.000',
			StandardDayOfWeek = 1, 
			StandardBias = 0,
			DaylightName = 'Sao Tome Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = 60
		WHERE StandardName = 'Sao Tome Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+01:00) Sao Tome',
			Bias = -60,
			DaylightType = 2,
			--StandardName = 'Sao Tome Standard Time', 
			StandardStart = '2000-01-01 01:00:00.000',
			StandardDayOfWeek = 1, 
			StandardBias = 0,
			DaylightName = 'Sao Tome Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = 60
		WHERE StandardName = 'Sao Tome Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 142, '(GMT+01:00) Sao Tome', -60, 2, 'Sao Tome Standard Time', '2000-01-01 01:00:00.000', 1, 0, 'Sao Tome Daylight Time', '2000-01-01 00:00:00.000', 1, 60
END
