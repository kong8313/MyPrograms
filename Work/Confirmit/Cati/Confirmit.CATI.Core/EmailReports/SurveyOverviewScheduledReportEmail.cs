using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.EmailReports
{
    internal class SurveyOverviewScheduledReportEmail : ScheduledReportEmail
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISystemSettings _systemSettings;

        public SurveyOverviewScheduledReportEmail(
            ISurveyRepository surveyRepository, ILocalTimeProvider localTimeProvider, ISystemSettings systemSettings, IScheduledEmailReportsRepository scheduledEmailReportsRepository)
            : base(localTimeProvider, scheduledEmailReportsRepository)
        {
            _surveyRepository = surveyRepository;
            _systemSettings = systemSettings;
        }

        public override ReportType ReportType
        {
            get
            {
                return ReportType.SurveyOverview;
            }
        }

        protected override bool ReportEnabled
        {
            get { return ReportSystemSettings.SurveyOverviewReportEnabled; }
        }

        protected override int ReportHour
        {
            get { return ReportSystemSettings.SurveyOverviewReportHour; }
        }

        public override string ReportRecipients
        {
            get { return ReportSystemSettings.SurveyOverviewReportRecepients; }
        }

        public override string ReportDataExportFileName
        {
            get
            {
                return "DailySurveyOverviewData.pdf";
            }
        }

        public override IReportBuilder GetReportBuilder()
        {
            return new SurveyOverviewReportBuilder(_surveyRepository, LocalTimeProvider, _systemSettings);
        }
    }
}