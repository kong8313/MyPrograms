UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-07-01 02:00:00.000',
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
			DaylightStart = '2000-07-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-07-01 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-05:00) Haiti',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Haiti Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Haiti Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Haiti Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-05:00) Haiti',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Haiti Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Haiti Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Haiti Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 129, '(GMT-05:00) Haiti', 300, 2, 'Haiti Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Haiti Daylight Time', '2000-03-02 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Saratov',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Saratov Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Saratov Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Saratov Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Saratov',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Saratov Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Saratov Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Saratov Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 138, '(GMT+04:00) Saratov', -240, 1, 'Saratov Standard Time', NULL, NULL, 0, 'Saratov Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+13:00) Coordinated Universal Time+13',
			Bias = -780,
			DaylightType = 1,
			--StandardName = 'UTC+13', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'UTC+13',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'UTC+13'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+13:00) Coordinated Universal Time+13',
			Bias = -780,
			DaylightType = 1,
			--StandardName = 'UTC+13', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'UTC+13',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'UTC+13'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 139, '(GMT+13:00) Coordinated Universal Time+13', -780, 1, 'UTC+13', NULL, NULL, 0, 'UTC+13', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Punta Arenas',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Magallanes Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Magallanes Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Magallanes Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Punta Arenas',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Magallanes Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Magallanes Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Magallanes Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 140, '(GMT-03:00) Punta Arenas', 180, 1, 'Magallanes Standard Time', NULL, NULL, 0, 'Magallanes Daylight Time', NULL, NULL, -60
END


GO
PRINT N'Update complete.';


GO
