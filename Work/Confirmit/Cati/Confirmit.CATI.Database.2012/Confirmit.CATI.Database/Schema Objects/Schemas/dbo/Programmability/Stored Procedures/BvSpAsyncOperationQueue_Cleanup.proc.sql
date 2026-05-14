CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_Cleanup]
	@State INT,
	@ExpirationDate DATETIME
AS
	DELETE FROM BvAsyncOperationQueue 
		WHERE State = @State AND COALESCE(FinishedDate, HeartBeat, StartedDate, QueuedDate) < @ExpirationDate
RETURN 0
