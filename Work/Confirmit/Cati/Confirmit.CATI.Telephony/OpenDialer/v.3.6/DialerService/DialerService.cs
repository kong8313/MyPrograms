using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Web.Hosting;
using Confirmit.CATI.Common.Logging;
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
        private RequestId _requestId;
        public ICommonLogger Logger;

        private readonly IDialerCoreApi _bridgeToDialer;
        private readonly IDialerRecordingApi _bridgeToDialerRecording;
        private readonly DialerEventNotificationSenders _notificationSenders;

        private const string DialerInterfaceNamePattern = "DialerInterface";

        public DialerServiceState ServiceState;

        private readonly RequestCounter _requestCounter = new RequestCounter();

        private readonly DialerWsRequestsAuthoriser _dialerWsRequestsAuthoriser;

        //TODO: Refactor this - move to a separate class
        private readonly HashSet<AgentState> _allowedAgentStates;
        public static readonly AgentState[] MandatoryAgentStates = { AgentState.LoggedIn, AgentState.LoggedOut, AgentState.NotReady };

        private readonly SupervisorResourceBindingTypeSetting _supervisorResourceBindingTypeSetting;

        public DialerService() : this(
            Settings.Default.DialerDriverAssemblyNamespace,
            Settings.Default.DialerDriverAssemblyMainClassName,
            Settings.Default.DialerDriverAssemblyName,
            null)
        {
            
        }

        public DialerService(string dialerDriverAssemblyNamespace,
            string dialerDriverAssemblyMainClassName,
            string dialerDriverAssemblyName,
            INotificationsSenderInitializer notificationsSenderInitializer)
        {
            _requestId = new RequestId();
            Logger = new Logger("DialerService", _requestId);

            if (notificationsSenderInitializer == null)
            {
                notificationsSenderInitializer = new NotificationsSenderInitializer(Logger);
            }

            Logger.InitReportingWsTraceListener();

            Logger.Info("DialerService.ctor",
                "DialerService object is created. Settings: [StatefulMode={0}, DialerId={1}, " +
                "UseAuthorization={2}, ServiceStateExpirationTimeout={3}]",
                Settings.Default.StatefulMode, Settings.Default.DialerId,
                Settings.Default.UseAuthorization, Settings.Default.ServiceStateExpirationTimeout);

            ConfigureSecurityProtocols();

            LogCodiVersion();

            var dialerAssembly = string.Format("{0}.{1}, {2}",
                dialerDriverAssemblyNamespace,
                dialerDriverAssemblyMainClassName,
                dialerDriverAssemblyName);

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

                var codiAssemblyInfo = "Unknown (unable to get the info)";

                try
                {
                    var assembly = Assembly.ReflectionOnlyLoad(dialerDriverAssemblyName);
                    codiAssemblyInfo = GetDialerInterfaceAssemblyNameFromList(assembly.GetReferencedAssemblies()).ToString();
                }
                catch (Exception)
                {
                    // There is no need to log the exception as the initial exception is already logged above
                }

                Logger.Info("DialerService.ctor", "Dialer driver assembly is built against CODI: [{0}]", codiAssemblyInfo);

                // Re-throw the original exception
                throw;
            }

            _notificationSenders = new DialerEventNotificationSenders(notificationsSenderInitializer);

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

            LogDialerDriverDllInfo();

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

        private void ConfigureSecurityProtocols()
        {
            Logger.Info("DialerService.ConfigureSecurityProtocols",
                "Default host security protocols: [{0}]", ServicePointManager.SecurityProtocol);

            var securityProtocolsFromSettings = Settings.Default.SecurityProtocols;

            if ((securityProtocolsFromSettings == null) || (securityProtocolsFromSettings.Count == 0))
            {
                Logger.Info("DialerService.ConfigureSecurityProtocols",
                    "'SecurityProtocols' section in web.config is empty or does not exist.");
            }
            else
            {
                Logger.Info("DialerService.ConfigureSecurityProtocols",
                    "'SecurityProtocols' section in web.config: [{0}]",
                    string.Join(", ", securityProtocolsFromSettings.Cast<string>()));

                var resultProtocols = new List<SecurityProtocolType>();

                foreach (var protocolType in securityProtocolsFromSettings)
                {
                    SecurityProtocolType securityProtocolType;

                    if (Enum.TryParse(protocolType, true, out securityProtocolType))
                    {
                        resultProtocols.Add(securityProtocolType);
                    }
                    else
                    {
                        Logger.Error(
                            "DialerService.ConfigureSecurityProtocols",
                            "Unknown SecurityProtocolType [{0}] in 'SecurityProtocols' section of web.config." +
                            "The following values are allowed: {1}.",
                            protocolType,
                            string.Join(", ", Enum.GetNames(typeof(SecurityProtocolType))));
                    }
                }

                if (resultProtocols.Count > 0)
                {
                    ServicePointManager.SecurityProtocol =
                        resultProtocols.Aggregate((result, x) => result | x);
                }
                else
                {
                    Logger.Warning(
                        "DialerService.ConfigureSecurityProtocols",
                        "There are no known security protocols in 'SecurityProtocols' section of web.config." +
                        "Default host security protocols will be used.");
                }
            }

            Logger.Info(
                "DialerService.ConfigureSecurityProtocols",
                "Effective security protocols: [{0}]", ServicePointManager.SecurityProtocol);
        }

        /// <summary>
        /// Note, the method is expected to be called from the constructor only. That's why it logs as "DialerService.ctor"
        /// </summary>
        private void LogCodiVersion()
        {
            try
            {
                var codiVersionInfo = GetDialerInterfaceAssemblyVersion();
                Logger.Info("DialerService.ctor", "CODI assembly's version: [{0}]", string.Join("/", codiVersionInfo));
            }
            catch (Exception ex)
            {
                Logger.Error("DialerService.ctor", "Unable to get CODI assembly's version. /// {0}", ex);
            }
        }

        /// <summary>
        /// Note, the method is expected to be called from the constructor only. That's why it logs as "DialerService.ctor"
        /// </summary>
        private void LogDialerDriverDllInfo()
        {
            try
            {
                var dialerDriverDllInfo = DialerDriverDllInfo();
                Logger.Info("DialerService.ctor", "Dialer Driver Dll Info: [{0}]", dialerDriverDllInfo);
            }
            catch (Exception ex)
            {
                Logger.Error("DialerService.ctor", "Unable to get Dialer Driver Dll Info. /// {0}", ex);
            }
        }

        /// <summary>
        /// This constructor is for testing needs only
        /// </summary>
        /// <param name="dialerCoreApiObject"></param>
        /// <param name="logger"></param>
        public DialerService(IDialerCoreApi dialerCoreApiObject, ICommonLogger logger)
        {
            _requestId = new RequestId();

            _bridgeToDialer = dialerCoreApiObject;
            Logger = logger;

            _dialerWsRequestsAuthoriser = new DialerWsRequestsAuthoriser("", false);

            _supervisorResourceBindingTypeSetting = new SupervisorResourceBindingTypeSetting(Logger);
        }

        //TODO: Refactor this - move to a separate class
        private HashSet<AgentState> GetAllowedAgentStatesFromConfig()
        {
            var allowedStates = Settings.Default.AllowedAgentStates;

            if ((allowedStates == null) || (allowedStates.Count == 0))
            {
                Logger.Warning(
                    "DialerService.GetAllowedAgentStatesFromConfig",
                    "'AllowedAgentStates' section in web.config is empty or does not exist. " +
                    "{0} are allowed by default." +
                    "The following agent states are possible: {1}.",
                    string.Join(", ", MandatoryAgentStates),
                    string.Join(", ", Enum.GetNames(typeof(AgentState))));

                return new HashSet<AgentState>(MandatoryAgentStates);
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

            foreach (var state in MandatoryAgentStates)
            {
                if (!result.Contains(state))
                {
                    Logger.Warning(
                        "DialerService.GetAllowedAgentStatesFromConfig",
                        "[{0}] agent state will be allowed automatically as it's a mandatory state.",
                        state);

                    result.Add(state);
                }
            }

            Logger.Info(
                "DialerService.GetAllowedAgentStatesFromConfig",
                "The next agent states are allowed: {0}.", string.Join(", ", result));
            
            return result;
        }

        private void RestoreServiceState()
        {
            bool readSuccess = DialerServiceState.Load(ref ServiceState, Logger);

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
                         () => DoDialerCall(
                            () => _bridgeToDialer.RestoreDialerDriverState(ServiceState.companyId, GetDialerDriverStateFullFilename())), 
                            false);

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
            Logger.Info("DialerService.Dispose",
                "ShutdownReason: [{0}]. Start disposing... /// {1}",
                HostingEnvironment.ShutdownReason, new StackTrace(true));

            ServiceState.Save(Logger);

            Execute(0,
                "DialerService.Dispose",
                "",
                () => DoDialerCall(() => _bridgeToDialer.SaveDialerDriverState(GetDialerDriverStateFullFilename())), false);

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

        internal T Execute<T>(int companyId, string methodName, string methodArguments, Func<T> methodBody, Func<T, string> getResultString, bool isIncrementRequestId = true)
        {
            if (isIncrementRequestId)
                _requestId.Next();

            var timer = Stopwatch.StartNew();

            Logger.Info(
                methodName,
                "Execute [{0}] /// {1}", _requestCounter.Increment(), methodArguments);

            T result = default(T);

            var resultString = string.Empty;

            try
            {
                //TODO: To think how to handle DialerWsInvalidCredentialsException
                _dialerWsRequestsAuthoriser.AuthoriseRequest();

                result = methodBody();
                resultString = getResultString(result);

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
                Logger.Error(companyId, string.Format("DialerService.{0}", methodName), "{0}", ex.ToString());
                resultString = ex.GetType() + " (re-thrown as FaultException)";

                throw new DialerExceptionToFaultException().Convert(ex);
            }
            finally
            {
                timer.Stop();

                Logger.Info(
                    methodName,
                    "Eof [{0}]. Result: {1}. Duration: {2} /// {3}",
                    _requestCounter.Decrement(), resultString, timer.ElapsedMilliseconds, methodArguments);
            }
        }

        internal T Execute<T>(int companyId, string methodName, string methodArguments, Func<T> methodBody, bool isIncrementRequestId = true)
        {
            return Execute(companyId, methodName, methodArguments, methodBody, result => result.ToString(), isIncrementRequestId);
        }

        internal T DoDialerCall<T>(Func<T> delegatedCall)
        {
            var timer = Stopwatch.StartNew();

            Logger.Info(delegatedCall.Method.Name, "DoDialerCall");

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
                    "Eof DoDialerCall. Result: {0}. Duration: {1}.",
                     resultString, timer.ElapsedMilliseconds);
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
                () => DoDialerCall(
                    () => _bridgeToDialer.GetName()));
        }

        public string GetVersion()
        {
            return Execute(0,
                "DialerService.GetVersion",
                "",
                () => DoDialerCall(
                    () => _bridgeToDialer.GetVersion()));
        }

        public string[] Version()
        {
            return Execute(0,
                "DialerService.Version",
                "",
                () =>
                {
                    return BuildVersionInfoArray();
                },
                versionStrings =>
                    string.Format("[{0}]", string.Join(", ", versionStrings.Select(x => x.ToString(CultureInfo.InvariantCulture)))));
        }

        private string[] BuildVersionInfoArray()
        {
            var result = GetDialerInterfaceAssemblyVersion();

            result.Add(DialerDriverDllInfo());

            return result.ToArray();
        }

        private string DialerDriverDllInfo()
        {
            var dialerDriverDllName = DoDialerCall(
                () => _bridgeToDialer.GetName());

            var dialerDriverDllVersion = DoDialerCall(
                () => _bridgeToDialer.GetVersion());

            return dialerDriverDllName + "#" + dialerDriverDllVersion;
        }

        public DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml)
        {            
            var argumentsAsString = string.Format("companyId={0}, dialerId={1}, configurationParametersXml={2}",
                                                 companyId, dialerId, configurationParametersXml);

            return Execute(companyId,
                "DialerService.Initialize",
                argumentsAsString,
                () =>
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
                        () => _bridgeToDialer.Initialize(companyId, dialerId, configurationParametersXml));
                });
        }

        public DialerErrorCode Release(int dialerId)
        {
            var argumentsAsString = string.Format("dialerId={0}", dialerId);

            return Execute(0,
                "DialerService.Release",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.Release(dialerId)));
        }

        public DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml)
        {
            var argumentsAsString = string.Format("companyId={0}, configurationParametersXml={1}", companyId, configurationParametersXml);

            return Execute(companyId,
                "DialerService.SetConfigurationParameters",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SetConfigurationParameters(companyId, configurationParametersXml)));
        }

        public DialerState GetState(int companyId, int dialerId)
        {
            var argumentsAsString = string.Format("companyId={0}, dialerId={1}", companyId, dialerId);

            return Execute(companyId,
                "DialerService.GetState",
                argumentsAsString,
                () =>
                {
                    var state = DoDialerCall(() => _bridgeToDialer.GetState(companyId, dialerId));

                    //TODO: Now we send back notification from this point, not from inside of the dialer driver dll,
                    // but maybe it would be better to say all dialer providers that they should call NotifyDialerState themselves,
                    // it would guarantee that all the notification chain from dialer driver to CATI works. 
                    // I am still not sure that it's required. But if we decide to implement it it would also be great to have 
                    // a special setting: if dialer sends notification we would not send them. If dialer does not send 
                    // then we would send it ourselves.
                    NotifyDialerState(companyId, dialerId, DialerState.Available);

                    return state;
                });
        }

        public DialerErrorCode StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}, recordWholeInterview={4}, campaignName={5}, campaignParametersXml={6}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode, recordWholeInterview, campaignName, campaignParametersXml);

            return Execute(companyId,
                "DialerService.StartCampaign",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.StartCampaign(
                        companyId, dialerIds, campaignId, campaignName, dialingMode, recordWholeInterview, campaignParametersXml)));
        }

        public DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            var argumentsAsString = string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode);

            return Execute(companyId,
                "DialerService.StopCampaign",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.StopCampaign(companyId, dialerIds, campaignId, dialingMode)));
        }

        public DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            var argumentsAsString = string.Format("companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode);

            return Execute(companyId,
                "DialerService.KillCampaign",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.KillCampaign(companyId, dialerIds, campaignId, dialingMode)));
        }

        public DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerIds=[{1}], campaignId={2}, dialingMode={3}, recordWholeInterview={4}, campaignParametersXml={5}",
                companyId, string.Join(", ", dialerIds), campaignId, dialingMode, recordWholeInterview, campaignParametersXml);

            return Execute(companyId,
                "DialerService.SetCampaignParameters",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SetCampaignParameters(
                        companyId, dialerIds, campaignId, dialingMode, recordWholeInterview, campaignParametersXml)));
        }

        public DialerErrorCode Login(int companyId, int dialerId, long campaignId, int agentId, string agentName, AgentType agentType, string agentConnectionString, bool isPredictive, ResourceBindingType resourceBindingType, IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, agentName={4}, agentType={5}, agentConnectionString={6}, isPredictive={7}, " +
                "resourceBindingType={8}, agentAttributes={9}",
                companyId, dialerId, campaignId, agentId, agentName, agentType, agentConnectionString, isPredictive,
                resourceBindingType, agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString()));

            return Execute(companyId,
                "DialerService.Login",
                argumentsAsString,
                () => DoDialerCall(
                        () => _bridgeToDialer.Login(
                        companyId, dialerId, campaignId, agentId, agentName, agentType, agentConnectionString, resourceBindingType, isPredictive, agentAttributes)));
        }

        public DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.SetCampaign",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SetCampaign(companyId, dialerId, campaignId, agentId)));
        }

        public DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, isPredictive={4}",
                companyId, dialerId, campaignId, agentId, isPredictive);

            return Execute(companyId,
                "DialerService.Logout",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.Logout(companyId, dialerId, campaignId, agentId, isPredictive)));
        }

        public DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.KillAgent",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.KillAgent(companyId, dialerId, campaignId, agentId)));
        }

        public DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.GoReady",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.GoReady(companyId, dialerId, campaignId, agentId)));
        }

        public DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, breakName={4}",
                companyId, dialerId, campaignId, agentId, breakName);

            return Execute(companyId,
                "DialerService.GoNotReady",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.GoNotReady(companyId, dialerId, campaignId, agentId, breakName)));
        }

        public DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, agentGroups=[{4}]",
                companyId, dialerId, campaignId, agentId, string.Join(",", agentGroups));

            return Execute(companyId,
                "DialerService.SetGroups",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SetGroups(companyId, dialerId, campaignId, agentId, agentGroups)));
        }

        public DialerErrorCode SendNumberToAgent(int companyId, int dialerId, long campaignId, int agentId, DialingMode dialingMode, int interviewId, long callId, string phoneNumber, bool isRecording, string callerId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, dialingMode={4}, interviewId={5}, callId={6}, phoneNumber={7}, isRecording={8}, callerId = {9}",
                companyId, dialerId, campaignId, agentId, dialingMode, interviewId, callId, phoneNumber, isRecording, callerId);

            return Execute(companyId,
                "DialerService.SendNumberToAgent",
                argumentsAsString,
                () => DoDialerCall(
                        () => _bridgeToDialer.SendNumberToAgent(
                        companyId, dialerId, campaignId, agentId, dialingMode, interviewId, callId, phoneNumber, isRecording, callerId)));
        }

        public DialerErrorCode Redial(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording, string callerId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}, callerId={8}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId);

            return Execute(companyId,
                "DialerService.Redial",
                argumentsAsString,
                () => DoDialerCall(
                        () => _bridgeToDialer.Redial(
                        companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording, callerId)));
        }

        public DialerErrorCode SendNumbers(string requestId, int companyId, int dialerId, long campaignId, DialingMode campaignDialingMode, List<CallInfo> callList, int callAgingTimeout)
        {
            var argumentsAsString = string.Format(
                "requestId={0}, companyId={1}, dialerId={2}, campaignId={3}, campaignDialingMode={4}, " +
                "callAgingTimeout={5}, numberOfCalls={6}, callList=({7})",
                requestId, companyId, dialerId, campaignId, campaignDialingMode,
                callAgingTimeout, callList.Count, string.Join(", ", callList));

            return Execute(companyId,
                "DialerService.SendNumbers",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SendNumbers(requestId, companyId, dialerId, campaignId, campaignDialingMode, callList, callAgingTimeout)));
        }

        public DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}",
                companyId, dialerId, campaignId, agentId);

            return Execute(companyId,
                "DialerService.Hangup",
                argumentsAsString,
                () =>
                {
                    var result = DoDialerCall(
                        () => _bridgeToDialer.Hangup(companyId, dialerId, campaignId, agentId));

                    if (result == DialerErrorCode.WrongStateAgentNotInCall)
                    {
                        Logger.Warning(
                            "DialerService.Hangup",
                            "DialerErrorCode.WrongStateAgentNotInCall is replaced with DialerErrorCode.Success /// {0}",
                            argumentsAsString);
                        result = DialerErrorCode.Success;
                    }

                    return result;
                });
        }

        public DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId, InterviewStatus interviewStatus, bool makeAgentReady, string breakName)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewStatus={4}, makeAgentReady={5}, breakName={6}",
                companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady, makeAgentReady? "NULL" : breakName);

            return Execute(companyId,
                "DialerService.CompleteCall",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.CompleteCall(companyId, dialerId, campaignId, agentId, interviewStatus, makeAgentReady, breakName)));
        }

        public DialerErrorCode SetNextInterview(int companyId, int dialerId, long currentCampaignId, int agentId,
            InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, currentCampaignId={2}, agentId={3}" +
                ", currentInterviewStatus={4}, nextCampaignId={5}, nextInterviewId={6}, nextCallId={7}",
                companyId, dialerId, currentCampaignId, agentId,
                currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);

            return Execute(companyId,
                "DialerService.SetNextInterview",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SetNextInterview(companyId, dialerId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId)));
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
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, interviewStatus={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, interviewStatus);

            return Execute(companyId,
                "DialerService.UpdateInterviewStatus",
                argumentsAsString,
                () =>
                {
                    Logger.Warning("DialerService.UpdateInterviewStatus", "UpdateInterviewStatus is obsolete");
                    return DialerErrorCode.Success;
                });
        }

        public DialerErrorCode CompletePreview(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string phoneNumber, bool isRecording)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, phoneNumber={6}, isRecording={7}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording);

            return Execute(companyId,
                "DialerService.CompletePreview",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.CompletePreview(companyId, dialerId, campaignId, agentId, interviewId, callId, phoneNumber, isRecording)));
        }

        public DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerIds=[{1}], campaignId={2}, numberOfCalls={3}",
                companyId, string.Join(", ", dialerIds), campaignId, callList.Count);

            return Execute(companyId,
                "DialerService.FlushNumbers",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.FlushNumbers(companyId, dialerIds, campaignId, callList)));
        }

        public DialerErrorCode StartRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string label)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, label={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, label);

            return Execute(companyId,
                "DialerService.StartRecording",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.StartRecording(companyId, dialerId, campaignId, agentId, interviewId, callId, label)));
        }

        public DialerErrorCode StopRecording(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, StopRecordingMode stopRecordingMode)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, stopRecordingMode={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, stopRecordingMode);

            return Execute(companyId,
                "DialerService.StopRecording",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.StopRecording(companyId, dialerId, campaignId, agentId, interviewId, callId, stopRecordingMode)));
        }

        public DialerErrorCode StartPlayback(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, string fileName, out int timeOfPlayingInSeconds)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, fileName={6}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, fileName);

            var internalTimeOfPlayingInSeconds = 0;

            var result = Execute(companyId,
                "DialerService.StartPlayback",
                argumentsAsString,
                () =>
                {
                    var startPlaybackResult = DoDialerCall(
                        () => _bridgeToDialer.StartPlayback(
                            companyId, dialerId, campaignId, agentId, interviewId, callId, fileName, out internalTimeOfPlayingInSeconds));

                    Logger.Verbose("DialerService.StartPlayback",
                        "timeOfPlayingInSeconds returned is [{0}] /// startPlaybackResult={1}, " +
                        "companyId={2}, dialerId={3}, campaignId={4}, agentId={5}, interviewId={6}, callId={7}, fileName={8}",
                        internalTimeOfPlayingInSeconds, startPlaybackResult,
                        companyId, dialerId, campaignId, agentId, interviewId, callId, fileName);

                    return startPlaybackResult;
                });

            timeOfPlayingInSeconds = internalTimeOfPlayingInSeconds;

            return result;
        }

        public DialerErrorCode StopPlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, callId={4}",
                companyId, dialerId, campaignId, agentId, callId);

            return Execute(companyId,
                "DialerService.StopPlayback",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.StopPlayback(companyId, dialerId, campaignId, agentId, callId)));
        }

        public DialerErrorCode PauseOrResumePlayback(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, callId={4}",
                companyId, dialerId, campaignId, agentId, callId);

            return Execute(companyId,
                "DialerService.PauseOrResumePlayback",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.PauseOrResumePlayback(companyId, dialerId, campaignId, agentId, callId)));
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, callId={4}",
                companyId, dialerId, campaignId, agentId, callId);

            return Execute(companyId,
                "DialerService.ToggleInterviewerListensToPlaybackOrRespondent",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.ToggleInterviewerListensToPlaybackOrRespondent(companyId, dialerId, campaignId, agentId, callId)));
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
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, agentId={2}, supervisorName={3}, supervisorConnectionString={4}, resourceBindingType={5}, sessionId={6}",
                companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, sessionId);

            var internalSessionId = sessionId;

            if (_supervisorResourceBindingTypeSetting.IsSet)
            {
                // Override the value with the one from config file
                resourceBindingType = _supervisorResourceBindingTypeSetting.Get();
            }

            var result = Execute(companyId,
                "DialerService.StartMonitor",
                argumentsAsString,
                () =>
                {
                    var startMonitorResult = DoDialerCall(
                        () =>_bridgeToDialer.StartMonitor(
                            companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType, ref internalSessionId));

                    Logger.Verbose(
                        "DialerService.StartMonitor",
                        "Monitoring sessionId returned is [{0}] /// startMonitorResult={1}, " +
                        "companyId={2}, dialerId={3}, agentId={4}, supervisorName={5}, supervisorConnectionString={6}, resourceBindingType={7}",
                        internalSessionId, startMonitorResult,
                        companyId, dialerId, agentId, supervisorName, supervisorConnectionString, resourceBindingType);

                    return startMonitorResult;
                });

            sessionId = internalSessionId;

            return result;
        }

        public DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, sessionId={2}",
                companyId, dialerId, sessionId);

            return Execute(companyId,
                "DialerService.StopMonitor",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.StopMonitor(companyId, dialerId, sessionId)));
        }

        public DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode)
        {
            var argumentsAsString = $"companyId={companyId}, dialerId={dialerId}, sessionId={sessionId}, monitorMode={monitorMode}";

            return Execute(companyId,
                "DialerService.SetMonitorMode",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.SetMonitorMode(companyId, dialerId, sessionId, monitorMode)));
        }

        public DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            var argumentsAsString = string.Format("companyId={0}, dialerId={1}", companyId, dialerId);

            IEnumerable<TrunkLineStateAndAlarms> internalTrunkLineStatesAndAlarms = null;

            var result = Execute(companyId,
                "DialerService.GetTrunkLineStatesAndAlarms",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.GetTrunkLineStatesAndAlarms(companyId, dialerId, out internalTrunkLineStatesAndAlarms)));

            trunkLineStatesAndAlarms = internalTrunkLineStatesAndAlarms;

            return result;
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
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, endpoint={6}, attrubutes={7}",
                companyId, dialerId, campaignId, agentId, interviewId, callId, endpoint, string.Join(", ", attributes));

            return Execute(companyId,
                "DialerService.TransferToIvr",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.TransferToIvr(
                        companyId, dialerId, campaignId, agentId, interviewId, callId, endpoint, attributes)));
        }

        public DialerErrorCode IvrRenderVoiceXml(int companyId, int dialerId, long campaignId, int agentId, string voiceXml)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}, voiceXml={4}",
                companyId, dialerId, campaignId, agentId, voiceXml);

            return Execute(companyId,
                "DialerService.IvrRenderVoiceXml",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.IvrRenderVoiceXml(
                        companyId, dialerId, campaignId, agentId, voiceXml)));
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, inboundDdiNumbers=[{2}]",
                companyId, dialerId, string.Join<InboundDdiNumber>(", ", inboundDdiNumbers));

            return Execute(companyId,
                "DialerService.ConfigureInboundDdiNumbers",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.ConfigureInboundDdiNumbers(
                        companyId, dialerId, inboundDdiNumbers)));
        }

        public DialerErrorCode DropInboundCall(int companyId, int dialerId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, inboundCallId={2}, audioMessageDescriptor={3}",
                companyId, dialerId, inboundCallId, audioMessageDescriptor.NullableToString());

            return Execute(companyId,
                "DialerService.DropInboundCall",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.DropInboundCall(
                        companyId, dialerId, inboundCallId, audioMessageDescriptor)));
        }

        public DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, inboundCallId={3}, callInfo={4}," +
                " campaignIdsToBorrowAgentsFrom=[{5}], audioMessageDescriptor={6}",
                companyId, dialerId, campaignId, inboundCallId, callInfo,
                campaignIdsToBorrowAgentsFrom != null ? string.Join(", ", campaignIdsToBorrowAgentsFrom) : "<NULL>",
                audioMessageDescriptor.NullableToString());

            return Execute(companyId,
                "DialerService.ConnectInboundCall",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.ConnectInboundCall(
                        companyId, dialerId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor)));
        }

        public DialerErrorCode ConnectInboundCallToAgent(int companyId, int dialerId, long campaignId, string inboundCallId,
            CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, inboundCallId={3}, callInfo={4}, audioMessageDescriptor={5}",
                companyId, dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor.NullableToString());

            return Execute(companyId,
                "DialerService.ConnectInboundCallToAgent",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.ConnectInboundCallToAgent(
                        companyId, dialerId, campaignId, inboundCallId, callInfo, audioMessageDescriptor)));
        }

        public DialerErrorCode TransferStart(int companyId, int dialerId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}, agentId={4}, transferType={5}",
                companyId, dialerId, campaignId, transferId, agentId, transferType);

            return Execute(companyId,
                "DialerService.TransferStart",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.TransferStart(
                        companyId, dialerId, campaignId, transferId, agentId, transferType)));
        }

        public DialerErrorCode TransferSetTarget(int companyId, int dialerId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}, targetType={4}, targetResource={5}, borrowAgentsFromAllCampaigns={6}",
                companyId, dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);

            return Execute(companyId,
                "DialerService.TransferSetTarget",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.TransferSetTarget(
                        companyId, dialerId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns)));
        }

        public DialerErrorCode TransferSetConnectionState(int companyId, int dialerId, long campaignId,
            string transferId, ConnectionState state)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}, state={4}",
                companyId, dialerId, campaignId, transferId, state);

            return Execute(companyId,
                "DialerService.TransferSetConnectionState",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.TransferSetConnectionState(
                        companyId, dialerId, campaignId, transferId, state)));
        }

        public DialerErrorCode TransferComplete(int companyId, int dialerId, long campaignId, string transferId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}",
                companyId, dialerId, campaignId, transferId);

            return Execute(companyId,
                "DialerService.TransferComplete",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.TransferComplete(
                        companyId, dialerId, campaignId, transferId)));
        }

        public DialerErrorCode TransferCancel(int companyId, int dialerId, long campaignId, string transferId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, campaignId={2}, transferId={3}",
                companyId, dialerId, campaignId, transferId);

            return Execute(companyId,
                "DialerService.TransferCancel",
                argumentsAsString,
                () => DoDialerCall(
                    () => _bridgeToDialer.TransferCancel(
                        companyId, dialerId, campaignId, transferId)));
        }

        public void InitializeRecording()
        {            
            const string argumentsAsString = ""; // There are no arguments

            Execute(0,
                "DialerService.InitializeRecording",
                argumentsAsString,
                () => DoDialerCall(() =>
            {
                _bridgeToDialerRecording.InitializeRecording();
                        return DialerErrorCode.Success; // Execute and DoDialerCall need a return value;
            }
                    ));
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long campaignId, int interviewId)
        {            
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, interviewId={2}",
                companyId, campaignId, interviewId);

            return Execute(0,
                "DialerService.GetAudioRecords",
                argumentsAsString,
                () => DoDialerCall(() =>
                    {
                        var result = _bridgeToDialerRecording.GetAudioRecords(companyId, campaignId, interviewId);

                        var resultAsStrings = result.Select(AudioRecordInfoToString).ToList();

                        Logger.Verbose(
                            "DialerService.GetAudioRecords",
                            "companyId={0}, campaignId={1}, Result is [{2}]({3})",
                            companyId, campaignId, resultAsStrings.Count, string.Join(", ", resultAsStrings));

                        return result;
                    }
                    ));
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
                () => DoDialerCall(
                    () => _bridgeToDialerRecording.GetBulkAudioRecords(companyId, interviewIdentities)));
        }

        public bool[] AreRecordsExists(int companyId, long campaignId, int[] interviewIds)
        {            
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, interviewIds={2}",
                companyId, campaignId, string.Join(", ", interviewIds.Select(s => s.ToString())));

            return Execute(0,
                "DialerService.InitializeRecording",
                argumentsAsString,
                () => DoDialerCall(() =>
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
                    ));
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
                chosenCompanyId,
                chosenDialerId,
                dialerState);

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
                chosenDialerId,
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
                chosenDialerId,
                campaignId,
                agentId,
                callId,
                (int)outcome,
                dialerAccompanyingCallInfo);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        public void NotifyInboundCall(
            int companyId,
            int dialerId,
            string ddiNumber,
            string cliNumber,
            string inboundCallId)
        {
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyInboundCall",
                "companyId={0}/{1}, dialerId={2}/{3}, " +
                "ddiNumber={4}, cliNumber={5}, inboundCallId={6}",
                companyId, chosenCompanyId, dialerId, chosenDialerId,
                ddiNumber, cliNumber, inboundCallId);

            var dialerEvent = new DialerEventNotifyInboundCall(
                DialerEventPriority.HighPriority,
                chosenCompanyId,
                chosenDialerId,
                ddiNumber,
                cliNumber,
                inboundCallId);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        public void NotifyCallDroppedByRespondent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId)
        {
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyCallDroppedByRespondent",
                "companyId={0}/{1}, dialerId={2}/{3}, campaignId={4}, agentId={5}, callId={6}",
                companyId, chosenCompanyId, dialerId, chosenDialerId, campaignId, agentId, callId);

            var dialerEvent = new DialerEventNotifyCallDroppedByRespondent(
                DialerEventPriority.LowPriority,
                chosenCompanyId,
                chosenDialerId,
                campaignId,
                agentId,
                callId);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        public void NotifyInboundCallDroppedByRespondent(int companyId, int dialerId, string inboundCallId)
        {
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyInboundCallDroppedByRespondent",
                "companyId={0}/{1}, dialerId={2}/{3}, inboundCallId={4}",
                companyId, chosenCompanyId, dialerId, chosenDialerId, inboundCallId);

            var dialerEvent = new DialerEventNotifyInboundCallDroppedByRespondent(
                DialerEventPriority.LowPriority,
                chosenCompanyId,
                chosenDialerId,
                inboundCallId);

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
                chosenDialerId,
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
                chosenDialerId,
                campaignId,
                groupId,
                callsSelectionAlgorithm,
                callCount);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        public void NotifyIvrSubmit(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            KeyValuePair<string, string>[] variables)
        {
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyIvrSubmit",
                "companyId={0}/{1}, dialerId={2}/{3}, " +
                "campaignId={4}, agentId={5}, " +
                "variables=[{6}]",
                companyId, chosenCompanyId, dialerId, chosenDialerId,
                campaignId, agentId,
                string.Join(", ", variables.Select(x => x.Key + ": " + x.Value)));

            var dialerEvent = new DialerEventNotifyIvrSubmit(
                DialerEventPriority.HighPriority,
                chosenCompanyId,
                chosenDialerId,
                campaignId,
                agentId,
                variables);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        public void NotifyTransferState(int companyId, int dialerId, string transferId, TransferState transferState)
        {
            var chosenCompanyId = ChooseCompanyId(companyId);
            var chosenDialerId = ChooseDialerId(dialerId);

            Logger.Verbose("DialerService.NotifyTransferState",
                "companyId={0}/{1}, dialerId={2}/{3}, " +
                "transferId={4}, transferState={5}, ",
                companyId, chosenCompanyId, dialerId, chosenDialerId,
                transferId, transferState);

            var dialerEvent = new DialerEventNotifyTransferState(
                DialerEventPriority.HighPriority,
                chosenCompanyId,
                chosenDialerId,
                transferId,
                transferState);

            TryToSendEventNotification(dialerEvent, chosenCompanyId, chosenDialerId);
        }

        private static string GetDialerDriverStateFullFilename()
        {
            return string.Format("{0}{1}.xml", DialerServiceAppDataPath.GetServiceAppDataPath(), Settings.Default.DialerDriverAssemblyName);
        }

        private List<string> GetDialerInterfaceAssemblyVersion()
        {
            var assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            var assemblyName = GetDialerInterfaceAssemblyNameFromList(assemblies);

            Logger.Info("DialerService.GetDialerInterfaceAssemblyVersion", "Dialer Interface assembly: [{0}]", assemblyName);

            var assembly = Assembly.ReflectionOnlyLoad(assemblyName.FullName);

            return new List<string>
            {
                string.Format("{0}.{1}", assemblyName.Version.Major, assemblyName.Version.Minor),
                FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion
            };
        }

        private AssemblyName GetDialerInterfaceAssemblyNameFromList(AssemblyName[] assemblies)
        {
            var assemblyName = assemblies.FirstOrDefault(x => x.Name.Contains(DialerInterfaceNamePattern));

            if (assemblyName == null)
            {
                throw new DialerException(
                    string.Format("Dialer Interface assembly is not found. Pattern=[{0}], All referenced assemblies=[{1}]",
                        DialerInterfaceNamePattern,
                        string.Join("; ", assemblies.Select(x => x.Name))));
            }

            return assemblyName;
        }

        public IEnumerable<LogFileInfo> GetLogFiles()
        {
            var argumentsAsString = "";

            return Execute(0,
                "DialerService.GetLogFiles",
                argumentsAsString,
                () => Logger.LogFileGetter.GetLogFiles());
        }

        public byte[] GetLogFileBodyZipped(string fileName)
        {
            var argumentsAsString = $"{nameof(fileName)}={fileName}";

            return Execute(0,
                "DialerService.GetLogFileBodyZipped",
                argumentsAsString,
                () => Logger.LogFileGetter.GetLogFileBodyZipped(fileName));

        }
    }
}
