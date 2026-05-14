using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public class QuotaClusteringConfiguration
    {
        public string QuotaName { get; set; }
        public int LiveThreshod { get; set; }
    }

    public interface IQuotaClusteringConfigurationService
    {
        QuotaClusteringConfiguration GetConfiguration(int surveyId);
        void Configure(int surveyId, QuotaClusteringConfiguration configuration);
        void Reset(int surveyId);

        bool IsEnabled(int surveyId);
        bool IsEnabled(BvSurveyEntity survey);
    }
}
