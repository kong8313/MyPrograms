using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class ManualTaskChoiceSwitchSurveyTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void FinishInterview_IsReloginNeeded_PendingLogoutWasSet()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Preview, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S1.I1", Call = new CallData()},
                            new InterviewData() { Tag="S1.I2", Call = new CallData()}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } },
                Dialers = new[] { new DialerData() { Tag = "D1", } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            dialer.Behavior.Methods.IsReloginNeededOnSurveyChange.Init(true);

            var console = new ManualModeConsoleController(context, person, dialer);

            var interview = console.LoginAndStart(context.GetInterview("S1.I1"));

            Assert.AreEqual(interview.Tag, "S1.I1");

            console.FinishInterview(interview);

            console.Check((state) =>
            {
                Assert.AreEqual((int)LoginState.LOGGED_IN, state.interviewerLoginToDialerState);
                Assert.AreEqual((int)LoginState.PENDING_LOGOUT, state.interviewerLoginState);
                Assert.AreEqual((int)InterviewState.SELECTING, state.interviewState);
            });
        }

        [TestMethod]
        public void FinishInterview_IsReloginNotNeeded_PendingLogoutWasnotSet()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Preview, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S1.I1", Call = new CallData()},
                            new InterviewData() { Tag="S1.I2", Call = new CallData()}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new ManualModeConsoleController(context, person, dialer);

            var interview = console.LoginAndStart(context.GetInterview("S1.I1"));

            Assert.AreEqual(interview.Tag, "S1.I1");

            console.FinishInterview(interview);

            console.Check((state) =>
            {
                Assert.AreEqual((int)LoginState.LOGGED_IN, state.interviewerLoginToDialerState);
                Assert.AreEqual((int)LoginState.LOGGED_IN, state.interviewerLoginState);
                Assert.AreEqual((int)InterviewState.SELECTING, state.interviewState);
            });
        }

        [TestMethod]
        public void StartInterview_PreviosInterviewFromTheSameSurvey_SetCampaignIsCalled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Automatic, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S1.I1", Call = new CallData()},
                            new InterviewData() { Tag="S1.I2", Call = new CallData()}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            var console = new ManualModeConsoleController(context, person, dialer);

            var interview = console.LoginAndStart(context.GetInterview("S1.I1"));
            console.FinishInterview(interview);

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");
            Assert.AreEqual(1, completeCallParams.Count, "Wrong count of SetCampaign calls");
            Assert.AreEqual(1, setCampaignParams.Count, "Wrong count of CompleteCall calls");

            console.StartInterview(context.GetInterview("S1.I2"));

            Assert.AreEqual(2, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");
            Assert.AreEqual(1, completeCallParams.Count, "Wrong count of SetCampaign calls");
            Assert.AreEqual(1, setCampaignParams.Count, "Wrong count of CompleteCall calls");
        }

        [TestMethod]
        public void StartInterview_PreviosInterviewFromTheOtherSurveyWithManualDialingMode_SetCampaignIsNotCalled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Automatic, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S1.I1", Call = new CallData()}
                    }},
                    new SurveyData(){ Tag="S2", DialMode = DialingMode.Manual, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S2.I1", Call = new CallData()}
                    }}
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            var console = new ManualModeConsoleController(context, person, dialer);

            var interview = console.LoginAndStart(context.GetInterview("S1.I1"));
            console.FinishInterview(interview);

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");
            Assert.AreEqual(1, completeCallParams.Count, "Wrong count of SetCampaign calls");
            Assert.AreEqual(1, setCampaignParams.Count, "Wrong count of CompleteCall calls");

            console.StartInterview(context.GetInterview("S2.I1"));

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");
            Assert.AreEqual(1, completeCallParams.Count, "Wrong count of SetCampaign calls");
            Assert.AreEqual(1, setCampaignParams.Count, "Wrong count of CompleteCall calls");
        }

        [TestMethod]
        public void StartInterview_PreviosInterviewFromTheOtherSurveyWithAutomaticDialingMode_SetCampaignIsCalled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag="S1", DialMode = DialingMode.Automatic, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S1.I1", Call = new CallData()}
                    }},
                    new SurveyData(){ Tag="S2", DialMode = DialingMode.Automatic, AssignsS = "P1",
                        Interviews = new [] {
                            new InterviewData() { Tag="S2.I1", Call = new CallData()}
                    }}
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            var console = new ManualModeConsoleController(context, person, dialer);

            var interview = console.LoginAndStart(context.GetInterview("S1.I1"));
            console.FinishInterview(interview);

            Assert.AreEqual(1, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");
            Assert.AreEqual(1, completeCallParams.Count, "Wrong count of SetCampaign calls");
            Assert.AreEqual(1, setCampaignParams.Count, "Wrong count of CompleteCall calls");

            console.StartInterview(context.GetInterview("S2.I1"));

            Assert.AreEqual(2, sendNumberToAgentParams.Count, "Wrong count of SendNumberToAgent calls");
            Assert.AreEqual(1, completeCallParams.Count, "Wrong count of SetCampaign calls");
            Assert.AreEqual(3, setCampaignParams.Count, "Wrong count of CompleteCall calls");
        }
    }
}
