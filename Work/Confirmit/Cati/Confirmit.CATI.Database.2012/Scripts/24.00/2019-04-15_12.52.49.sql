
UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Sao Tome',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Sao Tome Standard Time', 
			StandardStart = '2000-01-01 02:00:00.000',
			StandardDayOfWeek = 2, 
			StandardBias = 0,
			DaylightName = 'Sao Tome Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 2,
			DaylightBias = -60
		WHERE StandardName = 'Sao Tome Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+00:00) Sao Tome',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Sao Tome Standard Time', 
			StandardStart = '2000-01-01 02:00:00.000',
			StandardDayOfWeek = 2, 
			StandardBias = 0,
			DaylightName = 'Sao Tome Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 2,
			DaylightBias = -60
		WHERE StandardName = 'Sao Tome Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 142, '(GMT+00:00) Sao Tome', 0, 2, 'Sao Tome Standard Time', '2000-01-01 02:00:00.000', 2, 0, 'Sao Tome Daylight Time', '2000-01-01 00:00:00.000', 2, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+05:00) Qyzylorda',
			Bias = -300,
			DaylightType = 1,
			--StandardName = 'Qyzylorda Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Qyzylorda Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Qyzylorda Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+05:00) Qyzylorda',
			Bias = -300,
			DaylightType = 1,
			--StandardName = 'Qyzylorda Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Qyzylorda Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Qyzylorda Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 144, '(GMT+05:00) Qyzylorda', -300, 1, 'Qyzylorda Standard Time', NULL, NULL, 0, 'Qyzylorda Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.'

GO
