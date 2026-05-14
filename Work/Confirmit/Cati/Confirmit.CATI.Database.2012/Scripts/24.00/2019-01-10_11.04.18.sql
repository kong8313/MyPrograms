
UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Jerusalem',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Jerusalem Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Jerusalem Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Jerusalem Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Jerusalem',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Jerusalem Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Jerusalem Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Jerusalem Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 13, '(GMT+02:00) Jerusalem', -120, 2, 'Jerusalem Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'Jerusalem Daylight Time', '2000-03-05 02:00:00.000', 5, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-04 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-04 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 18, '(GMT+03:30) Tehran', -210, 2, 'Iran Standard Time', '2000-09-03 23:59:00.000', 6, 0, 'Iran Daylight Time', '2000-03-04 00:00:00.000', 5, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Greenland',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Greenland Standard Time', 
			StandardStart = '2000-10-05 23:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Greenland Daylight Time',
			DaylightStart = '2000-03-05 22:00:00.000',
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
			DaylightStart = '2000-03-05 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Greenland Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 56, '(GMT-03:00) Greenland', 180, 2, 'Greenland Standard Time', '2000-10-05 23:00:00.000', 6, 0, 'Greenland Daylight Time', '2000-03-05 22:00:00.000', 6, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Beirut',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Middle East Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Middle East Daylight Time',
			DaylightStart = '2000-03-05 23:59:00.000',
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
			DaylightStart = '2000-03-05 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Middle East Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 88, '(GMT+02:00) Beirut', -120, 2, 'Middle East Standard Time', '2000-10-05 23:59:00.000', 6, 0, 'Middle East Daylight Time', '2000-03-05 23:59:00.000', 6, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+01:00) Casablanca',
			Bias = -60,
			DaylightType = 1,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+01:00) Casablanca',
			Bias = -60,
			DaylightType = 1,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+01:00) Casablanca', -60, 1, 'Morocco Standard Time', NULL, NULL, 0, 'Morocco Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Damascus',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Syria Standard Time', 
			StandardStart = '2000-10-04 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Syria Daylight Time',
			DaylightStart = '2000-03-05 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Syria Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Damascus',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Syria Standard Time', 
			StandardStart = '2000-10-04 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Syria Daylight Time',
			DaylightStart = '2000-03-05 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Syria Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 96, '(GMT+02:00) Damascus', -120, 2, 'Syria Standard Time', '2000-10-04 23:59:00.000', 4, 0, 'Syria Daylight Time', '2000-03-05 00:00:00.000', 5, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+09:00) Pyongyang',
			Bias = -540,
			DaylightType = 1,
			--StandardName = 'North Korea Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'North Korea Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'North Korea Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+09:00) Pyongyang',
			Bias = -540,
			DaylightType = 1,
			--StandardName = 'North Korea Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'North Korea Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'North Korea Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 118, '(GMT+09:00) Pyongyang', -540, 1, 'North Korea Standard Time', NULL, NULL, 0, 'North Korea Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT-05:00) Turks and Caicos',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Turks and Caicos Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Turks and Caicos Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
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
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Turks and Caicos Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 128, '(GMT-05:00) Turks and Caicos', 300, 2, 'Turks and Caicos Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Turks and Caicos Daylight Time', '2000-03-02 02:00:00.000', 0, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+01:00) Sao Tome',
			Bias = -60,
			DaylightType = 1,
			--StandardName = 'Sao Tome Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Sao Tome Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Sao Tome Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+01:00) Sao Tome',
			Bias = -60,
			DaylightType = 1,
			--StandardName = 'Sao Tome Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Sao Tome Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Sao Tome Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 142, '(GMT+01:00) Sao Tome', -60, 1, 'Sao Tome Standard Time', NULL, NULL, 0, 'Sao Tome Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Volgograd',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Volgograd Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Volgograd Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Volgograd Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Volgograd',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Volgograd Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Volgograd Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Volgograd Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 143, '(GMT+04:00) Volgograd', -240, 1, 'Volgograd Standard Time', NULL, NULL, 0, 'Volgograd Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.'

GO
