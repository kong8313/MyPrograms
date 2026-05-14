using System.Globalization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class SentToDialerCallsFiltering
    {
        private const int _nSampleRecords = 100;
        private const int _nSentToDialerRecords = 60;
        
        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private int _timezoneId;
        private BackendTools _backendTools;
        private ISurveyStateService _surveyStateService;

        private const string SurveyPnumber = "p015366";

        private int SurveyId { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            CreateSurveyAndAddSampleAndSetCallStatesToSentCallsToDialer();

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SentToDialerCallsFiltering)]
        public void AllSentToDialerCallReturned()
        {
            var actualRecordSet = CallManager.GetCallsRange(SurveyId, null, CallStates.SentToDialer, new RangingArgs(1, 100, "ID", true), out _);

            Assert.AreEqual(_nSentToDialerRecords, actualRecordSet.Rows.Count);
        }


        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.SentToDialerCallsFiltering)]
        public void FilterByTelNumber_OneCallReturned()
        {
            var searchArgs = SearchTools.SearchBy("TelephoneNumber", SearchColumnType.Text, SearchOperator.Like, "21");

            var actualRecordSet = CallHelper.GetCallsPage(SurveyId, null, _timezoneId, CallStates.SentToDialer, searchArgs, out _, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
            Assert.AreEqual("21", actualRecordSet.Rows[0]["TelephoneNumber"]);
        }

        [TestMethod, Owner("LeonidS"), TestCategory(TestsCategoriesNames.SentToDialerCallsFiltering)]
        public void FilterByTelNumber_NonExistingRecord_NoCallsReturned()
        {
            var searchArgs = SearchTools.SearchBy("TelephoneNumber", SearchColumnType.Text, SearchOperator.Like, (_nSentToDialerRecords + 1).ToString(CultureInfo.InvariantCulture));

            var actualRecordSet = CallHelper.GetCallsPage(SurveyId, null, _timezoneId, CallStates.SentToDialer, searchArgs, out _, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(0, actualRecordSet.Rows.Count);
        }


        private void CreateSurveyAndAddSampleAndSetCallStatesToSentCallsToDialer()
        {
            SurveyId = _backendTools.CreateSurvey(SurveyPnumber);
            _surveyStateService.Open(SurveyId);

            _backendTools.AddSample(SurveyPnumber, 1, (int)SchedulingMode.Simple, 1, _nSampleRecords, null);
            BackendTools.SetCallsStateToSentToDialer(_framework.DbEngine, SurveyId, 1, _nSentToDialerRecords);
        }
    }
}
