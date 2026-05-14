using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class DialerLibrary : DialerLibraryBase, IDialerAPI, IDialerRecordingAPI
    {
        private readonly IDialerApiClient _dialerApiClient = ServiceLocator.Resolve<IDialerApiClient>();
        private readonly IToggleSettings _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

        public DialerLibrary() : base()
        {
        }

        public DialerLibrary(IChannelFactoryWrapperFactory<IDialerService> channelFactoryWrapperFactory) : base(channelFactoryWrapperFactory)
        {
        }

        //things to do when we remove the old dialer api
        //todo: remove tenantId parameter, now CompanyId is used
        //todo: use `string projectId1` instead of `int campaignId` 
        //todo: return DialerErrorCode instead of int
        //todo: add dialerId parameter instead of using DialerId property
        //todo: rename contactId to interviewId
        //todo: use `int agentId` instead of `string agentId`
        //todo: refactor IDialerRecordingAPI.Initialize method so it is not called every time we call other IDialerRecordingAPI methods, also signature should be changed to just Initialize(int dialerId)
        //todo: refactor IDIalerAPI.Initialize method, we don't need to initialize protected variables from class DialerLibraryBase also signature should be changed to just Initialize(int dialerId)
        public new int StartCampaign(string tenantId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, bool recordWholeInterview, string surveyParametersXml)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.StartCampaign(dialerIds, $"p{campaignId}", campaignName, surveyParametersXml, dialingMode, recordWholeInterview);

            return base.StartCampaign(tenantId, dialerIds, campaignId, campaignName, dialingMode, campaignType, recordWholeInterview, surveyParametersXml);
        }

        public new int StopCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.StopCampaign($"p{campaignId}", dialerIds, dialingMode);

            return base.StopCampaign(tenantId, dialerIds, campaignId, dialingMode);
        }

        public new int KillCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.KillCampaign($"p{campaignId}", dialerIds, dialingMode);

            return base.KillCampaign(tenantId, dialerIds, campaignId, dialingMode);
        }

        public new int SetCampaignParameters(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string surveyParametersXml)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.SetCampaignParameters($"p{campaignId}", dialerIds, surveyParametersXml, dialingMode, recordWholeInterview);

            return base.SetCampaignParameters(tenantId, dialerIds, campaignId, dialingMode, recordWholeInterview, surveyParametersXml);
        }

        public new int Logout(string tenantId, long campaignId, bool isPredictive, string agentId)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.Logout(DialerId, $"p{campaignId}", int.Parse(agentId), isPredictive);

            return base.Logout(tenantId, campaignId, isPredictive, agentId);
        }

        public new int KillAgent(string tenantId, long campaignId, string agentId)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.KillAgent(DialerId, $"p{campaignId}", int.Parse(agentId));

            return base.KillAgent(tenantId, campaignId, agentId);
        }

        public new int SetGroups(string tenantId, long campaignId, string agentId, int[] groupIds)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.SetGroups(DialerId, $"p{campaignId}", int.Parse(agentId), groupIds);

            return base.SetGroups(tenantId, campaignId, agentId, groupIds);
        }

        public new int CompleteCall(string tenantId, long campaignId, string agentId, InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.CompleteCall(DialerId, $"p{campaignId}", int.Parse(agentId), interviewId, callId, makeAgentReady, breakName, interviewStatus);

            return base.CompleteCall(tenantId, campaignId, agentId, interviewStatus, makeAgentReady, breakName, interviewId, callId);
        }

        public new int SendNumbers(
            string requestId,
            string tenantId,
            long campaignId,
            DialingMode campaignDiallingMode,
            List<CallInfo> callList,
            int callAgingTimeout,
            bool isRecording)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.SendNumbers(requestId, DialerId, $"p{campaignId}", campaignDiallingMode, callAgingTimeout, callList);

            return base.SendNumbers(requestId, tenantId, campaignId, campaignDiallingMode, callList, callAgingTimeout, isRecording);
        }

        public new int FlushNumbers(string tenantId, int[] dialerIds, long campaignId, List<CallInfo> callsList)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.FlushNumbers(dialerIds, $"p{campaignId}", callsList);

            return base.FlushNumbers(tenantId, dialerIds, campaignId, callsList);
        }

        public new int StartRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, string label)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.StartRecording(DialerId, $"p{campaignId}", int.Parse(agentId), contactId, callId, label);

            return base.StartRecording(tenantId, campaignId, agentId, contactId, callId, label);
        }

        public new int StopRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.StopRecording(DialerId, $"p{campaignId}", int.Parse(agentId), contactId, callId, stopRecordingMode);

            return base.StopRecording(tenantId, campaignId, agentId, contactId, callId, stopRecordingMode);
        }

        public new int CompletePreview(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {
            if (_toggleSettings.UseNewDialerApi)
                return (int)_dialerApiClient.CompletePreview(DialerId, $"p{campaignId}", int.Parse(agentId), contactId, callId, phoneNumber, isRecording);

            return base.CompletePreview(tenantId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
        }

        public new IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long surveyId, int interviewId, int dialerId)
        {
            if (_toggleSettings.UseNewDialerApi)
                return _dialerApiClient.GetAudioRecords($"p{surveyId}", interviewId, dialerId);

            return base.GetAudioRecords(companyId, surveyId, interviewId, dialerId);
        }

        public new bool[] AreRecordsExists(int companyId, long surveyId, int[] interviewIds, int dialerId)
        {
            if (_toggleSettings.UseNewDialerApi)
                return _dialerApiClient.AreRecordsExists($"p{surveyId}", interviewIds, dialerId);

            return base.AreRecordsExists(companyId, surveyId, interviewIds, dialerId);
        }

        public new AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            if (_toggleSettings.UseNewDialerApi)
                return _dialerApiClient.GetAudioFile(dialerId, audioUrl);

            return base.GetAudioFile(companyId, dialerId, audioUrl);
        }

        public new int ConnectInboundCall(
            int companyId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                var surveyId = $"p{campaignId}";
                var surveyIdsToBorrowAgentsFrom = campaignIdsToBorrowAgentsFrom?.Select(x => $"p{x}").ToArray();
                return (int)_dialerApiClient.ConnectInboundCall(DialerId, surveyId, inboundCallId, callInfo, surveyIdsToBorrowAgentsFrom, audioMessageDescriptor);
            }

            return base.ConnectInboundCall(companyId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
        }

        public new int DropInboundCall(int companyId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return (int)_dialerApiClient.DropInboundCall(DialerId, inboundCallId, audioMessageDescriptor);
            }

            return base.DropInboundCall(companyId, inboundCallId, audioMessageDescriptor);
        }

        public new int StartMonitor(string tenantId, string agentId, string number, ref string sessionId)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                var response = _dialerApiClient.StartMonitor(DialerId, int.Parse(agentId), number, sessionId);
                sessionId = response.SessionId;

                return (int)response.DialerErrorCode;
            }

            return base.StartMonitor(tenantId, agentId, number, ref sessionId);
        }

        public new int StopMonitor(string tenantId, string sessionId)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return (int)_dialerApiClient.StopMonitor(DialerId, sessionId);
            }

            return base.StopMonitor(tenantId, sessionId);
        }

        public new int SetMonitorMode(string tenantId, string sessionId, MonitorMode monitorMode)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return (int)_dialerApiClient.SetMonitorMode(DialerId, sessionId, monitorMode);
            }

            return base.SetMonitorMode(tenantId, sessionId, monitorMode);
        }

        public new DialerInitializeResult Initialize(
            int dialerId,
            string tenantId,
            string connectionParametersXml,
            string configurationParametersXml,
            string surveyDefaultParametersXml,
            bool sendInitializeToWebService = true)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                DialerId = dialerId;
                _commonConfigurationParameters = new GenericConfigurationParameters(configurationParametersXml);

                var dialerInfo = _dialerApiClient.GetDialerInfo(dialerId);
                if (dialerInfo.DialerErrorCode != DialerErrorCode.Success)
                    return new DialerInitializeResult(dialerInfo.DialerErrorCode, dialerInfo.ErrorMessage);

                _codiVersionInfo = new CodiVersionInfoCommon(dialerInfo.CodiMajorVersion, dialerInfo.CodiFullVersion, dialerInfo.DialerDriverNameAndVersion);

                if (sendInitializeToWebService == false)
                    return new DialerInitializeResult(dialerInfo.DialerErrorCode);

                var initializationResult = _dialerApiClient.Initialize(dialerId);
                if (initializationResult.DialerErrorCode != DialerErrorCode.Success)
                    return new DialerInitializeResult(initializationResult.DialerErrorCode, initializationResult.ErrorMessage);

                var stateResponse = _dialerApiClient.GetState(dialerId);
                if (stateResponse.DialerErrorCode != DialerErrorCode.Success)
                    return new DialerInitializeResult(stateResponse.DialerErrorCode, stateResponse.ErrorMessage);

                var result = stateResponse.DialerState == DialerState.Available ? DialerErrorCode.Success : DialerErrorCode.NotAvailable;
                return new DialerInitializeResult(result);
            }

            return new DialerInitializeResult((DialerErrorCode)base.Initialize(dialerId, tenantId, connectionParametersXml, configurationParametersXml, surveyDefaultParametersXml, sendInitializeToWebService));
        }

        public new void Initialize(string connectionParametersXml, string configurationParametersXml)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                _dialerApiClient.InitializeRecording(DialerId);
            }
            else
            {
                base.Initialize(connectionParametersXml, configurationParametersXml);
            }
        }

        public new int Release(int dialerId, int companyId)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return (int)_dialerApiClient.Release(dialerId);
            }

            return base.Release(dialerId, companyId);
        }

        public new DialerFeatures GetFeatures(string tenantId)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return _dialerApiClient.GetFeatures(DialerId).DialerFeatures;
            }

            return base.GetFeatures(tenantId);
        }

        public new DialerState GetState(int dialerId, string tenantId)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return _dialerApiClient.GetState(dialerId).DialerState;
            }

            return base.GetState(dialerId, tenantId);
        }

        public new IEnumerable<LogFileInfo> GetLogFiles()
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return _dialerApiClient.GetLogFiles(DialerId).LogFileInfos;
            }

            return base.GetLogFiles();
        }

        public new byte[] GetLogFileBodyZipped(string fileName)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return _dialerApiClient.GetLogFileBodyZipped(DialerId, fileName).LogFileBodyZipped;
            }

            return base.GetLogFileBodyZipped(fileName);
        }

        public new DialerErrorCode[] ConfigureInboundDdiNumbers(int companyId, InboundDdiNumber[] inboundDdiNumbers)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return _dialerApiClient.ConfigureInboundDdiNumbers(DialerId, inboundDdiNumbers).DialerErrorCodes;
            }

            return base.ConfigureInboundDdiNumbers(companyId, inboundDdiNumbers);
        }

        public new int TransferCancel(int companyId, long campaignId, string transferId)
        {
            if (_toggleSettings.UseNewDialerApi)
            {
                return (int)_dialerApiClient.TransferCancel(DialerId, $"p{campaignId}", transferId);
            }

            return base.TransferCancel(companyId, campaignId, transferId);
        }
    }
}