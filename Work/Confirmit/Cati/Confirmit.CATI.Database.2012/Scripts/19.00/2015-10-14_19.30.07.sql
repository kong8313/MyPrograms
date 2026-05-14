GO
PRINT N'Altering [dbo].[UTC2LT]...';


GO
ALTER FUNCTION [dbo].[UTC2LT](
  @utc  SMALLDATETIME,
                @bias  INT,
                @type  INT,
                @stdDOW  INT, 
                @stdStart SMALLDATETIME,
  @stdBias INT,
                @dltDOW  INT, 
                @dltStart SMALLDATETIME,
  @dltBias INT
)
RETURNS SMALLDATETIME
AS
-- DATEFIRST must be set to 7 before calling the function
BEGIN

 IF @type = 2
 BEGIN
  SET @stdStart = dbo.GetCurrentBiasDate( @utc, @stdStart, @stdDOW )
  SET @dltStart = dbo.GetCurrentBiasDate( @utc, @dltStart, @dltDOW )
  
  SET @stdStart = DATEADD(minute, @bias+@dltBias, @stdStart)
  SET @dltStart = DATEADD(minute, @bias, @dltStart)
 
  IF  @stdStart >  @dltStart
  BEGIN
   IF @dltStart <= @utc AND @utc < @stdStart
                  SET @bias = @bias + @dltBias
   ELSE 
                  SET @bias = @bias + @stdBias
  END
  ELSE
  BEGIN
   IF @stdStart <= @utc and @utc < @dltStart
                  SET @bias = @bias + @stdBias
   ELSE 
                  SET @bias = @bias + @dltBias
  END
 END
 RETURN( DATEADD( minute, -@bias, @utc ) )
END
GO


PRINT 'Update time zones'
GO

UPDATE BvTimezone SET  
			Name = '(GMT+10:00) Vladivostok',
			Bias = -600,
			DaylightType = 1,
			--StandardName = 'Vladivostok Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Vladivostok Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Vladivostok Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT+10:00) Vladivostok',
			Bias = -600,
			DaylightType = 1,
			--StandardName = 'Vladivostok Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Vladivostok Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Vladivostok Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    exec BvSpTimezoneMaster_Insert 46, '(GMT+10:00) Vladivostok', -600, 1, 'Vladivostok Standard Time', NULL, NULL, 0, 'Vladivostok Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Santiago',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Pacific SA Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Santiago',
			Bias = 180,
			DaylightType = 1,
			--StandardName = 'Pacific SA Standard Time', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Pacific SA Daylight Time',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Pacific SA Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 60, '(GMT-03:00) Santiago', 180, 1, 'Pacific SA Standard Time', NULL, NULL, 0, 'Pacific SA Daylight Time', NULL, NULL, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT-03:00) Montevideo',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Montevideo Standard Time', 
			StandardStart = '2000-03-02 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Montevideo Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE StandardName = 'Montevideo Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-03:00) Montevideo',
			Bias = 180,
			DaylightType = 2,
			--StandardName = 'Montevideo Standard Time', 
			StandardStart = '2000-03-02 02:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Montevideo Daylight Time',
			DaylightStart = '2000-01-01 00:00:00.000',
			DaylightDayOfWeek = 4,
			DaylightBias = -60
		WHERE StandardName = 'Montevideo Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 89, '(GMT-03:00) Montevideo', 180, 2, 'Montevideo Standard Time', '2000-03-02 02:00:00.000', 0, 0, 'Montevideo Daylight Time', '2000-01-01 00:00:00.000', 4, -60
END

UPDATE BvTimezone SET  
			Name = '(GMT) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-07-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT) Casablanca',
			Bias = 0,
			DaylightType = 2,
			--StandardName = 'Morocco Standard Time', 
			StandardStart = '2000-10-05 03:00:00.000',
			StandardDayOfWeek = 0, 
			StandardBias = 0,
			DaylightName = 'Morocco Daylight Time',
			DaylightStart = '2000-07-03 02:00:00.000',
			DaylightDayOfWeek = 0,
			DaylightBias = -60
		WHERE StandardName = 'Morocco Standard Time'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 90, '(GMT) Casablanca', 0, 2, 'Morocco Standard Time', '2000-10-05 03:00:00.000', 0, 0, 'Morocco Daylight Time', '2000-07-03 02:00:00.000', 0, -60
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
			Name = '(GMT-05:00) Chetumal',
			Bias = 300,
			DaylightType = 1,
			--StandardName = 'Eastern Standard Time (Mexico)', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Eastern Daylight Time (Mexico)',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Eastern Standard Time (Mexico)'

UPDATE BvTimezoneMaster SET  
			Name = '(GMT-05:00) Chetumal',
			Bias = 300,
			DaylightType = 1,
			--StandardName = 'Eastern Standard Time (Mexico)', 
			StandardStart = NULL,
			StandardDayOfWeek = NULL, 
			StandardBias = 0,
			DaylightName = 'Eastern Daylight Time (Mexico)',
			DaylightStart = NULL,
			DaylightDayOfWeek = NULL,
			DaylightBias = -60
		WHERE StandardName = 'Eastern Standard Time (Mexico)'
IF @@ROWCOUNT = 0
BEGIN
    EXEC BvSpTimezoneMaster_Insert 112, '(GMT-05:00) Chetumal', 300, 1, 'Eastern Standard Time (Mexico)', NULL, NULL, 0, 'Eastern Daylight Time (Mexico)', NULL, NULL, -60
END

GO
PRINT N'Update complete.';


GO
