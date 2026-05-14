using System;
using System.Collections;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.EmailReports
{
    public class Report : IReport
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public IEnumerable ReportDataSource { get; set; }

        public string ExportFilePath { get; set; }

        public Report()
        {
            ReportParameters = new Dictionary<string, object>();
        }

        protected readonly Dictionary<string, object> ReportParameters;

        public ICollection<KeyValuePair<string, object>> ReportParametersCollection
        {
            get { return ReportParameters; }
        }

        public DateTime StartDate
        {
            get { return (DateTime)ReportParameters["StartDate"]; }
            set { ReportParameters["StartDate"] = value; }
        }

        public DateTime EndDate
        {
            get { return (DateTime)ReportParameters["EndDate"]; }
            set { ReportParameters["EndDate"] = value; }
        }
    }
}