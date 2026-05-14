using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Assignment;
using Confirmit.CATI.Supervisor.Core.Persons;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.PersonTests
{
    [TestClass]
    public class ChangeAutomaticSurveyTest : BaseMockedIntegrationTest
    {
        private IChangeAutomaticSurveyService _changeAutomaticSurveyService;
        private IAssignmentManager _assignmentManager;

        [TestInitialize]
        public void Init()
        {
            var registrator = ServiceLocator.Resolve<IServiceRegistrator>();
            registrator.Register<IChangeAutomaticSurveyService, ChangeAutomaticSurveyService>();

            _changeAutomaticSurveyService = ServiceLocator.Resolve<IChangeAutomaticSurveyService>();
            _assignmentManager = ServiceLocator.Resolve<IAssignmentManager>();
        }

        [TestMethod, Owner(@"FIRM\VictorR"), TestCategory(TestsCategoriesNames.SurveySwitching)]
        public void SetAutomaticSurveySeamless_LoggedInterviewer_ChangedSeamless()
        {
            // arrange
            var context = CreateContext(TaskChoiceMode.SurveyAssignment);

            var survey = context.GetSurvey("S1");
            var newSurvey = context.GetSurvey("S2");

            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var task = TaskRepository.GetByPerson(person.Id);
            Assert.IsTrue(task.NewSurveySID == 0);

            // act
            var result = _changeAutomaticSurveyService.ChangeSeamless(person.Id, newSurvey.Id);
            task = TaskRepository.GetByPerson(person.Id);
            var updatedPerson = PersonRepository.GetById(person.Id);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(task.SurveySID == survey.Id);
            Assert.IsTrue(task.NewSurveySID == newSurvey.Id);
            Assert.IsTrue(updatedPerson.AutomaticSurveyID == newSurvey.Id);
            Assert.IsTrue(_assignmentManager.IsPersonOrGroupAssigned(newSurvey.Id, updatedPerson.SID));
        }

        [TestMethod, Owner(@"FIRM\VictorR"), TestCategory(TestsCategoriesNames.SurveySwitching)]
        public void SetAutomaticSurveySeamless_NotLoggedInterviewer_ChangedNotSeamless()
        {
            // arrange
            var context = CreateContext(TaskChoiceMode.SurveyAssignment);
            var newSurvey = context.GetSurvey("S2");
            var person = context.GetPerson("P1");

            // act
            var result = _changeAutomaticSurveyService.ChangeSeamless(person.Id, newSurvey.Id);

            // assert
            var updatedPerson = PersonRepository.GetById(person.Id);

            Assert.IsFalse(result);
            Assert.IsTrue(updatedPerson.AutomaticSurveyID == newSurvey.Id);
            Assert.IsTrue(_assignmentManager.IsPersonOrGroupAssigned(newSurvey.Id, updatedPerson.SID));
        }

        [TestMethod, Owner(@"FIRM\VictorR"), TestCategory(TestsCategoriesNames.SurveySwitching)]
        public void SetAutomaticSurveySeamless_InterviewerModAutomatic_NotChanged()
        {
            // arrange
            var context = CreateContext(TaskChoiceMode.Automatic);
            var survey2 = context.GetSurvey("S2");
            var person = context.GetPerson("P1");

            // act
            var result = _changeAutomaticSurveyService.ChangeSeamless(person.Id, survey2.Id);

            // assert
            var updatedPerson = PersonRepository.GetById(person.Id);

            Assert.IsFalse(result);
            Assert.IsFalse(updatedPerson.AutomaticSurveyID == survey2.Id);
            Assert.IsFalse(_assignmentManager.IsPersonOrGroupAssigned(survey2.Id, updatedPerson.SID));
        }

        [TestMethod, Owner(@"FIRM\VictorR"), TestCategory(TestsCategoriesNames.SurveySwitching)]
        public void SetAutomaticSurveySeamless_AssignmentTheSameSurvey_NotSetNewSurveySID()
        {
            // arrange
            var context = CreateContext(TaskChoiceMode.SurveyAssignment);
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var task = TaskRepository.GetByPerson(person.Id);
            Assert.IsTrue(task.NewSurveySID == 0);

            // act
            var result = _changeAutomaticSurveyService.ChangeSeamless(person.Id, survey.Id);
            task = TaskRepository.GetByPerson(person.Id);

            // assert
            Assert.IsFalse(result);
            Assert.IsTrue(task.SurveySID == survey.Id);
            Assert.IsTrue(task.NewSurveySID == 0);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SurveySwitching)]
        public void SwitchToPredictiveSurvey_InterviewerWasOnBreakDuringSwitch_SwitchingSuccessful()
        {
            PredictiveConsoleController console;

            var context = LoginTwoPersonsToDifferentPredictiveSurveys(out console);
            var survey = context.GetSurvey("S2");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var personWS = new CatiWsHelper(person.Data.Name, person.Data.Password);

            dialer.DialerHelper.AddRequestGoNotReady(()=>{});

            personWS.ConsoleService.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            Assert.AreEqual(context.GetSurvey("S1").Id, TaskRepository.GetByPerson(person.Id).SurveySID, "Original survey is not correct");

            _changeAutomaticSurveyService.ChangeSeamless(person.Id, survey.Id);

            dialer.DialerHelper.SetBehaviorForSetCampaign(args => (int) DialerErrorCode.Success);

            dialer.DialerHelper.AddRequestGoReady();

            personWS.ConsoleService.ContinueWorkAfterBreak(1);
            Assert.AreEqual(survey.Id, TaskRepository.GetByPerson(person.Id).SurveySID, "Survey was not switched");

            var requestedCalls = dialer.RequestCalls(survey, 10, CallsSelectionAlgorithm.ByCampaign);

            var interview = console.WaitInterview(requestedCalls, requestedCalls.CallList[0]);

            Assert.IsNotNull(interview);
        }


        private TestDataContext LoginTwoPersonsToDifferentPredictiveSurveys(out PredictiveConsoleController console)
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, Assigns = new [] {"P1", "P2"}
                    },
                   new SurveyData(){ Tag = "S2", IsOpen  = true, DialMode = DialingMode.Predictive,
                        Interviews = new []
                        {
                            new InterviewData() {Tag = "S2.I1", Call = new CallData()},
                            new InterviewData() {Tag = "S2.I2", Call = new CallData()},
                            new InterviewData() {Tag = "S2.I3", Call = new CallData()},
                            new InterviewData() {Tag = "S2.I4", Call = new CallData()}
                        },

                       Assigns = new [] {"P1", "P2"} },
                },
                Persons = new[]
                {
                    new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData() { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment } 
                },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();


            var survey1 = context.GetSurvey("S1");
            var survey2 = context.GetSurvey("S2");
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var dialer = context.GetDialer("D1");

            var console1 = new PredictiveConsoleController(context, person1, survey1, dialer);

            console1.Login();
            console1.LoginToDialer();

            var console2 = new PredictiveConsoleController(context, person2, survey2, dialer);

            console2.Login();
            console2.LoginToDialer();

            console = console1;
            return context;
        }

        private TestDataContext CreateContext(TaskChoiceMode taskChoiceMode)
        {
            return new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = false, IsOpen = true,
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Call = new CallData()}
                        },
                        Assigns = new[]{"P1"}},

                        new SurveyData() { Tag="S2", IsUseDb = false, IsOpen = true,Interviews = new[]
                        {
                            new InterviewData(){ Tag="S2.I1", Call = new CallData()}
                        }}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = taskChoiceMode } }
            }.Create();
        }

    }
}
