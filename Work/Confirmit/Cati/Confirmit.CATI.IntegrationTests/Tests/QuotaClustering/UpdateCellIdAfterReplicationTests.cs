using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaClustering
{
    [TestClass]
    public class UpdateCellIdAfterReplicationTests : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            BackendToolsObject.LaunchAllHoursScript();
            ServiceLocator.Resolve<ISystemSettings>().QuotaClustering.Enabled = true;
        }

        [TestMethod]
        public void UpdateReplicationData_ManualModeAndCellIdChangedToUnknown_CellIdIsUpdatedAndLiveCounterDecremented()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", ClusterQuota = "quota1", ClusterQuotaThreshold = 10, IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
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
                   Persons = new[]{new PersonData(){Tag="P1", TaskChoice = TaskChoiceMode.Manual}}
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            Assert.AreEqual(1, context.GetCall("S1.I1").Model.CellId);
            Assert.AreEqual(2, context.GetCall("S1.I2").Model.CellId);
            Assert.AreEqual(0, context.GetCall("S1.I3").Model.CellId);

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var console = new ManualModeConsoleController(context, person);
            console.Login();

            console.StartInterview(context.GetInterview("S1.I1"));

            Assert.AreEqual(1, GetCellInfo(survey, 1).LiveCount );
            Assert.AreEqual(0, GetCellInfo(survey, 2).LiveCount );

            context.GetInterview("S1.I1").SetData("q1=");
            context.GetInterview("S1.I2").SetData("q1=1");
            context.GetInterview("S1.I3").SetData("q1=2");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            Assert.AreEqual(0, context.GetCall("S1.I1").Model.CellId);
            Assert.AreEqual(1, context.GetCall("S1.I2").Model.CellId);
            Assert.AreEqual(2, context.GetCall("S1.I3").Model.CellId);

            Assert.AreEqual(0, GetCellInfo(survey, 1).LiveCount);
            Assert.AreEqual(0, GetCellInfo(survey, 2).LiveCount);
        }

        [TestMethod]
        public void UpdateReplicationData_ManualModeAndCellIdChangedToOther_CellIdIsUpdatedAndLiveCounterReincremented()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", ClusterQuota = "quota1", ClusterQuotaThreshold = 10, IsUseDb = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
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

            Assert.AreEqual(1, context.GetCall("S1.I1").Model.CellId);
            Assert.AreEqual(2, context.GetCall("S1.I2").Model.CellId);
            Assert.AreEqual(0, context.GetCall("S1.I3").Model.CellId);

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var console = new ManualModeConsoleController(context, person);
            console.Login();

            console.StartInterview(context.GetInterview("S1.I1"));

            Assert.AreEqual(1, GetCellInfo(survey, 1).LiveCount);
            Assert.AreEqual(0, GetCellInfo(survey, 2).LiveCount);

            context.GetInterview("S1.I1").SetData("q1=2");
            context.GetInterview("S1.I2").SetData("q1=");
            context.GetInterview("S1.I3").SetData("q1=1");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            Assert.AreEqual(2, context.GetCall("S1.I1").Model.CellId);
            Assert.AreEqual(0, context.GetCall("S1.I2").Model.CellId);
            Assert.AreEqual(1, context.GetCall("S1.I3").Model.CellId);

            Assert.AreEqual(0, GetCellInfo(survey, 1).LiveCount);
            Assert.AreEqual(1, GetCellInfo(survey, 2).LiveCount);
        }

        [TestMethod]
        public void UpdateReplicationData_SaModeAndCellIdChangedToOther_CellIdIsUpdatedAndLiveCounterReincremented()
        {
            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData() { Tag="S1", ClusterQuota = "quota1", ClusterQuotaThreshold = 10, IsUseDb = true, IsOpen = true,
                        Forms = new[] {
                            new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
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
                            new InterviewData(){ Tag="S1.I1", Data="q1=1", Call = new CallData(){Priority = 10}},
                            new InterviewData(){ Tag="S1.I2", Data="q1=2", Call = new CallData()},
                            new InterviewData(){ Tag="S1.I3", Call = new CallData()},
                        },
                        Assigns = new[]{"P1"}}
                   },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            Assert.AreEqual(1, context.GetCall("S1.I1").Model.CellId);
            Assert.AreEqual(2, context.GetCall("S1.I2").Model.CellId);
            Assert.AreEqual(0, context.GetCall("S1.I3").Model.CellId);

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();

            var interview = console.StartInterview();

            Assert.AreEqual(1, GetCellInfo(survey, 1).LiveCount);
            Assert.AreEqual(0, GetCellInfo(survey, 2).LiveCount);

            context.GetInterview("S1.I1").SetData("q1=2");
            context.GetInterview("S1.I2").SetData("q1=");
            context.GetInterview("S1.I3").SetData("q1=1");

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            Assert.AreEqual(2, context.GetCall("S1.I1").Model.CellId);
            Assert.AreEqual(0, context.GetCall("S1.I2").Model.CellId);
            Assert.AreEqual(1, context.GetCall("S1.I3").Model.CellId);

            Assert.AreEqual(0, GetCellInfo(survey, 1).LiveCount);
            Assert.AreEqual(1, GetCellInfo(survey, 2).LiveCount);
        }

        private static BvClusteredQuotaCellEntity GetCellInfo(SurveyController survey, int cellId)
        {
            return BvClusteredQuotaCellAdapter.GetAll().Single(x => x.SurveyId == survey.Id && x.CellId == cellId);
        }
    }
}
