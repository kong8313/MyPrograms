using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.EmailReports
{
    internal class CallHistoryScheduledReportEmail : ScheduledReportEmail
    {
        public CallHistoryScheduledReportEmail(ILocalTimeProvider localTimeProvider, IScheduledEmailReportsRepository scheduledEmailReportsRepository)
            : base(localTimeProvider, scheduledEmailReportsRepository)
        {
        }

        public override ReportType ReportType
        {
            get
            {
                return ReportType.CallHistory;
            }
        }

        protected override bool ReportEnabled
        {
            get { return ReportSystemSettings.CallHistoryReportEnabled; }
        }

        protected override int ReportHour
        {
            get { return ReportSystemSettings.CallHistoryReportHour; }
        }

        public override string ReportRecipients
        {
            get { return ReportSystemSettings.CallHistoryReportRecepients; }
        }

        public override string ReportDataExportFileName 
        { 
            get
            {
                return "DailyCallHistoryData.zip";
            }
        }

        public override IReportBuilder GetReportBuilder()
        {
            return new CallHistoryReportBuilder(LocalTimeProvider);
        }
    }
}
