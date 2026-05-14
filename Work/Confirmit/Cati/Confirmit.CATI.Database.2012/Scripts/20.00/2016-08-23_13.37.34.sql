
UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Cairo',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Egypt Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Egypt Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Egypt Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Cairo',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Egypt Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Egypt Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Egypt Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 10, '(GMT+02:00) Cairo', -120, 1, 'Egypt Standard Time', NULL, NULL, 0, 'Egypt Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.';

GO
