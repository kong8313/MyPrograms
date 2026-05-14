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
  
  SET @stdStart = DATEADD(minute, @bias, @stdStart)
  SET @dltStart = DATEADD(minute, @bias+@dltBias, @dltStart)

 
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

PRINT N'Refreshing [dbo].[BvSpGetActiveShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetActiveShifts]';


GO
PRINT N'Refreshing [dbo].[BvSpGetCallsSentToDialerDistribution]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetCallsSentToDialerDistribution]';


GO
PRINT N'Refreshing [dbo].[BvSpGetLiveShifts]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetLiveShifts]';


GO
PRINT N'Refreshing [dbo].[BvSpQueueUpSheduleTask3]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpQueueUpSheduleTask3]';


GO
PRINT N'Update complete.';


GO
