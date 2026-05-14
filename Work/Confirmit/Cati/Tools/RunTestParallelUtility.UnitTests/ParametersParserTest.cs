using System;
using System.IO;
using System.Windows.Forms;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility.UnitTests
{
    [TestClass]
    public class ParametersParserTest
    {
        private IParameterVerifier _parameterVerifier;

        [TestInitialize]
        public void TestInitialize()
        {
            _parameterVerifier = new TestParameterVerifier();
        }

        private static void CompareIntArrays(int[] expectedArr, int[] currentArray)
        {
            if (expectedArr.Length != currentArray.Length)
            {
                Assert.Fail("Expected array and current array have different length");
            }

            for (int i = 0; i < expectedArr.Length; i++)
            {
                if (expectedArr[i] != currentArray[i])
                {
                    Assert.Fail("Expected array and current array have different values");
                }
            }
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CreateObject_NoArgumentSent_AnArgumentExceptionReturn()
        {
            try
            {
                new ParametersParser(new string[0], _parameterVerifier);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Constructor of ParametersParser without parameters must throw an ArgumentException exception");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CreateObject_WrongArgumentsSent_AnArgumentExceptionReturn()
        {
            try
            {
                new ParametersParser(new[] { "threadcount:1" }, _parameterVerifier);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Constructor of ParametersParser must ignore all parameters without '/' or '-' at the start");
        }


        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CreateObject_WrongThreadCountSent_AnArgumentExceptionReturn()
        {
            try
            {
                new ParametersParser(new[] { "/threadcount:0" }, _parameterVerifier);
            }
            catch (ArgumentException)
            {
                return;
            }

            Assert.Fail("Constructor of ParametersParser must throw an ArgumentException exception, if /threadcount parameter is 0 or less");
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CreateObject_OnlyThreadCountSent_Success()
        {
            var paramParser = new ParametersParser(new[] { "/threadcount:10" }, _parameterVerifier);
            Assert.AreEqual(10, paramParser.ThreadCount);
            CompareIntArrays(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, paramParser.ValidThreadNumbers);
            Assert.IsTrue(paramParser.MsTestParameterString.StartsWith("/nologo /detail:duration /detail:owner /detail:errormessage /testsettings:\""));
            Assert.IsTrue(paramParser.MsTestParameterString.EndsWith("..\\Temp.testsettings\" "));
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void CreateObject_AllParametersSent_Success()
        {
            string tempFile1Path = Path.Combine(Application.StartupPath, "TempTest.dll");
            string tempFile2Path = Path.Combine(Application.StartupPath, "MyTest.dll");
            const string content = "This automatically generated file for tests can by removed";
            File.WriteAllText(tempFile1Path, content);
            File.WriteAllText(tempFile2Path, content);

            try
            {
                var paramParser = new ParametersParser(
                    new[] 
                    { 
                        "/threadcount:5", 
                        "/testcontainers:*Test.dll", 
                        "/threadlist:0,1-3,5"
                    },
                    _parameterVerifier);
                Assert.AreEqual(5, paramParser.ThreadCount, "Wrong generation of threadcount parameter");
                CompareIntArrays(new[] { 0, 1, 2, 3, 5 }, paramParser.ValidThreadNumbers);
                Assert.IsTrue(paramParser.MsTestParameterString.StartsWith("/nologo /detail:duration /detail:owner /detail:errormessage"), "Wrong generation of MsTestParameterString variable");
                Assert.IsTrue(paramParser.MsTestParameterString.Contains("/testcontainer:\"" + tempFile1Path + "\""), "Wrong generation of testcontainer parameter");
                Assert.IsTrue(paramParser.MsTestParameterString.Contains("/testcontainer:\"" + tempFile2Path + "\""), "Wrong generation of testcontainer parameter");
            }
            finally 
            {
                File.Delete(tempFile1Path);
                File.Delete(tempFile2Path);
            }
        }
    }
}
