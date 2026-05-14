using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.MessageBroker.Consume.Sdk;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class CatiBackendNotificationHandler : IMessageHandler<CatiBackendNotification>
    {
        private readonly Dictionary<string, ICatiBackendNotificationHandler> _handlersDictionary;

        public CatiBackendNotificationHandler(ICatiBackendNotificationHandler[] handlers)
        {
            _handlersDictionary = handlers.ToDictionary(x => x.NotificationTypeName);
        }

        public Task HandleMessage(Message<CatiBackendNotification> message, CancellationToken cancellationToken)
        {
            if (!_handlersDictionary.ContainsKey(message.Type))
            {
                Trace.TraceError($"ICatiBackendNotificationHandler for message type '{message.Type}' not found");
                return Task.CompletedTask;
            }

            _handlersDictionary[message.Type].HandleMessage(message.Content);

            return Task.CompletedTask;
        }
    }
}