CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_SetLastID] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT = 0,
 @MonitoringSessionID BIGINT = 0,
 @LastSentID BIGINT = 0
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 UPDATE BvPersonMonitoringLastID SET LastSentID = @LastSentID WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID)
END