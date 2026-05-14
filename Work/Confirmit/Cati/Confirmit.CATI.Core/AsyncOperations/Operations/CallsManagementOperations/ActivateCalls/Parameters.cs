using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.ActivateCalls
{
    [Serializable]
    public class Parameters : IAsyncBatchedOperationParameters
    {
        public int SurveyId { get; set; }
        public BatchParameters BatchParameters { get; set; }
        public int Priority { get; set; }
        public CallStates CallState { get; set; }
        public int[] ResourceIds { get; set; }
        public int ShiftTypeId { get; set; }
        public DateTime? TimeToCall { get; set; }
        public bool EnableDisabledCalls { get; set; }
        public int? ITS { get; set; }
    }
}
