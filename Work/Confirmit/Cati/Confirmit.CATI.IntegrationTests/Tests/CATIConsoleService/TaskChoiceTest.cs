using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.IntegrationTests.Framework.Fakes;

namespace Confirmit.CATI.IntegrationTests.Tests.CATIConsoleService
{
    [TestClass]
    public class TaskChoiceTest : BaseMockedIntegrationTest
    {
        private CatiWsHelper _serviceHelper;

        const string StationId = "";
        const string UserName = "APerson";
        const string UserPassword = "password";

        private PersonInfo _personInfo;
        private DiallerInfo _diallerInfo;

        private CatiConsolePropertiesContainer _outCatiConsoleProperties;

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        /// <summary>
        /// Prepare data for test
        /// 1. Add survey, launch 'all hours' script, open survey
        /// 2. Create interview with required parameters
        /// 3. Create calls
        /// 4. If use group functionality - create person groups and assign interviews to groups
        /// 5. Login for user
        /// </summary>
        private void PrepareDataForTest()
        {
            PersonTools.CreatePerson(UserName, UserPassword, AgentTaskChoiceMode.Manual);
            _serviceHelper = new CatiWsHelper(UserName, UserPassword);
        }


        /// <summary>        
        /// 1. Create interviewer        
        /// 2. Set task choice to "Choice"
        /// 2. Set permissions to "Automatic+Manual"
        /// 3. Login interviewer into Console
        /// 
        /// Console should receive correct permissions        
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SpecifyPermissionsInCP_GetPermissionsInCatiConsole_CorrectPermissions()
        {
            PrepareDataForTest();
            
            BvPersonEntity person = PersonRepository.GetByName(UserName);

            const TaskChoicePermissions permissions = TaskChoicePermissions.Automatic | TaskChoicePermissions.Manual;

            PersonService.UpdatePersonMode(
                person,
                AgentTaskChoiceMode.Choice,
                permissions,
                true);

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                StationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,                
                out _outCatiConsoleProperties);

            Assert.AreEqual((int)permissions, _personInfo.TaskChoicePermissions.Value, "CATI Console gets incorrect permissions on interviewer login");
        }

        /// <summary>        
        /// 1. Create interviewer        
        /// 2. Set task choice to "Choice"
        /// 2. Set permissions to "Manual+SurveyAssignement"
        /// 3. Login interviewer into Console
        /// 4. Update task choice from console side to "Manual"
        /// 
        /// Check interviewer task choice: should be "Manual"
        /// Check task state: interview state should "Selecting"
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void SpecifyPermissionsInCP_UdateInteviewerTaskChoiceInCatiConsole_CorrectInterviewerTaskChoice()
        {
            PrepareDataForTest();

            BvPersonEntity person = PersonRepository.GetByName(UserName);

            const TaskChoicePermissions permissions = TaskChoicePermissions.Manual| TaskChoicePermissions.SurveyAssignment;

            PersonService.UpdatePersonMode(
                person,
                AgentTaskChoiceMode.Choice,
                permissions,
                true);

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                StationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _outCatiConsoleProperties);

