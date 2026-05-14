using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.LoginTests
{
    [TestClass]
    public class ManualPersonLoginTest : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        private void CheckResultGetSurveyInterviews(CatiWsHelper ws,
            string project,
            int expectedCount,
            IEnumerable<int> expectedInterviewIds)
        {
            var table = ws.ConsoleService.GetSurveyInterviews(project, new SearchParameter[0]);
            Assert.AreEqual(expectedCount, table.Rows.Count, "Count of interview for first survey");
            TestAssert.AreEqual(expectedInterviewIds.OrderBy(x => x).Select(x => x),
                table.Select().Select(x => (int)x["InterviewId"]).OrderBy(x => x).Select(x => x));
        }

        private void CheckResultGetOpenedSurveys(CatiWsHelper ws,
            int expectedCount,
            IEnumerable<string> expectedProjects)
        {
            var surveys = ws.ConsoleService.GetOpenedSurveys();
            Assert.AreEqual(expectedCount, surveys.Length, "Count of opened surveys");
            TestAssert.AreEqual(expectedProjects.OrderBy(x => x).Select(x => x),
                surveys.OrderBy(x => x.id).Select(x => x.id));
        }

        private void CheckLeftCalls(int surveyId, int expectedCount)
        {
            Assert.AreEqual(expectedCount,
                BvSvyScheduleAdapter.GetAll().Count(x => (x.SurveySID == surveyId) && (x.CallState == (int)CallState.Scheduled)),
                "Count of interview which haven't passed yet");
        }

        /// <summary>
        /// Create 2 surveys, person in manual mode, some interviews and calls. 
        /// Assign person to survey. Login, get list of interview for first survey 
        /// then for second survey. StartInterview  for second survey. Get interviews 
        /// for first survey. Start interview from first survey. After each step check state of bvtasks table.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ManualPersonLogin_PersonGetCallFromDifferentSurveys_CallsDeliveredCorrectly()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Manual);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1);
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

            CheckResultGetOpenedSurveys(ws, 2, new[] { project1, project2 });

            CheckResultGetSurveyInterviews(ws, project1, 2, new[] { interview1.ID, interview2.ID });
            CheckResultGetSurveyInterviews(ws, project2, 1, new[] { interview3.ID });

            ws.ConsoleService.StartInterview(project2, interview3.ID);
            ws.ConsoleService.WrapUp(interview3.ID, 1);

            ws.ConsoleService.StartInterview(project1, interview1.ID);
            ws.ConsoleService.WrapUp(interview1.ID, 1);

            CheckResultGetOpenedSurveys(ws, 2, new[] { project1, project2 });

            CheckResultGetSurveyInterviews(ws, project1, 1, new[] { interview2.ID });
            CheckLeftCalls(surveyId1, 1);
            CheckLeftCalls(surveyId2, 0);
        }

        /// <summary>
        /// Create 2 surveys, person in manual mode, some interviews and calls. 
        /// Assign person to survey. Login, get list of interview for first survey 
        /// and startInterview. Change mode from manual to automatic. WrapUpInterview. 
        /// Check that person in automatic mode and interviews are got from both surveys.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ManualPersonLogin_ChangeModeDuringInterview_CallsDeliveredCorrectly()
        {
            const string project1 = "p87547584";
            const string project2 = "p948573342";
            const string personName = "gigigi";
            const string personPassword = "gigigi";

            int surveyId1 = BackendToolsObject.CreateSurvey(project1);
            int surveyId2 = BackendToolsObject.CreateSurvey(project2);
            _surveyStateService.Open(surveyId1);
            _surveyStateService.Open(surveyId2);

            int personId = PersonTools.CreatePerson(personName, personPassword, AgentTaskChoiceMode.Manual);

            var interview1 = BackendTools.CreateInterviewWithCall(surveyId1, 1);
            BackendTools.CreateInterviewWithCall(surveyId2, 2);
            BackendTools.CreateInterviewWithCall(surveyId2, 3);

            BackendTools.AssignCatiPersonToSurvey(surveyId1, personId);
            BackendTools.AssignCatiPersonToSurvey(surveyId2, personId);

            var ws = new CatiWsHelper(personName, personPassword);

            PersonInfo personInfo;
            DiallerInfo diallerInfo;
            CatiConsolePropertiesContainer catiConsoleProperties;

            var consoleDescriptor = new ConsoleDescription();

            ws.ConsoleService.Login("", consoleDescriptor, out personInfo, out diallerInfo, out catiConsoleProperties);

            BackendTools.RunSchedulingProcedure();

            ws.ConsoleService.StartInterview(project1, interview1.ID);

            PersonService.UpdatePersonMode(PersonRepository.GetById(personId), AgentTaskChoiceMode.Automatic, null, true);

            ws.ConsoleService.WrapUp(interview1.ID, 1);

            var task = TaskRepository.GetByPerson(personId);
            Assert.IsTrue(task.InterviewID > interview1.ID, "Next call should be delivered");

            ws.ConsoleService.WrapUp(task.InterviewID, 1);
            CheckLeftCalls(surveyId1, 0);
            CheckLeftCalls(surveyId2, 0);
        }

        /// <summary>
        /// Create 2 surveys, person in choise mode, some interviews and calls. 
        /// Assign person to survey. Login, choose manual mode get list of interview 
        /// for first survey and startInterview. Get interviews for second survey. Start 
        /// interview from second survey. After each step check state of bvtasks table.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void ManualPersonLogin_SelectManualModeDuringChoiseMode_CallsDeliveredCorrectly()
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
                TaskChoicePermissions.Manual, 
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

            ws.ConsoleService.UpdatePersonMode((int)AgentTaskChoiceMode.Manual);

            ws.ConsoleService.StartInterview(project1, interview1.ID);
            ws.ConsoleService.WrapUp(interview1.ID, 1);

            ws.ConsoleService.StartInterview(project2, interview2.ID);
            ws.ConsoleService.WrapUp(interview2.ID, 1);
            CheckLeftCalls(surveyId1, 0);
            CheckLeftCalls(surveyId2, 0);
        }        
    }
}
