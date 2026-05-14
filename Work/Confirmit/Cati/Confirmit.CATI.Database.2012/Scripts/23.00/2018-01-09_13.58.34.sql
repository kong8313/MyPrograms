
UPDATE BvTimezone SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-04 00:00:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-04 00:00:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 18, '(GMT+03:30) Tehran', -210, 2, 'Iran Standard Time', '2000-09-03 23:59:00.000', 5, 0, 'Iran Daylight Time', '2000-03-04 00:00:00.000', 4, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+12:00) Fiji',
			Bias = -720,
			DaylightType = 2,
			--StandardName = 'Fiji Standard Time', 
			StandardStart = '2000-01-02 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Fiji Daylight Time',
			DaylightStart = '2000-11-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Fiji Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+12:00) Fiji',
			Bias = -720,
			DaylightType = 2,
			--StandardName = 'Fiji Standard Time', 
			StandardStart = '2000-01-02 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Fiji Daylight Time',
			DaylightStart = '2000-11-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Fiji Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 49, '(GMT+12:00) Fiji', -720, 2, 'Fiji Standard Time', '2000-01-02 03:00:00.000', 0, 0, 'Fiji Daylight Time', '2000-11-01 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+13:00) Nuku''alofa',
			Bias = -780,
			DaylightType = 1,
			--StandardName = 'Tonga Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Tonga Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Tonga Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+13:00) Nuku''alofa',
			Bias = -780,
			DaylightType = 1,
			--StandardName = 'Tonga Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Tonga Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Tonga Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 50, '(GMT+13:00) Nuku''alofa', -780, 1, 'Tonga Standard Time', NULL, NULL, 0, 'Tonga Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Brasilia',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'E. South America Standard Time', 
			StandardStart = '2000-02-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'E. South America Daylight Time',
			DaylightStart = '2000-10-03 23:59:00.000',
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
			DaylightStart = '2000-10-03 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'E. South America Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 54, '(GMT-03:00) Brasilia', 180, 2, 'E. South America Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'E. South America Daylight Time', '2000-10-03 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Greenland',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Greenland Standard Time', 
			StandardStart = '2000-10-05 23:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Greenland Daylight Time',
			DaylightStart = '2000-03-04 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Greenland Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Greenland',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Greenland Standard Time', 
			StandardStart = '2000-10-05 23:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Greenland Daylight Time',
			DaylightStart = '2000-03-04 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Greenland Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 56, '(GMT-03:00) Greenland', 180, 2, 'Greenland Standard Time', '2000-10-05 23:00:00.000', 6, 0, 'Greenland Daylight Time', '2000-03-04 22:00:00.000', 6, -60
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
			DaylightStart = '2000-10-03 23:59:00.000',
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
			DaylightStart = '2000-10-03 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Central Brazilian Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 79, '(GMT-04:00) Cuiaba', 240, 2, 'Central Brazilian Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'Central Brazilian Daylight Time', '2000-10-03 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Beirut',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Middle East Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Middle East Daylight Time',
			DaylightStart = '2000-03-04 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Middle East Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Beirut',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Middle East Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Middle East Daylight Time',
			DaylightStart = '2000-03-04 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Middle East Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 88, '(GMT+02:00) Beirut', -120, 2, 'Middle East Standard Time', '2000-10-05 23:59:00.000', 6, 0, 'Middle East Daylight Time', '2000-03-04 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
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
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-03-05 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Asuncion',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Paraguay Standard Time', 
			StandardStart = '2000-03-04 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Paraguay Daylight Time',
			DaylightStart = '2000-10-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Paraguay Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Asuncion',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Paraguay Standard Time', 
			StandardStart = '2000-03-04 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Paraguay Daylight Time',
			DaylightStart = '2000-10-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Paraguay Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 95, '(GMT-04:00) Asuncion', 240, 2, 'Paraguay Standard Time', '2000-03-04 23:59:00.000', 6, 0, 'Paraguay Daylight Time', '2000-10-01 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Khartoum',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Sudan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Sudan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Sudan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Khartoum',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Sudan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Sudan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Sudan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 141, '(GMT+02:00) Khartoum', -120, 1, 'Sudan Standard Time', NULL, NULL, 0, 'Sudan Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.';


GO
