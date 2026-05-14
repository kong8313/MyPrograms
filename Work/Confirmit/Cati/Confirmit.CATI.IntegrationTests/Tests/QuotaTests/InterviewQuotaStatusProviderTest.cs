using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey.Quota;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuotaData = Confirmit.CATI.IntegrationTests.Framework.Data.QuotaData;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaTests
{
    [TestClass]
    public class InterviewQuotaStatusProviderTest : BaseMockedIntegrationTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void InterviewQuotaStatusProviderReturnsCorrectStatuses()
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
                            new QuotaData{ Id = 2, Name="Q2", Fields = new[] {"q1", "q2"},
                                Cells = new[] {
                                    new CellData{Id = 1, Values="q1=1,q2=1", Counter=0, Limit=10},
                                    new CellData{Id = 2, Values="q1=1,q2=2", Counter=0, Limit=10},
                                    new CellData{Id = 3, Values="q1=2,q2=1", Counter=10, Limit=10},
                                    new CellData{Id = 4, Values="q1=2,q2=2", Counter=0, Limit=0},
                                }
                            },
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", Data = "q1=1,q2=1"},
                            new InterviewData {Tag = "S1.I2", Data = "q1=1,q2=2"},
                            new InterviewData {Tag = "S1.I3", Data = "q1=2,q2=1"},
                            new InterviewData {Tag = "S1.I4", Data = "q1=2,q2=2"},
                            new InterviewData {Tag = "S1.I5", Data = "q1=1,q2="},
                            new InterviewData {Tag = "S1.I6", Data = "q1=1,q2=3"},
                        },
                    }}
            }.Create();

            var statusProvider = ServiceLocator.Resolve<InterviewQuotaStatusProvider>();

            var survey = context.GetSurvey("S1");
            var interviews = context.GetInterviews("S1.I1", "S1.I2", "S1.I3", "S1.I4", "S1.I5", "S1.I6").ToArray();

            var status = statusProvider.GetQuotaStatus(survey.Id, interviews[0].Id);
            
            Assert.AreEqual(2, status.Count);

            var first = status.First();
            Assert.AreEqual(true, first.IsOpen);
            Assert.AreEqual(1, first.QuotaId);
            Assert.AreEqual(true, first.IsFcdQuota);
            Assert.AreEqual("Q1", first.QuotaName);
            Assert.AreEqual(false, first.HasEmptyAnswers);
            Assert.AreEqual(true, first.IsNormalCell);
            Assert.AreEqual(false, first.IsZeroLimit);
            
            Assert.AreEqual(1, first.Fields.Count);
            Assert.AreEqual("1", first.Fields["q1"]);
            
            var second = status.Last();
            Assert.AreEqual(true, second.IsOpen);
            Assert.AreEqual(2, second.QuotaId);
            Assert.AreEqual(true, second.IsFcdQuota);
            Assert.AreEqual("Q2", second.QuotaName);
            Assert.AreEqual(false, second.HasEmptyAnswers);
            Assert.AreEqual(true, second.IsNormalCell);
            Assert.AreEqual(false, second.IsZeroLimit);
            
            Assert.AreEqual(2, second.Fields.Count);
            Assert.AreEqual("1", second.Fields["q1"]);
            Assert.AreEqual("1", second.Fields["q2"]);
            
            status = statusProvider.GetQuotaStatus(survey.Id, interviews[2].Id);
            second = status.Last();
            Assert.AreEqual(false, second.IsOpen);
            
            status = statusProvider.GetQuotaStatus(survey.Id, interviews[3].Id);
            second = status.Last();
            Assert.AreEqual(false, second.IsOpen);
            Assert.AreEqual(true, second.IsZeroLimit);
            
            status = statusProvider.GetQuotaStatus(survey.Id, interviews[4].Id);
            second = status.Last();
            Assert.AreEqual(true, second.IsOpen);
            Assert.AreEqual(false, second.IsNormalCell);
            Assert.AreEqual(true, second.HasEmptyAnswers);
            
            status = statusProvider.GetQuotaStatus(survey.Id, interviews[5].Id);
            second = status.Last();
            Assert.AreEqual(true, second.IsOpen);
            Assert.AreEqual(false, second.IsNormalCell);
            Assert.AreEqual(false, second.HasEmptyAnswers);
        }
    }
}
