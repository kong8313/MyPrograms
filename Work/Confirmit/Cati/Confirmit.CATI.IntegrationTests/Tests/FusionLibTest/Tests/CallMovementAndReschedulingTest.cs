using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.IntegrationTests.Framework;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.Test.Common.Attributes;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallMovementAndReschedulingTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            var script = new TestScript(
                new SubRule(
                    new Action(Action.Operation.IncrementPriority, "1"),
                    NewIts, 0, 2, "", false),
                new object[]{new Shift(1, (int)ShiftTypeIDs.Sunday, "0.00:00:00", "1.00:00:00"),
                      new Shift(2, (int)ShiftTypeIDs.Default, "1.00:00:00", "0.00:00:00")});

            _surveySid = _backendTools.CreateSurvey(script, ProjectId);
            _surveyStateService.Open(_surveySid);
            FusionLibTestTools.UpdateStatePriorityOfNewIts(_surveySid, NewIts, NewItsPriority);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _framework.TestCleanup();
        }

        

        const string ProjectId = "p0046436";
        const int NewIts = 37;
        const int NewItsPriority = 137;
        int _surveySid;

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_SomeCallExistsAnotheInterviewWithoutCall_CallsMoved()
        {
            var interviewIds = new[] { 1, 2, 3 };
            var movableInterviewIds = new[] { 1, 3 };
            const string userName = "u1";
            const string userPassword = "u1";

            _surveyStateService.Open(_surveySid);
            int personId = PersonTools.CreatePerson(userName, userPassword, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, personId);
            BackendTools.LoginPerson(personId, "");

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new[] { interviews[0], interviews[1] }).ToList();

            CallTools.MoveAndRescheduleCalls(_surveySid, movableInterviewIds, NewIts);

            interviews[0].TransientState = NewIts;
            interviews[2].TransientState = NewIts;
            calls[0].Priority = NewItsPriority + 1; //+ increment by 1 (see rule)
            calls.Add(BackendTools.NewCall(interviews[2]));
            calls[2].Priority = NewItsPriority + 1; //call is created with the same priority as priority of ITS (scheduling rules)
            //then priority is increased by script rule.

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_CallsExistAndCustomFiltered_CallsMoved()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new[] { interviews[1] }).ToList();
            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.Equal, "2");

            CallTools.MoveAndRescheduleCalls(_surveySid, filterSid, NewIts);

            interviews[1].TransientState = NewIts;
            calls[0].Priority = NewItsPriority + 1; //+ increment by 1 (see rule)

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)).Where(x => x != null));

            int personSid = PersonTools.CreatePerson("u1", "p1", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, personSid);
            BackendTools.LoginPerson(personSid, "");

            BvTasksEntity task = TaskService.LookupByPersonSid(personSid, _surveySid);
            Assert.AreEqual(task.CallID, calls[0].CallID, "Call was not delivered to person");
        }

        [TestMethod, Owner(@"FIRM\MaximL"), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_CallsExistAndRescheduleWithIncrement5ItsPriority_CallsMovedWithPriority6()
        {
            var interviewIds = new[] { 1, 2 };

            var interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            var calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            CallTools.MoveAndRescheduleCalls(_surveySid, interviewIds, NewIts);

            TestAssert.AreEqual(
                interviews.Select(x => { x.TransientState = NewIts; return x; }),
                interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));

            TestAssert.AreEqual(
                calls.Select(x => { x.Priority = NewItsPriority + 1; return x; }),
                interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)).Where(x => x != null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_CallsDoesntExistAndDefaultFiltered_CallsMoved()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();

            CallTools.MoveAndRescheduleCalls(_surveySid, 0, NewIts);

            TestAssert.AreEqual(
                interviews.Select(x => { x.TransientState = NewIts; return x; }),
                interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));

            TestAssert.AreEqual(
                interviews.Select(BackendTools.NewCall).Select(x => { x.Priority = NewItsPriority + 1; return x; }),
                interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)).Where(x => x != null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Bug(38350), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_MoveAndResheduledCallsWithNegativePhase_CallsDontMovedAndResheduled()
        {
            var interviewIds = new[] { 1, 2, 3 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new[] { interviews[0], interviews[1] }).ToList();

            calls[0].CallState = (int)PhaseState.ProcessedCall;
            CallQueueService.UpdateCall(calls[0], 0);
            calls[1].CallState = (int)PhaseState.PreparedForPredictiveCall;
            CallQueueService.UpdateCall(calls[1], 0);
            interviews[2].TransientState = NewIts;
            calls.Add(BackendTools.NewCall(interviews[2]));
            calls[2].Priority = NewItsPriority + 1;

            CallTools.MoveAndRescheduleCalls(_surveySid, interviewIds, NewIts);

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(38350), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_TwoMoveAndResheduledOneInterviewWithCall_ReschedulingSuccessWithoutCreatingCalls()
        {
            const int firstMoveIts = 31;
            const int firstScheduleIts = 32;
            const int secondMoveIts = 33;
            const int secondScheduleIts = 34;

            var script = new TestScript(
                new[]{
                    new SubRule(
                        new Action(Action.Operation.SetNewITS, firstScheduleIts.ToString(CultureInfo.InvariantCulture)), firstMoveIts, 0, 0, null, false ),
                    new SubRule(
                        new Action(Action.Operation.SetNewITS, secondScheduleIts.ToString(CultureInfo.InvariantCulture)), secondMoveIts, 0, 0, null, false )
                },
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                new Shift(1, 1, "1.00:00:00", "0.00:00:00"));

            var surveyId = _backendTools.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            CallTools.MoveAndRescheduleCalls(surveyId, new[] { interview.ID }, firstMoveIts);

            interview.TransientState = firstScheduleIts;
            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(call.SurveySID, call.InterviewID));

            CallTools.MoveAndRescheduleCalls(surveyId, new[] { interview.ID }, secondMoveIts);

            interview.TransientState = secondScheduleIts;
            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(call.SurveySID, call.InterviewID));
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_RescheduleToAppointmentStatus_AppointmentIsCreated()
        {
            const string contact = "Contact";

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {   Tag = "S1", IsUseDb = false, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1" }}
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var interviewId = context.GetInterview("S1.I1").Id;
            var surveyId = context.GetSurvey("S1").Id;

            CallTools.MoveAndRescheduleCalls(surveyId, new int[] { interviewId }, (int)CallOutcome.Appointment,
                new Appointment
                {
                    time = DateTime.Now,
                    expirationTime = null,
                    contactName = contact
                });

            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 1000);
            var app = AppointmentRepository.GetById(surveyId, interviewId);
            Assert.AreEqual(contact, app.ContactName);
            Assert.AreEqual(1, app.State);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.MoveAndRescheduleCalls)]
        public void MoveAndRescheduleCalls_RescheduleToAppointmentStatus_AppointmentExistsAlready_NewAppointmentIsCreated()
        {
            const string contact = "Contact";

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {   Tag = "S1", IsUseDb = false, SchedulingScript = AllHoursSchedule.Name,
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1" }}
                    }
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var interviewId = context.GetInterview("S1.I1").Id;
            var surveyId = context.GetSurvey("S1").Id;

            CallTools.MoveAndRescheduleCalls(surveyId, new int[] { interviewId }, (int)CallOutcome.Appointment,
                new Appointment
                {
                    time = DateTime.Now,
                    expirationTime = null,
                    contactName = contact
                });

            CallTools.MoveAndRescheduleCalls(surveyId, new int[] { interviewId }, (int)CallOutcome.Appointment,
                new Appointment
                {
                    time = DateTime.Now,
                    expirationTime = null,
                    contactName = contact
                });

            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 1000);

            var apps = BvAppointmentAdapter.GetByCondition(
                "SurveySID = @SurveySID AND InterviewSID = @InterviewID ORDER BY ID",
                new SqlParameter("@SurveySID", surveyId),
                new SqlParameter("@InterviewID", interviewId));

            Assert.AreEqual(2, apps.Count);
            Assert.AreEqual(2, apps.First().State);
            Assert.AreEqual(1, apps.Last().State);
        }
    }
}
