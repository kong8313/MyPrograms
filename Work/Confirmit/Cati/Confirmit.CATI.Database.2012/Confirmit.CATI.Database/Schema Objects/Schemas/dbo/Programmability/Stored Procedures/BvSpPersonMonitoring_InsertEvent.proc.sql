CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_InsertEvent] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT, 
 @MonitoringSessionID BIGINT,
 @TimeStamp DATETIME,
 @MessageType INT,
 @EventObject VARBINARY(MAX)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 INSERT INTO BvPersonMonitoringEvents ([PersonSID], [MonitoringSessionID], [TimeStamp], MessageType, EventObject) VALUES(@PersonSID, @MonitoringSessionID, @TimeStamp, @MessageType, @EventObject)
 
 RETURN scope_identity()
END