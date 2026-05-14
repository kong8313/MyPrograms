using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EnableCalls
{
    public class Parameters : IAsyncBatchedOperationParameters
    {
        public int SurveyId{ get; set; }
        public BatchParameters BatchParameters{ get; set; }
        public bool EnablingState { get; set; }
        public bool IsFcdOperation { get; set; }
    }
}
