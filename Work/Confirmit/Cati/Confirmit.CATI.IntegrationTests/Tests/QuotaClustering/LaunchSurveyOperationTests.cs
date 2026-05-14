using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaClustering
{
    [TestClass]
    public class LaunchSurveyOperationTests : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            BackendToolsObject.LaunchAllHoursScript();
            ServiceLocator.Resolve<ISystemSettings>().QuotaClustering.Enabled = true;
        }

        [TestMethod]
        public void RelaunchSurvey_QuotaIsnotChanged_WarningIsnotLogggedAndClusteringIsEnabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", ClusterQuota = "quota1", ClusterQuotaThreshold = 10, IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=2, Limit=10},
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");

            survey.Launch();

            var operation = BvAsyncOperationQueueAdapter.GetAll()
                .Where(x => x.Type == (int) OperationTypes.LaunchSurvey)
                .OrderByDescending(y => y.Id)
                .Last();

            Assert.AreEqual("", operation.Error);
            Assert.IsNotNull(survey.Model.ClusteredQuotaName);
        }

        [TestMethod]
        public void RelaunchSurvey_QuotaIsChanged_WarningIsLogggedAndClusteringIsEnabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", ClusterQuota = "quota1", ClusterQuotaThreshold = 10, IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=2, Limit=10},
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            
            var quota = survey.Data.Quotas[0];
            quota.Cells = quota.Cells.Union(new[] {new CellData() {Id = 3, Values = "q1=3", Counter = 6, Limit = 10}}).ToArray();
            
            survey.Launch();

            var operation = BvAsyncOperationQueueAdapter.GetAll()
                .Where(x => x.Type == (int)OperationTypes.LaunchSurvey)
                .OrderBy(y => y.Id)
                .Last();

            Assert.AreEqual("Warning! Quota clustering was reconfigured, because cluster quota was changed.", operation.Error);

            Assert.AreEqual(3, BvClusteredQuotaCellAdapter.GetAll().Where(x => x.SurveyId == survey.Id).Count());
            Assert.IsNotNull(survey.Model.ClusteredQuotaName);
        }

        [TestMethod]
        public void RelaunchSurvey_QuotaIsDeleted_WarningIsLogggedAndClusteringIsDisabled()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", ClusterQuota = "quota1", ClusterQuotaThreshold = 10, IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2", "3"}}
                        },
                        Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=2, Limit=10},
                                    new CellData(){Id = 2, Values="q1=2", Counter=6, Limit=10},
                                }
                            }
                        },
                        Interviews = new[]
                        {
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I2", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.Manual } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");

            survey.Data.Quotas = new QuotaData[] {};

            survey.Launch();

            var operation = BvAsyncOperationQueueAdapter.GetAll()
                .Where(x => x.Type == (int)OperationTypes.LaunchSurvey)
                .OrderBy(y => y.Id)
                .Last();

            Assert.AreEqual("Warning! Quota clustering was disabled, because cluster quota was deleted.", operation.Error);

            Assert.AreEqual(0, BvClusteredQuotaCellAdapter.GetAll().Where(x => x.SurveyId == survey.Id).Count());
            Assert.IsNull(survey.Model.ClusteredQuotaName);
        }
    }
}
