using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Channels;
using Confirmit.CATI.Core.IpLockDown;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.IpLockDown.Resolvers.Fakes;
using Confirmit.CATI.Core.IpLockDown.Validation;

namespace Confirmit.CATI.IntegrationTests.Tests.IpAndDnsFiltering
{
    [TestClass]
    public class BaseIpFilterInspectorTest : BaseMockedIntegrationTest
    {
        private IBaseIpFilterInspector _baseIpFilterInspector;
        private IpFilterCacheData _ipFilterCacheData;

        public override void OnPostTestInitialize()
        {
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = ResolveByIpAddress
            };

            BvDialersAdapter.Insert( new BvDialersEntity
            {
                Id = 1,
                Name = "TestName",
                WhiteList = "test1.com;255.255.255.3;TEST2.COM;"
            });

            _baseIpFilterInspector = new BaseIpFilterInspector(new IpAddressValidator(ipHostEntryResolver));

            var whiteList = _baseIpFilterInspector.ParseWhiteList(new List<string> { "test1.com;255.255.255.3;TEST2.COM;" });
            _ipFilterCacheData = new IpFilterCacheData(whiteList);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void AfterReceiveRequest_FilterByDns_Success()
        {
            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("255.255.255.1", 5432)
                );
            _baseIpFilterInspector.AfterReceiveRequest(message, _ipFilterCacheData);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void AfterReceiveRequest_FilterByDnsInUpperCase_Success()
        {
            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("255.255.255.249", 5432)
                );
            _baseIpFilterInspector.AfterReceiveRequest(message, _ipFilterCacheData);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void AfterReceiveRequest_FilterByIp_Success()
        {
            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("255.255.255.3", 5432)
                );
            _baseIpFilterInspector.AfterReceiveRequest(message, _ipFilterCacheData);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void AfterReceiveRequest_FilterByIp_Failed()
        {
            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty("255.255.255.4", 5432)
                );

            _baseIpFilterInspector.AfterReceiveRequest(message, _ipFilterCacheData);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void AfterReceiveRequest_FilterByWhiteListIps_Success()
        {
            const string validIp = "255.255.255.33";
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address => new IPHostEntry { HostName = "wronghostname.org" },
                ResolveByDnsNameString = dnsName =>
                {
                    if (dnsName.Equals("validhostname.com"))
                    {
                        return new IPHostEntry
                        {
                            HostName = "wronghostname.org",
                            AddressList = new[]
                            {
                                IPAddress.Parse("255.255.255.3"),
                                IPAddress.Parse(validIp)
                            }
                        };
                    }
                    return null;
                }
            };

           var baseIpFilterInspector = new BaseIpFilterInspector(new IpAddressValidator(ipHostEntryResolver));

           var whiteList = baseIpFilterInspector.ParseWhiteList(new List<string> { "validhostname.com" });
           var ipFilterDataCache = new IpFilterCacheData(whiteList);

            var message = Message.CreateMessage(MessageVersion.Default, "urn:Action");
            message.Properties.Add(
                RemoteEndpointMessageProperty.Name,
                new RemoteEndpointMessageProperty(validIp , 5432)
                );

            baseIpFilterInspector.AfterReceiveRequest(message, ipFilterDataCache);
        }

        private IPHostEntry ResolveByIpAddress(IPAddress address)
        {
            if (address.ToString().Equals("255.255.255.1"))
            {
                return new IPHostEntry { HostName = "test1.com" };
            }

            if (address.ToString().Equals("255.255.255.249"))
            {
                return new IPHostEntry { HostName = "test2.com" };
            }

            return new IPHostEntry
            {
                AddressList = new [] { address },
                HostName = "testfirm.com"
            };
        }
    }
}
