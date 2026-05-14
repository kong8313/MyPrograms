namespace Confirmit.CATI.Core.EmailReports
{
    public class SurveyPersonStatisticsDatedDurationCollectionDataReport : SurveyPersonStatisticsDatedReport
    {
        public bool DbShowDialerAttempts
        {
            get { return (bool)ReportParameters["DbShowDialerAttempts"]; }
            set { ReportParameters["DbShowDialerAttempts"] = value; }
        }

        public bool DbHideEmpty
        {
            get { return (bool)ReportParameters["DbHideEmpty"]; }
            set { ReportParameters["DbHideEmpty"] = value; }
        }

        public bool IncludeOpenEndReviewTimeInInterviewDuration
        {
            get { return (bool)ReportParameters["IncludeOpenEndReviewTimeInInterviewDuration"]; }
            set { ReportParameters["IncludeOpenEndReviewTimeInInterviewDuration"] = value; }
        }
    }
}
