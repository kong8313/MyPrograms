using System;
using System.Collections.Generic;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Handmade.Entity;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using System.Data;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation.Fakes;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Core.WcfServices.Clients.Fakes;

namespace Confirmit.CATI.Supervisor.Core.UnitTests
{
    [TestClass]
    public class QuotaManagerTest
    {
        [TestInitialize]
        public void Init()
        {
            var backendInstance = new BackendInstance();
            BackendInstance.Current = backendInstance;

            InitializeServiceLocation();
        }

        [TestCleanup]
        public void Cleanup()
        {
            BackendInstance.Current = null;

            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        private void InitializeServiceLocation()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();

            var registry = ServiceLocator.Resolve<IServiceRegistrator>();
            var quotaInfo = new StubIQuotaInfoService
            {
                GetQuotaFieldsInt32Int32 = (a, b) => new[] { "q1" }
            };

            registry.RegisterSingleton<IQuotaInfoService>(quotaInfo);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void IsQuotaSynchronized_DifferentTargets_LimitsWereChanged()
        {
            var designQuota =
                new QuotaList {QuotaRows = new[] {
                    new QuotaRow {Target = 1, Priority = QuotaLimitPriority.Medium },
                    new QuotaRow {Target = 3, Priority = QuotaLimitPriority.Medium}}};

            var productionQuota =
                new QuotaList {QuotaRows = new[] {new QuotaRow {Target = 1}, new QuotaRow {Target = 2 } }};

            IAuthoringService authoringService = ServiceLocator.Resolve<IAuthoringService>();
            IAuthoringService authoringServiceStub = new StubIAuthoringService
            {
                Inner = authoringService,
                GetQuotaListStringStringQuotaMode = (projectId, quotaName, quotaMode) => 
                {
                    if(projectId == string.Empty && quotaName == string.Empty && quotaMode == QuotaMode.DesignWithProductionCounter)
                    {
                        return designQuota;
                    }

                    if (projectId == string.Empty && quotaName == string.Empty && quotaMode == QuotaMode.Production)
                    {
                        return productionQuota;
                    }

                    return authoringService.GetQuotaList(projectId, quotaName, quotaMode);
                }
            };
            ServiceLocator.RegisterInstance(authoringServiceStub);

            var quotaState = QuotaManager.GetQuotaState(string.Empty, string.Empty);

            Assert.AreEqual(QuotaSyncState.Synchronized, quotaState);
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void IsQuotaSynchronized_DifferentCellsCount_NotSynchronized()
        {
            var designQuota =
                new QuotaList {QuotaRows = new[] {new QuotaRow {Target = 1}, new QuotaRow {Target = 3}}};

            var productionQuota =
                new QuotaList
                    {
                        QuotaRows =
                            new[] {new QuotaRow {Target = 1}, new QuotaRow {Target = 3}, new QuotaRow {Target = 2}}
                    };

            IAuthoringService authoringService = ServiceLocator.Resolve<IAuthoringService>();
            IAuthoringService authoringServiceStub = new StubIAuthoringService
            {
                Inner = authoringService,
                GetQuotaListStringStringQuotaMode = (projectId, quotaName, quotaMode) => 
                {
                    if(projectId == string.Empty && quotaName == string.Empty && quotaMode == QuotaMode.DesignWithProductionCounter)
                    {
                        return designQuota;
                    }

                    if (projectId == string.Empty && quotaName == string.Empty && quotaMode == QuotaMode.Production)
                    {
                        return productionQuota;
                    }

                    return authoringService.GetQuotaList(projectId, quotaName, quotaMode);
                }
            };
            ServiceLocator.RegisterInstance(authoringServiceStub);

            var quotaState = QuotaManager.GetQuotaState(string.Empty, string.Empty);

            Assert.AreEqual(QuotaSyncState.NotSynchronized, quotaState);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void IsQuotaSynchronized_SameQuotas_Synchronized()
        {
            var designQuota =
                new QuotaList
                {
                    QuotaRows =new[] {
                            new QuotaRow { Target = 1, Priority = QuotaLimitPriority.Medium },
                            new QuotaRow { Target = 3, Priority = QuotaLimitPriority.Medium },
                        new QuotaRow { Target = 2, Priority = QuotaLimitPriority.Medium } }
                };

            var productionQuota =
                new QuotaList
                {
                    QuotaRows =
                        new[] { new QuotaRow { Target = 1 },
                            new QuotaRow { Target = 3 },
                            new QuotaRow { Target = 2} }
                };

            IAuthoringService authoringService = ServiceLocator.Resolve<IAuthoringService>();
            IAuthoringService authoringServiceStub = new StubIAuthoringService
            {
                Inner = authoringService,
                GetQuotaListStringStringQuotaMode = (projectId, quotaName, quotaMode) => 
                {
                    if(projectId == string.Empty && quotaName == string.Empty && quotaMode == QuotaMode.DesignWithProductionCounter)
                    {
                        return designQuota;
                    }

                    if (projectId == string.Empty && quotaName == string.Empty && quotaMode == QuotaMode.Production)
                    {
                        return productionQuota;
                    }

                    return authoringService.GetQuotaList(projectId, quotaName, quotaMode);
                }
            };
            ServiceLocator.RegisterInstance(authoringServiceStub);

            var quotaState = QuotaManager.GetQuotaState(string.Empty, string.Empty);

            Assert.AreEqual(QuotaSyncState.Synchronized, quotaState);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetQuota_3FieldsAnd3Cells_ValidResults()
        {
            var designQuota =
                new QuotaList
                    {
                        QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, FieldPrecodes = new[] {"1","1","1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 1, FieldPrecodes = new[] {"2","1","1"}, QuotaRowId = 2},
                                    new QuotaRow{Target = 30, Counter = 1, FieldPrecodes = new[] {"1","2","1"}, QuotaRowId = 3}
                                },
                        FieldNames = new[] {"q1", "q2", "q3"},
                        QuotaFullEmailAddress = "123",
                        QuotaId = 123,
                        QuotaName = "quota1"
                    };

            var q11 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a11" } } };
            var q12 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a12" } } };

            var q21 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a21" } } };
            var q22 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a22" } } };

            var q31 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a31" } } };
            var q32 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a32" } } };

            var q1 =
                new SingleForm
                    {
                        Name = "q1",
                        SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] {q11, q12} },
                        FormTexts = new[] {new FormText {Title = "t1"}}
                    };

            var q2 =
                new SingleForm
                    {
                        Name = "q2",
                        SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] {q21, q22} },
                        FormTexts = new[] {new FormText {Title = "t2"}}
                    };

            var q3 =
                new SingleForm
                    {
                        Name = "q3",
                        SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] {q31, q32} },
                        FormTexts = new[] {new FormText {Title = "t3"}}
                    };

