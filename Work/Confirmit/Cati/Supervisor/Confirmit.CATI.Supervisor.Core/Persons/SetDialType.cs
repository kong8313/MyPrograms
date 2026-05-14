using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Supervisor.Core.Persons
{
    public class SetDialType : ISetDialType
    {
        private readonly IToggleSettings _toggleSettings;
        private readonly ISqlTableUpdatedPublisher _updatedPublisher;

        public SetDialType(IToggleSettings toggleSettings, ISqlTableUpdatedPublisher updatedPublisher)
        {
            _toggleSettings = toggleSettings;
            _updatedPublisher = updatedPublisher;
        }

        public void Set(DialType dialType, IEnumerable<int> personIds)
        {
            if (!_toggleSettings.ShowDialType)
            {
                return;
            }

            var sids = personIds as int[] ?? personIds.ToArray();
            var qualifier = string.Join(",", sids);

            var evt = new SetInterviewerDialTypeEvent(sids, dialType);

            BvSpPerson_UpdateBatchedAdapter.ExecuteNonQuery(qualifier, (byte)dialType);
            PersonRepository.RefreshCache();
            evt.Finish();
        }
    }
}