using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.Scheduling
{
    [TestClass]
    public class EnableDisableCallTests : BaseMockedIntegrationTest
    {
        private ICallQueueService _callQueueService;

        public override void OnPostTestInitialize()
        {
            _callQueueService = ServiceLocator.Resolve<ICallQueueService>();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DisableSelectedCall_EnabledCall_DisabledCall()
        {
            var call = CreateCall(true);

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(call.SurveySID, new[] { call.InterviewID }, false);

            call.CallState = (int)CallState.DisabledByUser;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DisableSelectedCall_DisabledCall_DisabledCall()
        {
            var call = CreateCall(false);

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(call.SurveySID, new[] { call.InterviewID }, false);

            call.CallState = (int)CallState.DisabledByUser;

            BackendTools.CheckCall(call);
        }

        private BvCallEntity CreateCall(bool enableState)
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);

            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            if (!enableState)
            {
                call.CallState = (int)CallState.DisabledByUser;
            }

            BackendTools.CreateCall(call);
            return call;
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void EnableSelectedCall_DisabledCall_EnabledCall()
        {
            var call = CreateCall(false);

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(call.SurveySID, new[] { call.InterviewID }, true);

            call.CallState = 2;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void EnableSelectedCall_EnabledCall_EnabledCall()
        {
            var call = CreateCall(true);

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(call.SurveySID, new[] { call.InterviewID }, true);

            call.CallState = 2;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(57937)]
        public void ExpiredCall_DisabledCall_DisabledCall()
        {
            var surveyId = BackendToolsObject.CreateSurvey(new TestScript(new Action(Action.Operation.SetNewCallPriority, "10"),
                new Shift(1, 1, "0.00:00:00", "6.00:00:00")));

            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            call.CallState = (int)CallState.DisabledByUser;
            call.TimeToExpire = DateTime.UtcNow.AddDays(-1);

            BackendTools.CreateCall(call);

            var stubIAuthoringService = new StubIAuthoringService { GetDBVersionString = id => 0 };
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoringService);

            _callQueueService.ExpireAllCalls();

            call.CallState = (int)CallState.DisabledByUser;
            call.Priority = 10;
            call.TimeToExpire = new DateTime(9999, 1, 1);

            BackendTools.CheckCall(call);
            CheckHistoryRecordsAreInCallHistory(surveyId, interview.ID);
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(57937)]
        public void ExpiredCall_DisabledCall_EnabledCall()
        {
            var surveyId = BackendToolsObject.CreateSurvey(new TestScript(new Action(Action.Operation.EnableCall, ""),
                new Shift(1, 1, "0.00:00:00", "6.00:00:00")));

            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            call.CallState = 1;
            call.TimeToExpire = DateTime.UtcNow.AddDays(-1);

            BackendTools.CreateCall(call);

            var stubIAuthoringService = new StubIAuthoringService { GetDBVersionString = id => 0 };
            ServiceLocator.RegisterInstance<IAuthoringService>(stubIAuthoringService);

            _callQueueService.ExpireAllCalls();

            call.CallState = 2;
            call.TimeToExpire = new DateTime(9999, 1, 1);

            BackendTools.CheckCall(call);
            CheckHistoryRecordsAreInCallHistory(surveyId, interview.ID);

        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DisabledCall_EnabledCall_EnabledCall()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            test.CreateSurveyWithPerson(DialingMode.Manual, user, password, AgentTaskChoiceMode.Manual);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(4);

            new TestCallManagementOperationFactory().CreateEnableCallsSelected(test.SurveySID, interviews.Take(2).Select(x => x.ID).ToArray(), false);

            test.Login(user, password, AgentTaskChoiceMode.Manual, true);
            test.LoginToDialer(extensionNumber);
            var dataTable = test.WS.GetSurveyInterviews(test.SurveyName, new SearchParameter[] { });
            var result = dataTable.Rows.Cast<DataRow>().Select(x => (int)x["InterviewID"]).ToArray();
            CollectionAssert.AreEquivalent(result, interviews.Skip(2).Select(x => x.ID).ToArray());


        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void EnableFilteredCall_DisabledCallHitToFilter_DisabledCallAreEnabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
               surveyId,
               1,
               new[] { "q1", "q2" },
               new[] { 2, 2 },
               true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 4, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "1" });

            //disable two interviews from different cells
            new TestCallManagementOperationFactory().CreateEnableCallsSelected(surveyId, new[] { interviews[0].ID, interviews[2].ID }, false);
            calls[0].CallState = (int)CallState.DisabledByUser;
            calls[2].CallState = (int)CallState.DisabledByUser;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);

            int filterId = FilterAndPagingTools.CreateSimpleFilter(0,
                AndOrOperator.And,
                new[]{ new FilterField(TableTypes.CFVariables,
                    "q1",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    1,
                    false), new FilterField(TableTypes.CFVariables,
                    "q2",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    1,
                    false) });

            new TestCallManagementOperationFactory().CreateEnableCallsFiltered(surveyId, filterId, true);
            //first interview is enabled
            calls[0].CallState = (int)CallState.Scheduled;
            calls[1].CallState = (int)CallState.Scheduled;
            calls[2].CallState = (int)CallState.DisabledByUser;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DisableFilteredCall_EnabledCallHitToFilter_EnabledCallAreDisabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
               surveyId,
               1,
               new[] { "q1", "q2" },
               new[] { 2, 2 },
               true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 4, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "1" });

            //disable two interviews from different cells
            new TestCallManagementOperationFactory().CreateEnableCallsSelected(surveyId, new[] { interviews[0].ID, interviews[2].ID }, false, true);
            calls[0].CallState = (int)CallState.DisabledByFCD;
            calls[2].CallState = (int)CallState.DisabledByFCD;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);

            int filterId = FilterAndPagingTools.CreateSimpleFilter(surveyId,
                AndOrOperator.And,
                new[]{ new FilterField(TableTypes.CFVariables,
                    "q1",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    1,
                    false), new FilterField(TableTypes.CFVariables,
                    "q2",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    1,
                    false) });

            new TestCallManagementOperationFactory().CreateEnableCallsFiltered(surveyId, filterId, false);
            //first interview is disabled
            calls[0].CallState = (int)CallState.DisabledByUser;
            calls[1].CallState = (int)CallState.DisabledByUser;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void CallDelivery_DisabledCalls_DisabledCallsIsNotDelivered()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            const string user = "testUser";
            const string password = "password";
            const string extensionNumber = "101010";

            test.CreateSurveyWithPerson(DialingMode.Manual, user, password, AgentTaskChoiceMode.Automatic);
            BvInterviewEntity[] interviews = test.CreateInterviewsWithCalls(1);
            new TestCallManagementOperationFactory().CreateEnableCallsSelected(test.SurveySID, interviews.Select(x => x.ID).ToArray(), false);

            test.Login(user, password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(extensionNumber);

            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            Assert.IsNull(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateSelectedWithoutEnabling_DisabledCall_CallIsDisabled()
        {
            var call = CreateCall(false);

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                call.SurveySID, new[] { call.InterviewID }, 2, 0, (int)CallShiftType.None, CallStates.Scheduled, false);

            call.Priority = 2;
            call.CallState = (int)CallState.DisabledByUser;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateSelectedScheduledWithEnabling_DisabledCall_CallIsEnabled()
        {
            var call = CreateCall(false);

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                call.SurveySID, new[] { call.InterviewID }, 2, 0, (int)CallShiftType.None, CallStates.Scheduled, true);

            call.Priority = 2;
            call.CallState = 2;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateSelectedAllWithoutEnabling_DisabledCall_CallIsEnabled()
        {
            var call = CreateCall(false);

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                call.SurveySID, new[] { call.InterviewID }, 2, 0, (int)CallShiftType.None, CallStates.All, false);

            call.Priority = 2;
            call.CallState = (int)CallState.DisabledByUser;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateSelectedAllWithEnabling_DisabledCall_CallIsEnabled()
        {
            var call = CreateCall(false);

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                call.SurveySID, new[] { call.InterviewID }, 2, 0, (int)CallShiftType.None, CallStates.All, true);

            call.Priority = 2;
            call.CallState = 2;

            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCellWithoutEnablingByFullFilter_CallIsNotExists_CallIsCreated()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);
            var fields = new[] { "q1", "q2" };
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                fields,
                new[] { 2, 2 },
                true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            var testedInterview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(testedInterview);

            BackendTools.CreateInterviewsWithCalls(surveyId, 7, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "2", "1" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "2" });
            quota.PutInterviewInCell(interviews[4].ID, new[] { "1", null });
            quota.PutInterviewInCell(interviews[5].ID, new[] { "2", null });
            //the interviews[6] interview doesn't hit to any cell
            quota.PutInterviewInCell(testedInterview.ID, new[] { "1", "2" });

            var cells = new[] { new[] { "1", "2" } };

            new TestCallManagementOperationFactory().CreateActivateCallsFilteredCells(
                surveyId, fields, cells, 10, CallStates.All, 0, (int)CallShiftType.None, false);

            var call = BackendTools.NewCall(testedInterview);

            call.ShiftID = (int)CallShiftType.None;
            call.Priority = 10;

            calls[1].ShiftID = (int)CallShiftType.None;
            calls[1].Priority = 10;

            BackendTools.CheckInterviewsWithCalls(interviews, calls);
            BackendTools.CheckInterview(testedInterview);
            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCellWithEnablingByFullFilter_CallIsNotExists_CallIsCreated()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);
            var fields = new[] { "q1", "q2" };
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                fields,
                new[] { 2, 2 },
                true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            var testedInterview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(testedInterview);

            BackendTools.CreateInterviewsWithCalls(surveyId, 7, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "2", "1" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "2" });
            quota.PutInterviewInCell(interviews[4].ID, new[] { "1", null });
            quota.PutInterviewInCell(interviews[5].ID, new[] { "2", null });
            //the interviews[6] interview doesn't hit to any cell
            quota.PutInterviewInCell(testedInterview.ID, new[] { "1", "2" });

            var cells = new[] { new[] { "1", "2" } };

            new TestCallManagementOperationFactory().CreateActivateCallsFilteredCells(
                surveyId, fields, cells, 10, CallStates.All, 0, (int)CallShiftType.None, true);

            var call = BackendTools.NewCall(testedInterview);

            call.ShiftID = (int)CallShiftType.None;
            call.Priority = 10;

            calls[1].ShiftID = (int)CallShiftType.None;
            calls[1].Priority = 10;

            BackendTools.CheckInterviewsWithCalls(interviews, calls);
            BackendTools.CheckInterview(testedInterview);
            BackendTools.CheckCall(call);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCellWithoutEnablingByFullFilter_DisabledCall_CallIsNotEnabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);
            var fields = new[] { "q1", "q2" };
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                fields,
                new[] { 2, 2 },
                true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            var testedInterview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(testedInterview);
            var testedCall = BackendTools.NewCall(testedInterview);
            testedCall.CallState = 1;
            BackendTools.CreateCall(testedCall);

            BackendTools.CreateInterviewsWithCalls(surveyId, 7, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "2", "1" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "2" });
            quota.PutInterviewInCell(interviews[4].ID, new[] { "1", null });
            quota.PutInterviewInCell(interviews[5].ID, new[] { "2", null });
            //the interviews[6] interview doesn't hit to any cell
            quota.PutInterviewInCell(testedInterview.ID, new[] { "1", "2" });

            var cells = new[] { new[] { "1", "2" } };

            new TestCallManagementOperationFactory().CreateActivateCallsFilteredCells(
                surveyId, fields, cells, 10, CallStates.All, 0, (int)CallShiftType.None, false);

            calls[1].ShiftID = (int)CallShiftType.None;
            calls[1].Priority = 10;
            testedCall.ShiftID = (int)CallShiftType.None;
            testedCall.Priority = 10;
            testedCall.CallState = 1;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);
            BackendTools.CheckInterview(testedInterview);
            BackendTools.CheckCall(testedCall);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCellWithEnablingByFullFilter_DisabledCall_CallIsEnabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);
            var fields = new[] { "q1", "q2" };
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                fields,
                new[] { 2, 2 },
                true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            var testedInterview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(testedInterview);
            var testedCall = BackendTools.NewCall(testedInterview);
            testedCall.CallState = 1;
            BackendTools.CreateCall(testedCall);

            BackendTools.CreateInterviewsWithCalls(surveyId, 7, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "2", "1" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "2" });
            quota.PutInterviewInCell(interviews[4].ID, new[] { "1", null });
            quota.PutInterviewInCell(interviews[5].ID, new[] { "2", null });
            //the interviews[6] interview doesn't hit to any cell
            quota.PutInterviewInCell(testedInterview.ID, new[] { "1", "2" });

            var cells = new[] { new[] { "1", "2" } };

            new TestCallManagementOperationFactory().CreateActivateCallsFilteredCells(
                surveyId, fields, cells, 10, CallStates.All, 0, (int)CallShiftType.None, true);

            testedCall.Priority = 10;
            testedCall.CallState = 2;
            testedCall.ShiftID = (int)CallShiftType.None;

            calls[1].ShiftID = (int)CallShiftType.None;
            calls[1].Priority = 10;

            BackendTools.CheckInterviewsWithCalls(interviews, calls);
            BackendTools.CheckInterview(testedInterview);
            BackendTools.CheckCall(testedCall);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCellWithoutEnablingByNotFullFilter_DisabledCall_CallIsNotEnabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 },
                true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            var testedInterview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(testedInterview);
            var testedCall = BackendTools.NewCall(testedInterview);
            testedCall.CallState = 1;
            BackendTools.CreateCall(testedCall);

            BackendTools.CreateInterviewsWithCalls(surveyId, 7, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "2", "1" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "2" });
            quota.PutInterviewInCell(interviews[4].ID, new[] { "1", null });
            quota.PutInterviewInCell(interviews[5].ID, new[] { "2", null });
            //the interviews[6] interview doesn't hit to any cell
            quota.PutInterviewInCell(testedInterview.ID, new[] { "1", null });

            var fields = new[] { "q1" };
            var cells = new[] { new[] { "1" } };

            new TestCallManagementOperationFactory().CreateActivateCallsFilteredCells(
                surveyId, fields, cells, 10, CallStates.All, 0, (int)CallShiftType.None, false);

            calls[0].ShiftID = (int)CallShiftType.None;
            calls[0].Priority = 10;
            calls[1].ShiftID = (int)CallShiftType.None;
            calls[1].Priority = 10;
            calls[4].ShiftID = (int)CallShiftType.None;
            calls[4].Priority = 10;

            testedCall.ShiftID = (int)CallShiftType.None;
            testedCall.Priority = 10;
            testedCall.CallState = 1;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);
            BackendTools.CheckInterview(testedInterview);
            BackendTools.CheckCall(testedCall);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateCellWithEnablingByNotFullFilter_DisabledCall_CallIsEnabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 },
                true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;

            var testedInterview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(testedInterview);
            var testedCall = BackendTools.NewCall(testedInterview);
            testedCall.CallState = 1;
            BackendTools.CreateCall(testedCall);

            BackendTools.CreateInterviewsWithCalls(surveyId, 7, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "2", "1" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "2" });
            quota.PutInterviewInCell(interviews[4].ID, new[] { "1", null });
            quota.PutInterviewInCell(interviews[5].ID, new[] { "2", null });
            //the interviews[6] interview doesn't hit to any cell
            quota.PutInterviewInCell(testedInterview.ID, new[] { "1", null });

            var fields = new[] { "q1" };
            var cells = new[] { new[] { "1" } };

            new TestCallManagementOperationFactory().CreateActivateCallsFilteredCells(
                surveyId, fields, cells, 10, CallStates.All, 0, (int)CallShiftType.None, true);

            calls[0].ShiftID = (int)CallShiftType.None;
            calls[0].Priority = 10;
            calls[1].ShiftID = (int)CallShiftType.None;
            calls[1].Priority = 10;
            calls[4].ShiftID = (int)CallShiftType.None;
            calls[4].Priority = 10;
            testedCall.ShiftID = (int)CallShiftType.None;
            testedCall.Priority = 10;
            testedCall.CallState = 2;

            BackendTools.CheckInterviewsWithCalls(interviews, calls);
            BackendTools.CheckInterview(testedInterview);
            BackendTools.CheckCall(testedCall);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void ActivateFilteredCallInAllModeWithEnabling_DisabledCallHitToFilter_DisabledCallsAreEnabled()
        {
            var surveyId = BackendToolsObject.CreateSurvey((string)null);

            var quota = TestQuota.Create(TestingFramework.DbEngine,
               surveyId,
               1,
               new[] { "q1", "q2" },
               new[] { 2, 2 },
               true);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 4, out interviews, out calls);
            quota.PutInterviewInCell(interviews[0].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[1].ID, new[] { "1", "1" });
            quota.PutInterviewInCell(interviews[2].ID, new[] { "1", "2" });
            quota.PutInterviewInCell(interviews[3].ID, new[] { "2", "1" });

            //disable two interviews from different cells
            new TestCallManagementOperationFactory().CreateEnableCallsSelected(surveyId, new[] { interviews[0].ID, interviews[2].ID }, false, true);
            calls[0].CallState = (int)CallState.DisabledByFCD;
            calls[2].CallState = (int)CallState.DisabledByFCD;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);

            int filterId = FilterAndPagingTools.CreateSimpleFilter(0,
                AndOrOperator.And,
                new[]{ new FilterField(TableTypes.CFVariables,
                    "q1",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    1,
                    false), new FilterField(TableTypes.CFVariables,
                    "q2",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    1,
                    false) });

            var timezoneId = ServiceLocator.Resolve<ITimezoneService>().GetDefaultCallCenterTimezoneId();
            new TestCallManagementOperationFactory().CreateActivateCallsFiltered(
                surveyId, filterId, 10, 0, (int)CallShiftType.None, timezoneId, CallStates.All, true);

            //first interview is disabled
            calls[0].Priority = 10;
            calls[0].ShiftID = (int)CallShiftType.None;
            calls[0].CallState = (int)CallState.Scheduled;
            calls[1].Priority = 10;
            calls[1].ShiftID = (int)CallShiftType.None;
            BackendTools.CheckInterviewsWithCalls(interviews, calls);
        }

        private void CheckHistoryRecordsAreInCallHistory(int surveyId, int interviewId)
        {
            var callHistory = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", surveyId), new SqlParameter("@InterviewId", interviewId)).Single();

            var history = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", surveyId), new SqlParameter("@InterviewId", interviewId));

            Assert.AreEqual((int)OperationType.ExpiredCall, callHistory.OperationType);
            Assert.AreEqual(0, history.Count);
        }
    }
}
