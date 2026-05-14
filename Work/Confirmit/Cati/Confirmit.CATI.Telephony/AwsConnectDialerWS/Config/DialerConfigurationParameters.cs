namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Config
{
    public class DialerConfigurationParameters
    {
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsRegion { get; set; }
        public string AwsPublicApiUrl { get; set; }
        public string AwsConnectId { get; set; }
        public string AwsContactFlowId { get; set; }
        public string AwsCallStatusQueueUrl { get; set; }
    }
}
