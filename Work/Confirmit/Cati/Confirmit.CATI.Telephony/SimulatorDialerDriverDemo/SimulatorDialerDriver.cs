using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;
using SimulatorDialerDriver;
using ILogger = ConfirmitDialerInterface.ILogger;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SimulatorDialerDriver : IDialerCoreApi, IDialerRecordingApi, IDisposable
    {
        private readonly IDialerEvents _dialerEvents;
        private readonly ILogger _logger;

        private SimulatorScenario Scenario { get; set; }

        private ConcurrentDictionary<long, SurveyInstance> SurveyInstances { get; set; }

        private bool IsInitialized { get; set; }
        private int _dialerId;

        public SimulatorDialerDriver(IDialerEvents dialerEvents, ILogger logger)
        {
            _dialerEvents = dialerEvents;
            _logger = logger;

            logger.WriteLine(TraceEventType.Information, "SimulatorDialerDriver.SimulatorDialerDriver",
                string.Format("ScenarioXmlFileName='{0}', StatefulMode='{1}'",
                Settings.Default.ScenarioXmlFileName, Settings.Default.StatefulMode));

            try
            {
                var scenarioFullFileName = Settings.Default.ScenarioXmlFileName;

                if (Path.GetDirectoryName(scenarioFullFileName) == string.Empty)
                {
                    scenarioFullFileName = Path.Combine(GetServiceAppDataPath(), scenarioFullFileName);
                }

                Scenario = SimulatorScenario.Deserialize(scenarioFullFileName);
            }
            catch (Exception ex)
            {
                logger.WriteLine(TraceEventType.Error, "SimulatorDialerDriver.SimulatorDialerDriver", string.Format("{0}", ex));

                Scenario = new SimulatorScenario
                {
                    CallOutcomeList = new List<CallOutcome> { CallOutcome.Connected },
                    GenerationMethod = CallOutcomeGenerationMethod.Sequence
                };

                logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SimulatorDialerDriver", "Default scenario with the only CallOutcome.Connected will be used");
            }

            SurveyInstances = new ConcurrentDictionary<long, SurveyInstance>();

            if (Settings.Default.StatefulMode)
            {
                IsInitialized = false;
            }
            else
            {
                // Stateless mode means no initialization or 'autoinitialization'
                IsInitialized = true;
            }
        }

        public static string GetServiceAppDataPath()
        {
            string path = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).LocalPath).ToUpper();
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar)); // path above bin directory
            path = path + Path.DirectorySeparatorChar + "App_Data" + Path.DirectorySeparatorChar;
            return path;
        }

        public void Dispose()
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Dispose", "Start disposing");

            _logger.WriteLine(TraceEventType.Information, "SimulatorDialerDriver.Dispose", "SimulatorDialerDriver object is disposed");
        }

        private void CheckIfInitialized()
        {
            if (!IsInitialized)
            {
                throw new DialerIsNotInitializedException("Simulator Dialer is not Initialized");
            }
        }

        public string GetName()
        {
            return @"SimulatorDialerDriver (DEMO)";
        }

        public string GetVersion()
        {
            return @"1.1";
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Initialize",
                string.Format("companyId={0}, dialerId={1}, configurationParametersXml=[{2}]",
                companyId, dialerId, configurationParametersXml));

            _dialerId = dialerId;
            IsInitialized = true;

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Release(int dialerId, int companyId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Release",
                $"dialerId={dialerId}, companyId={companyId}");

            return DialerErrorCode.Success;
        }

        public IDialerFeatures GetFeatures(int companyId, int dialerId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.GetFeatures",
                string.Format("companyId={0}, dialerId={1}",
                companyId, dialerId));

            return new YourDialerFeaturesRealization
            {
                IsBargingSupported = true,
                IsCoachingSupported = true,
                IsMonitoringMuteSupported = true,
                IsExternalTransferSupported = false,
                IsInternalTransferSupported = false,
                IsInboundSupported = true,
                IsIVRSupported = false,
                IsSoftphoneSingleSignOnSupported = false,
                IsAudioContentDownloadSupported = false,
                CustomIvrPipeline = false
            };
        }

        public DialerErrorCode RestoreDialerDriverState(int companyId, string filename)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.RestoreDialerDriverState",
                string.Format("companyId={0}, filename={1}",
                companyId, filename));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SaveDialerDriverState(string filename)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.RestoreDialerDriverState",
                string.Format("filename={0}", filename));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SetConfigurationParameters",
                string.Format("companyId={0}, configurationParametersXml=[{1}]",
                companyId, configurationParametersXml));

            return DialerErrorCode.Success;
        }

        public DialerState GetState(int companyId, int dialerId)
        {
            CheckIfInitialized();
            
            // TODO: implement quick dialer health check logic  
            var isDialerAlive = true;
            
            return isDialerAlive ? DialerState.Available : DialerState.Unavailable;
        }

        public DialerErrorCode StartCampaign(
            int companyId,
            int[] dialerIds,
            long campaignId,
            string campaignName,
            DialingMode dialingMode,
            bool recordWholeInterview,
            string campaignParametersXml)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.StartCampaign",
                string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, campaignName={3}, dialingMode={4}, recordWholeInterview={5}, campaignParametersXml=[{6}]",
                companyId, string.Join(", ", dialerIds), campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml));

            CheckIfInitialized();

            AddSurvey(campaignId, dialingMode);

            return DialerErrorCode.Success;
        }

        private void AddSurvey(long campaignId, DialingMode dialingMode)
        {
            if ((dialingMode == DialingMode.Preview) || (dialingMode == DialingMode.Automatic))
            {
                try
                {
                    SurveyInstances.TryAdd(
                        campaignId,
                        new SurveyInstancePreview(
                            campaignId, new CallOutcomeSequence(Scenario.CallOutcomeList, Scenario.GenerationMethod)));
                }
                catch (Exception)
                {
                    _logger.WriteLine(TraceEventType.Error, "SimulatorDialerDriver.AddSurvey",
                        string.Format("campaignId={0}, dialingMode={1}",
                        campaignId, dialingMode));
                    throw;
                }
            }
            else
            {
                throw new Exception(string.Format("Dialing mode {0} is not supported", dialingMode));
            }
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.StopCampaign",
                string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode));

            CheckIfInitialized();

            SurveyInstance surveyInstance;
            SurveyInstances.TryRemove(campaignId, out surveyInstance);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.KillCampaign",
                string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode));

            CheckIfInitialized();

            SurveyInstance surveyInstance;
            SurveyInstances.TryRemove(campaignId, out surveyInstance);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SetCampaignParameters",
                string.Format("companyId = {0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}, recordWholeInterview={4}, campaignParametersXml=[{5}]",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode, recordWholeInterview, campaignParametersXml));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, ResourceBindingType resourceBindingType, bool isPredictive, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Login",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, agentName={4}, agentConnectionString={5}, resourceBindingType={6}, isPredictive={7}, agentAttributes={8}",
                companyId, dialerId, campaignId, agentId, agentName, agentConnectionString, resourceBindingType, isPredictive,
                agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString())));

            CheckIfInitialized();

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
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SetCampaign",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Logout",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, isPredictive={4}",
                companyId, dialerId, campaignId, agentId, isPredictive));

            CheckIfInitialized();

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
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.KillAgent",
                 string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                 companyId, dialerId, campaignId, agentId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.GoReady",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                    companyId, dialerId, campaignId, agentId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.GoNotReady",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, breakName={4}",
                    companyId, dialerId, campaignId, agentId, breakName));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SetGroups",
                string.Format("companyId={0}, dialerId={1}, agentId={2}, campaignId={3}, agentGroups=[{4}]",
                companyId, dialerId, agentId, campaignId, agentGroups.ToArray()));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SendNumberToAgent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            DialingMode diallingMode,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording,
            string callerId, 
            Dictionary<string, object> respondentVariables)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SendNumberToAgent",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, diallingMode={4}, interviewId={5}, callId={6}, phoneNumber={7}, isRecording={8}, respondentVariables={9}",
                companyId, dialerId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording, respondentVariables?.Stringify()));

            CheckIfInitialized();

            if (!Settings.Default.StatefulMode)
            {
                // Stateless mode means 'autoinitialization'

                AddSurvey(campaignId, diallingMode);
            }

            _dialerEvents.NotifyOutcome(
                companyId,
                dialerId,
                campaignId,
                agentId,
                interviewId,
                callId,
                SurveyInstances[campaignId].OutcomeSequence.GetOutcome(),
                null,
                TimeSpan.Zero,
                null,
                null);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Redial(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording,
            string callerId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Redial",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}, callerId = {8}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId));

            CheckIfInitialized();

            // We assume here that we have the only instance of SimulatorDialerDriver inside a single dialer WS instanse
            // Changes are needed in order tu support multi-instanse configuration.
            if (!SurveyInstances.ContainsKey(campaignId))
            {
                throw new Exception(string.Format("SurveyInstance[campaignId={0}] is not initialized", campaignId));
            }

            _dialerEvents.NotifyOutcome(
                companyId,
                dialerId,
                campaignId,
                agentId,
                interviewId,
                callId,
                SurveyInstances[campaignId].OutcomeSequence.GetOutcome(),
                null,
                TimeSpan.Zero,
                null,
                null);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout)
        {
            _logger.WriteLine(TraceEventType.Verbose, "ProtsDialerDriver.SendNumbers",
                string.Format("requestId={0}, companyId={1}, dialerId={2}, campaignId={3}, campaignDialingMode={4}, NumberOfCalls={5}, callAgingTimeout={6}",
                requestId, companyId, dialerId, campaignId, campaignDialingMode, callList.Count, callAgingTimeout));

            throw new NotImplementedException();
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.Hangup",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}",
                companyId, dialerId, campaignId, agentId, interviewId, callId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId, 
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.CompleteCall",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewStatus={4}, makeAgentReady={5}, breakName={6}, interviewId={7}, callId={8}",
                companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady ? "true" : "false", makeAgentReady ? "NULL" : breakName, interviewId, callId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode StartCustomIvrInterview(int companyId, int dialerId, long campaignId,
            int agentId, int interviewId, long callId, string respondentSurveyLink)
        {
            throw new NotImplementedException();
        }
        
        public DialerErrorCode UpdateInterviewStatus(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            InterviewStatus interviewStatus)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver..UpdateInterviewStatus",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, interviewStatus={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, interviewStatus));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode CompletePreview(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.CompletePreview",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode StartRecording(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string label)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.StartRecording",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5},label={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, label));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StopRecording(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            StopRecordingMode stopRecordingMode)
        {
            _logger.WriteLine(TraceEventType.Verbose, "ProtsDialerDriver.StopRecording",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, stopRecordingMode={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, stopRecordingMode));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StartPlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string fileName,
            out int timeOfPlayingInSeconds)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.StartPlayback",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, fileName={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, fileName));

            CheckIfInitialized();

            timeOfPlayingInSeconds = 5;

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StopPlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.StopPlayback",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode PauseOrResumePlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.PauseOrResumePlayback",
                string.Format("companyId = {0}, dialerId={1}, campaignId={2}, agentId={3}, callId={4}",
                companyId, dialerId, campaignId, agentId, callId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.ToggleInterviewerListensToPlaybackOrRespondent",
               string.Format("companyId = {0}, dialerId={1}, campaignId={2}, agentId={3}, callId={4}",
               companyId, dialerId, campaignId, agentId, callId));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StartMonitor(
            int companyId,
            int dialerId,
            int agentId,
            string supervisorName,
            string supervisorConnectionString,
            ResourceBindingType resourceBindingType,
            ref string sessionId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "ProtsDialerDriver.MonitorStart",
                string.Format("companyId={0}, dialerId={1}, agentId={2}, supervisorName={3}, supervisorConnectionString={4}, resourceBindingType={5}, ref sessionId={6}",
                companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, sessionId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.MonitorStop",
               string.Format("companyId={0}, dialerId={1}, monitorExtension(sessionId)={2}", companyId, dialerId, sessionId));

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.SetMonitorMode",
               $"companyId={companyId}, dialerId={dialerId}, monitorExtension(sessionId)={sessionId}, monitorMode={monitorMode}");

            CheckIfInitialized();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            _logger.WriteLine(TraceEventType.Verbose, "SimulatorDialerDriver.GetTrunkLineStatesAndAlarms",
                string.Format("companyId={0}, dialerId={1}", companyId, dialerId));

            CheckIfInitialized();

            trunkLineStatesAndAlarms = new List<TrunkLineStateAndAlarms>();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode TransferToIvr(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes)
        {
            _logger.WriteLine(TraceEventType.Verbose, "ProtsDialerDriver.TransferToIvr",
                string.Format("companyId={0}, dialerId={1}, campaignId={2}, agentId={3}" +
                "interviewId={4}, callId={5}, endpoint={6}, " +
                "attrubutes={7}",
                companyId, dialerId, campaignId, agentId,
                interviewId, callId, endpoint,
                string.Join(", ", attributes)));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(int companyId, int dialerId, InboundDdiNumber[] inboundDdiNumbers)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            throw new NotImplementedException();
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
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

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId, TargetType targetType,
            string targetResource, bool borrowAgentsFromAllCampaigns)
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


        public void InitializeRecording(int dialerId)
        {
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId)
        {
            var result = new List<AudioRecordInfo[]>();

            return (IEnumerable<AudioRecordInfo>)result;
        }

        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            var result = new AudioFile
            {
                FileName = "empty_test_file.wav", 
                Content = Array.Empty<byte>(), 
                CreationTime = DateTime.Now 
            };

            return result;
        }
        
        public BulkAudioResult GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId)
        {
            var result = new BulkAudioResult();
            var resultIds = new List<CampaignInterviewIdentity>();
            var resultAudio = new List<AudioRecordInfo[]>();

            result.AudioRecords = resultAudio.ToArray();
            result.CampaignInterviewIdentities = resultIds.ToArray();

            return result;
        }

        public bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds, int dialerId)
        {
            var result = Enumerable.Repeat(false, interviewIds.Length).ToArray();

            return result;
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            throw new NotImplementedException();
        }
    }
}
