using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.DialerLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IDialerService = Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace DialerLibrary.UnitTests
{
    [TestClass]
    public class CodiVersionProxyFactoryTest
    {
        private ServiceLocator _serviceLocator;

        [TestInitialize]
        public void TestInitialize()
        {
            _serviceLocator = new ServiceLocator();
            _serviceLocator.Cleanup();
            _serviceLocator.Initialize();
            new SystemSettingUnitTestRegistrator().RegisterTypes(_serviceLocator);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _serviceLocator.Cleanup();
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CodiMajorVersion37_CreateProxy_CodiCoreProxy37IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var codiVersionProxy = target.Create("3.7", null, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion37CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion36_CreateProxy_CodiCoreProxy36IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.Create("3.6", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion36CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion35_CreateProxy_CodiCoreProxy35IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.Create("3.5", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion35CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion34_CreateProxy_CodiCoreProxy34IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.Create("3.4", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion34CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion33_CreateProxy_CodiCoreProxy33IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.Create("3.3", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion33CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion32_CreateProxy_CodiCoreProxy32IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.Create("3.2", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion32CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion31_CreateProxy_UnknownVersionExceptionIsThrown()
        {
            // We treat this version as unknown because of we don't have (and don't really need) a proxy for.
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            try
            {
                target.Create("3.1", dialerChannel, "", "", "", null);

                Assert.Fail("System.Exception (Unknown CODI version) was expected but not thrown.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown CODI version: [3.1]")
                {
                    // Expected. Just return.
                    return;
                }

                throw;
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion30_CreateProxy_CodiCoreProxy30IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.Create("3.0", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion30CoreProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void UnknownCodiMajorVersion_CreateProxy_UnknownVersionExceptionIsThrown()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            try
            {
                target.Create("1.1", dialerChannel, "", "", "", null);

                Assert.Fail("System.Exception (Unknown CODI version) was expected but not thrown.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown CODI version: [1.1]")
                {
                    // Expected. Just return.
                    return;
                }

                throw;
            }
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void CodiMajorVersion37_CreateRecordingProxy_CodiRecordingProxy37IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var codiVersionProxy = target.CreateRecordingProxy("3.7", null, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion37RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion36_CreateRecordingProxy_CodiRecordingProxy36IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.CreateRecordingProxy("3.6", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion36RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion35_CreateRecordingProxy_CodiRecordingProxy35IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.CreateRecordingProxy("3.5", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion35RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion34_CreateRecordingProxy_CodiRecordingProxy34IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.CreateRecordingProxy("3.4", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion34RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion33_CreateRecordingProxy_CodiRecordingProxy33IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.CreateRecordingProxy("3.3", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion33RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion32_CreateRecordingProxy_CodiRecordingProxy32IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.CreateRecordingProxy("3.2", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion32RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion31_CreateRecordingProxy_CodiRecordingProxy31IsCreated()
        {
            // We treat this version as unknown because of we don't have (and don't really need) a proxy for.
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            try
            {
                target.CreateRecordingProxy("3.1", dialerChannel, "", "", "", null);

                Assert.Fail("System.Exception (Unknown CODI version) was expected but not thrown.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown CODI version: [3.1]")
                {
                    // Expected. Just return.
                    return;
                }

                throw;
            }
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void CodiMajorVersion30_CreateRecordingProxy_CodiRecordingProxa30IsCreated()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            var codiVersionProxy = target.CreateRecordingProxy("3.0", dialerChannel, "", "", "", null);

            Assert.IsInstanceOfType(codiVersionProxy, typeof(CodiVersion30RecordingProxy), "Object of wrong type is created by CodiVersionProxyFactory");
        }

        [TestMethod, Owner(@"FIRM\alm")]
        public void UnknownCodiMajorVersion_CreateRecordingProxy_UnknownVersionExceptionIsThrown()
        {
            var target = new CodiVersionProxyFactory();

            var dialerChannel = CreateEmptyDialerChannel();

            try
            {
                target.CreateRecordingProxy("1.1", dialerChannel, "", "", "", null);

                Assert.Fail("System.Exception (Unknown CODI version) was expected but not thrown.");
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unknown CODI version: [1.1]")
                {
                    // Expected. Just return.
                    return;
                }

                throw;
            }
        }

        private static IChannelFactoryWrapper<IDialerService> CreateEmptyDialerChannel()
        {
            var configuration = new DialerChannelFactoryWrapperConfiguration("", "", "", false);
            var channelFactoryWrapperFactory = new ChannelFactoryWrapperFactory<IDialerService>();
            var dialerChannel = channelFactoryWrapperFactory.Create(configuration, null);

            return dialerChannel;
        }
    }
}
