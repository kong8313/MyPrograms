CREATE PROCEDURE [dbo].[BvSpDeleteTransfer]
@BatchID INTEGER
AS

    DELETE FROM BvTransferArrays WHERE BatchID = @BatchID

RETURN (0)