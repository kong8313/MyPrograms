extern alias CodiV30;

using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.WcfTools;
using ConfirmitDialerInterface;
using DialerCommon;

using IDialerService30 = CodiV30::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using ConfirmitDialerInterface30 = CodiV30::ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersion30CoreProxy : ICodiVersionCoreProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService30> _dialerChannel;

        public CodiVersion30CoreProxy(IChannelFactoryWrapper<IDialerService30> dialerChannel)
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
            throw new NotImplementedException();
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Initialize(
                    companyId,
                    dialerId,
                    configurationParametersXml));
        }

        public DialerErrorCode Release(int dialerId, int companyId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Release());
        }

        public DialerFeatures GetFeatures(int companyId, int dialerId)
        {
            return DialerFeaturesFactory.CreateDefault();
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(x => x.SetConfigurationParameters(companyId, configurationParametersXml));
        }

        public DialerState GetState(int companyId, int dialerId)
        {
            return (DialerState)_dialerChannel.Execute(x => x.GetState(companyId));
        }

        public DialerErrorCode StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode,
            bool recordWholeInterview, string campaignParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StartCampaign(
                    companyId,
                    campaignId,
                    campaignName,
                    (ConfirmitDialerInterface30.DialingMode)dialingMode,
                    recordWholeInterview,
                    campaignParametersXml));
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopCampaign(
                    companyId,
                    campaignId,
                    (ConfirmitDialerInterface30.DialingMode)dialingMode));
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.KillCampaign(
                    companyId,
                    campaignId,
                    (ConfirmitDialerInterface30.DialingMode)dialingMode));
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview,
            string campaignParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetCampaignParameters(
                    companyId,
                    campaignId,
                    (ConfirmitDialerInterface30.DialingMode)dialingMode,
                    recordWholeInterview,
                    campaignParametersXml));
        }

        public DialerErrorCode Login(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            string agentName,
            AgentType agentType,
            string agentConnectionString,
            bool isPredictive, ResourceBindingType resourceBindingType,
            IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Login(
                    companyId,
                    campaignId,
                    agentId,
                    agentName,
                    agentConnectionString,
                    isPredictive,
                    (ConfirmitDialerInterface30.ResourceBindingType)resourceBindingType,
                    agentAttributes));
        }

        public DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {
            throw new NotSupportedException("SetCampaign is not supported in CODI v.3.0 and earlier versions");
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Logout(
                    companyId,
                    campaignId,
                    agentId,
                    isPredictive));
        }

        public DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.KillAgent(
                    companyId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.GoReady(
                    companyId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.GoNotReady(
                    companyId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetGroups(
                    companyId,
                    campaignId,
                    agentId,
                    agentGroups));
        }

        public DialerErrorCode SendNumberToAgent(int companyId, int dialerId, long campaignId, int agentId, DialingMode dialingMode,
            int interviewId, long callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, object> respondentVariables)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SendNumberToAgent(
                    companyId,
                    campaignId,
                    agentId,
                    (ConfirmitDialerInterface30.DialingMode)dialingMode,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording));
        }

        public DialerErrorCode Redial(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording, string callerId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Redial(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording));
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode,
            List<CallInfo> callList, int callAgingTimeout)
        {
            var callList30 = new List<ConfirmitDialerInterface30.CallInfo>(
                callList.Select(x => new ConfirmitDialerInterface30.CallInfo(
                    x.agentId,
                    x.interviewId,
                    x.callId,
                    x.agentGroupId,
                    x.phoneNumber,
                    x.timeToCall,
                    (ConfirmitDialerInterface30.DialingMode)x.diallingMode,
                    x.wasAbandoned,
                    x.dialingAttemptsMade,
                    x.previousConnects,
                    x.numberOfNoAnswer,
                    x.dialerSpecificAccompanyInfo,
                    x.isRecording,
                    x.agingTimeout))
                );

            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SendNumbers(
                    requestId,
                    companyId,
                    campaignId,
                    (ConfirmitDialerInterface30.DialingMode)campaignDialingMode,
                    callList30,
                    callAgingTimeout));
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Hangup(
                    companyId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId,
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.CompleteCall(
                    companyId,
                    campaignId,
                    agentId,
                    makeAgentReady));
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode StartCustomIvrInterview(int companyId, int dialerId, long campaignId, int agentId, 
            int interviewId, long callId, string respondentSurveyLink)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode UpdateInterviewStatus(int companyId, int dialerId, long campaignId, int agentId, int interviewId,
            long callId, InterviewStatus interviewStatus)
        {
            var interviewStatus30 = new ConfirmitDialerInterface30.InterviewStatus { Code = interviewStatus.Code, Name = interviewStatus.Name };

            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.UpdateInterviewStatus(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    interviewStatus30));
        }

        public DialerErrorCode CompletePreview(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.CompletePreview(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording));
        }

        public DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {
            var callList30 = new List<ConfirmitDialerInterface30.CallInfo>(
                callList.Select(x => new ConfirmitDialerInterface30.CallInfo(
                    x.agentId,
                    x.interviewId,
                    x.callId,
                    x.agentGroupId,
                    x.phoneNumber,
                    x.timeToCall,
                    (ConfirmitDialerInterface30.DialingMode)x.diallingMode,
                    x.wasAbandoned,
                    x.dialingAttemptsMade,
                    x.previousConnects,
                    x.numberOfNoAnswer,
                    x.dialerSpecificAccompanyInfo,
                    x.isRecording,
                    x.agingTimeout))
                );

            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.FlushNumbers(
                    companyId,
                    campaignId,
                    callList30));
        }

        public DialerErrorCode StartRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string label)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StartRecording(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    label));
        }

        public DialerErrorCode StopRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            StopRecordingMode stopRecordingMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopRecording(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    (ConfirmitDialerInterface30.StopRecordingMode)stopRecordingMode));
        }

        public DialerErrorCode StartPlayback(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string fileName, out int timeOfPlayingInSeconds)
        {
            int outTimeOfPlayingInSeconds = 0;

            var result = _dialerChannel.Execute(
                x => x.StartPlayback(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    fileName,
                    out outTimeOfPlayingInSeconds));

            timeOfPlayingInSeconds = outTimeOfPlayingInSeconds;

            return (DialerErrorCode)result;
        }

        public DialerErrorCode StopPlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopPlayback(
                    companyId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode PauseOrResumePlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.PauseOrResumePlayback(
                    companyId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId, int agentId,
            long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.ToggleInterviewerListensToPlaybackOrRespondent(
                    companyId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode StartMonitor(int companyId, int dialerId, int agentId, string supervisorName,
            string supervisorConnectionString, ResourceBindingType resourceBindingType, ref string sessionId)
        {
            var outSessionId = string.Empty;

            var result = _dialerChannel.Execute(
                x => x.StartMonitor(
                    companyId,
                    agentId,
                    supervisorName,
                    supervisorConnectionString,
                    (ConfirmitDialerInterface30.ResourceBindingType)resourceBindingType,
                    ref outSessionId));

            sessionId = outSessionId;

            return (DialerErrorCode)result;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopMonitor(
                    companyId,
                    sessionId));
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            IEnumerable<ConfirmitDialerInterface30.TrunkLineStateAndAlarms> outTrunkLineStatesAndAlarms = null;

            var result = _dialerChannel.Execute(
                x => x.GetTrunkLineStatesAndAlarms(
                    companyId,
                    out outTrunkLineStatesAndAlarms));

            trunkLineStatesAndAlarms = new List<TrunkLineStateAndAlarms>(
                outTrunkLineStatesAndAlarms.Select(x => new TrunkLineStateAndAlarms
                {
                    LineName = x.LineName,

                    State = new AlarmEntry
                    {
                        State = (TrunkLineState)x.State.State,
                        Duration = x.State.Duration,
                        Time = x.State.Time

                    },

                    AlarmsList = new List<AlarmEntry>(x.AlarmsList.Select(alarmEntry => new AlarmEntry
                    {
                        State = (TrunkLineState)alarmEntry.State,
                        Duration = alarmEntry.Duration,
                        Time = alarmEntry.Time

                    }))
                }));

            return (DialerErrorCode)result;
        }

        public DialerErrorCode TransferToIvr(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.TransferToIvr(
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    endpoint,
                    attributes));
        }

        public DialerErrorCode IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId,
            string transferId, ConnectionState state)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            throw new NotImplementedException();
        }

        public void ReleaseDialerChannel()
        {
            _dialerChannel.Release();
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            throw new NotImplementedException();
        }
    }
}