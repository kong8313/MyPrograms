using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.UnitTests.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests
{
    /// <summary>
    /// This class is used as the base for all others test in the project.
    /// It contains <see cref="TestContext"/> property.
    /// </summary>
    [TestClass]
    public class BaseTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public virtual void TestInitialize()
        {
            UnitTestsServiceLocatorInitializer.InitializeServiceLocator();
        }

        [TestCleanup]
        public virtual void TestCleanup()
        {
            UnitTestsServiceLocatorInitializer.CleanupServiceLocator();
        }

        public S RegistryStub<I, S>() where S : I, new()
        {
            var stub = new S();
            ServiceLocator.RegisterSingleton<I>(stub);
            return stub;
        }
    }
}
