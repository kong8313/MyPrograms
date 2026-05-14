using System;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIQuotaBalancingConfigurationValidator : IQuotaBalancingConfigurationValidator 
    {
        private IQuotaBalancingConfigurationValidator _inner;

        public StubIQuotaBalancingConfigurationValidator()
        {
            _inner = null;
        }

        public IQuotaBalancingConfigurationValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckConfigurationInt32QuotaBalancingConfigurationDelegate(int surveyId, QuotaBalancingConfiguration configuration);
        public CheckConfigurationInt32QuotaBalancingConfigurationDelegate CheckConfigurationInt32QuotaBalancingConfiguration;

        void IQuotaBalancingConfigurationValidator.CheckConfiguration(int surveyId, QuotaBalancingConfiguration configuration)
        {

            if (CheckConfigurationInt32QuotaBalancingConfiguration != null)
            {
                CheckConfigurationInt32QuotaBalancingConfiguration(surveyId, configuration);
            } else if (_inner != null)
            {
                ((IQuotaBalancingConfigurationValidator)_inner).CheckConfiguration(surveyId, configuration);
            }
        }

    }
}