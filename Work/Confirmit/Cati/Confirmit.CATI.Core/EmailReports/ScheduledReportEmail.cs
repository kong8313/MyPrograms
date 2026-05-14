using System;

using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.EmailReports
{
    public abstract class ScheduledReportEmail : IScheduledReportEmail
    {
        internal const int RecentSentTimeout = 6; // hours

        protected IReportsSettings ReportSystemSettings;

        protected ILocalTimeProvider LocalTimeProvider;

        private readonly IScheduledEmailReportsRepository _scheduledEmailReportsRepository;

        protected ScheduledReportEmail(ILocalTimeProvider localTimeProvider, IScheduledEmailReportsRepository scheduledEmailReportsRepository)
        {
            ReportSystemSettings = ServiceLocator.Resolve<ISystemSettings>().Reports;
            LocalTimeProvider = localTimeProvider;
            _scheduledEmailReportsRepository = scheduledEmailReportsRepository;
        }

        public abstract ReportType ReportType { get; }

        public abstract string ReportRecipients { get; }

        public abstract IReportBuilder GetReportBuilder();

        public abstract string ReportDataExportFileName { get; }

        public bool IsSwitchedOnAndConfiguredAndItsTimeToSend()
        {
            return ReportEnabled && !string.IsNullOrEmpty(ReportRecipients) && ItsTimeToSendReport(ReportHour);
        }

        protected abstract bool ReportEnabled { get; }

        protected abstract int ReportHour { get; }

        private bool ItsTimeToSendReport(int reportHour)
        {
            return DateTime.UtcNow.Hour == reportHour;
        }

        public bool IsLastDateSentRecent()
        {
            var reportEntity = _scheduledEmailReportsRepository.GetCreateByReportType(ReportType);

            return (reportEntity.LastSent != null &&
                reportEntity.LastSent.Value.AddHours(RecentSentTimeout).CompareTo(DateTime.UtcNow) > 0); //"lastSent + RecentSentTimeout hours" is later than "now". 
        }

        public void UpdateReportLastSentTime()
        {
            var reportEntity = _scheduledEmailReportsRepository.GetCreateByReportType(ReportType);
            reportEntity.LastSent = DateTime.UtcNow;
            _scheduledEmailReportsRepository.Update(reportEntity);
        }
    }
}
