using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class SeveralSurveysTests : BaseMockedIntegrationTest
    {
        private const string ProjectId1 = "p006563";
        private const string ProjectId2 = "p00656344";

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SeveralSurveys_OneSurveyOpenedAnotherClosed_DeliveredCallsAreFromOpenedSurvey()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId1);

            BackendTools.CreateInterviewWithCall(surveyId2);
            BackendTools.CreateInterviewWithCall(surveyId1);

            var personId = PersonTools.CreatePerson("user");
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(surveyId1, task.SurveySID, "Delivered calls belong to close survey");

            task = TaskService.LookupByPersonSid(personId, 0);

            Assert.IsNull(task, "Delivered calls belong to close survey");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SeveralSurveys_BothSurveysAreOpen_DeliveredCallsAreFromBothSurveys()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            BackendTools.CreateInterviewWithCall(surveyId2);
            BackendTools.CreateInterviewWithCall(surveyId1);

            var personId = PersonTools.CreatePerson("user");
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            BackendTools.LoginPerson(personId, "");

            var task1 = TaskService.LookupByPersonSid(personId, 0);
            var task2 = TaskService.LookupByPersonSid(personId, 0);

            TestAssert.AreEqual(new[] { surveyId1, surveyId2 }.OrderBy(x=>x).Select(x=>x),
                new[] { task1.SurveySID, task2.SurveySID }.OrderBy(x => x).Select(x => x));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SeveralSurveys_BothSurveysAreOpen_DeliveredMostPriorityCall()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            var interview = BackendTools.NewInterview(surveyId2);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            call.Priority = 1000;
            BackendTools.CreateCall(call);

            BackendTools.CreateInterviewWithCall(surveyId1);

            var personId = PersonTools.CreatePerson("user");
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            BackendTools.LoginPerson(personId, "");

            var task = TaskService.LookupByPersonSid(personId, 0);

            Assert.AreEqual(surveyId2, task.SurveySID, "Call with highiest priority shoul be delivered first");
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SeveralSurveys_InterviewsWithTheSameId_DeliveredBothCalls()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            var interview1 = BackendTools.NewInterview(surveyId2);
            interview1.ID = 1;
            BackendTools.CreateInterview(interview1);
            var call1 = BackendTools.NewCall(interview1);
            BackendTools.CreateCall(call1);

            var interview2 = BackendTools.NewInterview(surveyId1);
            interview2.ID = 1;
            BackendTools.CreateInterview(interview2);
            var call2 = BackendTools.NewCall(interview2);
            BackendTools.CreateCall(call2);

            var personId = PersonTools.CreatePerson("user");
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            BackendTools.LoginPerson(personId, "");

            var task1 = TaskService.LookupByPersonSid(personId, 0);
            var task2 = TaskService.LookupByPersonSid(personId, 0);

            CollectionAssert.AreEquivalent(new[] { surveyId1, surveyId2 }, new[] { task1.SurveySID, task2.SurveySID });
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SeveralSurveys_RunScheduling_CallsInCacheAreUpdatedCorrectly()
        {
            var surveyId1 = BackendToolsObject.CreateSurvey(ProjectId1);
            var surveyId2 = BackendToolsObject.CreateSurvey(ProjectId2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            var interview1 = BackendTools.NewInterview(surveyId2);
            interview1.ID = 1;
            BackendTools.CreateInterview(interview1);
            var call1 = BackendTools.NewCall(interview1);
            call1.Priority = 10;
            BackendTools.CreateCall(call1);

            var interview2 = BackendTools.NewInterview(surveyId1);
            interview2.ID = 1;
            BackendTools.CreateInterview(interview2);
            var call2 = BackendTools.NewCall(interview2);
            BackendTools.CreateCall(call2);

            var interview3 = BackendTools.NewInterview(surveyId1);
            interview3.ID = 2;
            BackendTools.CreateInterview(interview3);
            var call3 = BackendTools.NewCall(interview3);
            BackendTools.CreateCall(call3);

            var personId = PersonTools.CreatePerson("user");
            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            BackendTools.LoginPerson(personId, "");
            
            var task1 = TaskService.LookupByPersonSid(personId, 0);
            var task2 = TaskService.LookupByPersonSid(personId, 0);
            var task3 = TaskService.LookupByPersonSid(personId, 0);

            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, new[] { task1.CallID, task2.CallID, task3.CallID });
        }
    }
}
