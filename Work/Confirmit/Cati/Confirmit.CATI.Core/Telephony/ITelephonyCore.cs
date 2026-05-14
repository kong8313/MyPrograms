using System.Collections.Generic;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony
{
    public interface ITelephonyCore
    {
        void InitializeDialers();

        void UninitializeDialers(bool releaseDialerWs);

        ICollection<DialerStartCampaignResult> StartCampaign(long campaignId, string campaignName, DialingMode dialingMode, string campaignType, string surveyParametersXml);

        void StopCampaign(long campaignId, DialingMode dialingMode);

        void KillCampaign(long campaignId, DialingMode dialingMode);

        DialerErrorCode Login(
            int dialerId,
            long campaignId,
            string agentId,
            string agentName,
            AgentType agentType,
            string agentExtension,
            string userId,
            bool isPredictive,
            bool isLocal,
            IEnumerable<KeyValuePair<string, string>> agentAttributes);

        DialerErrorCode SetCampaign(int dialerId, long campaignId, int agentId);

        DialerErrorCode Logout(int dialerId, long campaignId, bool isPredictive, string agentId);

        DialerErrorCode KillAgent(int dialerId, long campaignId, string agentId);

        DialerErrorCode GoReady(int dialerId, long campaignId, string agentId);

        DialerErrorCode GoNotReady(int dialerId, long campaignId, string agentId, string breakName);

        DialerErrorCode SendNumber(
            int dialerId,
            long campaignId,
            string agentId,
            DialingMode diallingMode,
            int groupId,
            int contactId,
            int callId,
            string phoneNumber,
            int callAgingTimeout,
            bool isRecording);

        DialerErrorCode SendNumbers(
            int dialerId,
            string requestId,
            long campaignId,
            DialingMode campaignDiallingMode,
            List<CallInfo> callList,
            int callAgingTimeout,
            bool isRecording);

        DialerErrorCode SendNumberToAgent(
            int dialerId,
            long campaignId,
            string agentId,
            DialingMode diallingMode,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording,
            string callerId,
            Dictionary<string, object> respondentVariables);

        DialerErrorCode SendNumberToAgentEx(
            int dialerId,
            long campaignId,
            string agentId,
            DialingMode dialingMode,
            int contactId,
            int callId,
            string phoneNumber,
            int callAgingTimeout,
            bool isRecording);

        DialerErrorCode Redial(
            int dialerId,
            long campaignId,
            string agentId,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording,
            string callerid);

        DialerErrorCode Hangup(int dialerId, long campaignId, string agentId, int contactId, long callId);

        DialerErrorCode CompleteCall(int dialerId, long campaignId, string agentId, int contactId, bool makeAgentReady,
            string breakName, InterviewStatus status, long callId);

        DialerErrorCode SetNextInterview(
            int dialerId,
            long currentCampaignId,
            string agentId,
            InterviewStatus currentInterviewStatus,
            long nextCampaignId,
            int nextInterviewId,
            long nextCallId);

        DialerErrorCode UpdateInterviewStatus(
            int dialerId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            InterviewStatus interviewStatus);

        DialerErrorCode SetGroups(int dialerId, long campaignId, string agentId, int[] agentGroups);

        void FlushNumbers(long campaignId, List<CallInfo> callsList);

        DialerErrorCode StartRecording(
            int dialerId,
            long campaignId,
            string agentId,
            int contactId,
            int callId,
            string label);

        DialerErrorCode StopRecording(int dialerId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode);

        DialerErrorCode StartMonitor(int dialerId, string agentId, string phoneNumber, ref string sessionId);

        DialerErrorCode StopMonitor(int dialerId, string agentId, int contactId, string sessionId);

        DialerErrorCode SetMonitorMode(int dialerId, string agentId, string sessionId, MonitorMode monitorMode);

        DialerErrorCode CompletePreview(
            int dialerId,
            long campaignId,
            string agentId,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording);

        DialerErrorCode TransferToIvr(
            int dialerId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes);

        DialerErrorCode IvrRenderVoiceXml(int dialerId, int companyId, long campaignId, int agentId, int contactId,
            string voiceXml);

        DialerErrorCode[] ConfigureInboundDdiNumbers(
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers);

        DialerErrorCode DropInboundCall(
            int dialerId,
            string inboundCallId,
            AudioMessageDescriptor audioMessageDescriptor);

        DialerErrorCode ConnectInboundCall(int dialerId,
            long campaignId,
            int agentId,
            int contactId,
            string inboundCallId,
            CallInfo callInfo,
            long[] campaignIdsToBorrowAgentsFrom,
            AudioMessageDescriptor audioMessageDescriptor);

        DialerErrorCode ConnectInboundCallToAgent(
            int dialerId,
            long campaignId,
            int agentId,
            int contactId,
            string inboundCallId,
            CallInfo callInfo,
            AudioMessageDescriptor audioMessageDescriptor);

        DialerErrorCode TransferStart(
            int dialerId,
            long campaignId,
            string transferId,
            int agentId,
            int contactId,
            TransferType transferType);

        DialerErrorCode TransferSetTarget(
            int dialerId,
            long campaignId,
            string transferId,
            int agentId,
            int contactId,
            TargetType targetType,
            string targetResource,
            bool borrowAgentsFromAllCampaigns);

        DialerErrorCode TransferSetConnectionState(
            int dialerId,
            long campaignId,
            string transferId,
            int agentId,
            int contactId,
            ConnectionState state);

        DialerErrorCode TransferComplete(
            int dialerId,
            long campaignId,
            string transferId,
            int agentId,
            int contactId);

        DialerErrorCode TransferCancel(
            int dialerId,
            long campaignId,
            string transferId,
            int agentId,
            int contactId);

        bool IsPersonModeSupported(AgentTaskChoiceMode mode, int? dialerId = null);

        bool IsReloginNeededOnSurveyChange(int? dialerId = null);

        CallOutcome TranslateOutcome(long outcome);

        bool IsHangUpSupported();

        bool IsDynamicExtensionNumberAllowed(bool isAgentLocal, int? dialerId = null);

        DialerErrorCode SetConfigurationParameters(int dialerId, string configurationParametersXml);

        DialerErrorCode ValidateCampaignParameters(string surveyParametersXml);

        void SetCampaignParameters(long campaignId, DialingMode dialingMode, string surveyParametersXml);

        DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);

        DialerErrorCode StartCustomIvrInterview(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink);
    }
}
