using System.Collections.Generic;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.CallStatusConsumer
{
    public class CallStatusEvent
    {
        public ContactEventDetail Detail { get; set; }
    }

    public class ContactEventDetail
    {
        public string EventType { get; set; }
        public string Channel { get; set; }
        public string InitiationMethod { get; set; }
        public string ContactId { get; set; }
        public string AnsweringMachineDetectionStatus { get; set; }
        public Dictionary<string,string> Tags { get; set; }
    }
}
