using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;

namespace Confirmit.CATI.Core.Services
{
    public interface IQuotaBalancingConfigurationValidator
    {
        void CheckConfiguration(int surveyId, QuotaBalancingConfiguration configuration);
    }
}