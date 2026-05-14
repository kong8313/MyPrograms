using System.Net;

namespace Confirmit.CATI.Core.IpLockDown.Resolvers
{
    public interface IIpHostEntryResolver
    {
        IPHostEntry ResolveByIpAddress(IPAddress address);

        IPHostEntry ResolveByDnsName(string dnsName);
    }
}
