
UPDATE BvTimezone SET  
			Name = '(GMT+13:00) Nuku''alofa',
			Bias = -780,
			DaylightType = 2,
			--StandardName = 'Tonga Standard Time', 
			StandardStart = '2000-01-03 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Tonga Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
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
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Tonga Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 50, '(GMT+13:00) Nuku''alofa', -780, 2, 'Tonga Standard Time', '2000-01-03 03:00:00.000', 0, 0, 'Tonga Daylight Time', '2000-01-01 00:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Khartoum',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Sudan Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 2, 
			StandardBias = 0,
			DaylightName = 'Sudan Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Sudan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Khartoum',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Sudan Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 2, 
			StandardBias = 0,
			DaylightName = 'Sudan Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Sudan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 141, '(GMT+02:00) Khartoum', -120, 2, 'Sudan Standard Time', '2000-10-05 23:59:00.000', 2, 0, 'Sudan Daylight Time', '2000-01-01 00:00:00.000', 0, -60
END

GO
PRINT N'Update complete.';


GO
