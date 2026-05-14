using System;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Config
{
    public class DialerConfigurationException : Exception
    {
        public DialerConfigurationException(string message) : base(message)
        {
        }
    }
}