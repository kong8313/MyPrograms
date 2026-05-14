using System;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Logger;
using Confirmit.Common;
using Confirmit.MessageBroker.Publish.Sdk;

namespace Confirmit.CATI.Core.RabbitMQ
{
    public class CatiMessageBrokerPublisher
    {
        private readonly IConfirmitMessageBrokerPublisher _publisher;
        private readonly ISideBySideManager _sideBySideManager;

        public CatiMessageBrokerPublisher(IConfirmitMessageBrokerPublisher publisher, ISideBySideManager sideBySideManager)
        {
            _publisher = publisher;
            _sideBySideManager = sideBySideManager;
        }

        public void Publish<T>(string exchange, T message, string topic, string typeName = null) where T : class
        {
            if (_sideBySideManager.SideBySideName == "Test")
                return;

            Action action = () =>
            {
                var stopWatch = Stopwatch.StartNew();

                _publisher.Publish(exchange, message, new MessageCorrelationId(CorrelationId.Current), topic: topic, typeName: typeName);

                if (stopWatch.Elapsed.TotalSeconds > 1)
                    TraceHelper.TraceVerbose($"RabbitMQ message publishing to {exchange} exchange took {stopWatch.Elapsed.TotalSeconds} seconds");
            };

            if (DatabaseTransactionScope.Current != null)
            {
                DatabaseTransactionScope.Current.ExecuteAfterTransactionCommit(action);
            }
            else
            {
                action.Invoke();
            }
        }
    }
}