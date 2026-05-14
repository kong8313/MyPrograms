using System.Collections.Generic;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers
{
    public class PredictiveDialerController : DialerController
    {
        public DialerController Dialer { get; }
        public SurveyController Survey { get; }
        public long CampaignId { get; }
        public List<CallInfo> Calls { get; } = new List<CallInfo>();

        public class Agent
        {
            public int Id;
            public bool Ready;
            public CallInfo Call;
            public long CampaignId;
            public string Name;
        }

        public List<Agent> Agents { get; } = new List<Agent>();

        public class Transfer
        {
            public string Id;
            public int InitiatorId;
            public CallInfo Call;
            public TransferState TransferState;
        }

        public List<Transfer> Transfers { get; } = new List<Transfer>();

        public PredictiveDialerController(DialerController dialer, SurveyController survey) :
            base(dialer.Context, dialer.Tag, dialer.Id, dialer.Behavior)
        {
            Dialer = dialer;
            Survey = survey;
            CampaignId = survey.Model.CampaignId;
        }
    }
}