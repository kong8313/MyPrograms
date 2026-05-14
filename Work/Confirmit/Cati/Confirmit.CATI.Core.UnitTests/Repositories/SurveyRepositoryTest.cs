using Confirmit.CATI.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.Repositories
{
    [TestClass]
    public class SurveyRepositoryTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void SurveyIdIsZero_GetSurveyNameOrErrorString_ExpectedErrorStringIsReturned()
        {
            var target = new SurveyRepository();

            const string expectedErrorString = "unknown (0)";

            var actualErrorString = target.GetSurveyNameOrErrorString(0);

            Assert.AreEqual(expectedErrorString, actualErrorString, "Error string is not as expected");
        }
    }
}