            var fields = new List<SingleForm>( new[] { q1, q2, q3 });
            var parameters = QuotaManager.GetExtraCounterParameters(
                ExtraQuotaCounterTypes.None, 0, designQuota.QuotaId, false, new int[] { });
            var aggregateQuotaViewAdditionalColumnsBuilder = new AdditionalColumnsBuilderFactory().Create(designQuota.IsOptimistic, false, true, parameters);

            DataTable result = QuotaManager.CreateQuotaDataTable(fields, aggregateQuotaViewAdditionalColumnsBuilder);
            QuotaManager.FillQuotaDataTable(result, designQuota, fields, aggregateQuotaViewAdditionalColumnsBuilder);

            Assert.AreEqual(designQuota.FieldNames.Length * 2 + 7, result.Columns.Count); // 7 additional columns.
            Assert.AreEqual(designQuota.QuotaRows.Length, result.Rows.Count);

            Assert.AreEqual("a11", result.Rows[0]["q1"]);
            Assert.AreEqual("a21", result.Rows[0]["q2"]);
            Assert.AreEqual("a31", result.Rows[0]["q3"]);
            Assert.AreEqual(10, result.Rows[0]["Limit"]);
            Assert.AreEqual(1, result.Rows[0]["Counter"]);
            Assert.AreEqual("Medium", result.Rows[0][QuotaManager.Priority]);

