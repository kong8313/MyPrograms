using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Confirmit.CATI.Core.IpLockDown.Resolvers;

namespace Confirmit.CATI.Core.IpLockDown.Validation
{
    public class IpAddressValidator : IIpAddressValidator
    {
        private readonly IIpHostEntryResolver _ipHostEntryResolver;

        public IpAddressValidator(IIpHostEntryResolver ipHostEntryResolver)
        {
            _ipHostEntryResolver = ipHostEntryResolver;
        }

        public bool IsIpInWhiteList(WhiteList whiteList, IPAddress callerAddress, Dictionary<string, List<string>> hostName2IpsList)
        {
            if (whiteList.IpRanges.Any(addressRange => addressRange.Contains(callerAddress)))
            {
                return true;
            }

            if (IsIpInDnsWhiteList(whiteList.HostNames, callerAddress, hostName2IpsList))
            {
                return true;
            }

            return false;
        }

        private bool IsIpInDnsWhiteList(IList<string> whiteHostNames, IPAddress callerAddress, Dictionary<string, List<string>> hostName2IpsList)
        {
            if (!whiteHostNames.Any())
            {
                return false;
            }

            IPHostEntry callerHostEntry = _ipHostEntryResolver.ResolveByIpAddress((callerAddress));

            if (DoesDnsWhiteListContainHostName(callerHostEntry, whiteHostNames, hostName2IpsList))
            {
                return true;
            }

            return DoesDnsWhiteListContainIpAddress(callerAddress, whiteHostNames, hostName2IpsList);
        }

        private bool DoesDnsWhiteListContainHostName(IPHostEntry hostEntry, IList<string> whiteHostNames, Dictionary<string, List<string>> hostName2IpsList)
        {
            if (hostEntry == null)
            {
                return false;
            }

            AddHostEntryToDictionary(hostName2IpsList, hostEntry);

            return whiteHostNames.Any(n => n.Equals(hostEntry.HostName, StringComparison.OrdinalIgnoreCase));
        }

        private bool DoesDnsWhiteListContainIpAddress(IPAddress callerAddress, IList<string> whiteHostNames, Dictionary<string, List<string>> hostName2IpsList)
        {
            foreach (var hostName in whiteHostNames)
            {
                var hostEntry = _ipHostEntryResolver.ResolveByDnsName(hostName);

                if (hostEntry == null)
                {
                    continue;
                }

                AddHostEntryToDictionary(hostName2IpsList, hostEntry);

                if (hostEntry.AddressList.Contains(callerAddress))
                {
                    return true;
                }
            }

            return false;
        }

        private void AddHostEntryToDictionary(Dictionary<string, List<string>> hostName2IpsList, IPHostEntry hostEntry)
        {
            if (hostEntry.AddressList == null || hostEntry.HostName == null)
            {
                var addressList = hostEntry.AddressList == null ? "null" : string.Join(",", hostEntry.AddressList.ToList());
                var hostName = hostEntry.HostName ?? "null";

                Trace.TraceWarning("Host entry has null value in AddressList or in HostName.\r\nAddressList='{0}'\r\nHostName='{1}'",
                    addressList,
                    hostName);

                return;
            }

            if (!hostName2IpsList.ContainsKey(hostEntry.HostName))
            {
                hostName2IpsList.Add(hostEntry.HostName, new List<string>());
            }

            hostName2IpsList[hostEntry.HostName].AddRange(hostEntry.AddressList.Select(a => a.ToString()).Distinct());
        }
    }
}
