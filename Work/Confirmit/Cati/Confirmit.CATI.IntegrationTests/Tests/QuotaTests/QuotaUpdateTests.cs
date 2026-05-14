using System.Linq;
using System.Threading;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaTests
{
    /// <summary>
    /// Summary description for ExtraCounterTests
    /// </summary>
    [TestClass]
    public class QuotaUpdateTests : BaseMockedIntegrationTest
    {
        private IQuotaRepository _quotaRepository;
        private IQuotaCellRepository _quotaCellRepository;
        private ISurveyRepository _surveyRepository;

        public override void OnPostTestInitialize()
        {
            _quotaRepository = ServiceLocator.Resolve<IQuotaRepository>();
            _quotaCellRepository = ServiceLocator.Resolve<IQuotaCellRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        }

        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void OnQuotaUpdate_ImportQuotaFromSurveyDbToCatiDb_ResultCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=1,q2=B", Counter=0, Limit=10},
                                    new CellData{Id = 3, Values="q1=2,q2=A", Counter=0, Limit=10},
                                    new CellData{Id = 4, Values="q1=2,q2=B", Counter=0, Limit=10},
                                }
                            }}
                }}
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("Q1");

            quota.OnQuotaChanged();

            var targetSurvey = _surveyRepository.GetById(survey.Id);
            Assert.AreEqual(targetSurvey.IsQuotaInCatiDb, true);

            var target = _quotaRepository.TryGetById(survey.Id, quota.Data.Id);
            Assert.AreEqual(quota.Data.Id, target.QuotaID);
            Assert.AreEqual(quota.Data.Name, target.Name);
            Assert.AreEqual(survey.Id, target.SurveyID);
        }

        [TestMethod, Owner(@"FIRM\OlegZ")]
        public void QuotaCellChange_ImportQuotaFromSurveyDbToCatiDbAndCloseCell_ResultCorrect()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=1,q2=B", Counter=0, Limit=10},
                                    new CellData{Id = 3, Values="q1=2,q2=A", Counter=0, Limit=10},
                                    new CellData{Id = 4, Values="q1=2,q2=B", Counter=0, Limit=10},
                                }
                            }}
                }}
            }.Create();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("Q1");
            var cell = quota.GetCell(1);

            quota.OnQuotaChanged();
            cell.ChangeState(QuotaCellState.PessimisticallyClosed);

            var targetSurvey = _surveyRepository.GetById(survey.Id);
            Assert.AreEqual(targetSurvey.IsQuotaInCatiDb, true);

            var target = _quotaCellRepository.TryGetById(survey.Id, quota.Data.Id, cell.Data.Id);
            Assert.AreEqual(survey.Id, target.SurveyID);
            Assert.AreEqual(quota.Data.Id, target.QuotaID);
            Assert.AreEqual(cell.Data.Id, target.CellID);
            Assert.AreEqual(false, target.IsDisabled);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void LaunchSurvey_StopUsingOfAnyCatiQuotas_AllQuotaInfoAreCleaned()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=1,q2=B", Counter=0, Limit=10},
                                    new CellData{Id = 3, Values="q1=2,q2=A", Counter=0, Limit=10},
                                    new CellData{Id = 4, Values="q1=2,q2=B", Counter=0, Limit=10},
                                }
                            }}
                }}
            }.Create();

            Assert.AreEqual(1, BvSurveyQuotaAdapter.GetAll().Count, "Wrong quotas count");
            Assert.AreEqual(9, BvSurveyQuotaCellAdapter.GetAll().Count, "Wrong quota cells count");

            var survey = context.GetSurvey("S1");
            survey.Data.Quotas = new QuotaData[] { };

            survey.Launch();

            Assert.AreEqual(0, BvSurveyQuotaAdapter.GetAll().Count, "Wrong quotas count");
            Assert.AreEqual(0, BvSurveyQuotaCellAdapter.GetAll().Count, "Wrong quota cells count");
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void OnQuotaUpdate_OneCellIsClosed_DataAreSynchronized()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[] {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1"},
                                Cells = new[] {
                                    new CellData{Id = 1, Values="q1=1", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=2", Counter=0, Limit=10},
                                }
                            },
                            new QuotaData{ Id = 2, Name="Q2", Fields = new[] {"q2"},
                                Cells = new[] {
                                    new CellData{Id = 1, Values="q2=1", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q2=2", Counter=0, Limit=10},
                                }
                            },
                        }
                    }}
            }.Create();

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID TableName Name IsOptimistic IsFCD  Email
{     S1}       1   quota_1   Q1        False     1 <NULL>
{     S1}       2   quota_2   Q2        False     1 <NULL>"),
                BackendTools.Format(BvSurveyQuotaAdapter.GetAll()
                    .Select(x => new { x.SurveyID, x.QuotaID, x.TableName, x.Name, x.IsOptimistic, x.IsFCD, x.Email })),
                "Wrong BvSurveyQuota table state");

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit LiveCounter LiveLimit IsDisabled
{     S1}       1     -1       0     2           0         0      False
{     S1}       1      1       0    10           0        10      False
{     S1}       1      2       0    10           0        10      False
{     S1}       2     -1       0     2           0         0      False
{     S1}       2      1       0    10           0        10      False
{     S1}       2      2       0    10           0        10      False"),
                BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll()
                    .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.LiveCounter, x.LiveLimit, x.IsDisabled })),
                "Wrong BvSurveyQuotaCell table state");

            var survey = context.GetSurvey("S1");
            survey.Database.CloseCell(1, 1);
            survey.GetQuota("Q1").OnQuotaChanged();

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID TableName Name IsOptimistic IsFCD  Email
{     S1}       1   quota_1   Q1        False     1 <NULL>
{     S1}       2   quota_2   Q2        False     1 <NULL>"),
                BackendTools.Format(BvSurveyQuotaAdapter.GetAll().Select(x => new { x.SurveyID, x.QuotaID, x.TableName, x.Name, x.IsOptimistic, x.IsFCD, x.Email })),
                "Wrong BvSurveyQuota table state");

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit LiveCounter LiveLimit IsDisabled
{     S1}       1     -1       1     2           0         0      False
{     S1}       1      1      10    10           0        10      False
{     S1}       1      2       0    10           0        10      False
{     S1}       2     -1       0     2           0         0      False
{     S1}       2      1       0    10           0        10      False
{     S1}       2      2       0    10           0        10      False"),
                BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll()
                    .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.LiveCounter, x.LiveLimit, x.IsDisabled })),
                "Wrong BvSurveyQuotaCell table state");
        }


        [TestMethod, Owner(@"FIRM\MaximL")]
        public void OnCellChanged_CellIsClosed_DataAreSynchronized()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[] {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"1", "2"}}
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1"},
                                Cells = new[] {
                                    new CellData{Id = 1, Values="q1=1", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=2", Counter=0, Limit=10},
                                }
                            },
                            new QuotaData{ Id = 2, Name="Q2", Fields = new[] {"q2"},
                                Cells = new[] {
                                    new CellData{Id = 1, Values="q2=1", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q2=2", Counter=0, Limit=10},
                                }
                            },
                        }
                    }}
            }.Create();

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID TableName Name IsOptimistic IsFCD  Email
{     S1}       1   quota_1   Q1        False     1 <NULL>
{     S1}       2   quota_2   Q2        False     1 <NULL>"),
                BackendTools.Format(BvSurveyQuotaAdapter.GetAll()
                    .Select(x => new { x.SurveyID, x.QuotaID, x.TableName, x.Name, x.IsOptimistic, x.IsFCD, x.Email })),
                "Wrong BvSurveyQuota table state");

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit LiveCounter LiveLimit IsDisabled
{     S1}       1     -1       0     2           0         0      False
{     S1}       1      1       0    10           0        10      False
{     S1}       1      2       0    10           0        10      False
{     S1}       2     -1       0     2           0         0      False
{     S1}       2      1       0    10           0        10      False
{     S1}       2      2       0    10           0        10      False"),
                BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll()
                    .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.LiveCounter, x.LiveLimit, x.IsDisabled })),
                "Wrong BvSurveyQuotaCell table state");

            context.GetSurvey("S1").GetQuota("Q2").CloseCellById(2);

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID TableName Name IsOptimistic IsFCD  Email
{     S1}       1   quota_1   Q1        False     1 <NULL>
{     S1}       2   quota_2   Q2        False     1 <NULL>"),
                BackendTools.Format(BvSurveyQuotaAdapter.GetAll().Select(x => new { x.SurveyID, x.QuotaID, x.TableName, x.Name, x.IsOptimistic, x.IsFCD, x.Email })),
                "Wrong BvSurveyQuota table state");

            Assert.AreEqual(
                BackendTools.Format(context, @"
 SurveyID QuotaID CellID Counter Limit LiveCounter LiveLimit IsDisabled
{     S1}       1     -1       0     2           0         0      False
{     S1}       1      1       0    10           0        10      False
{     S1}       1      2       0    10           0        10      False
{     S1}       2     -1       1     2           0         0      False
{     S1}       2      1       0    10           0        10      False
{     S1}       2      2      10    10           0        10      False"),
                BackendTools.Format(BvSurveyQuotaCellAdapter.GetAll()
                    .Select(x => new { x.SurveyID, x.QuotaID, x.CellID, x.Counter, x.Limit, x.LiveCounter, x.LiveLimit, x.IsDisabled })),
                "Wrong BvSurveyQuotaCell table state");
        }

        [TestMethod, Owner(@"FIRM\Egork")]
        public void LaunchSurvey_ImportQuotaCalledWhenQuotaStructureChanged()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"},
                                Cells = new[]
                                {
                                    new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10}
                                }
                            }}
                }}
            }.Create();

            var survey = context.GetSurvey("S1");

            bool populateCalled = false;
            var currentReplicationService = ServiceLocator.Resolve<IInterviewQuotaCellService>();
            ServiceLocator.RegisterInstance<IInterviewQuotaCellService>(new StubIInterviewQuotaCellService()
            {
                Inner = currentReplicationService,
                PopulateInt32CancellationToken = (surveySid, ct) =>
                {
                    populateCalled = true;
                    currentReplicationService.Populate(surveySid, ct);
                }
            });


            survey.Data.Quotas = new[] {
                new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"},
                    Cells = new[]
                    {
                        new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                        new CellData{Id = 2, Values="q1=2,q2=A", Counter=0, Limit=10}
                    }
                }};
            populateCalled = false;
            survey.Launch();
            Assert.IsTrue(populateCalled);

            survey.Data.Quotas = new[] {
                new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "q2"},
                    Cells = new[]
                    {
                        new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                        new CellData{Id = 2, Values="q1=2,q2=A", Counter=0, Limit=10}
                    }
                },
                new QuotaData{ Id = 2, Name="Q2", Fields = new[] {"q1", "q2"},
                    Cells = new[]
                    {
                        new CellData{Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                        new CellData{Id = 2, Values="q1=2,q2=A", Counter=0, Limit=10}
                    }
                }};
            populateCalled = false;
            survey.Launch();
            Assert.IsTrue(populateCalled);

            populateCalled = false;
            survey.Launch();
            Assert.IsFalse(populateCalled);
        }
    }
}
