using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace RunTestParallelUtility.UnitTests
{
    [TestClass]
    public class TestResultFileEngineTest
    {
        private TestResultFileEngine _engine;
        private ILogger _logger;
        private string _failedTrxFilePath;
        private string _fixedTrxFilePath;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new TempLogger();
            _engine = new TestResultFileEngine();

            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            _failedTrxFilePath = Path.Combine(assemblyPath, @"RunTestParallelUtility.TestData\FailedTestResult.trx");
            _fixedTrxFilePath = Path.Combine(assemblyPath, @"RunTestParallelUtility.TestData\FixedTestResult.trx"); ;
        }

        [TestMethod]
        public void RemoveFailedTestInfo_TrxFileWith3FailedTests2WithTheSameName_ResultTrxFileIsCorrect()
        {
            string tempTrxFilePath = _failedTrxFilePath + ".temp";
            File.Copy(_failedTrxFilePath, tempTrxFilePath, true);

            _engine.RemoveFailedTestInfo(
                new string[] 
                {
                    "Confirmit.CATI.IntegrationTests.Tests.FusionLibTest.Tests.CallAdditionTests.AddCall_InterviewInClosedCell_CallsIsNotAdded",
                    "Confirmit.CATI.IntegrationTests.Tests.SampleTest.UpdateSampleTests.ProcessSample_UpdateMode_EnableDisabledByFCDCallsDuringUpdate_CallsEnabled",
                    "Confirmit.CATI.IntegrationTests.Tests.AsyncOperations.EnableCallsAsyncOperationTest.AddCall_InterviewInClosedCell_CallsIsNotAdded"
                },
                tempTrxFilePath);

            CompareFiles(tempTrxFilePath, _fixedTrxFilePath);
        }

        private void CompareFiles(string tempTrxFilePath, string fixedTrxFilePath)
        {
            string fixedTrxFileContent = File.ReadAllText(fixedTrxFilePath);
            string tempTrxFileContent = File.ReadAllText(tempTrxFilePath);

            Assert.AreEqual(fixedTrxFileContent, tempTrxFileContent, "Looks like RemoveFailedTest method works incorrect");
        }
    }
}
