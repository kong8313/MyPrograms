GO
PRINT N'Update timezone list';

GO


UPDATE BvTimezone SET  
			Name = '(GMT) Dublin, Edinburgh, Lisbon, London',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'GMT Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'GMT Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'GMT Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT) Dublin, Edinburgh, Lisbon, London',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'GMT Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'GMT Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'GMT Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 1, '(GMT) Dublin, Edinburgh, Lisbon, London', 0, 2, 'GMT Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'GMT Daylight Time', '2000-03-05 01:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) E. Europe',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'E. Europe Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'E. Europe Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'E. Europe Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) E. Europe',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'E. Europe Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'E. Europe Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'E. Europe Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 9, '(GMT+02:00) E. Europe', -120, 2, 'E. Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'E. Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
END

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

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Yerevan',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Caucasus Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Caucasus Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Caucasus Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Yerevan',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Caucasus Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Caucasus Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Caucasus Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 20, '(GMT+04:00) Yerevan', -240, 1, 'Caucasus Standard Time', NULL, NULL, 0, 'Caucasus Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+05:00) Ashgabat, Tashkent',
			Bias = -300,
			DaylightType = 1,
			--StandardName = 'West Asia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'West Asia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'West Asia Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+05:00) Ashgabat, Tashkent',
			Bias = -300,
			DaylightType = 1,
			--StandardName = 'West Asia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'West Asia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'West Asia Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 23, '(GMT+05:00) Ashgabat, Tashkent', -300, 1, 'West Asia Standard Time', NULL, NULL, 0, 'West Asia Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+12:00) Fiji',
			Bias = -720,
			DaylightType = 2,
			--StandardName = 'Fiji Standard Time', 
			StandardStart = '2000-01-03 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Fiji Daylight Time',
			DaylightStart = '2000-10-04 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Fiji Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+12:00) Fiji',
			Bias = -720,
			DaylightType = 2,
			--StandardName = 'Fiji Standard Time', 
			StandardStart = '2000-01-03 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Fiji Daylight Time',
			DaylightStart = '2000-10-04 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Fiji Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 49, '(GMT+12:00) Fiji', -720, 2, 'Fiji Standard Time', '2000-01-03 03:00:00.000', 0, 0, 'Fiji Daylight Time', '2000-10-04 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-01:00) Azores',
			Bias = 60,
			DaylightType = 2,
			--StandardName = 'Azores Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Azores Daylight Time',
			DaylightStart = '2000-03-05 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Azores Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-01:00) Azores',
			Bias = 60,
			DaylightType = 2,
			--StandardName = 'Azores Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Azores Daylight Time',
			DaylightStart = '2000-03-05 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Azores Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 51, '(GMT-01:00) Azores', 60, 2, 'Azores Standard Time', '2000-10-05 01:00:00.000', 0, 0, 'Azores Daylight Time', '2000-03-05 00:00:00.000', 0, -60
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

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Santiago',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Pacific SA Standard Time', 
			StandardStart = '2000-04-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-09-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Santiago',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Pacific SA Standard Time', 
			StandardStart = '2000-04-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-09-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 60, '(GMT-04:00) Santiago', 240, 2, 'Pacific SA Standard Time', '2000-04-05 23:59:00.000', 6, 0, 'Pacific SA Daylight Time', '2000-09-01 23:59:00.000', 6, -60
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
			Name = '(GMT+03:00) Amman',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Jordan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Jordan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Jordan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Amman',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Jordan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Jordan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Jordan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 83, '(GMT+03:00) Amman', -180, 1, 'Jordan Standard Time', NULL, NULL, 0, 'Jordan Daylight Time', NULL, NULL, -60
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

UPDATE BvTimezone SET  
			Name = '(GMT) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-09-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-04-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-09-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-04-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT) Casablanca', 0, 2, 'Morocco Standard Time', '2000-09-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-04-05 02:00:00.000', 0, -60
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
			Name = '(GMT-03:00) Salvador',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Bahia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Bahia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Bahia Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Salvador',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Bahia Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Bahia Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Bahia Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 103, '(GMT-03:00) Salvador', 180, 1, 'Bahia Standard Time', NULL, NULL, 0, 'Bahia Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+01:00) Tripoli',
			Bias = -60,
			DaylightType = 2,
			--StandardName = 'Libya Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Libya Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Libya Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+01:00) Tripoli',
			Bias = -60,
			DaylightType = 2,
			--StandardName = 'Libya Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Libya Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Libya Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 104, '(GMT+01:00) Tripoli', -60, 2, 'Libya Standard Time', '2000-10-05 02:00:00.000', 5, 0, 'Libya Daylight Time', '2000-03-05 01:00:00.000', 5, -60
END


GO
PRINT N'Altering [dbo].[BvSpGetVersion]...';


GO
 ALTER PROCEDURE [dbo].[BvSpGetVersion]
 AS
 SELECT 'Confirmit Horizons 18.0.0.0'
 RETURN (0)
GO
PRINT N'Update complete.';


GO
