using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SL = Confirmit.CATI.Common.ServiceLocation.ServiceLocator;

namespace Confirmit.CATI.IntegrationTests.Tests.Repositories
{
    [TestClass]
    public class SurveyRepositoryTest
    {
        

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIdIsUnknown_GetSurveyNameOrErrorString_ExpectedErrorStringIsReturned()
        {
            var target = SL.Resolve<ISurveyRepository>();

            const int surveyId = 824521;

            var expectedErrorString = "unknown (" + surveyId + ")";

            var actualErrorString = target.GetSurveyNameOrErrorString(surveyId);

            Assert.AreEqual(expectedErrorString, actualErrorString, "Error string is not as expected");
        }
    }
}