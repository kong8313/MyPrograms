using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework
{
    public interface IAsyncOperationQueue
    {
        BvAsyncOperationQueueEntity Enqueue(
            int callCenterId,
            string title,
            bool isInitiatedBySystem,
            IAsyncOperationParameters parameters,
            int priority,
            string supervisorName);

        BvAsyncOperationQueueEntity Dequeue();

        void UpdateHanged();
        void Abort(int id, string supervisorName);
    }
}