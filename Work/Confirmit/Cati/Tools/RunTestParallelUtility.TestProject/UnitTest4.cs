using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RunTestParallelUtility.TestProject
{
    [TestClass]
    public class UnitTest4
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }


        [TestMethod, Owner(@"FIRM\Test")]
        public void TestMethod41()
        {
        }

        [TestMethod, Owner(@"FIRM\Test"), CannotWorkInParallel]
        public void TestMethod42()
        {
        }

        [TestMethod, Owner(@"FIRM\Test"), CannotWorkInParallel]
        public void TestMethod43()
        {
        }
    }
}
