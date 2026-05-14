namespace Confirmit.CATI.Core.EmailReports
{
    public interface IScheduledReportEmail
    {
        ReportType ReportType
        {
            get;
        }

        string ReportDataExportFileName
        {
            get;
        }

        bool IsSwitchedOnAndConfiguredAndItsTimeToSend();

        bool IsLastDateSentRecent();

        string ReportRecipients { get; }

        void UpdateReportLastSentTime();

        IReportBuilder GetReportBuilder();
    }
}
