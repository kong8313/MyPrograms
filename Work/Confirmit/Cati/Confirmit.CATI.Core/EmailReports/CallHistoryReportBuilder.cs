using System;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Export;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.EmailReports
{
    public class CallHistoryReportBuilder : IReportBuilder
    {
        private readonly ILocalTimeProvider _localTimeProvider;
        private readonly ICallHistoryDataProvider _dataProvider;
        private readonly IReportsSettings _reportSettings;
        private readonly ISurveyService _surveyService;
        private const string ReportDataExportFileName = "DailyCallHistoryData.txt";

        public bool ShouldBeEncrypted { get { return true; } }
        public void Prepare()
        {
        }

        public CallHistoryReportBuilder(ILocalTimeProvider localTimeProvider)
        {
            _localTimeProvider = localTimeProvider;
            _dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();
            _reportSettings = ServiceLocator.Resolve<IReportsSettings>();
            _surveyService = ServiceLocator.Resolve<ISurveyService>();
        }

        public IReport BuildReport(DateTime reportStartTime, DateTime reportEndTime)
        {
            var cleaner = new DelimitedStringCleaner();
            var replicatedVariables = cleaner.ParseString(_reportSettings.CallHistoryReportReplicatedVariables);
            var includeVariables =
                _dataProvider.IncludeReplicatedVariables = _reportSettings.CallHistoryReportReplicatedVariablesEnabled;

            var callHistoryList = _surveyService.GetCallHistoryData(null, reportStartTime, reportEndTime,
                includeVariables ? replicatedVariables.ToArray() : null);

            // Include time breaks

            var interviewerTimeBreaksEntities = _dataProvider.GetInterviewerBreaksData(null, reportStartTime, reportEndTime);
            callHistoryList = callHistoryList.Concat(interviewerTimeBreaksEntities).ToList();

            //Include login logout events
            var bvSpCallHistoryDataEntities = _dataProvider.GetPersonSessionHistoryData(null, reportStartTime, reportEndTime);
            callHistoryList = callHistoryList.Concat(bvSpCallHistoryDataEntities).ToList();

            // we use tab separated values format to export call history data
            var dsvString = DsvManager.ExportToDsv(callHistoryList, "\t", _dataProvider.PrepareForExport);

            if (includeVariables)
            {
                dsvString = _dataProvider.GetHeader(_reportSettings.CallHistoryReportReplicatedVariables) + dsvString;
            }

            return new CallHistoryReport
            {
                Title = "Forsta CATI Daily Call History Export",
                Name = "Call History",
                ReportDataSource = callHistoryList,
                DsvString = dsvString,

                StartDate = _localTimeProvider.ConvertToLocalTime(reportStartTime),
                EndDate = _localTimeProvider.ConvertToLocalTime(reportEndTime)
            };
        }

        public string ExportReportToDisk(IReport report, string fileName)
        {
            // we pack DSV data file before sending it to client
            return new Packaging().CreatePackage(ReportDataExportFileName, ((CallHistoryReport)report).DsvString);
        }
    }
}