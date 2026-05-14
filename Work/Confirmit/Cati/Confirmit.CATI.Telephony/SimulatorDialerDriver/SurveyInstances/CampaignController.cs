using System;
using ConfirmitDialerInterface;
using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public abstract class CampaignController
    {
        protected readonly ISimulator Simulator;
        protected readonly Dialer Dialer;
        public Campaign Campaign { get; }
        public long CampaignId { get; }
        public DialingMode DialingMode { get; }

        protected CampaignController(ISimulator simulator, Dialer dialer, Campaign campaign, DialingMode dialingMode)
        {
            Simulator = simulator;
            //campaignId, 
            Dialer = dialer;
            Campaign = campaign;
            CampaignId = campaign.CampaignId;
            DialingMode = dialingMode;
        }

        public virtual void Destroy()
        {
        }
    }
}