namespace Confirmit.CATI.Core.Batch.Tools
{
    public interface IDatabaseBatchItemTransfer
    {
        bool TransferTo(IDatabaseBatch sourceBatch, IDatabaseBatch destinationBatch, int countItems);
    }
}