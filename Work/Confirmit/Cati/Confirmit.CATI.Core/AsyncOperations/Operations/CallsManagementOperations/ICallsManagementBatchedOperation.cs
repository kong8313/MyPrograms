using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public interface ICallsManagementBatchedOperation : IAsyncOperation
    {
        void ProcessSubBatch(
            ICallsManagementBatchedOperationBase operation, 
            IDatabaseBatch subBatch, 
            object state,
            BvAsyncOperationQueueEntity entity);
    }
}
