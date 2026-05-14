extern alias CodiV34;

using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.WcfTools;
using ConfirmitDialerInterface;
using DialerCommon;

using IDialerService34 = CodiV34::Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;
using ConfirmitDialerInterface34 = CodiV34::ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class CodiVersion34CoreProxy : ICodiVersionCoreProxy
    {
        private readonly IChannelFactoryWrapper<IDialerService34> _dialerChannel;

        public CodiVersion34CoreProxy(IChannelFactoryWrapper<IDialerService34> dialerChannel)
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
                    (ConfirmitDialerInterface34.DialingMode)dialingMode,
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
                    (ConfirmitDialerInterface34.DialingMode)dialingMode));
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.KillCampaign(
                    companyId,
                    dialerIds,
                    campaignId,
                    (ConfirmitDialerInterface34.DialingMode)dialingMode));
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview,
            string campaignParametersXml)
        {
            return (DialerErrorCode)_dialerChannel.Execute(
                x => x.SetCampaignParameters(
                    companyId,
                    dialerIds,
                    campaignId,
                    (ConfirmitDialerInterface34.DialingMode)dialingMode,
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
                    (ConfirmitDialerInterface34.AgentType)agentType,
                    agentConnectionString,
                    isPredictive,
                    (ConfirmitDialerInterface34.ResourceBindingType)resourceBindingType,
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
                    agentId));
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
                    (ConfirmitDialerInterface34.DialingMode)dialingMode,
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
                    (ConfirmitDialerInterface34.DialingMode)campaignDialingMode,
                    CallList34(callList),
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
                    InterviewStatus34(interviewStatus),
                    makeAgentReady));
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            throw new System.NotImplementedException();
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
                    CallList34(callList)));
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
                    (ConfirmitDialerInterface34.StopRecordingMode)stopRecordingMode));
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

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId, int agentId,
            long callId)
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
            var outSessionId = string.Empty;

            var result = _dialerChannel.Execute(
                x => x.StartMonitor(
                    companyId,
                    dialerId,
                    agentId,
                    supervisorName,
                    supervisorConnectionString,
                    (ConfirmitDialerInterface34.ResourceBindingType)resourceBindingType,
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

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            IEnumerable<ConfirmitDialerInterface34.TrunkLineStateAndAlarms> outTrunkLineStatesAndAlarms = null;

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
            throw new System.NotImplementedException();
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId,
            string transferId, ConnectionState state)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            throw new System.NotImplementedException();
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseDialerChannel()
        {
            _dialerChannel.Release();
        }

        private ConfirmitDialerInterface34.InterviewStatus InterviewStatus34(InterviewStatus interviewStatus)
        {
            return new ConfirmitDialerInterface34.InterviewStatus { Code = interviewStatus.Code, Name = interviewStatus.Name };
        }

        private List<ConfirmitDialerInterface34.CallInfo> CallList34(IEnumerable<CallInfo> callList)
        {
            return new List<ConfirmitDialerInterface34.CallInfo>(
                callList.Select(x => new ConfirmitDialerInterface34.CallInfo(
                    x.agentId,
                    x.interviewId,
                    x.callId,
                    x.agentGroupId,
                    x.phoneNumber,
                    x.timeToCall,
                    (ConfirmitDialerInterface34.DialingMode)x.diallingMode,
                    x.wasAbandoned,
                    x.dialingAttemptsMade,
                    x.previousConnects,
                    x.numberOfNoAnswer,
                    x.dialerSpecificAccompanyInfo,
                    x.isRecording,
                    x.agingTimeout,
                    x.callerId))
            );
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}