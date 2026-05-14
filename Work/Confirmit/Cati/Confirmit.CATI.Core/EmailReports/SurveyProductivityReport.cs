namespace Confirmit.CATI.Core.EmailReports
{
    public class SurveyProductivityReport : SurveyPersonStatisticsDatedReport
    {
        public string ITSNames
        {
            get { return (string)ReportParameters["ITSNames"]; }
            set { ReportParameters["ITSNames"] = value; }
        }

        public bool IncludePercentage
        {
            get { return (bool)ReportParameters["IncludePercentage"]; }
            set { ReportParameters["IncludePercentage"] = value; }
        }
    }
}