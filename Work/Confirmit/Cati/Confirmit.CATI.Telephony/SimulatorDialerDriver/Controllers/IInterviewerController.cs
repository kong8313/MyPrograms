using ConfirmitDialerInterface;
using SimulatorDialerDriver.Models;

namespace SimulatorDialerDriver.Controllers
{
    public interface  IInterviewerController
    {
        Interviewer Interviewer { get; }

        CallManager.CallInfoEx ActiveCall { get;}


        bool SetReady(bool isReady);
        void SetCampaignId(long campaignId);

        void Destroy();
        void SetGroups(int[] agentGroups);

        void SendNumberToAgent(int companyId, int dialerId, CallManager.CallInfoEx call);
        void Redial(int companyId, int dialerId, CallManager.CallInfoEx call);
        void CompletePreview(int companyId, int dialerId);
        void Hangup();
        void CompleteCall();
        void ConnectInboundCallToAgent(int companyId, int dialerId, CallManager.CallInfoEx call);
        void StartSectionalRecording(string label);
        void StopRecording(StopRecordingMode stopRecordingMode);
        void SetNextInterview(long nextCampaignId, long nextCallId);
    }
}