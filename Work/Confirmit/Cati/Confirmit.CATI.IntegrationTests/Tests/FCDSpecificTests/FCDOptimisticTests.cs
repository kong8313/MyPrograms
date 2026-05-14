using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FCDSpecificTests
{
    [TestClass]
    public class FCDOptimisticTests : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void CloseOptimisticCell_OldFcdBehavior_CellIsNotClosed()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DeleteCalls;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                         Quotas = new [] {
                            new QuotaData{ Id = 1, Name="quota", Fields = new[] {"q1"}, IsOptimistic=true,
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData {Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", Data = "q1=1"},
                            new InterviewData {Tag = "S1.I2", Data = "q1=2", Call = new CallData()},
                            new InterviewData {Tag = "S1.I3", Data = "q1=", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, optimisticallyClosedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1").Assert.IsNull();
            context.GetCalls("S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void CloseOptimisticCell_NewFcdBehavior_CellIsClosed()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
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
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, optimisticallyClosedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void CloseOptimisticallyAndThenClosePessimisticallyCell_NewFcdBehavior_CellIsClosed()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
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
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, optimisticallyClosedCellsIds: new[] { 2 });
            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, pessimisticallyClosedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void ClosePessimisticallyCellWithNullOptimisticParameter_NewFcdBehavior_CellIsClosed()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
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
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, pessimisticallyClosedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void DoubleCloseOptimisticallyCell_NewFcdBehavior_CellIsClosed()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
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
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, optimisticallyClosedCellsIds: new[] { 2 });
            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, optimisticallyClosedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
        }

        [TestMethod]
        public void CloseOptimisticallyAndThenReopenCell_NewFcdBehavior_CellIsOpened()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
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
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, optimisticallyClosedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1", "S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id, pessimisticallyOpenedCellsIds: new[] { 2 });

            context.GetCalls("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I2", "S1.I3").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
        }

        [TestMethod]
        public void ChangeDifferentCellsStates_NewFcdBehavior_CellAreChangedInSeparatedOperationsCorrectly()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData {Name = "q1", Precodes = new[] {"1","2","3","4","5","6","7","8","9","10","11","12","13"}}
                        },
                         Quotas = new [] {
                            new QuotaData{ Id = 1, Name="quota", Fields = new[] {"q1"}, IsOptimistic=true,
                                Cells = new[]
                                {
                                    new CellData {Id = 1, Values="q1=1", Counter=1, Limit=1},
                                    new CellData {Id = 2, Values="q1=2", Counter=1, Limit=1},
                                    new CellData {Id = 3, Values="q1=3", Counter=0, Limit=1},
                                    new CellData {Id = 4, Values="q1=4", Counter=0, Limit=1},
                                    new CellData {Id = 5, Values="q1=5", Counter=0, Limit=1},
                                    new CellData {Id = 6, Values="q1=6", Counter=0, Limit=1},
                                    new CellData {Id = 7, Values="q1=7", Counter=0, Limit=1},
                                    new CellData {Id = 8, Values="q1=8", Counter=0, Limit=1},
                                    new CellData {Id = 9, Values="q1=9", Counter=0, Limit=1},
                                    new CellData {Id = 10, Values="q1=10", Counter=0, Limit=1},
                                    new CellData {Id = 11, Values="q1=11", Counter=0, Limit=1},
                                    new CellData {Id = 12, Values="q1=12", Counter=1, Limit=1},
                                    new CellData(){Id = 13, Values="q1=13", Counter=1, Limit=1}
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=3", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=4", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I5", Data = "q1=5", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I6", Data = "q1=6", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I7", Data = "q1=7", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I8", Data = "q1=8", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I9", Data = "q1=9", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I10", Data = "q1=10", Call = new CallData{CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I11", Data = "q1=11", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I12", Data = "q1=12", Call = new CallData{CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I13", Data = "q1=13", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISystemSettings>().Quotas.MaxQuestionsPerQuota = 14;

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id,
                pessimisticallyOpenedCellsIds: new[] { 1, 4, 7 }, pessimisticallyClosedCellsIds: new[] { 2, 5, 8 },
                optimisticallyClosedCellsIds: new[] { 3, 6, 9 }, optimisticallyOpenedCellsIds: new[] { 10, 11, 12, 13 });

            context.GetCalls("S1.I2", "S1.I3", "S1.I5", "S1.I6", "S1.I8", "S1.I9", "S1.I10", "S1.I12").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);
            context.GetCalls("S1.I1", "S1.I4", "S1.I7", "S1.I11", "S1.I13").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);


            //launch, add sample and 9 fcd operations, no fcd operations for optimistically opened cells
            Assert.AreEqual(12, BvAsyncOperationQueueAdapter.GetAll().Count);
        }

        [TestMethod]
        public void ChangeDifferentCellsStates_NewFcdBehavior_CellAreChangedInSingleOperationCorrectly()
        {
            ServiceLocator.Resolve<ISystemSettings>().FCD.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData() { Tag = "S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2","3","4","5","6","7","8","9","10","11","12"}}
                        },
                         Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota", Fields = new[] {"q1"}, IsOptimistic = true,
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=1, Limit=1},
                                    new CellData(){Id = 3, Values="q1=3", Counter=1, Limit=1},
                                    new CellData(){Id = 4, Values="q1=4", Counter=0, Limit=1},
                                    new CellData(){Id = 5, Values="q1=5", Counter=1, Limit=1},
                                    new CellData(){Id = 6, Values="q1=6", Counter=1, Limit=1},
                                    new CellData(){Id = 7, Values="q1=7", Counter=0, Limit=1},
                                    new CellData(){Id = 8, Values="q1=8", Counter=1, Limit=1},
                                    new CellData(){Id = 9, Values="q1=9", Counter=1, Limit=1},
                                    new CellData(){Id = 10, Values="q1=10", Counter=0, Limit=1},
                                    new CellData(){Id = 11, Values="q1=11", Counter=0, Limit=1},
                                    new CellData(){Id = 12, Values="q1=12", Counter=0, Limit=1}
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=2", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=3", Call = new CallData(){CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=4", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I5", Data = "q1=5", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I6", Data = "q1=6", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I7", Data = "q1=7", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I8", Data = "q1=8", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I9", Data = "q1=9", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I10", Data = "q1=10", Call = new CallData{CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I11", Data = "q1=11", Call = new CallData{CallState = (int)CallState.DisabledByFCD}},
                            new InterviewData() {Tag = "S1.I12", Data = "q1=12", Call = new CallData{CallState = (int)CallState.DisabledByFCD}}
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISystemSettings>().Quotas.MaxQuestionsPerQuota = 1;

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota");

            quota.ChangeQuotaCellsStates(survey.Data.ProjectId, quota.Data.Id,
                pessimisticallyOpenedCellsIds: new[] { 1, 4, 7 }, pessimisticallyClosedCellsIds: new[] { 2, 5, 8 },
                optimisticallyClosedCellsIds: new[] { 3, 6, 9 }, optimisticallyOpenedCellsIds: new[] { 10, 11, 12 });


            context.GetCalls("S1.I2", "S1.I3", "S1.I5", "S1.I6", "S1.I8", "S1.I9").Assert.IsTrue(x => x.CallState == (int)CallState.DisabledByFCD);

            context.GetCalls("S1.I1", "S1.I4", "S1.I7", "S1.I10", "S1.I11", "S1.I12").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);

            //launch, add sample and 1 fcd operation
            Assert.AreEqual(4, BvAsyncOperationQueueAdapter.GetAll().Count);
        }
    }
}
