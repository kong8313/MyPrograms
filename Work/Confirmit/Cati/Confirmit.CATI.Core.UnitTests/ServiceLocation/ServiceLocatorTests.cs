using Confirmit.CATI.Common.ServiceLocation;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ServiceLocation
{
    public interface ITestDependency
    {
        string DependencyValue { get; set;  }
    }

    public class TestDependency : ITestDependency
    {
        public string DependencyValue { get; set; }
    }

    [TestClass]
    public class ServiceLocatorTests
    {
        #region Initialize and Cleanup methods

        private IServiceInitializer _serviceInitializer;
        private IServiceRegistrator _serviceRegistrator;
        private IServiceResolver _serviceResolver;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceLocator = new ServiceLocator();
            _serviceInitializer = serviceLocator;
            _serviceRegistrator = serviceLocator;
            _serviceResolver = serviceLocator;
            
            _serviceInitializer.Cleanup();
            _serviceInitializer.Initialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _serviceInitializer.Cleanup();
        }

        public TestContext TestContext { get; set; }

        #endregion
        
        [TestMethod, Owner(@"FIRM\EgorS")]
        public void ServiceRegistrator_RegisterSingleton_BehavesAsSingleton()
        {
            _serviceRegistrator.RegisterSingleton<ITestDependency, TestDependency>();

            var resolveResult1 = _serviceResolver.Resolve<ITestDependency>();
            resolveResult1.DependencyValue = "Value1";

            var resolveResult2 = _serviceResolver.Resolve<ITestDependency>();

            Assert.AreEqual(resolveResult1.DependencyValue, resolveResult2.DependencyValue);
            Assert.AreSame(resolveResult1, resolveResult2);
        }
    }
}
