using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class RandomCallDeliveryTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        IEnumerable<BvTasksEntity> GetRandomOrderedCall(int surveyId, IEnumerable<BvInterviewEntity> interviews)
        {
            var calls = BvSvyScheduleAdapter.GetAll();

            Assert.IsTrue(calls.Count(x => x.CallOrder != x.InterviewID) >= calls.Count-1, "order is random. Almost all callorder shoudld differ from interviewid");
            Assert.IsTrue(
                calls.GroupBy(x => x.CallOrder).Count() >= calls.Count-1,
                "order is random. Almost all calls should has different callorder");

            return
                from call in calls.Where(x => surveyId == 0 || x.SurveySID == surveyId)
                from interview in interviews
                where call.SurveySID == interview.SurveySID && call.InterviewID == interview.ID
                orderby call.CallOrder
                select new BvTasksEntity { SurveySID = call.SurveySID, InterviewID = interview.ID };
        }

        IEnumerable<BvTasksEntity> GetOrderedByInterviewCalls(int surveyId, IEnumerable<BvInterviewEntity> interviews)
        {
            var calls = BvSvyScheduleAdapter.GetAll();

            Assert.IsTrue(calls.Count(x => x.CallOrder == x.InterviewID) == calls.Count, "call order should be the same as interview id");

            return
                from call in calls.Where(x => surveyId == 0 || x.SurveySID == surveyId)
                from interview in interviews
                where call.SurveySID == interview.SurveySID && call.InterviewID == interview.ID
                orderby call.CallOrder
                select new BvTasksEntity { SurveySID = call.SurveySID, InterviewID = interview.ID };
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void RandomCallDelivery_FullFillCache_OnlyRightCallsAreInCache()
        {
            var defaultTimeToCall = new DateTime(2000, 03, 20, 22, 22, 22);

            var surveyId1 = BackendToolsObject.CreateSurvey("p0123123");
            _surveyStateService.Open(surveyId1);
            SurveyService.SetCallDeliveryMode(surveyId1, CallDeliveryMode.Random);
            var surveyId2 = BackendToolsObject.CreateSurvey("p0123124");
            _surveyStateService.Open(surveyId2);
            SurveyService.SetCallDeliveryMode(surveyId2, CallDeliveryMode.Random);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);

            var interviews1 = Tools.CreateInterviewWithCalls(surveyId1, Tools.AmountOfCallsPerGroup, defaultTimeToCall).ToArray();
            var interviews2 = Tools.CreateInterviewWithCalls(surveyId2, Tools.AmountOfCallsPerGroup, defaultTimeToCall).ToArray();

            var priorityInterview1 = Tools.CreateInterviewWithCall(surveyId2, 10, defaultTimeToCall.AddHours(10));
            var priorityInterview2 = Tools.CreateInterviewWithCall(surveyId1, 10, defaultTimeToCall.AddHours(10));

            var earlyInterview1 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall.AddHours(-10));
            var earlyInterview2 = Tools.CreateInterviewWithCall(surveyId2, 1, defaultTimeToCall.AddHours(-10));

            Tools.AssignPersonToInterviews(
                surveyId1,
                personId,
                interviews1.Union(new[] { priorityInterview2, earlyInterview1 }).Select(x => x.ID));

            Tools.AssignPersonToInterviews(
                surveyId2,
                personId,
                interviews2.Union(new[] { priorityInterview1, earlyInterview2 }).Select(x => x.ID));

            BackendTools.LoginPerson(personId, "");

            TestAssert.AreEqual(
                GetRandomOrderedCall(0, new[]{priorityInterview1, priorityInterview2}).
                Concat(GetRandomOrderedCall(0, new[] { earlyInterview1, earlyInterview2 })).
                Concat(GetRandomOrderedCall(0, interviews1.Concat(interviews2))),
                Tools.GetAllAccessibleTasks(personId),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void RandomCallDelivery_ChangeRandomEnableFlag_OrderIsChanged()
        {
            var defaultTimeToCall = new DateTime(2000, 03, 20, 22, 22, 22);

            var surveyId1 = BackendToolsObject.CreateSurvey("p0123123");
            _surveyStateService.Open(surveyId1);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);

            var priorityInterview1 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall);
            var priorityInterview2 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall);
            var priorityInterview3 = Tools.CreateInterviewWithCall(surveyId1, 1, defaultTimeToCall);

            SurveyService.SetCallDeliveryMode(surveyId1, CallDeliveryMode.Random);

            Tools.AssignPersonToInterviews(
                surveyId1,
                personId,
                new[] { priorityInterview1.ID, priorityInterview2.ID, priorityInterview3.ID });

            BackendTools.LoginPerson(personId, "");

            TestAssert.AreEqual(
                GetRandomOrderedCall(0, new[] { priorityInterview1, priorityInterview2, priorityInterview3 }),
                Tools.GetAllAccessibleTasks(personId),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
        }

        private void ActivateInterviews_OrderIsCorrect(CallDeliveryMode callDeliveryMode, CallStates mode)
        {
            BackendToolsObject.LaunchAllHoursScript();

            var surveyId1 = BackendToolsObject.CreateSurvey("p0123123");
            _surveyStateService.Open(surveyId1);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.NewInterview(surveyId1);
            var interview2 = BackendTools.NewInterview(surveyId1);
            var interview3 = BackendTools.NewInterview(surveyId1);

            BackendTools.CreateInterview(interview1);
            BackendTools.CreateInterview(interview2);
            BackendTools.CreateInterview(interview3);

            SurveyService.SetCallDeliveryMode(surveyId1, callDeliveryMode);

            BackendTools.AssignCatiPersonsToSurvey(surveyId1, new[] { personId });

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                surveyId1, new[] { interview1.ID, interview2.ID, interview3.ID }, 1, personId, -1, DateTime.FromOADate(0), mode, false);

            BackendTools.LoginPerson(personId, "");

            TestAssert.AreEqual(
                callDeliveryMode == CallDeliveryMode.Random ?
                    GetRandomOrderedCall(0, new[] { interview1, interview2, interview3 }) :
                    GetOrderedByInterviewCalls(0, new[] { interview1, interview2, interview3 }),
                Tools.GetAllAccessibleTasks(personId),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void RandomCallDelivery_ActivateAllInterviewsWithRandomEnabledFlag_OrderIsChanged()
        {
            ActivateInterviews_OrderIsCorrect(CallDeliveryMode.Random, CallStates.All);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void RandomCallDelivery_ActivateSuspendedInterviewsWithRandomEnabledFlag_OrderIsChanged()
        {
            ActivateInterviews_OrderIsCorrect(CallDeliveryMode.Random, CallStates.Suspended);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void OrderByInterviewIdCallDelivery_ActivateAllInterviewsWithRandomEnabledFlag_OrderIsChanged()
        {
            ActivateInterviews_OrderIsCorrect(CallDeliveryMode.InOrder, CallStates.All);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void OrderByInterviewIdCallDelivery_ActivateSuspendedInterviewsWithRandomEnabledFlag_OrderIsChanged()
        {
            ActivateInterviews_OrderIsCorrect(CallDeliveryMode.InOrder, CallStates.Suspended);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void RandomCallDelivery_AddSampleWithRandomEnabledFlag_OrderIsChanged()
        {
            BackendToolsObject.LaunchAllHoursScript();
            const int batchId = 1;
            string projectID;
            var confirmitDb = ConfirmitTools.GetConfirmitSurveyDbOnClass(out projectID);
            var timeZones = Enumerable.Repeat(0, 5).ToArray();
            const int startRespId = 1;
            const int count = 5;

            var surveyId = BackendToolsObject.CreateSurvey(projectID, confirmitDb.ConnectionString);
            _surveyStateService.Open(surveyId);
            SurveyService.SetCallDeliveryMode(surveyId, CallDeliveryMode.Random);

            BackendToolsObject.AddSample(projectID, batchId, (int)SchedulingMode.Simple, startRespId, count, timeZones);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonsToSurvey(surveyId, new[] { personId });
            BackendTools.LoginPerson(personId, "");

            TestAssert.AreEqual(
                GetRandomOrderedCall(0, Enumerable.Range(1, 5).Select(x => new BvInterviewEntity { SurveySID = surveyId, ID = x})),
                Tools.GetAllAccessibleTasks(personId),
                (x, y) => x.InterviewID == y.InterviewID && x.SurveySID == y.SurveySID);
        }
    }
}
