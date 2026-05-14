using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.SurveyInstances;

namespace SimulatorDialerDriver
{
    public class CampaignsManager : ICampaignsManager
    {
        private readonly ISimulator _simulator;
        private readonly Dialer _dialer;

        public ConcurrentDictionary<long, CampaignController> Campaigns = new ConcurrentDictionary<long, CampaignController>();

        public CampaignsManager(ISimulator simulator, Dialer dialer)
        {
            _simulator = simulator;
            _dialer = dialer;
        }

        public CampaignController Get(long campaignId)
        {
            if (!Campaigns.TryGetValue(campaignId, out var controller))
            {
                throw new DialerException(DialerErrorCode.UnknownCampaign, String.Format("Campaign {0} not found", campaignId));
            }

            return controller;
        }

        public IEnumerable<Campaign> GetAllCampaign()
        {
            return Campaigns.Select(x => x.Value.Campaign);
        }

        public CampaignControllerPredictive TryGetPredictive(long campaignId)
        {
            CampaignController controller;

            if (!Campaigns.TryGetValue(campaignId, out controller))
            {
                return null;
            }

            return controller as CampaignControllerPredictive;
        }

        public CampaignControllerPredictive GetPredictive(long campaignId)
        {
            var campaign = Get(campaignId);
            
            if(campaign.DialingMode != DialingMode.Predictive)
                throw new DialerException(DialerErrorCode.InvalidDialingMode, String.Format("Requested Campaign {0} doesn't predictive dial mode DialingMode = {1}", campaignId, campaign.DialingMode));

            return (CampaignControllerPredictive) campaign;
        }

        public void DestroyAll()
        {
            Campaigns.ToList().ForEach(x => x.Value.Destroy());
            Campaigns.Clear();
        }

        public void Kill(Campaign campaign)
        {
            if (Campaigns.TryRemove(campaign.CampaignId, out var controller))
            {
                controller.Destroy();
            }
        }

        public void Stop(Campaign campaign)
        {
            if (!Campaigns.TryRemove(campaign.CampaignId, out var controller))
                return;

            controller.Destroy();
        }

        public void Start(Campaign campaign)
        {
            AddSurvey(campaign);
        }

        private CampaignController AddSurvey(Campaign campaign)
        {
            try
            {
                CampaignController controller;
                lock (Campaigns)
                {
                    if (!Campaigns.TryGetValue(campaign.CampaignId, out controller))
                    {
                        controller = CreateCampaignController(campaign);
                        Campaigns.TryAdd(campaign.CampaignId, controller);
                    }
                }
                
                return controller;
            }
            catch (Exception)
            {
                _simulator.Logger.Error("SimulatorDialerDriver.AddSurvey", "campaign({0})", campaign);

                throw;
            }
        }

        private CampaignController CreateCampaignController(Campaign campaign)
        {
            switch (campaign.DialingMode)
            {
                case DialingMode.Automatic:
                case DialingMode.Preview:
                    return new CampaignControllerPreview(_simulator, _dialer, campaign);

                case DialingMode.Predictive:
                    return new CampaignControllerPredictive(_simulator, _dialer, campaign);
                default:
                    throw new Exception(string.Format("Dialing mode {0} is not supported", campaign.DialingMode));
            }
        }

        
    }
}