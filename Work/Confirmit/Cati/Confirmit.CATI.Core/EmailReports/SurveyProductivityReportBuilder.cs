using System;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using TelerikReport = Telerik.Reporting.Report;

namespace Confirmit.CATI.Core.EmailReports
{
    public class SurveyProductivityReportBuilder : TelerikReportBuilder
    {
        private readonly Reports.SurveyProductivityReport _report;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ILocalTimeProvider _localTimeProvider;

        public SurveyProductivityReportBuilder(ISurveyRepository surveyRepository, ILocalTimeProvider localTimeProvider, ISystemSettings systemSettings)
        {
            _report = new Reports.SurveyProductivityReport();
            _report.SqlDataSource.ConnectionString = BackendInstance.Current.ConnectionString;
            _report.SqlDataSource.CommandTimeout = systemSettings.Reports.ReportGenerationTimeout;
            _surveyRepository = surveyRepository;
            _localTimeProvider = localTimeProvider;
        }

        public override IReport BuildReport(DateTime reportStartTime, DateTime reportEndTime)
        {
            var allSurveys = _surveyRepository.GetAll();
            var allSurveySids = allSurveys.Select(item => item.SID).ToList();

            var report = new SurveyProductivityReport
            {
                Title = "Forsta CATI Daily Survey Productivity Export",
                Name = "Survey Productivity",

                StartDate = _localTimeProvider.ConvertToLocalTime(reportStartTime),
                EndDate = _localTimeProvider.ConvertToLocalTime(reportEndTime),

                ReportDate = _localTimeProvider.GetCurrentLocalTime(),
                PersonNames = "All",
                SurveyNames = "All",

                ITSNames = "All",
                SurveyDataFilter = "N/A",
                IncludePercentage = false,

                DbSurveyIds = ReportManager.ConvertArrayToStringParameter(allSurveySids),
                DbPersonIds = null,
                DbStateIds = null,
                DbStartDate = reportStartTime,
                DbEndDate = reportEndTime,
                DbSurveyDataFilter = null
            };

            return report;
        }

        protected override TelerikReport Report
        {
            get { return _report; }
        }
    }
}