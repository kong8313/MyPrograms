using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationAwaiter
    {
        BvAsyncOperationQueueEntity Await(int operationId);

        BvAsyncOperationQueueEntity Await(BvAsyncOperationQueueEntity operation);
    }
}
