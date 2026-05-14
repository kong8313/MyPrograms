using Confirmit.CATI.Common.SideBySide;
using Confirmit.Test.Common.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Common.UnitTests.SideBySide
{
    [TestClass]
    public class SideBySideManagerTest
    {
        private SideBySideManager _sideBySideManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _sideBySideManager = new SideBySideManager();
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), Bug(59307)]
        public void AddSideBySideNameToUrl_SendUrlWithPort_CorrectUrlReturn()
        {
            const string testUrl = "http://localhost:81/MonitoringInterviewerMultimodeInstance";

            string expectedUrl = "http://localhost:81/" + _sideBySideManager.SideBySideName + "/MonitoringInterviewerMultimodeInstance";

            string actualUrl = _sideBySideManager.AddSideBySideNameToBackendWCFServiceUrl(testUrl);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK"), Bug(59307)]
        public void AddSideBySideNameToUrl_SendUrlWithoutPort_CorrectUrlReturn()
        {
            const string testUrl = "http://localhost/DialerMultimodeInstance";

            string expectedUrl = "http://localhost/" + _sideBySideManager.SideBySideName + "/DialerMultimodeInstance";

            string actualUrl = _sideBySideManager.AddSideBySideNameToBackendWCFServiceUrl(testUrl);

            Assert.AreEqual(expectedUrl, actualUrl);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void AddSideBySideNameToServiceName_ServiceNameWithoutSideBySideNameSend_CorrectServiceNameReturn()
        {
            const string serviceName = "Confirmit.CATI.Backend$1";

            string expectedServiceName = "Confirmit.CATI.Backend." + _sideBySideManager.SideBySideName + "$1";

            string actualServiceName = _sideBySideManager.AddSideBySideNameToServiceName(serviceName);

            Assert.AreEqual(expectedServiceName, actualServiceName);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void RemoveSideBySideNameFromServiceName_ServiceNameWithSideBySideNameSend_CorrectServiceNameReturn()
        {
            string serviceName = "Confirmit.CATI.Backend." + _sideBySideManager.SideBySideName + "$22";
            const string expectedServiceName = "Confirmit.CATI.Backend$22";

            string actualServiceName = _sideBySideManager.RemoveSideBySideNameFromServiceName(serviceName);

            Assert.AreEqual(expectedServiceName, actualServiceName);
        }

        [TestMethod, Owner(@"FIRM\MikhailT")]
        public void AddSideBySideNameToDialerServiceName_DialerServiceNameWithoutSideBySideNameSend_CorrectDialerServiceNameReturn()
        {
            const string serviceName = "http://localhost/TciDialerService/BvTciDialer.svc";

            string expectedServiceName = "http://localhost/TciDialerService." + _sideBySideManager.SideBySideName + "/BvTciDialer.svc";

            string actualServiceName = _sideBySideManager.AddSideBySideNameToIISServiceUrl(serviceName);

            Assert.AreEqual(expectedServiceName, actualServiceName);
        }
    }
}
