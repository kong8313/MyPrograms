using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.EmailReports
{
    internal class SurveyProductivityScheduledReportEmail : ScheduledReportEmail
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISystemSettings _systemSettings;

        public SurveyProductivityScheduledReportEmail(
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
                return ReportType.SurveyProductivity;
            }
        }

        protected override bool ReportEnabled
        {
            get { return ReportSystemSettings.SurveyProductivityReportEnabled; }
        }

        protected override int ReportHour
        {
            get { return ReportSystemSettings.SurveyProductivityReportHour; }
        }

        public override string ReportRecipients
        {
            get { return ReportSystemSettings.SurveyProductivityReportRecepients; }
        }

        public override string ReportDataExportFileName
        {
            get
            {
                return "DailySurveyProductivityData.pdf";
            }
        }

        public override IReportBuilder GetReportBuilder()
        {
            return new SurveyProductivityReportBuilder(_surveyRepository, LocalTimeProvider, _systemSettings);
        }
    }
}