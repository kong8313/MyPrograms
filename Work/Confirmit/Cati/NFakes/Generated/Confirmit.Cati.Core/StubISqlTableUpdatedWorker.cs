using System;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes
{
    public class StubISqlTableUpdatedWorker : ISqlTableUpdatedWorker 
    {
        private ISqlTableUpdatedWorker _inner;

        public StubISqlTableUpdatedWorker()
        {
            _inner = null;
        }

        public ISqlTableUpdatedWorker Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task ExecuteSqlTableUpdatedMessageDelegate(SqlTableUpdatedMessage message);
        public ExecuteSqlTableUpdatedMessageDelegate ExecuteSqlTableUpdatedMessage;

        Task ISqlTableUpdatedWorker.Execute(SqlTableUpdatedMessage message)
        {


            if (ExecuteSqlTableUpdatedMessage != null)
            {
                return ExecuteSqlTableUpdatedMessage(message);
            } else if (_inner != null)
            {
                return ((ISqlTableUpdatedWorker)_inner).Execute(message);
            }

            return default(Task);
        }

    }
}