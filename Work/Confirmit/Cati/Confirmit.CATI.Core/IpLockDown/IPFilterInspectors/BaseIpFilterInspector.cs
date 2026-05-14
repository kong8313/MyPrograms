using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Confirmit.CATI.Core.IpLockDown.Validation;
using NetTools;

namespace Confirmit.CATI.Core.IpLockDown.IPFilterInspectors
{
    public class BaseIpFilterInspector : IBaseIpFilterInspector
    {
        private const char Separator = ';';

        private readonly IIpAddressValidator _ipAddressValidator;

        public BaseIpFilterInspector(IIpAddressValidator ipAddressValidator)
        {
            _ipAddressValidator = ipAddressValidator;
        }

        public WhiteList ParseWhiteList(List<string> whiteAddressList)
        {
            var whiteList = new WhiteList();

            foreach (var whiteAddress in whiteAddressList)
            {
                if (string.IsNullOrWhiteSpace(whiteAddress))
                {
                    continue;
                }

                string[] whiteListArray = whiteAddress.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var listEntity in whiteListArray)
                {
                    if (string.IsNullOrWhiteSpace(listEntity))
                    {
                        continue;
                    }

                    string trimmedListEntry = listEntity.Trim();

                    IPAddressRange addressRange;
                    if (IPAddressRange.TryParse(RemoveInterfaceInfo(trimmedListEntry), out addressRange))
                    {
                        whiteList.IpRanges.Add(addressRange);
                        continue;
                    }

                    if (Uri.CheckHostName(trimmedListEntry) == UriHostNameType.Dns)
                    {
                        whiteList.HostNames.Add(trimmedListEntry);
                        continue;
                    }

                    Trace.TraceError("Can't convert address \"{0}\" to IP address or DNS name.", trimmedListEntry);
                }
            }

            return whiteList;
        }

        /// <summary>
        /// Remove information about interface from ipv6 like the following:
        /// 2001:db8:11a3:9d7:1f34:8a2e:7a0:765d%eth1 -> 2001:db8:11a3:9d7:1f34:8a2e:7a0:765d
        /// </summary>
        /// <param name="trimmedListEntry"></param>
        /// <returns></returns>
        internal string RemoveInterfaceInfo(string trimmedListEntry)
        {
            var fixedAddresses = trimmedListEntry.Split('-').Select(FixAddress);
            return string.Join("-", fixedAddresses);
        }

        string FixAddress(string address)
        {
            if (!address.Contains('%'))
            {
                return address;
            }

            if (!address.Contains('/'))
            {
                return address.Split('%')[0].Trim();
            }

            return address.Split('%')[0].Trim() + '/' + address.Split('/')[1].Trim();
        }

        public object AfterReceiveRequest(Message request, IpFilterCacheData ipFilterCacheData)
        {
            IPAddress address = null;
            string xForwardedFor = null;
            string remoteEndpointAddress = null;

            if (request.Properties.Keys.Contains(HttpRequestMessageProperty.Name))
            {                
                HttpRequestMessageProperty httpRequest = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
                if (httpRequest != null)
                {
                    xForwardedFor = httpRequest.Headers["X-Forwarded-For"];
                    IPAddress.TryParse(xForwardedFor, out address);
                }
            }

            RemoteEndpointMessageProperty remoteEndpoint = request.Properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            if (remoteEndpoint != null)
            {
                remoteEndpointAddress = remoteEndpoint.Address;
                if (address == null)
                {
                    IPAddress.TryParse(remoteEndpointAddress, out address);
                }                
            }

