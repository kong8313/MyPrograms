using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using CatiOlympicPrepareTest.Constants;
using CatiOlympicPrepareTest.Helpers;
using Confirmit.CATI.REST.SDK.Interfaces;
using Confirmit.CATI.REST.SDK.Model;
using Confirmit.CATI.REST.SDK.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CatiOlympicPrepareTest
{
    [TestClass]
    public class AuthoringAndCatiPrepare
    {
        private IRestClient _client;
        private IInterviewerService _interviewerService;
        private ISurveyService _surveyService;
        private ICallHistoryService _callHistoryService;
        private ICallHistoryWithVariablesService _callHistoryWithVariablesService;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            CatiOlympicPrepareTestHelper.ConnectionString = Environment.GetEnvironmentVariable("CatiSQLConnectionString");

            ConfigurationManager.AppSettings.Set("CatiServerAddress",
                "http://" + TestContext.DataRow["catiServer"] + "/");
            ConfigurationManager.AppSettings.Set("WsServerAddress", TestContext.DataRow["wsServer"].ToString());
            //ConfigurationManager.AppSettings.Set("ProxyServerAddress", "http://" + TestContext.DataRow["wsServer"].ToString());

            ConfigurationManager.AppSettings.Set("CompanyId", TestContext.DataRow["companyId"].ToString());
            ConfigurationManager.AppSettings.Set("UserName", TestContext.DataRow["login"].ToString());
            ConfigurationManager.AppSettings.Set("Password", TestContext.DataRow["password"].ToString());

            _client = RestClientFactory.Create();
            _interviewerService = new InterviewerService(_client);
            _surveyService = new SurveyService(_client);
            _callHistoryService = new CallHistoryService(_client);
            _callHistoryWithVariablesService = new CallHistoryWithVariablesService(_client);
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            _client.Dispose();
        }

        [TestMethod]
        [DeploymentItem("OlympicData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\OlympicData.xml",
            "work", DataAccessMethod.Sequential)]
        public async Task OlympicPrepareSurveyTest()
        {
            var server = TestContext.DataRow["wsServer"].ToString();
            var login = TestContext.DataRow["login"].ToString();
            var password = TestContext.DataRow["password"].ToString();
            var dialMode = TestContext.DataRow["dialMode"].ToString();
            var surveyMode = TestContext.DataRow["surveyMode"].ToString();
            var surveyLocation = TestContext.DataRow["surveyLocation"].ToString();
            var surveyName = TestContext.DataRow["surveyName"].ToString();
            var languageFilePath = TestContext.DataRow["language_q5_q6"].ToString();
            var taskChoice = TestContext.DataRow["taskChoice"].ToString();
            var shedulingScriptName = TestContext.DataRow["shedulingScript"].ToString();
            var interviewerName = TestContext.DataRow["interviewerName"].ToString();
            var interviewerLocation = TestContext.DataRow["interviewerLocation"].ToString();
            var companyId = TestContext.DataRow["companyId"].ToString();

            var logOnServer = CatiOlympicPrepareTestHelper.GetLogonClient(server);
            var authKey = logOnServer.LogOnUser(login, password);

            int dialerId = 0;

            if (SurveyMode.IsDialerRequired(surveyMode))
            {   // if survey need dialer
                dialerId = CatiOlympicPrepareTestHelper.CheckFirstDialerStateAndEnableIfNeed(companyId);
            }
            
            var surveyId = CatiOlympicPrepareTestHelper.ImportSurvey(server, surveyLocation, surveyName, authKey );
            CatiOlympicPrepareTestHelper.LaunchSurvey(server, surveyId, authKey);

            if (SurveyMode.IsDialerRequired(surveyMode))
            {
                int newDialerId;
                var dialerState = CatiOlympicPrepareTestHelper.GetFirstDialerState(companyId, out newDialerId);
                if (!dialerState || newDialerId != dialerId)
                {
                    throw new Exception(string.Format("Dialer {0} can't be turned to available and operational state, real state and dialerId:{1},{2}",dialerId,dialerState,newDialerId));
                }
            }
            // open survey in CATI
            await _surveyService.Open(surveyId);
            var openedSurvey = await _surveyService.GetAsyncByKey(surveyId);
            Assert.AreEqual(SurveyState.Open, openedSurvey.State);

            // set appropriate task choice for interviewer
            var mode = (taskChoice == "SurveyAssignment") ? TaskChoiceMode.SurveyAssignment :
                (taskChoice == "Automatic") ? TaskChoiceMode.Automatic :
                    (taskChoice == "Manual") ? TaskChoiceMode.Manual : TaskChoiceMode.Choice;

            // check if interviewer exists
            var interviewers = await _interviewerService.GetAsync(String.Format("$filter=Name eq '{0}'", interviewerName));
            var interviewer = interviewers.FirstOrDefault();
            int interviewerId;

            var interviewerProperties = CatiOlympicPrepareTestHelper.CreateEmptyInterviewerEntity();

            if (interviewer != null)
            {
                interviewerId = interviewer.InterviewerId;
            }
            else
            {
                // create interviewer
                interviewerProperties.Name = interviewerName;
                interviewerProperties.Location = interviewerLocation;
                interviewerProperties.Password = password;
                interviewerProperties.Mode = mode;
                interviewerProperties.AssignmentsListMode = AssignmentListMode.AllCalls;
                
                interviewerId = await _interviewerService.Create(interviewerProperties);
            }

            //we should check all info about this interviewer (doesn't matter if he has been in system already or was just created by test)
            interviewers = await _interviewerService.GetAsync(String.Format("$filter=InterviewerId eq {0}", interviewerId));
            Assert.IsNotNull(interviewers);
            interviewer = interviewers.FirstOrDefault();
            Assert.IsNotNull(interviewer);

            // compare expected with actual interviewer properties
            var interviewerActualProprerties = new List<string>
            {
                interviewer.Name,
                interviewer.Location,
                interviewer.ManualSelection.ToString(),
                interviewer.AssignmentsListMode.ToString()
            };
                
            //get expected info about interviewer
            var interviewerExpectedProprerties = new List<string>
            {
                interviewerName,
                interviewerLocation,
                ((int)mode).ToString(),
                AssignmentListMode.AllCalls.ToString()
            };

            CollectionAssert.AreEqual(
                interviewerExpectedProprerties, interviewerActualProprerties,
                string.Format("Interviewer {0} has unexpected property value(s)", interviewerName));
            
            // interviewer's assignment
            await _interviewerService.AssignOnSurvey(interviewerId, surveyId);
            var assignments = await _interviewerService.GetAssignments(interviewerId);
            Assert.IsNotNull(assignments);
            Assert.IsTrue(assignments.Any(assignment => assignment.SurveyId == surveyId));
            
            // set automatic survey
            interviewerProperties.AutomaticSurveyId = surveyId;
            await _interviewerService.Update(interviewerProperties);

            var updatedUnterviewer = await _interviewerService.GetAsync(interviewerProperties.InterviewerId);
            Assert.IsNotNull(updatedUnterviewer);
            Assert.AreEqual(surveyId, updatedUnterviewer.AutomaticSurveyId);

            var properties = await _surveyService.GetBasicProperties(surveyId);
            Assert.IsNotNull(properties);
            Assert.AreEqual(surveyId, properties.SurveyId);

            properties.Scheduling = shedulingScriptName;
            await _surveyService.PutBasicProperties(properties);

            properties = await _surveyService.GetBasicProperties(surveyId);
            Assert.IsNotNull(properties);
            Assert.AreEqual(surveyId, properties.SurveyId);
            Assert.AreEqual(shedulingScriptName, properties.Scheduling);

            CatiOlympicPrepareTestHelper.AddSampleAndUpdateSample(dialMode, surveyMode, server, authKey, surveyId, languageFilePath, 12);
        }

        [TestMethod]
        [DeploymentItem("OlympicData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\OlympicData.xml", "work", DataAccessMethod.Sequential)]
        public async Task OlympicCloseSurveyTest()
        {
            var surveyName = TestContext.DataRow["surveyName"].ToString();
            var survey = await GetSurvey(surveyName);

            Assert.AreEqual(SurveyState.Open, survey.State);

            await _surveyService.Close(survey.SurveyId);

            var closedSurvey = await _surveyService.GetAsyncByKey(survey.SurveyId);

            Assert.AreEqual(SurveyState.Closed, closedSurvey.State, String.Format("Survey {0} can't be closed", survey.SurveyId));
        }

        public async Task<Survey> GetSurvey(string surveyName)
        {
            var surveys = await _surveyService.GetAsync(String.Format("$filter=SurveyName eq '{0}'", surveyName));
            //Test gets survey by it's complete name written in data source 'OlympicCodedUi'+OlympicMode+DateTime (example 
            //'OlympicCodedUiNoDialer9/2/2015 6:17:14 PM).
            //DateTime is substituted there during work of target "BeforeTests" in OlympicActions.proj
            var survey = surveys.First();
            return survey;
        }

        [TestMethod]
        [DeploymentItem("OlympicData.xml")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\OlympicData.xml", "work", DataAccessMethod.Sequential)]
        public async Task OlympicCheckFcdInterviewStatusesCallStatesTestAsync()
        {
            var companyId = TestContext.DataRow["companyId"].ToString();
            // ******* Checking if 'FCD.BehaviorType' setting is proper for current company and set it if not *******
            // for Confirmit 'FCD.BehaviorType' should be equal to '0', for CatiOldDialerApi 'FCD.BehaviorType' should be equal to '1'
            var fcdBehaviorType = TestContext.DataRow["fcdBehaviorType"].ToString();
            CatiOlympicPrepareTestHelper.SetFcdBehaviorType(companyId, fcdBehaviorType);
            
            var interviewIds = new List<int>(); //id's of interviews which status we want to check
            var interviewExpectedStatuses = new List<int>(); //expected statuses of interviews
            var interviewCallHistoryStatus = new List<short>();
            var interviewData = TestContext.DataRow.GetChildRows("work_interview");
            foreach (var interview in interviewData)
            {
                interviewIds.Add(Convert.ToInt32(interview["id"]));
                interviewExpectedStatuses.Add(Convert.ToInt32(interview["expectedStatus" + fcdBehaviorType]));
                if (interviewIds[interviewIds.Count-1] < 7)
                    interviewCallHistoryStatus.Add(Convert.ToInt16(interview["CallHistoryStatus"]));
            }

            var surveyName = TestContext.DataRow["surveyName"].ToString();
            var survey = await GetSurvey(surveyName);
            var surveySid = CatiOlympicPrepareTestHelper.GetSurveySid(surveyName, companyId);
            //Test gets survey's SID by it's complete name
            //written in data source 'OlympicCodedUi'+OlympicMode+DateTime            
            var actualInterviewStatuses = CatiOlympicPrepareTestHelper.GetActualInterviewStatuses(surveySid, interviewIds, companyId); 
            //get real statuses of these interviews

            // ******* Checking interview's statuses *******           
                CollectionAssert.AreEqual(
                    interviewExpectedStatuses, actualInterviewStatuses, new InterviewsComparer(TestContext, 0,interviewIds.Count, "Status"),
                    string.Format("Interview(s) of project {0} (SID = {1} ) doesn't(don't) have expected status(es)",
                    surveyName, surveySid));            
            // comparing of expected status of interview with actual status

            // Checking disabled and enabled call states for Call filtering behaviour for filled quotas == Disable call(s)
            // with re-enabling on open cell(s)
            if (fcdBehaviorType == "1") //let's check that calls 7,8,9 are disabled cause quotas are full,
            {   // calls 10,11,12 should be enabled 
                var callsId = interviewIds.GetRange(6, 6); // contains {7,8,9,10,11,12}
                var expectedCallStates = new List<int>(); //by data source calls with id 7,8,9 are expected to have CallState=1 (disabled)
                //for calls 10,11,12 expected CallState is 2 (enabled)
                for (var i = 6; i <= 11; i++)
                {
                    expectedCallStates.Add(Convert.ToInt32(interviewData[i]["expectedCallState"]));
                }
                var actualCallStates = CatiOlympicPrepareTestHelper.GetActualCallStates(surveySid, callsId, companyId);
                //get real CallStates of these calls

                // comparing of expected states of calls with real states
                CollectionAssert.AreEqual(
                        expectedCallStates, actualCallStates, new InterviewsComparer(TestContext, 0, callsId.Count,"CallState"),
                        string.Format("Call(s) of project {0} (SID = {1} ) is(are) in wrong state(s)",
                        surveyName, surveySid));
                
            }

            //var actual0 = await _callHistoryService.GetAsync("");

            var actualCallHistory = await _callHistoryService.GetAsync(String.Format("$filter=SurveyId eq '{0}'", survey.SurveyId));

            var actualCallHistoryWithVars = await _callHistoryWithVariablesService.GetAsync(new List<string>() { survey.SurveyId }, true, true, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow.AddHours(1), new List<string> { "q5", "q6",  });

            CollectionAssert.AreEqual(actualCallHistory, actualCallHistoryWithVars.GetRange(0,actualCallHistory.Count), new CallHistoryComparer(TestContext, 0, actualCallHistory.Count));

            CollectionAssert.AreEqual(actualCallHistory, interviewCallHistoryStatus, new CallHistoryWithIntArrayComparer(TestContext, 0, interviewCallHistoryStatus.Count));
        }
    }
}