

UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Dublin, Edinburgh, Lisbon, London',
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
			Name = '(GMT+00:00) Dublin, Edinburgh, Lisbon, London',
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
    EXEC BvSpTimezoneMaster_Insert 1, '(GMT+00:00) Dublin, Edinburgh, Lisbon, London', 0, 2, 'GMT Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'GMT Daylight Time', '2000-03-05 01:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+00:00) Monrovia, Reykjavik',
			Bias = 0,
			DaylightType = 1,
			--StandardName = 'Greenwich Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Greenwich Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Greenwich Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+00:00) Monrovia, Reykjavik',
			Bias = 0,
			DaylightType = 1,
			--StandardName = 'Greenwich Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Greenwich Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Greenwich Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 2, '(GMT+00:00) Monrovia, Reykjavik', 0, 1, 'Greenwich Standard Time', NULL, NULL, 0, 'Greenwich Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Chisinau',
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
			Name = '(GMT+02:00) Chisinau',
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
    EXEC BvSpTimezoneMaster_Insert 9, '(GMT+02:00) Chisinau', -120, 2, 'E. Europe Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'E. Europe Daylight Time', '2000-03-05 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Cairo',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Egypt Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Egypt Daylight Time',
			DaylightStart = '2000-07-01 23:59:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE StandardName = 'Egypt Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Cairo',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Egypt Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Egypt Daylight Time',
			DaylightStart = '2000-07-01 23:59:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE StandardName = 'Egypt Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 10, '(GMT+02:00) Cairo', -120, 2, 'Egypt Standard Time', '2000-10-05 23:59:00.000', 4, 0, 'Egypt Daylight Time', '2000-07-01 23:59:00.000', 4, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+03:00) Moscow, St. Petersburg, Volgograd',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Russia TZ 2 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 2 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 2 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Moscow, St. Petersburg, Volgograd',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Russia TZ 2 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 2 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 2 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 16, '(GMT+03:00) Moscow, St. Petersburg, Volgograd', -180, 1, 'Russia TZ 2 Standard Time', NULL, NULL, 0, 'Russia TZ 2 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 2, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-03 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 2, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-03 00:00:00.000',
			DaylightDayOfWeek = 1,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 18, '(GMT+03:30) Tehran', -210, 2, 'Iran Standard Time', '2000-09-03 23:59:00.000', 2, 0, 'Iran Daylight Time', '2000-03-03 00:00:00.000', 1, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+05:00) Ekaterinburg',
			Bias = -300,
			DaylightType = 1,
			--StandardName = 'Russia TZ 4 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 4 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 4 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+05:00) Ekaterinburg',
			Bias = -300,
			DaylightType = 1,
			--StandardName = 'Russia TZ 4 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 4 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 4 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 22, '(GMT+05:00) Ekaterinburg', -300, 1, 'Russia TZ 4 Standard Time', NULL, NULL, 0, 'Russia TZ 4 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+06:00) Novosibirsk',
			Bias = -360,
			DaylightType = 1,
			--StandardName = 'Russia TZ 5 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 5 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 5 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+06:00) Novosibirsk',
			Bias = -360,
			DaylightType = 1,
			--StandardName = 'Russia TZ 5 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 5 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 5 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 26, '(GMT+06:00) Novosibirsk', -360, 1, 'Russia TZ 5 Standard Time', NULL, NULL, 0, 'Russia TZ 5 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Krasnoyarsk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Russia TZ 6 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 6 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 6 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Krasnoyarsk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Russia TZ 6 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 6 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 6 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 31, '(GMT+07:00) Krasnoyarsk', -420, 1, 'Russia TZ 6 Standard Time', NULL, NULL, 0, 'Russia TZ 6 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:00) Irkutsk',
			Bias = -480,
			DaylightType = 1,
			--StandardName = 'Russia TZ 7 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 7 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 7 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+08:00) Irkutsk',
			Bias = -480,
			DaylightType = 1,
			--StandardName = 'Russia TZ 7 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 7 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 7 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 33, '(GMT+08:00) Irkutsk', -480, 1, 'Russia TZ 7 Standard Time', NULL, NULL, 0, 'Russia TZ 7 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+09:00) Yakutsk',
			Bias = -540,
			DaylightType = 1,
			--StandardName = 'Russia TZ 8 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 8 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 8 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+09:00) Yakutsk',
			Bias = -540,
			DaylightType = 1,
			--StandardName = 'Russia TZ 8 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 8 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 8 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 39, '(GMT+09:00) Yakutsk', -540, 1, 'Russia TZ 8 Standard Time', NULL, NULL, 0, 'Russia TZ 8 Daylight Time', NULL, NULL, -60
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
			DaylightStart = '2000-11-01 02:00:00.000',
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
			DaylightStart = '2000-11-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Fiji Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 49, '(GMT+12:00) Fiji', -720, 2, 'Fiji Standard Time', '2000-01-03 03:00:00.000', 0, 0, 'Fiji Daylight Time', '2000-11-01 02:00:00.000', 0, -60
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
			StandardStart = '2000-05-02 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-08-02 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Santiago',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Pacific SA Standard Time', 
			StandardStart = '2000-05-02 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-08-02 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 60, '(GMT-04:00) Santiago', 240, 2, 'Pacific SA Standard Time', '2000-05-02 23:59:00.000', 6, 0, 'Pacific SA Daylight Time', '2000-08-02 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+13:00) Samoa',
			Bias = -780,
			DaylightType = 2,
			--StandardName = 'Samoa Standard Time', 
			StandardStart = '2000-04-01 04:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Samoa Daylight Time',
			DaylightStart = '2000-09-05 03:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Samoa Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+13:00) Samoa',
			Bias = -780,
			DaylightType = 2,
			--StandardName = 'Samoa Standard Time', 
			StandardStart = '2000-04-01 04:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Samoa Daylight Time',
			DaylightStart = '2000-09-05 03:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Samoa Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 74, '(GMT+13:00) Samoa', -780, 2, 'Samoa Standard Time', '2000-04-01 04:00:00.000', 0, 0, 'Samoa Daylight Time', '2000-09-05 03:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) City of Buenos Aires',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Argentina Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Argentina Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Argentina Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) City of Buenos Aires',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Argentina Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Argentina Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Argentina Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 76, '(GMT-03:00) City of Buenos Aires', 180, 1, 'Argentina Standard Time', NULL, NULL, 0, 'Argentina Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Baku',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Azerbaijan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Azerbaijan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Azerbaijan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Baku',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Azerbaijan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Azerbaijan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Azerbaijan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 77, '(GMT+04:00) Baku', -240, 1, 'Azerbaijan Standard Time', NULL, NULL, 0, 'Azerbaijan Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Magadan',
			Bias = -660,
			DaylightType = 2,
			--StandardName = 'Magadan Standard Time', 
			StandardStart = '2000-04-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Magadan Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Magadan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Magadan',
			Bias = -660,
			DaylightType = 2,
			--StandardName = 'Magadan Standard Time', 
			StandardStart = '2000-04-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Magadan Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Magadan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 86, '(GMT+11:00) Magadan', -660, 2, 'Magadan Standard Time', '2000-04-05 02:00:00.000', 0, 0, 'Magadan Daylight Time', '2000-01-01 00:00:00.000', 5, 60
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
			Name = '(GMT-03:00) Montevideo',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Montevideo Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Montevideo Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Montevideo Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Montevideo',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Montevideo Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Montevideo Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Montevideo Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 89, '(GMT-03:00) Montevideo', 180, 1, 'Montevideo Standard Time', NULL, NULL, 0, 'Montevideo Daylight Time', NULL, NULL, -60
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
			DaylightStart = '2000-07-02 02:00:00.000',
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
			DaylightStart = '2000-07-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT+00:00) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-07-02 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-08:00) Baja California',
			Bias = 480,
			DaylightType = 2,
			--StandardName = 'Pacific Standard Time (Mexico)', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Pacific Daylight Time (Mexico)',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Pacific Standard Time (Mexico)'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-08:00) Baja California',
			Bias = 480,
			DaylightType = 2,
			--StandardName = 'Pacific Standard Time (Mexico)', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Pacific Daylight Time (Mexico)',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Pacific Standard Time (Mexico)'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 93, '(GMT-08:00) Baja California', 480, 2, 'Pacific Standard Time (Mexico)', '2000-11-01 02:00:00.000', 0, 0, 'Pacific Daylight Time (Mexico)', '2000-03-02 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Asuncion',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Paraguay Standard Time', 
			StandardStart = '2000-03-05 23:59:00.000',
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
			StandardStart = '2000-03-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Paraguay Daylight Time',
			DaylightStart = '2000-10-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Paraguay Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 95, '(GMT-04:00) Asuncion', 240, 2, 'Paraguay Standard Time', '2000-03-05 23:59:00.000', 6, 0, 'Paraguay Daylight Time', '2000-10-01 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Damascus',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Syria Standard Time', 
			StandardStart = '2000-10-05 23:59:00.000',
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
			StandardStart = '2000-10-05 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Syria Daylight Time',
			DaylightStart = '2000-03-05 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Syria Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 96, '(GMT+02:00) Damascus', -120, 2, 'Syria Standard Time', '2000-10-05 23:59:00.000', 4, 0, 'Syria Daylight Time', '2000-03-05 00:00:00.000', 5, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:00) Ulaanbaatar',
			Bias = -480,
			DaylightType = 2,
			--StandardName = 'Ulaanbaatar Standard Time', 
			StandardStart = '2000-09-04 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Ulaanbaatar Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Ulaanbaatar Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+08:00) Ulaanbaatar',
			Bias = -480,
			DaylightType = 2,
			--StandardName = 'Ulaanbaatar Standard Time', 
			StandardStart = '2000-09-04 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Ulaanbaatar Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Ulaanbaatar Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 98, '(GMT+08:00) Ulaanbaatar', -480, 2, 'Ulaanbaatar Standard Time', '2000-09-04 23:59:00.000', 5, 0, 'Ulaanbaatar Daylight Time', '2000-03-05 02:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Caracas',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Venezuela Standard Time', 
			StandardStart = '2000-05-01 02:30:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Venezuela Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 30
		WHERE StandardName = 'Venezuela Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Caracas',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Venezuela Standard Time', 
			StandardStart = '2000-05-01 02:30:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Venezuela Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 30
		WHERE StandardName = 'Venezuela Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 102, '(GMT-04:00) Caracas', 240, 2, 'Venezuela Standard Time', '2000-05-01 02:30:00.000', 0, 0, 'Venezuela Daylight Time', '2000-01-01 00:00:00.000', 5, 30
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Kaliningrad',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Russia TZ 1 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 1 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 1 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Kaliningrad',
			Bias = -120,
			DaylightType = 1,
			--StandardName = 'Russia TZ 1 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 1 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 1 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 107, '(GMT+02:00) Kaliningrad', -120, 1, 'Russia TZ 1 Standard Time', NULL, NULL, 0, 'Russia TZ 1 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Chokurdakh',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Russia TZ 10 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 10 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 10 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Chokurdakh',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Russia TZ 10 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 10 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 10 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 108, '(GMT+11:00) Chokurdakh', -660, 1, 'Russia TZ 10 Standard Time', NULL, NULL, 0, 'Russia TZ 10 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+12:00) Anadyr, Petropavlovsk-Kamchatsky',
			Bias = -720,
			DaylightType = 1,
			--StandardName = 'Russia TZ 11 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 11 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 11 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+12:00) Anadyr, Petropavlovsk-Kamchatsky',
			Bias = -720,
			DaylightType = 1,
			--StandardName = 'Russia TZ 11 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 11 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 11 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 109, '(GMT+12:00) Anadyr, Petropavlovsk-Kamchatsky', -720, 1, 'Russia TZ 11 Standard Time', NULL, NULL, 0, 'Russia TZ 11 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Izhevsk, Samara',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Russia TZ 3 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 3 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 3 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Izhevsk, Samara',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Russia TZ 3 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 3 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 3 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 110, '(GMT+04:00) Izhevsk, Samara', -240, 1, 'Russia TZ 3 Standard Time', NULL, NULL, 0, 'Russia TZ 3 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+10:00) Vladivostok',
			Bias = -600,
			DaylightType = 1,
			--StandardName = 'Russia TZ 9 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 9 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 9 Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+10:00) Vladivostok',
			Bias = -600,
			DaylightType = 1,
			--StandardName = 'Russia TZ 9 Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Russia TZ 9 Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Russia TZ 9 Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 111, '(GMT+10:00) Vladivostok', -600, 1, 'Russia TZ 9 Standard Time', NULL, NULL, 0, 'Russia TZ 9 Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Gaza, Hebron',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'West Bank Gaza Standard Time', 
			StandardStart = '2000-10-03 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'West Bank Gaza Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'West Bank Gaza Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+02:00) Gaza, Hebron',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'West Bank Gaza Standard Time', 
			StandardStart = '2000-10-03 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'West Bank Gaza Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'West Bank Gaza Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 113, '(GMT+02:00) Gaza, Hebron', -120, 2, 'West Bank Gaza Standard Time', '2000-10-03 23:59:00.000', 4, 0, 'West Bank Gaza Daylight Time', '2000-03-05 01:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Astrakhan, Ulyanovsk',
			Bias = -240,
			DaylightType = 2,
			--StandardName = 'Astrakhan Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Astrakhan Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Astrakhan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Astrakhan, Ulyanovsk',
			Bias = -240,
			DaylightType = 2,
			--StandardName = 'Astrakhan Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Astrakhan Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Astrakhan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 114, '(GMT+04:00) Astrakhan, Ulyanovsk', -240, 2, 'Astrakhan Standard Time', '2000-03-05 02:00:00.000', 0, 0, 'Astrakhan Daylight Time', '2000-01-01 00:00:00.000', 5, 60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Barnaul, Gorno-Altaysk',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'Altai Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Altai Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Altai Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Barnaul, Gorno-Altaysk',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'Altai Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Altai Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Altai Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 115, '(GMT+07:00) Barnaul, Gorno-Altaysk', -420, 2, 'Altai Standard Time', '2000-03-05 02:00:00.000', 0, 0, 'Altai Daylight Time', '2000-01-01 00:00:00.000', 5, 60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Hovd',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'W. Mongolia Standard Time', 
			StandardStart = '2000-09-04 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'W. Mongolia Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'W. Mongolia Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Hovd',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'W. Mongolia Standard Time', 
			StandardStart = '2000-09-04 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'W. Mongolia Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'W. Mongolia Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 116, '(GMT+07:00) Hovd', -420, 2, 'W. Mongolia Standard Time', '2000-09-04 23:59:00.000', 5, 0, 'W. Mongolia Daylight Time', '2000-03-05 02:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Tomsk',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'Tomsk Standard Time', 
			StandardStart = '2000-05-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Tomsk Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Tomsk Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Tomsk',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'Tomsk Standard Time', 
			StandardStart = '2000-05-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Tomsk Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Tomsk Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 117, '(GMT+07:00) Tomsk', -420, 2, 'Tomsk Standard Time', '2000-05-05 02:00:00.000', 0, 0, 'Tomsk Daylight Time', '2000-01-01 00:00:00.000', 5, 60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:30) Pyongyang',
			Bias = -510,
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
			Name = '(GMT+08:30) Pyongyang',
			Bias = -510,
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
    EXEC BvSpTimezoneMaster_Insert 118, '(GMT+08:30) Pyongyang', -510, 1, 'North Korea Standard Time', NULL, NULL, 0, 'North Korea Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:45) Eucla',
			Bias = -525,
			DaylightType = 1,
			--StandardName = 'Aus Central W. Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Aus Central W. Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Aus Central W. Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+08:45) Eucla',
			Bias = -525,
			DaylightType = 1,
			--StandardName = 'Aus Central W. Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Aus Central W. Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Aus Central W. Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 119, '(GMT+08:45) Eucla', -525, 1, 'Aus Central W. Standard Time', NULL, NULL, 0, 'Aus Central W. Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+09:00) Chita',
			Bias = -540,
			DaylightType = 2,
			--StandardName = 'Transbaikal Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Transbaikal Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Transbaikal Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+09:00) Chita',
			Bias = -540,
			DaylightType = 2,
			--StandardName = 'Transbaikal Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Transbaikal Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Transbaikal Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 120, '(GMT+09:00) Chita', -540, 2, 'Transbaikal Standard Time', '2000-03-05 02:00:00.000', 0, 0, 'Transbaikal Daylight Time', '2000-01-01 00:00:00.000', 5, 60
