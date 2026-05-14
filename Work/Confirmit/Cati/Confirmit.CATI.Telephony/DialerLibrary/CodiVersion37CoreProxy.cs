using System.Collections.Generic;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersion37CoreProxy : ICodiVersionCoreProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService> _dialerChannel;

        public CodiVersion37CoreProxy(IChannelFactoryWrapper<IDialerService> dialerChannel)
        {
            _dialerChannel = dialerChannel;
        }

        public string GetName()
        {
            return _dialerChannel.Execute(
                x => x.GetName());
        }

        public string GetVersion()
        {
            return _dialerChannel.Execute(
                x => x.GetVersion());
        }

        public string[] Version()
        {
            return _dialerChannel.Execute(
                x => x.Version());
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {
            return _dialerChannel.Execute(
                x => x.Initialize(
                    companyId,
                    dialerId,
                    configurationParametersXml));
        }

        public DialerFeatures GetFeatures(int companyId, int dialerId)
        {
            return _dialerChannel.Execute(x => x.GetFeatures(companyId, dialerId));
        }

        public DialerErrorCode Release(int dialerId, int companyId)
        {
            return _dialerChannel.Execute(x => x.Release(dialerId, companyId));
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            return _dialerChannel.Execute(x => x.SetConfigurationParameters(companyId, configurationParametersXml));
        }

        public DialerState GetState(int companyId, int dialerId)
        {
            return _dialerChannel.Execute(x => x.GetState(companyId, dialerId));
        }

        public DialerErrorCode StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode,
            bool recordWholeInterview, string campaignParametersXml)
        {
            return _dialerChannel.Execute(
                x => x.StartCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    campaignName,
                    dialingMode,
                    recordWholeInterview,
                    campaignParametersXml));
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return _dialerChannel.Execute(
                x => x.StopCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    dialingMode));
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return _dialerChannel.Execute(
                x => x.KillCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    dialingMode));
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview,
            string campaignParametersXml)
        {
            return _dialerChannel.Execute(
                x => x.SetCampaignParameters(
                    companyId,
                    dialerIds,
                    campaignId,
                    dialingMode,
                    recordWholeInterview,
                    campaignParametersXml));
        }

        public DialerErrorCode Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, bool isPredictive, ResourceBindingType resourceBindingType, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            return _dialerChannel.Execute(
                x => x.Login(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    agentName,
                    agentType,
                    agentConnectionString,
                    isPredictive,
                    resourceBindingType,
                    agentAttributes));
        }

        public DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {
            return _dialerChannel.Execute(
                x => x.SetCampaign(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            return _dialerChannel.Execute(
                x => x.Logout(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    isPredictive));
        }

        public DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {
            return _dialerChannel.Execute(
                x => x.KillAgent(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            return _dialerChannel.Execute(
                x => x.GoReady(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            return _dialerChannel.Execute(
                x => x.GoNotReady(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    breakName));
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            return _dialerChannel.Execute(
                x => x.SetGroups(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    agentGroups));
        }

        public DialerErrorCode SendNumberToAgent(int companyId, int dialerId, long campaignId, int agentId, DialingMode dialingMode,
            int interviewId, long callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, object> respondentVariables)
        {
            return _dialerChannel.Execute(
                x => x.SendNumberToAgent(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    dialingMode,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording,
                    callerId,
                    respondentVariables));
        }

        public DialerErrorCode Redial(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording, string callerId)
        {
            return _dialerChannel.Execute(
                x => x.Redial(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording,
                    callerId));
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode,
            List<CallInfo> callList, int callAgingTimeout)
        {
            return _dialerChannel.Execute(
                x => x.SendNumbers(
                    requestId,
                    companyId,
                    dialerId,
                    campaignId,
                    campaignDialingMode,
                    callList,
                    callAgingTimeout));
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {
            return _dialerChannel.Execute(
                x => x.Hangup(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId, 
                    callId));
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId,
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            return _dialerChannel.Execute(
                x => x.CompleteCall(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewStatus,
                    makeAgentReady,
                    breakName,
                    interviewId, 
                    callId));
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            return _dialerChannel.Execute(
                x => x.SetNextInterview(
                    companyId,
                    dialerId,
                    currentCampaignId,
                    agentId,
                    currentInterviewStatus,
                    nextCampaignId,
                    nextInterviewId,
                    nextCallId));
        }

        public DialerErrorCode StartCustomIvrInterview(int companyId, int dialerId, long campaignId, int agentId, 
            int interviewId, long callId, string respondentSurveyLink)
        {
            return _dialerChannel.Execute(
                x => x.StartCustomIvrInterview(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    respondentSurveyLink));
        }
        
        public DialerErrorCode UpdateInterviewStatus(int companyId, int dialerId, long campaignId, int agentId, int interviewId,
            long callId, InterviewStatus interviewStatus)
        {
            // UpdateInterviewStatus is obsolete. Just return Success.
            //TODO: Get rid of UpdateInterviewStatus at upper level

            return DialerErrorCode.Success;
        }

        public DialerErrorCode CompletePreview(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording)
        {
            return _dialerChannel.Execute(
                x => x.CompletePreview(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording));
        }

        public DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {
            return _dialerChannel.Execute(
                x => x.FlushNumbers(
                    companyId,
                    dialerIds,
                    campaignId,
                    callList));
        }

        public DialerErrorCode StartRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string label)
        {
            return _dialerChannel.Execute(
                x => x.StartRecording(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    label));
        }

        public DialerErrorCode StopRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            StopRecordingMode stopRecordingMode)
        {
            return _dialerChannel.Execute(
                x => x.StopRecording(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    stopRecordingMode));
        }

        public DialerErrorCode StartPlayback(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string fileName, out int timeOfPlayingInSeconds)
        {
            int outTimeOfPlayingInSeconds = 0;

            var result = _dialerChannel.Execute(
                x => x.StartPlayback(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    fileName,
                    out outTimeOfPlayingInSeconds));

            timeOfPlayingInSeconds = outTimeOfPlayingInSeconds;

            return result;
        }

        public DialerErrorCode StopPlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return _dialerChannel.Execute(
                x => x.StopPlayback(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode PauseOrResumePlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return _dialerChannel.Execute(
                x => x.PauseOrResumePlayback(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId, int agentId,
            long callId)
        {
            return _dialerChannel.Execute(
                x => x.ToggleInterviewerListensToPlaybackOrRespondent(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode StartMonitor(int companyId, int dialerId, int agentId, string supervisorName,
            string supervisorConnectionString, ResourceBindingType resourceBindingType, ref string sessionId)
        {
            var outSessionId = sessionId;

            var result = _dialerChannel.Execute(
                x => x.StartMonitor(
                    companyId,
                    dialerId,
                    agentId,
                    supervisorName,
                    supervisorConnectionString,
                    resourceBindingType,
                    ref outSessionId));

            sessionId = outSessionId;

            return result;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            return _dialerChannel.Execute(
                x => x.StopMonitor(
                    companyId,
                    dialerId,
                    sessionId));
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            return _dialerChannel.Execute(
                x => x.SetMonitorMode(
                    companyId,
                    dialerId,
                    sessionId,
                    monitorMode));
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            IEnumerable<TrunkLineStateAndAlarms> outTrunkLineStatesAndAlarms = null;

            var result = _dialerChannel.Execute(
                x => x.GetTrunkLineStatesAndAlarms(
                    companyId,
                    dialerId,
                    out outTrunkLineStatesAndAlarms));

            trunkLineStatesAndAlarms = outTrunkLineStatesAndAlarms;

            return result;
        }

        public DialerErrorCode TransferToIvr(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            return _dialerChannel.Execute(
                x => x.TransferToIvr(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    endpoint,
                    attributes));
        }

        public DialerErrorCode IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {
            return _dialerChannel.Execute(
                x => x.IvrRenderVoiceXml(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    voiceXml));
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            return _dialerChannel.Execute(
                x => x.ConfigureInboundDdiNumbers(
                    companyId,
                    dialerId,
                    inboundDdiNumbers));
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            return _dialerChannel.Execute(
                x => x.DropInboundCall(
                    companyId,
                    dialerId,
                    inboundCallId,
                    audioMessageDescriptor));
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            return _dialerChannel.Execute(
                x => x.ConnectInboundCall(
                    companyId,
                    dialerId,
                    campaignId,
                    inboundCallId,
                    callInfo,
                    campaignIdsToBorrowAgentsFrom,
                    audioMessageDescriptor));
        }

        public DialerErrorCode ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            return _dialerChannel.Execute(
                x => x.ConnectInboundCallToAgent(
                    companyId,
                    dialerId,
                    campaignId,
                    inboundCallId,
                    callInfo,
                    audioMessageDescriptor));
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            return _dialerChannel.Execute(
                x => x.TransferStart(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId,
                    agentId,
                    transferType));
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            return _dialerChannel.Execute(
                x => x.TransferSetTarget(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId,
                    targetType,
                    targetResource,
                    borrowAgentsFromAllCampaigns));
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId,
            string transferId, ConnectionState state)
        {
            return _dialerChannel.Execute(
                x => x.TransferSetConnectionState(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId,
                    state));
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            return _dialerChannel.Execute(
                x => x.TransferComplete(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId));
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            return _dialerChannel.Execute(
                x => x.TransferCancel(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId));
        }

        public void ReleaseDialerChannel()
        {
            _dialerChannel.Release();
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            var outLogin = "";
            var outPassword = "";
            var outHost = "";
            var outExtension = "";
            var outFrontendUrl = "";

            var result = _dialerChannel.Execute(
                x => x.RegisterAgentSoftphone(
                    companyId,
                    dialerId,
                    agentId,
                    agentName,
                    out outLogin,
                    out outPassword,
                    out outHost,
                    out outExtension,
                    out outFrontendUrl));

            login = outLogin;
            password = outPassword;
            host = outHost;
            extension = outExtension;
            frontendUrl = outFrontendUrl;

            return result;
        }
    }
}