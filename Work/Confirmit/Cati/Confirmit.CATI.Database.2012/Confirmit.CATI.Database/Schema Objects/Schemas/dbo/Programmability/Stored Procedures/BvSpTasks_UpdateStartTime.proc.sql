CREATE PROCEDURE [dbo].[BvSpTasks_UpdateStartTime]
	@personSid INT
AS
SET XACT_ABORT ON

	UPDATE [dbo].[BvTasks]
	SET StartTime = GETUTCDATE()
	WHERE PersonSID = @personSid AND
	      StartTime IS NULL
	
RETURN 0