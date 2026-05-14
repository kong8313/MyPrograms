CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_Start]
        @PersonSID INT,
        @SupervisorName NVARCHAR(256),
  @MonitoringSessionID BIGINT,
  @TelephoneNumber NVARCHAR(256),
  @IsWebMonitoring BIT = 0,
  @IsLiveMonitoringEnabled BIT = 0
AS

INSERT INTO BvPersonMonitoring 
    SELECT @PersonSID, @SupervisorName, @MonitoringSessionID, @TelephoneNumber, @IsWebMonitoring, @IsLiveMonitoringEnabled
        WHERE NOT EXISTS( 
            SELECT 1 
            FROM BvPersonMonitoring 
            WHERE PersonSID = @PersonSID )

IF @@ROWCOUNT <> 0
BEGIN
 DELETE FROM BvPersonMonitoringLastID WHERE (PersonSID = @PersonSID)

 INSERT INTO BvPersonMonitoringLastID
  SELECT @PersonSID, @MonitoringSessionID, 0
   WHERE EXISTS(
    SELECT 1 FROM BvPersonMonitoring WHERE PersonSID = @PersonSID)

    SELECT 1 as result, '' as supervisorNameAlreadyMonitoring, 0 as monitoringSessionID
    RETURN (1)
END
ELSE
BEGIN
    SELECT 0 as result, supervisorName as supervisorNameAlreadyMonitoring, monitoringSessionID FROM BvPersonMonitoring WHERE PersonSID = @PersonSID
    RETURN (0)
END

RETURN (0)