using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;

namespace Confirmit.CATI.Core.AsyncOperations.Operations.CallsManagementOperations.EditCalls
{
    [Serializable]
    public class Parameters : IAsyncBatchedOperationParameters
    {
        public int SurveyId { get; set; }
        public BatchParameters BatchParameters { get; set; }
        public DateTime? TimeToCall { get; set; }
        public DateTime? TimeToExpire { get; set; }
        public int? CallState { get; set; }
        public int? CallPriority { get; set; }
        public int? ShiftType { get; set; }
        public int? ExtendedStatus { get; set; }
        public byte? DialingMode { get; set; }
    }
}
