using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class SimultaneousCallDeliveringTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        private void SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(AgentTaskChoiceMode personMode1,
            AgentTaskChoiceMode personMode2,
            int interviewsCount)
        {
            var surveyId = BackendToolsObject.CreateSurvey("p3746537");
            _surveyStateService.Open(surveyId);

            var personId1 = PersonTools.CreatePerson("i1", "password", personMode1);
            var personId2 = PersonTools.CreatePerson("i2", "password", personMode2);

            var interviewIds = Enumerable.Repeat(0, 2).Select( (x, id) =>
                (id < interviewsCount ? BackendTools.CreateInterviewWithCall(surveyId).ID : x)).ToArray();

            BackendTools.AssignCatiPersonToSurvey(surveyId, personId1);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId2);

            BackendTools.LoginPerson(personId1, "");
            if(personMode1 == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId1, surveyId);
            }

            BackendTools.LoginPerson(personId2, "");
            if (personMode2 == AgentTaskChoiceMode.CampaignAssignment)
            {
                PersonService.LoginPersonOnSurveyForSurveySelectionMode(personId2, surveyId);
            }

            BvTasksEntity task1 = null;
            BvTasksEntity task2 = null;

            bool hasExceptionBeenThrown = false;
            Func<int, AgentTaskChoiceMode, BvTasksEntity> getCallAction = (personId, personMode) =>
            {
                try
                {
                    return TaskService.LookupByPersonSid(personId, (personMode == AgentTaskChoiceMode.Automatic ? 0 : surveyId));
                }
                catch (Exception)
                {
                    hasExceptionBeenThrown = true;
                }

                return null;
            };

            var thread1 = new Thread(() => { task1 = getCallAction(personId1, personMode1); });
            var thread2 = new Thread(() => { task2 = getCallAction(personId2, personMode2); });

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Assert.IsFalse(hasExceptionBeenThrown, "exception during call delivering have been thrown");

            var actual1 = new[] {task1 == null ? 0 : task1.InterviewID, task2 == null ? 0 : task2.InterviewID};
            var actual2 = actual1.Reverse().ToArray();
            for (int i = 0; i < interviewIds.Length; ++i)
            {
                //using readpast can be reason of absence of call for some persons
                Assert.IsTrue((interviewIds[i] == actual1[i]) || (actual1[i] == 0) || (interviewIds[i] == actual2[i]) || (actual2[i] == 0));
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BothAutoMode_OneCallLookupBy2Person_CallDeliveringToOnePerson()
        {
            SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(
                AgentTaskChoiceMode.Automatic, AgentTaskChoiceMode.Automatic, 1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BothSurveyAssignmentMode_OneCallLookupBy2Person_CallDeliveringToOnePerson()
        {
            SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(
                AgentTaskChoiceMode.CampaignAssignment, AgentTaskChoiceMode.CampaignAssignment, 1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void DifferentMode_OneCallLookupBy2Person_CallDeliveringToOnePerson()
        {
            SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(
                AgentTaskChoiceMode.Automatic, AgentTaskChoiceMode.CampaignAssignment, 1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BothAutoMode_TwoCallLookupBy2Person_CallDeliveringToEachPerson()
        {
            SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(
                AgentTaskChoiceMode.Automatic, AgentTaskChoiceMode.Automatic, 2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void BothSurveyAssignmentMode_TwoCallLookupBy2Person_CallDeliveringToEachPerson()
        {
            SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(
                AgentTaskChoiceMode.CampaignAssignment, AgentTaskChoiceMode.CampaignAssignment, 2);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void DifferentMode_TwoCallLookupBy2Person_CallDeliveringToEachPerson()
        {
            SimultaneousCallDelivering_OneCallLookupBy2Person_CallDeliveringToOnePerson(
                AgentTaskChoiceMode.Automatic, AgentTaskChoiceMode.CampaignAssignment, 2);
        }
    }
}
