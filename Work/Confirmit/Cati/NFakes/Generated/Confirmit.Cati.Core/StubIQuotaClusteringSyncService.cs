using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIQuotaClusteringSyncService : IQuotaClusteringSyncService 
    {
        private IQuotaClusteringSyncService _inner;

        public StubIQuotaClusteringSyncService()
        {
            _inner = null;
        }

        public IQuotaClusteringSyncService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void InitializeCallsAndCountersBvSurveyEntityCancellationTokenDelegate(BvSurveyEntity survey, CancellationToken cancellationToken);
        public InitializeCallsAndCountersBvSurveyEntityCancellationTokenDelegate InitializeCallsAndCountersBvSurveyEntityCancellationToken;

        void IQuotaClusteringSyncService.InitializeCallsAndCounters(BvSurveyEntity survey, CancellationToken cancellationToken)
        {

            if (InitializeCallsAndCountersBvSurveyEntityCancellationToken != null)
            {
                InitializeCallsAndCountersBvSurveyEntityCancellationToken(survey, cancellationToken);
            } else if (_inner != null)
            {
                ((IQuotaClusteringSyncService)_inner).InitializeCallsAndCounters(survey, cancellationToken);
            }
        }

        public delegate void SyncCallsAndCountersBvSurveyEntityBatchParametersDelegate(BvSurveyEntity survey, BatchParameters batch);
        public SyncCallsAndCountersBvSurveyEntityBatchParametersDelegate SyncCallsAndCountersBvSurveyEntityBatchParameters;

        void IQuotaClusteringSyncService.SyncCallsAndCounters(BvSurveyEntity survey, BatchParameters batch)
        {

            if (SyncCallsAndCountersBvSurveyEntityBatchParameters != null)
            {
                SyncCallsAndCountersBvSurveyEntityBatchParameters(survey, batch);
            } else if (_inner != null)
            {
                ((IQuotaClusteringSyncService)_inner).SyncCallsAndCounters(survey, batch);
            }
        }

        public delegate void ResetCallsAndCountersBvSurveyEntityCancellationTokenDelegate(BvSurveyEntity survey, CancellationToken cancellationToken);
        public ResetCallsAndCountersBvSurveyEntityCancellationTokenDelegate ResetCallsAndCountersBvSurveyEntityCancellationToken;

        void IQuotaClusteringSyncService.ResetCallsAndCounters(BvSurveyEntity survey, CancellationToken cancellationToken)
        {

            if (ResetCallsAndCountersBvSurveyEntityCancellationToken != null)
            {
                ResetCallsAndCountersBvSurveyEntityCancellationToken(survey, cancellationToken);
            } else if (_inner != null)
            {
                ((IQuotaClusteringSyncService)_inner).ResetCallsAndCounters(survey, cancellationToken);
            }
        }

        public delegate ReinitializeQuotaClusteringStatus ReinitializeCallsAndCountersBvSurveyEntityActionOfStringCancellationTokenDelegate(BvSurveyEntity survey, Action<string> taskLog, CancellationToken cancellationToken);
        public ReinitializeCallsAndCountersBvSurveyEntityActionOfStringCancellationTokenDelegate ReinitializeCallsAndCountersBvSurveyEntityActionOfStringCancellationToken;

        ReinitializeQuotaClusteringStatus IQuotaClusteringSyncService.ReinitializeCallsAndCounters(BvSurveyEntity survey, Action<string> taskLog, CancellationToken cancellationToken)
        {


            if (ReinitializeCallsAndCountersBvSurveyEntityActionOfStringCancellationToken != null)
            {
                return ReinitializeCallsAndCountersBvSurveyEntityActionOfStringCancellationToken(survey, taskLog, cancellationToken);
            } else if (_inner != null)
            {
                return ((IQuotaClusteringSyncService)_inner).ReinitializeCallsAndCounters(survey, taskLog, cancellationToken);
            }

            return default(ReinitializeQuotaClusteringStatus);
        }

    }
}