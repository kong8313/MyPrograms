using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Telephony.DialerCommon;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;
using DialerCommon;
using DialerCommon.Logging;

namespace Confirmit.CATI.Telephony.DialerService
{
    [ErrorContextHandler(WebServiceType.Internal)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DialerService : IDialerService, IDisposable, IDialerEvents
    {
        public static ICommonLogger Logger = new Logger("DialerService");

        private readonly IDialerCoreApi _bridgeToDialer;
        private readonly IDialerRecordingApi _bridgeToDialerRecording;
        private readonly NotificationsSenderInitializer _notificationsSenderInitializer;
        private readonly DialerEventNotificationSenders _notificationSenders;

        public DialerServiceState ServiceState;

        private readonly RequestId _requestId = new RequestId();
        private readonly RequestCounter _requestCounter = new RequestCounter();

        private readonly DialerWsRequestsAuthoriser _dialerWsRequestsAuthoriser;

        private readonly HashSet<AgentState> _allowedAgentStates;

        private readonly SupervisorResourceBindingTypeSetting _supervisorResourceBindingTypeSetting;

        public DialerService()
        {
            Logger.InitReportingWsTraceListener();

            Logger.Info("DialerService.ctor",
                "DialerService object is created. Settings: [StatefulMode={0}, DialerId={1}, " +
                "UseAuthorization={2}, ServiceStateExpirationTimeout={3}]",
                Settings.Default.StatefulMode, Settings.Default.DialerId,
                Settings.Default.UseAuthorization, Settings.Default.ServiceStateExpirationTimeout);

            var dialerAssembly = string.Format("{0}.{1}, {2}",
                                               Settings.Default.DialerDriverAssemblyNamespace,
                                               Settings.Default.DialerDriverAssemblyMainClassName,
                                               Settings.Default.DialerDriverAssemblyName);

            _allowedAgentStates = GetAllowedAgentStatesFromConfig();

            Type dialerType;

            try
            {
                dialerType = Type.GetType(dialerAssembly, true);

                Logger.Info("DialerService.ctor",
                    "Dialer driver assembly [{0}] was successfully loaded from file [{1}]. The assembly info is [{2}]",
                    dialerAssembly,
                    dialerType.Assembly.CodeBase,
                    dialerType.Assembly.FullName);
            }
            catch (Exception ex)
            {
                Logger.Error("DialerService.ctor", "Dialer driver assembly [{0}] can't be loaded: {1}", dialerAssembly, ex);
                throw;
            }

            _notificationsSenderInitializer = new NotificationsSenderInitializer(Logger);
            _notificationSenders = new DialerEventNotificationSenders(_notificationsSenderInitializer);

            try
            {
                // Create instance of the Dialer driver dll

                var bridgeToDialerObject = Activator.CreateInstance(dialerType, (IDialerEvents)this, (ILogger)Logger);

                _bridgeToDialer = (IDialerCoreApi)bridgeToDialerObject;
                _bridgeToDialerRecording = (IDialerRecordingApi)bridgeToDialerObject;
            }
            catch (Exception ex)
            {
                Logger.Error("DialerService.Ctor", "Failed to create dialer driver object from assembly [{0}]. /// {1}", dialerAssembly, ex);
                throw;
            }

            string authorizationKeyForIncomingRequests = "";
            var authorizationEnabled = Settings.Default.UseAuthorization;

            if (authorizationEnabled)
            {
                using (var ecryptor = new DialerAuthorizationKeyEncryptor())
                {
                    authorizationKeyForIncomingRequests = ecryptor.DecryptString(Settings.Default.AuthorizationKeyForIncomingRequests);
                    ecryptor.Clear();
                }
            }

            _dialerWsRequestsAuthoriser = new DialerWsRequestsAuthoriser(authorizationKeyForIncomingRequests, authorizationEnabled);

            ServiceState = new DialerServiceState();

            if (Settings.Default.StatefulMode)
            {
                //TODO: We should call RestoreDialerDriverState in the both cases?
                RestoreServiceState();
            }

            if (Settings.Default.DialerId != 0)
            {
                // There is a hard configured value - let's use it
                ServiceState.dialerId = Settings.Default.DialerId;
            }

            _supervisorResourceBindingTypeSetting = new SupervisorResourceBindingTypeSetting(Logger);

            Logger.Info("DialerService.Ctor",
                "SupervisorResourceBindingType is {0}",
                _supervisorResourceBindingTypeSetting.IsSet
                    ? "[hard configured] as [" + _supervisorResourceBindingTypeSetting.Get() + "]"
                    : "[not hard configured]");
        }

        /// <summary>
        /// This constructor is for testing needs only
        /// </summary>
        /// <param name="dialerCoreApiObject"></param>
        /// <param name="logger"></param>
        public DialerService(IDialerCoreApi dialerCoreApiObject, ICommonLogger logger)
        {
            _bridgeToDialer = dialerCoreApiObject;
            Logger = logger;

            _dialerWsRequestsAuthoriser = new DialerWsRequestsAuthoriser("", false);

            _supervisorResourceBindingTypeSetting = new SupervisorResourceBindingTypeSetting(Logger);
        }

        private HashSet<AgentState> GetAllowedAgentStatesFromConfig()
        {
            var allowedStates = Settings.Default.AllowedAgentStates;
            if (allowedStates == null || allowedStates.Count == 0)
            {
                Logger.Warning(
                    "DialerService.GetAllowedAgentStatesFromConfig",
                    "'AllowedAgentStates' section in web.config is empty or does not exist. " +
                    "'LoggedIn' and 'LoggedOut' are allowed by default." +
                     "The following agent states are possible: {0}.", 
                     string.Join(", ", Enum.GetNames(typeof(AgentState))));

                return new HashSet<AgentState>
                    {
                        AgentState.LoggedIn,
                        AgentState.LoggedOut
                    };
            }

            var result = new HashSet<AgentState>();
            foreach (var state in allowedStates)
            {
                AgentState agentState;
                if (Enum.TryParse(state, true, out agentState))
                {
                    result.Add(agentState);
                }
                else
                {
                    Logger.Error(
                        "DialerService.GetAllowedAgentStatesFromConfig",
                        "Unknown agent state [{0}] in 'AllowedAgentStates' section of web.config, Dialer service cannot be started." +
                        "The following agent states are possible: {1}.", 
                        state,
                        string.Join(", ", Enum.GetNames(typeof(AgentState))));

                    throw new ArgumentException("Error loading 'AllowedAgentStates' from config.");
                }
            }

            Logger.Info(
                "DialerService.GetAllowedAgentStatesFromConfig",
                "The next agent states are allowed: {0}.", string.Join(", ", result));
            
            return result;
        }

        private void RestoreServiceState()
        {
            bool readSuccess = DialerServiceState.Load(ref ServiceState);

            if (readSuccess)
            {
                if (ServiceState.IsExpired())
                {
                    Logger.Info(
                        "DialerService.RestoreServiceState",
                        "Dialer service state expired. The expired values [companyId={0}, dialerId={1}] will be reset to 0.", 
                        ServiceState.companyId, ServiceState.dialerId);
                    ServiceState.Clear();

                    return;
                }

                if (ServiceState.companyId > 0)
                {
                    Execute(ServiceState.companyId,
                        "DialerService.RestoreServiceState",
                        "",
                        requestId => DoDialerCall(
                            () => _bridgeToDialer.RestoreDialerDriverState(ServiceState.companyId, GetDialerDriverStateFullFilename()), 

                            requestId));

                    new ServiceStartedNotificationSender(Logger).SendServiceStartedNotification(
                        ServiceState.dialerId,
                        ServiceState.companyId);
                }
                else
                {
                    Logger.Info(
                        "DialerService.RestoreServiceState",
                        "ServiceState.companyId={0}, so Dialer service state will not be restored.", ServiceState.companyId);
                }
            }
        }

        public void Dispose()
        {
            Logger.Verbose("DialerService.Dispose", "Start disposing");

            ServiceState.Save();

            Execute(0,
                "DialerService.Dispose",
                "",
                requestId => DoDialerCall(() => _bridgeToDialer.SaveDialerDriverState(GetDialerDriverStateFullFilename()), requestId));

            try
            {
                _notificationSenders.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error("DialerService.Dispose", ex.ToString());
            }

            Logger.Info("DialerService.Dispose", "DialerService object is disposed");
        }

        internal T Execute<T>(int companyId, string methodName, string methodArguments, Func<long, T> methodBody)
        {
            long id = _requestId.Next();
            var timer = Stopwatch.StartNew();

            Logger.Info(
                methodName,
                "Execute [{0}, {1}] /// {2}", id, _requestCounter.Increment(), methodArguments);

            T result = default(T);

            var resultString = string.Empty;

            try
            {
                //TODO: To think how to handle DialerWsInvalidCredentialsException
                _dialerWsRequestsAuthoriser.AuthoriseRequest();

                result = methodBody(id);
                resultString = result.ToString();

                //TODO: The code below is commented out because of it's not clear do we really need to throw DialerException
                //      instead of returning error codes. If we'll decide that the code is needed it possibly should be moved to
                //      "DialerErrorCode Execute()" overload in order to avoid type checking and casting
                // Throw DialerException in case of an error code returned
                //if (typeof(T) == typeof(DialerErrorCode))
                //{
                //    var errorCode = (DialerErrorCode)(Object)result;
                //
                //    if (errorCode != DialerErrorCode.Success)
                //    {
                //        throw new DialerException(
                //            errorCode, string.Format("DialerService.{0} failed with error: {1}.", methodName, errorCode));
                //    }
                //}

                return result;
            }
            catch (Exception ex)
            {
                Logger.Error(companyId, string.Format("DialerService.{0}", methodName), ex.ToString());
                resultString = ex.GetType() + " (re-thrown as FaultException)";

                throw new DialerExceptionToFaultException().Convert(ex);
            }
            finally
            {
                timer.Stop();

                Logger.Info(
                    methodName,
                    "Eof [{0}, {1}]. Result: {2}. Duration: {3} /// {4}",
                    id, _requestCounter.Decrement(), resultString, timer.ElapsedMilliseconds, methodArguments);                
            }
        }

        internal DialerErrorCode Execute(int companyId, string methodName, string methodArguments, Func<long, DialerErrorCode> methodBody)
        {
            return Execute<DialerErrorCode>(companyId, methodName, methodArguments, methodBody);
        }

        internal T DoDialerCall<T>(Func<T> delegatedCall, long requestId)
        {
            var timer = Stopwatch.StartNew();

            Logger.Info(delegatedCall.Method.Name, "DoDialerCall [{0}]", requestId);

            var resultString = string.Empty;

            try
            {
                T result = delegatedCall();
                resultString = string.Format("{0}", result);

                return result;
            }
            catch (Exception ex)
            {
                // All exception ditails are logged "upper" as the result of re-throwing. Here we note the exception type name only.
                resultString = ex.GetType() + " (re-thrown)";

                throw;
            }
            finally
            {
                timer.Stop();

                Logger.Info(
                    delegatedCall.Method.Name,
                    "Eof DoDialerCall [{0}]. Result: {1}. Duration: {2}.",
                    requestId, resultString, timer.ElapsedMilliseconds);
            }
        }

        int ChooseDialerId(int dialerId)
        {
            return (ServiceState.dialerId == 0) ? dialerId : ServiceState.dialerId;
        }

        int ChooseCompanyId(int companyId)
        {
            return (ServiceState.companyId == 0) ? companyId : ServiceState.companyId;
        }

        public string GetName()
        {
            return Execute(0,
                "DialerService.GetName",
                "",
                requestId => DoDialerCall(
                    () => _bridgeToDialer.GetName(), requestId));
        }

        public string GetVersion()
        {
            return Execute(0,
                "DialerService.GetVersion",
                "",
                requestId => DoDialerCall(
                    () => _bridgeToDialer.GetVersion(), requestId));
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {            
            var argumentsAsString = string.Format("companyId={0}, dialerId={1}, configurationParametersXml={2}",
                                                 companyId, dialerId, configurationParametersXml);

            return Execute(companyId,
                "DialerService.Initialize",
                argumentsAsString,
                requestId =>
                {
                    if (Settings.Default.StatefulMode)
                    {
                        if (ServiceState.dialerId == 0)
                        {
                            // It means that there is no hard configured value
                            ServiceState.dialerId = dialerId;
                        }

                        ServiceState.companyId = companyId;
                    }

                    if ((Settings.Default.DialerId != 0) && (dialerId != Settings.Default.DialerId))
                    {
                        throw new ArgumentException(
                            string.Format("DialerId ({0}) differs from the one in web.config ({1}).", dialerId, ServiceState.dialerId));
                    }

                    return DoDialerCall(
                        () => _bridgeToDialer.Initialize(companyId, dialerId, configurationParametersXml), requestId);
                });
        }

        public DialerErrorCode Release()
        {
            return Execute(0,
                "DialerService.Release",
                "",
                requestId => DoDialerCall(
                    () => _bridgeToDialer.Release(), requestId));
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            var argumentsAsString = string.Format("companyId={0}, configurationParametersXml={1}", companyId, configurationParametersXml);

            return Execute(companyId,
                "DialerService.SetConfigurationParameters",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.SetConfigurationParameters(companyId, configurationParametersXml), requestId));
        }

        public DialerState GetState(int companyId)
        {
            var argumentsAsString = string.Format("companyId={0}", companyId);

            return Execute(companyId,
                "DialerService.GetState",
                argumentsAsString,
                requestId =>
                {
                    var state = DoDialerCall(() => _bridgeToDialer.GetState(companyId), requestId);

                    //TODO: Now we send back notification from this point, not from inside of the dialer driver dll,
                    // but maybe it would be better to say all dialer providers that they should call NotifyDialerState themselves,
                    // it would guarantee that all the notification chain from dialer driver to CATI works. 
                    // I am still not sure that it's required. But if we decide to implement it it would also be great to have 
                    // a special setting: if dialer sends notification we would not send them. If dialer does not send 
                    // then we would send it ourselves.
                    NotifyDialerState(companyId, ServiceState.dialerId, DialerState.Available);

                    return state;
                });
        }

        public DialerErrorCode StartCampaign(int companyId, long campaignId, string campaignName, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, dialingMode={2}, recordWholeInterview={3}, campaignName={4}, campaignParametersXml={5}",
                companyId, campaignId, dialingMode, recordWholeInterview, campaignName, campaignParametersXml);

            return Execute(companyId,
                "DialerService.StartCampaign",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.StartCampaign(
                        companyId, campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml),
                    requestId));
        }