            Assert.AreEqual("a12", result.Rows[1]["q1"]);
            Assert.AreEqual("a21", result.Rows[1]["q2"]);
            Assert.AreEqual("a31", result.Rows[1]["q3"]);
            Assert.AreEqual(20, result.Rows[1]["Limit"]);
            Assert.AreEqual(1, result.Rows[1]["Counter"]);
            Assert.AreEqual("Medium", result.Rows[1][QuotaManager.Priority]);

            Assert.AreEqual("a11", result.Rows[2]["q1"]); 
            Assert.AreEqual("a22", result.Rows[2]["q2"]); 
            Assert.AreEqual("a31", result.Rows[2]["q3"]);
            Assert.AreEqual(30, result.Rows[2]["Limit"]);
            Assert.AreEqual(1, result.Rows[2]["Counter"]);
            Assert.AreEqual("Medium", result.Rows[2][QuotaManager.Priority]);
        }

        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetOptimisticQuotaWithScheduledExtraCountersAndBalancinfPriority_1FieldsAnd2Cells_ValidResults()
        {
            var designQuota =
                new QuotaList
                    {
                        QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 19, Counter = 10, LiveTarget = 15, LiveCounter = 12, FieldPrecodes = new[] {"1"}, QuotaRowId = 1, Priority = QuotaLimitPriority.High},
                                    new QuotaRow{Target = 29, Counter = 20, LiveTarget = 25, LiveCounter = 22, FieldPrecodes = new[] {"2"}, QuotaRowId = 2, Priority = QuotaLimitPriority.Disabled}
                                },
                        FieldNames = new[] {"q1"},
                        QuotaFullEmailAddress = "123",
                        QuotaId = 123,
                        QuotaName = "quota1",
                        IsOptimistic = true,
                    };

            var q11 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a11" } } };
            var q12 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a12" } } };

            var q1 =
                new SingleForm
                    {
                        Name = "q1",
                        SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] {q11, q12} },
                        FormTexts = new[] {new FormText {Title = "t1"}}
                    };

            var extraCounters =
                (IEnumerable<QuotaCellCounter>)
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1", Value = 11 },
                        new QuotaCellCounter { Descriptor = "2", Value = 21 }
                    };

            IExtraQuotaCounterCalculator extraQuotaCounterCalculator = new StubIExtraQuotaCounterCalculator
            {
                GetCellCounter = () => extraCounters
            };

            IExtraQuotaCounterService extraQuotaCounterService = new StubIExtraQuotaCounterService
            {
                CreateIExtraQuotaCounterParameters = param => extraQuotaCounterCalculator
            };
            ServiceLocator.RegisterInstance(extraQuotaCounterService);
           
            var fields = new List<SingleForm>( new[] { q1 });
            var parameters = QuotaManager.GetExtraCounterParameters(
                    ExtraQuotaCounterTypes.Scheduled, 0, designQuota.QuotaId, false, new int[] { });
            var aggregateQuotaViewAdditionalColumnsBuilder = new AdditionalColumnsBuilderFactory().Create(designQuota.IsOptimistic, false, true, parameters);

            DataTable result = QuotaManager.CreateQuotaDataTable(fields, aggregateQuotaViewAdditionalColumnsBuilder);
            QuotaManager.FillQuotaDataTable(result, designQuota, fields, aggregateQuotaViewAdditionalColumnsBuilder);

            Assert.AreEqual(designQuota.FieldNames.Length * 2 + 10, result.Columns.Count); // 10 additional columns.
            Assert.AreEqual(designQuota.QuotaRows.Length, result.Rows.Count);

            Assert.AreEqual("a11", result.Rows[0]["q1"]);
            Assert.AreEqual(19, result.Rows[0]["Limit"]);
            Assert.AreEqual(10, result.Rows[0]["Counter"]);
            Assert.AreEqual(15, result.Rows[0]["OptimisticTotalLimit"]);
            Assert.AreEqual(12, result.Rows[0]["InProgress"]);
            Assert.AreEqual(11, result.Rows[0]["ExtraCounter"]);
            Assert.AreEqual("High", result.Rows[0][QuotaManager.Priority]);

            Assert.AreEqual("a12", result.Rows[1]["q1"]);
            Assert.AreEqual(29, result.Rows[1]["Limit"]);
            Assert.AreEqual(20, result.Rows[1]["Counter"]);
            Assert.AreEqual(25, result.Rows[1]["OptimisticTotalLimit"]);
            Assert.AreEqual(22, result.Rows[1]["InProgress"]);
            Assert.AreEqual(21, result.Rows[1]["ExtraCounter"]);
            Assert.AreEqual("No Balancing", result.Rows[1][QuotaManager.Priority]);
        }
        
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetOptimisticQuota_1FieldsAnd2Cells_ValidResults()
        {
            var designQuota =
                new QuotaList
                {
                    QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"2"}, QuotaRowId = 2}
                                },
                    FieldNames = new[] { "q1" },
                    QuotaFullEmailAddress = "123",
                    QuotaId = 123,
                    QuotaName = "quota1",
                    IsOptimistic = true,
                };

            var q11 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a11" } } };
            var q12 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a12" } } };

            var q1 =
                new SingleForm
                {
                    Name = "q1",
                    SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { q11, q12 } },
                    FormTexts = new[] { new FormText { Title = "t1" } }
                };

            var fields = new List<SingleForm>(new[] { q1 });
            var parameters = QuotaManager.GetExtraCounterParameters(
                ExtraQuotaCounterTypes.None, 0, designQuota.QuotaId, false, new int[] { });
            var aggregateQuotaViewAdditionalColumnsBuilder = new AdditionalColumnsBuilderFactory().Create(designQuota.IsOptimistic, false, false, parameters);

            DataTable result = QuotaManager.CreateQuotaDataTable(fields, aggregateQuotaViewAdditionalColumnsBuilder);
            QuotaManager.FillQuotaDataTable(result, designQuota, fields, aggregateQuotaViewAdditionalColumnsBuilder);

            Assert.AreEqual(designQuota.FieldNames.Length * 2 + 8, result.Columns.Count); // 8 additional columns.
            Assert.AreEqual(designQuota.QuotaRows.Length, result.Rows.Count);

            Assert.AreEqual("a11", result.Rows[0]["q1"]);
            Assert.AreEqual(10, result.Rows[0]["Limit"]);
            Assert.AreEqual(1, result.Rows[0]["Counter"]);
            Assert.AreEqual(15, result.Rows[0]["OptimisticTotalLimit"]);
            Assert.AreEqual(2, result.Rows[0]["InProgress"]);

            Assert.AreEqual("a12", result.Rows[1]["q1"]);
            Assert.AreEqual(20, result.Rows[1]["Limit"]);
            Assert.AreEqual(2, result.Rows[1]["Counter"]);
            Assert.AreEqual(25, result.Rows[1]["OptimisticTotalLimit"]);
            Assert.AreEqual(4, result.Rows[1]["InProgress"]);
        }

        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void GetQuotaWithSpecificByITSExtraCounters_1FieldsAnd2Cells_ValidResults()
        {
            var designQuota =
                new QuotaList
                {
                    QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 19, Counter = 10, LiveTarget = 15, LiveCounter = 12, FieldPrecodes = new[] {"1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 29, Counter = 20, LiveTarget = 25, LiveCounter = 22, FieldPrecodes = new[] {"2"}, QuotaRowId = 2}
                                },
                    FieldNames = new[] { "q1" },
                    QuotaFullEmailAddress = "123",
                    QuotaId = 123,
                    QuotaName = "quota1",
                    IsOptimistic = true,
                };

            var q11 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a11" } } };
            var q12 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a12" } } };

            var q1 =
                new SingleForm
                {
                    Name = "q1",
                    SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { q11, q12 } },
                    FormTexts = new[] { new FormText { Title = "t1" } }
                };

            var extraCounters =
                (IEnumerable<QuotaCellCounter>)
                new[]
                    {
                        new QuotaCellCounter { Descriptor = "1", Value = 11 },
                        new QuotaCellCounter { Descriptor = "2", Value = 21 }
                    };

            IExtraQuotaCounterCalculator extraQuotaCounterCalculator = new StubIExtraQuotaCounterCalculator
            {
                GetCellCounter = () => extraCounters
            };

            IExtraQuotaCounterService extraQuotaCounterService = new StubIExtraQuotaCounterService
            {
                CreateIExtraQuotaCounterParameters = param => extraQuotaCounterCalculator
            };
            ServiceLocator.RegisterInstance(extraQuotaCounterService);

            var fields = new List<SingleForm>(new[] { q1 });
            var parameters = QuotaManager.GetExtraCounterParameters(
                ExtraQuotaCounterTypes.InterviewsWithSpecificStatuses, 0, designQuota.QuotaId, false, new int[] { });
            var aggregateQuotaViewAdditionalColumnsBuilder = new AdditionalColumnsBuilderFactory().Create(designQuota.IsOptimistic, false, false, parameters);

            DataTable result = QuotaManager.CreateQuotaDataTable(fields, aggregateQuotaViewAdditionalColumnsBuilder);
            QuotaManager.FillQuotaDataTable(result, designQuota, fields, aggregateQuotaViewAdditionalColumnsBuilder);

            Assert.AreEqual(designQuota.FieldNames.Length * 2 + 9, result.Columns.Count); // 9 additional columns.
            Assert.AreEqual(designQuota.QuotaRows.Length, result.Rows.Count);

            Assert.AreEqual("a11", result.Rows[0]["q1"]);
            Assert.AreEqual(19, result.Rows[0]["Limit"]);
            Assert.AreEqual(10, result.Rows[0]["Counter"]);
            Assert.AreEqual(15, result.Rows[0]["OptimisticTotalLimit"]);
            Assert.AreEqual(12, result.Rows[0]["InProgress"]);
            Assert.AreEqual(11, result.Rows[0]["ExtraCounter"]);

            Assert.AreEqual("a12", result.Rows[1]["q1"]);
            Assert.AreEqual(29, result.Rows[1]["Limit"]);
            Assert.AreEqual(20, result.Rows[1]["Counter"]);
            Assert.AreEqual(25, result.Rows[1]["OptimisticTotalLimit"]);
            Assert.AreEqual(22, result.Rows[1]["InProgress"]);
            Assert.AreEqual(21, result.Rows[1]["ExtraCounter"]);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellsValues_EmptyCellId_ValidResult()
        {
            var designQuota = new QuotaList
                {
                    QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"1", "1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"1", "2"}, QuotaRowId = 2},
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"2", "1"}, QuotaRowId = 3},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"2", "2"}, QuotaRowId = 4}
                                },
                    FieldNames = new[] { "q1", "q2" },
                    QuotaFullEmailAddress = "123",
                    QuotaId = 123,
                    QuotaName = "quota1",
                    IsOptimistic = true,
                };

            var result = QuotaManager.GetCellsValues(designQuota, new List<int>(), new[] { "q1", "q2" }).ToArray();
            CompareCellsValues(
                new string[][]{},
                result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellsValues_CellId_ValidResult()
        {
            var designQuota = new QuotaList
            {
                QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"1", "1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"1", "2"}, QuotaRowId = 2},
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"2", "1"}, QuotaRowId = 3},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"2", "2"}, QuotaRowId = 4}
                                },
                FieldNames = new[] { "q1", "q2" },
                QuotaFullEmailAddress = "123",
                QuotaId = 123,
                QuotaName = "quota1",
                IsOptimistic = true,
            };

            var result = QuotaManager.GetCellsValues(designQuota, new List<int>( new []{2}), new[] { "q1", "q2" }).ToArray();

            CompareCellsValues(new[] { new[] { "1", "2" } }, result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellsValues_TowCells_ValidResult()
        {
            var designQuota = new QuotaList
            {
                QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"1", "1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"1", "2"}, QuotaRowId = 2},
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"2", "1"}, QuotaRowId = 3},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"2", "2"}, QuotaRowId = 4}
                                },
                FieldNames = new[] { "q1", "q2" },
                QuotaFullEmailAddress = "123",
                QuotaId = 123,
                QuotaName = "quota1",
                IsOptimistic = true,
            };

            var result = QuotaManager.GetCellsValues(designQuota, new List<int>(new[] { 2,3 }), new[] { "q1", "q2" }).ToArray();

            CompareCellsValues(new[]
                {
                    new[] { "1", "2" },
                    new[] { "2", "1" }
                }, result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellsValues_TowCellsInSameGroup_ValidResult()
        {
            var designQuota = new QuotaList
            {
                QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"1", "1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"1", "2"}, QuotaRowId = 2},
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"2", "1"}, QuotaRowId = 3},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"2", "2"}, QuotaRowId = 4}
                                },
                FieldNames = new[] { "q1", "q2" },
                QuotaFullEmailAddress = "123",
                QuotaId = 123,
                QuotaName = "quota1",
                IsOptimistic = true,
            };

            var result = QuotaManager.GetCellsValues(designQuota, new List<int>(new[] { 2, 4 }), new[] { "q2" }).ToArray();

            CompareCellsValues(new[]
                {
                    new[] { "2" }
                }, result);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void GetCellsValues_TowCellsInDifGroup_ValidResult()
        {
            var designQuota = new QuotaList
            {
                QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"1", "1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"1", "2"}, QuotaRowId = 2},
                                    new QuotaRow{Target = 10, Counter = 1, LiveTarget = 15, LiveCounter = 2, FieldPrecodes = new[] {"2", "1"}, QuotaRowId = 3},
                                    new QuotaRow{Target = 20, Counter = 2, LiveTarget = 25, LiveCounter = 4, FieldPrecodes = new[] {"2", "2"}, QuotaRowId = 4}
                                },
                FieldNames = new[] { "q1", "q2" },
                QuotaFullEmailAddress = "123",
                QuotaId = 123,
                QuotaName = "quota1",
                IsOptimistic = true,
            };

            var result = QuotaManager.GetCellsValues(designQuota, new List<int>(new[] { 2, 3 }), new[] { "q2" }).ToArray();

            CompareCellsValues(new[]
                {
                    new[] { "1" },
                    new[] { "2" }
                }, result);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetQuotaWithUsedCallsCounters_1FieldsAnd2Cells_ValidResults()
        {
            var designQuota =
                new QuotaList
                {
                    QuotaRows = new[]
                                {
                                    new QuotaRow{Target = 19, Counter = 10, FieldPrecodes = new[] {"1"}, QuotaRowId = 1},
                                    new QuotaRow{Target = 29, Counter = 20, FieldPrecodes = new[] {"2"}, QuotaRowId = 2}
                                },
                    FieldNames = new[] { "q1" },                    
                    QuotaId = 123,
                    QuotaName = "quota1",                    
                };

            var q11 = new Answer { Precode = "1", Texts = new[] { new AnswerText { Value = "a11" } } };
            var q12 = new Answer { Precode = "2", Texts = new[] { new AnswerText { Value = "a12" } } };

            var q1 =
                new SingleForm
                {
                    Name = "q1",
                    SingleAnswers = new SingleAnswers(){ Items = new AnswerBase[] { q11, q12 } },
                    FormTexts = new[] { new FormText { Title = "t1" } }
                };            

            var extraCounters = (IEnumerable<QuotaCellCounter>)new[]{new QuotaCellCounter { Descriptor = "1", Value = 11 },
                                                                     new QuotaCellCounter { Descriptor = "2", Value = 21 }};

            var usedCallsCounters = (IEnumerable<QuotaCellCounter>)new[]{  new QuotaCellCounter { Descriptor = "1", Value = 5 },
                                                                           new QuotaCellCounter { Descriptor = "2", Value = 30 }};

            IExtraQuotaCounterCalculator extraQuotaCounterCalculator = new StubIExtraQuotaCounterCalculator
            {
                GetCellCounter = () => extraCounters
            };

            IExtraQuotaCounterService extraQuotaCounterService = new StubIExtraQuotaCounterService
            {
                CreateIExtraQuotaCounterParameters = param => extraQuotaCounterCalculator
            };
            ServiceLocator.RegisterInstance(extraQuotaCounterService);

            IUsedCallsCalculator usedCallsCalculator = new StubIUsedCallsCalculator
            {
                GetCountersOfNotScheduledExcludingCompletesIExtraQuotaCounterParameters = param => usedCallsCounters
            };
            ServiceLocator.RegisterInstance(usedCallsCalculator);

            var fields = new List<SingleForm>(new[] { q1 });
            var parameters = QuotaManager.GetExtraCounterParameters(ExtraQuotaCounterTypes.Scheduled, 0, designQuota.QuotaId, false, new int[] { });
            var aggregateQuotaViewAdditionalColumnsBuilder = new AdditionalColumnsBuilderFactory().Create(false, true, false, parameters);

            DataTable result = QuotaManager.CreateQuotaDataTable(fields, aggregateQuotaViewAdditionalColumnsBuilder);
            QuotaManager.FillQuotaDataTable(result, designQuota, fields, aggregateQuotaViewAdditionalColumnsBuilder);

            Assert.AreEqual(designQuota.FieldNames.Length * 2  + 9, result.Columns.Count);
            Assert.AreEqual(designQuota.QuotaRows.Length, result.Rows.Count);

            Assert.AreEqual("a11", result.Rows[0]["q1"]);
            Assert.AreEqual(19, result.Rows[0]["Limit"]);
            Assert.AreEqual(10, result.Rows[0]["Counter"]);
            Assert.AreEqual(11, result.Rows[0]["ExtraCounter"]);
            Assert.AreEqual(5, result.Rows[0]["UsedCalls"]);
            Assert.AreEqual(0.5, result.Rows[0]["BurnRate"]);

            Assert.AreEqual("a12", result.Rows[1]["q1"]);
            Assert.AreEqual(29, result.Rows[1]["Limit"]);
            Assert.AreEqual(20, result.Rows[1]["Counter"]);
            Assert.AreEqual(30, result.Rows[1]["UsedCalls"]);
            Assert.AreEqual(1.5, result.Rows[1]["BurnRate"]);
        }

        [TestMethod, Owner(@"FIRM\vyacheslavb")]
        public void QuotaWithCounterPercentage_CalculateCounterPercentage_CalculateSuccess()
        {
            var designQuota =
                new QuotaList
                {
                    QuotaRows = new[]
                    {
                        new QuotaRow{Target = 100, Counter = 10, LiveTarget = 15, LiveCounter = 12, FieldPrecodes = new[] {"1"}, QuotaRowId = 1},
                        new QuotaRow{Target = 200, Counter = 10, LiveTarget = 25, LiveCounter = 22, FieldPrecodes = new[] {"2"}, QuotaRowId = 2}
                    },
                    FieldNames = new[] { "q1" },
                    QuotaFullEmailAddress = "123",
                    QuotaId = 123,
                    QuotaName = "quota1",
                    IsOptimistic = true,
                };

            var parameters = QuotaManager.GetExtraCounterParameters(
                ExtraQuotaCounterTypes.InterviewsWithSpecificStatuses, 0, designQuota.QuotaId, false, new int[] { });

            var aggregateQuotaViewAdditionalColumnsBuilder = new AdditionalColumnsBuilderFactory().Create(
                designQuota.IsOptimistic,
                false, false,
                parameters
                );

            DataTable result = QuotaManager.CreateQuotaDataTable(new List<SingleForm>(), aggregateQuotaViewAdditionalColumnsBuilder);
            QuotaManager.FillQuotaDataTable(result, designQuota, new List<SingleForm>(), aggregateQuotaViewAdditionalColumnsBuilder);

            Assert.AreEqual(10, result.Rows[0]["CounterPercentage"]);
            Assert.AreEqual(5, result.Rows[1]["CounterPercentage"]);
        }

        private void CompareCellsValues(string[][] expected, string[][] actual)
        {
            Assert.AreEqual(expected.Length , actual.Length);

            var expectedResult = String.Join( Environment.NewLine, expected.Select( x => String.Join(",", x )).OrderBy(y => y).ToArray());
            var actualResult = String.Join(Environment.NewLine, expected.Select(x => String.Join(",", x)).OrderBy(y => y).ToArray());
            Assert.AreEqual(expectedResult, actualResult);
        }
    }
}