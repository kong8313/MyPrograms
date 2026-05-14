using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DialType = Confirmit.CATI.Common.DialType;

namespace Confirmit.CATI.IntegrationTests.Tests.CATI
{
    [TestClass]
    public class TcpaTests : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void AutomaticSurvey_TwoPersonsDifferentDialTypes_TwoDialersWithDifferentDialTypes_InterviewersLoggedInToDialersBasedOnDialType()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){
                        Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic, Assigns = new [] {"P1.A","P2.M"},
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I3", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I4", DialType = DialType.Cellphone, Call = new CallData() }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1.A", Name = "P1.A", DialType = DialType.Landline},
                    new PersonData() {Tag = "P2.M", Name = "P2.M", DialType = DialType.Cellphone}
                },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1.A", DialType = DialType.Landline},
                    new DialerData(){ Tag = "D2.M", DialType = DialType.Cellphone}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var personA = context.GetPerson("P1.A");
            var personM = context.GetPerson("P2.M");
            var dialerA = context.GetDialer("D1.A");
            var dialerM = context.GetDialer("D2.M");

            var usersLoggedToAutomaticDialers = new List<string>();
            var usersLoggedToManualDialers = new List<string>();

            var consoleA = new AutomaticConsoleController(context, personA, survey);
            var consoleM = new AutomaticConsoleController(context, personM, survey);

            dialerA.DialerHelper.SetBehaviorForLogin((args)=> {
                usersLoggedToAutomaticDialers.Add(args.AgentName);
                return 0;
            });

            dialerM.DialerHelper.SetBehaviorForLogin((args) => {
                usersLoggedToManualDialers.Add(args.AgentName);
                return 0;
            });

            consoleA.Login();
            consoleA.LoginToDialer();

            consoleM.Login();
            consoleM.LoginToDialer();

            CollectionAssert.AreEqual(new[] {"P1.A"}, usersLoggedToAutomaticDialers);
            CollectionAssert.AreEqual(new[] { "P2.M" }, usersLoggedToManualDialers);

            var dialerCollection = ServiceLocator.Resolve<IDialerCollection>();
            
            Assert.AreEqual(DialType.Landline, dialerCollection.GetDialerById(dialerA.Id).DialType);
            Assert.AreEqual(DialType.Cellphone, dialerCollection.GetDialerById(dialerM.Id).DialType);
        }

        [TestMethod]
        public void AutomaticSurvey_TwoPersonsDifferentDialTypes_TwoDialersWithDifferentDialTypes_CellphoneCallsProcessedInPreviewModeWithoutDial()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){
                        Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic, Assigns = new [] {"P1.A","P2.M"},
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I3", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I4", DialType = DialType.Cellphone, Call = new CallData() }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1.A", Name = "P1.A", DialType = DialType.Landline, TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2.M", Name = "P2.M", DialType = DialType.Cellphone, TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1.A", DialType = DialType.Landline},
                    new DialerData(){ Tag = "D2.M", DialType = DialType.Cellphone}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var personA = context.GetPerson("P1.A");
            var personM = context.GetPerson("P2.M");
            var dialerA = context.GetDialer("D1.A");
            var dialerM = context.GetDialer("D2.M");

            var automaticSentInterviews = new List<int>();
            var manualSentInterviews = new List<int>();

            var consoleA = new AutomaticConsoleController(context, personA, survey);
            var consoleM = new AutomaticConsoleController(context, personM, survey);

            dialerA.DialerHelper.SetBehaviorForSendNumberToAgent((args) =>
            {
                automaticSentInterviews.Add(args.InterviewId);
                return CallOutcome.Connected;
            });

            dialerM.DialerHelper.SetBehaviorForSendNumberToAgent((args) =>
            {
                manualSentInterviews.Add(args.InterviewId);
                return CallOutcome.Connected;
            });

            consoleA.Login();
            consoleA.LoginToDialer();
            var actualAutomaticInterviews = consoleA.ProcessAllInterviews();

            consoleM.Login();
            consoleM.LoginToDialer();

            var actualManualInterviews = consoleM.ProcessAllInterviews();

            CollectionAssert.AreEqual(context.GetInterviews("S1.I1", "S1.I3").Select(x => x.Id).ToArray(), automaticSentInterviews.ToArray());
            CollectionAssert.AreEqual(new int[]{}, manualSentInterviews.ToArray());

            CollectionAssert.AreEqual(context.GetInterviews("S1.I1", "S1.I3").Select(x => x.Id).ToArray(), actualAutomaticInterviews.Select(x => x.Id).ToArray());
            CollectionAssert.AreEqual(context.GetInterviews("S1.I2", "S1.I4").Select(x => x.Id).ToArray(), actualManualInterviews.Select(x => x.Id).ToArray());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredicitveSurvey_LoginForLandlineMode_SetGroupsIsCalled()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){ Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() }
                    }}},
                Persons = new[] { new PersonData() { Tag = "P1", DialType = DialType.Landline, TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var dialer = context.GetDialer("D1").Predictive("S1").Auto();
            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start().Wait().Check(interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.Login.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.SetGroups.History.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredicitveSurvey_LoginForCellphoneMode_SetGroupsIsnotCalled()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){ Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new []{"P1"},
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() }
                    }}},
                Persons = new[] {new PersonData() { Tag="P1", DialType = DialType.Cellphone, TaskChoice = TaskChoiceMode.SurveyAssignment} },
                Dialers = new[] {new DialerData() { Tag="D1" } }
            }.Create();

            var dialer = context.GetDialer("D1");
            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start().Wait().Check(interviewTag:"S1.I2");

            Assert.AreEqual(1, dialer.Behavior.Methods.Login.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.SetGroups.History.Count);
        }

        [TestMethod]
        public void AutomaticSurvey_TwoPersonsDifferentDialTypes_TwoDialersWithDifferentDialTypes_CellphoneCallsProcessedInPreviewModeWithDial()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){
                        Tag="S1", IsOpen = true, DialMode = DialingMode.Automatic, Assigns = new [] {"P1.A","P2.M"},
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I3", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I4", DialType = DialType.Cellphone, Call = new CallData() }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1.A", Name = "P1.A", DialType = DialType.Landline, TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2.M", Name = "P2.M", DialType = DialType.Cellphone, TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1.A", DialType = DialType.Landline},
                    new DialerData(){ Tag = "D2.M", DialType = DialType.Cellphone}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var personA = context.GetPerson("P1.A");
            var personM = context.GetPerson("P2.M");
            var dialerA = context.GetDialer("D1.A");
            var dialerM = context.GetDialer("D2.M");

            var automaticSentInterviews = new List<int>();
            var manualSentInterviews = new List<int>();

            var consoleA = new AutomaticConsoleController(context, personA, survey);
            var consoleM = new AutomaticConsoleController(context, personM, survey);

            dialerA.DialerHelper.SetBehaviorForSendNumberToAgent((args) =>
            {
                automaticSentInterviews.Add(args.InterviewId);
                return CallOutcome.Connected;
            });

            dialerM.DialerHelper.SetBehaviorForSendNumberToAgent((args) =>
            {
                manualSentInterviews.Add(args.InterviewId);
                return CallOutcome.Connected;
            });

            consoleA.Login();
            consoleA.LoginToDialer();
            var actualAutomaticInterviews = consoleA.ProcessAllInterviews();

            consoleM.Login();
            consoleM.LoginToDialer();

            var actualManualInterviews = consoleM.ProcessAllInterviewsWithPreviewDial();

            CollectionAssert.AreEqual(context.GetInterviews("S1.I1", "S1.I3").Select(x => x.Id).ToArray(), automaticSentInterviews.ToArray());
            CollectionAssert.AreEqual(context.GetInterviews("S1.I2", "S1.I4").Select(x => x.Id).ToArray(), manualSentInterviews.ToArray());

            CollectionAssert.AreEqual(context.GetInterviews("S1.I1", "S1.I3").Select(x => x.Id).ToArray(), actualAutomaticInterviews.Select(x => x.Id).ToArray());
            CollectionAssert.AreEqual(context.GetInterviews("S1.I2", "S1.I4").Select(x => x.Id).ToArray(), actualManualInterviews.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void PredictiveSurvey_TwoPersonsDifferentDialTypes_TwoDialersWithDifferentDialTypes_CellphoneCallsProcessedInPreviewModeWithDial()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){
                        Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new [] {"P1.A","P2.M"},
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I3", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I4", DialType = DialType.Cellphone, Call = new CallData() }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1.A", Name = "P1.A", DialType = DialType.Landline, TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2.M", Name = "P2.M", DialType = DialType.Cellphone, TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1.A", DialType = DialType.Landline},
                    new DialerData(){ Tag = "D2.M", DialType = DialType.Cellphone}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var personA = context.GetPerson("P1.A");
            var personM = context.GetPerson("P2.M");
            var dialerA = context.GetDialer("D1.A");
            var dialerM = context.GetDialer("D2.M");

            var consoleA = new PredictiveConsoleController(context, personA, survey, dialerA);
            var consoleM = new AutomaticConsoleController(context, personM, survey);

            var manualSentInterviews = dialerM.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected).Select(x => x.InterviewId);

            consoleA.Login();
            consoleA.LoginToDialer();
            var actualPredicitveInterviews = consoleA.ProcessAllInterviews();

            consoleM.Login();
            consoleM.LoginToDialer();

            var actualManualInterviews = consoleM.ProcessAllInterviewsWithPreviewDial();

            CollectionAssert.AreEqual(context.GetInterviews("S1.I2", "S1.I4").Select(x => x.Id).ToArray(), manualSentInterviews.ToArray());

            CollectionAssert.AreEqual(context.GetInterviews("S1.I1", "S1.I3").Select(x => x.Id).ToArray(), actualPredicitveInterviews.Select(x => x.Id).ToArray());
            CollectionAssert.AreEqual(context.GetInterviews("S1.I2", "S1.I4").Select(x => x.Id).ToArray(), actualManualInterviews.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void PredictiveSurvey_TwoPersonsDifferentDialTypes_TwoDialersWithDifferentDialTypes_AutomaticPersonTerminateWorkCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){
                        Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new [] {"P1.A","P2.M"},
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I3", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I4", DialType = DialType.Cellphone, Call = new CallData() }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1.A", Name = "P1.A", DialType = DialType.Landline, TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2.M", Name = "P2.M", DialType = DialType.Cellphone, TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1.A", DialType = DialType.Landline},
                    new DialerData(){ Tag = "D2.M", DialType = DialType.Cellphone}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1.A");
            var dialer = context.GetDialer("D1.A");

            var console = new PredictiveConsoleController(context, person, survey, dialer);

            var requestCalls = console.LoginAndStart();
            console.WaitInterview(requestCalls, requestCalls.CallList.First());

            TestDialerHelper.CompleteCallParams completeCallParams = null;
            TestDialerHelper.LogoutParams logoutParams = null;
            TestDialerHelper.KillAgentParams killAgentParams = null;

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) => {
                completeCallParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForLogout((args) => {
                logoutParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForKillAgent((args) =>
            {
                killAgentParams = args;
                return 0;
            });

            new SupervisorService().TerminateTaskByPerson(person.Id, null);
            
            Assert.IsNotNull(killAgentParams);
            Assert.IsNull(completeCallParams);
            Assert.IsNull(logoutParams);
        }

        [TestMethod]
        public void PredictiveSurvey_TwoPersonsDifferentDialTypes_TwoDialersWithDifferentDialTypes_ManualPersonTerminateWorkCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData(){
                        Tag="S1", IsOpen = true, DialMode = DialingMode.Predictive, Assigns = new [] {"P1.A","P2.M"},
                        Interviews = new[]
                        {
                            new InterviewData() { Tag = "S1.I1", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I2", DialType = DialType.Cellphone, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I3", DialType = DialType.Landline, Call = new CallData() },
                            new InterviewData() { Tag = "S1.I4", DialType = DialType.Cellphone, Call = new CallData() }
                        }
                    }
                },
                Persons = new[]
                {
                    new PersonData() {Tag = "P1.A", Name = "P1.A", DialType = DialType.Landline, TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData() {Tag = "P2.M", Name = "P2.M", DialType = DialType.Cellphone, TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1.A", DialType = DialType.Landline},
                    new DialerData(){ Tag = "D2.M", DialType = DialType.Cellphone}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P2.M");
            var dialer = context.GetDialer("D2.M");

            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            console.LoginToDialer();
            console.StartInterview();

            TestDialerHelper.CompleteCallParams completeCallParams = null;
            TestDialerHelper.LogoutParams logoutParams = null;
            TestDialerHelper.KillAgentParams killAgentParams = null;

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) => {
                completeCallParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForLogout((args) => {
                logoutParams = args;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForKillAgent((args) =>
            {
                killAgentParams = args;
                return 0;
            });

            new SupervisorService().TerminateTaskByPerson(person.Id, null);

            Assert.IsNull(killAgentParams);
            Assert.IsNotNull(completeCallParams);
            Assert.IsNotNull(logoutParams);
            Assert.AreEqual(false, logoutParams.IsPredicitve);
        }
    }
}
