using System.Threading;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public interface ICallsManagementOperationBase
    {
        AsyncOperationResult Execute(ICallsManagementOperation operation,
            BatchParameters batchParameters,
            IAsyncOperationProgressLogger progressLogger,
            BvAsyncOperationQueueEntity entity,
            int surveySid,
            int portionSize,
            int maxItems,
            string itemsName,
            object state, CancellationToken cancellationToken);
    }
}