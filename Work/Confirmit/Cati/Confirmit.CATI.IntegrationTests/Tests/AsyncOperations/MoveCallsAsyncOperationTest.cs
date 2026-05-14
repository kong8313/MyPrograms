using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class MoveCallsAsyncOperationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        private BackendTools _backendTools;
      
        [TestInitialize]
        public  void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void MoveCalls_OneInterviewWithoutCall_3CallHistoryRecordsCreated()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                { 
                    new SurveyData 
                    { Tag="S1", IsUseDb = true,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){Tag="S1.I3", ITS=CallOutcome.FreshSample}
                        }
                   }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var operation = CallManager.MoveCalls(survey.Id, 31, new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
            operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);

            // assert
            Assert.AreEqual(3, operation.ProcessedItemsCount);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId ORDER BY ID", new SqlParameter("@SurveyId", survey.Id));

            Assert.AreEqual(3, history.Count);

            TestCallHistoryRecord(history, operation, context.GetInterview("S1.I1").Id, (short)CallState.Scheduled);
            TestCallHistoryRecord(history, operation, context.GetInterview("S1.I2").Id, (short)CallState.Scheduled);
            TestCallHistoryRecord(history, operation, context.GetInterview("S1.I3").Id, null);
        }

        [TestMethod, Owner(@"FIRM\LeonidS")]
        public void MoveCalls_AllInterviewsWithCalls_3CallHistoryRecordsCreated()
        {
            // arrange
            var context = new TestData
            {
                Surveys = new[]
                { 
                    new SurveyData 
                    { Tag="S1", IsUseDb = true,
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData(){Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData()}
                        }
                   }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var operation = CallManager.MoveCalls(survey.Id, 31, new SelectedBatchParameters(context.GetInterviews("S1.I1", "S1.I2", "S1.I3").Select(x => x.Id)));

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);
            operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);

            // assert
            Assert.AreEqual(3, operation.ProcessedItemsCount);

            var history = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId", new SqlParameter("@SurveyId", survey.Id));

            Assert.AreEqual(3, history.Count);

            TestCallHistoryRecord(history, operation, context.GetInterview("S1.I1").Id, (short)CallState.Scheduled);
            TestCallHistoryRecord(history, operation, context.GetInterview("S1.I2").Id, (short)CallState.Scheduled);
            TestCallHistoryRecord(history, operation, context.GetInterview("S1.I3").Id, (short)CallState.Scheduled);
        }

        private void TestCallHistoryRecord(IEnumerable<BvCallHistoryExEntity> history, BvAsyncOperationQueueEntity operation,  int interviewId, short? callState)
        {
            var record = history.Single(x => x.InterviewID == interviewId);

            Assert.AreEqual(operation.Id, record.OperationId);
            Assert.AreEqual((int)OperationType.MoveCallsToIts, (int)record.OperationType);
            Assert.AreEqual(record.CallState, callState);
            
        }
    }
}
