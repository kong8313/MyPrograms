namespace Confirmit.CATI.Core.Batch
{
    public interface IBatchFactory
    {
        IDatabaseBatch CreateDatabaseBatch(BatchParameters parameters);
        
        IMemoryBatch CreateMemoryBatch(BatchParameters parameters);

        IDatabaseBatch CreateEmptyDatabaseBatch();
    }
}
