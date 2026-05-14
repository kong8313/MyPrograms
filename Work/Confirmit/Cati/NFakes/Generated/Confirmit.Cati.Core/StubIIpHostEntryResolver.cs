using System;
using System.Net;
using Confirmit.CATI.Core.IpLockDown.Resolvers;

namespace Confirmit.CATI.Core.IpLockDown.Resolvers.Fakes
{
    public class StubIIpHostEntryResolver : IIpHostEntryResolver 
    {
        private IIpHostEntryResolver _inner;

        public StubIIpHostEntryResolver()
        {
            _inner = null;
        }

        public IIpHostEntryResolver Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IPHostEntry ResolveByIpAddressIPAddressDelegate(IPAddress address);
        public ResolveByIpAddressIPAddressDelegate ResolveByIpAddressIPAddress;

        IPHostEntry IIpHostEntryResolver.ResolveByIpAddress(IPAddress address)
        {


            if (ResolveByIpAddressIPAddress != null)
            {
                return ResolveByIpAddressIPAddress(address);
            } else if (_inner != null)
            {
                return ((IIpHostEntryResolver)_inner).ResolveByIpAddress(address);
            }

            return default(IPHostEntry);
        }

        public delegate IPHostEntry ResolveByDnsNameStringDelegate(string dnsName);
        public ResolveByDnsNameStringDelegate ResolveByDnsNameString;

        IPHostEntry IIpHostEntryResolver.ResolveByDnsName(string dnsName)
        {


            if (ResolveByDnsNameString != null)
            {
                return ResolveByDnsNameString(dnsName);
            } else if (_inner != null)
            {
                return ((IIpHostEntryResolver)_inner).ResolveByDnsName(dnsName);
            }

            return default(IPHostEntry);
        }

    }
}