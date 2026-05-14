namespace Confirmit.CATI.Core.Reports
{
    public class AlertsHistoryAggregatedReportItem
    {
        public int InterviewerId { get; set; }

        public string InterviewerName { get; set; }

        public int RedCount { get; set; }

        public int AmberCount { get; set; }

        public int TotalCount { get; set; }
    }
}