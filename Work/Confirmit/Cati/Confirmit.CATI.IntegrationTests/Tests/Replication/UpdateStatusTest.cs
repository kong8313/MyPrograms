using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Replication
{
    [TestClass]
    public class UpdateStatusTest : BaseMockedIntegrationTest
    {
        public override void OnPostTestInitialize()
        {
            TestingFramework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>().GetQuotaInfosInt32 = id => new QuotaInfo[] { };
        }
        ///<summary>
        /// 1. Create survey.
        /// 2. Set non-empty replication scheme.
        /// 3. Check that ReplicationStatus flag in the BvSurvey table was set to True.
        /// 4. Call UpdateSurveyReplicationStatus with False parameter
        /// 5. Check that replicationStatus flag in the BvSurvey table was set to False
        /// 6. Call UpdateSurveyReplicationStatus with True parameter.
        /// 7. Check that ReplicationStatus flag in the BvSurvey table was set to True.
        ///</summary>
        [TestMethod, Owner(@"FIRM\AlexanderM")]
        public void UpdateScheme_CreateAndDeleteSurvey_NoReplicationRecordsExists()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;

            TableInfo[] testData = ReplicationTools.GetTestData();
            new ManagementService().UpdateSurveyReplicationScheme(projectId, testData);
            Assert.AreEqual(true, SurveyRepository.GetById(surveySid).ReplicationStatus);

            new ManagementService().UpdateSurveyReplicationStatus(projectId, false);
            Assert.AreEqual(false, SurveyRepository.GetById(surveySid).ReplicationStatus);

            new ManagementService().UpdateSurveyReplicationStatus(projectId, true);
            Assert.AreEqual(true, SurveyRepository.GetById(surveySid).ReplicationStatus);
        }
    }
}
