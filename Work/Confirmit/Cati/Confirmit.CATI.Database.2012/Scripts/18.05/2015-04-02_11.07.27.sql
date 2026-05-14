
UPDATE BvTimezone SET  
			Name = '(GMT+03:00) Moscow, St. Petersburg, Volgograd (RTZ 2)',
			Bias = -180,
			DaylightType = 1,
			StandardName = 'Russia TZ 2 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 2 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 16

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Moscow, St. Petersburg, Volgograd (RTZ 2)',
			Bias = -180,
			DaylightType = 1,
			StandardName = 'Russia TZ 2 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 2 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 16
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 16, '(GMT+03:00) Moscow, St. Petersburg, Volgograd (RTZ 2)', -180, 1, 'Russia TZ 2 Standard Time', NULL, NULL, 0, 'Russia TZ 2 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+05:00) Ekaterinburg (RTZ 4)',
			Bias = -300,
			DaylightType = 1,
			StandardName = 'Russia TZ 4 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 4 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 22

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+05:00) Ekaterinburg (RTZ 4)',
			Bias = -300,
			DaylightType = 1,
			StandardName = 'Russia TZ 4 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 4 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 22
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 22, '(GMT+05:00) Ekaterinburg (RTZ 4)', -300, 1, 'Russia TZ 4 Standard Time', NULL, NULL, 0, 'Russia TZ 4 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+06:00) Novosibirsk (RTZ 5)',
			Bias = -360,
			DaylightType = 1,
			StandardName = 'Russia TZ 5 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 5 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 26

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+06:00) Novosibirsk (RTZ 5)',
			Bias = -360,
			DaylightType = 1,
			StandardName = 'Russia TZ 5 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 5 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 26
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 26, '(GMT+06:00) Novosibirsk (RTZ 5)', -360, 1, 'Russia TZ 5 Standard Time', NULL, NULL, 0, 'Russia TZ 5 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Krasnoyarsk (RTZ 6)',
			Bias = -420,
			DaylightType = 1,
			StandardName = 'Russia TZ 6 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 6 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 31

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Krasnoyarsk (RTZ 6)',
			Bias = -420,
			DaylightType = 1,
			StandardName = 'Russia TZ 6 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 6 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 31
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 31, '(GMT+07:00) Krasnoyarsk (RTZ 6)', -420, 1, 'Russia TZ 6 Standard Time', NULL, NULL, 0, 'Russia TZ 6 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:00) Irkutsk (RTZ 7)',
			Bias = -480,
			DaylightType = 1,
			StandardName = 'Russia TZ 7 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 7 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 33

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+08:00) Irkutsk (RTZ 7)',
			Bias = -480,
			DaylightType = 1,
			StandardName = 'Russia TZ 7 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 7 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 33
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 33, '(GMT+08:00) Irkutsk (RTZ 7)', -480, 1, 'Russia TZ 7 Standard Time', NULL, NULL, 0, 'Russia TZ 7 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+09:00) Yakutsk (RTZ 8)',
			Bias = -540,
			DaylightType = 1,
			StandardName = 'Russia TZ 8 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 8 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 39

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+09:00) Yakutsk (RTZ 8)',
			Bias = -540,
			DaylightType = 1,
			StandardName = 'Russia TZ 8 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 8 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 39
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 39, '(GMT+09:00) Yakutsk (RTZ 8)', -540, 1, 'Russia TZ 8 Standard Time', NULL, NULL, 0, 'Russia TZ 8 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-02:00) Mid-Atlantic - Old',
			Bias = 120,
			DaylightType = 2,
			StandardName = 'Mid-Atlantic Standard Time', 
			StandardStart = '2000-09-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Mid-Atlantic Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE Id = 53

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-02:00) Mid-Atlantic - Old',
			Bias = 120,
			DaylightType = 2,
			StandardName = 'Mid-Atlantic Standard Time', 
			StandardStart = '2000-09-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Mid-Atlantic Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE Id = 53
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 53, '(GMT-02:00) Mid-Atlantic - Old', 120, 2, 'Mid-Atlantic Standard Time', '2000-09-05 02:00:00.000', 0, 0, 'Mid-Atlantic Daylight Time', '2000-03-05 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Greenland',
			Bias = 180,
			DaylightType = 2,
			StandardName = 'Greenland Standard Time', 
			StandardStart = '2000-10-04 23:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Greenland Daylight Time',
			DaylightStart = '2000-03-05 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 56

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Greenland',
			Bias = 180,
			DaylightType = 2,
			StandardName = 'Greenland Standard Time', 
			StandardStart = '2000-10-04 23:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Greenland Daylight Time',
			DaylightStart = '2000-03-05 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 56
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 56, '(GMT-03:00) Greenland', 180, 2, 'Greenland Standard Time', '2000-10-04 23:00:00.000', 6, 0, 'Greenland Daylight Time', '2000-03-05 22:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Santiago',
			Bias = 240,
			DaylightType = 2,
			StandardName = 'Pacific SA Standard Time', 
			StandardStart = '2000-03-02 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-10-02 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 60

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Santiago',
			Bias = 240,
			DaylightType = 2,
			StandardName = 'Pacific SA Standard Time', 
			StandardStart = '2000-03-02 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-10-02 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 60
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 60, '(GMT-04:00) Santiago', 240, 2, 'Pacific SA Standard Time', '2000-03-02 23:59:00.000', 6, 0, 'Pacific SA Daylight Time', '2000-10-02 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-05:00) Bogota, Lima, Quito, Rio Branco',
			Bias = 300,
			DaylightType = 1,
			StandardName = 'SA Pacific Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'SA Pacific Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 61

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-05:00) Bogota, Lima, Quito, Rio Branco',
			Bias = 300,
			DaylightType = 1,
			StandardName = 'SA Pacific Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'SA Pacific Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 61
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 61, '(GMT-05:00) Bogota, Lima, Quito, Rio Branco', 300, 1, 'SA Pacific Standard Time', NULL, NULL, 0, 'SA Pacific Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Amman',
			Bias = -120,
			DaylightType = 2,
			StandardName = 'Jordan Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Jordan Daylight Time',
			DaylightStart = '2000-03-05 23:59:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE Id = 83

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Amman',
			Bias = -120,
			DaylightType = 2,
			StandardName = 'Jordan Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Jordan Daylight Time',
			DaylightStart = '2000-03-05 23:59:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE Id = 83
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 83, '(GMT+02:00) Amman', -120, 2, 'Jordan Standard Time', '2000-10-05 01:00:00.000', 5, 0, 'Jordan Daylight Time', '2000-03-05 23:59:00.000', 4, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+03:00) Minsk',
			Bias = -180,
			DaylightType = 1,
			StandardName = 'Belarus Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Belarus Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 84

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Minsk',
			Bias = -180,
			DaylightType = 1,
			StandardName = 'Belarus Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Belarus Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 84
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 84, '(GMT+03:00) Minsk', -180, 1, 'Belarus Standard Time', NULL, NULL, 0, 'Belarus Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+10:00) Magadan',
			Bias = -600,
			DaylightType = 1,
			StandardName = 'Magadan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Magadan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 86

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+10:00) Magadan',
			Bias = -600,
			DaylightType = 1,
			StandardName = 'Magadan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Magadan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 86
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 86, '(GMT+10:00) Magadan', -600, 1, 'Magadan Standard Time', NULL, NULL, 0, 'Magadan Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Beirut',
			Bias = -120,
			DaylightType = 2,
			StandardName = 'Middle East Standard Time', 
			StandardStart = '2000-10-04 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Middle East Daylight Time',
			DaylightStart = '2000-03-05 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 88

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Beirut',
			Bias = -120,
			DaylightType = 2,
			StandardName = 'Middle East Standard Time', 
			StandardStart = '2000-10-04 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Middle East Daylight Time',
			DaylightStart = '2000-03-05 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 88
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 88, '(GMT+02:00) Beirut', -120, 2, 'Middle East Standard Time', '2000-10-04 23:59:00.000', 6, 0, 'Middle East Daylight Time', '2000-03-05 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT) Casablanca',
			Bias = 0,
			DaylightType = 2,
			StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE Id = 90

