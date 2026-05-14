using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.AssignCalls
{
    [Serializable]
    public class Parameters : IAsyncBatchedOperationParameters
    {
        public int SurveyId { get; set; }
        public int[] ResourceIds { get; set; }
        public BatchParameters BatchParameters { get; set; }
    }
}
