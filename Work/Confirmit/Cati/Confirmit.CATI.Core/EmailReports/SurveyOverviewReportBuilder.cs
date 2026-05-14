using System;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;
using TelerikReport = Telerik.Reporting.Report;

namespace Confirmit.CATI.Core.EmailReports
{
    public class SurveyOverviewReportBuilder : TelerikReportBuilder
    {
        private readonly SurveyOverviewReport _report;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ILocalTimeProvider _localTimeProvider;
        private readonly ISystemSettings _systemSettings;
        
        public SurveyOverviewReportBuilder(ISurveyRepository surveyRepository, ILocalTimeProvider localTimeProvider, ISystemSettings systemSettings)
        {
            _report = new SurveyOverviewReport();
            _report.SqlDataSource.ConnectionString = BackendInstance.Current.ConnectionString;
            _report.SqlDataSource.CommandTimeout = systemSettings.Reports.ReportGenerationTimeout;
            _surveyRepository = surveyRepository;
            _localTimeProvider = localTimeProvider;
            _systemSettings = systemSettings;
        }

        public override IReport BuildReport(DateTime reportStartTime, DateTime reportEndTime)
        {
            int[] statusIds = { (int)CallOutcome.Completed };

            var allSurveys = _surveyRepository.GetAll();
            var allSurveySids = allSurveys.Select(item => item.SID).ToList();

            var report = new SurveyPersonStatisticsDatedDurationCollectionDataReport
            {
                Title = "Forsta CATI Daily Survey Overview Export",
                Name = "Survey Overview",

                StartDate = _localTimeProvider.ConvertToLocalTime(reportStartTime),
                EndDate = _localTimeProvider.ConvertToLocalTime(reportEndTime),

                ReportDate = _localTimeProvider.GetCurrentLocalTime(),
                PersonNames = "All",
                SurveyNames = "All",
                SurveyDataFilter = "N/A",

                DbSurveyIds = ReportManager.ConvertArrayToStringParameter(allSurveySids),
                DbPersonIds = null,
                DbStateIds = ReportManager.ConvertArrayToStringParameter(statusIds),
                DbShowDialerAttempts = true,
                DbHideEmpty = true,
                DbStartDate = reportStartTime,
                DbEndDate = reportEndTime,
                DbSurveyDataFilter = null,
                IncludeOpenEndReviewTimeInInterviewDuration = _systemSettings.Console.IncludeOpenEndReviewTimeInInterviewDuration
            };

            return report;
        }

        protected override TelerikReport Report
        {
            get { return _report; }
        }
    }
}