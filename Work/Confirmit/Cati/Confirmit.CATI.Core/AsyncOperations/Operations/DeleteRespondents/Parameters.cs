using Confirmit.CATI.Core.AsyncOperations.Framework;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.DeleteRespondents
{
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public string ProjectId { get; set; }
        public int[] RespondentIds { get; set; }
    }
}