using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerStartCampaignResult
    {
        public DialerErrorCode ErrorCode { get; set; }

        public int DialerId { get; set; }

        public string DialerName { get; set; }
    }
}