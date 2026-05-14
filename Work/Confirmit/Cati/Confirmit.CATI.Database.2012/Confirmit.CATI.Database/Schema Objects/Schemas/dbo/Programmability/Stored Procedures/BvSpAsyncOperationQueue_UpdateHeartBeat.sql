CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateHeartBeat]
	@Id INT
AS
    UPDATE BvAsyncOperationQueue SET HeartBeat = GETUTCDATE() WHERE Id = @Id
RETURN 0
