using System;
using System.Globalization;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.CallDelivering.CallDeliveringTools;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;

using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Confirmit.CATI.Core.Timezones;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class CorrectnessDeliveringTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        
        /// <summary>
        /// 
        /// Test creates two surveys and assigns them to two interviewers and group.
        /// Test checks that LookupByPersonSID returns right calls. Calls for predictive
        /// survey are not returned
        /// 
        /// create 2 surveys - survey1, survey2.
        /// lets survey2 is predictive.
        /// Add user – i1
        /// Add sample records for survey1 (call 1)
        /// Add sample records for survey2 (call 2)
        /// Assign i1 to survey1, survey2
        /// Set time to now for all calls
        /// Execute LookupByPerson for user twice.
        /// first time call for survey1 should be returned
        /// second call should not be returned
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void CorrectnessDelivering_LookupByPersonSID_PredictiveCallsAreNotReturned()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey("p3746537");
            var surveyId2 = BackendToolsObject.CreateSurvey("p3746538");
            SurveyService.SetDialingMode(surveyId2, DialingMode.Predictive);

            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            var personId = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.NewInterview(surveyId1);
            BackendTools.CreateInterview(interview1);
            var interview2 = BackendTools.NewInterview(surveyId2);
            BackendTools.CreateInterview(interview2);

            var call1 = BackendTools.NewCall(interview1);
            BackendTools.CreateCall(call1);
            var call2 = BackendTools.NewCall(interview2);
            BackendTools.CreateCall(call2);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(call1.SurveySID, task.SurveySID);

            task = TaskService.LookupByPersonSid(personId, 0);

            Assert.IsNull(task);
        }

        private void CorrectnessDelivering_CallsFromDifferentShifts_DeliveredCallsAreInActiveShifts(AgentTaskChoiceMode personMode)
        {
            const int timeZoneId = 11; //GMT + 2

            var personId = PersonTools.CreatePerson("user", "password", personMode);

            TimezoneManager.AddTimezone(timeZoneId);

            var script = new TestScript(
                new Action(Action.Operation.AssignResource, personId.ToString(CultureInfo.InvariantCulture)),
                new Shift(1, 1, new ShiftTimezone(null, "3.10:00:00", "3.10:30:00"),
                                new ShiftTimezone(timeZoneId, "3.12:00:00", "3.13:00:00")),
                new Shift(2, 2, new ShiftTimezone(null, "2.20:00:00", "2.22:00:00")));

            var surveyId = BackendToolsObject.CreateSurvey(script);
            _surveyStateService.Open(surveyId);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            var interview1 = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview1);
            var call1 = BackendTools.NewCall(interview1);
            call1.ShiftID = 2;
            call1.TimeInShift = DateTime.Parse("2010.10.19T19:10:00");
            BackendTools.CreateCall(call1);

            var interview2 = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview2);
            var call2 = BackendTools.NewCall(interview2);
            call2.ShiftID = 1;
            call2.TimeInShift = DateTime.Parse("2010.10.20T09:10:00");
            BackendTools.CreateCall(call2);

            var interview3 = BackendTools.NewInterview(surveyId);
            interview3.TimezoneID = timeZoneId;
            BackendTools.CreateInterview(interview3);
            var call3 = BackendTools.NewCall(interview3);
            call3.ShiftID = 1;
            call3.TimeInShift = DateTime.Parse("2010.10.20T10:10:00");
            BackendTools.CreateCall(call3);

            var interview4 = BackendTools.NewInterview(surveyId);
            interview4.TimezoneID = timeZoneId;
            BackendTools.CreateInterview(interview4);
            var call4 = BackendTools.NewCall(interview4);
            call4.ShiftID = 1;
            call4.TimeInShift = DateTime.Parse("2010.10.20T10:40:00");
            BackendTools.CreateCall(call4);

            BackendTools.LoginPerson(personId, "");

            if(personMode == AgentTaskChoiceMode.CampaignAssignment)
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId, surveyId);

            BackendTools.RunSchedulingProcedure(DateTime.Parse("2010.10.20T10:40:00"));

            var passedSurveyId = (personMode == AgentTaskChoiceMode.CampaignAssignment ? surveyId : 0);

            var task1 = TaskService.LookupByPersonSid(personId, passedSurveyId);
            var task2 = TaskService.LookupByPersonSid(personId, passedSurveyId);
            var task3 = TaskService.LookupByPersonSid(personId, passedSurveyId);

            CollectionAssert.AreEqual(
                new[] { call3.InterviewID, call4.InterviewID },
                new[] { task1.InterviewID, task2.InterviewID });

            Assert.IsNull(task3);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyAssignmentMode_CallsFromDifferentShifts_DeliveredCallsAreInActiveShifts()
        {
            CorrectnessDelivering_CallsFromDifferentShifts_DeliveredCallsAreInActiveShifts(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutoMode_CallsFromDifferentShifts_DeliveredCallsAreInActiveShifts()
        {
            CorrectnessDelivering_CallsFromDifferentShifts_DeliveredCallsAreInActiveShifts(AgentTaskChoiceMode.Automatic);
        }

        private void CorrectnessDelivering_CallWithTimeGreaterThanNow_CallIsNotDelivered(AgentTaskChoiceMode personMode)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p01023");
            _surveyStateService.Open(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", personMode);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            var interview = BackendTools.NewInterview(surveyId);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            call.TimeInShift = DateTime.Parse("2010.10.11T10:10:10");
            BackendTools.CreateCall(call);

            BackendTools.LoginPerson(personId, "");

            BackendTools.RunSchedulingProcedure(DateTime.Parse("2010.10.10T10:10:11"));
            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(DateTime.Parse("2010.10.10T10:10:11")));
            var task = TaskService.LookupByPersonSid(personId, (personMode == AgentTaskChoiceMode.CampaignAssignment ? surveyId : 0));

            Assert.IsNull(task, "Call with time which is greater than now should not be delivered");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SurveyAssignmentMode_CallWithTimeGreaterThanNow_CallIsNotDelivered()
        {
            CorrectnessDelivering_CallWithTimeGreaterThanNow_CallIsNotDelivered(AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutoMode_CallWithTimeGreaterThanNow_CallIsNotDelivered()
        {
            CorrectnessDelivering_CallWithTimeGreaterThanNow_CallIsNotDelivered(AgentTaskChoiceMode.Automatic);
        }
    }
}
