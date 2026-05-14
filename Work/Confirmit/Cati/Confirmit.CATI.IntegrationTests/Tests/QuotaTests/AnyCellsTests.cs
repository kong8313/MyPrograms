using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Core.Services.Interfaces;
using QuotaData = Confirmit.CATI.IntegrationTests.Framework.Data.QuotaData;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Services.Survey.Quota;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaTests
{
    /// <summary>
    /// Summary description for AnyCellsTests
    /// </summary>
    [TestClass]
    public class AnyCellsTests : BaseMockedIntegrationTest
    {
        private IQuotaCellRepository _quotaCellRepository;
        private IFcdQuotaService _quotaService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            BackendToolsObject.LaunchAllHoursScript();

            _quotaCellRepository = ServiceLocator.Resolve<IQuotaCellRepository>();
            _quotaService = ServiceLocator.Resolve<IFcdQuotaService>();
        }

        private void CompareQuotaCellDatas(QuotaCellData expectedData, QuotaCellData actualData)
        {
            Assert.IsNotNull(expectedData);
            Assert.IsNotNull(actualData);
            Assert.IsNotNull(expectedData.FieldValues);
            Assert.IsNotNull(actualData.FieldValues);
            Assert.AreEqual(expectedData.FieldValues.Count(), actualData.FieldValues.Count());

            for (int i = 0; i < expectedData.FieldValues.Count(); i++)
            {
                Assert.AreEqual(expectedData.FieldValues[i].Field, actualData.FieldValues[i].Field);
                Assert.AreEqual(expectedData.FieldValues[i].Value, actualData.FieldValues[i].Value);
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddQuotaCells_RelaunchSurveyWithOneQuotaWithOneQuestion_InformationAboutAnyCellsWasAddedAndUpdatedCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag="S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() { Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new []
                        {
                            new QuotaData() { Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData() { Id = 1, Values="q1=1", Counter=2, Limit=10 },
                                    new CellData() { Id = 2, Values="q1=2", Counter=6, Limit=10 },
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var cells = _quotaCellRepository.GetBySurveyId(survey.Id);
            Assert.IsNotNull(cells);
            Assert.AreEqual(3, cells.Count());

            var cell = cells.First(x => x.CellID == -1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.Limit);
            Assert.AreEqual(true, cell.IsOpen);

            QuotaCellData expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cell.Data);

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var quota = survey.Data.Quotas[0];
            quota.Cells = quota.Cells.Union(new[] { new CellData() { Id = 3, Values = "q1=3", Counter = 6, Limit = 10 } }).ToArray();

            survey.Launch();

            cells = _quotaCellRepository.GetBySurveyId(survey.Id);
            Assert.IsNotNull(cells);
            Assert.AreEqual(4, cells.Count());

            cell = cells.First(x => x.CellID == -1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(3, cell.Limit);
            Assert.AreEqual(true, cell.IsOpen);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void RemoveQuota_LunchSurveyWithTwoQuotasThenRemoveOne_InformationAboutAnyCellsWasUpdatedCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag="S1", IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData(){Name="q1", Precodes = new [] { "1", "2" }},
                            new SingleFormData(){Name="q2", Precodes = new [] { "5", "6" }}
                        },
                        Quotas = new []
                        {
                            new QuotaData() { Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=5", Counter=2, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=6", Counter=6, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=5", Counter=3, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=6", Counter=5, Limit=10},
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var cells = _quotaCellRepository.GetBySurveyId(survey.Id);
            Assert.IsNotNull(cells);
            Assert.AreEqual(9, cells.Count());

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            new DatabaseEngine().ExecuteNonQuery($"update BvSurveyQuota set XmlData ='<QuotaData xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><FieldNames><string>q1</string></FieldNames></QuotaData>' WHERE SurveyID = {survey.Id} AND QuotaID = 1");

            BvSurveyQuotaCellAdapter.DeleteByCondition(
                    $"[SurveyId] = @SurveyId AND [QuotaId] = @QuotaId AND [CellID] in (2, 4)",
                    new SqlParameter("@SurveyId", survey.Id),
                    new SqlParameter("@QuotaId", 1));

            _quotaService.OnQuotaCellChanged(survey.Id, 1, 1, QuotaCellState.PessimisticallyClosed);

            cells = _quotaCellRepository.GetBySurveyId(survey.Id);
            Assert.IsNotNull(cells);
            Assert.AreEqual(3, cells.Count());

            var cell = cells.First(x => x.CellID == -1);
            Assert.IsNotNull(cell);
            QuotaCellData expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cell.Data);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddQuotaCells_CreateSurveyWithOneQuotaWithTwoQuestions_InformationAboutAnyCellsWasAddedCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new [] { "1", "2" }},
                            new SingleFormData(){Name="q2", Precodes = new [] { "5", "6" }}
                        },
                        Quotas = new [] {
                            new QuotaData(){Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=5", Counter=2, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=6", Counter=6, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=5", Counter=3, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=6", Counter=5, Limit=10},
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            var cells = _quotaCellRepository.GetBySurveyId(survey.Id);
            Assert.IsNotNull(cells);
            Assert.AreEqual(9, cells.Count());

            var cell = cells.First(x => x.CellID == -1);
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.Limit);
            Assert.AreEqual(true, cell.IsOpen);
            QuotaCellData expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cell.Data);

            cell = cells.First(x => x.CellID == -4);
            Assert.IsNotNull(cell);
            Assert.AreEqual(2, cell.Limit);
            Assert.AreEqual(true, cell.IsOpen);
            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cell.Data);

            cell = cells.First(x => x.CellID == -5);
            Assert.IsNotNull(cell);
            Assert.AreEqual(4, cell.Limit);
            Assert.AreEqual(true, cell.IsOpen);
            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cell.Data);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddQuotaCells_CreateSurveyWithTwoQuotasWithThreeQuestions_InformationAboutAnyCellsWasAddedCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"5", "6" }},
                            new SingleFormData(){Name="q3", Precodes = new []{"8", "9" }}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=5", Counter=10, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=6", Counter=10, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=5", Counter=3, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=6", Counter=5, Limit=10},
                                }
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q3" },
                                Cells = new[]
                                {
                                    new CellData(){Id = 5, Values="q3=8", Counter=2, Limit=10},
                                    new CellData(){Id = 6, Values="q3=9", Counter=10, Limit=10}
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            Assert.AreEqual(
    BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit IsOpen
{     S1}       1     -5       1     4   True
{     S1}       1     -4       0     2   True
{     S1}       1     -3       2     2  False
{     S1}       1     -2       1     2   True
{     S1}       1     -1       1     2   True
{     S1}       2     -1       1     2   True
{     S1}       1      1      10    10  False
{     S1}       1      2      10    10  False
{     S1}       1      3       3    10   True
{     S1}       1      4       5    10   True
{     S1}       2      5       2    10   True
{     S1}       2      6      10    10  False"),
    BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll().OrderBy(x => x.CellID)
        .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.IsOpen })),
    "Wrong BvSurveyQuotaCell table state");

            var cells = _quotaCellRepository.GetBySurveyId(survey.Id);

            QuotaCellData expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q3", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 2 && x.CellID == -1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -5)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -4)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -3)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "6" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -2)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x =>  x.CellID == 1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "6" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 2)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 3)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "6" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 4)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q3", Value = "8" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 5)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q3", Value = "9" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 6)?.Data);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddQuotaCells_CreateSurveyWithOneFullQuotaAndOneNotFull_InformationAboutAnyCellsWasAddedCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"5", "6" }},
                            new SingleFormData(){Name="q3", Precodes = new []{"8", "9" }}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=5", Counter=10, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=6", Counter=10, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=5", Counter=10, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=6", Counter=10, Limit=10},
                                }
                            },
                            new QuotaData(){ Id = 2, Name="quota2", Fields = new[] {"q3" },
                                Cells = new[]
                                {
                                    new CellData(){Id = 5, Values="q3=8", Counter=2, Limit=10},
                                    new CellData(){Id = 6, Values="q3=9", Counter=10, Limit=10}
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            Assert.AreEqual(
    BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit IsOpen
{     S1}       1     -5       4     4  False
{     S1}       1     -4       2     2  False
{     S1}       1     -3       2     2  False
{     S1}       1     -2       2     2  False
{     S1}       1     -1       2     2  False
{     S1}       2     -1       1     2   True
{     S1}       1      1      10    10  False
{     S1}       1      2      10    10  False
{     S1}       1      3      10    10  False
{     S1}       1      4      10    10  False
{     S1}       2      5       2    10   True
{     S1}       2      6      10    10  False"),
    BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll().OrderBy(x => x.CellID)
        .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.IsOpen })),
    "Wrong BvSurveyQuotaCell table state");

            var cells = _quotaCellRepository.GetBySurveyId(survey.Id);

            QuotaCellData expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q3", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 2 && x.CellID == -1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -5)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -4)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -3)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "6" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -2)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.QuotaID == 1 && x.CellID == -1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "6" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 2)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "5" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 3)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" },
                    new QuotaCellFieldValue() { Field = "q2", Value = "6" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 4)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q3", Value = "8" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 5)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q3", Value = "9" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 6)?.Data);

        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddQuotaCells_CreateSurveyWithOneQuota_CloseFirstQuestionThenCloseTheSecond_InformationAboutAnyCellsWasUpdatedCorrectly()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=9, Limit=10},
                                    new CellData(){Id = 2, Values="q1=2", Counter=9, Limit=10}
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=2", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");

            Assert.AreEqual(
    BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit IsOpen
{     S1}       1     -1       0     2   True
{     S1}       1      1       9    10   True
{     S1}       1      2       9    10   True"),
    BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll().OrderBy(x => x.CellID)
        .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.IsOpen })),
    "Wrong BvSurveyQuota table state");

            CompareQuotaCellsData(survey.Id);

            var quota = survey.Data.Quotas[0];
            quota.Cells[0].Limit = 9;
            survey.Launch();

            Assert.AreEqual(
    BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit IsOpen
{     S1}       1     -1       1     2   True
{     S1}       1      1       9     9  False
{     S1}       1      2       9    10   True"),
    BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll().OrderBy(x => x.CellID)
        .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.IsOpen })),
    "Wrong BvSurveyQuota table state");

            CompareQuotaCellsData(survey.Id);

            quota.Cells[1].Limit = 9;
            survey.Launch();

            Assert.AreEqual(
   BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit IsOpen
{     S1}       1     -1       2     2  False
{     S1}       1      1       9     9  False
{     S1}       1      2       9     9  False"),
   BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll().OrderBy(x => x.CellID)
       .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.IsOpen })),
   "Wrong BvSurveyQuota table state");

            CompareQuotaCellsData(survey.Id);
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void AddQuotaCells_CreateSurveyWithOneQuota_InterviewsAssignedToCorrectAcyCells()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData(){Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1","q2"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=1", Counter=0, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=2", Counter=0, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=1", Counter=0, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=2", Counter=0, Limit=10}
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1,q2=", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=1,q2=3", Call = new CallData()}
                        }
                    }
                }
            }.Create();

            var interviewQuotaCells = BvInterviewQuotaCellAdapter.GetAll();

            Assert.AreEqual(interviewQuotaCells[0].CellID, interviewQuotaCells[1].CellID);
        }



        private void CompareQuotaCellsData(int surveyId)
        {
            var cells = _quotaCellRepository.GetBySurveyId(surveyId);

            QuotaCellData expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == -1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "1" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 1)?.Data);

            expectedData = new QuotaCellData()
            {
                FieldValues = new[]
                {
                    new QuotaCellFieldValue() { Field = "q1", Value = "2" }
                }
            };
            CompareQuotaCellDatas(expectedData, cells.First(x => x.CellID == 2)?.Data);
        }
    }
}
