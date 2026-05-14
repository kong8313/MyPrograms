using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Confirmit.CATI.Core.IpLockDown.Resolvers
{
    public class DnsIpHostEntryResolver : IIpHostEntryResolver
    {
        public IPHostEntry ResolveByIpAddress(IPAddress address)
        {
            IPHostEntry result = null;
            try
            {
                result = Dns.GetHostEntry(address);
            }
            catch (SocketException se)
            {
                Trace.TraceError($"Cannnot get host entry for ip address {address}.\r\nException: {se}.\r\nSocket exception details:\r\nErrorCode={se.ErrorCode}, SocketErrorCode={se.SocketErrorCode}, NativeErrorCode={se.NativeErrorCode}");
            }
            catch (Exception e)
            {
                // TODO: Need to think about better exception processing approach.
                Trace.TraceError("Cannnot get host entry for ip address {0}. Exception: {1}", address, e);
            }

            return result;
        }

        public IPHostEntry ResolveByDnsName(string dnsName)
        {
            IPHostEntry result = null;
            try
            {
                result = Dns.GetHostEntry(dnsName);
            }
            catch (SocketException se)
            {
                Trace.TraceError($"Cannnot get host entry for dns name {dnsName}.\r\nException: {se}.\r\nSocket exception details:\r\nErrorCode={se.ErrorCode}, SocketErrorCode={se.SocketErrorCode}, NativeErrorCode={se.NativeErrorCode}");
            }
            catch (Exception e)
            {
                // TODO: Need to think about better exception processing approach.
                Trace.TraceError("Cannnot get host entry for dns name: {0}. Exception: {1}", dnsName, e);
            }

            return result;
        }
    }
}
