using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.SurveyInstances;

namespace SimulatorDialerDriver
{
    public interface ICampaignsManager
    {
        CampaignController Get(long campaignId);
        CampaignControllerPredictive TryGetPredictive(long campaignId);
        CampaignControllerPredictive GetPredictive(long campaignId);
        void DestroyAll();
        void Kill(Campaign campaign);
        void Stop(Campaign campaign);
        void Start(Campaign campaign);
    }
}