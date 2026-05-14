using System.Linq;

using Confirmit.CATI.Core.DAL.Framework.BulkCopy;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ServiceRegistration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.ServiceLocation
{
    [TestClass]
    public class ServicesRegistryInitializerTests
    {
        #region Initialize and Cleanup methods

        private IServiceInitializer _serviceInitializer;
        private IServiceResolver _serviceResolver;
        private IServiceRegistrator _serviceRegistrator;
        private IServicesRegistryInitializer _serviceRegistryInitializer;

        [TestInitialize]
        public void TestInitialize()
        {
            var serviceLocator = new ServiceLocator();
            _serviceInitializer = serviceLocator;
            _serviceResolver = serviceLocator;
            _serviceRegistrator = serviceLocator;
            _serviceInitializer.Cleanup();
            _serviceInitializer.Initialize();

            _serviceRegistryInitializer = new ServicesRegistryInitializer(_serviceRegistrator);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _serviceInitializer.Cleanup();
        }

        public TestContext TestContext { get; set; }

        #endregion
        
        [TestMethod, Owner(@"FIRM\EgorS")]
        public void ServicesRegistryInitialize_GetRegistries_Ok()
        {
            var registries = _serviceRegistryInitializer.GetRegistries();
            
            Assert.IsNotNull(registries);
            Assert.IsTrue(registries.Any());
        }

        [TestMethod, Owner(@"FIRM\EgorS")]
        public void ServicesRegistryInitialize_GetRegistries_RegisterRegistries()
        {
            var registries = _serviceRegistryInitializer.GetRegistries();
            _serviceRegistryInitializer.RegisterRegistries(registries);

            // Try to resolve any basec type
            var bulkCopy = _serviceResolver.Resolve<IBulkCopy>();
            Assert.IsNotNull(bulkCopy);
        }
    }
}
