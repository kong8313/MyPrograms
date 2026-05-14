using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using Confirmit.CATI.Core.IpLockDown;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.IpLockDown.Resolvers.Fakes;
using Confirmit.CATI.Core.IpLockDown.Validation;
using NetTools;

namespace Confirmit.CATI.Core.UnitTests.IpAndDnsFiltering
{
    [TestClass]
    public class IpAndDnsFilteringTest
    {
        private BaseIpFilterInspector _baseIpFilterInspector;

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _baseIpFilterInspector = new BaseIpFilterInspector(null);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void BaseIpFilterInspector_ParseWhiteList_Success()
        {
            IBaseIpFilterInspector baseIpFilterInspector = new BaseIpFilterInspector(null);
            WhiteList whiteList = baseIpFilterInspector.ParseWhiteList(new List<string> { 
                "127.0.0.1;firmglobal.com;fe80::836:ff85:9c57:dcae",  
                " 255.255.255.0 ; confirmit.com ; ;" });
                    
            Assert.AreEqual("127.0.0.1", whiteList.IpRanges[0].ToString());
            Assert.AreEqual("fe80::836:ff85:9c57:dcae", whiteList.IpRanges[1].ToString());
            Assert.AreEqual("255.255.255.0", whiteList.IpRanges[2].ToString());

            Assert.AreEqual("firmglobal.com", whiteList.HostNames[0]);
            Assert.AreEqual("confirmit.com", whiteList.HostNames[1]);
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void BaseIpFilterInspector_ParseWhiteList_ReadWhiteListByEmptyString_Success()
        {
            IBaseIpFilterInspector baseIpFilterInspector = new BaseIpFilterInspector(null);
            WhiteList whiteList = baseIpFilterInspector.ParseWhiteList(new List<string> { "" });

            Assert.IsNotNull(whiteList);
            Assert.AreEqual(0, whiteList.IpRanges.Count);
            Assert.AreEqual(0, whiteList.HostNames.Count);
        }
        
        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void IpAddressValidator_ValidateIpAddresses_Success()
        {
            var validator = new IpAddressValidator(null);

            var whiteList = new WhiteList
            {
                IpRanges = new[] { IPAddressRange.Parse("244.243.242.241"), IPAddressRange.Parse("244.243.242.240") },
                HostNames = new List<string>()
            };
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.241"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.240"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void IpAddressValidator_ValidateIpAddresses_Failed()
        {
            var validator = new IpAddressValidator(null);

            var whiteList = new WhiteList
            {
                IpRanges = new[] { IPAddressRange.Parse("244.243.242.241"), IPAddressRange.Parse("244.243.242.240") },
                HostNames = new List<string>()
            };
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.242"), new Dictionary<string, List<string>>()));
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.243"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAddressValidator_ValidateOneIpAddressRange_AllWorksCorrect()
        {
            var validator = new IpAddressValidator(null);

            var whiteList = new WhiteList
            {
                IpRanges = new[] { IPAddressRange.Parse("244.243.242.240-244.243.242.242") },
                HostNames = new List<string>()
            };

            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.239"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.240"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.241"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.242"), new Dictionary<string, List<string>>()));
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.243"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAddressValidator_ValidateTwoIpAddressRanges_AllWorksCorrect()
        {
            var validator = new IpAddressValidator(null);

            var whiteList = new WhiteList
            {
                IpRanges = new[] { IPAddressRange.Parse("244.243.242.240-244.243.242.241"), IPAddressRange.Parse("244.243.242.243-244.243.242.244") },
                HostNames = new List<string>()
            };

            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.239"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.240"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.241"), new Dictionary<string, List<string>>()));
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.242"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.243"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.244"), new Dictionary<string, List<string>>()));
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.245"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAddressValidator_ValidateOneIpv6AddressRange_AllWorksCorrect()
        {
            var validator = new IpAddressValidator(null);

            var whiteList = new WhiteList
            {
                IpRanges = new[] { IPAddressRange.Parse("fe80::836:ff85:9c57:dcab-fe80::836:ff85:9c57:dcac") },
                HostNames = new List<string>()
            };

            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("fe80::836:ff85:9c57:dcaa"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("fe80::836:ff85:9c57:dcab"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("fe80::836:ff85:9c57:dcac"), new Dictionary<string, List<string>>()));
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("fe80::836:ff85:9c57:dcad"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void IpAddressValidator_ValidateAllIpv6AddressRange_AllWorksCorrect()
        {
            var validator = new IpAddressValidator(null);

            var whiteList = new WhiteList
            {
                IpRanges = new[] { IPAddressRange.Parse("::0-ffff::ffff:ffff:ffff:ffff") },
                HostNames = new List<string>()
            };

            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("0::0:0:0:0"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("fe80::836:ff85:9c57:dcab"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("ffff::ffff:ffff:ffff:ffff"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void IpAddressValidator_ValidateDnsNames_Success()
        {
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address =>
                {
                    if (address.ToString().Equals("244.243.242.241"))
                    {
                        return new IPHostEntry { HostName = "test1.com" };
                    }

                    if (address.ToString().Equals("244.243.242.240"))
                    {
                        return new IPHostEntry { HostName = "test2.com" };
                    }

                    throw new Exception();
                }
            };

            var validator = new IpAddressValidator(ipHostEntryResolver);
            var whiteList = new WhiteList
            {
                HostNames = new List<string> { "test1.com", "test2.com" },
                IpRanges = new List<IPAddressRange>()
            };

            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.241"), new Dictionary<string, List<string>>()));
            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.240"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void IpAddressValidator_ValidateDnsNamesInUpperCase_Success()
        {
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address =>
                {
                    if (address.ToString().Equals("244.243.242.241"))
                    {
                        return new IPHostEntry { HostName = "TEST1.com" };
                    }

                    throw new Exception();
                }
            };

            var validator = new IpAddressValidator(ipHostEntryResolver);
            var whiteList = new WhiteList
            {
                HostNames = new List<string> { "test1.com" },
                IpRanges = new List<IPAddressRange>()
            };

            Assert.IsTrue(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.241"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\vyacheslavb")]
        public void IpAddressValidator_ValidateDnsNames_Failed()
        {
            var ipHostEntryResolver = new StubIIpHostEntryResolver
            {
                ResolveByIpAddressIPAddress = address => null
            };

            var validator = new IpAddressValidator(ipHostEntryResolver);
            var whiteList = new WhiteList
            {
                HostNames = new List<string> { "test1.com", "test2.com" },
                IpRanges = new List<IPAddressRange>()
            };
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.241"), new Dictionary<string, List<string>>()));
            Assert.IsFalse(validator.IsIpInWhiteList(whiteList, IPAddress.Parse("244.243.242.240"), new Dictionary<string, List<string>>()));
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6_Success()
        {
            var whiteAddressList = "2001:db8:11a3:9d7:1f34:8a2e:7a0:765d";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:765d", result, "RemoveInterfaceInfo method works incorrect with ipv6 address without percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6ContainsPercent_Success()
        {
            var whiteAddressList = "2001:db8:11a3:9d7:1f34:8a2e:7a0:765d%eth1";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:765d", result, "RemoveInterfaceInfo method works incorrect with ipv6 address with percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6Range_Success()
        {
            var whiteAddressList = "2001:db8:11a3:9d7:1f34:8a2e:7a0:765d-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:765d-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f", result, "RemoveInterfaceInfo method works incorrect with ipv6 address with percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6RangeContainsPercentInSecondIp_Success()
        {
            var whiteAddressList = "2001:db8:11a3:9d7:1f34:8a2e:7a0:765d-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f%123";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:765d-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f", result, "RemoveInterfaceInfo method works incorrect with ipv6 address with percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6RangeContainsPercentInFirstIp_Success()
        {
            var whiteAddressList = "2001:db8:11a3:9d7:1f34:8a2e:7a0:765d%123-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:765d-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f", result, "RemoveInterfaceInfo method works incorrect with ipv6 address with percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6RangeContains2Percents_Success()
        {
            var whiteAddressList = "2001:db8:11a3:9d7:1f34:8a2e:7a0:765d%123-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f%123";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:765d-2001:db8:11a3:9d7:1f34:8a2e:7a0:765f", result, "RemoveInterfaceInfo method works incorrect with ipv6 address with percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6TrickyRange_Success()
        {
            var whiteAddressList = "2001:1db8:11a3:19d7:1f34:8a2e:17a0:765d/24";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:1db8:11a3:19d7:1f34:8a2e:17a0:765d/24", result, "RemoveInterfaceInfo method works incorrect with ipv6 address range with slash and without percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void RemoveInterfaceInfo_Ipv6TrickyRangeContainsPercent_Success()
        {
            var whiteAddressList = "2001:1db8:11a3:19d7:1f34:8a2e:17a0:765d%test/24";

            string result = _baseIpFilterInspector.RemoveInterfaceInfo(whiteAddressList);
            Assert.AreEqual("2001:1db8:11a3:19d7:1f34:8a2e:17a0:765d/24", result, "RemoveInterfaceInfo method works incorrect with ipv6 address range with slash and percent");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void ParseWhiteList_SeveralIpv6ContainsPercent_Success()
        {
            var whiteAddressList = new List<string> { "2001:db8:11a3:9d7:1f34:8a2e:7a0:764d%et1h;2001:db8:11a3:9d7:1f34:8a2e:7a0:764e%1eth",
                                                      "2001:db8:11a3:9d7:1f34:8a2e:7a0:7650%1et1h - 2001:db8:11a3:9d7:1f34:8a2e:7a0:765e%1eth1"};

            WhiteList whiteList = _baseIpFilterInspector.ParseWhiteList(whiteAddressList);
            Assert.AreEqual(3, whiteList.IpRanges.Count, "ParseWhiteList method works incorrect with ipv6 address with percent");
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:764d", whiteList.IpRanges[0].ToString(), "ParseWhiteList method works incorrect with ipv6 address with percent");
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:764e", whiteList.IpRanges[1].ToString(), "ParseWhiteList method works incorrect with ipv6 address with percent");
            Assert.AreEqual("2001:db8:11a3:9d7:1f34:8a2e:7a0:7650-2001:db8:11a3:9d7:1f34:8a2e:7a0:765e", whiteList.IpRanges[2].ToString(), "ParseWhiteList method works incorrect with ipv6 address with percent");
        }
    }
}
