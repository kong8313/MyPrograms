using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class AsyncOperationCancelledHandler : ICatiBackendNotificationHandler
    {
        private readonly AsyncOperationCancelledWorker _worker;

        public AsyncOperationCancelledHandler(AsyncOperationCancelledWorker worker)
        {
            _worker = worker;
        }

        public string NotificationTypeName => nameof(AsyncOperationCancelledNotification);

        public Task HandleMessage(CatiBackendNotification message)
        {
            var notification = JsonConvert.DeserializeObject<AsyncOperationCancelledNotification>(message.JsonContent);
            _worker.Execute(notification);

            return Task.CompletedTask;
        }
    }
}