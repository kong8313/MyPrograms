using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Context;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Properties;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage.Model;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS
{
    public class AwsConnectDialerWs: IDialerCoreApi, IDialerRecordingApi, IDisposable
    {
        private readonly IDialerEvents _dialerEvents;
        private readonly ILogger _logger;
        private readonly RedisClient _redisClient;
        
        private readonly ConcurrentDictionary<string, DialerInstanceCacheItem> _dialers = new ConcurrentDictionary<string, DialerInstanceCacheItem>();

        public AwsConnectDialerWs(IDialerEvents dialerEvents, ILogger logger)
        {
            logger.Info("AwsConnectDialerWS.AwsConnectDialerWS", "ctor");

            _dialerEvents = dialerEvents;
            _logger = logger;
            _redisClient = new RedisClient(Settings.Default.RedisConnString, logger);
        }
        
        private AwsConnectDialer GetDialerWithCheck(int companyId, int dialerId)
        {
            var dialerContext = new DialerContext(companyId, dialerId);

            _dialers.TryGetValue(dialerContext, out var dialerInstanceCacheItem);
            
            if (dialerInstanceCacheItem != null && !dialerInstanceCacheItem.IsExpired)
                return dialerInstanceCacheItem.Dialer;
            
            var dialerCacheItem = _redisClient.Get<DialerInfo>(dialerContext);
            if (dialerCacheItem == null)
                throw new DialerIsNotInitializedException("Dialer is not Initialized");

            if (dialerInstanceCacheItem != null && dialerInstanceCacheItem.IsExpired
                && dialerCacheItem.ConfigurationParametersXml == dialerInstanceCacheItem.ConfigurationParametersXml)
            {
                dialerInstanceCacheItem.ExtendLifetime();
                return dialerInstanceCacheItem.Dialer;
            }

            return ReCreateDialer(dialerCacheItem.Context, dialerCacheItem.ConfigurationParametersXml);
        }

        public string GetName()
        {
            return "Amazon Connect IVR Dialer";
        }

        public string GetVersion()
        {
            return "1.0";
        }

        private AwsConnectDialer ReCreateDialer(DialerContext dialerContext, string configurationParametersXml)
        {
            _logger.Info(nameof(ReCreateDialer), () => dialerContext);
            
            if (_dialers.TryRemove(dialerContext, out var dialerInstanceCacheItem)) 
                dialerInstanceCacheItem?.Dialer?.Dispose();
            
            var dialer = new AwsConnectDialer(_dialerEvents, _logger, _redisClient);
            dialer.Initialize(configurationParametersXml);

            if (!_dialers.TryAdd(dialerContext, new DialerInstanceCacheItem(dialer, configurationParametersXml)))
                throw new Exception("Cannot add dialer to the dictionary");

            return dialer;
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {
            _logger.Verbose("AwsConnectDialerWS.Initialize",
                string.Format("companyId={0}, dialerId={1}, configurationParametersXml=[{2}]",
                    companyId, dialerId, configurationParametersXml));
            try
            {
                var dialerContext = new DialerContext(companyId, dialerId);
                ReCreateDialer(dialerContext, configurationParametersXml);
                _redisClient.Set(dialerContext, new DialerInfo(dialerContext, configurationParametersXml));
                
                return DialerErrorCode.Success;
            }
            catch (Exception ex)
            {
                _logger.Error("AwsConnectDialerWS.Initialize", () => ex.Message);
                return DialerErrorCode.Exception;
            }
        }

        public DialerErrorCode Release(int dialerId, int companyId)
        {
            _logger.Verbose("AwsConnectDialerWS.Release",
                $"dialerId={dialerId}, companyId={companyId}");
            
            var dialerContext = new DialerContext(companyId, dialerId);
            if (_dialers.TryRemove(dialerContext, out var dialerInstanceCacheItem))
                dialerInstanceCacheItem?.Dialer?.Dispose();
            
            _redisClient.Remove(dialerContext);

            return DialerErrorCode.Success;
        }

        public IDialerFeatures GetFeatures(int companyId, int dialerId)
        {
            _logger.Verbose("AwsConnectDialerWS.GetFeatures",
                string.Format("companyId={0}, dialerId={1}",
                    companyId, dialerId));

            return new AwsConnectDialerFeatures();
        }

        public DialerErrorCode RestoreDialerDriverState(int companyId, string filename)
        {
            _logger.Verbose("AwsConnectDialerWS.RestoreDialerDriverState",
                string.Format("companyId={0}, filename={1}",
                    companyId, filename));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SaveDialerDriverState(string filename)
        {
            _logger.Verbose("AwsConnectDialerWS.RestoreDialerDriverState",
                string.Format("filename={0}", filename));
            
            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            _logger.Verbose("AwsConnectDialerWS.SetConfigurationParameters",
                string.Format("companyId={0}, configurationParametersXml=[{1}]",
                    companyId, configurationParametersXml));

            return DialerErrorCode.Success;
        }

        public DialerState GetState(int companyId, int dialerId)
        {
            GetDialerWithCheck(companyId, dialerId);

            return DialerState.Available;
        }

        public DialerErrorCode StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName,
            DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            _logger.Verbose("AwsConnectDialerWS.StartCampaign",
                string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, campaignName={3}, dialingMode={4}, recordWholeInterview={5}, campaignParametersXml=[{6}]",
                    companyId, string.Join(", ", dialerIds), campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml));

            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);
            
                try
                {
                    var ctx = new SurveyContext(companyId, dialerId, campaignId);
                    dialer.CreateSurveySession(ctx, campaignParametersXml);
                }
                catch (Exception ex)
                {
                    _logger.Error("AwsConnectDialerWS.StartCampaign", () => ex.Message);
                    return DialerErrorCode.Exception;
                }
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            _logger.Verbose("AwsConnectDialerWS.StopCampaign",
                string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                    companyId, string.Join(", ", dialerIds), campaignId, dialingMode));
            
            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);
                
                try
                {
                    var ctx = new SurveyContext(companyId, dialerId, campaignId);
                    dialer.RemoveSurveySession(ctx);
                }
                catch (Exception ex)
                {
                    _logger.Error("AwsConnectDialerWS.StopCampaign", () => ex.Message);
                    return DialerErrorCode.Exception;
                }
            }
            
            return DialerErrorCode.Success;
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            _logger.Verbose("AwsConnectDialerWS.KillCampaign",
                string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                    companyId, string.Join(", ", dialerIds), campaignId, dialingMode));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode,
            bool recordWholeInterview, string campaignParametersXml)
        {
            _logger.Verbose("AwsConnectDialerWS.SetCampaignParameters",
                string.Format("companyId = {0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}, recordWholeInterview={4}, campaignParametersXml=[{5}]",
                    companyId, string.Join(", ", dialerIds), campaignId, dialingMode, recordWholeInterview, campaignParametersXml));

            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);

                try
                {
                    var ctx = new SurveyContext(companyId, dialerId, campaignId);
                    dialer.UpdateSurveySession(ctx, campaignParametersXml);
                }
                catch (Exception ex)
                {
                    _logger.Error("AwsConnectDialerWS.SetCampaignParameters", () => ex.Message);
                    return DialerErrorCode.Exception;
                }
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType,
            string agentConnectionString, ResourceBindingType resourceBindingType, bool isPredictive,
            IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            _logger.Verbose("AwsConnectDialerWS.Login",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, agentName={4}, agentConnectionString={5}, resourceBindingType={6}, isPredictive={7}, agentAttributes={8}",
                    companyId, dialerId, campaignId, agentId, agentName, agentConnectionString, resourceBindingType, isPredictive,
                    agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString())));

            GetDialerWithCheck(companyId, dialerId);

            _dialerEvents.NotifyAgentState(
                companyId,
                dialerId,
                campaignId,
                agentId,
                AgentState.LoggedIn);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {
            _logger.Verbose("AwsConnectDialerWS.SetCampaign",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                    companyId, dialerId, campaignId, agentId));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            _logger.Verbose("AwsConnectDialerWS.Logout",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, isPredictive={4}",
                    companyId, dialerId, campaignId, agentId, isPredictive));

            GetDialerWithCheck(companyId, dialerId);

            _dialerEvents.NotifyAgentState(
                companyId,
                dialerId,
                campaignId,
                agentId,
                AgentState.LoggedOut);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {
            _logger.Verbose("AwsConnectDialerWS.KillAgent",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                    companyId, dialerId, campaignId, agentId));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            _logger.Verbose("AwsConnectDialerWS.GoReady",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                    companyId, dialerId, campaignId, agentId));
            
            return DialerErrorCode.Success;
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            _logger.Verbose("AwsConnectDialerWS.GoNotReady",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, breakName={4}",
                    companyId, dialerId, campaignId, agentId, breakName));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode SendNumberToAgent(int companyId, int dialerId, long campaignId, int agentId, DialingMode diallingMode,
            int interviewId, long callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, object> respondentVariables)
        {
            _logger.Verbose("AwsConnectDialerWS.SendNumberToAgent",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, diallingMode={4}, interviewId={5}, callId={6}, phoneNumber={7}, isRecording={8}, respondentVariables={9}",
                    companyId, dialerId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording, respondentVariables));

            var dialer = GetDialerWithCheck(companyId, dialerId);
            var ctx = new RespondentContext(companyId, dialerId, campaignId, agentId, interviewId, callId);

            try
            {
                dialer.CreateRespondentSession(ctx, phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.Error("AwsConnectDialerWS.SendNumberToAgent", () => ex.Message);
                dialer?.NotifyOutcome(ctx, CallOutcome.ExternallyValidatedNumber);
            }
            
            return DialerErrorCode.Success;
        }

        public DialerErrorCode Redial(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording, string callerId)
        {
            _logger.Verbose("AwsConnectDialerWS.Redial",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}, callerId = {8}",
                    companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, int dialerId, long campaignId,
            DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId, InterviewStatus interviewStatus,
            bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            _logger.Verbose("AwsConnectDialerWS.CompleteCall",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewStatus={4}, makeAgentReady={5}, breakName={6}, interviewId={7}, callId={8}",
                    companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady ? "true" : "false", makeAgentReady ? "NULL" : breakName, interviewId, callId));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StartCustomIvrInterview(int companyId, int dialerId, long campaignId, int agentId, int interviewId,
            long callId, string respondentSurveyLink)
        {
            _logger.Verbose("AwsConnectDialerWS.StartCustomIvrInterview",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, respondentSurveyLink={6}",
                    companyId, dialerId, campaignId, agentId, interviewId, callId, respondentSurveyLink));

            var dialer = GetDialerWithCheck(companyId, dialerId);

            try
            {
                var ctx = new RespondentContext(companyId, dialerId, campaignId, agentId, interviewId, callId);
                dialer.StartCall(ctx, respondentSurveyLink);
            }
            catch (Exception ex)
            {
                _logger.Error("AwsConnectDialerWS.StartCustomIvrInterview", () => ex.Message);
                return DialerErrorCode.Exception;
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode CompletePreview(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string phoneNumber, bool isRecording)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StartRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string label)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StopRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            StopRecordingMode stopRecordingMode)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StartPlayback(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = 0;
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StopPlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode PauseOrResumePlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId,
            int agentId, long callId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StartMonitor(int companyId, int dialerId, int agentId, string supervisorName,
            string supervisorConnectionString, ResourceBindingType resourceBindingType, ref string sessionId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            trunkLineStatesAndAlarms = null;
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode TransferToIvr(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId,
            string endpoint, IEnumerable<KeyValuePair<string, string>> attributes)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(int companyId, int dialerId, InboundDdiNumber[] inboundDdiNumbers)
        {
            return Enumerable.Repeat(DialerErrorCode.NotSupported, inboundDdiNumbers.Length).ToArray();
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId,
            AudioMessageDescriptor audioMessageDescriptor)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId, string transferId,
            ConnectionState state)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            return DialerErrorCode.NotSupported;
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login,
            out string password, out string host, out string extension, out string frontendUrl)
        {
            login = null;
            password = null;
            host = null;
            extension = null;
            frontendUrl = null;
            return DialerErrorCode.NotSupported;
        }

        public void Dispose()
        {
            foreach (var dialerInstanceCacheItem in _dialers.Values)
            {
                dialerInstanceCacheItem.Dialer?.Dispose();
            }
        }

        
        public void InitializeRecording(int dialerId)
        {
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId)
        {
            return Enumerable.Empty<AudioRecordInfo>();
        }

        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            return null;
        }
        
        public BulkAudioResult GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId)
        {
            return null;
        }

        public bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds, int dialerId)
        {
            return Enumerable.Repeat(false, interviewIds.Length).ToArray();
        }
    }
}