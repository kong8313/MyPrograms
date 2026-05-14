
UPDATE BvTimezone SET  
			Name = '(GMT-04:00) Santiago',
			Bias = 240,
			DaylightType = 2,
			--StandardName = 'Pacific SA Standard Time', 
			StandardStart = '2000-04-01 23:59:00.000',
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
			StandardStart = '2000-04-01 23:59:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = '2000-09-01 23:59:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 60, '(GMT-04:00) Santiago', 240, 2, 'Pacific SA Standard Time', '2000-04-01 23:59:00.000', 6, 0, 'Pacific SA Daylight Time', '2000-09-01 23:59:00.000', 6, -60
END

GO
PRINT N'Update complete.'

GO

UPDATE BvTimezone SET  
			Name = '(GMT-06:00) Easter Island',
			Bias = 360,
			DaylightType = 2,
			--StandardName = 'Easter Island Standard Time', 
			StandardStart = '2000-04-01 22:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Easter Island Daylight Time',
			DaylightStart = '2000-09-01 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Easter Island Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-06:00) Easter Island',
			Bias = 360,
			DaylightType = 2,
			--StandardName = 'Easter Island Standard Time', 
			StandardStart = '2000-04-01 22:00:00.000',
			StandardDayOfWeek = 6, 
			StandardBias = 0,
			DaylightName = 'Easter Island Daylight Time',
			DaylightStart = '2000-09-01 22:00:00.000',
			DaylightDayOfWeek = 6,
			DaylightBias = -60
		WHERE StandardName = 'Easter Island Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 131, '(GMT-06:00) Easter Island', 360, 2, 'Easter Island Standard Time', '2000-04-01 22:00:00.000', 6, 0, 'Easter Island Daylight Time', '2000-09-01 22:00:00.000', 6, -60
END

GO
PRINT N'Update complete.'

GO
