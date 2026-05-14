using Confirmit.CATI.Telephony.AwsConnectDialerWS.Context;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage.Model
{
    public class RespondentInfo : IStorageModel
    {
        public RespondentContext Context { get; set; }
        public string DestPhoneNumber { get; set; }

        public RespondentInfo(RespondentContext context, string destPhoneNumber)
        {
            Context = context;
            DestPhoneNumber = destPhoneNumber;
        }
    }
}
