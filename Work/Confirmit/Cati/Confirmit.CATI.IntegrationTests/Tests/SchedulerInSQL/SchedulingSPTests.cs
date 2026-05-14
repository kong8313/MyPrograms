using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Confirmit.Test.Common.Attributes;
using System.Threading;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.SchedulerInSQL
{
    [TestClass]
    public class SchedulingSPTests : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;
        private ICallQueueService _callQueueService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
            _callQueueService = ServiceLocator.Resolve<ICallQueueService>();
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderL"), Bug(39214)]
        public void SchedulingSPTests_RunSchedulingForExpiredCallWhichIsInInterview_CallIsNotDeleted()
        {
            const string projectId = "p000111";
            const int secondsBeforeNowTime = 10;

            BackendToolsObject.LaunchAllHoursScript();
            int surveySid = BackendToolsObject.CreateSurvey(projectId);
            _surveyStateService.Open(surveySid);

            int personSid = PersonTools.CreatePerson("u1", "p1", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveySid, personSid);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);
            interview.TransientState = 57;
            var call = BackendTools.NewCall(interview);
            call.TimeToExpire = DateTime.UtcNow.AddSeconds(secondsBeforeNowTime);
            BackendTools.CreateCall(call);

            BackendTools.LoginPerson(personSid, "");
            var task = TaskService.LookupByPersonSid(personSid, surveySid);
            Assert.IsNotNull(task, "Call was not delivered to person");

            var scheduleEntity = BvSvyScheduleAdapter.GetAll()[0];
            Assert.AreEqual(-1, scheduleEntity.CallState, "For interviewing call phase should be -1");

            Thread.Sleep(secondsBeforeNowTime*1000);

            _callQueueService.ScheduleAndRemoveDeletedCalls();

            var scheduleEntityList = BvSvyScheduleAdapter.GetAll();
            Assert.AreEqual(1, scheduleEntityList.Count, "Call should not be deleted as expired call if phase = -1");
            Assert.AreEqual(-1, scheduleEntityList[0].CallState, "For interviewing call phase should be -1");
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void DeleteCallsWithCallStateEQZero()
        {
            CreateSurveyWithTwoCallsOneCallWithCallStateEQZero();
            var calls = BvSvyScheduleAdapter.GetAll();

            Assert.AreEqual(2, calls.Count);

            _callQueueService.ScheduleAndRemoveDeletedCalls();
            calls = BvSvyScheduleAdapter.GetAll();

            Assert.AreEqual(1, calls.Count);
            Assert.AreEqual(2,calls[0].CallState, "The only call should be with CallState=2");
        }
        
        private void CreateSurveyWithTwoCallsOneCallWithCallStateEQZero()
        {
            const string projectId = "p01011234";

            BackendToolsObject.LaunchAllHoursScript();
            int surveySid = BackendToolsObject.CreateSurvey(projectId);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);
            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);
            call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            call.CallState = 0;
            CallQueueService.UpdateCall(call,0);
            
        }
    }
}
