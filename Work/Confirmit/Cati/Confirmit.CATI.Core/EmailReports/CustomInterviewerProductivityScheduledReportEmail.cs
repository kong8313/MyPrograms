using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.EmailReports
{
    internal class CustomInterviewerProductivityScheduledReportEmail : ScheduledReportEmail
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISystemSettings _systemSettings;
        private readonly ISupervisorApiClient _supervisorApiClient;

        public CustomInterviewerProductivityScheduledReportEmail(
            ISurveyRepository surveyRepository, ILocalTimeProvider localTimeProvider, ISystemSettings systemSettings, IScheduledEmailReportsRepository scheduledEmailReportsRepository, ISupervisorApiClient supervisorApiClient)
            : base(localTimeProvider, scheduledEmailReportsRepository)
        {
            _surveyRepository = surveyRepository;
            _systemSettings = systemSettings;
            _supervisorApiClient = supervisorApiClient;
        }

        public override ReportType ReportType
        {
            get
            {
                return ReportType.InterviewerProductivity;
            }
        }

        protected override bool ReportEnabled
        {
            get { return ReportSystemSettings.InterviewerProductivityReportEnabled; }
        }

        protected override int ReportHour
        {
            get { return ReportSystemSettings.InterviewerProductivityReportHour; }
        }

        public override string ReportRecipients
        {
            get { return ReportSystemSettings.InterviewerProductivityReportRecepients; }
        }

        public override string ReportDataExportFileName
        {
            get
            {
                return "DailyInterviewerProductivityData.pdf";
            }
        }

        public override IReportBuilder GetReportBuilder()
        {
            return new CustomInterviewerProductivityReportBuilder(_surveyRepository, LocalTimeProvider, _systemSettings, _supervisorApiClient);
        }
    }
}