using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class FlushCallsTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private FusionLibTestTools _fusionLibTools;

        private ITelephony _telephony;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize(true);
            _backendTools = new BackendTools(_framework);
            _fusionLibTools = new FusionLibTestTools(_backendTools);

            _timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            _telephony = ServiceLocator.Resolve<ITelephony>();
            _now = TimezoneManager.GetCurrentTimeByTzId(_timezoneId);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        const int Priority = 5;
        const int NewPriority = Priority * 3;
        private DateTime _now;
        private int _timezoneId;

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void SendCallsToDialerPredictively_CloseQuotaCell_FlushCallsIsCalled()
        {
            TestQuota quota;
            int cellId1;

            long campaignIdFlushCalledWith = 0;
            var stubITelephony = new StubITelephony
            {
                FlushNumbersInt64ListOfCallInfo = (id, list) => { campaignIdFlushCalledWith = id; }
            };
            Stubs.SetNewITelephonyStub(stubITelephony);

            var surveySid = PrepareTestCallsToBeFlushed(out quota, out cellId1, true);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            quota.CloseCell(cellId1);

            //Assert 
            var surveyEntity = SurveyRepository.GetById(surveySid);
            Assert.AreEqual(ProjectIdConverter.ProjectIdToCampaignId(surveyEntity.ProjectId), campaignIdFlushCalledWith);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void SendCallsToDialerPredictively_CloseQuotaCell_CorrectSetOfCallsIsTakenForFlushing()
        {
            TestQuota quota;
            int cellId1;
            int surveySid = PrepareTestCallsToBeFlushed(out quota, out cellId1, true);

            //Create another survey with the same interview ids to check that only interviews from one correct survey are taken to flush numbers on dialer
            int otherSurveySid;
            int otherPersonSid;
            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out otherSurveySid, out otherPersonSid, (int)AgentTaskChoiceMode.CampaignAssignment);
            FusionLibTestTools.CreateInterviewsForTestWithTelephoneNumbers(otherSurveySid, new[] { 1, 2, 3, 4 }, true).ToList();

            var callList = new List<CallInfo>();

            var originaICallsManagementService = ServiceLocator.Resolve<ICallsManagementService>();
            var stubICallsManagementService = new StubICallsManagementService
            {
                GetCallsToFlushOnDialerInt32Int32Boolean = (surveyId, batchId, isRecording) =>
                {
                    callList = originaICallsManagementService.GetCallsToFlushOnDialer(surveyId, batchId, isRecording);

                    return callList;
                },
                RemoveFilteredCallsInt32Int32NullableOfInt32 = (id, batchId, newIts) => { }
            };
            ServiceLocator.RegisterInstance<ICallsManagementService>(stubICallsManagementService);

            ServiceLocator.Resolve<IInterviewQuotaCellService>().Populate(surveySid, (CancellationToken)default);
            
            quota.CloseCell(cellId1);

            // Check that two calls which correspond to the closed cell were flushed on dialer: they are calls for interviews 1 and 3.
            Assert.AreEqual(2, callList.Count); // calls to flush

            Assert.AreEqual(1, callList[0].interviewId);
            Assert.AreEqual("1", callList[0].phoneNumber);
            Assert.AreEqual(3, callList[1].interviewId);
            Assert.AreEqual("3", callList[1].phoneNumber);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(61171)]
        public void CloseQuotaCell_NoCallsToBeFlushed_FlushCallsIsNotCalled()
        {
            TestQuota quota;
            int cellId1;

            PrepareTestCallsToBeFlushed(out quota, out cellId1, false);

            bool flushNumberIsCalled = false;
            var stubITelephony = new StubITelephony
            {
                FlushNumbersInt64ListOfCallInfo = (id, list) => { flushNumberIsCalled = true; }
            };
            Stubs.ExtendExistingITelephonyStub(_telephony, stubITelephony);

            quota.CloseCell(cellId1);

            //Assert 
            Assert.IsFalse(flushNumberIsCalled);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(60617)]
        public void CloseQuotaCell_DialingModeIsPreview_FlushCallsIsNotCalled()
        {
            CheckFlushCallsIsNotCalled(DialingMode.Preview);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(60617)]
        public void CloseQuotaCell_DialingModeIsManual_FlushCallsIsNotCalled()
        {
            CheckFlushCallsIsNotCalled(DialingMode.Manual);
        }

        [TestMethod, Owner(@"FIRM\MikhailT"), Bug(60617)]
        public void CloseQuotaCell_DialingModeIsAutomatic_FlushCallsIsNotCalled()
        {
            CheckFlushCallsIsNotCalled(DialingMode.Automatic);
        }

        private int PrepareTestCallsToBeFlushed(out TestQuota quota, out int cellId1, bool putInterviewsInCells)
        {
            int surveySid;
            int personSid;
            List<BvInterviewEntity> interviews;
            _fusionLibTools.CreateSurveyWithPersonForTest(SchedulingScriptType.Default, out surveySid, out personSid, (int)AgentTaskChoiceMode.CampaignAssignment);
            interviews = FusionLibTestTools.CreateInterviewsForTestWithTelephoneNumbers(surveySid, new[] { 1, 2, 3, 4 }, true).ToList();

            SurveyService.SetDialingMode(surveySid, DialingMode.Predictive);

            //there are should be 2 cells
            quota = TestQuota.Create(_framework.DbEngine,
                surveySid,
                1,
                new[] { "q1" },//quota column
                new[] { 2 });

            cellId1 = 1;
            const int cellId2 = 2;

            if (putInterviewsInCells)
            {
                quota.PutInterviewsInCells(
                    new[] { interviews[0].ID, interviews[1].ID, interviews[2].ID, interviews[3].ID },
                    new[] { cellId1, cellId2, cellId1, cellId2 });
            }

            BackendTools.SyncResponseControl(_framework.DbEngine, surveySid);

            //Sync data
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveySid,
                0 /*filterSid*/, //all scheduled calls
                NewPriority,
                personSid,
                (int)CallShiftType.None/*shifttypeid*/,
                _timezoneId,
                _now,
                CallStates.Suspended,
                false);

            BackendTools.LoginPerson(personSid, "");
            PersonService.LoginPersonOnSurveyForSurveySelectionMode(personSid, surveySid);

            // Emulate calls moved to dialer predictively
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteEntityList(surveySid, 1, 4, currentTime, 0);

            return surveySid;
        }

        private void CheckFlushCallsIsNotCalled(DialingMode dialingMode)
        {
            TestQuota quota;
            int cellId1;
            var surveySid = PrepareTestCallsToBeFlushed(out quota, out cellId1, true);
            SurveyService.SetDialingMode(surveySid, dialingMode);

            bool flushIsCalled = false;
            var stubITelephony = new StubITelephony
            {
                FlushNumbersInt64ListOfCallInfo = (id, list) => { flushIsCalled = true; }
            };
            ServiceLocator.RegisterInstance<ITelephony>(stubITelephony);

            quota.CloseCell(cellId1);

            //Assert 
            Assert.IsFalse(flushIsCalled);
        }
    }
}
