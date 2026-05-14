UPDATE BvTimezone SET  
			Name = '(GMT+13:00) Nuku''alofa',
			Bias = -780,
			DaylightType = 2,
			--StandardName = 'Tonga Standard Time', 
			StandardStart = '2000-01-03 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Tonga Daylight Time',
			DaylightStart = '2000-11-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Tonga Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+13:00) Nuku''alofa',
			Bias = -780,
			DaylightType = 2,
			--StandardName = 'Tonga Standard Time', 
			StandardStart = '2000-01-03 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Tonga Daylight Time',
			DaylightStart = '2000-11-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Tonga Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 50, '(GMT+13:00) Nuku''alofa', -780, 2, 'Tonga Standard Time', '2000-01-03 03:00:00.000', 0, 0, 'Tonga Daylight Time', '2000-11-01 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:00) Ulaanbaatar',
			Bias = -480,
			DaylightType = 1,
			--StandardName = 'Ulaanbaatar Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Ulaanbaatar Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Ulaanbaatar Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+08:00) Ulaanbaatar',
			Bias = -480,
			DaylightType = 1,
			--StandardName = 'Ulaanbaatar Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Ulaanbaatar Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Ulaanbaatar Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 98, '(GMT+08:00) Ulaanbaatar', -480, 1, 'Ulaanbaatar Standard Time', NULL, NULL, 0, 'Ulaanbaatar Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Hovd',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'W. Mongolia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'W. Mongolia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'W. Mongolia Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Hovd',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'W. Mongolia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'W. Mongolia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'W. Mongolia Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 116, '(GMT+07:00) Hovd', -420, 1, 'W. Mongolia Standard Time', NULL, NULL, 0, 'W. Mongolia Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.';


GO
