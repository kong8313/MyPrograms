using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Tests.QuotaBalancing.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Common;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaBalancing
{
    [TestClass]
    public class LifeCycleOfPromotedCallsTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;

        const string ProjectId = "p0012132";
        const int PromotionThreshold = 9;
        const int QuotaId = 1;
        const int PromotionPriority = 10;
        const int ProcessedCallsPerMinute = 2;

        private TestQuota _quota;
        private int _surveyId;
        private BvInterviewEntity _interview1;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize(false);
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _backendTools.LaunchAllHoursScript();
            _surveyId = _backendTools.CreateSurvey(ProjectId);
            _surveyStateService.Open(_surveyId);
            QuotaBalancingTools.SetProcessedCallsPerMinute(ProcessedCallsPerMinute, ProcessedCallsPerMinute, _surveyId);

            _quota = TestQuota.Create(_framework.DbEngine,
                _surveyId,
                QuotaId,
                new[] { "q1" },
                new[] { 2 },
                new[] { 4, 5 },
                new[] { 10, 10 });

            _quota.MarkQuotaAsBalanced(PromotionPriority, new[] { "q1" }, PromotionThreshold);

            _interview1 = BackendTools.CreateInterviewWithCall(_surveyId);

            _quota.PutInterviewsInCells(
                new[] { _interview1.ID },
                new[] { 1 });
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        private void AssertOldPriority(int oldPriority)
        {
            var call = BvSvyScheduleAdapter.GetAll().First();
            Assert.AreEqual(oldPriority, call.OldPriority, "OldPriority should be changed during promotion routine");
        }

        private void AssertNewPriority(int newPriority)
        {
            var call = BvSvyScheduleAdapter.GetAll().First();
            Assert.AreEqual(newPriority, call.Priority, "Priority should be set to previous value");
        }

        private void LifeCycleOfPromotedCalls_ActivateCall_OldPriorityIs0(CallStates callStates)
        {
            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                _surveyId, new[] { _interview1.ID }, 10, 0, -1, DateTime.UtcNow, callStates, false);

            AssertOldPriority(0);
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_ActivateAllCall_OldPriorityIs0()
        {
            LifeCycleOfPromotedCalls_ActivateCall_OldPriorityIs0(CallStates.All);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_ActivateSheduledCall_OldPriorityIs0()
        {
            LifeCycleOfPromotedCalls_ActivateCall_OldPriorityIs0(CallStates.Scheduled);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_IncrementPriorityDuringShedulingPromotionCall_IncrementPriorityFromOld()
        {
            const int its = 17;
            var script = new TestScript(
                    new SubRule(new Action(Action.Operation.IncrementPriority, "1"), its, 0, 2, null, false),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(2, 1, "1.00:00:00", "2.00:00:00"),
                    new Shift(3, 1, "2.00:00:00", "3.00:00:00"),
                    new Shift(4, 1, "3.00:00:00", "4.00:00:00"),
                    new Shift(5, 1, "4.00:00:00", "5.00:00:00"),
                    new Shift(6, 1, "5.00:00:00", "6.00:00:00"),
                    new Shift(7, 1, "6.00:00:00", "0.00:00:00"));
            _backendTools.LaunchScript(_surveyId, script);

            CallTools.MoveAndRescheduleCalls(_surveyId, new[] { _interview1.ID }, its);

            var actualCall = CallQueueService.GetCallAndNoLock(_surveyId, _interview1.ID);

            Assert.AreEqual(2, actualCall.Priority, "Priority");
            Assert.AreEqual(0, actualCall.OldPriority, "OldPriority");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_CallIsNotUpdatedDuringScheduling_CallIsDeleted()
        {
            const int its = 17;
            var script = new TestScript(
                    new SubRule(new Action(Action.Operation.SetNewITS, "61"), its, 0, 2, null, false),
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(2, 1, "1.00:00:00", "2.00:00:00"),
                    new Shift(3, 1, "2.00:00:00", "3.00:00:00"),
                    new Shift(4, 1, "3.00:00:00", "4.00:00:00"),
                    new Shift(5, 1, "4.00:00:00", "5.00:00:00"),
                    new Shift(6, 1, "5.00:00:00", "6.00:00:00"),
                    new Shift(7, 1, "6.00:00:00", "0.00:00:00"));
            _backendTools.LaunchScript(_surveyId, script);

            CallTools.MoveAndRescheduleCalls(_surveyId, new[] { _interview1.ID }, its);

            var actualCall = CallQueueService.GetCallAndNoLock(_surveyId, _interview1.ID);

            Assert.IsNull(actualCall);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_UpdateCall_OldPriorityIs0()
        {
            var call = CallQueueService.GetCallAndNoLock(_surveyId, _interview1.ID);

            CallManager.UpdateCall(call);

            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_MoveCall_OldPriorityIs0()
        {
            CallTools.MoveCalls(_surveyId, new[] { _interview1.ID }, 17);

            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_AssignCall_OldPriorityIs0NewPriorityIs1()
        {
            var personId = PersonTools.CreatePerson("Spi");
            CallTools.AssignCalls(_surveyId, new[] { _interview1.ID }, personId);

            AssertNewPriority(1);
            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_AssignFilterredCall_OldPriorityIs0NewPriorityIs1()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.Or,
                new[]{ new FilterField(TableTypes.Call,
                    "InterviewID",
                    VariableTypes.Integer,
                    FilterOperator.Equal,
                    _interview1.ID,
                    false)});

            var personId = PersonTools.CreatePerson("Spi");
            CallTools.AssignCalls(_surveyId, filterId, personId);

            AssertNewPriority(1);
            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_ChangeCallsPriority_OldPriorityIs0()
        {
            CallTools.ChangeCallsPriority(_surveyId, new[] { _interview1.ID }, CallStates.Scheduled, 1);

            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_ChangeShiftTypeToAnyValid_OldPriorityIs0NewPriorityIs1()
        {
            CallTools.ChangeCallsShiftType(_surveyId, new[] { _interview1.ID }, CallStates.Scheduled, (int)CallShiftType.AnyValid);

            AssertNewPriority(1);
            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_ChangeShiftTypeToAnyNone_OldPriorityIs0NewPriorityIs1()
        {
            CallTools.ChangeCallsShiftType(_surveyId, new[] { _interview1.ID }, CallStates.Scheduled, (int)CallShiftType.None);

            AssertNewPriority(1);
            AssertOldPriority(0);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void LifeCycleOfPromotedCalls_ChangeShiftTypeTo1_OldPriorityIs0NewPriorityIs1()
        {
            CallTools.ChangeCallsShiftType(_surveyId, new[] { _interview1.ID }, CallStates.Scheduled, 1);

            AssertNewPriority(1);
            AssertOldPriority(0);
        }
    }
}
