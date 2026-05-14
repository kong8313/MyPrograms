using System.Diagnostics;
using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class AsyncOperationCancelledWorker
    {
        private readonly AsyncOperationCancellationService _cancellationService;

        public AsyncOperationCancelledWorker(AsyncOperationCancellationService cancellationService)
        {
            _cancellationService = cancellationService;
        }

        public void Execute(AsyncOperationCancelledNotification notification)
        {
            _cancellationService.CancelOperation(notification.OperationEntityId);
            Trace.TraceInformation($"Cancellation of async operation with Id = {notification.OperationEntityId}");
        }
    }
}