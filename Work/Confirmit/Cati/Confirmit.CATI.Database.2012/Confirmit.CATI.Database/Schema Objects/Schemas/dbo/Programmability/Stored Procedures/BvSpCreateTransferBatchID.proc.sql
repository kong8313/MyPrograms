CREATE PROCEDURE [dbo].[BvSpCreateTransferBatchID]
@bibb INT
AS
DECLARE @BatchID INTEGER

    UPDATE BvTransferBatches 
    SET LastBatchID = LastBatchID + 1,
        @BatchID = LastBatchID + 1

RETURN (@BatchID)