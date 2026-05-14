using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RunTestParallelUtility.TestProject
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }
        
         [ClassInitialize]
         public static void ClassInitialize(TestContext testContext) 
         { 
         }
        
         [ClassCleanup]
         public static void ClassCleanup() 
         { 
         }
        
         [TestInitialize]
         public void TestInitialize() 
         { 
         }
        
         [TestCleanup]
         public void TestCleanup() 
         { 
         }
        

        [TestMethod, Owner(@"FIRM\Test")]
        public void TestMethod11()
        {           
        }

        [TestMethod, Owner(@"FIRM\Test")]
        public void TestMethod12()
        {
        }
    }
}
