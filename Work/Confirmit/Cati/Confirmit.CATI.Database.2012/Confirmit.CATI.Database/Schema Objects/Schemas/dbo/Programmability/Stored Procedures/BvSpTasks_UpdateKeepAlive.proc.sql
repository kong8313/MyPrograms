CREATE PROCEDURE [dbo].[BvSpTasks_UpdateKeepAlive]
 @PersonSID int
AS

DECLARE @Now DATETIME
SET @Now = GETUTCdate()

UPDATE [dbo].[BvTasks]
    SET LastKeepAliveTime = @Now
WHERE PersonSID = @PersonSID

RETURN @@ROWCOUNT