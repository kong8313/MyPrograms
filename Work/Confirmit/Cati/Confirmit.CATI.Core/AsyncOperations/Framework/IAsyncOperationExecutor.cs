using System.Collections.Generic;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationExecutor
    {
        bool DequeueAndExecute();

        Task<AsyncOperationResult> ExecuteOperationAsync(BvAsyncOperationQueueEntity entity);

        AsyncOperationResult ExecuteOperationSync(BvAsyncOperationQueueEntity entity);

        void WaitForAllRunningOperationsToComplete();

        IEnumerable<int> GetExecutingOperationIds();
    }
}