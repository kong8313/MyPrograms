using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Services.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests
{
    [TestClass]
    public class CallMovementTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public void Init()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();

            _localTimezoneId = ServiceLocator.Resolve<ICallCenterRepository>().Default.LocalTimezoneId;

            ProjectId = BackendTools.GenerateSurveyName();
            _cfSurveyDbName = "survey_" + ProjectId;
            _confirmitSurveyDb = new DatabaseEngine(_framework.GetConfirmitSqlServerConnectionString(_cfSurveyDbName));

            new DatabaseTools(_framework.ConfirmitSqlServerMasterConnectionString).
                CreateEmptyDatabase(_cfSurveyDbName);

            ConfirmitTools.CreateQuotaTables(_confirmitSurveyDb);

            _surveySid = _backendTools.CreateSurvey(ProjectId, _confirmitSurveyDb.ConnectionString);
            FusionLibTestTools.UpdateStatePriorityOfNewIts(_surveySid, NewIts, NewItsPriority);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                new DatabaseTools(_framework.GetConfirmitSqlServerConnectionString("master")).DropDatabase(_cfSurveyDbName);
            }
            finally
            {
                    _framework.TestCleanup();
            }
        }

        
        void FillRespondentTable(IEnumerable<int> interviewIds)
        {
            const int defaultIts = 16;

            foreach (var interviewId in interviewIds)
            {
                _confirmitSurveyDb.ExecuteNonQuery(
                    "insert into response_control(respid, its) values(@respid, @its)",
                    CommandType.Text,
                    new SqlParameter("@respid", interviewId),
                    new SqlParameter("@its", defaultIts));
            }
        }

        

        private string ProjectId;
        private const int NewIts = 73;
        private const int NewItsPriority = 173;
        private int _surveySid;
        private DatabaseEngine _confirmitSurveyDb;
        private string _cfSurveyDbName;
        private int _localTimezoneId;

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_Scheduled_CallsMoved()
        {
            var interviewIds = new[] { 1, 2, 3 };
            var movableInterviewIds = new[] { 1, 3 };
            const string userName = "u1";
            const string userPassword = "u1";

            _surveyStateService.Open(_surveySid);
            int personId = PersonTools.CreatePerson(userName, userPassword, AgentTaskChoiceMode.Automatic);
            BackendTools.AssignCatiPersonToSurvey(_surveySid, personId);
            BackendTools.LoginPerson(personId, "");

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            FillRespondentTable(interviewIds);
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new[] { interviews[0], interviews[1] }).ToList();

            CallTools.MoveCalls(_surveySid, movableInterviewIds, NewIts);

            interviews[movableInterviewIds[0] - 1].TransientState = NewIts;
            interviews[movableInterviewIds[1] - 1].TransientState = NewIts;
            calls[movableInterviewIds[0] - 1].Priority = NewItsPriority;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)).Where(x => x != null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_ScheduledAndCustomFiltered_CallsMoved()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.LessEqual, "1");
            
            CallTools.MoveCalls(_surveySid, filterSid, CallStates.Scheduled, NewIts, _localTimezoneId, null);

            interviews[0].TransientState = NewIts;
            calls[0].Priority = NewItsPriority;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_ScheduledAndDefaultFiltered_CallsMoved()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(interviews).ToList();

            CallTools.MoveCalls(_surveySid, 0, CallStates.Scheduled, NewIts, _localTimezoneId, null);

            TestAssert.AreEqual(
                interviews.Select(x => {x.TransientState = NewIts; return x;} ),
                interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));

            TestAssert.AreEqual(
                calls.Select(x => { x.Priority = NewItsPriority; return x; }),
                interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_Suspended_CallsMoved()
        {
            var interviewIds = new[] { 1, 2, 3 };
            var movableInterviewIds = new[] { 1, 3 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();

            CallTools.MoveCalls(_surveySid, movableInterviewIds, NewIts);

            interviews[movableInterviewIds[0] - 1].TransientState = NewIts;
            interviews[movableInterviewIds[1] - 1].TransientState = NewIts;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_SuspendedAndCustomFiltered_CallsMoved()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();

            int filterSid = FusionLibTestTools.CreateFilterForTest("ID", FilterOperator.LessEqual, "1");

            CallTools.MoveCalls(_surveySid, filterSid, CallStates.Suspended, NewIts, _localTimezoneId, null);

            interviews[0].TransientState = NewIts;

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_SuspendedAndDefaultFiltered_CallsMoved()
        {
            var interviewIds = new[] { 1, 2 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();

            CallTools.MoveCalls(_surveySid, 0, CallStates.Suspended, NewIts, _localTimezoneId, null);

            TestAssert.AreEqual(
                interviews.Select(x => { x.TransientState = NewIts; return x; }),
                interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Bug(38350)]
        public void MoveCalls_MoveCallsWithNegativePhase_CallsDontMoved()
        {
            var interviewIds = new[] { 1, 2, 3 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            List<BvCallEntity> calls = FusionLibTestTools.CreateCallsForTest(new[]{ interviews[0], interviews[1] }).ToList();

            calls[0].CallState = (int)PhaseState.ProcessedCall;
            CallQueueService.UpdateCall(calls[0], 0);
            calls[1].CallState = (int)PhaseState.PreparedForPredictiveCall;
            CallQueueService.UpdateCall(calls[1], 0);
            interviews[2].TransientState = NewIts;

            CallTools.MoveCalls(_surveySid, interviewIds, NewIts);

            TestAssert.AreEqual(interviews, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));
            TestAssert.AreEqual(calls, interviewIds.Select(x => CallQueueService.GetCallAndNoLock(_surveySid, x)).Where(x => x!=null));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Bug(39242)]
        public void MoveCalls_LinkedServerIsSeparateMachine_DistributedTransactionIsComplete()
        {
            var interviewIds = new[] { 1 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            _confirmitSurveyDb.ExecuteNonQuery("insert response_control values(1, 1)", CommandType.Text);
            interviews[0].TransientState = NewIts;

            CallTools.MoveCalls(_surveySid, interviewIds, NewIts);

            TestAssert.AreEqual(interviews[0], InterviewRepository.GetById(_surveySid, interviewIds[0]));

            var responceControlTable = _confirmitSurveyDb.ExecuteDataTable<DataTable>("select its from response_control", CommandType.Text);

            TestAssert.AreEqual(interviews[0], InterviewRepository.GetById(_surveySid, interviewIds[0]));
            Assert.AreEqual(1, responceControlTable.Rows.Count, "There is should be 1 record in response_control table");
            Assert.AreEqual(NewIts, (int)responceControlTable.Rows[0]["ITS"]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_CallsAmountIsGreaterThanPortionSize_AllBactesOfCallShouldBeMoved()
        {
            var interviewIds = new[] { 1, 2, 3 };

            List<BvInterviewEntity> interviews = FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds).ToList();
            _confirmitSurveyDb.ExecuteNonQuery("insert response_control values(1, 1), (2,1), (3,1)", CommandType.Text);

            CallTools.MoveCalls(_surveySid, interviewIds, NewIts);

            TestAssert.AreEqual(interviews.Select(x => { x.TransientState = NewIts; return x; }), interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)));

            var responceControlTable = _confirmitSurveyDb.ExecuteDataTable<DataTable>("select its from response_control", CommandType.Text);

            Assert.AreEqual(3, responceControlTable.Rows.Count, "There is should be 1 record in response_control table");
            CollectionAssert.AreEqual(Enumerable.Repeat(NewIts, 3).ToArray(), responceControlTable.Select().Select(x => (int)x["ITS"]).ToArray());
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_SecondportionIsIncorrect_AllBactesOfCallExceptSecondShouldBeMoved()
        {
            const int portionSize = 1;
            var interviewIds = new[] { 1, 2, 3 };
            int retryCount = ServiceLocator.Resolve<IRetryingServiceSettings>().NumberOfRetryAttempts;

            FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds);
            _confirmitSurveyDb.ExecuteNonQuery("insert response_control values(1, 1), (2,1), (3,1)", CommandType.Text);

            var originalICallsManagementService = ServiceLocator.Resolve<ICallsManagementService>();

            var callNumber = 0;
            var stubICallsManagementService = new StubICallsManagementService
            {
                MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32 = (SurveySID, BatchID, StateID) =>
                {
                    ++callNumber;

                    if (callNumber > 1 && callNumber < retryCount + 2)
                    {
                        throw new Exception();
                    }

                    originalICallsManagementService.MoveToIts(
                        SurveySID,
                        BatchID,
                        StateID);
                }
            };
            ServiceLocator.RegisterInstance<ICallsManagementService>(stubICallsManagementService);

            ServiceLocator.Resolve<ISystemSettings>().AsyncOperation.MovePortionSize = portionSize;

            var operationResult = CallTools.MoveCalls(_surveySid, interviewIds, NewIts);

            Assert.AreEqual(AsyncOperationState.PartiallyCompleted, operationResult.State);
            Assert.AreEqual(1, operationResult.FailedItemsCount);
            Assert.AreEqual(2, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)).Count(x => x.TransientState == NewIts));
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void MoveCalls_SecondPortionIsFailedOnce_AllBactesOfCallShouldBeMoved()
        {
            var interviewIds = new[] { 1, 2, 3 };

            FusionLibTestTools.CreateInterviewsForTest(_surveySid, interviewIds);
            _confirmitSurveyDb.ExecuteNonQuery("insert response_control values(1, 1), (2,1), (3,1)", CommandType.Text);

            var originalICallsManagementService = ServiceLocator.Resolve<ICallsManagementService>();

            var callNumber = 0;
            var stubICallsManagementService = new StubICallsManagementService
            {
                MoveToItsNullableOfInt32NullableOfInt32NullableOfInt32 = (SurveySID, BatchID, StateID) =>
                {
                    ++callNumber;

                    if (callNumber == 2)
                    {
                        throw new Exception();
                    }

                    originalICallsManagementService.MoveToIts(
                        SurveySID,
                        BatchID,
                        StateID);
                }
            };
            ServiceLocator.RegisterInstance<ICallsManagementService>(stubICallsManagementService);

            CallTools.MoveCalls(_surveySid, interviewIds, NewIts);

            Assert.AreEqual(3, interviewIds.Select(x => InterviewRepository.GetById(_surveySid, x)).Count(x => x.TransientState == NewIts));
        }
    }
}
