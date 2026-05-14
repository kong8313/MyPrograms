using System;
using System.Net;
using System.ServiceModel.Channels;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.IpLockDown.Resolvers;
using Confirmit.CATI.Core.IpLockDown.Resolvers.Fakes;
using Confirmit.CATI.IntegrationTests.Tests.IpAndDnsFiltering.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.IpAndDnsFiltering
{
    [TestClass]
    public class IpAndDnsFilterInspectorTest : BaseMockedIntegrationTest
    {
        private ServiceLocator _serviceLocator;
        private IpFilteringEngine _ipFilteringEngine;

        public override void OnPostTestInitialize()
        {
            _ipFilteringEngine = new IpFilteringEngine();
            _ipFilteringEngine.UpdateAccessAllowedIpAddresses("test1.com;255.255.255.3;TEST2.COM;");
            _serviceLocator = new ServiceLocator();
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAndDnsFiltering_ResetCache_ResetSuccess()
        {
            int callCount = 0;
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address =>
                {
                    if (address.ToString().Equals("244.243.242.241"))
                    {
                        callCount++;
                        return new IPHostEntry { HostName = "test1.com" };
                    }
                    throw new Exception();
                }
            };
            ServiceLocator.RegisterInstance<IIpHostEntryResolver>(ipHostEntryResolver);

            var ipFilterInspector = ServiceLocator.Resolve<IpAndDnsFilterInspector>();

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.241", 5432)
                );
            ipFilterInspector.AfterReceiveRequest(ref message, null, null);
            ipFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(1, callCount);

            var systemSettingCache = ServiceLocator.Resolve<ISystemSettingCache>();
            systemSettingCache.Reset();

            _ipFilteringEngine.UpdateAccessAllowedIpAddresses("test1.com;255.255.255.3;TEST2.COM;255.255.255.10");

            ipFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(2, callCount);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAndDnsFiltering_WrongIpDoesnotCached_BothRequestsFailed()
        {
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address => new IPHostEntry
                {
                    AddressList = new[] { address },
                    HostName = "testfirm.com"
                }
            };
            ServiceLocator.RegisterInstance<IIpHostEntryResolver>(ipHostEntryResolver);

            var ipFilterInspector = ServiceLocator.Resolve<IpAndDnsFilterInspector>();

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.240", 5432)
                );

            SendRejectedRequest(ipFilterInspector, message);

            // Check that the second bad request from the same IP won't be accepted
            SendRejectedRequest(ipFilterInspector, message);
        }

        private void SendRejectedRequest(IpAndDnsFilterInspector ipFilterInspector, Message message)
        {
            try
            {
                ipFilterInspector.AfterReceiveRequest(ref message, null, null);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            Assert.Fail("Authorization not failed!");
        }
    }
}