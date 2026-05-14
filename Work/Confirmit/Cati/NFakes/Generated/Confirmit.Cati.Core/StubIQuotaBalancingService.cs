using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using System.Collections.Generic;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIQuotaBalancingService : IQuotaBalancingService 
    {
        private IQuotaBalancingService _inner;

        public StubIQuotaBalancingService()
        {
            _inner = null;
        }

        public IQuotaBalancingService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate QuotaBalancingConfiguration GetQuotaBalancingConfigurationInt32Delegate(int surveyId);
        public GetQuotaBalancingConfigurationInt32Delegate GetQuotaBalancingConfigurationInt32;

        QuotaBalancingConfiguration IQuotaBalancingService.GetQuotaBalancingConfiguration(int surveyId)
        {


            if (GetQuotaBalancingConfigurationInt32 != null)
            {
                return GetQuotaBalancingConfigurationInt32(surveyId);
            } else if (_inner != null)
            {
                return ((IQuotaBalancingService)_inner).GetQuotaBalancingConfiguration(surveyId);
            }

            return default(QuotaBalancingConfiguration);
        }

        public delegate void SetQuotaBalancingConfigurationInt32QuotaBalancingConfigurationDelegate(int surveyId, QuotaBalancingConfiguration configuration);
        public SetQuotaBalancingConfigurationInt32QuotaBalancingConfigurationDelegate SetQuotaBalancingConfigurationInt32QuotaBalancingConfiguration;

        void IQuotaBalancingService.SetQuotaBalancingConfiguration(int surveyId, QuotaBalancingConfiguration configuration)
        {

            if (SetQuotaBalancingConfigurationInt32QuotaBalancingConfiguration != null)
            {
                SetQuotaBalancingConfigurationInt32QuotaBalancingConfiguration(surveyId, configuration);
            } else if (_inner != null)
            {
                ((IQuotaBalancingService)_inner).SetQuotaBalancingConfiguration(surveyId, configuration);
            }
        }

        public delegate void ResetQuotaBalancingConfigurationInt32Delegate(int surveyId);
        public ResetQuotaBalancingConfigurationInt32Delegate ResetQuotaBalancingConfigurationInt32;

        void IQuotaBalancingService.ResetQuotaBalancingConfiguration(int surveyId)
        {

            if (ResetQuotaBalancingConfigurationInt32 != null)
            {
                ResetQuotaBalancingConfigurationInt32(surveyId);
            } else if (_inner != null)
            {
                ((IQuotaBalancingService)_inner).ResetQuotaBalancingConfiguration(surveyId);
            }
        }

        public delegate void AdjustQuotaBalancingConfigurationInt32IEnumerableOfTableInfoDelegate(int surveySid, IEnumerable<TableInfo> tables);
        public AdjustQuotaBalancingConfigurationInt32IEnumerableOfTableInfoDelegate AdjustQuotaBalancingConfigurationInt32IEnumerableOfTableInfo;

        void IQuotaBalancingService.AdjustQuotaBalancingConfiguration(int surveySid, IEnumerable<TableInfo> tables)
        {

            if (AdjustQuotaBalancingConfigurationInt32IEnumerableOfTableInfo != null)
            {
                AdjustQuotaBalancingConfigurationInt32IEnumerableOfTableInfo(surveySid, tables);
            } else if (_inner != null)
            {
                ((IQuotaBalancingService)_inner).AdjustQuotaBalancingConfiguration(surveySid, tables);
            }
        }

    }
}