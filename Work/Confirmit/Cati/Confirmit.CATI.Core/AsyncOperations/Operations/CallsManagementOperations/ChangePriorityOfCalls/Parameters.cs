using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ChangePriorityOfCalls
{
    [Serializable]
    public class Parameters : IAsyncOperationParameters
    {
        public int SurveyId { get; set; }
        public int Priority { get; set; }
        public BatchParameters BatchParameters { get; set; }
    }
}
