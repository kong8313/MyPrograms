using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.AsyncOperations
{
    [TestClass]
    public class FcdAsyncOperationsPrioritiesTests : BaseMockedIntegrationTest
    {
        private IAsyncOperationQueue _asyncOperationQueue;
        private IAsyncOperationRepository _asyncOperationRepository;
        private IFcdQuotaService _fcdQuotaService;
        private IFCDSettings _fcdSettings;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();

            _asyncOperationQueue = ServiceLocator.Resolve<IAsyncOperationQueue>();
            _asyncOperationRepository = ServiceLocator.Resolve<IAsyncOperationRepository>();
            _fcdQuotaService = ServiceLocator.Resolve<IFcdQuotaService>();
            _fcdSettings = ServiceLocator.Resolve<IFCDSettings>();
        }

        private void UpdateAsyncOperationStatus(int id)
        {
            var completedOperation = _asyncOperationRepository.Get(id);
            completedOperation.State = (byte)AsyncOperationState.Completed;
            _asyncOperationRepository.Update(completedOperation);
        }

        [TestMethod, Owner("YahorS")]
        public void DeleteRespondentsOperationHasHigherPriorityThanDeleteCallsOperationProducingByCloseQuotaCellOperation()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag="S1", IsUseDb = true,
                        Forms = new[]{
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3", "4"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                    new CellData(){Id = 3, Values="q1=3", Counter=0, Limit=1},
                                    new CellData(){Id = 4, Values="q1=4", Counter=0, Limit=1}
                                }
                            }
                        }
                    }
               }
            }.Create();

            var survey = context.GetSurvey("S1");

            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyClosed);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 2, QuotaCellState.PessimisticallyClosed);
            new ManagementService().DeleteRespondentsAsync(new[] { 1 }, survey.Data.ProjectId);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 3, QuotaCellState.PessimisticallyClosed);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 4, QuotaCellState.PessimisticallyClosed);
            
            var queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeleteRespondents, queuedOperation.Type);
            UpdateAsyncOperationStatus(queuedOperation.Id);

            queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeactivateCalls, queuedOperation.Type);
            UpdateAsyncOperationStatus(queuedOperation.Id);
        }

        [TestMethod, Owner("YahorS")]
        public void DeleteRespondentsOperationHasHigherPriorityThanDisableCallsOperationProducingByCloseQuotaCellOperation()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag="S1", IsUseDb = true,
                        Forms = new[]{
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3", "4"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                    new CellData(){Id = 3, Values="q1=3", Counter=0, Limit=1},
                                    new CellData(){Id = 4, Values="q1=4", Counter=0, Limit=1}
                                }
                            }
                        }
                    }
               }
            }.Create();

            var survey = context.GetSurvey("S1");

            _fcdSettings.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;

            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyClosed);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 2, QuotaCellState.PessimisticallyClosed);
            new ManagementService().DeleteRespondentsAsync(new[] { 1 }, survey.Data.ProjectId);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 3, QuotaCellState.PessimisticallyClosed);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 4, QuotaCellState.PessimisticallyClosed);
            
            var queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeleteRespondents, queuedOperation.Type);
            UpdateAsyncOperationStatus(queuedOperation.Id);

            queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.EnableCalls, queuedOperation.Type);
            Assert.AreEqual("Disable calls for closed quota \"quota1\" cell \"q1=1\"", queuedOperation.Title);
        }

        [TestMethod, Owner("YahorS")]
        public void DeleteRespondentsOperationHasHigherPriorityThanEnableCallsOperationProducingByOpenQuotaCellOperation()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData{ Tag="S1", IsUseDb = true,
                        Forms = new[]{
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3", "4"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                    new CellData(){Id = 3, Values="q1=3", Counter=0, Limit=1},
                                    new CellData(){Id = 4, Values="q1=4", Counter=0, Limit=1}
                                }
                            }
                        }
                    }
               }
            }.Create();

            var survey = context.GetSurvey("S1");

            _fcdSettings.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyOpened);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 2, QuotaCellState.PessimisticallyOpened);
            new ManagementService().DeleteRespondentsAsync(new[] { 1 }, survey.Data.ProjectId);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 3, QuotaCellState.PessimisticallyOpened);
            _fcdQuotaService.OnQuotaCellChanged(survey.Id, 1, 4, QuotaCellState.PessimisticallyOpened);

            var queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeleteRespondents, queuedOperation.Type);
            UpdateAsyncOperationStatus(queuedOperation.Id);

            queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.UpdateFcdQuota, queuedOperation.Type);
            Assert.AreEqual("Update FCD state of calls for opened quota \"quota1\" cell \"q1=1\"", queuedOperation.Title);
        }

        [TestMethod, Owner("YahorS")]
        public void DeleteRespondentsOperationHasHigherPriorityThanDeleteCallsOperationProducingByUpdateQuotaOperation()
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
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 3, Name="quota3", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 4, Name="quota4", Fields = new[] {"q1"}, 
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

            _fcdQuotaService.OnQuotaUpdate(survey.Id, 1);
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 2);
            new ManagementService().DeleteRespondentsAsync(new[] { 1 }, survey.Data.ProjectId);
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 3);
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 4);

            var queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeleteRespondents, queuedOperation.Type);
            UpdateAsyncOperationStatus(queuedOperation.Id);

            queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeactivateCalls, queuedOperation.Type);
            Assert.AreEqual("Delete calls for closed quota \"quota1\" cells", queuedOperation.Title);
        }

        [TestMethod, Owner("YahorS")]
        public void DeleteRespondentsOperationHasHigherPriorityThanUpdateStatusOfCallsOperationProducingByUpdateQuotaOperation()
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
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 3, Name="quota3", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=1},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            },
                            new QuotaData(){ Id = 4, Name="quota4", Fields = new[] {"q1"},
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

            _fcdSettings.AlgorithmType = FcdAlgorithmType.DisableCallsWithReenabling;
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 1);
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 2);
            new ManagementService().DeleteRespondentsAsync(new[] { 1 }, survey.Data.ProjectId);
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 3);
            _fcdQuotaService.OnQuotaUpdate(survey.Id, 4);

            var queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.DeleteRespondents, queuedOperation.Type);
            UpdateAsyncOperationStatus(queuedOperation.Id);

            queuedOperation = _asyncOperationQueue.Dequeue();
            Assert.AreEqual((int)OperationTypes.UpdateFcdQuota, queuedOperation.Type);
            Assert.AreEqual("Update status of calls on quota \"quota1\" update", queuedOperation.Title);
        }
    }
}
