using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RunTestParallelUtility.UnitTests
{
    [TestClass]
    public class EngineTest
    {
        private Engine _engine;
        private ILogger _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new TempLogger();
            _engine = new Engine(_logger, new FakeParametersParser());
        }

        [TestMethod]
        public void GetLogFilePathFromArgs_SendStringWithResultsfileInformation_Success()
        {
            const string str = "/nologo /detail:duration /detail:owner /detail:errormessage /runconfig:C:\\TestRun.testsettings /resultsfile:c:\\Folder Name\\TestResult.trx /testcontainer:C:\\DllWithTests.dll /test:Tz0AndTimeOutOfShift_RecallAfter15Min_TimeInShift ";
            string logPath = _engine.GetLogFilePathFromArgs(str, true);
            Assert.AreEqual("c:\\Folder Name\\TestResult_Output.txt", logPath);
        }

        [TestMethod]
        public void GetLogFilePathFromArgs_SendStringWithPublishresultsfileInformation_Success()
        {
            const string str = "/publishresultsfile:c:\\Folder Name\\TestResult.trx /publish:http://fi-osl-tfs:8080/tfs/DefaultCollection /publishbuild:vstfs:///Build/Build/29766 /teamproject:Confirmit \"/platform:Any CPU\" /flavor:Debug ";
            string logPath = _engine.GetLogFilePathFromArgs(str, false);
            Assert.AreEqual("c:\\Folder Name\\TestResult_Error.txt", logPath);
        }
    }
}
