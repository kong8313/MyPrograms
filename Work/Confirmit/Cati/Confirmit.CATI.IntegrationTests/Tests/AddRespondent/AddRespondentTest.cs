using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;


namespace Confirmit.CATI.IntegrationTests.Tests.AddRespondent
{
    [TestClass]
    public class AddRespondentTest
    {
        private const int RespId = 1;
        private const int BatchId = 3;
        private const int RecordsCount = 1;
        private const CallOutcome DefaultCallOutcome = CallOutcome.Busy;

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private static DatabaseEngine _confirmitDb;
        private static string _projectId;

        private TestScript _testScript;
        private const int NewPriority = 100;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _confirmitDb = ConfirmitTools.GetConfirmitSurveyDbOnClass(out _projectId);

            ConfirmitTools.FillRespondentTable(_confirmitDb,
               BatchId,
               RespId,
               RecordsCount,
               Enumerable.Range(1, RecordsCount));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            IntegrationTestingFramework.ClassCleanup();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);

            _testScript = new TestScript(
                new SubRule(new[]
                {
                    new Action(Action.Operation.SetNewCallPriority, NewPriority.ToString(CultureInfo.InvariantCulture))
                }),
                new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                new Shift(2, 1, "1.00:00:00", "2.00:00:00"),
                new Shift(3, 1, "2.00:00:00", "3.00:00:00"),
                new Shift(4, 1, "3.00:00:00", "4.00:00:00"),
                new Shift(5, 1, "4.00:00:00", "5.00:00:00"),
                new Shift(6, 1, "5.00:00:00", "6.00:00:00"),
                new Shift(7, 1, "6.00:00:00", "0.00:00:00"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), ExpectedException(typeof(SurveyNotFoundException))]
        public void AddRespondent_SurveyWasNotLaunched_ThrowException()
        {
            new ManagementService().AddRespondent(_projectId, RespId, (int)CallOutcome.Completed);
        }

        void AssertSystemState(int surveyId, Role role)
        {
            const CallOutcome expectedIts = CallOutcome.Completed;
            var interview = InterviewRepository.GetById(surveyId, RespId);
            Assert.IsNotNull(interview, "Interview");
            Assert.AreEqual(0, interview.BatchID, "interview batchid");
            Assert.AreEqual(expectedIts, (CallOutcome)interview.TransientState, "interview its");
            Assert.AreEqual(1, interview.TimezoneID, "interview timezoneid");
            Assert.AreEqual("1", interview.TelephoneNumber, "interview telephone number");
            Assert.AreEqual("resp1", interview.RespondentName, "interview respondent name");

            var calls = BvSvyScheduleAdapter.GetAll();
            Assert.AreEqual(1, calls.Count, "calls count");
            var call = calls.First();
            Assert.AreEqual(RespId, call.InterviewID, "call interviewid");
            Assert.AreEqual(surveyId, call.SurveySID, "call surveysid");
            Assert.AreEqual(2, call.CallState, "call phase");
            Assert.AreEqual(NewPriority, call.Priority, "call priority");

            var histories = BvHistoryAdapter.GetAll();
            Assert.AreEqual(1, histories.Count, "history count");
            var history = histories.First();
            Assert.AreEqual(RespId, history.InterviewId, "history interviewid");
            Assert.AreEqual(expectedIts, (CallOutcome)history.ITS, "history its");
            Assert.AreEqual(0, history.PersonSID, "history person id");
            Assert.AreEqual(surveyId, history.SurveyId, "history survey id");
            Assert.AreEqual("1", history.TelephoneNumber, "history telephone number");
            Assert.AreEqual((byte)role, history.RoleID, "history RoleID");

        }

