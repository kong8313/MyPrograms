using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.DeleteCalls
{
    public class Parameters : IAsyncBatchedOperationParameters
    {
        public int SurveyId{ get; set; }
        public int? NewITS { get; set; }
        public BatchParameters BatchParameters{ get; set; }
    }
}
