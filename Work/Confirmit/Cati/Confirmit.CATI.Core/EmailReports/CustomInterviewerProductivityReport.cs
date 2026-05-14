namespace Confirmit.CATI.Core.EmailReports
{
    public class CustomInterviewerProductivityReport : SurveyPersonStatisticsDatedDurationCollectionDataReport
    {
        public bool IncludeBreaksInAverages
        {
            get { return (bool)ReportParameters["IncludeBreaksInAverages"]; }
            set { ReportParameters["IncludeBreaksInAverages"] = value; }
        }

        public bool DbCalcAllBreakHistory
        {
            get { return (bool)ReportParameters["DbCalcAllBreakHistory"]; }
            set { ReportParameters["DbCalcAllBreakHistory"] = value; }
        }
    }
}