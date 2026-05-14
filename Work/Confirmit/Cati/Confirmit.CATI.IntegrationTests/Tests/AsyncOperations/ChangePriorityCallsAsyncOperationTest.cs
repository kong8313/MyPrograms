using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class ChangePriorityCallsAsyncOperationTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void ChangeInterviewPriority_SelectedCells_FilterOneField_CellPriorityUpdated()
        {
            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            var entity = CallManager.ChangeCallsPriority(survey.Id, 20, new FilteredByCellsBatchParameters(survey.Id, quota.Data.Fields, new[] { new[] { "1" } }));
            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            executor.ExecuteOperationSync(entity);

            context.GetCalls("S1.I1").Assert.IsTrue(x => x.Priority == 20);
            context.GetCalls("S1.I3", "S1.I5").Assert.IsTrue(x => x.Priority == 10);
            context.GetCalls("S1.I2", "S1.I4", "S1.I6").Assert.IsNull();
            TestAssert.ManagementActivityEventExists(ManagementEvent.ChangePriorityOfFilteredByCellsCalls, typeof(ChangePriorityOfFilteredByCellsCallsEvent).Name, survey.Id);

        }

        [TestMethod]
        public void ChangeInterviewPriority_SelectedCells_FilterByTwoFields_CellPriorityUpdated()
        {
            var context = CreateSurveyWithQuota();
            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            var entity = CallManager.ChangeCallsPriority(survey.Id, 20, new FilteredByCellsBatchParameters(survey.Id, quota.Data.Fields, new[] { new[] { "1" }, new[] { "2" } }));
            var executor = ServiceLocator.Resolve<IAsyncOperationExecutor>();

            executor.ExecuteOperationSync(entity);

            context.GetCalls("S1.I1", "S1.I3").Assert.IsTrue(x => x.Priority == 20);
            context.GetCalls("S1.I5").Assert.IsTrue(x => x.Priority == 10);
            context.GetCalls("S1.I2", "S1.I4", "S1.I6").Assert.IsNull();
            TestAssert.ManagementActivityEventExists(ManagementEvent.ChangePriorityOfFilteredByCellsCalls, typeof(ChangePriorityOfFilteredByCellsCallsEvent).Name, survey.Id);
        }

        private TestDataContext CreateSurveyWithQuota()
        {
           return new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){Priority = 10}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=1"},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=2", Call = new CallData(){Priority = 10}},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=2"},
                            new InterviewData() {Tag = "S1.I5", Data = "q1=", Call = new CallData(){Priority = 10}},
                            new InterviewData() {Tag = "S1.I6", Data = "q1="}
                        }
                    }
                }
            }.Create();
        }
    }
}