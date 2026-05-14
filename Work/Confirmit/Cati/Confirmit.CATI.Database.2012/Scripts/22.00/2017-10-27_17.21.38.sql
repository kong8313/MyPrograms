PRINT N'Altering [dbo].[BvSpPersonMonitoring_Stop]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonMonitoring_Stop]
        @PersonSID INT,
  @MonitoringSessionID BIGINT
AS
DECLARE @Count INT

SELECT @Count = COUNT(*) FROM BvPersonMonitoring WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

IF @Count <> 0
BEGIN

 DELETE FROM BvPersonMonitoringEvents WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)
 
 DELETE FROM BvPersonMonitoring WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

 DELETE FROM BvPersonMonitoringLastID WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)

    RETURN (1)
END

RETURN (0)
GO
PRINT N'Update complete.';


GO
