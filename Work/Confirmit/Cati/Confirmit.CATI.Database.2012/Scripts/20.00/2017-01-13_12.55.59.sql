
UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Jerusalem',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'Jerusalem Standard Time', 
			StandardStart = '2000-10-05 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Jerusalem Daylight Time',
			DaylightStart = '2000-03-04 02:00:00.000',
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
			DaylightStart = '2000-03-04 02:00:00.000',
			DaylightDayOfWeek = 5,
			DaylightBias = -60
		WHERE StandardName = 'Jerusalem Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 13, '(GMT+02:00) Jerusalem', -120, 2, 'Jerusalem Standard Time', '2000-10-05 02:00:00.000', 0, 0, 'Jerusalem Daylight Time', '2000-03-04 02:00:00.000', 5, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-04 00:00:00.000',
			DaylightDayOfWeek = 3,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:30) Tehran',
			Bias = -210,
			DaylightType = 2,
			--StandardName = 'Iran Standard Time', 
			StandardStart = '2000-09-03 23:59:00.000',
			StandardDayOfWeek = 4, 
			StandardBias = 0,
			DaylightName = 'Iran Daylight Time',
			DaylightStart = '2000-03-04 00:00:00.000',
			DaylightDayOfWeek = 3,
			DaylightBias = -60
		WHERE StandardName = 'Iran Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 18, '(GMT+03:30) Tehran', -210, 2, 'Iran Standard Time', '2000-09-03 23:59:00.000', 4, 0, 'Iran Daylight Time', '2000-03-04 00:00:00.000', 3, -60
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
			DaylightStart = '2000-10-02 23:59:00.000',
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
			DaylightStart = '2000-10-02 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'E. South America Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 54, '(GMT-03:00) Brasilia', 180, 2, 'E. South America Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'E. South America Daylight Time', '2000-10-02 23:59:00.000', 6, -60
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
			DaylightStart = '2000-10-02 23:59:00.000',
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
			DaylightStart = '2000-10-02 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Central Brazilian Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 79, '(GMT-04:00) Cuiaba', 240, 2, 'Central Brazilian Standard Time', '2000-02-03 23:59:00.000', 6, 0, 'Central Brazilian Daylight Time', '2000-10-02 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Magadan',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Magadan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Magadan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Magadan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Magadan',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Magadan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Magadan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Magadan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 86, '(GMT+11:00) Magadan', -660, 1, 'Magadan Standard Time', NULL, NULL, 0, 'Magadan Daylight Time', NULL, NULL, -60
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
			StandardStart = '2000-03-05 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Paraguay Daylight Time',
			DaylightStart = '2000-09-05 23:59:00.000',
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
			DaylightStart = '2000-09-05 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Paraguay Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 95, '(GMT-04:00) Asuncion', 240, 2, 'Paraguay Standard Time', '2000-03-05 23:59:00.000', 6, 0, 'Paraguay Daylight Time', '2000-09-05 23:59:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+03:00) Istanbul',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Turkey Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Turkey Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Turkey Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+03:00) Istanbul',
			Bias = -180,
			DaylightType = 1,
			--StandardName = 'Turkey Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Turkey Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Turkey Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 97, '(GMT+03:00) Istanbul', -180, 1, 'Turkey Standard Time', NULL, NULL, 0, 'Turkey Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+08:00) Ulaanbaatar',
			Bias = -480,
			DaylightType = 2,
			--StandardName = 'Ulaanbaatar Standard Time', 
			StandardStart = '2000-09-05 23:59:00.000',
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
			StandardStart = '2000-09-05 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'Ulaanbaatar Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Ulaanbaatar Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 98, '(GMT+08:00) Ulaanbaatar', -480, 2, 'Ulaanbaatar Standard Time', '2000-09-05 23:59:00.000', 5, 0, 'Ulaanbaatar Daylight Time', '2000-03-05 02:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Caracas',
			Bias = 240,
			DaylightType = 1,
			--StandardName = 'Venezuela Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Venezuela Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Venezuela Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-04:00) Caracas',
			Bias = 240,
			DaylightType = 1,
			--StandardName = 'Venezuela Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Venezuela Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Venezuela Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 102, '(GMT-04:00) Caracas', 240, 1, 'Venezuela Standard Time', NULL, NULL, 0, 'Venezuela Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+02:00) Gaza, Hebron',
			Bias = -120,
			DaylightType = 2,
			--StandardName = 'West Bank Gaza Standard Time', 
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 6, 
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
			StandardStart = '2000-10-05 01:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'West Bank Gaza Daylight Time',
			DaylightStart = '2000-03-05 01:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'West Bank Gaza Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 113, '(GMT+02:00) Gaza, Hebron', -120, 2, 'West Bank Gaza Standard Time', '2000-10-05 01:00:00.000', 6, 0, 'West Bank Gaza Daylight Time', '2000-03-05 01:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+04:00) Astrakhan, Ulyanovsk',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Astrakhan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Astrakhan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Astrakhan Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+04:00) Astrakhan, Ulyanovsk',
			Bias = -240,
			DaylightType = 1,
			--StandardName = 'Astrakhan Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Astrakhan Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Astrakhan Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 114, '(GMT+04:00) Astrakhan, Ulyanovsk', -240, 1, 'Astrakhan Standard Time', NULL, NULL, 0, 'Astrakhan Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Barnaul, Gorno-Altaysk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Altai Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Altai Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Altai Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Barnaul, Gorno-Altaysk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Altai Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Altai Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Altai Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 115, '(GMT+07:00) Barnaul, Gorno-Altaysk', -420, 1, 'Altai Standard Time', NULL, NULL, 0, 'Altai Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Hovd',
			Bias = -420,
			DaylightType = 2,
			--StandardName = 'W. Mongolia Standard Time', 
			StandardStart = '2000-09-05 23:59:00.000',
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
			StandardStart = '2000-09-05 23:59:00.000',
			StandardDayOfWeek = 5, 
			StandardBias = 0,
			DaylightName = 'W. Mongolia Daylight Time',
			DaylightStart = '2000-03-05 02:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'W. Mongolia Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 116, '(GMT+07:00) Hovd', -420, 2, 'W. Mongolia Standard Time', '2000-09-05 23:59:00.000', 5, 0, 'W. Mongolia Daylight Time', '2000-03-05 02:00:00.000', 6, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Tomsk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Tomsk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Tomsk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Tomsk Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Tomsk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Tomsk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Tomsk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Tomsk Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 117, '(GMT+07:00) Tomsk', -420, 1, 'Tomsk Standard Time', NULL, NULL, 0, 'Tomsk Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+09:00) Chita',
			Bias = -540,
			DaylightType = 1,
			--StandardName = 'Transbaikal Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Transbaikal Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Transbaikal Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+09:00) Chita',
			Bias = -540,
			DaylightType = 1,
			--StandardName = 'Transbaikal Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Transbaikal Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Transbaikal Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 120, '(GMT+09:00) Chita', -540, 1, 'Transbaikal Standard Time', NULL, NULL, 0, 'Transbaikal Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+11:00) Sakhalin',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Sakhalin Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Sakhalin Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Sakhalin Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+11:00) Sakhalin',
			Bias = -660,
			DaylightType = 1,
			--StandardName = 'Sakhalin Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Sakhalin Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Sakhalin Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 124, '(GMT+11:00) Sakhalin', -660, 1, 'Sakhalin Standard Time', NULL, NULL, 0, 'Sakhalin Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+06:00) Omsk',
			Bias = -360,
			DaylightType = 1,
			--StandardName = 'Omsk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Omsk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Omsk Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+06:00) Omsk',
			Bias = -360,
			DaylightType = 1,
			--StandardName = 'Omsk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Omsk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Omsk Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 136, '(GMT+06:00) Omsk', -360, 1, 'Omsk Standard Time', NULL, NULL, 0, 'Omsk Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT+07:00) Novosibirsk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Novosibirsk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Novosibirsk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Novosibirsk Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+07:00) Novosibirsk',
			Bias = -420,
			DaylightType = 1,
			--StandardName = 'Novosibirsk Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Novosibirsk Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Novosibirsk Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 137, '(GMT+07:00) Novosibirsk', -420, 1, 'Novosibirsk Standard Time', NULL, NULL, 0, 'Novosibirsk Daylight Time', NULL, NULL, -60
END

GO
PRINT N'Update complete.';


GO
