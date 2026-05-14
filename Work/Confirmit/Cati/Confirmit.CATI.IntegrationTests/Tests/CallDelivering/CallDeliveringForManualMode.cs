using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using System.Diagnostics;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.Backend.WebApiServices.Models;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class CallDeliveringForManualMode : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void CallDeliveringForManualMode_InterviewWithCompletedES_IsNotDelivered()
        {
            var surveyId = BackendToolsObject.CreateSurvey("p01234");
            _surveyStateService.Open(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Manual);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            var interview = BackendTools.NewInterview(surveyId);
            interview.TransientState = (int)CallOutcome.Completed;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            BackendTools.LoginPerson(personId, "");

            var calls = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId, personId, new SearchParameter[0]);
            Assert.AreEqual(0, calls.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void CallDeliveringForManualMode_CallWithDifferentAssignments_NecessaryCallsAreDelivered()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey("p01234");
            var surveyId2 = BackendToolsObject.CreateSurvey("p0123446");
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            var groupId = PersonTools.CreatePersonGroup("personGroup");
            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Manual, new[] { groupId }, CallCenterTools.DefaultId);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId1);
            BackendTools.CreateInterviewWithCall(surveyId2);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignResourceToInterview(surveyId1, interview2.ID, personId);
            BackendTools.AssignResourceToInterview(surveyId1, interview3.ID, groupId);

            BackendTools.LoginPerson(personId, "");

            var calls = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId2, personId, new SearchParameter[0]);
            Assert.AreEqual(0, calls.Rows.Count);
            calls = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId1, personId, new SearchParameter[0]);
            TestAssert.AreEqual(
                new[] { interview1.ID, interview2.ID, interview3.ID }.OrderBy(x => x).Select(x => x),
                calls.Select().Select(x => (int)x["interviewid"]).OrderBy(x => x).Select(x => x));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void CallDeliveringForManualMode_CallWasDelivered_CallIsNotDeliveredTwice()
        {
            var surveyId = BackendToolsObject.CreateSurvey("p01234");
            _surveyStateService.Open(surveyId);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Manual);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            var interview = BackendTools.CreateInterviewWithCall(surveyId);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(
                    personId,
                    surveyId,
                    interview.ID);

            Assert.IsNotNull(task, "First call should be delivered");

            task = null;

            try
            {
                task = TaskService.LookupByPersonSid(personId, surveyId, interview.ID);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(ex.ToString());
            }

            Assert.IsNull(task, "Second call should not be delivered");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void CallDeliveringForManualMode_SeveralSurveys_CallsAreDeliveredForAllSurveys()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey("p01234");
            var surveyId2 = BackendToolsObject.CreateSurvey("p012343");
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            var personId = PersonTools.CreatePerson("user", "pass", AgentTaskChoiceMode.Manual);
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId2);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(
                    personId,
                    surveyId1,
                    interview1.ID);

            Assert.IsNotNull(task, "First call should be delivered");

            task = TaskService.LookupByPersonSid(
                    personId,
                    surveyId2,
                    interview2.ID);

            Assert.IsNotNull(task, "First call should be delivered");
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void CallDeliveringForManualMode_SeveralSurveys_OnlyCallsWithCorrespondingDialTypeDelivered()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData
                {
                    IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", DialType = DialType.Landline, Call = new CallData() },
                        new InterviewData { Tag="S1.I2", DialType = DialType.Cellphone, Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.Manual, DialType = DialType.Landline } },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var interview = context.GetInterview("S1.I1");

            BackendTools.LoginPerson(person.Id, "");

            var calls = ConsoleSurveyInterviewsService.GetSurveyInterviews(survey.Id, person.Id, new SearchParameter[0]);

            Assert.AreEqual(1, calls.Rows.Count);
            TestAssert.AreEqual(
                new[] { interview.Id },
                calls.Select().Select(x => (int)x["interviewid"]));
        }

        [TestMethod, Owner(@"FIRM\DmitryS")]
        public void CallDeliveringForManualMode_SeveralSurveys_NoInterviewDeliveredForNotLoggedInPerson()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData
                {
                    IsOpen = true, DialMode = DialingMode.Predictive, Tag="S1", Assigns = new []{"P1"},
                    Interviews = new[]
                    {
                        new InterviewData { Tag="S1.I1", DialType = DialType.Landline, Call = new CallData() },
                        new InterviewData { Tag="S1.I2", DialType = DialType.Landline, Call = new CallData()}
                    }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.Manual, DialType = DialType.Landline } },
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            // Skipping login
            //BackendTools.LoginPerson(person.Id, "");

            var calls = ConsoleSurveyInterviewsService.GetSurveyInterviews(survey.Id, person.Id, new SearchParameter[0]);

            Assert.AreEqual(0, calls.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallDeliveringForManualMode_SeveralAssignedUsersGroupsAndAssignedOnlyCallsListMode_CallsOfGivenPersonAreReturned()
        {
            int person1Id;
            List<BvInterviewEntity> interviews;
            var surveyId = PrepareTestDataWithSeveralUsersGroupAndLogInPerson(out person1Id, out interviews);

            var result = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId, person1Id, new SearchParameter[0]);

            Assert.AreEqual(2, result.Rows.Count);
            TestAssert.AreEqual(
                new[] { interviews[0].ID, interviews[1].ID }.OrderBy(x => x).Select(x => x),
                result.Select().Select(x => (int)x["interviewid"]).OrderBy(x => x).Select(x => x));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallDeliveringForManualMode_SeveralAssignedUsersGroupsAndAllCallsListMode_AllCallsAreReturned()
        {
            int person1Id;
            List<BvInterviewEntity> interviews;
            var surveyId = PrepareTestDataWithSeveralUsersGroupAndLogInPerson(out person1Id, out interviews);

            var result = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId, person1Id, new SearchParameter[0], PersonAssignmentListMode.AllCalls);

            Assert.AreEqual(interviews.Count, result.Rows.Count);
            TestAssert.AreEqual(
                interviews.Select(x => x.ID).OrderBy(x => x).Select(x => x),
                result.Select().Select(x => (int)x["interviewid"]).OrderBy(x => x).Select(x => x));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallDeliveringForManualMode_SingleUserWithAssignedInterviewsAndAssignedOnlyCallsListMode_CallsOfGivenPersonAreReturned()
        {
            int personId;
            List<BvInterviewEntity> interviews;
            var surveyId = PrepareTestDataWitgSingleUserAndLogInPerson(out personId, out interviews);

            var result = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId, personId, new SearchParameter[0]);

            Assert.AreEqual(1, result.Rows.Count);
            Assert.AreEqual(interviews[0].ID, (int)result.Rows[0]["interviewid"]);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void CallDeliveringForManualMode_SingleUserWithAssignedInterviewsAndAllCallsListMode_AllCallsAreReturned()
        {
            int personId;
            List<BvInterviewEntity> interviews;
            var surveyId = PrepareTestDataWitgSingleUserAndLogInPerson(out personId, out interviews);

            var result = ConsoleSurveyInterviewsService.GetSurveyInterviews(surveyId, personId, new SearchParameter[0], PersonAssignmentListMode.AllCalls);

            Assert.AreEqual(interviews.Count, result.Rows.Count);
            TestAssert.AreEqual(
                interviews.Select(x => x.ID).OrderBy(x => x).Select(x => x),
                result.Select().Select(x => (int)x["interviewid"]).OrderBy(x => x).Select(x => x));
        }

        private int PrepareTestDataWitgSingleUserAndLogInPerson(out int personId, out List<BvInterviewEntity> interviews)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p1234");
            _surveyStateService.Open(surveyId);

            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 3, out interviews, out calls);

            personId = PersonTools.CreatePerson("user1", "password", AgentTaskChoiceMode.Manual);

            // assign first interview to the person
            BackendTools.AssignResourceToInterview(surveyId, interviews[0].ID, personId);

            BackendTools.LoginPerson(personId, "");
            return surveyId;
        }

        private int PrepareTestDataWithSeveralUsersGroupAndLogInPerson(out int person1Id, out List<BvInterviewEntity> interviews)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p1234");
            _surveyStateService.Open(surveyId);

            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, 5, out interviews, out calls);

            person1Id = PersonTools.CreatePerson("user1", "password", AgentTaskChoiceMode.Manual);
            var person2Id = PersonTools.CreatePerson("user2", "password", AgentTaskChoiceMode.Manual);
            var groupId = PersonGroupService.CreatePersonGroup("group", string.Empty,
                                                                   new[] { PersonGroupService.RootGroupId });

            // assign first person to survey
            BackendTools.AssignCatiPersonToSurvey(surveyId, person1Id);

            // assign interviews 3 and 4 to second person
            BackendTools.AssignResourceToInterview(surveyId, interviews[2].ID, person2Id);
            BackendTools.AssignResourceToInterview(surveyId, interviews[3].ID, person2Id);

            // assign interview 5 to group
            BackendTools.AssignResourceToInterview(surveyId, interviews[4].ID, groupId);

            // log first person in
            BackendTools.LoginPerson(person1Id, "");
            return surveyId;
        }
    }
}
