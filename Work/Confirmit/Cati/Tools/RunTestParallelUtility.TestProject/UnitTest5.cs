using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RunTestParallelUtility.TestProject
{
    [TestClass, CannotWorkInParallel]
    public class UnitTest5
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
        public void TestMethod51()
        {
        }

        [TestMethod, Owner(@"FIRM\Test")]
        public void TestMethod52()
        {
        }
    }
}