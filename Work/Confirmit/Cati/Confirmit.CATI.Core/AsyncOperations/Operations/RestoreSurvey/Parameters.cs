using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.RestoreSurvey
{
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public string Data { get; set; }
    }
}