        public DialerErrorCode StopCampaign(int companyId, long campaignId, DialingMode dialingMode)
        {
            var argumentsAsString = string.Format("companyId={0}, campaignId={1}, dialingMode={2}", companyId, campaignId, dialingMode);

            return Execute(companyId,
                "DialerService.StopCampaign",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.StopCampaign(companyId, campaignId, dialingMode), requestId));
        }

        public DialerErrorCode KillCampaign(int companyId, long campaignId, DialingMode dialingMode)
        {
            var argumentsAsString = string.Format("companyId={0}, campaignId={1}, dialingMode={2}", companyId, campaignId, dialingMode);

            return Execute(companyId,
                "DialerService.KillCampaign",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.KillCampaign(companyId, campaignId, dialingMode), requestId));
        }

        public DialerErrorCode SetCampaignParameters(int companyId, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, dialingMode={2}, recordWholeInterview={3}, campaignParametersXml={4}",
                companyId, campaignId, dialingMode, recordWholeInterview, campaignParametersXml);

            return Execute(companyId,
                "DialerService.SetCampaignParameters",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.SetCampaignParameters(
                        companyId, campaignId, dialingMode, recordWholeInterview, campaignParametersXml),
                    requestId));
        }

        public DialerErrorCode Login(
            int companyId,
            long campaignId,
            int agentId,
            string agentName,
            string agentConnectionString,
            bool isPredictive,
            ResourceBindingType resourceBindingType,
            IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, agentName={3}, agentConnectionString={4}, isPredictive={5}, resourceBindingType={6}, agentAttributes={7}",
                companyId, campaignId, agentId, agentName, agentConnectionString, isPredictive, resourceBindingType,
                agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString()));

            return Execute(companyId,
                "DialerService.Login",
                argumentsAsString,
                requestId => DoDialerCall(
                        () => _bridgeToDialer.Login(
                        companyId, campaignId, agentId, agentName, agentConnectionString, resourceBindingType, isPredictive, agentAttributes),
                    requestId));
        }

        public DialerErrorCode SetCampaign(int companyId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}",
                companyId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.SetCampaign",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.SetCampaign(companyId, campaignId, agentId), requestId));
        }

        public DialerErrorCode Logout(int companyId, long campaignId, int agentId, bool isPredictive)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, isPredictive={3}",
                companyId, campaignId, agentId, isPredictive);

            return Execute(companyId,
                "DialerService.Logout",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.Logout(companyId, campaignId, agentId, isPredictive), requestId));
        }

        public DialerErrorCode KillAgent(int companyId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}",
                companyId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.KillAgent",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.KillAgent(companyId, campaignId, agentId), requestId));
        }

        public DialerErrorCode GoReady(int companyId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}",
                companyId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.GoReady",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.GoReady(companyId, campaignId, agentId), requestId));
        }

        public DialerErrorCode GoNotReady(int companyId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}",
                companyId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.GoNotReady",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.GoNotReady(companyId, campaignId, agentId), requestId));
        }

        public DialerErrorCode SetGroups(int companyId, long campaignId, int agentId, int[] agentGroups)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, agentGroups=[{3}]",
                companyId, campaignId, agentId, string.Join(",", agentGroups));

            return Execute(companyId,
                "DialerService.SetGroups",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.SetGroups(companyId, campaignId, agentId, agentGroups), requestId));
        }

        public DialerErrorCode SendNumberToAgent(int companyId, long campaignId, int agentId, DialingMode diallingMode, int interviewId, long callId, string phoneNumber, bool isRecording)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, diallingMode={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}",
                companyId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording);

            return Execute(companyId,
                "DialerService.SendNumberToAgent",
                argumentsAsString,
                requestId => DoDialerCall(
                        () => _bridgeToDialer.SendNumberToAgent(
                        companyId, campaignId, agentId, diallingMode, interviewId, callId, phoneNumber, isRecording),
                    requestId));
        }

        public DialerErrorCode Redial(int companyId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, phoneNumber={5}, isRecording={6}",
                companyId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording);

            return Execute(companyId,
                "DialerService.Redial",
                argumentsAsString,
                requestId => DoDialerCall(
                        () => _bridgeToDialer.Redial(
                        companyId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording),
                    requestId));
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, long campaignId, DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout)
        {
            var argumentsAsString = string.Format(
                "requestId={0}, companyId={1}, campaignId={2}, campaignDialingMode={3}, " +
                "callAgingTimeout={4},  numberOfCalls={5}, callList=({6})",
                requestId, companyId, campaignId, campaignDialingMode,
                callAgingTimeout, callList.Count, string.Join(", ", callList));

            return Execute(companyId,
                "DialerService.SendNumbers",
                argumentsAsString,
                reqId => DoDialerCall(
                    () => _bridgeToDialer.SendNumbers(requestId, companyId, campaignId, campaignDialingMode, callList, callAgingTimeout), reqId));
        }

        public DialerErrorCode Hangup(int companyId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}",
                companyId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.Hangup",
                argumentsAsString,
                requestId =>
                {
                    var result = DoDialerCall(
                        () => _bridgeToDialer.Hangup(companyId, campaignId, agentId), requestId);

                    if (result == DialerErrorCode.WrongStateAgentNotInCall)
                    {
                        Logger.Warning(
                            "DialerService.Hangup",
                            "[{0}] DialerErrorCode.WrongStateAgentNotInCall is replaced with DialerErrorCode.Success /// {1}",
                            requestId, argumentsAsString);
                        result = DialerErrorCode.Success;
                    }

                    return result;
                });
        }

        public DialerErrorCode CompleteCall(int companyId, long campaignId, int agentId, bool makeAgentReady)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, makeAgentReady={3}",
                companyId, campaignId, agentId, makeAgentReady);

            return Execute(companyId,
                "DialerService.CompleteCall",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.CompleteCall(companyId, campaignId, agentId, makeAgentReady), requestId));
        }

        public DialerErrorCode UpdateInterviewStatus(
            int companyId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            InterviewStatus interviewStatus)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, interviewStatus={5}",
                companyId, campaignId, agentId, interviewId, callId, interviewStatus);

            return Execute(companyId,
                "DialerService.UpdateInterviewStatus",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.UpdateInterviewStatus(companyId, campaignId, agentId, interviewId, callId, interviewStatus), requestId));
        }

        public DialerErrorCode CompletePreview(int companyId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, phoneNumber={5}, isRecording={6}",
                companyId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording);

            return Execute(companyId,
                "DialerService.CompletePreview",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.CompletePreview(companyId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording), requestId));
        }

        public DialerErrorCode FlushNumbers(int companyId, long campaignId, List<CallInfo> callList)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, numberOfCalls={2}",
                companyId, campaignId, callList.Count);

            return Execute(companyId,
                "DialerService.FlushNumbers",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.FlushNumbers(companyId, campaignId, callList), requestId));
        }

        public DialerErrorCode StartRecording(int companyId, long campaignId, int agentId, int interviewId, long callId, string label)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, label={5}",
                companyId, campaignId, agentId, interviewId, callId, label);

            return Execute(companyId,
                "DialerService.StartRecording",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.StartRecording(companyId, campaignId, agentId, interviewId, callId, label), requestId));
        }

        public DialerErrorCode StopRecording(int companyId, long campaignId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, stopRecordingMode={5}",
                companyId, campaignId, agentId, interviewId, callId, stopRecordingMode);

            return Execute(companyId,
                "DialerService.StopRecording",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.StopRecording(companyId, campaignId, agentId, interviewId, callId, stopRecordingMode), requestId));
        }

        public DialerErrorCode StartPlayback(int companyId, long campaignId, int agentId, int interviewId, long callId, string fileName, out int timeOfPlayingInSeconds)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, fileName={5}",
                companyId, campaignId, agentId, interviewId, callId, fileName);

            var internalTimeOfPlayingInSeconds = 0;

            var result = Execute(companyId,
                "DialerService.StartPlayback",
                argumentsAsString,
                requestId =>
                {
                    var startPlaybackResult = DoDialerCall(
                        () => _bridgeToDialer.StartPlayback(
                            companyId, campaignId, agentId, interviewId, callId, fileName, out internalTimeOfPlayingInSeconds),
                        requestId);

                    Logger.Verbose("DialerService.StartPlayback",
                        "timeOfPlayingInSeconds returned is [{0}] /// startPlaybackResult={1}, " +
                        "companyId={2}, campaignId={3}, agentId={4}, interviewId={5}, callId={6}, fileName={7}",
                        internalTimeOfPlayingInSeconds, startPlaybackResult,
                        companyId, campaignId, agentId, interviewId, callId, fileName);

                    return startPlaybackResult;
                });

            timeOfPlayingInSeconds = internalTimeOfPlayingInSeconds;

            return result;
        }

        public DialerErrorCode StopPlayback(int companyId, long campaignId, int agentId, long callId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, callId={3}",
                companyId, campaignId, agentId, callId);

            return Execute(companyId,
                "DialerService.StopPlayback",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.StopPlayback(companyId, campaignId, agentId, callId), requestId));
        }

        public DialerErrorCode PauseOrResumePlayback(int companyId, long campaignId, int agentId, long callId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, callId={3}",
                companyId, campaignId, agentId, callId);

            return Execute(companyId,
                "DialerService.PauseOrResumePlayback",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.PauseOrResumePlayback(companyId, campaignId, agentId, callId), requestId));
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, long campaignId, int agentId, long callId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, callId={3}",
                companyId, campaignId, agentId, callId);

            return Execute(companyId,
                "DialerService.ToggleInterviewerListensToPlaybackOrRespondent",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.ToggleInterviewerListensToPlaybackOrRespondent(companyId, campaignId, agentId, callId), requestId));
        }

        public DialerErrorCode StartMonitor(
            int companyId,
            int agentId,
            string supervisorName, 
            string supervisorConnectionString,
            ResourceBindingType resourceBindingType,
            ref string sessionId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, agentId={1}, supervisorName={2}, supervisorConnectionString={3}, resourceBindingType={4}, sessionId={5}",
                companyId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, sessionId);

            var internalSessionId = sessionId;

            if (_supervisorResourceBindingTypeSetting.IsSet)
            {
                // Override the value with the one from config file
                resourceBindingType = _supervisorResourceBindingTypeSetting.Get();
            }

            var result = Execute(companyId,
                "DialerService.StartMonitor",
                argumentsAsString,
                requestId =>
                {
                    var startMonitorResult = DoDialerCall(
                        () =>_bridgeToDialer.StartMonitor(
                            companyId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, ref internalSessionId),
                        requestId);

                    Logger.Verbose(
                        "DialerService.StartMonitor",
                        "Monitoring sessionId returned is [{0}] /// startMonitorResult={1}, " +
                        "companyId={2}, agentId={3}, supervisorName={4}, supervisorConnectionString={5}, resourceBindingType={6}",
                        internalSessionId, startMonitorResult,
                        companyId, agentId, supervisorName, supervisorConnectionString, resourceBindingType);

                    return startMonitorResult;
                });

            sessionId = internalSessionId;

            return result;
        }

        public DialerErrorCode StopMonitor(int companyId, string sessionId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, sessionId={1}",
                companyId, sessionId);

            return Execute(companyId,
                "DialerService.StopMonitor",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.StopMonitor(companyId, sessionId), requestId));
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            var argumentsAsString = string.Format("companyId={0}", companyId);

            IEnumerable<TrunkLineStateAndAlarms> internalTrunkLineStatesAndAlarms = null;

            var result = Execute(companyId,
                "DialerService.GetTrunkLineStatesAndAlarms",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.GetTrunkLineStatesAndAlarms(companyId, out internalTrunkLineStatesAndAlarms), requestId));

            trunkLineStatesAndAlarms = internalTrunkLineStatesAndAlarms;

            return result;
        }

        public DialerErrorCode TransferToIvr(
            int companyId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, endpoint={5}, attrubutes={6}",
                companyId, campaignId, agentId, interviewId, callId, endpoint, string.Join(", ", attributes));

            return Execute(companyId,
                "DialerService.TransferToIvr",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialer.TransferToIvr(
                        companyId, campaignId, agentId, interviewId, callId, endpoint, attributes),
                    requestId));
        }

        public void InitializeRecording()
        {            
            const string argumentsAsString = ""; // There is no arguments

            Execute(0,
                "DialerService.InitializeRecording",
                argumentsAsString,
                requestId => DoDialerCall(() =>
            {
                _bridgeToDialerRecording.InitializeRecording();
                        return DialerErrorCode.Success; // Execute and DoDialerCall need a return value;
            }
                    , requestId));
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId)
        {            
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, interviewId={2}",
                companyId, campaignId, interviewId);

            return Execute(0,
                "DialerService.GetAudioRecords",
                argumentsAsString,
                requestId => DoDialerCall(() =>
                    {
                        var result = _bridgeToDialerRecording.GetAudioRecords(companyId, campaignId, interviewId);

                        var resultAsStrings = result.Select(AudioRecordInfoToString).ToList();

                        Logger.Verbose(
                            "DialerService.GetAudioRecords",
                            "companyId={0}, campaignId={1}, Result is [{2}]({3})",
                            companyId, campaignId, resultAsStrings.Count, string.Join(", ", resultAsStrings));

                        return result;
                    }
                    , requestId));
        }

        private string AudioRecordInfoToString(AudioRecordInfo audioRecordInfo)
        {
            return string.Format("audioRecordInfo[{0}, {1}]", audioRecordInfo.DateTime, audioRecordInfo.Url);
        }

        //TODO: We don't use bulk request anymore
        public BulkAudioResult GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIdentities)
        {            
            var argumentsAsString = string.Format(
                "companyId={0}, interviewCount={1}",
                companyId, interviewIdentities.Count());

            return Execute(0,
                "DialerService.GetBulkAudioRecords",
                argumentsAsString,
                requestId => DoDialerCall(
                    () => _bridgeToDialerRecording.GetBulkAudioRecords(companyId, interviewIdentities), requestId));
        }

        public bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds)
        {            
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, interviewIds={2}",
                companyId, campaignId, string.Join(", ", interviewIds.Select(s => s.ToString())));

            return Execute(0,
                "DialerService.InitializeRecording",
                argumentsAsString,
                requestId => DoDialerCall(() =>
                    {
                        var result = _bridgeToDialerRecording.AreRecordsExists(companyId, campaignId, interviewIds);

                        if (result.Length != interviewIds.Length)
                        {
                            Logger.Error(
                                "DialerService.AreRecordsExists",
                                string.Format("Result array length is invalid. Actual: {0}. Expected: {1}.",
                                result.Length, interviewIds.Length));
                        }

                        var length = (result.Length < interviewIds.Length) ? result.Length : interviewIds.Length;

                        var resultAsStrings = interviewIds.Take(length).Select((id, index) => string.Format("{0}: {1}", id, result[index]));

                        Logger.Verbose(
                            "DialerService.AreRecordsExists",
                            "companyId={0}, campaignId={1}, Result is [{2}]({3})",
                            companyId, campaignId, length, string.Join(", ", resultAsStrings));

                        return result;
                    }
                    , requestId));
        }

        private void TryToSendEventNotification(
            DialerEvent dialerEvent,
            int companyId,
            int dialerId,
            bool isSyncronous = false)
        {
            try
            {
                // GetSender throws exception in case of (companyId == 0)
                var notificationSender = _notificationSenders.GetSender(companyId, dialerId);

                if (isSyncronous)
                {
                    notificationSender.SendEventNotificationSynchronously(dialerEvent);
                }
                else
                {
                    notificationSender.SendEventNotification(dialerEvent);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DialerService.TryToSendEventNotification", "{0} /// {1}", ex.ToString(), dialerEvent);
            }
        }

        /// <summary>
        /// Notifies Confirmit CATI about the dialer state
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="dialerState">Dialer state</param>
        public void NotifyDialerState(
            int companyId, 
            int dialerId, 
            DialerState dialerState)
        {            
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyDialerState",
                "companyId={0}/{1}, dialerId={2}/{3}, dialerState={4}({5})",
                companyId, chosenCompanyId, dialerId, chosenDialerId, dialerState, (int)dialerState);

            var dialerEvent = new DialerEventNotifyDialerState(
                DialerEventPriority.LowPriority,
                dialerState,
                chosenCompanyId);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId, true);
        }

        /// <summary>
        /// Notifies Confirmit CATI abot the agent state.
        /// Confirmit CATI (asynchronously) waits for this event after calling IDialerCoreApi.Login. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="agentState">Current agent state</param>
        public void NotifyAgentState(
            int companyId, 
            int dialerId, 
            long campaignId, 
            int agentId, 
            AgentState agentState)
        {            
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyAgentState",
                "companyId={0}/{1}, dialerId={2}/{3}, campaignId={4}, agentId={5}, agentState={6}({7})",
                companyId, chosenCompanyId, dialerId, chosenDialerId, campaignId, agentId, agentState, (int)agentState);

            if (!_allowedAgentStates.Contains(agentState))
            {
                Logger.Verbose("DialerService.NotifyAgentState",
                    "agentState={0}({1}) is filtered and will not be sent CATI server.",
                    agentState, (int)agentState);
                return;
            }
            
            var dialerEvent = new DialerEventNotifyUserState(
                DialerEventPriority.LowPriority,
                chosenCompanyId,
                campaignId,
                agentId,
                agentState);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        /// <summary>
        /// Notifies Confirmit CATI about the call outcome.
        /// Confirmit CATI (asynchronously) waits for this event after calling the next methods:
        /// - IDialerCoreApi.SendNumberToAgent. 
        /// - IDialerCoreApi.SendNumbers
        /// - IDialerCoreApi.CompletePreview
        /// - IDialerCoreApi.FlushNumbers
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="outcome">The call outcome</param>
        /// <param name="dialerAccompanyingCallInfo">Some accompanying info received from dialer</param>
        public void NotifyOutcome(
            int companyId, 
            int dialerId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            CallOutcome outcome, 
            string dialerAccompanyingCallInfo)
        {            
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyOutcome",
                "companyId={0}/{1}, dialerId={2}/{3}, campaignId={4}, agentId={5}, " +
                "interviewId={6}, callId={7}, outcome={8}({9}), dialerAccompanyingCallInfo='{10}'",
                companyId, chosenCompanyId, dialerId, chosenDialerId, campaignId, agentId,
                interviewId, callId, outcome, (int)outcome, dialerAccompanyingCallInfo);

            var dialerEvent = new DialerEventNotifyOutcome(
                (outcome == CallOutcome.Connected) ? DialerEventPriority.HighPriority : DialerEventPriority.LowPriority,
                chosenCompanyId,
                campaignId,
                agentId,
                callId,
                (int)outcome,
                dialerAccompanyingCallInfo);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="callDialingMode">The call dialing mode</param>
        public void ScreenPop(
            int companyId, 
            int dialerId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            DialingMode callDialingMode)
        {            
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.ScreenPop",
                "companyId={0}/{1}, dialerId={2}/{3}, campaignId={4}, agentId={5}, " +
                "interviewId={6}, callId={7}, callDialingMode={8}({9})",
                companyId, chosenCompanyId, dialerId, chosenDialerId, campaignId, agentId,
                interviewId, callId, callDialingMode, (int)callDialingMode);

            var dialerEvent = new DialerEventScreenPop(
                DialerEventPriority.LowPriority,
                chosenCompanyId,
                campaignId,
                agentId,
                callId);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        /// <summary>
        /// Dialer requests calls for predicive dialing
        /// </summary>
        /// <param name="requestId"> </param>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="groupId">Identifier of the agent group which dialer requests calls for</param>
        /// <param name="callsSelectionAlgorithm">Confirmit CATI can select calls using two selection algorithms: 'by campaign' or 'by agent group'</param>
        /// <param name="callCount">Amount of calls the dialer requests for</param>
        public void RequestCalls(
            string requestId, 
            int companyId, 
            int dialerId, 
            long campaignId, 
            int groupId, 
            CallsSelectionAlgorithm callsSelectionAlgorithm, 
            int callCount)
        {                        
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.RequestCalls",
                "requestId='{0}', companyId={1}/{2}, dialerId={3}/{4}, campaignId={5}, " +
                "groupId={6}, callsSelectionAlgorithm={7}({8}), callCount={9}",
                requestId, companyId, chosenCompanyId, dialerId, chosenDialerId, campaignId,
                groupId, callsSelectionAlgorithm, (int)callsSelectionAlgorithm, callCount);

            var dialerEvent = new DialerEventRequestCalls(
                DialerEventPriority.LowPriority,
                requestId,
                chosenCompanyId,
                campaignId,
                groupId,
                callsSelectionAlgorithm,
                callCount);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        private static string GetDialerDriverStateFullFilename()
        {
            return string.Format("{0}{1}.xml", DialerServiceAppDataPath.GetServiceAppDataPath(), Settings.Default.DialerDriverAssemblyName);
        }
    }
}