END

UPDATE BvTimezone SET  
			Name = '(GMT+10:30) Lord Howe Island',
			Bias = -630,
			DaylightType = 2,
			--StandardName = 'Lord Howe Standard Time', 
			StandardStart = '2000-04-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Lord Howe Daylight Time',
			DaylightStart = '2000-10-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -30
		WHERE StandardName = 'Lord Howe Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+10:30) Lord Howe Island',
			Bias = -630,
			DaylightType = 2,
			--StandardName = 'Lord Howe Standard Time', 
			StandardStart = '2000-04-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Lord Howe Daylight Time',
			DaylightStart = '2000-10-01 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -30
		WHERE StandardName = 'Lord Howe Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 121, '(GMT+10:30) Lord Howe Island', -630, 2, 'Lord Howe Standard Time', '2000-04-01 02:00:00.000', 0, 0, 'Lord Howe Daylight Time', '2000-10-01 02:00:00.000', 0, -30
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Bougainville Island',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Bougainville Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Bougainville Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Bougainville Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Bougainville Island',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Bougainville Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Bougainville Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Bougainville Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 122, '(GMT+11:00) Bougainville Island', -660, 1, 'Bougainville Standard Time', NULL, NULL, 0, 'Bougainville Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Norfolk Island',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Norfolk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Norfolk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Norfolk Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Norfolk Island',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Norfolk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Norfolk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Norfolk Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 123, '(GMT+11:00) Norfolk Island', -660, 1, 'Norfolk Standard Time', NULL, NULL, 0, 'Norfolk Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Sakhalin',
			Bias = -660,
			DaylightType = 2,
			--StandardName = 'Sakhalin Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Sakhalin Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Sakhalin Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Sakhalin',
			Bias = -660,
			DaylightType = 2,
			--StandardName = 'Sakhalin Standard Time', 
			StandardStart = '2000-03-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Sakhalin Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = 60
		WHERE StandardName = 'Sakhalin Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 124, '(GMT+11:00) Sakhalin', -660, 2, 'Sakhalin Standard Time', '2000-03-05 02:00:00.000', 0, 0, 'Sakhalin Daylight Time', '2000-01-01 00:00:00.000', 5, 60
