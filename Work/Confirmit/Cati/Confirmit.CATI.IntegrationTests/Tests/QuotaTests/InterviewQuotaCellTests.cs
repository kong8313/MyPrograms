using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaTests
{
    [TestClass]
    public class InterviewQuotaCellTests : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\EgorK")]
        public void PopulatingInterviewQuotaCellsTable_RunReplication_CorrectCellsData()
        {
            var context = new TestData
            {
                Surveys = new[]{
                    new SurveyData { Tag="S1", IsUseDb = true, IsQuotaInCatiDb = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3" }},
                            new SingleFormData{Name="key", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{
                            new QuotaData{ Id = 1, Name="Q1", Fields = new[] {"q1", "key"},
                                Cells = new[]
                                {
                                    new CellData{Id = 1, Values="q1=1,key=A", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=1,key=B", Counter=0, Limit=10},
                                    new CellData{Id = 3, Values="q1=2,key=A", Counter=0, Limit=10},
                                    new CellData{Id = 4, Values="q1=2,key=B", Counter=0, Limit=10},
                                }
                            }
                        },
                        Interviews = new[]{
                            new InterviewData() { Data = "q1=1,key=A", Tag="I1" },
                            new InterviewData() { Data = "q1=1,key=A", Tag="I2" },
                            new InterviewData() { Data = "q1=2,key=A", Tag="I3" },
                            new InterviewData() { Data = "q1=3,key=A", Tag="I4" },
                            new InterviewData() { Data = "key=A", Tag="I5" },
                            new InterviewData() { Data = "q1=2", Tag="I6" }
                        }

                }}
            }.Create();

            var survey = context.GetSurvey("S1");

            var resp1 = context.GetInterview("I1").Model.ID;
            var resp2 = context.GetInterview("I2").Model.ID;
            var resp3 = context.GetInterview("I3").Model.ID;
            var resp4 = context.GetInterview("I4").Model.ID;
            var resp5 = context.GetInterview("I5").Model.ID;
            var resp6 = context.GetInterview("I6").Model.ID;

            var query = "SurveyId = @SurveyId AND QuotaId = @QuotaId";

            var interviewQuotaCells = BvInterviewQuotaCellAdapter.GetByCondition(query, new SqlParameter[] {
                new SqlParameter("SurveyId", survey.Id),
                new SqlParameter("QuotaId", 1)
            });
            Assert.AreEqual(6, interviewQuotaCells.Count);
            var cell1 = interviewQuotaCells.Find(x => x.InterviewId == resp1 && x.CellID == 1);
            Assert.IsNotNull(cell1);
            var cell2 = interviewQuotaCells.Find(x => x.InterviewId == resp2 && x.CellID == 1);
            Assert.IsNotNull(cell2);
            var cell3 = interviewQuotaCells.Find(x => x.InterviewId == resp3 && x.CellID == 3);
            Assert.IsNotNull(cell3);
            var cell4 = interviewQuotaCells.Find(x => x.InterviewId == resp4 && x.CellID < 0);
            Assert.IsNotNull(cell4);
            var cell5 = interviewQuotaCells.Find(x => x.InterviewId == resp5 && x.CellID < 0);
            Assert.IsNotNull(cell5);
            var cell6 = interviewQuotaCells.Find(x => x.InterviewId == resp6 && x.CellID < 0);
            Assert.IsNotNull(cell6);

            survey.Database.SetInterviewData(resp4, "q1=2,key=B");
            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();


            interviewQuotaCells = BvInterviewQuotaCellAdapter.GetByCondition(query, new SqlParameter[] {
                new SqlParameter("SurveyId", survey.Id),
                new SqlParameter("QuotaId", 1)
            });

            Assert.AreEqual(6, interviewQuotaCells.Count);
            cell1 = interviewQuotaCells.Find(x => x.InterviewId == resp1 && x.CellID == 1);
            Assert.IsNotNull(cell1);
            cell2 = interviewQuotaCells.Find(x => x.InterviewId == resp2 && x.CellID == 1);
            Assert.IsNotNull(cell2);
            cell3 = interviewQuotaCells.Find(x => x.InterviewId == resp3 && x.CellID == 3);
            Assert.IsNotNull(cell3);
            cell4 = interviewQuotaCells.Find(x => x.InterviewId == resp4 && x.CellID == 4);
            Assert.IsNotNull(cell4);
            cell5 = interviewQuotaCells.Find(x => x.InterviewId == resp5 && x.CellID < 0);
            Assert.IsNotNull(cell5);
            cell6 = interviewQuotaCells.Find(x => x.InterviewId == resp6 && x.CellID < 0);
            Assert.IsNotNull(cell6);
        }
    }
}
