using System;
using System.Threading;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.MessageBroker.Consume.Sdk;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated
{
    public class SqlTableUpdatedConsumptionRegistry : IDisposable
    {
        private IConnection _connection;
        private RabbitMQConsumption<SqlTableUpdatedMessage> _consumption;
        private readonly RabbitMQConnectionProvider _connectionProvider;
        private readonly IMessageHandler<SqlTableUpdatedMessage> _messageHandler;

        public SqlTableUpdatedConsumptionRegistry(
            RabbitMQConnectionProvider connectionProvider,
            IMessageHandler<SqlTableUpdatedMessage> messageHandler)
        {
            _connectionProvider = connectionProvider;
            _messageHandler = messageHandler;
        }

        public void Start()
        {
            _connection = _connectionProvider.GetConnection();
            _consumption = new RabbitMQConsumption<SqlTableUpdatedMessage>(
                null,
                _connection,
                new RabbitMqConsumptionLogger<SqlTableUpdatedMessage>(),
                GetOptions(),
                _messageHandler,
                null);

            _consumption.Start(CancellationToken.None);
            EventDetailsScope.Current.AddTiming("Start sql table updated RabbitMQ consumption");
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

        private OptionsWrapper<RabbitMQConsumptionOptions<SqlTableUpdatedMessage>> GetOptions()
        {
            var processInfo = ServiceLocator.Resolve<IProcessAndEnvironmentInfo>();
            var companyId = ServiceLocator.Resolve<ICompanyInfo>().CompanyId;
            var machineName = processInfo.MachineName;
            var processId = processInfo.ProcessId;

            return new OptionsWrapper<RabbitMQConsumptionOptions<SqlTableUpdatedMessage>>(
                new RabbitMQConsumptionOptions<SqlTableUpdatedMessage> {
                    QueueName = $"Confirmit.Cati.Backend.SqlTableUpdated.{companyId}.{machineName}.{processId}",
                    ExchangeName = "Confirmit.Cati.Backend.SqlTableUpdated",
                    RoutingKey = companyId.ToString(),
                    QueueType = QueueType.Quorum,
                    AutoDeleteQueue = true,
                    DeleteQueueOnClose = true,
                    AutoDeleteQueueAfter = TimeSpan.FromMinutes(5)
                });
        }
    }
}