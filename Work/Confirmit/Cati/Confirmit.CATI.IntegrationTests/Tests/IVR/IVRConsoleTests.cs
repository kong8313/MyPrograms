using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.SystemSettings.Toggle.Fakes;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Mocks;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.IVR
{
    [TestClass]
    public class IvrConsoleTests : BaseMockedIntegrationTest
    {
        private VoiceXmlServiceController voiceXmlService = null;

        public override void OnPostTestInitialize()
        {
            voiceXmlService = new VoiceXmlServiceController(TestingFramework, new[] { new VoiceXmlPageModel()});

            var stubToggleSettings = TestingFramework.RegistryStub<IToggleSettings, StubIToggleSettings>();
            stubToggleSettings.CatiAgentGet = () => new StubICatiAgentSettings() { IvrThreadGet = () => false };
            stubToggleSettings.EnableIVRGet = () => true;
            stubToggleSettings.EnableDesktopConsoleLoginGet = () => true;
        }

        [TestMethod, Owner(@"FIRM\Grigoryk"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Login_IvrAgentIsNotLogged_LoginToDesktopConsoleIsProhibited_AgentIsLogged()
        {
            var stubToggleSettings = TestingFramework.RegistryStub<IToggleSettings, StubIToggleSettings>();
            stubToggleSettings.CatiAgentGet = () => new StubICatiAgentSettings() { IvrThreadGet = () => false };
            stubToggleSettings.EnableIVRGet = () => true;
            stubToggleSettings.EnableDesktopConsoleLoginGet = () => false;

            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");

            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.SELECTING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.StatusLogout, (int)LoginState.LOGGED_IN);
            Assert.AreEqual(task.LoggedInToDialerState, (int)LoginState.NOT_LOGGED_IN);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Login_IvrAgentIsNotLogged_AgentIsLogged()
        {
            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag="P1", Type = AgentType.IvrAgent} },
                Dialers = new [] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            
            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);
            
            var task = ivrConsoleController.Task;
            
            Assert.AreEqual(task.InterviewState, (int)InterviewState.SELECTING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.StatusLogout, (int) LoginState.LOGGED_IN);
            Assert.AreEqual(task.LoggedInToDialerState, (int)LoginState.NOT_LOGGED_IN);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Login_NotLoggedInIvrAgentTryToLoginIntoDeactivatedDialer_AgentIsNotLoggedInToDialer()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData {
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData {Tag="S1.I1", Call = new CallData()}
                        }
                    }

                },
                Persons = new[] { new PersonData { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            ServiceLocator.Resolve<IDialerAvailabilityManager>().DeactivateDialer(1);

            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.SELECTING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.StatusLogout, (int)LoginState.LOGGED_IN);
            Assert.AreEqual(task.LoggedInToDialerState, (int)LoginState.NOT_LOGGED_IN);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Login_NotLoggedInIvrAgentTryToLoginIntoDeactivatedAndDisconnectedDialer_AgentIsNotLoggedInToDialer()
        {
            var context = new TestData
            {
                Surveys = new []{
                    new SurveyData {
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData {Tag="S1.I1", Call = new CallData()}
                        }
                    }

                },
                Persons = new[] { new PersonData { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");

            ServiceLocator.Resolve<IDialerAvailabilityManager>().DeactivateDialer(1);
            ServiceLocator.Resolve<IDialerAvailabilityManager>().DisableDialer(1);

            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.SELECTING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.StatusLogout, (int)LoginState.LOGGED_IN);
            Assert.AreEqual(task.LoggedInToDialerState, (int)LoginState.NOT_LOGGED_IN);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Interviewing_DialerIsDeactivatedDuringInterview_AgentIsNotLoggedOut()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData {Tag="S1.I1", Call = new CallData()}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData() { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var person = context.GetPerson("P1");

            IvrConsoleController.ExecutePeriodicalWork();
            context.GetDialer("D1").ProcessAllPosponedNotification();

            var ivrConsoleController = new IvrConsoleController(context, person);

            Assert.AreEqual(ivrConsoleController.Task.InterviewID, context.GetInterview("S1.I1").Id, "Ivr agent uses wrong interview id.");

            ServiceLocator.Resolve<IDialerAvailabilityManager>().DeactivateDialer(1);

            IvrConsoleController.ExecutePeriodicalWork();

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.INTERVIEWING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.InterviewID, context.GetInterview("S1.I1").Id);
            Assert.AreEqual(task.SurveySID, context.GetSurvey("S1").Id);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Interviewing_DialerIsDeactivatedAndDisconnectedDuringInterview_AgentIsLoggedOut()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData {Tag="S1.I1", Call = new CallData()}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");

            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);


            Assert.AreEqual(ivrConsoleController.Task.InterviewID, context.GetInterview("S1.I1").Id, "Ivr agent uses wrong interview id.");

            ServiceLocator.Resolve<IDialerAvailabilityManager>().DeactivateDialer(1);
            ServiceLocator.Resolve<IDialerAvailabilityManager>().DisableDialer(1);

            IvrConsoleController.ExecutePeriodicalWork();

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task, null, "Task should not exists");
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Login_OnNotLoggedInLockedIvrAgent_AgentIsNotLogged()
        {
            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag="P1", Type = AgentType.IvrAgent } },
                Dialers = new [] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            person.Model.IsLocked = true;
            PersonRepository.Update(person.Model);

            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);
            
            var task = ivrConsoleController.Task;
            
            Assert.IsNull(task, "Task should not be created");
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Lock_OnLoggedInWithoutCallsIvrAgent_AgentIsLoggedOut()
        {
            var context = new TestData
            {
                Persons = new[] { new PersonData { Tag="P1", Type = AgentType.IvrAgent } },
                Dialers = new [] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");

            IvrConsoleController.ExecutePeriodicalWork();

            ServiceLocator.Resolve<IPersonService>().LockPersonBySupervisor(person.Id);
            
            var ivrConsoleController = new IvrConsoleController(context, person);
            
            var task = ivrConsoleController.Task;
            
            Assert.IsNull(task, "Task should be terminated");
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void Lock_OnLoggedInWorkingIvrAgent_AgentIsLoggedOut()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"},
                        Interviews = new [] {
                            new InterviewData {Tag="S1.I1", Call = new CallData()},
                            new InterviewData {Tag="S1.I2", Call = new CallData()}
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");

            //Login
            IvrConsoleController.ExecutePeriodicalWork();
            //Start interview
            IvrConsoleController.ExecutePeriodicalWork();

            ServiceLocator.Resolve<IPersonService>().LockPersonBySupervisor(person.Id);
            
            var ivrConsoleController = new IvrConsoleController(context, person);
            
            var task = ivrConsoleController.Task;
            
            Assert.IsNull(task, "Task should be terminated");
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void StartInterview_FirstInterviewIsConnected_AgentIsLoggedAndGetFirstInterview()
        {
            var context = new TestData
            {
                Surveys = new[] {
                    new SurveyData
                    { 
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData {Tag="S1.I1", Call = new CallData()},
                            new InterviewData {Tag="S1.I2", Call = new CallData()} 
                        }
                    } 
                },
                Persons = new[] { new PersonData() { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer(("D1"));

            dialer.SetOutcomes(CallOutcome.Connected);
            dialer.SetNotificationReply(ReplyType.Postponed);

            IvrConsoleController.ExecutePeriodicalWork();
            dialer.ProcessAllPosponedNotification();

            var ivrConsoleController = new IvrConsoleController(context, person);

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.INTERVIEWING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.InterviewID, context.GetInterview("S1.I1").Id);
            Assert.AreEqual(task.SurveySID, context.GetSurvey("S1").Id);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void StartInterview_FirstInterviewIsNotConnected_AgentIsLoggedAndGetSecondInterview()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData() },
                            new InterviewData { Tag="S1.I2", Call = new CallData() } 
                        }
                    } 
                },
                Persons = new[] { new PersonData { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer(("D1"));

            dialer.SetNotificationReply(ReplyType.Postponed);
            dialer.SetOutcomes(CallOutcome.NoReply, CallOutcome.Connected);
            
            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);

            var task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.DIALLING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.InterviewID, context.GetInterview("S1.I1").Id);
            Assert.AreEqual(task.SurveySID, context.GetSurvey("S1").Id);

            dialer.ProcessAllPosponedNotification();

            task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.DIALLING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.InterviewID, context.GetInterview("S1.I2").Id);
            Assert.AreEqual(task.SurveySID, context.GetSurvey("S1").Id);

            dialer.ProcessAllPosponedNotification();

            task = ivrConsoleController.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.INTERVIEWING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.InterviewID, context.GetInterview("S1.I2").Id);
            Assert.AreEqual(task.SurveySID, context.GetSurvey("S1").Id);
        }

        [TestMethod, Owner(@"FIRM\KirillV"), TestCategory(TestsCategoriesNames.Ivr)]
        public void StartInterview_ProcessSingleInterviewWith2Pages_InterviewProcessedAndGetNoCall()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData
                    { 
                        Tag="S1", IsOpen = true, Assigns = new []{"P1"}, DialMode = DialingMode.Automatic,
                        Interviews = new [] {
                            new InterviewData { Tag="S1.I1", Call = new CallData() }
                        }
                    } 
                },
                Persons = new[] { new PersonData { Tag = "P1", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData { Tag = "D1" } }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");
            var interview = context.GetInterview("S1.I1");
            dialer.SetOutcomes(CallOutcome.Connected);

            dialer.SetNotificationReply(ReplyType.Postponed);

            IvrConsoleController.ExecutePeriodicalWork();

            var ivrConsoleController = new IvrConsoleController(context, person);
            
            dialer.ProcessAllPosponedNotification();//process Connected call

            Assert.AreEqual(1, voiceXmlService.InitialPageCallCount, "RenderInitialPage wasn't called");
            Assert.AreEqual(0, voiceXmlService.NextPageCallCount, "RenderNextPage should not be called");

            var task = ivrConsoleController.Task;
            
            Assert.AreEqual(task.InterviewState, (int)InterviewState.INTERVIEWING);
            Assert.AreEqual(task.PersonSID, person.Id);
            Assert.AreEqual(task.InterviewID, context.GetInterview("S1.I1").Id);
            Assert.AreEqual(task.SurveySID, context.GetSurvey("S1").Id);

            dialer.ProcessAllPosponedNotification();//process IVR call

            Assert.AreEqual(1, voiceXmlService.InitialPageCallCount, "RenderInitialPage wasn't called");
            Assert.AreEqual(1, voiceXmlService.NextPageCallCount, "RenderNextPage should not be called");

            Assert.AreEqual(ivrConsoleController.Task.InterviewState, (int)InterviewState.NO_CALLS);
        }
    }
}
