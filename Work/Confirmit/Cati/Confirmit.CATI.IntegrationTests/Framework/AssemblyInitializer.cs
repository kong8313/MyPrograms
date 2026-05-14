using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework
{
    /// <summary>
    /// Class needs only to initialize framework.
    /// Method AssemblyInitialize is called before all tests executed.
    /// Method AssemblyCleanup is called after all tests finished.
    /// </summary>
    [TestClass]
    public class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            IntegrationTestingFramework.Instance.FrameworkInitialize();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            IntegrationTestingFramework.Instance.FrameworkCleanup();
        }
    }
}