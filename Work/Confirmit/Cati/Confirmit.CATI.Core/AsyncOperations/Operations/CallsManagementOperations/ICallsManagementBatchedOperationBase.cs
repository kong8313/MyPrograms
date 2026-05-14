using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public interface ICallsManagementBatchedOperationBase
    {
        AsyncOperationResult Execute(ICallsManagementBatchedOperation operation,
            BatchParameters batchParameters,
            IAsyncOperationProgressLogger progressLogger,
            BvAsyncOperationQueueEntity entity,
            int surveySid,
            int portionSize,
            object state, CancellationToken cancellationToken);

        void WriteContextInfo(BvAsyncOperationQueueEntity entity, OperationType type, int its = 0, DialingMode dialMode = 0);
    }
}