END

UPDATE BvTimezone SET  
			Name = '(GMT+12:45) Chatham Islands',
			Bias = -765,
			DaylightType = 2,
			--StandardName = 'Chatham Islands Standard Time', 
			StandardStart = '2000-04-01 03:45:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Chatham Islands Daylight Time',
			DaylightStart = '2000-09-05 02:45:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Chatham Islands Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+12:45) Chatham Islands',
			Bias = -765,
			DaylightType = 2,
			--StandardName = 'Chatham Islands Standard Time', 
			StandardStart = '2000-04-01 03:45:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Chatham Islands Daylight Time',
			DaylightStart = '2000-09-05 02:45:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Chatham Islands Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 125, '(GMT+12:45) Chatham Islands', -765, 2, 'Chatham Islands Standard Time', '2000-04-01 03:45:00.000', 0, 0, 'Chatham Islands Daylight Time', '2000-09-05 02:45:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Araguaina',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Tocantins Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Tocantins Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Tocantins Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Araguaina',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Tocantins Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Tocantins Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Tocantins Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 126, '(GMT-03:00) Araguaina', 180, 1, 'Tocantins Standard Time', NULL, NULL, 0, 'Tocantins Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Saint Pierre and Miquelon',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Saint Pierre Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Saint Pierre Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Saint Pierre Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Saint Pierre and Miquelon',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Saint Pierre Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Saint Pierre Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Saint Pierre Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 127, '(GMT-03:00) Saint Pierre and Miquelon', 180, 2, 'Saint Pierre Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Saint Pierre Daylight Time', '2000-03-02 02:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Turks and Caicos',
			Bias = 240,
			DaylightType = 1,
			--StandardName = 'Turks and Caicos Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Turks and Caicos Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Turks and Caicos Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Turks and Caicos',
			Bias = 240,
			DaylightType = 1,
			--StandardName = 'Turks and Caicos Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Turks and Caicos Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Turks and Caicos Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 128, '(GMT-04:00) Turks and Caicos', 240, 1, 'Turks and Caicos Standard Time', NULL, NULL, 0, 'Turks and Caicos Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-05:00) Haiti',
			Bias = 300,
			DaylightType = 1,
			--StandardName = 'Haiti Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Haiti Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Haiti Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-05:00) Haiti',
			Bias = 300,
			DaylightType = 1,
			--StandardName = 'Haiti Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Haiti Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Haiti Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 129, '(GMT-05:00) Haiti', 300, 1, 'Haiti Standard Time', NULL, NULL, 0, 'Haiti Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-05:00) Havana',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Cuba Standard Time', 
			StandardStart = '2000-11-01 01:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Cuba Daylight Time',
			DaylightStart = '2000-03-02 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Cuba Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-05:00) Havana',
			Bias = 300,
			DaylightType = 2,
			--StandardName = 'Cuba Standard Time', 
			StandardStart = '2000-11-01 01:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Cuba Daylight Time',
			DaylightStart = '2000-03-02 00:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Cuba Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 130, '(GMT-05:00) Havana', 300, 2, 'Cuba Standard Time', '2000-11-01 01:00:00.000', 0, 0, 'Cuba Daylight Time', '2000-03-02 00:00:00.000', 0, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-06:00) Easter Island',
			Bias = 360,
			DaylightType = 2,
			--StandardName = 'Easter Island Standard Time', 
			StandardStart = '2000-05-02 22:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Easter Island Daylight Time',
			DaylightStart = '2000-08-02 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Easter Island Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-06:00) Easter Island',
			Bias = 360,
			DaylightType = 2,
			--StandardName = 'Easter Island Standard Time', 
			StandardStart = '2000-05-02 22:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Easter Island Daylight Time',
			DaylightStart = '2000-08-02 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Easter Island Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 131, '(GMT-06:00) Easter Island', 360, 2, 'Easter Island Standard Time', '2000-05-02 22:00:00.000', 6, 0, 'Easter Island Daylight Time', '2000-08-02 22:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-08:00) Coordinated Universal Time-08',
			Bias = 480,
			DaylightType = 1,
			--StandardName = 'UTC-08', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'UTC-08',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'UTC-08'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-08:00) Coordinated Universal Time-08',
			Bias = 480,
			DaylightType = 1,
			--StandardName = 'UTC-08', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'UTC-08',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'UTC-08'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 132, '(GMT-08:00) Coordinated Universal Time-08', 480, 1, 'UTC-08', NULL, NULL, 0, 'UTC-08', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-09:00) Coordinated Universal Time-09',
			Bias = 540,
			DaylightType = 1,
			--StandardName = 'UTC-09', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'UTC-09',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'UTC-09'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-09:00) Coordinated Universal Time-09',
			Bias = 540,
			DaylightType = 1,
			--StandardName = 'UTC-09', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'UTC-09',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'UTC-09'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 133, '(GMT-09:00) Coordinated Universal Time-09', 540, 1, 'UTC-09', NULL, NULL, 0, 'UTC-09', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-09:30) Marquesas Islands',
			Bias = 570,
			DaylightType = 1,
			--StandardName = 'Marquesas Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Marquesas Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Marquesas Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-09:30) Marquesas Islands',
			Bias = 570,
			DaylightType = 1,
			--StandardName = 'Marquesas Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Marquesas Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Marquesas Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 134, '(GMT-09:30) Marquesas Islands', 570, 1, 'Marquesas Standard Time', NULL, NULL, 0, 'Marquesas Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-10:00) Aleutian Islands',
			Bias = 600,
			DaylightType = 2,
			--StandardName = 'Aleutian Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Aleutian Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Aleutian Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-10:00) Aleutian Islands',
			Bias = 600,
			DaylightType = 2,
			--StandardName = 'Aleutian Standard Time', 
			StandardStart = '2000-11-01 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Aleutian Daylight Time',
			DaylightStart = '2000-03-02 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Aleutian Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 135, '(GMT-10:00) Aleutian Islands', 600, 2, 'Aleutian Standard Time', '2000-11-01 02:00:00.000', 0, 0, 'Aleutian Daylight Time', '2000-03-02 02:00:00.000', 0, -60
END


GO
PRINT N'Update complete.';


GO
