
UPDATE BvTimezone SET  
			Name = '(GMT+03:00) Istanbul',
			Bias = -180,
			DaylightType = 2,
			--StandardName = 'Turkey Standard Time', 
			StandardStart = '2000-03-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Turkey Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Turkey Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Istanbul',
			Bias = -180,
			DaylightType = 2,
			--StandardName = 'Turkey Standard Time', 
			StandardStart = '2000-03-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Turkey Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Turkey Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 97, '(GMT+03:00) Istanbul', -180, 2, 'Turkey Standard Time', '2000-03-05 03:00:00.000', 0, 0, 'Turkey Daylight Time', '2000-01-01 00:00:00.000', 5, 60
END

GO
PRINT N'Update complete.';


GO