        [TestMethod, Owner(@"FIRM\LeshinE")]
        public void Custom_AddRespondent_SurveyWasLaunched_InsertRecordInInterviewCallAndHistoryTableAndRunScheduling()
        {
            var confirmitDb = ConfirmitTools.GetConfirmitSurveyDbOnClass(out _projectId);

            var lines = new List<string>
        {
            "TelephoneNumber\tEmail\tRespondentName\tCatiShiftType\tCatiCallPriority\tCatiCallState",
            "90001\ta1@com.com\tMr.1\t-1\t11\t0",
            "90002\ta2@com.com\tMr.2\t0\t12\t1",
            "90003\ta3@com.com\tMr.3\t-1\t13",
            "90004\ta1@com.com\tMr.4\t0\t14\tA",
            "90005\ta2@com.com\tMr.5\t\t15\t0",
            "90006\ta3@com.com\tMr.6\tA\t16\t1",
            "90007\ta1@com.com\tMr.7",
            "90008\ta2@com.com\tMr.8",
            "90009\ta3@com.com\tMr.9"
        };

            ConfirmitTools.FillRespondentTable(confirmitDb,
               BatchId,
               RespId,
               lines);

            var surveyId = _backendTools.CreateSurvey(_projectId, confirmitDb.ConnectionString);
            _backendTools.LaunchScript(surveyId, _testScript);


            for (int i = 1; i < lines.Count(); i++)
            {
                var sampleData = lines[i].Split('\t');

                new ManagementService().AddRespondent(_projectId, i, (int)CallOutcome.Completed);

                const CallOutcome expectedIts = CallOutcome.Completed;
                var interview = InterviewRepository.GetById(surveyId, i);
                Assert.IsNotNull(interview, "Interview");
                Assert.AreEqual(0, interview.BatchID, "interview batchid");
                Assert.AreEqual(expectedIts, (CallOutcome)interview.TransientState, "interview its");
                Assert.AreEqual(sampleData[2], interview.RespondentName, "RespondentName");
            }

            var callHistory = BvCallHistoryExAdapter.GetAll();
            foreach (var call in callHistory)
            {
                Assert.AreEqual((byte)OperationType.AddRecordInWebInterview, call.OperationType);
            }
            Assert.AreEqual(callHistory.Count(), lines.Count() - 1);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddRespondent_SurveyWasLaunched_InsertRecordInInterviewCallAndHistoryTableAndRunScheduling()
        {
            var surveyId = _backendTools.CreateSurvey(_projectId, _confirmitDb.ConnectionString);
            _backendTools.LaunchScript(surveyId, _testScript);

            new ManagementService().AddRespondent(_projectId, RespId, (int)CallOutcome.Completed);

            AssertSystemState(surveyId, Role.WebRespondent);
            var callHistory = BvCallHistoryExAdapter.GetAll().Single();
            Assert.AreEqual((byte)OperationType.AddRecordInWebInterview, callHistory.OperationType);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void AddRespondent_InterviewAlreadyExists_FullSchedulingShouldBeRun()
        {
            var surveyId = _backendTools.CreateSurvey(_projectId, _confirmitDb.ConnectionString);
            _backendTools.LaunchScript(surveyId, _testScript);

            var interview = BackendTools.NewInterview(surveyId);
            interview.ID = RespId;
            interview.RespondentName = "Other respondent name";
            interview.TelephoneNumber = "strange telephone number";
            interview.TransientState = (int)DefaultCallOutcome;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Priority = NewPriority + 1;
            BackendTools.CreateCall(call);

            new ManagementService().AddRespondent(_projectId, RespId, (int)CallOutcome.Completed);

            AssertSystemState(surveyId, Role.WebRespondent);
            AssertSystemState(surveyId, Role.WebRespondent);
            var callHistory = BvCallHistoryExAdapter.GetAll().Single();
            Assert.AreEqual((byte)OperationType.UpdateRecordInWebInterview, callHistory.OperationType);
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void AddRespondent_AddRespondentTwice_JustSingleReplicationDataRowInserted()
        {
            var surveyId = _backendTools.CreateSurvey(_projectId, _confirmitDb.ConnectionString);
            _backendTools.LaunchScript(surveyId, _testScript);

            var interview = BackendTools.NewInterview(surveyId);
            interview.ID = RespId;
            interview.RespondentName = "Other respondent name";
            interview.TelephoneNumber = "strange telephone number";
            interview.TransientState = (int)DefaultCallOutcome;
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Priority = NewPriority + 1;
            BackendTools.CreateCall(call);

            new ManagementService().AddRespondent(_projectId, RespId, (int)CallOutcome.Completed);
            new ManagementService().AddRespondent(_projectId, RespId, (int)CallOutcome.Completed);

            var _replicationService = ServiceLocator.Resolve<IReplicationService>();
            var replicationRecords = _replicationService.GetNumberOfReplicationRecords(_projectId, RespId);
            Assert.AreEqual(1, replicationRecords);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void AddRespondent_AddRespondentSeveralTimesInDifferentThreads_AllAddRespondentCallsCompletedSuccessfully()
        {
            var surveyId = _backendTools.CreateSurvey(_projectId, _confirmitDb.ConnectionString);
            _backendTools.LaunchScript(surveyId, _testScript);

            var tasks = new List<Task>();
            for (var i = 0; i < 10; i++)
            {
                var task = Task.Run(() =>
                {
                    new ManagementService().AddRespondent(_projectId, RespId, (int)CallOutcome.Completed);
                });
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddRespondent_AddRespondentThrowSaveInterviewHistoryAndContralData_InterviewIsCreatedWithreplicationData()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData { Tag = "S1", SchedulingScript = AllHoursSchedule.Name, IsUseDb = true } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviewData = new InterviewData() { RespondentName = "respName", TelephoneNumber = "123450" };

            var respId = survey.Database.AddInterview(0, "13", interviewData);

            new ManagementService().SaveInterviewHistoryAndControlData(new InterviewHistoryData()
            {
                appointmentID = 0,
                grossDuration = 10,
                interviewerID = 0,
                interviewID = respId,
                netDuration = 0,
                projectID = survey.Model.Name,
                respondentPhone = interviewData.TelephoneNumber,
                roleID = (int)Role.WebRespondent,
                status = "complete",
                time = DateTime.UtcNow,
                totalDuration = 20

            },
                new InterviewControlData()
                {
                    projectID = survey.Model.Name,
                    interviewID = respId,
                    interviewerID = 0,
                    lastCallTime = DateTime.UtcNow,
                    lastChannelID = 0,
                    respondentName = interviewData.RespondentName,
                    respondentPhone = interviewData.TelephoneNumber,
                    roleID = (int)Role.WebRespondent,
                    status = "complete",
                    totalDuration = 20
                });
            var interview = BvInterviewAdapter.GetAll().SingleOrDefault();
            Assert.IsNotNull(interview);

            Assert.AreEqual(survey.Id, interview.SurveySID);
            Assert.AreEqual(respId, interview.ID);
            Assert.AreEqual(interviewData.TelephoneNumber, interview.TelephoneNumber);
            Assert.AreEqual(interviewData.RespondentName, interview.RespondentName);
            Assert.AreEqual((int)CallOutcome.Completed, interview.TransientState);

            Assert.AreEqual(1, ServiceLocator.Resolve<IReplicationService>().GetNumberOfReplicationRecords(survey.Model.Name, respId));
        }
    }
}
