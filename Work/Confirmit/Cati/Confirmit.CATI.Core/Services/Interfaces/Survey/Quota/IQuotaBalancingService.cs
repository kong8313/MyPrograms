using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IQuotaBalancingService
    {
        QuotaBalancingConfiguration GetQuotaBalancingConfiguration(int surveyId);
        void SetQuotaBalancingConfiguration(int surveyId, QuotaBalancingConfiguration configuration);
        void ResetQuotaBalancingConfiguration(int surveyId);

        void AdjustQuotaBalancingConfiguration(int surveySid, IEnumerable<TableInfo> tables);
    }
}
