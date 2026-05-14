using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.SystemSettings;
using System.Data.SqlClient;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class RespondentsSynchronizationAsyncOperationTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void RespondentsSynchronization_10InterviewsSynchronized_RespondentsDeleted_InterviewsSynchronized()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData { Tag="S1", IsUseDb = true}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var missingRespondentsNumber = 10;
            var extendedStatus = (int)CallOutcome.SynchronizedSample;
            var respIds = new List<int>();

            for (int i = 0; i < missingRespondentsNumber; i++)
            {
                var respId = survey.Database.CreateRespondent(0, extendedStatus.ToString(), new InterviewData() { RespondentName = $"respName_{i}", TelephoneNumber = $"123450{i}" });
                respIds.Add(respId);
            }

            // only respondents was added and no new interviews
            Assert.AreEqual(0, BvInterviewAdapter.GetAll().Count);

            var operation = SyncRespondents(survey);

            Assert.AreEqual((byte)AsyncOperationState.Completed, operation.State);

            var interviews = BvInterviewAdapter.GetAll();
            Assert.AreEqual(missingRespondentsNumber, interviews.Count);

            var replicationService = ServiceLocator.Resolve<IReplicationService>();

            foreach (var respId in respIds)
            {
                Assert.IsTrue(interviews.Any(i => i.ID == respId && i.TransientState == extendedStatus));
                Assert.AreEqual(1, replicationService.GetNumberOfReplicationRecords(survey.Model.Name, respId));
            }

            //delete and repeat sync process
            survey.Database.DeleteRespondent(respIds[0]);
            operation = SyncRespondents(survey);
            interviews = BvInterviewAdapter.GetAll();
            Assert.AreEqual(missingRespondentsNumber-1, interviews.Count);
            foreach (var interview in interviews)
            {
                Assert.IsTrue(respIds.Any(id=>id == interview.ID));
            }
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void RespondentsSynchronization_BvInterviewQuotaCellsPopulated()
        {
            var context = new TestData
            {
                Surveys = new[]
                 {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"}, IsOptimistic=true,
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var missingRespondentsNumber = 10;
            var extendedStatus = (int)CallOutcome.SynchronizedSample;
            var respIds = new List<int>();

            for (int i = 0; i < missingRespondentsNumber; i++)
            {
                var respId = survey.Database.CreateRespondent(0, extendedStatus.ToString(), new InterviewData() { RespondentName = $"respName_{i}", TelephoneNumber = $"123450{i}" });
                survey.Database.SetInterviewData(respId, $"q1={i}");
                respIds.Add(respId);
            }

            // only respondents was added and no new interviews
            Assert.AreEqual(0, BvInterviewAdapter.GetAll().Count);

            var title = $"Synchronize respondents for survey \"{survey.Model.Name}\" ({survey.Model.Description})";

            var parameters = new Core.AsyncOperations.Operations.SynchronizeRespondents.Parameters
            {
                SurveyId = survey.Id
            };

            var operation = StartAsyncOperation(parameters, title);

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);

            operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);

            Assert.AreEqual((byte)AsyncOperationState.Completed, operation.State);
            Assert.AreEqual(missingRespondentsNumber, BvInterviewAdapter.GetAll().Count);

            var interviews = BvInterviewAdapter.GetAll();
            var replicationService = ServiceLocator.Resolve<IReplicationService>();

            foreach (var respId in respIds)
            {
                Assert.IsTrue(interviews.Any(i => i.ID == respId && i.TransientState == extendedStatus));
                Assert.AreEqual(1, replicationService.GetNumberOfReplicationRecords(survey.Model.Name, respId));
            }


            var query = "SurveyId = @SurveyId AND QuotaId = @QuotaId";
            var interviewQuotaCells = BvInterviewQuotaCellAdapter.GetByCondition(query, new SqlParameter[] {
                new SqlParameter("SurveyId", survey.Id),
                new SqlParameter("QuotaId", 1)
            });

            Assert.AreEqual(missingRespondentsNumber, interviewQuotaCells.Count);
            Assert.AreEqual(1, interviewQuotaCells.Where(x => x.CellID == 1).Count());
            Assert.AreEqual(1, interviewQuotaCells.Where(x => x.CellID == 2).Count());
            Assert.AreEqual(missingRespondentsNumber - 2, interviewQuotaCells.Where(x => x.CellID == -1).Count());
        }

        private static BvAsyncOperationQueueEntity SyncRespondents(SurveyController survey)
        {
            var title = $"Synchronize respondents for survey \"{survey.Model.Name}\" ({survey.Model.Description})";

            var parameters = new Core.AsyncOperations.Operations.SynchronizeRespondents.Parameters
            {
                SurveyId = survey.Id
            };

            var operation = StartAsyncOperation(parameters, title);

            // act
            ServiceLocator.Resolve<IAsyncOperationExecutor>().ExecuteOperationSync(operation);

            operation = ServiceLocator.Resolve<IAsyncOperationRepository>().Get(operation.Id);

            return operation;
        }

        private static BvAsyncOperationQueueEntity StartAsyncOperation(IAsyncOperationParameters parameters, string title)
        {
            var supervisorName = ServiceLocator.Resolve<ISupervisorNameProvider>().Name;

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var operationEntity = ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                callCenterId,
                title,
                false,
                parameters,
                AsyncOperationConstants.HighPriority,
                supervisorName);

            return operationEntity;
        }
    }
}
