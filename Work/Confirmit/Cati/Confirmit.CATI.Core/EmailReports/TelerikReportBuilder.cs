using System;
using System.IO;
using System.Linq;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using TelerikReport = Telerik.Reporting.Report;

namespace Confirmit.CATI.Core.EmailReports
{
    public abstract class TelerikReportBuilder : IReportBuilder
    {
        public abstract IReport BuildReport(DateTime reportStartTime, DateTime reportEndTime);

        public bool ShouldBeEncrypted { get { return false; } }

        protected abstract TelerikReport Report { get; }

        public string ExportReportToDisk(IReport report, string fileName)
        {
            foreach (var param in report.ReportParametersCollection)
            {
                if (Report.ReportParameters.Contains(param.Key))
                    Report.ReportParameters[param.Key].Value = param.Value;
            }

            Prepare();

            var reportSource = new InstanceReportSource { ReportDocument = Report };

            var reportProcessor = new ReportProcessor();
            var result = reportProcessor.RenderReport("PDF", reportSource, null);

            File.WriteAllBytes(fileName, result.DocumentBytes);

            return fileName;
        }

        public virtual void Prepare()
        {
        }
    }
}