using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.Configuration;
using RabbitMQ.Client;

namespace Confirmit.CATI.Core.RabbitMQ
{
    public class RabbitMQConnectionProvider : IDisposable
    {
        private readonly object _lockObject = new object();
        private volatile IConnection _connection;
        private readonly string _clientName;

        public RabbitMQConnectionProvider(ICompanyInfo companyInfo)
        {
            _clientName = $"Confirmit.Cati.Backend.{companyInfo.CompanyId}";
        }

        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                lock (_lockObject)
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        CloseConnection();
                        _connection = AsyncTaskRunner.RunSync(() => CreateConnection());
                    }
                }
            }

            return _connection;
        }

        public void Dispose()
        {
            try
            {
                CloseConnection();
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e, "Error while closing RabbitMQ connection");
            }
        }

        private void CloseConnection()
        {
            if (_connection != null)
            {
                AsyncTaskRunner.RunSync(() => _connection.CloseAsync());
                _connection = null;
            }
        }

        private async Task<IConnection> CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                UserName = ConfirmitConfiguration.MessageBrokerUserName,
                Password = ConfirmitConfiguration.MessageBrokerPassword,
                Port = ConfirmitConfiguration.MessageBrokerNodes.First<MessageBrokerNode>().Port,
                AutomaticRecoveryEnabled = true
            };
            return await factory.CreateConnectionAsync(ConfirmitConfiguration.MessageBrokerNodes.Select(n => n.Host).ToList(), _clientName);
        }
    }
}