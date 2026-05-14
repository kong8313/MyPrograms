using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.SynchronizeRespondents
{
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
    }
}
