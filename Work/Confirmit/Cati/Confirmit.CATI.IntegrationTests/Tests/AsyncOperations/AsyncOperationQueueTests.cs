using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class AsyncOperationQueueTests : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\MaximL")]
        public void Dequeue_AsyncOperation_AsyncOperationHaveCorrectAttributes()
        {
            var time = DateTime.Parse("2010.10.10T10:10:11");
            var title = "operation title";
            var supervisorName = "surpervisor";

            var context = new TestData()
            {
                Surveys = new[] {new SurveyData() {Tag = "S1"}}
            }.Create();

            ServiceLocator.RegisterInstance<ITimeService>(new TestTimeService(time));
            
            var parameters = new Core.AsyncOperations.Operations.CallsManagementOperations.MoveCalls.Parameters
            {
                SurveyId = context.GetSurvey("S1").Id,
                BatchParameters = new SelectedBatchParameters(new[]{1}),
                StateId = 1,
            };

            var queue = ServiceLocator.Resolve<IAsyncOperationQueue>();

            queue.Enqueue(1, title,
                 false,
                 parameters,
                 AsyncOperationConstants.NormalPriority,
                 supervisorName);

            var operation = queue.Dequeue();

            Assert.AreEqual(time, operation.StartedDate);
            Assert.AreEqual(time, operation.HeartBeat);
            Assert.AreEqual(title, operation.Title);
            Assert.AreEqual(supervisorName, operation.CreatedBySupervisorName);
        }
        [Ignore]//skip because it does not work like that anymore 
        [TestMethod, Owner("YahorS")]
        public void AsyncOperationStartsNewAsyncOperationWhenCurrentIsFinished()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag="S1", IsUseDb = true,
                        Forms = new[]{
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                    }
               }
            }.Create();

            var survey = context.GetSurvey("S1");

            var quotaService = ServiceLocator.Resolve<IFcdQuotaService>();
            quotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyClosed);
            quotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyClosed);
            quotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyClosed);

            // Because there are 3 operations i the queue all them must be executed after a single call to the DequeueAndExecute
            var asyncOperationExecutor = ServiceLocator.Resolve<IAsyncOperationExecutor>();
            asyncOperationExecutor.DequeueAndExecute();

            var asyncOperationRepository = ServiceLocator.Resolve<IAsyncOperationRepository>();
            foreach (var bvAsyncOperationQueueEntity in asyncOperationRepository.GetAll())
            {
                Assert.AreEqual((int)AsyncOperationState.Completed, bvAsyncOperationQueueEntity.State);
            }
        }
    }
}
