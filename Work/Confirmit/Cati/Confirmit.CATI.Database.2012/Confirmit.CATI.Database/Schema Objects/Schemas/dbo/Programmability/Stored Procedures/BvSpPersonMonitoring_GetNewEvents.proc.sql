CREATE PROCEDURE [dbo].[BvSpPersonMonitoring_GetNewEvents] 
 -- Add the parameters for the stored procedure here
 @PersonSID INT = 0, 
 @MonitoringSessionID BIGINT = 0,
 @MaxEventID BIGINT = 0
AS
BEGIN

 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

 BEGIN TRANSACTION

 SELECT *
 FROM BvPersonMonitoringEvents
 WHERE (PersonSID = @PersonSID) AND (MonitoringSessionID = @MonitoringSessionID) AND (ID > @MaxEventID)
 ORDER BY [ID] ASC
 
 COMMIT TRANSACTION
END