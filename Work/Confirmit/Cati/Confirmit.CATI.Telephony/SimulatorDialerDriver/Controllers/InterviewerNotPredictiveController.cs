using System.Linq;
using System.Threading;
using Confirmit.CATI.Telephony;
using Confirmit.CATI.Telephony.SimulatorDialerDriver;
using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver.Controllers
{
    public class InterviewerNotPredictiveController : BaseInterviewerController
    {
        public InterviewerNotPredictiveController(ISimulator simulator, Dialer dialer, Interviewer interviewer)
            :base(simulator, dialer, interviewer)
        {
        }

        public override bool SetReady(bool isReady)
        {
            return true;
        }

        public override void SetCampaignId(long campaignId)
        {
            Interviewer.CampaignId = campaignId;
        }

        public override void Destroy()
        {
        }

        public override void SetGroups(int[] groups)
        {
            throw new DialerException(DialerErrorCode.InvalidDialingMode, string.Format("Agent {0} was logged not in predictive mode.", Interviewer.AgentId));
        }

        public override void CompletePreview(int companyId, int dialerId)
        {
            throw new DialerException(DialerErrorCode.InvalidDialingMode, string.Format("Agent {0} was logged not in predictive mode.", Interviewer.AgentId));
        }

        public override void SendNumberToAgent(int companyId, int dialerId, CallManager.CallInfoEx call)
        {
            Dial(companyId, dialerId, call);
        }

        public override void ConnectInboundCallToAgent(int companyId, int dialerId, CallManager.CallInfoEx call)
        {
            Dial(companyId, dialerId, call);
        }

        public override void SetNextInterview(long campaignId, long callId)
        {
            ActiveCall.CampaignId = campaignId;
            ActiveCall.Info.callId = callId;
        }
    }
}