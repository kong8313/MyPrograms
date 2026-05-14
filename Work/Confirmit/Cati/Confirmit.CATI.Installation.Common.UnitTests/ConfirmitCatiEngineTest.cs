using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Installation.Common.UnitTests
{
    [TestClass]
    public class ConfirmitCatiEngineTest
    {
        private ILogger _logger;
        private IConfirmitCatiEngine _convertCatiEngine;
        private const string TestSchemeAndHost = "http://testservername";

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new TraceLogger();
            _convertCatiEngine = new ConfirmitCatiEngine(_logger);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetServerNameFromUrl_EmptyString_LocalhostWillBeReturned()
        {
            string url = _convertCatiEngine.GetSchemeAndHostFromUrl(string.Empty);
            Assert.AreEqual("http://localhost", url, "Method has to return http://localhost, if url is empty");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetServerNameFromUrl_UrlContainsHttpAndServerNameOnly_CorrectPathWillBeReturned()
        {
            string url = _convertCatiEngine.GetSchemeAndHostFromUrl(TestSchemeAndHost);
            Assert.AreEqual(TestSchemeAndHost, url, "Method has returned wrong scheme and host");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetServerNameFromUrl_UrlContainsManyPartsInPath_CorrectPathWillBeReturned()
        {
            string url = _convertCatiEngine.GetSchemeAndHostFromUrl(TestSchemeAndHost + "/confirmit/empty.aspx");
            Assert.AreEqual(TestSchemeAndHost, url, "Method has returned wrong scheme and host");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetServerNameFromUrl_UrlHasHttpsScheme_CorrectPathWillBeReturned()
        {
            string url = _convertCatiEngine.GetSchemeAndHostFromUrl("https://testservername:9999/confirmit/empty.aspx");
            Assert.AreEqual("https://testservername:9999", url, "Method has returned wrong scheme and host");
        }
    }
}
