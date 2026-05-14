CREATE PROCEDURE [dbo].[BvSpTimezoneMaster_Get]
        @ID int
AS

IF @ID = 0 BEGIN
    SELECT  
  ID,
        Name,
        Bias,
        DaylightType,
        StandardName,
        StandardStart,
        StandardDayOfWeek,
        StandardBias,
        DaylightName,
        DaylightStart,
        DaylightDayOfWeek,
        DaylightBias
    FROM 
  BvTimezoneMaster 
 WHERE 
  ID NOT IN(
   SELECT ID
   FROM BvTimezone
   )
END
ELSE BEGIN
    SELECT
  ID,
        Name,
        Bias,
        DaylightType,
        StandardName,
        StandardStart,
        StandardDayOfWeek,
        StandardBias,
        DaylightName,
        DaylightStart,
        DaylightDayOfWeek,
        DaylightBias
    FROM
  BvTimezoneMaster 
    WHERE
  ID = @ID
END