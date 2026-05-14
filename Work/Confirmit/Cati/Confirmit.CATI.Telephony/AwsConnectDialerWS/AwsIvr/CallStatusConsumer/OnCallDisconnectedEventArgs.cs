namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.CallStatusConsumer
{
    public class OnCallDisconnectedEventArgs
    {
        public string ContextId { get; set; }
        public string AnsweringMachineDetectionStatus { get; set; }
    }
}