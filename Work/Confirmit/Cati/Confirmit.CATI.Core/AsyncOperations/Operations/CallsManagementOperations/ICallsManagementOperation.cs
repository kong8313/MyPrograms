using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations
{
    public interface ICallsManagementOperation : IAsyncOperation
    {
        void ProcessItem(
            ICallsManagementOperationBase operation,
            int interviewId,
            object state,
            BvAsyncOperationQueueEntity entity);
    }
}
