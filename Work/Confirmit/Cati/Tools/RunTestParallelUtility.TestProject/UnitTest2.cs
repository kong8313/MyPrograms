using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RunTestParallelUtility.TestProject
{
    [TestClass]
    public class UnitTest2
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
        public void TestMethod21()
        {
        }

        [TestMethod, Owner(@"FIRM\Test"), Ignore]
        public void TestMethod22()
        {
        }

        [TestMethod, Owner(@"FIRM\Test")]
        public void TestMethod23()
        {
        }
    }
}
