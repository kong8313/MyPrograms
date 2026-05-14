CREATE PROCEDURE [dbo].[BvSpTransferArray_Move]
	@srcBatchId int, 
	@dstBatchId int,
	@count int
AS
	UPDATE TOP(@count) BvTransferArrays
		SET BatchID = @dstBatchId
		WHERE BatchID = @srcBatchId
RETURN @@ROWCOUNT