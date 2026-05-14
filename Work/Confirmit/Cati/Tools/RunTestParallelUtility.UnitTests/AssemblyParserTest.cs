using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RunTestParallelUtility.UnitTests
{
    [TestClass]
    public class AssemblyParserTest
    {
        public TestContext TestContext { get; set; }

        private string _testProjectPath;

        [TestInitialize]
        public void TestInitialize()
        {
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            _testProjectPath = Path.Combine(assemblyPath, "RunTestParallelUtility.TestProject.dll");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetActiveTests_GetAllTests_Success()
        {            
            Dictionary<string, TestClassInfo> testList = new AssemblyParser().GetActiveTests(new[] { _testProjectPath });

            Assert.AreEqual(3, testList.Count);

            Assert.IsTrue(testList.ContainsKey("RunTestParallelUtility.TestProject.UnitTest1"));
            Assert.IsTrue(testList.ContainsKey("RunTestParallelUtility.TestProject.UnitTest2"));
            Assert.IsTrue(testList.ContainsKey("RunTestParallelUtility.TestProject.UnitTest4"));

            Assert.AreEqual(1, testList["RunTestParallelUtility.TestProject.UnitTest1"].TestList.Length);
            Assert.AreEqual(2, testList["RunTestParallelUtility.TestProject.UnitTest2"].TestList.Length);
            Assert.AreEqual(1, testList["RunTestParallelUtility.TestProject.UnitTest4"].TestList.Length);

            Assert.AreEqual("RunTestParallelUtility.TestProject.UnitTest1.*", testList["RunTestParallelUtility.TestProject.UnitTest1"].TestList[0]);
            Assert.AreEqual("TestMethod21", testList["RunTestParallelUtility.TestProject.UnitTest2"].TestList[0]);
            Assert.AreEqual("TestMethod23", testList["RunTestParallelUtility.TestProject.UnitTest2"].TestList[1]);
            Assert.AreEqual("TestMethod41", testList["RunTestParallelUtility.TestProject.UnitTest4"].TestList[0]);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetCannotWorkInParallelTests_GetAllTests_Success()
        {
            Dictionary<string, TestClassInfo> testList = new AssemblyParser().GetCannotWorkInParallelTests(new[] { _testProjectPath });

            Assert.AreEqual(2, testList.Count);

            Assert.IsTrue(testList.ContainsKey("RunTestParallelUtility.TestProject.UnitTest4"));
            Assert.IsTrue(testList.ContainsKey("RunTestParallelUtility.TestProject.UnitTest5"));

            Assert.AreEqual(2, testList["RunTestParallelUtility.TestProject.UnitTest4"].TestList.Length);
            Assert.AreEqual(1, testList["RunTestParallelUtility.TestProject.UnitTest5"].TestList.Length);

            Assert.AreEqual("TestMethod42", testList["RunTestParallelUtility.TestProject.UnitTest4"].TestList[0]);
            Assert.AreEqual("TestMethod43", testList["RunTestParallelUtility.TestProject.UnitTest4"].TestList[1]);
            Assert.AreEqual("RunTestParallelUtility.TestProject.UnitTest5.*", testList["RunTestParallelUtility.TestProject.UnitTest5"].TestList[0]);            
        }
    }
}