            if (address != null)
            {
                if (IPAddress.IsLoopback(address))
                {
                    address = IPAddress.Parse("127.0.0.1");
                }
                
                // If ip address is denied clear the request mesage so service method does not get execute
                var hostNames2IpsList = new Dictionary<string, List<string>>();
                if (!VerifyAddress(address, ipFilterCacheData, hostNames2IpsList))
                {
                    var accessDeniedMessage = string.Format(
                        "{0}\r\n\r\n\r\nRequest:\r\n{1}",
                        GetAccessDeniedLogMessage(address, xForwardedFor, remoteEndpointAddress, ipFilterCacheData.WhiteList, hostNames2IpsList),
                        request);

                    Trace.TraceError(accessDeniedMessage);

                    if (OperationContext.Current != null)
                    {
                        var responseProperty = new HttpResponseMessageProperty
                        {
                            StatusCode = HttpStatusCode.Unauthorized
                        };
                        OperationContext.Current.OutgoingMessageProperties["httpResponse"] = responseProperty;
                    }

                    throw new UnauthorizedAccessException(accessDeniedMessage);
                }
            }

            return null;
        }

        /// <summary>
        /// Verify, that IP address is valid
        /// </summary>
        /// <param name="callerAddress">IP address</param>
        /// <param name="ipFilterCacheData">Information about caches</param>
        /// <param name="hostNames2IpsList">Information about resolved dns names and IP</param>
        /// <returns></returns>
        private bool VerifyAddress(IPAddress callerAddress, IpFilterCacheData ipFilterCacheData, Dictionary<string, List<string>> hostNames2IpsList)
        {
            if (ipFilterCacheData.ValidatedIpsCache.Contains(callerAddress))
            {
                return true;
            }

            bool isIpInWhiteList = _ipAddressValidator.IsIpInWhiteList(ipFilterCacheData.WhiteList, callerAddress, hostNames2IpsList);
            if (isIpInWhiteList)
            {
                ipFilterCacheData.ValidatedIpsCache.Add(callerAddress);
            }

            return isIpInWhiteList;
        }

        private string GetAccessDeniedLogMessage(IPAddress callerAddress, string xForwardedFor, string remoteEndpointAddress, WhiteList whiteList, Dictionary<string, List<string>> hostNames2IpsList)
        {
            string ipsMessageString = GetIPsAccessDeniedLogMessage(whiteList.IpRanges);
            string dnsMessageString = GetDnsAccessDeniedLogMessage(whiteList.HostNames, hostNames2IpsList);

            return string.Format(
                "Access denied for IP: {0} (DNS name: '{1}', X-Forwarded-For header value: '{2}', remote endpoint address: '{3}').{4}{5}",
                callerAddress,
                GetResolvedDnsNameByIp(hostNames2IpsList, callerAddress),
                xForwardedFor,
                remoteEndpointAddress,
                ipsMessageString,
                dnsMessageString);
        }

        private string GetIPsAccessDeniedLogMessage(IList<IPAddressRange> whiteIpRanges)
        {
            if (!whiteIpRanges.Any())
            {
                return string.Empty;
            }

            return string.Format("\r\nAllowed IPs [{0}].", string.Join(",", whiteIpRanges));
        }

        private string GetDnsAccessDeniedLogMessage(IList<string> whiteHostNames, Dictionary<string, List<string>> hostNames2IpsList)
        {
            if (!whiteHostNames.Any())
            {
                return string.Empty;
            }

            var dnsList = whiteHostNames.Select(
                    dns => string.Format(
                        "\r\n\t{0} (IPs: [{1}])",
                        dns,
                        !hostNames2IpsList.ContainsKey(dns) ?
                            string.Empty : string.Join(", ", hostNames2IpsList[dns])
                        )
                    );

            return string.Format("\r\nAllowed DNS names: [{0}].", string.Join(", ", dnsList));
        }

        private string GetResolvedDnsNameByIp(Dictionary<string, List<string>> hostNames2IpsList, IPAddress callerAddress)
        {
            var ipAddressString = callerAddress.ToString();
            return hostNames2IpsList
                .Where(k => k.Value.Contains(ipAddressString))
                .Select(k => k.Key)
                .FirstOrDefault() ?? string.Empty;
        } 
    }
}