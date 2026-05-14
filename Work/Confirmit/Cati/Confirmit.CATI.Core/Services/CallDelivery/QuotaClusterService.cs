using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.CallDelivery
{
    public class QuotaClusterService : IQuotaClusterService
    {
        private readonly IQuotaClusteringConfigurationService _quotaClusteringConfigurationService;

        public QuotaClusterService(IQuotaClusteringConfigurationService quotaClusteringConfigurationService)
        {
            _quotaClusteringConfigurationService = quotaClusteringConfigurationService;
        }

        public bool TryIncrenent(int surveyId, int callId)
        {
            if (!_quotaClusteringConfigurationService.IsEnabled(surveyId))
                return true;
            
            int isOk = 0;
            
            BvSpCluster_TryIncrenentAdapter.ExecuteNonQuery(surveyId, callId, false, out isOk);

            return isOk > 0;
        }

        public void Decrement(int surveyId, int cellId)
        {
            if (!_quotaClusteringConfigurationService.IsEnabled(surveyId))
                return;

            BvSpCluster_DecrementAdapter.ExecuteNonQuery(surveyId, cellId);
        }

        public bool Increnent(int surveyId, int callId)
        {
            if (!_quotaClusteringConfigurationService.IsEnabled(surveyId))
                return true;

            int isOk = 0;

            BvSpCluster_TryIncrenentAdapter.ExecuteNonQuery(surveyId, callId, true, out isOk);

            return isOk > 0;
        }
    }
}
