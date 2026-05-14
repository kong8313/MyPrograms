using System;
using System.Threading;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.MessageBroker.Consume.Sdk;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class CatiBackendNotificationConsumptionRegistry : IDisposable
    {
        private IConnection _connection;
        private RabbitMQConsumption<CatiBackendNotification> _consumption;
        private readonly RabbitMQConnectionProvider _connectionProvider;
        private readonly IMessageHandler<CatiBackendNotification> _messageHandler;

        public CatiBackendNotificationConsumptionRegistry(
            RabbitMQConnectionProvider connectionProvider,
            IMessageHandler<CatiBackendNotification> messageHandler)
        {
            _connectionProvider = connectionProvider;
            _messageHandler = messageHandler;
        }

        public void Start()
        {
            _connection = _connectionProvider.GetConnection();
            _consumption = new RabbitMQConsumption<CatiBackendNotification>(
                null,
                _connection,
                new RabbitMqConsumptionLogger<CatiBackendNotification>(),
                GetOptions(),
                _messageHandler,
                null);

            _consumption.Start(CancellationToken.None);
            EventDetailsScope.Current.AddTiming("Start cati backend notifications RabbitMQ consumption");
        }

        public void Dispose()
        {
            try
            {
                _consumption?.Dispose();
                _consumption = null;
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e, "Error while disposing RabbitMQ consumption");
            }
        }

        private OptionsWrapper<RabbitMQConsumptionOptions<CatiBackendNotification>> GetOptions()
        {
            var processInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            var companyId = ServiceLocator.Resolve<ICompanyInfo>().CompanyId;
            var machineName = processInfo.MachineName;
            var processId = processInfo.ProcessId;

            return new OptionsWrapper<RabbitMQConsumptionOptions<CatiBackendNotification>>(
                new RabbitMQConsumptionOptions<CatiBackendNotification> {
                    QueueName = $"Confirmit.Cati.Backend.Notification.{companyId}.{machineName}.{processId}",
                    ExchangeName = "Confirmit.Cati.Backend.Notification",
                    RoutingKey = companyId.ToString(),
                    QueueType = QueueType.Quorum,
                    AutoDeleteQueue = true,
                    DeleteQueueOnClose = true,
                    AutoDeleteQueueAfter = TimeSpan.FromMinutes(5)
                });
        }
    }
}