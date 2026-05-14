using System;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using TelerikReport = Telerik.Reporting.Report;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.EmailReports
{
    public class CustomInterviewerProductivityReportBuilder : TelerikReportBuilder
    {
        private readonly Reports.InterviewerProductivityCustomReport _report;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ILocalTimeProvider _localTimeProvider;
        private readonly ISupervisorApiClient _supervisorApiClient;
        private int _templateId;
        private InterviewerProductivityReportTemplate _template;

        public CustomInterviewerProductivityReportBuilder(
            ISurveyRepository surveyRepository,
            ILocalTimeProvider localTimeProvider, 
            ISystemSettings systemSettings,
            ISupervisorApiClient supervisorApiClient)
        {
            _report = new InterviewerProductivityCustomReport();
            _templateId = systemSettings.Reports.ScheduledInterviewerProductivityReportTemplateId;
            _surveyRepository = surveyRepository;
            _localTimeProvider = localTimeProvider;
            _supervisorApiClient = supervisorApiClient;
        }

        public override IReport BuildReport(DateTime reportStartTime, DateTime reportEndTime)
        {
            int[] statusIds = { (int)CallOutcome.Completed };

            var allSurveys = _surveyRepository.GetAll();
            var allSurveySids = allSurveys.Select(item => item.SID).ToList();

            _template = _templateId > 0
                ? _supervisorApiClient.GetTemplate(_templateId).Result
                : _supervisorApiClient.GetSystemTemplate().Result;

            var report = new CustomInterviewerProductivityReport
            {
                Title = "Forsta CATI Daily Interviewer Productivity Export",
                Name = "Interviewer Productivity",

                StartDate = _localTimeProvider.ConvertToLocalTime(reportStartTime),
                EndDate = _localTimeProvider.ConvertToLocalTime(reportEndTime),

                ReportDate = _localTimeProvider.GetCurrentLocalTime(),
                PersonNames = "All",
                SurveyNames = "All",

                IncludeBreaksInAverages = _template.IncludeBreakTimeInCalculations, // Do not include break times in calculations

                SurveyDataFilter = "N/A",

                DbSurveyIds = ReportManager.ConvertArrayToStringParameter(allSurveySids),
                DbPersonIds = null,
                DbStateIds = ReportManager.ConvertArrayToStringParameter(statusIds),
                DbShowDialerAttempts = _template.ShowDialerAttempts,
                DbCalcAllBreakHistory = true,
                DbHideEmpty = !_template.IncludeZeroValues,
                DbStartDate = reportStartTime,
                DbEndDate = reportEndTime,
                DbSurveyDataFilter = null
            };

            return report;
        }

        public override void Prepare()
        {
            _report.Prepare(_template);
        }

        protected override TelerikReport Report => _report;
    }
}