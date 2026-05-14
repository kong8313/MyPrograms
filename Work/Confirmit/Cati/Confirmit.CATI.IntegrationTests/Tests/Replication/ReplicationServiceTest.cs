using Confirmit.CATI.Backend.WcfServices.Internal.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Replication
{
    [TestClass]
    public class ReplicationServiceTest : BaseMockedIntegrationTest
    {
        public override void OnPostTestInitialize()
        {
            TestingFramework.RegistryStub<IQuotaInfoService, StubIQuotaInfoService>().GetQuotaInfosInt32 = id => new QuotaInfo[] { };
        }

        [TestMethod, Owner(@"FIRM\SvetlanaT"), Bug(39633)]
        public void SurveyWithQ1AndQ2_ReadQ1Null_ReadSuccessed()
        {
            int surveySid = ReplicationTools.AddSurvey();
            string projectId = SurveyRepository.GetById(surveySid).Name;
            var testData = ReplicationTools.GetTestData();

            new ManagementService().UpdateSurveyReplicationScheme(projectId, testData);

            //no data for Var1 in the ReplicatedData table

            Assert.IsNull(ReplicationService.GetReplicationValue(surveySid, 15, "Var1"));
        }
    }
}
