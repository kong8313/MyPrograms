using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaClustering
{
    [TestClass]
    public class CallDeliveryTests : BaseMockedIntegrationTest
    {
        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }
        
        [TestMethod, Owner(@"Firm\MaximL")]
        public void CallDeliveryIsSA_LiveCountIsnotOverlimit_CallAreDelivered()
        {
            int surveyId = BackendToolsObject.CreateSurvey("p00000001");
            _surveyStateService.Open(surveyId);

            //there are should be 2*2 cells;
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });
            
            var interviews = BackendTools.CreateInterviewsWithCalls(surveyId, 2);

            var cellId1 = 1;

            quota.PutInterviewsInCells(
                new[] { interviews[0].ID, interviews[1].ID },
                new[] { cellId1, cellId1 });

            EnableQuotaClustering(surveyId, quota, 2);

            var console1 = TestCatiConsole.CreateAndLoginAsSA(surveyId, "u1");
            var interview1 = console1.Start();
            Assert.AreEqual(interviews[0].ID, interview1.ID);

            var console2 = TestCatiConsole.CreateAndLoginAsSA(surveyId, "u2");
            var interview2 = console2.Start();
            
            Assert.IsNotNull(interview2);
            Assert.AreEqual(interviews[1].ID, interview2.ID);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void CallDeliveryIsSA_LiveCountIsOverlimit_CallAreNotDelivered()
        {
            int surveyId = BackendToolsObject.CreateSurvey("p00000001");
            _surveyStateService.Open(surveyId);

            //there are should be 2*2 cells;
            var quota = TestQuota.Create(TestingFramework.DbEngine,
                surveyId,
                1,
                new[] { "q1", "q2" },
                new[] { 2, 2 });

            var interviews = BackendTools.CreateInterviewsWithCalls(surveyId, 2);

            var cellId1 = 1;

            quota.PutInterviewsInCells(
                new[] { interviews[0].ID, interviews[1].ID },
                new[] { cellId1, cellId1 });

            EnableQuotaClustering(surveyId, quota, 1);

            var console1 = TestCatiConsole.CreateAndLoginAsSA(surveyId, "u1");
            var interview1 = console1.Start();
            Assert.AreEqual(interviews[0].ID, interview1.ID);

            var console2 = TestCatiConsole.CreateAndLoginAsSA(surveyId, "u2");
            var interview2 = console2.Start();
            if (interview2 != null)
            {
                Trace.TraceInformation("interview2 SID={0}, IID={1}", interview2.SurveySID, interview2.ID);
            }
            Assert.IsNull(interview2);
        }

        private static void EnableQuotaClustering(int surveyId, TestQuota quota, int liveThreshold)
        {
            ServiceLocator.Resolve<IQuotaClusteringSettingsGroup>().Enabled = true;
            var service = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();

            service.Configure(surveyId, new QuotaClusteringConfiguration() { LiveThreshod = liveThreshold, QuotaName = quota.Name });
            BackendTools.ExecuteAllAsyncOperations();
        }
    }
}