UPDATE BvTimezoneMaster SET  
			Name = '(GMT) Casablanca',
			Bias = 0,
			DaylightType = 2,
			StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE Id = 90
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-03-05 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Asuncion',
			Bias = 240,
			DaylightType = 2,
			StandardName = 'Paraguay Standard Time', 
			StandardStart = '2000-03-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Paraguay Daylight Time',
			DaylightStart = '2000-10-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 95

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Asuncion',
			Bias = 240,
			DaylightType = 2,
			StandardName = 'Paraguay Standard Time', 
			StandardStart = '2000-03-03 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Paraguay Daylight Time',
			DaylightStart = '2000-10-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE Id = 95
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 95, '(GMT-04:00) Asuncion', 240, 2, 'Paraguay Standard Time', '2000-03-03 23:59:00.000', 6, 0, 'Paraguay Daylight Time', '2000-10-01 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Tripoli',
			Bias = -120,
			DaylightType = 1,
			StandardName = 'Libya Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Libya Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 104

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Tripoli',
			Bias = -120,
			DaylightType = 1,
			StandardName = 'Libya Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Libya Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE Id = 104
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 104, '(GMT+02:00) Tripoli', -120, 1, 'Libya Standard Time', NULL, NULL, 0, 'Libya Daylight Time', NULL, NULL, -60
END

    EXEC BvSpTimezoneMaster_Insert 105, '(GMT-01:00) Cabo Verde Is.', 60, 1, 'Cabo Verde Standard Time', NULL, NULL, 0, 'Cabo Verde Daylight Time', NULL, NULL, -60
    EXEC BvSpTimezoneMaster_Insert 106, '(GMT+14:00) Kiritimati Island', -840, 1, 'Line Islands Standard Time', NULL, NULL, 0, 'Line Islands Daylight Time', NULL, NULL, -60
    EXEC BvSpTimezoneMaster_Insert 107, '(GMT+02:00) Kaliningrad (RTZ 1)', -120, 1, 'Russia TZ 1 Standard Time', NULL, NULL, 0, 'Russia TZ 1 Daylight Time', NULL, NULL, -60
    EXEC BvSpTimezoneMaster_Insert 108, '(GMT+11:00) Chokurdakh (RTZ 10)', -660, 1, 'Russia TZ 10 Standard Time', NULL, NULL, 0, 'Russia TZ 10 Daylight Time', NULL, NULL, -60
    EXEC BvSpTimezoneMaster_Insert 109, '(GMT+12:00) Anadyr, Petropavlovsk-Kamchatsky (RTZ 11)', -720, 1, 'Russia TZ 11 Standard Time', NULL, NULL, 0, 'Russia TZ 11 Daylight Time', NULL, NULL, -60
    EXEC BvSpTimezoneMaster_Insert 110, '(GMT+04:00) Izhevsk, Samara (RTZ 3)', -240, 1, 'Russia TZ 3 Standard Time', NULL, NULL, 0, 'Russia TZ 3 Daylight Time', NULL, NULL, -60
    EXEC BvSpTimezoneMaster_Insert 111, '(GMT+10:00) Vladivostok, Magadan (RTZ 9)', -600, 1, 'Russia TZ 9 Standard Time', NULL, NULL, 0, 'Russia TZ 9 Daylight Time', NULL, NULL, -60


GO
PRINT N'Update complete.';


GO
