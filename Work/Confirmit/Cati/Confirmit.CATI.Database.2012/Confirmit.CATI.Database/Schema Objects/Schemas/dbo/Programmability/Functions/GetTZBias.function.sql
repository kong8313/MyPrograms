CREATE FUNCTION [dbo].[GetTZBias]
(
	@Date datetime,
	@TZID INT
)
RETURNS INT
AS
BEGIN 
    DECLARE @RESULT INT

	DECLARE @DaylightDOW INT, 
			@StandardDOW INT, 
			@StandardStart datetime, 
			@DaylightStart datetime,
			@StandardBias INT, 
			@DaylightBias INT, 
			@Type INT,
			@OriginalBias int

	SELECT
		@Type			= DaylightType,
		@DaylightDOW	= DaylightDayOfWeek,
		@StandardDOW	= StandardDayOfWeek,
		@StandardStart	= StandardStart,
		@DaylightStart	= DaylightStart,
		@StandardBias	= StandardBias,
		@DaylightBias	= DaylightBias,
		@OriginalBias	= Bias
	FROM BvTimezone 
	WHERE ID = @TZID

	IF @Type = 1
	BEGIN
	   RETURN @OriginalBias 
	END

	-- Compute Start Date for Daylight

	DECLARE @CurrentDaylightStart datetime

	SET @CurrentDaylightStart = 
					dbo.GetCurrentBiasDate (@date,@DaylightStart, @DaylightDOW)

	-- Compute Start Date for Standard

	DECLARE @CurrentStandardStart datetime

	SET @CurrentStandardStart = 
					dbo.GetCurrentBiasDate (@date, @StandardStart, @StandardDOW)


	-- get Bias

	IF  @CurrentStandardStart >  @CurrentDaylightStart
	BEGIN
		IF @CurrentDaylightStart <= @Date AND @Date < @CurrentStandardStart 
			SET @RESULT = @OriginalBias + @DaylightBias
		ELSE 
			SET @RESULT = @OriginalBias + @StandardBias
	END
	ELSE 
	BEGIN
		IF @CurrentStandardStart <= @Date and @Date < @CurrentDaylightStart
			SET @RESULT = @OriginalBias + @StandardBias
		ELSE
			SET @RESULT = @OriginalBias + @DaylightBias
	END

	RETURN @RESULT
END