CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_GetLastID] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT = 0,
 @MonitoringSessionID BIGINT
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 SELECT LastSentID FROM BvPersonMonitoringLastID WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)
END