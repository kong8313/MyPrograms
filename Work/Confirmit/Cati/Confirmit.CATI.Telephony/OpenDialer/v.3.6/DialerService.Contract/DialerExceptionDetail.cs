using System.Runtime.Serialization;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerService.Contract
{
    [DataContract]
    public class DialerExceptionDetail
    {
        [DataMember]
        public DialerErrorCode ErrorCode { get; private set; }

        [DataMember]
        public string ErrorString { get; private set; }

        public DialerExceptionDetail(DialerException dialerException)
        {
            ErrorCode = dialerException.ErrorCode;
            ErrorString = dialerException.ToString();
        }

        public override string ToString()
        {
            return "DialerExceptionDetail: ErrorCode=" + ErrorCode + ", ErrorString=" + ErrorString;
        }
    }
}