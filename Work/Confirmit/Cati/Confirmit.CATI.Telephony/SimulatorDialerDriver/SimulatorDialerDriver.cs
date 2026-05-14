using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Security.Policy;
using System.Threading;
using System.Web.Hosting;
using Confirmit.CATI.Common;
using ConfirmitDialerInterface;
using DialerCommon;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using SimulatorDialerDriver;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriver.Models;
using SimulatorDialerDriver.WebApi;
using ILogger = ConfirmitDialerInterface.ILogger;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class SimulatorDialerDriver : ISimulator, IDialerCoreApi, IDialerRecordingApi, IDisposable
    {
        public static ISimulator Instance { get; private set; }
        public ConcurrentDictionary<string, Dialer> Dialers { get; } = new ConcurrentDictionary<string, Dialer>();

        public IDialerEvents DialerEvents { get; }
        public ILogger Logger { get; }
        public SimulatorScenario Scenario { get; }
        public ICallOutcomeDistributor CallOutcomeDistributor { get; }
        public SimulatorActivities Activities { get; } = new SimulatorActivities();

        public RequestId RequestId { get; private set; }

        private static IDisposable _webApp;

        public SimulatorDialerDriver(IDialerEvents dialerEvents, ILogger logger)
        {
            Instance = this;
            DialerEvents = dialerEvents;
            Logger = logger;
            RequestId = new RequestId();

            logger.Info("SimulatorDialerDriver.SimulatorDialerDriver",
                "ScenarioXmlFileName='{0}', StatefulMode='{1}'",
                Settings.Default.ScenarioXmlFileName, Settings.Default.StatefulMode);

            try
            {
                var scenarioFullFileName = Settings.Default.ScenarioXmlFileName;

                if (Path.GetDirectoryName(scenarioFullFileName) == string.Empty)
                {
                    scenarioFullFileName = Path.Combine(GetServiceAppDataPath(), scenarioFullFileName);
                }

                logger.Info("SimulatorDialerDriver.SimulatorDialerDriver",
                    "scenarioFullFileName=[{0}]",
                    scenarioFullFileName);

                Scenario = new SimulatorScenarioDeserializer().Deserialize(scenarioFullFileName);

                if (Settings.Default.EnablePerformanceCounters)
                {
                    logger.Info("SimulatorDialerDriver.SimulatorDialerDriver", "Initialising performance counters ...");
                    SimulatorDialerDriverPerformanceCounters.Initialize(logger);
                    logger.Info("SimulatorDialerDriver.SimulatorDialerDriver", "Performance counters are initialized");
                }
                else
                {
                    logger.Info("SimulatorDialerDriver.SimulatorDialerDriver", "Performance counters are disabled");
                }
            }
            catch (Exception ex)
            {
                logger.Error("SimulatorDialerDriver.SimulatorDialerDriver", "{0}", ex);

                throw;
            }

            if (_webApp == null)
            {
                var webAppUrl = Settings.Default.WebApiUrl;
                _webApp = WebApp.Start<Startup>(webAppUrl);
                logger.Info("SimulatorDialerDriver.SimulatorDialerDriver", "Web Application is started at URL: [{0}]", webAppUrl);
            }

            CallOutcomeDistributor = new CallOutcomeDistributor(Scenario.CallOutcomeDistributionScenario);
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
            Logger.Verbose("SimulatorDialerDriver.Dispose", "Start disposing");

            Release(0, 0); // dialerId in Release method isn't used by confirmit code, it is introduced for clients inner usage

            Logger.Info("SimulatorDialerDriver.Dispose", "SimulatorDialerDriver object is disposed");
        }

        string DialerKey(int companyId, int dialerId) => $"{companyId}:{dialerId}";

        public Dialer TryGetDialer(int companyId, int dialerId)
        {
            Dialers.TryGetValue(DialerKey(companyId, dialerId), out var dialer);

            return dialer;
        }

        public Dialer GetDialerWithCheck(int companyId, int dialerId)
        {
            var dialer = TryGetDialer(companyId, dialerId);

            if (dialer == null)
            {
                throw new DialerIsNotInitializedException($"Simulator Dialer(CompanyId:{companyId}, DialerId:{dialerId}) is not Initialized");
            }

            return dialer;
        }

        public string GetName()
        {
            return @"SimulatorDialerDriver";
        }

        public string GetVersion()
        {
            return @"1.5";
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {
            CollectTrash();

            Logger.Verbose("SimulatorDialerDriver.Initialize",
                "Version={0} companyId={1}, dialerId={2}, configurationParametersXml=[{3}]",
                GetVersion(), companyId, dialerId, configurationParametersXml);

            lock (Dialers)
            {
                if (Dialers.TryGetValue(DialerKey(companyId, dialerId), out var dialer))
                {
                    // Already initialized.

                    // Note, "{0:yyyy-MM-dd HH:mm:ss.fff}" is the same format as being used in the logger
                    Logger.Info("SimulatorDialerDriver.Initialize",
                        "Already initialized. Initialization time (UTC): {0:yyyy-MM-dd HH:mm:ss.fff}",
                        dialer.InitializationTime);

                    dialer.ValidateState();

                    return DialerErrorCode.Success;
                }

                Logger.Info("SimulatorDialerDriver.Initialize", "Initialize from a not initialized state.");

                Dialers.TryAdd(DialerKey(companyId, dialerId), value: new Dialer(companyId, dialerId, this, Logger, DialerEvents));
            }

            return DialerErrorCode.Success;
        }

        private void CollectTrash()
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }

        public IDialerFeatures GetFeatures(int companyId, int dialerId)
        {
            return new SimulatorDialerFeatures
            {
                IsBargingSupported = Settings.Default.IsBargingSupported,
                IsCoachingSupported = Settings.Default.IsCoachingSupported,
                IsMonitoringMuteSupported = Settings.Default.IsMonitoringMuteSupported,
                IsExternalTransferSupported = Settings.Default.IsExternalTransferSupported,
                IsInternalTransferSupported = Settings.Default.IsInternalTransferSupported,
                IsInboundSupported = Settings.Default.IsInboundSupported,
                IsIVRSupported = Settings.Default.IsIVRSupported,
                IsSoftphoneSingleSignOnSupported = Settings.Default.IsSoftphoneSingleSignOnSupported,
                IsAudioContentDownloadSupported = Settings.Default.IsAudioContentDownloadSupported,
                CustomIvrPipeline = Settings.Default.CustomIvrPipeline
            };
        }

        public DialerErrorCode Release(int dialerId, int companyId)
        {
            Logger.Verbose("SimulatorDialerDriver.Release", $"DialerId={dialerId}, companyId={companyId}");

            if (Dialers.TryRemove(DialerKey(companyId, dialerId), out var dialer))
            {
                dialer.Destroy();
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode RestoreDialerDriverState(int companyId, string filename)
        {
            Logger.Verbose("SimulatorDialerDriver.RestoreDialerDriverState", "companyId={0}, filename={1}", companyId, filename);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SaveDialerDriverState(string filename)
        {
            Logger.Verbose("SimulatorDialerDriver.RestoreDialerDriverState", "filename={0}", filename);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            Logger.Verbose("SimulatorDialerDriver.SetConfigurationParameters",
                "companyId={0}, configurationParametersXml=[{1}]",
                companyId, configurationParametersXml);

            return DialerErrorCode.Success;
        }

        public DialerState GetState(int companyId, int dialerId)
        {
            GetDialerWithCheck(companyId, dialerId);

            return DialerState.Available;
        }

        public DialerErrorCode StartCampaign(
            int companyId,
            int[] dialerIds,
            long campaignId,
            string campaignName,
            DialingMode dialingMode,
            bool recordWholeInterview,
            string campaignParametersXml
            )
        {
            Logger.Verbose("SimulatorDialerDriver.StartCampaign",
                "companyId={0}, dialerIds=[{1}], campaignId={2}, campaignName={3}, dialingMode={4}, recordWholeInterview={5}, campaignParametersXml=[{6}]",
                companyId, string.Join(", ", dialerIds), campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml);

            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);

                var campaign = new Campaign(companyId, dialerId, campaignId, campaignName, dialingMode, recordWholeInterview);
                dialer.CampaignsManager.Start(campaign);
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            Logger.Verbose("SimulatorDialerDriver.StopCampaign",
                "companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode);

            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);

                var campaign = new Campaign(companyId, dialerId, campaignId, null, dialingMode);

                dialer.CampaignsManager.Stop(campaign);
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            Logger.Verbose("SimulatorDialerDriver.KillCampaign",
                "companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode);

            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);

                var campaign = new Campaign(companyId, dialerId, campaignId, null, dialingMode);

                dialer.CampaignsManager.Kill(campaign);

            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            Logger.Verbose("SimulatorDialerDriver.SetCampaignParameters",
                "companyId = {0}, dialerIds={1}, campaignId={2}, dialingMode={3}, recordWholeInterview={4}, campaignParametersXml=[{5}]",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode, recordWholeInterview, campaignParametersXml);

            foreach (var dialerId in dialerIds)
            {
                GetDialerWithCheck(companyId, dialerId);
            }

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, ResourceBindingType resourceBindingType, bool isPredictive, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            Logger.Verbose("SimulatorDialerDriver.Login",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, agentName={4}, agentConnectionString={5}, resourceBindingType={6}, isPredictive={7}, agentAttributes={8}",
                companyId, dialerId, campaignId, agentId, agentName, agentConnectionString, resourceBindingType, isPredictive,
                agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString()));

            var dialer = GetDialerWithCheck(companyId, dialerId);

            if (Scenario.LoginResultCode != DialerErrorCode.Success)
            {
                Logger.Warning("SimulatorDialerDriver.Login",
                    "Returning [{0}] as the rusult according to the setting in the Scenario", Scenario.LoginResultCode);

                return Generators.LoginResultCode.GetValue(new ContextInfo(companyId, dialerId, campaignId, agentId), Scenario.LoginResultCode);
            }

            dialer.InterviewersManager.Login(companyId, dialerId, campaignId, agentId, agentName, agentType, agentConnectionString, resourceBindingType, isPredictive, agentAttributes);

            return Generators.LoginResultCode.GetValue(new ContextInfo(companyId, dialerId, campaignId, agentId), DialerErrorCode.Success);
        }

        public DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {
            Logger.Verbose("SimulatorDialerDriver.SetCampaign",
                "companyId={0}, campaignId={1}, agentId={2}",
                companyId, campaignId, agentId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            try
            {
                dialer.InterviewersManager.Get(agentId).SetCampaignId(campaignId);
            }
            catch (DialerException ex)
            {
                Logger.Error("SimulatorDialerDriver.SetCampaign", ex.ToString());
                return ex.ErrorCode;
            }

            // Either DialerErrorCode.Success or an error as configured in the scenario
            return Generators.SetCampaignResultCode.GetValue(new ContextInfo(companyId, dialerId, campaignId, agentId), Scenario.SetCampaignResultCode);
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            Logger.Verbose("SimulatorDialerDriver.Logout",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, isPredictive={4}",
                companyId, dialerId, campaignId, agentId, isPredictive);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Logout(companyId, dialerId, campaignId, agentId);

            return Generators.LogoutResultCode.GetValue(new ContextInfo(companyId, dialerId, campaignId, agentId), DialerErrorCode.Success);
        }

        public DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {
            Logger.Verbose("SimulatorDialerDriver.KillAgent",
                 "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                 companyId, dialerId, campaignId, agentId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Logout(companyId, dialerId, campaignId, agentId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            var sw = Stopwatch.StartNew();

            Logger.Verbose("SimulatorDialerDriver.GoReady",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).SetReady(true);

            SimulatorDialerDriverPerformanceCounters.AverageOfGoReadyDurationPerSecond.IncrementBy(sw.Elapsed);
            SimulatorDialerDriverPerformanceCounters.RateOfGoReadyCountPerSecond.Increment();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            var sw = Stopwatch.StartNew();

            Logger.Verbose("SimulatorDialerDriver.GoNotReady",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, breakName={4}",
                companyId, dialerId, campaignId, agentId, breakName);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).SetReady(false);

            SimulatorDialerDriverPerformanceCounters.AverageOfGoNotReadyDurationPerSecond.IncrementBy(sw.Elapsed);
            SimulatorDialerDriverPerformanceCounters.RateOfGoNotReadyCountPerSecond.Increment();

            AsyncManager.Execute(Logger, () =>
            {
                Thread.Sleep(Scenario.GoNotReadyNotificationDelay);

                DialerEvents.NotifyAgentState(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    AgentState.NotReady);
            });

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            Logger.Verbose("SimulatorDialerDriver.SetGroups",
                "companyId={0}, dialerId={1}, agentId={2}, campaignId={3}, agentGroups=[{4}]",
                companyId, dialerId, agentId, campaignId, agentGroups.ToArray());

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).SetGroups(agentGroups);

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
            var sw = Stopwatch.StartNew();

            Logger.Verbose("SimulatorDialerDriver.SendNumberToAgent",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, diallingMode={4}, interviewId={5}, callId={6}, phoneNumber={7}, isRecording={8}, respondentVariables={9}",
                companyId, dialerId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording, respondentVariables?.Stringify());

            var dialer = GetDialerWithCheck(companyId, dialerId);

            if (!Settings.Default.StatefulMode)
            {
                // Stateless mode means 'autoinitialization'
                dialer.CampaignsManager.Start(new Campaign(companyId, dialerId, campaignId, null, DialingMode.Preview));
            }

            dialer.InterviewersManager.Get(agentId).SendNumberToAgent(companyId, dialerId, new CallManager.CallInfoEx(new CallInfo()
            {
                agentId = agentId,
                diallingMode = diallingMode,
                interviewId = interviewId,
                callId = callId,
                phoneNumber = phoneNumber,
                isRecording = isRecording,
                callerId = callerId

            }, campaignId, CallManager.CallType.Outbound));

            SimulatorDialerDriverPerformanceCounters.AverageOfSendNumberToAgentDurationPerSecond.IncrementBy(sw.Elapsed);
            SimulatorDialerDriverPerformanceCounters.RateOfSendNumberToAgentCountPerSecond.Increment();

            return Generators.SendNumberToAgentResultCode.GetValue(new ContextInfo(companyId, dialerId, campaignId, agentId), DialerErrorCode.Success);
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
            Logger.Verbose("SimulatorDialerDriver.Redial",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}, callerId = {8}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            // We assume here that we have the only instance of SimulatorDialerDriver inside a single dialer WS instanse
            // Changes are needed in order tu support multi-instanse configuration.
            dialer.InterviewersManager.Get(agentId).Redial(companyId, dialerId, new CallManager.CallInfoEx(new CallInfo()
            {
                agentId = agentId,
                interviewId = interviewId,
                callId = callId,
                phoneNumber = phoneNumber,
                isRecording = isRecording,
                callerId = callerId

            }, campaignId, CallManager.CallType.Outbound));

            //TODO: add performance counters

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout)
        {
            Logger.Verbose("SimulatorDialerDriver.SendNumbers",
                "requestId={0}, companyId={1}, dialerId={2}, campaignId={3}, campaignDialingMode={4}, NumberOfCalls={5}, callAgingTimeout={6}",
                requestId, companyId, dialerId, campaignId, campaignDialingMode, callList.Count, callAgingTimeout);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            var surveyInstance = dialer.CampaignsManager.TryGetPredictive(campaignId);

            if (surveyInstance != null && callList.Count > 0)
            {
                surveyInstance.CallManager.AddCalls(campaignId, callList);
            }

            SimulatorDialerDriverPerformanceCounters.AverageOfReceivedCallsCountPerSecond.IncrementBy(callList.Count);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId)
        {
            var sw = Stopwatch.StartNew();

            Logger.Verbose("SimulatorDialerDriver.Hangup",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}",
                companyId, dialerId, campaignId, agentId, interviewId, callId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).Hangup();

            SimulatorDialerDriverPerformanceCounters.AverageOfHangupDurationPerSecond.IncrementBy(sw.Elapsed);
            SimulatorDialerDriverPerformanceCounters.RateOfHangupCountPerSecond.Increment();

            return DialerErrorCode.Success;
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId,
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            var sw = Stopwatch.StartNew();

            Logger.Verbose("SimulatorDialerDriver.CompleteCall",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewStatus={4}, makeAgentReady={5}, breakName={6}, interviewId={7}, callId={8}",
                companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady ? "true" : "false", makeAgentReady ? "NULL" : breakName, interviewId, callId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            var interviewer = dialer.InterviewersManager.Get(agentId);
            if (interviewer.ActiveCall != null)
            {
                dialer.Transfers.OnCallCompleted(companyId, dialerId, campaignId, interviewer);
                interviewer.CompleteCall();
                interviewer.SetReady(makeAgentReady);

                SimulatorDialerDriverPerformanceCounters.AverageOfCompleteCallsDurationPerSecond.IncrementBy(sw.Elapsed);
                SimulatorDialerDriverPerformanceCounters.RateOfCompleteCallsCountPerSecond.Increment();
            }
            else
                Logger.Warning("SimulatorDialerDriver.CompleteCall",
                   "CompleteCall without previous dial - just ignore");

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            Logger.Verbose("SimulatorDialerDriver.SetNextInterview",
                "companyId={0}, dialerId={1}, currentCampaignId={2}, agentId={3}"
                + ", currentInterviewStatus={4}, nextCampaignId={5}, nextInterviewId={6}, nextCallId={7}",
                companyId, dialerId, currentCampaignId, agentId,
                currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).SetCampaignId(nextCampaignId);

            dialer.InterviewersManager.Get(agentId).SetNextInterview(nextCampaignId, nextCallId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StartCustomIvrInterview(int companyId, int dialerId, long campaignId,
            int agentId, int interviewId, long callId, string respondentSurveyLink)
        {
            Logger.Verbose("SimulatorDialerDriver.StartCustomIvrInterview",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}"
                + ", interviewId={4}, callId={5}, respondentSurveyLink={6}",
                companyId, dialerId, campaignId, agentId,
                interviewId, callId, respondentSurveyLink);
            
            var dialer = GetDialerWithCheck(companyId, dialerId);
            
            AsyncManager.Execute(Logger, () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(Convert.ToDouble(Scenario.IvrAnswerDelayInSeconds)));
            
                DialerEvents.NotifyCustomIvrInterviewEnd(
                    companyId,
                    dialerId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    CallOutcome.Completed);
            });
            
            return DialerErrorCode.Success;
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
            Logger.Verbose("SimulatorDialerDriver..UpdateInterviewStatus",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, interviewStatus={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, interviewStatus);

            GetDialerWithCheck(companyId, dialerId);

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
            Logger.Verbose("SimulatorDialerDriver.CompletePreview",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).CompletePreview(companyId, dialerId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {
            foreach (var dialerId in dialerIds)
            {
                var dialer = GetDialerWithCheck(companyId, dialerId);

                // step one - remove thses calls from the call manager
                var surveyInstance = dialer.CampaignsManager.TryGetPredictive(campaignId);

                if (surveyInstance != null)
                {
                    surveyInstance.CallManager.RemoveCalls(callList);
                }

                // step two need all calls return back with pre-defined dial result
                AsyncManager.Execute(Logger, () =>
                {
                    foreach (var call in callList)
                    {
                        DialerEvents.NotifyOutcome(
                            companyId,
                            dialerIds[0], // TODO: Assume one dialer indeed, what to do if more? In the real system for instance is it mean we should send the list to the all of them?
                            campaignId,
                            0,
                            call.interviewId,
                            call.callId,
                            CallOutcome.ReturnedNotDialled, // CallOutcome.InterruptedBySystem?? // TODO: What exact call outcome for Generic type of the dialer
                            null,
                            TimeSpan.Zero,
                            null,
                            null);
                    }
                });
            }

            return DialerErrorCode.Success;
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
            Logger.Verbose("SimulatorDialerDriver.StartRecording",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5},label={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, label);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).StartSectionalRecording(label);

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
            Logger.Verbose("ProtsDialerDriver.StopRecording",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, stopRecordingMode={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, stopRecordingMode);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(agentId).StopRecording(stopRecordingMode);

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
            Logger.Verbose("SimulatorDialerDriver.StartPlayback",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, fileName={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, fileName);

            GetDialerWithCheck(companyId, dialerId);

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
            Logger.Verbose("SimulatorDialerDriver.StopPlayback",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId);

            GetDialerWithCheck(companyId, dialerId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode PauseOrResumePlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            Logger.Verbose("SimulatorDialerDriver.PauseOrResumePlayback",
                "companyId = {0}, dialerId={1}, campaignId={2}, agentId={3},  callId={4}",
                companyId, dialerId, campaignId, agentId, callId);

            GetDialerWithCheck(companyId, dialerId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            Logger.Verbose("SimulatorDialerDriver.ToggleInterviewerListensToPlaybackOrRespondent",
               "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, callId={4}",
               companyId, dialerId, campaignId, agentId, callId);

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
            Logger.Verbose("ProtsDialerDriver.MonitorStart",
                "companyId={0}, dialerId={1}, agentId={2}, supervisorName={3}, supervisorConnectionString={4}, resourceBindingType={5}, ref sessionId={6}",
                companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, sessionId);

            GetDialerWithCheck(companyId, dialerId);

            sessionId = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            Logger.Verbose("SimulatorDialerDriver.MonitorStop", "companyId={0}, dialerId={1}, monitorExtension(sessionId)={2}", companyId, dialerId, sessionId);

            GetDialerWithCheck(companyId, dialerId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            Logger.Verbose("SimulatorDialerDriver.SetMonitorMode", $"companyId={companyId}, dialerId={dialerId}, monitorExtension(sessionId)={sessionId}, monitorMode={monitorMode}");

            GetDialerWithCheck(companyId, dialerId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            Logger.Verbose("SimulatorDialerDriver.GetTrunkLineStatesAndAlarms", "companyId={0}, dialerId={1}", companyId, dialerId);

            GetDialerWithCheck(companyId, dialerId);

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
            Logger.Verbose("SimulatorDialerDriver.TransferToIvr",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}" +
                "interviewId={4}, callId={5}, endpoint={6}, " +
                "attrubutes={7}",
                companyId, dialerId, campaignId, agentId,
                interviewId, callId, endpoint,
                string.Join(", ", attributes));

            return DialerErrorCode.Success;
        }

        public DialerErrorCode IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {
            Logger.Verbose("SimulatorDialerDriver.IvrRenderVoiceXml",
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}," +
                "voiceXml=[{4}]",
                companyId, dialerId, campaignId, agentId, voiceXml);

            // That's not a good solution. XML parsing and responce generation should be separated
            var randomResponse = new VoiceXmlProcessor().GenerateResponse(voiceXml);
            SetStaticResponseIfExists(randomResponse);

            AsyncManager.Execute(Logger, () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(Convert.ToDouble(Scenario.IvrAnswerDelayInSeconds)));

                DialerEvents.NotifyIvrSubmit(
                    companyId, dialerId, campaignId, agentId, randomResponse.ToSubmitVariables());
            });

            return DialerErrorCode.Success;
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            Logger.Verbose("SimulatorDialerDriver.ConfigureInboundDdiNumbers",
                "companyId={0}, dialerId={1}, inboundDdiNumbers={2}",
                companyId, dialerId, JsonConvert.SerializeObject(inboundDdiNumbers));

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.ConfigureInboundDdiNumbers(inboundDdiNumbers);

            return Enumerable.Repeat(DialerErrorCode.Success, inboundDdiNumbers.Length).ToArray();
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            Logger.Verbose("SimulatorDialerDriver.DropInboundCall",
                "companyId={0}, dialerId={1}, inboundCallId={2}, audioMessageDescriptor = {3} ",
                companyId, dialerId, inboundCallId, audioMessageDescriptor.NullableToString());

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.GlobalInboundCalls.RemoveInboundCall(inboundCallId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode ConnectInboundCall(
            int companyId,
            int dialerId,
            long campaignId,
            string inboundCallId,
            CallInfo callInfo,
            long[] campaignIdsToBorrowAgentsFrom,
            AudioMessageDescriptor audioMessageDescriptor)
        {
            Logger.Verbose("SimulatorDialerDriver.ConnectInboundCall",
                "companyId={0}, dialerId={1}, campaignId={2}, inboundCallId={3}," +
                "callInfo=[{4}], campaignIdsToBorrowAgentsFrom=[{5}], audioMessageDescriptor = {6} ",
                companyId, dialerId, campaignId, inboundCallId,
                callInfo, campaignIdsToBorrowAgentsFrom != null ? string.Join(", ", campaignIdsToBorrowAgentsFrom) : "<NULL>",
                audioMessageDescriptor.NullableToString());

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.GlobalCallManager.AddInboundCall(campaignId, campaignIdsToBorrowAgentsFrom, callInfo);

            dialer.GlobalInboundCalls.RemoveInboundCall(inboundCallId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode ConnectInboundCallToAgent(
            int companyId,
            int dialerId,
            long campaignId,
            string inboundCallId,
            CallInfo callInfo,
            AudioMessageDescriptor audioMessageDescriptor)
        {
            Logger.Verbose("SimulatorDialerDriver.ConnectInboundCall",
                "companyId={0}, dialerId={1}, campaignId={2}, inboundCallId={3}," +
                "callInfo=[{4}], audioMessageDescriptor = {5} ",
                companyId, dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor.NullableToString());

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.InterviewersManager.Get(callInfo.agentId).ConnectInboundCallToAgent(companyId, dialerId,
                new CallManager.CallInfoEx(callInfo, campaignId, CallManager.CallType.Inbound));

            dialer.GlobalInboundCalls.RemoveInboundCall(inboundCallId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            Logger.Verbose("SimulatorDialerDriver.TransferStart",
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}," +
                " agentId={4}, transferType={5} ",
                companyId, dialerId, campaignId, transferId, agentId, transferType);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.Transfers.Start(companyId, dialerId, campaignId, transferId, agentId, transferType);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId, TargetType targetType,
            string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            Logger.Verbose("SimulatorDialerDriver.TransferSetTarget",
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}," +
                " targetType={4}, targetResource={5}, borrowAgentsFromAllCampaigns={6}",
                companyId, dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.Transfers.SetTarget(companyId, dialerId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId,
            string transferId, ConnectionState state)
        {
            Logger.Verbose("SimulatorDialerDriver.TransferSetConnectionState",
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}, state={4}",
                companyId, dialerId, campaignId, transferId, state);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.Transfers.SetConnectionState(transferId, state);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            Logger.Verbose("SimulatorDialerDriver.TransferComplete",
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}",
                companyId, dialerId, campaignId, transferId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.Transfers.TransferComplete(transferId);

            return DialerErrorCode.Success;
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            Logger.Verbose("SimulatorDialerDriver.TransferCancel",
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}",
                companyId, dialerId, campaignId, transferId);

            var dialer = GetDialerWithCheck(companyId, dialerId);

            dialer.Transfers.TransferCancel(transferId);

            return DialerErrorCode.Success;
        }

        private void SetStaticResponseIfExists(IvrSimulatedResponse simulatedResponse)
        {
            if (!simulatedResponse.SimulatedUserInput.HasValue)
            {
                return;
            }

            var keyValue = simulatedResponse.SimulatedUserInput.Value;

            if (!Scenario.IvrQuestionIdToAnswer.ContainsKey(keyValue.Key))
            {
                return;
            }

            simulatedResponse.SimulatedUserInput = new KeyValuePair<string, string>(
                keyValue.Key,
                Scenario.IvrQuestionIdToAnswer[keyValue.Key]);
        }

        public void InitializeRecording(int dialerId)
        {
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId, int dialerId)
        {
            Logger.Verbose("SimulatorDialerDriver.GetAudioRecords",
                "companyId={0},campaignId={1}, interviewId={2}, dialerId={3}",
                companyId, campaignId, interviewId, dialerId);

            var result = new List<AudioRecordInfo>();

            var audioFolderName = string.IsNullOrEmpty(Scenario.AudioFolderName) ? "audio" : Scenario.AudioFolderName;
            var audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, audioFolderName);
            var surveyFolder = new DirectoryInfo(audioPath).GetDirectories().FirstOrDefault(x => x.Name == campaignId.ToString());

            if (surveyFolder == null)
            {
                return result;
            }

            var audioSchema = string.IsNullOrEmpty(Scenario.AudioServerSchema) ? "http" : Scenario.AudioServerSchema;
            var audioServer = string.IsNullOrEmpty(Scenario.AudioServerName) ? System.Net.Dns.GetHostEntry("").HostName : Scenario.AudioServerName;

            var files = surveyFolder.GetFiles($"{interviewId}_*.wav");
            string path;
            var defaultHostname = Environment.GetEnvironmentVariable("Confirmit__DefaultHostname");
            if (defaultHostname != null)
            {
                // for k8s
                path = $"https://{defaultHostname}/catidialersimulator/audio/{surveyFolder.Name}/";
            }
            else
            {
                path = $"{audioSchema}://{audioServer}{HostingEnvironment.ApplicationVirtualPath}/{audioFolderName}/{surveyFolder.Name}/";
            }

            foreach (var audioFile in files)
            {
                result.Add(new AudioRecordInfo { DateTime = DateTime.SpecifyKind(audioFile.CreationTimeUtc, DateTimeKind.Utc), Url = $"{path}{audioFile.Name}" });
            }

            return result;
        }

        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            Logger.Verbose("SimulatorDialerDriver.GetAudioFile",
                            "companyId={0}, dialerId={1}, audioUrl={2}",
                            companyId, dialerId, audioUrl);

            var result = new AudioFile { CreationTime = DateTime.Now };
            using (var client = new WebClient())
            {
                result.Content = client.DownloadData(audioUrl);
            }

            result.FileName = audioUrl.Split('/').Last();

            return result;
        }

        public BulkAudioResult GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities, int dialerId)
        {
            var result = new BulkAudioResult();
            var resultAudio = new List<AudioRecordInfo[]>();
            var campaignInterviewIdentities = interviewIdentities as CampaignInterviewIdentity[] ?? interviewIdentities.ToArray();

            Logger.Verbose("SimulatorDialerDriver.GetBulkAudioRecords",
                "companyId={0},interviewIdentities={1}, dialerId={2}",
                companyId, string.Join(",", campaignInterviewIdentities), dialerId);

            foreach (var campaignInterviewIdentity in campaignInterviewIdentities)
            {
                resultAudio.Add(GetAudioRecords(companyId, campaignInterviewIdentity.CampaignId, campaignInterviewIdentity.InterviewId, dialerId).ToArray());
            }

            result.AudioRecords = resultAudio.ToArray();
            result.CampaignInterviewIdentities = campaignInterviewIdentities;

            return result;
        }

        public bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds, int dialerId)
        {
            Logger.Verbose("SimulatorDialerDriver.AreRecordsExists",
                "companyId={0},campaignId={1}, interviewIds={2}",
                companyId, campaignId, string.Join(",", interviewIds));

            return interviewIds.Select(interviewId => GetAudioRecords(companyId, campaignId, interviewId, dialerId).Any()).ToArray();
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            Logger.Verbose("SimulatorDialerDriver.RegisterAgentSoftphone", "companyId={0}, dialerId={1}, agentId={2}, agentName={3}", companyId, dialerId, agentId, agentName);

            var softphoneFile = "SoftphoneSimulatorClient.html";
            string softphoneUrl;
            var defaultHostname = Environment.GetEnvironmentVariable("Confirmit__DefaultHostname");
            if (defaultHostname != null)
            {
                // for k8s
                softphoneUrl = $"https://{defaultHostname}/catidialersimulator/{softphoneFile}";
            }
            else
            {
                softphoneUrl = $"https://{Dns.GetHostEntry("").HostName}{HostingEnvironment.ApplicationVirtualPath}/{softphoneFile}";
            }

            var outcome = Generators.RegisterAgentSoftphoneOutcome.GetValue(new ContextInfo(companyId, dialerId, 0, agentId), new RegisteredSoftphoneAgent("testuser", "testuser", "", "ext999", softphoneUrl));

            login = outcome.Login;
            password = outcome.Password;
            host = outcome.Host;
            // Although we have extension here, the actual extension number will be passed from integrated client
            // Via postMessage.
            extension = outcome.Extension;
            frontendUrl = outcome.FrontendUrl;

            return Generators.RegisterAgentSoftphoneResultCode.GetValue(new ContextInfo(companyId, dialerId, 0, agentId), DialerErrorCode.Success);
        }
    }
}
