using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class CampaignControllerPreview : CampaignController
    {
        public CampaignControllerPreview(ISimulator simulator, Dialer dialer, Campaign campaign )
            : base(simulator, dialer, campaign, ConfirmitDialerInterface.DialingMode.Preview)
        {
        }
    }
}