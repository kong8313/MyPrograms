using Confirmit.CATI.Telephony.AwsConnectDialerWS.Context;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage.Model
{
    public class SurveyInfo : IStorageModel
    {
        public SurveyContext Context { get; set; }
        public string SourcePhoneNumber { get; internal set; }
        public string ConnectQueueId { get; set; }
        public bool EnableAnswerMachineDetection { get; set; }

        public SurveyInfo(SurveyContext context, string sourcePhoneNumber, string connectQueueId, bool enableAnswerMachineDetection)
        {
            Context = context;
            SourcePhoneNumber = sourcePhoneNumber;
            ConnectQueueId = connectQueueId;
            EnableAnswerMachineDetection = enableAnswerMachineDetection;
        }
    }
}