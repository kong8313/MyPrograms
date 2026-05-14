using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class FilteringByDate
    {
        public static readonly DateTime TimeInsteadNowTimeToCall = CallQueueService.DefaultTimeInShift; 

        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private BackendTools _backendTools;
        private int _timezoneId;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
            TimezoneManager.AddTimezone(62);  //Eastern time (US & Canada)
            _timezoneId = 62;
            var callCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
            
            var callCenter = callCenterRepository.Default;
            callCenter.LocalTimezoneId = _timezoneId;
            callCenterRepository.Update(callCenter);

            _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.ClearTestHttpContextCurrent();

            _framework.TestCleanup();
        }

        [TestMethod, Owner("FIRM/LeonidS"), TestCategory(TestsCategoriesNames.FilteringByDate)]
        public void FilterByDate_NotEqual_ByTimeInShift_OneRecordReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCalls();

            var searchArgs = SearchTools.SearchByDateColumn("TimeInShift", SearchOperator.NotEqual, new DateTime(2016, 1, 1, 19, 22, 23));

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("FIRM/LeonidS"), TestCategory(TestsCategoriesNames.FilteringByDate)]
        public void  FilterByDate_Equal_ByTimeInShift_2RecordReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCalls();

            var searchArgs = SearchTools.SearchByDateColumn("TimeInShift", SearchOperator.Equal, new DateTime(2016, 1, 1, 19, 22, 23));

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(2, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("FIRM/LeonidS"), TestCategory(TestsCategoriesNames.FilteringByDate)]
        public void FilterByDate_Equal_ByTimeInShiftForToday_2RecordReturned()
        {
            int totalCount;
            var utcNow = new DateTime(2017, 1, 1, 13, 0, 0);
            new DateTimeMocker(_framework).MockDate(utcNow);

            var context = CreateSurveyWithCalls();
            CallTools.ActivateCalls(context.GetSurvey("S1").Id, 1, CallStates.All, 0, (int)CallShiftType.None, TimeInsteadNowTimeToCall, true,
               context.GetInterviews("S1.I1").Select(x => x.Id));
            CallTools.ActivateCalls(context.GetSurvey("S1").Id, 1, CallStates.All, 0, (int)CallShiftType.None, utcNow, true,
                context.GetInterviews("S1.I2").Select(x => x.Id));

            var searchArgs = SearchTools.SearchByDateColumn("TimeInShift", SearchOperator.Equal, TimezoneManager.ConvertToTzLocalTime(_timezoneId, utcNow));

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(2, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("FIRM/LeonidS"), TestCategory(TestsCategoriesNames.FilteringByDate)]
        public void FilterByDate_NotEqual_ByTimeInShiftForToday_1RecordReturned()
        {
            int totalCount;
            var utcNow = new DateTime(2017, 1, 1, 13, 0, 0);
            new DateTimeMocker(_framework).MockDate(utcNow);

            var context = CreateSurveyWithCalls();
            CallTools.ActivateCalls(context.GetSurvey("S1").Id, 1, CallStates.All, 0, (int)CallShiftType.None, TimeInsteadNowTimeToCall, true,
               context.GetInterviews("S1.I1").Select(x => x.Id));
            CallTools.ActivateCalls(context.GetSurvey("S1").Id, 1, CallStates.All, 0, (int)CallShiftType.None, utcNow, true,
                context.GetInterviews("S1.I2").Select(x => x.Id));

            var searchArgs = SearchTools.SearchByDateColumn("TimeInShift", SearchOperator.NotEqual, TimezoneManager.ConvertToTzLocalTime(_timezoneId, utcNow));

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("FIRM/LeonidS"), TestCategory(TestsCategoriesNames.FilteringByDate)]
        public void FilterByDate_Equal_ByExpireTime_1RecordReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCalls();

            var searchArgs = SearchTools.SearchByDateColumn("ExpireTime", SearchOperator.Equal, new DateTime(2016, 2, 2, 19, 22, 23));

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
        }

        [TestMethod, Owner("FIRM/LeonidS"), TestCategory(TestsCategoriesNames.FilteringByDate)]
        public void FilterByDate_NotEqual_ByExpireTime_2RecordReturned()
        {
            int totalCount;

            var context = CreateSurveyWithCalls();

            var searchArgs = SearchTools.SearchByDateColumn("ExpireTime", SearchOperator.NotEqual, new DateTime(2016, 2, 2, 19, 22, 23));

            var actualRecordSet = CallHelper.GetCallsPage(context.GetSurvey("S1").Id, null, _timezoneId, CallStates.Scheduled, searchArgs, out totalCount, ShowTimeMode.Interviewer, false);

            Assert.AreEqual(2, actualRecordSet.Rows.Count);
        }


        private TestDataContext CreateSurveyWithCalls()
        {
            var context = new TestData
            {
                Surveys = new[]
                { 
                    new SurveyData 
                    { Tag="S1", IsUseDb = true,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData(){Model = {TimeInShift = DateTime.Parse("01/01/2016 19:22:23"),TimeToExpire = DateTime.Parse("02/02/2016 19:22:23") }}},
                            new InterviewData(){Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData(){Model = {TimeInShift = DateTime.Parse("01/01/2016 19:22:23")}}},
                            new InterviewData(){Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData(){Model = {TimeInShift = DateTime.Parse("01/02/2016 19:22:23")}}},
                        }
                   }
                }
            }.Create();

            return context;
        }

    }
}
