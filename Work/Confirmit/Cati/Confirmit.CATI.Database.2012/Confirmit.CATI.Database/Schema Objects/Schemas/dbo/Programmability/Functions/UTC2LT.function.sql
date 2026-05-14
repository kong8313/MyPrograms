CREATE FUNCTION [dbo].[UTC2LT](
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