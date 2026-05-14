CREATE PROCEDURE [dbo].[BvSpAsyncOperationQueue_UpdateProgress]
    @Id INT,
    @TotalItemsCount INT,
    @ProcessedItemsCount INT,
    @FailedItemsCount INT
AS
    UPDATE BvAsyncOperationQueue SET TotalItemsCount = @TotalItemsCount, ProcessedItemsCount = @ProcessedItemsCount, FailedItemsCount = @FailedItemsCount, HeartBeat = GETUTCDATE() WHERE Id = @Id
RETURN 0
GO