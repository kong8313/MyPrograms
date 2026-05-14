using System;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class AsyncOperationProgressInfo
    {
        public int OperationId { get; set; }
        public int Status { get; set; }
        public string StatusDescription { get; set; }
        public int? PercentageComplete { get; set; }
        public string Text { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsStateRetrievalException { get; set; }
    }
}