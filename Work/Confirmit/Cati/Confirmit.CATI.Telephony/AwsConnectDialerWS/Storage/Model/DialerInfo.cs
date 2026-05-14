using Confirmit.CATI.Telephony.AwsConnectDialerWS.Context;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage.Model
{
    public class DialerInfo : IStorageModel
    {
        public DialerContext Context { get; set; }
        public string ConfigurationParametersXml { get; internal set; }

        public DialerInfo(DialerContext context, string configurationParametersXml)
        {
            Context = context;
            ConfigurationParametersXml = configurationParametersXml;
        }
    }
}