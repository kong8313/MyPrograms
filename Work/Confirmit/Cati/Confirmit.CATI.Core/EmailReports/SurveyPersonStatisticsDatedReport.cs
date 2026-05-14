using System;

namespace Confirmit.CATI.Core.EmailReports
{
    public class SurveyPersonStatisticsDatedReport : Report
    {
        public DateTime ReportDate
        {
            get { return (DateTime)ReportParameters["ReportDate"]; }
            set { ReportParameters["ReportDate"] = value; }
        }

        public string PersonNames
        {
            get { return (string)ReportParameters["PersonNames"]; }
            set { ReportParameters["PersonNames"] = value; }
        }

        public string SurveyNames
        {
            get { return (string)ReportParameters["SurveyNames"]; }
            set { ReportParameters["SurveyNames"] = value; }
        }

        public string SurveyDataFilter
        {
            get { return (string)ReportParameters["SurveyDataFilter"]; }
            set { ReportParameters["SurveyDataFilter"] = value; }
        }

        public string DbSurveyIds
        {
            get { return (string)ReportParameters["DbSurveyIds"]; }
            set { ReportParameters["DbSurveyIds"] = value; }
        }

        public string DbPersonIds
        {
            get { return (string)ReportParameters["DbPersonIds"]; }
            set { ReportParameters["DbPersonIds"] = value; }
        }

        public string DbStateIds
        {
            get { return (string)ReportParameters["DbStateIds"]; }
            set { ReportParameters["DbStateIds"] = value; }
        }

        public DateTime DbStartDate
        {
            get { return (DateTime)ReportParameters["DbStartDate"]; }
            set { ReportParameters["DbStartDate"] = value; }
        }

        public DateTime DbEndDate
        {
            get { return (DateTime)ReportParameters["DbEndDate"]; }
            set { ReportParameters["DbEndDate"] = value; }
        }

        public string DbSurveyDataFilter
        {
            get { return (string)ReportParameters["DbSurveyDataFilter"]; }
            set { ReportParameters["DbSurveyDataFilter"] = value; }
        }

    }
}
