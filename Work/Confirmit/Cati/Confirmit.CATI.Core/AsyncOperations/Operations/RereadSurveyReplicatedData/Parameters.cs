using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.RereadSurveyReplicatedData
{
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public string Reason { get; set; }
    }
}
