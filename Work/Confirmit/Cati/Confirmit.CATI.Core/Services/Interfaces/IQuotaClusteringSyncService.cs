using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public enum ReinitializeQuotaClusteringStatus
    {
        NotChanged,
        Changed,
        Disabled
    }

    public interface IQuotaClusteringSyncService
    {
        void InitializeCallsAndCounters(BvSurveyEntity survey, CancellationToken cancellationToken);
        void SyncCallsAndCounters(BvSurveyEntity survey, BatchParameters batch);
        void ResetCallsAndCounters(BvSurveyEntity survey, CancellationToken cancellationToken);
        ReinitializeQuotaClusteringStatus ReinitializeCallsAndCounters(BvSurveyEntity survey, Action<string> taskLog, CancellationToken cancellationToken);
    }
}
