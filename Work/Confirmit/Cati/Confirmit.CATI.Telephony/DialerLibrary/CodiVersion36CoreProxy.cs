extern alias CodiV36;
using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.WcfTools;
using ConfirmitDialerInterface;
using DialerCommon;

using IDialerService36 = CodiV36::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using DialingMode36 = CodiV36::ConfirmitDialerInterface.DialingMode;
using AudioMessageType36 = CodiV36::ConfirmitDialerInterface.AudioMessageType;
using AudioSourceType36 = CodiV36::ConfirmitDialerInterface.AudioSourceType;
using AudioMessageDescriptor36 = CodiV36::ConfirmitDialerInterface.AudioMessageDescriptor;
using ConnectionState36 = CodiV36::ConfirmitDialerInterface.ConnectionState;
using TargetType36 = CodiV36::ConfirmitDialerInterface.TargetType;
using TransferType36 = CodiV36::ConfirmitDialerInterface.TransferType;
using MonitorMode36 = CodiV36::ConfirmitDialerInterface.MonitorMode;

using ConfirmitDialerInterface36 = CodiV36::ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersion36CoreProxy : ICodiVersionCoreProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService36> _dialerChannel;

        public CodiVersion36CoreProxy(IChannelFactoryWrapper<IDialerService36> dialerChannel)
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
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Initialize(
                    companyId,
                    dialerId,
                    configurationParametersXml));
        }

        public DialerErrorCode Release(int dialerId, int companyId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(x => x.Release(dialerId));
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
            return (DialerState)_dialerChannel.Execute(x => x.GetState(companyId, dialerId));
        }

        public DialerErrorCode StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode,
            bool recordWholeInterview, string campaignParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StartCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    campaignName,
                    (DialingMode36)dialingMode,
                    recordWholeInterview,
                    campaignParametersXml));
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    (DialingMode36)dialingMode));
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.KillCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    (DialingMode36)dialingMode));
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview,
            string campaignParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetCampaignParameters(
                    companyId,
                    dialerIds,
                    campaignId,
                    (DialingMode36)dialingMode,
                    recordWholeInterview,
                    campaignParametersXml));
        }

        public DialerErrorCode Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, bool isPredictive, ResourceBindingType resourceBindingType, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Login(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    agentName,
                    (ConfirmitDialerInterface36.AgentType)agentType,
                    agentConnectionString,
                    isPredictive,
                    (ConfirmitDialerInterface36.ResourceBindingType)resourceBindingType,
                    agentAttributes));
        }

        public DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetCampaign(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Logout(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    isPredictive));
        }

        public DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.KillAgent(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.GoReady(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.GoNotReady(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    breakName));
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
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
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SendNumberToAgent(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    (DialingMode36)dialingMode,
                    interviewId,
                    callId,
                    phoneNumber,
                    isRecording,
                    callerId));
        }

        public DialerErrorCode Redial(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording, string callerId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
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
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SendNumbers(
                    requestId,
                    companyId,
                    dialerId,
                    campaignId,
                    (DialingMode36)campaignDialingMode,
                    ToCallList36(callList),
                    callAgingTimeout));
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.Hangup(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId));
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId,
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.CompleteCall(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    ToInterviewStatus36(interviewStatus),
                    makeAgentReady,
                    breakName));
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetNextInterview(
                    companyId,
                    dialerId,
                    currentCampaignId,
                    agentId,
                    ToInterviewStatus36(currentInterviewStatus),
                    nextCampaignId,
                    nextInterviewId,
                    nextCallId));
        }

        public DialerErrorCode StartCustomIvrInterview(int companyId, int dialerId, long campaignId, int agentId, 
            int interviewId, long callId, string respondentSurveyLink)
        {
            throw new System.NotImplementedException();
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
            return (DialerErrorCode)_dialerChannel.Execute(
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
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.FlushNumbers(
                    companyId,
                    dialerIds,
                    campaignId,
                    ToCallList36(callList)));
        }

        public DialerErrorCode StartRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string label)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
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
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopRecording(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    (ConfirmitDialerInterface36.StopRecordingMode)stopRecordingMode));
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

            return (DialerErrorCode)result;
        }

        public DialerErrorCode StopPlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopPlayback(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode PauseOrResumePlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.PauseOrResumePlayback(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    callId));
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
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
                    (ConfirmitDialerInterface36.ResourceBindingType)resourceBindingType,
                    ref outSessionId));

            sessionId = outSessionId;

            return (DialerErrorCode)result;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.StopMonitor(
                    companyId,
                    dialerId,
                    sessionId));
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetMonitorMode(
                    companyId,
                    dialerId,
                    sessionId,
                    (MonitorMode36)monitorMode));
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            IEnumerable<ConfirmitDialerInterface36.TrunkLineStateAndAlarms> outTrunkLineStatesAndAlarms = null;

            var result = _dialerChannel.Execute(
                x => x.GetTrunkLineStatesAndAlarms(
                    companyId,
                    dialerId,
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
            return (DialerErrorCode)_dialerChannel.Execute(
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
            var inboundDdiNumbersV36 = inboundDdiNumbers.Select(
                x => new ConfirmitDialerInterface36.InboundDdiNumber
                {
                    Number = x.Number,
                    AudioMessages = x.AudioMessages
                        .Where(a => Enum.IsDefined(typeof(AudioMessageType36), (int)a.Key))
                        .Select(y => new KeyValuePair<AudioMessageType36, AudioMessageDescriptor36>(
                            (AudioMessageType36)y.Key,
                            ToAudioMessageDescriptor36(y.Value)))
                        .ToArray()
                }).ToArray();

            var result = _dialerChannel.Execute(
                x => x.ConfigureInboundDdiNumbers(
                    companyId,
                    dialerId,
                    inboundDdiNumbersV36));

            return result.Select(x => (DialerErrorCode)x).ToArray();
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.DropInboundCall(
                    companyId,
                    dialerId,
                    inboundCallId,
                    ToAudioMessageDescriptor36(audioMessageDescriptor)));
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.ConnectInboundCall(
                    companyId,
                    dialerId,
                    campaignId,
                    inboundCallId,
                    ToCallInfo36(callInfo),
                    campaignIdsToBorrowAgentsFrom,
                    ToAudioMessageDescriptor36(audioMessageDescriptor)));
        }

        public DialerErrorCode ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.ConnectInboundCallToAgent(
                    companyId,
                    dialerId,
                    campaignId,
                    inboundCallId,
                    ToCallInfo36(callInfo),
                    ToAudioMessageDescriptor36(audioMessageDescriptor)));
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.TransferStart(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId,
                    agentId,
                    (TransferType36)transferType));
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.TransferSetTarget(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId,
                    (TargetType36)targetType,
                    targetResource,
                    borrowAgentsFromAllCampaigns));
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId, string transferId, ConnectionState state)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.TransferSetConnectionState(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId,
                    (ConnectionState36)state));
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.TransferComplete(
                    companyId,
                    dialerId,
                    campaignId,
                    transferId));
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
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

        private ConfirmitDialerInterface36.InterviewStatus ToInterviewStatus36(InterviewStatus interviewStatus)
        {
            return new ConfirmitDialerInterface36.InterviewStatus { Code = interviewStatus.Code, Name = interviewStatus.Name };
        }

        private ConfirmitDialerInterface36.CallInfo ToCallInfo36(CallInfo callInfo)
        {
            return new ConfirmitDialerInterface36.CallInfo(
                callInfo.agentId,
                callInfo.interviewId,
                callInfo.callId,
                callInfo.agentGroupId,
                callInfo.phoneNumber,
                callInfo.timeToCall,
                (DialingMode36)callInfo.diallingMode,
                callInfo.wasAbandoned,
                callInfo.dialingAttemptsMade,
                callInfo.previousConnects,
                callInfo.numberOfNoAnswer,
                callInfo.dialerSpecificAccompanyInfo,
                callInfo.isRecording,
                callInfo.agingTimeout,
                callInfo.callerId);
        }

        private List<ConfirmitDialerInterface36.CallInfo> ToCallList36(IEnumerable<CallInfo> callList)
        {
            return new List<ConfirmitDialerInterface36.CallInfo>(
                callList.Select(ToCallInfo36)
            );
        }

        private AudioMessageDescriptor36 ToAudioMessageDescriptor36(AudioMessageDescriptor audioMessageDescriptor)
        {
            if (audioMessageDescriptor == null)
            {
                return null;
            }

            return new AudioMessageDescriptor36
            {
                Type = (AudioSourceType36)audioMessageDescriptor.Type,
                Source = audioMessageDescriptor.Source
            };
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            throw new NotImplementedException();
        }
    }
}