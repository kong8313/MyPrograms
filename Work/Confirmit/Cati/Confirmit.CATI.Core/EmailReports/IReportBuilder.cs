using System;

namespace Confirmit.CATI.Core.EmailReports
{
    public interface IReportBuilder
    {
        IReport BuildReport(DateTime reportStartTime, DateTime reportEndTime);
        string ExportReportToDisk(IReport report, string fileName);
        bool ShouldBeEncrypted { get; }
        void Prepare();
    }
}
