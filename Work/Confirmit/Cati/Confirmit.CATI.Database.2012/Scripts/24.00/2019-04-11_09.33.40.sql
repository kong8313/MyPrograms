GO
PRINT N'Altering [dbo].[BvPersonMonitoring]...';


GO
ALTER TABLE [dbo].[BvPersonMonitoring]
    ADD [TelephoneNumber] NVARCHAR (256) NULL;


GO
PRINT N'Altering [dbo].[BvSpPersonMonitoring_Start]...';


GO
ALTER PROCEDURE [dbo].[BvSpPersonMonitoring_Start]
        @PersonSID INT,
        @SupervisorName NVARCHAR(256),
  @MonitoringSessionID BIGINT,
  @TelephoneNumber NVARCHAR(256)
AS

INSERT INTO BvPersonMonitoring 
    SELECT @PersonSID, @SupervisorName, @MonitoringSessionID, @TelephoneNumber
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
PRINT N'Refreshing [dbo].[BvSpPersonMonitoring_IsStart]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonMonitoring_IsStart]';


GO
PRINT N'Refreshing [dbo].[BvSpPersonMonitoring_Stop]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[BvSpPersonMonitoring_Stop]';


GO
PRINT N'Update complete.';


GO
