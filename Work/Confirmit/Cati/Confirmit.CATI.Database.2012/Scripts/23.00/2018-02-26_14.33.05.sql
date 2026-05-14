
PRINT N'Updating timezones';

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Windhoek',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Namibia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Namibia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Namibia Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Windhoek',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Namibia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Namibia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Namibia Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 92, '(GMT+02:00) Windhoek', -120, 1, 'Namibia Standard Time', NULL, NULL, 0, 'Namibia Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-05:00) Turks and Caicos',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Turks and Caicos Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Turks and Caicos Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = -60
		WHERE StandardName = 'Turks and Caicos Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-05:00) Turks and Caicos',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Turks and Caicos Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Turks and Caicos Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = -60
		WHERE StandardName = 'Turks and Caicos Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 128, '(GMT-05:00) Turks and Caicos', 300, 2, 'Turks and Caicos Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Turks and Caicos Daylight Time', '2000-01-01 00:00:00.000', 1, -60
END


GO
PRINT N'Update complete.';


GO
