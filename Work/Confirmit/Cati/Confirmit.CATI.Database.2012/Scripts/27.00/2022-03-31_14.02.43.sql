GO
PRINT N'Altering [dbo].[BvPersonMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring]
    ADD [IsWebMonitoring] BIT CONSTRAINT [DF_BvPersonMonitoring_IsWebMonitoring] DEFAULT (0) NOT NULL,
	[IsLiveMonitoringEnabled] BIT CONSTRAINT [DF_BvPersonMonitoring_IsLiveMonitoringEnabled] DEFAULT(0) NOT NULL;


GO
PRINT N'Altering [dbo].[BvSpPersonMonitoring_IsStart]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonMonitoring_IsStart]
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
GO
PRINT N'Altering [dbo].[BvSpPersonMonitoring_Start]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonMonitoring_Start]
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
GO
PRINT N'Refreshing [dbo].[BvSpGetListSurveyTasks]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpGetListSurveyTasks]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonMonitoring_Stop]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonMonitoring_Stop]';


GO
PRINT N'Update complete.';


GO