            _serviceHelper.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.Manual);

            person = PersonRepository.GetByName(UserName);
            Assert.AreEqual(AgentTaskChoiceMode.Manual, (AgentTaskChoiceMode)person.ManualSelection, "Incorrect person mode after update from CATI Console side");

            BvTasksEntity task = TaskRepository.GetByPerson(person.SID);
            Assert.AreEqual(InterviewState.SELECTING, (InterviewState)task.InterviewState, "Incorrect task state after update person mode from CATI Console side");            
        }

        /// <summary>        
        /// 1. Create interviewer        
        /// 2. Set task choice to "Choice"
        /// 2. Set permissions to "Automatic + Manual"
        /// 3. Login interviewer into Console
        /// 4. Update task choice from console side to "SurveyAssignement"
        /// 
        /// InvalidOperationException
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SpecifyPermissionsInCP_TryToUdateInteviewerTaskChoiceWithNotAllowedChoiceInCatiConsole_InvalidOperationException()
        {
            PrepareDataForTest();

            BvPersonEntity person = PersonRepository.GetByName(UserName);

            const TaskChoicePermissions permissions = TaskChoicePermissions.Automatic | TaskChoicePermissions.Manual;

            PersonService.UpdatePersonMode(
                person,
                AgentTaskChoiceMode.Choice,
                permissions,
                true);

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                StationId,
                consoleDescriptor,
                out _personInfo,
                out _diallerInfo,
                out _outCatiConsoleProperties);

            _serviceHelper.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.CampaignAssignment);
        }

        /// <summary>        
        /// 1. Create interviewer        
        /// 2. Set task choice to "Choice"
        /// 2. Dont set permissions at all
        /// 3. Login interviewer into Console
        /// 4. Try to update task choice from console side to "Manual"
        /// 
        /// InvalidOperationException        
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DontSpecifyPermissionsInCP_TryToUdateInteviewerTaskChoiceWithAnyChoiceInCatiConsole_InvalidOperationException()
        {
            PrepareDataForTest();

            PersonRepository.GetByName(UserName);

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(
                 StationId,
                 consoleDescriptor,
                 out _personInfo,
                 out _diallerInfo,
                 out _outCatiConsoleProperties);

            _serviceHelper.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.CampaignAssignment);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void StartInterviewForManualTaskChoice_AsynchPartOfStartInterviewIsDelayed_InterviewerStateIsWaiting()
        {
            ((FakeAsyncManager)ServiceLocator.Resolve<IAsyncManager>()).QueueWorkItemAction = null;

            PrepareDataForTest();

            BvPersonEntity person = PersonRepository.GetByName(UserName);
            const TaskChoicePermissions permissions = TaskChoicePermissions.Manual | TaskChoicePermissions.SurveyAssignment;
            PersonService.UpdatePersonMode(person, AgentTaskChoiceMode.Manual, permissions, true);
            const string surveyName = "p000001";

            int surveySid = BackendToolsObject.CreateSurvey(surveyName);
            BackendToolsObject.LaunchAllHoursScript();
            _surveyStateService.Open(surveySid);

            int personId = PersonTools.CreatePerson("i1", "password", AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(surveySid, personId);

            BvInterviewEntity interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);
            BvCallEntity call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            var consoleDescriptor = new ConsoleDescription();

            _serviceHelper.ConsoleService.Login(StationId, consoleDescriptor, out _personInfo, out _diallerInfo, out _outCatiConsoleProperties);

            _serviceHelper.ConsoleService.StartInterview(surveyName, interview.ID);

            BvTasksEntity task = TaskRepository.GetByPerson(person.SID);
            Assert.AreEqual(InterviewState.WAITING, (InterviewState)task.InterviewState);
        }

        /// <summary>
        /// 1. Create, launch and open CATI survey.
        /// 2. Create 2 interviews with calls.
        /// 3. Create person with person mode "Choice" and choice permissions "Manual" and "Survey Assignment".
        /// 4. Assign it to the survey.
        /// 5. Login to CATI WS and change person mode to "Survey Assignment".
        /// 6. Logout from CATI WS.
        /// 7. Login to CATI WS and change person mode to "Manual".
        /// 8. Get list of survey interviews. The list should contain 2 created interviews.
        /// </summary>
        [TestMethod, Owner(@"FIRM\SergeyC")]
        [Cr(50397)]
        public void GetSurveyInterviews_ManualModeAfterSurveyAssignmentModeForTaskChoice_InterviewsAreReturnedSuccessfully()
        {
            var test = new TestCati2(true, false, BackendToolsObject);
            int surveySid = test.CreateSurveyWithPerson(DialingMode.Manual, "user", "password", AgentTaskChoiceMode.Automatic);
            const TaskChoicePermissions permissions = TaskChoicePermissions.Manual | TaskChoicePermissions.SurveyAssignment;
            BvPersonEntity person = PersonRepository.GetById(test.PersonSID);
            PersonService.UpdatePersonMode(person, AgentTaskChoiceMode.Choice, permissions, true);

            test.CreateInterviewsWithCalls(2);
            BackendTools.AssignCatiPersonToSurvey(surveySid, test.PersonSID);

            test.Login("user", "password", AgentTaskChoiceMode.Automatic, true);
            test.WS.UpdatePersonMode((int)AgentTaskChoiceMode.CampaignAssignment);
            test.Logout();

            test.Login(
                "user",
                "password",
                AgentTaskChoiceMode.CampaignAssignment,
                true,
                new State
                    {
                        interviewerLoginState = (int) LoginState.LOGGED_IN,
                        callOutcome = (int) CallOutcome.NotDefined,
                        interviewState = (int) InterviewState.NO_CALLS
                    });
            test.WS.UpdatePersonMode((int)AgentTaskChoiceMode.Manual);

            test.GetSurveyInterviews(2);
        }
    }
}
