using Confirmit.Configuration;
using Confirmit.IPLockdown;

namespace Confirmit.CATI.Supervisor.Core.Security
{
    public class OidcManager
    {
        private readonly string _remoteIpAddr;
        private readonly string _ipRange;

        public OidcManager(string remoteIpAddr)
        {
            _remoteIpAddr = remoteIpAddr;
            _ipRange = ConfirmitConfiguration.GetStringValue("OpenIdConnectIpRange", null);
        }

        public bool ShouldUseOidc()
        {
            if (string.IsNullOrWhiteSpace(_ipRange)) return false;

            IPAddressRangeCollection ipRanges;
            var ipRangesAreValid = IPAddressRangeCollection.TryParse(_ipRange, out ipRanges);
            var shouldUseOidc = ipRangesAreValid && ipRanges.Contains(_remoteIpAddr);
            return shouldUseOidc;
        }
    }
}