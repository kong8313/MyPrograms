namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.OutboundCall
{
    public class OutboundCallPayload
    {
        public string InstanceId { get; set; }
        public string ContactFlowId { get; set; }
        public string SourcePhoneNumber { get; set; }
        public string DestinationPhoneNumber { get; set; }
        public string SurveyId { get; set; }
        public string SurveyUrl { get; set; }
        public string ContextId { get; set; }
        public string QueueId { get; set; }
        public string CampaignId { get; set; }
        public bool EnableAnswerMachineDetection { get; set; }
    }
}
