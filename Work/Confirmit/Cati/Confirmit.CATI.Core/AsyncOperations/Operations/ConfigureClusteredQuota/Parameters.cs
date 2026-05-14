using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.ConfigureClusteredQuota
{

    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public string QuotaName { get; set; }
        public int LiveThreshold { get; set; }
    }

    
}
