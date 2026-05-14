using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.LinkedInterviews;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.Test.Common.Attributes;

namespace Confirmit.CATI.IntegrationTests.Tests.LoginTests
{
    [TestClass]
    public class AutomaticPersonLoginTest : BaseMockedIntegrationTest
    {
        private int CheckActualIdInExpectedListOfIdAndReturnIt(int actualId, IEnumerable<int> expectedIds)
        {
            var result = expectedIds.FirstOrDefault(x => x == actualId);
            Assert.IsTrue(result != 0);

            return result;
        }

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        /// <summary>
        /// Create survey, person in automatic mode, some interviews and calls. 
        /// Assign person to survey. Login, startInterview. Create second survey 
        /// and assign person on it. Check that calls are delivered from both surveys.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_SurveySidInBvTasksDoesntSpoil_CallsDeliveredCorrectly()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            _surveyStateService.Open(surveyId1);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview("", 0);

            var task1 = TaskRepository.GetByPerson(personId);
            var firstInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task1.InterviewID, new[] { interview1.ID, interview2.ID });

            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId2);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var interview3 = BackendTools.CreateInterviewWithCall(surveyId2);
            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.WrapUp(firstInterviewId, 1);

            var task2 = TaskRepository.GetByPerson(personId);
            var secondInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task2.InterviewID,
                new[] { interview1.ID, interview2.ID, interview3.ID });

            ws.ConsoleService.WrapUp(secondInterviewId, 1);

            var task3 = TaskRepository.GetByPerson(personId);
            CheckActualIdInExpectedListOfIdAndReturnIt(task3.InterviewID,
                new[] { interview1.ID, interview2.ID, interview3.ID });

            var deliveredSurveys = new[] { task1.SurveySID, task2.SurveySID, task3.SurveySID };
            Assert.IsTrue(deliveredSurveys.Contains(surveyId1));
            Assert.IsTrue(deliveredSurveys.Contains(surveyId2));
        }

        /// <summary>
        /// Create 2 surveys, person in choise mode, some interviews and calls. 
        /// Assign person to surveys. Login, choose automatic mode. Choose that 
        /// calls are delivered from both survyes.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_SelectAutomaticModeDuringChoiseMode_CallsDeliveredCorrectly()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Choice);
            PersonService.UpdatePersonMode(PersonRepository.GetById(personId),
                AgentTaskChoiceMode.Choice,
                TaskChoicePermissions.Automatic, 
                true);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId2);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.Automatic);

            ws.ConsoleService.StartInterview("", 0);

            var task1 = TaskRepository.GetByPerson(personId);
            var firstInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task1.InterviewID,
                new[] { interview1.ID, interview2.ID });

            ws.ConsoleService.WrapUp(task1.InterviewID, 1);

            var task2 = TaskRepository.GetByPerson(personId);
            CheckActualIdInExpectedListOfIdAndReturnIt(task2.InterviewID,
                new[] { interview1.ID, interview2.ID });

            var deliveredSurveys = new[] { task1.SurveySID, task2.SurveySID };
            Assert.IsTrue(deliveredSurveys.Contains(surveyId1));
            Assert.IsTrue(deliveredSurveys.Contains(surveyId2));
        }

        /// <summary>
        /// Create 2 surveys, person in automatic mode, some interviews and calls. 
        /// Assign person to surveys. Login, start interview. Perform Scheduling. 
        /// Check that calls for both surveys are in cache.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_DuringInterviewPerformScheduling_SChedulingIsCorrect()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.NewInterview(surveyId1);
            BackendTools.CreateInterview(interview1);

            var call = BackendTools.NewCall(interview1);
            call.Priority = 1001;
            BackendTools.CreateCall(call);

            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId2);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview("", 0);

            var task = TaskRepository.GetByPerson(personId);
            CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview1.ID });

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.WrapUp(task.InterviewID, 1);

            task = TaskRepository.GetByPerson(personId);
            var secondInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview2.ID, interview3.ID });

            ws.ConsoleService.WrapUp(task.InterviewID, 1);

            task = TaskRepository.GetByPerson(personId);
            CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview2.ID, interview3.ID }.Except(new[] { secondInterviewId }));
        }

        /// <summary>
        /// Create 2 surveys, person in automatic mode, some interviews and calls. 
        /// Assign person to surveys. Login, start interview. Clear clr cache for persons. 
        /// Check that calls still delivered for both surveys.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_DuringInterviewPersonCacheIsCleanedInCLR_CallsAreDelivered()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.NewInterview(surveyId1);
            BackendTools.CreateInterview(interview1);

            var call = BackendTools.NewCall(interview1);
            call.Priority = 1001;
            BackendTools.CreateCall(call);

            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview3 = BackendTools.CreateInterviewWithCall(surveyId2);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(personId);

            ws.ConsoleService.WrapUp(task.InterviewID, 1);

            task = TaskRepository.GetByPerson(personId);
            var secondInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview2.ID, interview3.ID });

            ws.ConsoleService.WrapUp(task.InterviewID, 1);

            task = TaskRepository.GetByPerson(personId);
            CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview2.ID, interview3.ID }.Except(new[] { secondInterviewId }));
        }

        /// <summary>
        /// Create 2 surveys, person in automatic mode, one interview and call. 
        /// Assign person to surveys. Login, start interview, wrapup. Add call to 
        /// another survey, exec scheduling. Perform start interview (it emulate no 
        /// calls) Clear clr cache for persons. Check that calls still delivered for both surveys.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AutomaticPersonLogin_SecondStartInterviewAfterNoCallsIsCorrect_CallsAreDelivered()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            Trace.TraceInformation("Survey1 {0} Sid={1}", project1, surveyId1);

            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            Trace.TraceInformation("Survey2 {0} Sid={1}", project2, surveyId2);

            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Automatic);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
            Trace.TraceInformation("Interview1: Survey={0} InterviewId={1}", interview1.SurveySID, interview1.ID);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview("", 0);
            var task = TaskRepository.GetByPerson(personId);

            ws.ConsoleService.WrapUp(task.InterviewID, 1);

            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);
            Trace.TraceInformation("Interview2: Survey={0} InterviewId={1}", interview2.SurveySID, interview2.ID);

            var interview3 = BackendTools.CreateInterviewWithCall(surveyId2);
            Trace.TraceInformation("Interview3: Survey={0} InterviewId={1}", interview3.SurveySID, interview3.ID);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview("", 0);

            task = TaskRepository.GetByPerson(personId);
            Trace.TraceInformation("Task SurveySid {0} InterviewId {1} InterviewState {2}", task.SurveySID, task.InterviewID, task.InterviewState);

            var secondInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview2.ID, interview3.ID });

            Trace.TraceInformation("secondInterviewId {0}", secondInterviewId);

            ws.ConsoleService.WrapUp(secondInterviewId, 1);

            task = TaskRepository.GetByPerson(personId);
            Trace.TraceInformation("Task SurveySid {0} InterviewId {1} InterviewState {2}", task.SurveySID, task.InterviewID, task.InterviewState);

            CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview2.ID, interview3.ID }.Except(new[] { secondInterviewId }));
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh"), Bug(75409)]
        public void AutomaticPersonLogin_SelectAutomaticModeAfterSurveyAssignmentMode_CallsDeliveredCorrectly()
        {
            const string project1 = "p87547584";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            _surveyStateService.Open(surveyId1);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Choice);

            PersonService.UpdatePersonMode(PersonRepository.GetById(personId),
                                           AgentTaskChoiceMode.Choice,
                                           TaskChoicePermissions.SurveyAssignment | TaskChoicePermissions.Automatic, 
                                           true);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
            var interview2 = BackendTools.CreateInterviewWithCall(surveyId1);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.CampaignAssignment);

            ws.ConsoleService.StartInterview(project1, 0);

            var task = TaskRepository.GetByPerson(personId);

            var firstInterviewId = CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID, new[] { interview1.ID, interview2.ID });

            ws.ConsoleService.WrapUp(task.InterviewID, false, 1, new CompletedInterviewDetails());

            ws.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.Automatic);

            ws.ConsoleService.StartInterview("", 0);

            task = TaskRepository.GetByPerson(personId);

            CheckActualIdInExpectedListOfIdAndReturnIt(task.InterviewID,
                new[] { interview1.ID, interview2.ID }.Except(new[] { firstInterviewId }));
        }

        [TestMethod]
        public void PersonInAutomaticMode_DialerSupportsAutomaticMode_TwoSurveys_CallsAreDeliveredForBothSurveys()
        {
            var context = new TestData
            {
                Surveys = new[]{ new SurveyData
                {
                    Tag="S1", DialMode = DialingMode.Automatic, Assigns = new []{"P1"},
                    SchedulingScript = AllHoursSchedule.Name,
                    Interviews = new[] {
                        new InterviewData { Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData{Priority = 1}},
                        new InterviewData { Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData{Priority = 3}},
                    },
                },
                new SurveyData
                    {
                        Tag="S2", DialMode = DialingMode.Automatic, Assigns = new []{"P1"},

                        Interviews = new[] {
                            new InterviewData { Tag="S2.I1", ITS=CallOutcome.FreshSample, Call = new CallData{Priority = 2}},
                            new InterviewData { Tag="S2.I2", ITS=CallOutcome.FreshSample, Call = new CallData{Priority = 4}},
                        },
                    }
                },

                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.Automatic } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[]
                {
                    new DialerData(){ Tag = "D1", Type = "Generic"},
                }
            }.Create();

            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, null, dialer);

            int countOfCompleteCalls = 0;
            int countOfSendNumberToAgentCalls = 0;

            TestDialerHelper.IsPersonModeSupportedParams isPersonModeSupportedParams = null;
            TestDialerHelper.LoginParams loginParams = null;

            dialer.DialerHelper.SetBehaviorForLogin(args =>
            {
                loginParams = args;
                dialer.DialerHelper.SendEventNotifyAgentState(
                        args.CampaignId,
                        int.Parse(args.AgentId),
                        "1");
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForSendNumberToAgent(args =>
            {
                countOfSendNumberToAgentCalls++;
                return CallOutcome.Connected;
            });

            dialer.DialerHelper.SetBehaviorForCompleteCall((args) =>
            {
                countOfCompleteCalls++;
                return 0;
            });

            dialer.DialerHelper.SetBehaviorForIsPersonModeSupported(args =>
            {
                isPersonModeSupportedParams = args;
                if (isPersonModeSupportedParams.mode == (int)AgentTaskChoiceMode.Automatic)
                {
                    return true;
                }
                return false;
            });

            console.Login();
            console.LoginToDialer();

            Assert.IsTrue(loginParams.CampaignId > 0, "Campaign id is not set");

            var expected = context.GetInterviewsInOrder("S2.I2", "S1.I2", "S2.I1", "S1.I1");

            var actual = console.ProcessAllInterviews();

            Assert.AreEqual(4, countOfCompleteCalls);
            Assert.AreEqual(4, countOfSendNumberToAgentCalls);
            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray());
        }
    }
}
