using System;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS
{
    public class DialerInstanceCacheItem
    {
        public AwsConnectDialer Dialer { get; internal set; }
        public DateTime ExpireAt { get; internal set; }
        public string ConfigurationParametersXml { get; set; }
        

        public DialerInstanceCacheItem(AwsConnectDialer dialer, string configurationParametersXml)
        {
            Dialer = dialer;
            ConfigurationParametersXml = configurationParametersXml;
            
            ExtendLifetime();
        }

        public bool IsExpired => DateTime.UtcNow > ExpireAt;

        public void ExtendLifetime()
        {
            ExpireAt = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5));
        }
    }
}