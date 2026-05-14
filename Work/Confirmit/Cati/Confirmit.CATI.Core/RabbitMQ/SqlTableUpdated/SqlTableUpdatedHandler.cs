using System.Threading;
using System.Threading.Tasks;
using Confirmit.MessageBroker.Consume.Sdk;

namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated
{
    public class SqlTableUpdatedHandler : IMessageHandler<SqlTableUpdatedMessage>
    {
        private readonly SqlTableUpdatedWorker _worker;

        public SqlTableUpdatedHandler(SqlTableUpdatedWorker worker)
        {
            _worker = worker;
        }

        public Task HandleMessage(Message<SqlTableUpdatedMessage> message, CancellationToken cancellationToken)
        {
            _worker.Execute(message.Content);

            return Task.CompletedTask;
        }
    }
}