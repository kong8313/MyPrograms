using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.Fakes;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptCacheTest : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\EgorS")]
        [ClassData(typeof(TestDataGenerator))]
        public void SurveyExists_RelaunchSurvey_CacheIsReset(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData {Tag = "S1", IsUseDb = true}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            
            bool cacheReset = false;

            var stub = TestingFramework.RegistryStub<ISurveyMetadataCacheService, StubISurveyMetadataCacheService>();
            stub.ResetSurveyCacheInt32 = (id) => { cacheReset = true; };

            survey.Launch();

            Assert.IsTrue(cacheReset);
        }
    }
}
