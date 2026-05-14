using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated
{
    public class SqlTableUpdatedWorker
    {
        private readonly IEnumerable<IAsynchronousTrigger> _triggers;
        private readonly IAsyncManager _asyncManager;

        public SqlTableUpdatedWorker(
            IEnumerable<IAsynchronousTrigger> triggers,
            IAsyncManager asyncManager)
        {
            _triggers = triggers;
            _asyncManager = asyncManager;
        }

        public void Execute(SqlTableUpdatedMessage message)
        {
            var triggersForTable = _triggers.Where(x => x.TableName.Equals(message.TableName, StringComparison.OrdinalIgnoreCase)).ToArray();

            if (!triggersForTable.Any())
                Trace.TraceWarning($"No async trigger for message SqlTableUpdatedMessage with TableName {message.TableName}");

            foreach (var trigger in triggersForTable)
            {
                _asyncManager.QueueWorkItem(() =>
                {
                    trigger.OnTableChanged(new TriggerMessage() { TableName = message.TableName });
                    TraceHelper.TraceVerbose($"Cache reset for table {message.TableName}");
                });
            }
        }
    }
}