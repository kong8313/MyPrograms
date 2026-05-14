using ConfirmitDialerInterface;
using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class Transfer
    {
        public int CompanyId { get; set; }
        public int DialerId { get; set; }
        public long CampaignId { get; set; }
        public string TransferId { get; set; }
        public int InitiatorAgentId { get; set; }
        public int TargetAgentId { get; set; }
        public TransferType TransferType { get; set; }
        public TransferState TransferState { get; set; }
        public CallManager.CallInfoEx Call { get; set; }
    }
}