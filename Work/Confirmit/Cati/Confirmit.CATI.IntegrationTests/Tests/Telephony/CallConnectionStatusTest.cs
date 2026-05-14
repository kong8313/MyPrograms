using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class CallConnectionStatusTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\denism")]
        public void StartInterviewForLiveAgent_DropCall_Disconnected()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            console.StartInterview();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.IsNotNull(task.CallID);
            var callId = task.CallID.Value;

            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, person.Id, callId);

            task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual((byte)CallConnectionState.Disconnected, task.CallConnectionState);
        }

        [TestMethod, Owner(@"FIRM\denism")]
        public void StartInterviewForLiveAgent_DropCallAndRedial_Connected()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);

            console.Login();
            console.LoginToDialer();

            var interview = console.StartInterview();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.IsNotNull(task.CallID);
            var callId = task.CallID.Value;

            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, person.Id, callId);

            task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual((byte)CallConnectionState.Disconnected, task.CallConnectionState);
            
            dialer.DialerHelper.AddRequestRedial();
            console.Redial(interview);

            dialer.Helper.SendEventNotifyOutcome(survey.Model.CampaignId,person.Id,callId,CallOutcome.Connected);

            task = TaskRepository.GetByPerson(person.Id);
            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);
        }

        [TestMethod, Owner(@"FIRM\denism")]
        public void StartInterviewForLiveAgent_DropCallForWrongCallId_NothingHappened()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            console.StartInterview();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.IsNotNull(task.CallID);
            var callId = task.CallID.Value;

            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, person.Id, callId + 100);

            task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);
        }

        [TestMethod, Owner(@"FIRM\denism")]
        public void StartInterviewForLiveAgent_HangupCalledThenFinish_CallConnectionCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic,

                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData { Resource = "P1" }}
                    },
                }},
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();

            var interview = console.StartInterview();

            var task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);

            console.Hangup(1);

            task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual((byte)CallConnectionState.Disconnected, task.CallConnectionState);

            console.FinishInterview(interview);

            task = TaskRepository.GetByPerson(person.Id);

            Assert.AreEqual((byte)CallConnectionState.NotDialed, task.CallConnectionState);
        }
    }
}
