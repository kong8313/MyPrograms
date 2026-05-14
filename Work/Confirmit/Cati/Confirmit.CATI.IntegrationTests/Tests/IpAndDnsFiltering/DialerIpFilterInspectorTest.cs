﻿using System;
using System.Net;
using System.ServiceModel.Channels;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Cache;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
﻿using Confirmit.CATI.Core.IpLockDown.Resolvers;
﻿using Confirmit.CATI.Core.IpLockDown.Resolvers.Fakes;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated.Fakes;
 using Confirmit.CATI.IntegrationTests.Tests.IpAndDnsFiltering.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.IpAndDnsFiltering
{
    [TestClass]
    public class DialerIpFilterInspectorTest : BaseMockedIntegrationTest
    {
        private IpFilteringEngine _ipFilteringEngine;

        public override void OnPostTestInitialize()
        {
            BvDialersAdapter.Insert(new BvDialersEntity
            {
                Id = 1,
                Name = "TestName",
                WhiteList = "test1.com;255.255.255.3;"
            });

            BvDialersAdapter.Insert(new BvDialersEntity
            {
                Id = 2,
                Name = "TestName2",
                WhiteList = "test2.com"
            });

            _ipFilteringEngine = new IpFilteringEngine();
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void DialerIpFiltering_CheckDnsForFirstDiler_ResetCacheSuccess()
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

            var dialerIpFilterInspector = ServiceLocator.Resolve<DialerIpFilterInspector>();

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.241", 5432)
                );
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(1, callCount);

            var dialerTableTrigger = ServiceLocator.ResolveByName<IAsynchronousTrigger>("BvDialersTrigger");
            dialerTableTrigger.OnTableChanged(new TriggerMessage());

            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(2, callCount);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void DialerIpFiltering_CheckDnsForSecondDiler_ResetSuccess()
        {
            int callCount = 0;
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address =>
                {
                    if (address.ToString().Equals("244.243.242.241"))
                    {
                        callCount++;
                        return new IPHostEntry { HostName = "test2.com" };
                    }
                    throw new Exception();
                }
            };
            ServiceLocator.RegisterInstance<IIpHostEntryResolver>(ipHostEntryResolver);

            var dialerIpFilterInspector = ServiceLocator.Resolve<DialerIpFilterInspector>();

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.241", 5432)
                );
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(1, callCount);

            var dialerTableTrigger = ServiceLocator.ResolveByName<IAsynchronousTrigger>("BvDialersTrigger");
            dialerTableTrigger.OnTableChanged(new TriggerMessage());

            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(2, callCount);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAndDnsFiltering_ResetCacheByDialerTrigger_ResetSuccess()
        {
            _ipFilteringEngine.UpdateAccessAllowedIpAddresses("test3.com");

            int callCount = 0;
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address =>
                {
                    if (address.ToString().Equals("244.243.242.241"))
                    {
                        callCount++;
                        return new IPHostEntry { HostName = "test3.com" };
                    }
                    throw new Exception();
                }
            };
            ServiceLocator.RegisterInstance<IIpHostEntryResolver>(ipHostEntryResolver);

            var dialerIpFilterInspector = ServiceLocator.Resolve<DialerIpFilterInspector>();

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.241", 5432)
                );
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(1, callCount);

            var dialerTableTrigger = ServiceLocator.ResolveByName<IAsynchronousTrigger>("BvDialersTrigger");
            dialerTableTrigger.OnTableChanged(new TriggerMessage());

            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(2, callCount);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAndDnsFiltering_ResetCacheByBvSystemSettings_ResetSuccess()
        {
            int callCount = 0;
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address =>
                {
                    if (address.ToString().Equals("244.243.242.241"))
                    {
                        callCount++;
                        return new IPHostEntry { HostName = "test3.com" };
                    }
                    throw new Exception();
                }
            };
            ServiceLocator.RegisterInstance<IIpHostEntryResolver>(ipHostEntryResolver);

            var dialerIpFilterInspector = ServiceLocator.Resolve<DialerIpFilterInspector>();

            _ipFilteringEngine.UpdateAccessAllowedIpAddresses("test3.com");

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.241", 5432)
                );
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);
            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(1, callCount);

            var systemSettingCache = new SystemSettingCache(ServiceLocator.Resolve<IIpFilterCache>(), new StubISqlTableUpdatedPublisher(), new StubICompanyInfo());
            systemSettingCache.Reset();

            _ipFilteringEngine.UpdateAccessAllowedIpAddresses("test3.com;test4.com");

            dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);

            Assert.AreEqual(2, callCount);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void DialerIpFiltering_WrongIpDoesnotCached_BothRequestsFailed()
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

            var dialerIpFilterInspector = ServiceLocator.Resolve<DialerIpFilterInspector>();

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("244.243.242.240", 5432)
                );

            SendRejectedRequest(dialerIpFilterInspector, message);

            // Check that the second bad request from the same IP won't be accepted
            SendRejectedRequest(dialerIpFilterInspector, message);
        }

        private void SendRejectedRequest(DialerIpFilterInspector dialerIpFilterInspector, Message message)
        {
            try
            {
                dialerIpFilterInspector.AfterReceiveRequest(ref message, null, null);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            Assert.Fail("Authorization not failed!");
        }
    }
}