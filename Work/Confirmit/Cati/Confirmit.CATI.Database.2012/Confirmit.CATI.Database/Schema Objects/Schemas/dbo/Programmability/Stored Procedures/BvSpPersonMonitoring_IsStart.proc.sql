CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_IsStart]
		@PersonSID INT
AS
DECLARE @supervisorNameAlreadyMonitoring NVARCHAR( 256 )
DECLARE @monitoringSessionID BIGINT
DECLARE @isWebMonitoring BIT
DECLARE @isLiveMonitoringEnabled BIT
SELECT @supervisorNameAlreadyMonitoring = supervisorName, @monitoringSessionID = MonitoringSessionID, @isWebMonitoring = IsWebMonitoring, @isLiveMonitoringEnabled = IsLiveMonitoringEnabled  FROM BvPersonMonitoring WHERE PersonSID = @PersonSID

IF @supervisorNameAlreadyMonitoring IS NULL
BEGIN
	/*
		we need it to get correct type for DAL generated entities
	*/
	SET @monitoringSessionID = 0

    SELECT 0 as result, '' as supervisorNameAlreadyMonitoring, @monitoringSessionID as monitoringSessionID, CAST(0 AS BIT) as isWebMonitoring, CAST(0 AS BIT) as isLiveMonitoringEnabled
    RETURN (0)
END
ELSE
BEGIN
    SELECT 1 as result, @supervisorNameAlreadyMonitoring as supervisorNameAlreadyMonitoring, @monitoringSessionID as monitoringSessionID, @isWebMonitoring as isWebMonitoring, @isLiveMonitoringEnabled as isLiveMonitoringEnabled
    RETURN (1)
END

RETURN (0)