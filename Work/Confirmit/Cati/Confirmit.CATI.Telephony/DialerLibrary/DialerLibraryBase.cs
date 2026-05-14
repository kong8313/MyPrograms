using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony.DialerService.Contract;
using ConfirmitDialerInterface;
using DialerCommon;
using DialerCommon.DialerParameters;
using IDialerService = Confirmit.CATI.Telephony.DialerService.Contract.IDialerService;

namespace Confirmit.CATI.Telephony.DialerLibrary
{
    public class DialerLibraryBase
    {
        internal const int DelayBetweenRetries = 500;
        internal Logger logger;
        private readonly CatiCommonILoggerToCodiILogger _catiCommonILoggerToCodiILogger;

        private ConnectionParameters _connectionParameters;

        // TODO: This static instance of the configuration parameters is used to access static configuration parameters, 
        // i.e. parmeters of the dialer type, such as "IsHangupSupported" etc.
        // For the moment this static member is accessed from so called "EmptyDialerObject",
        // The behaviour of this static configuration parameters member would change if "EmptyDialerObject" was eliminated.
        protected static GenericConfigurationParameters _commonConfigurationParameters;

        private readonly IChannelFactoryWrapperFactory<IDialerService> _channelFactoryWrapperFactory;

        private ICodiVersionCoreProxy _codiWsCoreProxy;
        private ICodiVersionRecordingProxy _codiWsRecordingProxy;
        private ICodiVersionFacilitiesProxy _codiWsFacilitiesProxy;

        internal readonly Type[] ExceptionTypesToRetry =
        {
            typeof (EndpointNotFoundException),
            typeof (ServerTooBusyException)
        };

        // Internal use only and should not be sent from dialer dll"
        internal readonly CallOutcome[] UnexpectedOutcomes =
        {
            CallOutcome.NotDefined,
            CallOutcome.Appointment,
            CallOutcome.QuotaFail,
            CallOutcome.Completed,
            CallOutcome.FreshSample,
            CallOutcome.Blacklist,
            CallOutcome.NotAutomaticallyDialled,
            CallOutcome.TransferToWeb,
            CallOutcome.TransferToCati,
            CallOutcome.TransferToCapi,
            CallOutcome.TransferToIvr,
            CallOutcome.FilteredByCallDelivery
        };

        protected CodiVersionInfoCommon _codiVersionInfo;

        public CodiVersionInfoCommon GetCodiVersionInfo()
        {
            return _codiVersionInfo;
        }

        protected int DialerId { set; get; }

        public DialerLibraryBase() :
            this(new ChannelFactoryWrapperFactory<IDialerService>()) //TODO replace this 'new' by getting ChannelFactoryWrapperFactory instance through the UnityContainer (ServiceLocator)
        {
        }

        public DialerLibraryBase(IChannelFactoryWrapperFactory<IDialerService> channelFactoryWrapperFactory)
        {
            logger = new Logger("DialerLib");
            _catiCommonILoggerToCodiILogger = new CatiCommonILoggerToCodiILogger(logger);
            _channelFactoryWrapperFactory = channelFactoryWrapperFactory;
        }

        internal TResult ExecuteWithException<TResult>(string methodArguments, Func<TResult> methodBody)
        {
            try
            {
                return methodBody();
            }
            catch (Exception ex)
            {
                var methodName = methodBody.Method.Name;

                logger.WriteLine(TraceEventType.Error, "DialerLibrary." + methodName, ex + " /// " + methodArguments);

                throw;
            }
        }

        internal TResult Execute<TResult>(string methodArguments, Func<TResult> methodBody, TResult onExceptionResult)
        {
            try
            {
                return ExecuteWithException(methodArguments, methodBody);
            }
            catch (Exception)
            {
                return onExceptionResult;
            }
        }

        internal DialerErrorCode Execute(string methodArguments, Func<DialerErrorCode> methodBody)
        {
            var result = DialerErrorCode.UnknownError;
            string exceptionString = null;

            try
            {
                result = methodBody();
            }
            catch (FaultException<DialerExceptionDetail> ex)
            {
                result = ex.Detail.ErrorCode;
                exceptionString = ex.ToString();
            }
            catch (DialerParametersException ex)
            {
                exceptionString = ex.ToString();
                throw;
            }
            catch (Exception ex)
            {
                result = DialerErrorCode.Exception;
                exceptionString = ex.ToString();
            }
            finally
            {
                if (result != DialerErrorCode.Success)
                {
                    var methodName = methodBody.Method.Name;

                    logger.WriteLine(
                        DialerErrorSeverityProvider.IsWarning(result) ? TraceEventType.Warning : TraceEventType.Error,
                        "DialerLibrary." + methodName,
                        string.Format("Failed with error: {0}{1} /// {2}",
                            result, (exceptionString == null) ? string.Empty : ", " + exceptionString, methodArguments));
                }
            }

            return result;
        }

        internal TResult DoDialerServiceCall<TResult>(Func<TResult> delegatedCall, int retryLimit)
        {
            var retryCount = 0;
            string methodName;

            do
            {
                retryCount++; // will count starting from 1

                try
                {
                    return delegatedCall();
                }
                catch (Exception ex)
                {
                    if (!ExceptionTypesToRetry.Contains(ex.GetType()))
                    {
                        throw;
                    }

                    methodName = delegatedCall.Method.Name;

                    logger.WriteLine(
                        TraceEventType.Error,
                        "DialerLibrary." + methodName,
                        string.Format("[retry: {0}] {1}", retryCount, ex));

                    if (retryCount < retryLimit)
                    {
                        //TODO: Should it be configurable?
                        Thread.Sleep(DelayBetweenRetries);
                    }
                }
            } while (retryCount < retryLimit);

            throw new Exception(string.Format("{0} is failed. Service calls retry limit [{1}] is reached", methodName, retryLimit));
        }

        private TResult DoDialerServiceCall<TResult>(Func<TResult> delegatedCall)
        {
            return DoDialerServiceCall(delegatedCall, ServiceLocator.Resolve<IDialerSettings>().ServiceCallsRetryLimit);
        }

        private IChannelFactoryWrapper<IDialerService> ConfigureDialerChannelFactory(
            string endpointName,
            string dialerServiceAddress,
            string authorizationKeyForOutgoingRequests)
        {
            var keepAlive = ServiceLocator.Resolve<IToggleSettings>().EnableHttpKeepAliveForDialer;
            
            var configuration = new DialerChannelFactoryWrapperConfiguration(
                endpointName,
                dialerServiceAddress,
                authorizationKeyForOutgoingRequests,
                keepAlive);

            return _channelFactoryWrapperFactory.Create(configuration, _catiCommonILoggerToCodiILogger);
        }

        public int Initialize(
            int dialerId,
            string tenantId,
            string connectionParametersXml,
            string configurationParametersXml,
            string surveyDefaultParametersXml,
            bool sendInitializeToWebService = true)
        {
            var argumentsAsString = string.Format(
                "dialerId = {0}, tenantId={1}, connectionParametersXml={2}, configurationParametersXml={3}, surveyDefaultParametersXml={4}",
                dialerId, tenantId, connectionParametersXml, configurationParametersXml, surveyDefaultParametersXml);

            logger.WriteLine(TraceEventType.Information, "DialerLibrary.Initialize", argumentsAsString);

            return (int)Execute(argumentsAsString, () =>
                {
                    DialerId = dialerId;

                    _connectionParameters = new ConnectionParameters(connectionParametersXml);

                    string authorizationKeyForOutgoingRequests;

                    using (var encryptor = new DialerAuthorizationKeyEncryptor())
                    {
                        authorizationKeyForOutgoingRequests =
                            encryptor.DecryptString(_connectionParameters.AuthorizationKeyForOutgoingRequests);
                        encryptor.Clear();
                    }

                    _commonConfigurationParameters = new GenericConfigurationParameters(configurationParametersXml);

                    CreateCodiWsProxy(authorizationKeyForOutgoingRequests);
                    CreateFacilitiesCodiWsProxy(authorizationKeyForOutgoingRequests);

                    if (sendInitializeToWebService)
                    {
                        var initialisationResult = _codiWsCoreProxy.Initialize(
                            int.Parse(tenantId), // TODO: CODI changes 
                            dialerId,
                            configurationParametersXml);

                        if (initialisationResult != DialerErrorCode.Success)
                        {
                            return initialisationResult;
                        }

                        //TODO: propagate tenantId 'int' type to the upper level, and (maybe) rename it to companyId.
                        return _codiWsCoreProxy.GetState(int.Parse(tenantId), dialerId) == DialerState.Available
                            ? DialerErrorCode.Success
                            : DialerErrorCode.NotAvailable;
                    }
                    else
                    {
                        return DialerErrorCode.Success;
                    }
                });
        }

        public DialerFeatures GetFeatures(string tenantId)
        {
            var argumentsAsString = $"DialerId = {DialerId}, tenantId={tenantId}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.GetFeatures", argumentsAsString);

            return _codiWsCoreProxy.GetFeatures(int.Parse(tenantId), DialerId);
        }

        private void CreateCodiWsProxy(string authorizationKeyForOutgoingRequests)
        {
            if (_codiWsCoreProxy != null)
            {
                _codiWsCoreProxy.ReleaseDialerChannel();
                _codiWsCoreProxy = null;
            }

            var dialerChannel = ConfigureDialerChannelFactory(
                _connectionParameters.DialerServiceEndpoint,
                _connectionParameters.DialerServiceAddress,
                authorizationKeyForOutgoingRequests);

            _codiVersionInfo = new CodiVersionDetector().Version(dialerChannel);

            Trace.TraceInformation("DialerLibrary.Initialize, CODI Version: {0}", _codiVersionInfo);

            _codiWsCoreProxy = new CodiVersionProxyFactory().Create(
                _codiVersionInfo.CodiMajorVersion,
                dialerChannel,
                _connectionParameters.DialerServiceEndpoint,
                _connectionParameters.DialerServiceAddress,
                authorizationKeyForOutgoingRequests,
                _catiCommonILoggerToCodiILogger);
        }

        private void CreateRecordingCodiWsProxy(string authorizationKeyForOutgoingRequests)
        {
            if (_codiWsRecordingProxy != null)
            {
                _codiWsRecordingProxy.ReleaseDialerChannel();
                _codiWsRecordingProxy = null;
            }

            var dialerChannel = ConfigureDialerChannelFactory(
                _connectionParameters.DialerServiceEndpoint,
                _connectionParameters.DialerServiceAddress,
                authorizationKeyForOutgoingRequests);

            _codiVersionInfo = new CodiVersionDetector().Version(dialerChannel);

            Trace.TraceInformation("DialerLibrary.Initialize, CODI Version: {0}", _codiVersionInfo);

            _codiWsRecordingProxy = new CodiVersionProxyFactory().CreateRecordingProxy(
                _codiVersionInfo.CodiMajorVersion,
                dialerChannel,
                _connectionParameters.DialerServiceEndpoint,
                _connectionParameters.DialerServiceAddress,
                authorizationKeyForOutgoingRequests,
                _catiCommonILoggerToCodiILogger);
        }

        private void CreateFacilitiesCodiWsProxy(string authorizationKeyForOutgoingRequests)
        {
            if (_codiWsFacilitiesProxy != null)
            {
                _codiWsFacilitiesProxy.ReleaseDialerChannel();
                _codiWsFacilitiesProxy = null;
            }

            var dialerChannel = ConfigureDialerChannelFactory(
                _connectionParameters.DialerServiceEndpoint,
                _connectionParameters.DialerServiceAddress,
                authorizationKeyForOutgoingRequests);

            _codiVersionInfo = new CodiVersionDetector().Version(dialerChannel);

            Trace.TraceInformation("DialerLibrary.Initialize, CODI Version: {0}", _codiVersionInfo);

            _codiWsFacilitiesProxy = new CodiVersionProxyFactory().CreateFacilitiesProxy(
                _codiVersionInfo.CodiMajorVersion,
                dialerChannel,
                _connectionParameters.DialerServiceEndpoint,
                _connectionParameters.DialerServiceAddress,
                authorizationKeyForOutgoingRequests,
                _catiCommonILoggerToCodiILogger);
        }

        public int Release(int dialerId, int companyId)
        {
            if (_codiWsCoreProxy == null)
            {
                return 0;
            }
            var argumentsAsString = $"DialerId={dialerId}, companyId={companyId}";

            logger.WriteLine(TraceEventType.Information, "DialerLibrary.Release", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.Release(dialerId, companyId)));
        }

        public int CreateTenant(string tenantId, out string tokenId)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.CreateTenant",
                string.Format("DialerId = {0}, tenantId={1}", DialerId, tenantId));

            tokenId = "DialerLibrary: tokenId is not used";
            return 0;
        }

        public int StartCampaign(string tenantId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, bool recordWholeInterview, string surveyParametersXml)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, tenantId={1}, dialerIds=[{2}], campaignId={3}, surveyDialingMode={4}, " +
                "campaignType={5}, recordWholeInterview={6}, campaignName={7}, surveyParametersXml={8}",
                DialerId, tenantId, string.Join(", ", dialerIds), campaignId, dialingMode, campaignType, recordWholeInterview, campaignName, surveyParametersXml);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StartCampaign", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StartCampaign(
                int.Parse(tenantId), // TODO: Eliminate the type conversion
                dialerIds,
                campaignId,
                campaignName,
                dialingMode,
                recordWholeInterview,
                surveyParametersXml)));
        }

        public int StopCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, tenantId={1}, dialerIds=[{2}], campaignId={3}, surveyDialingMode={4}",
                 DialerId, tenantId, string.Join(", ", dialerIds), campaignId, dialingMode);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StopCampaign", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StopCampaign(
                int.Parse(tenantId),
                dialerIds,
                campaignId,
                dialingMode))); // TODO: Eliminate the type conversion
        }

        public int KillCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, tenantId={1}, dialerIds=[{2}], campaignId={3}, surveyDialingMode={4}",
                 DialerId, tenantId, string.Join(", ", dialerIds), campaignId, dialingMode);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.KillCampaign", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.KillCampaign(
                int.Parse(tenantId),
                dialerIds,
                campaignId,
                dialingMode))); // TODO: Eliminate the type conversion
        }

        public int Login(
            string tenantId,
            long campaignId,
            string agentId,
            string agentName,
            AgentType agentType,
            string agentExtension,
            string userId,
            bool isPredictive,
            bool isLocal,
            IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, agentName={4}, agentType={5}, agentExtension={6}, " +
                "userId={7}, isPredictive={8}, isLocal={9}, agentAttributes={10}",
                DialerId, tenantId, campaignId, agentId, agentName, agentType, agentExtension,
                userId, isPredictive, isLocal,
                agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString()));

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.Login", argumentsAsString);

            //TODO: CODI changes: get real values of the resourceBindingType from the upper level
            var resourceBindingType = isLocal ? ResourceBindingType.Local : ResourceBindingType.PhoneNumber; //It is a stub for now.

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.Login(
                    int.Parse(tenantId), // TODO: Eliminate the type conversion
                    DialerId,
                    campaignId,
                    int.Parse(agentId), // TODO: Eliminate the type conversion
                    agentName,
                    agentType,
                    agentExtension,
                    isPredictive,
                    resourceBindingType,
                    agentAttributes)));
        }

        // Note, the tenantId in other (older) methods is nothing else but the companyId. The older methods need to be refactored.
        public int SetCampaign(int companyId, long campaignId, int agentId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, companyId={1}, campaignId={2}, agentId={3}",
                DialerId, companyId, campaignId, agentId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.Logout", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SetCampaign(
                companyId,
                DialerId,
                campaignId,
                agentId)));
        }

        public int Logout(string tenantId, long campaignId, bool isPredictive, string agentId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, isPredictive={3}, agentId={4}",
                DialerId, tenantId, campaignId, isPredictive, agentId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.Logout", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.Logout(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                false)));  // TODO: Eliminate the type conversion
        }

        /// <summary>
        /// A function that forcefully logs an Agent out. The function does not wait for ongoing calls to complete.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// </returns>
        public int KillAgent(string tenantId, long campaignId, string agentId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}",
                DialerId, tenantId, campaignId, agentId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.KillAgent", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.KillAgent(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId)))); // TODO: Eliminate the type conversion
        }

        public int GoReady(string tenantId, long campaignId, string agentId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}",
                DialerId, tenantId, campaignId, agentId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.GoReady", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.GoReady(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId)))); // TODO: Eliminate the type conversion
        }

        public int CompletePreview(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, " +
                "contactId={4}, callId={5}, phoneNumber={6}, isRecording={7}",
                DialerId, tenantId, campaignId, agentId,
                contactId, callId, phoneNumber, isRecording);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.CompletePreview", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.CompletePreview(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                contactId,
                callId,
                phoneNumber,
                isRecording)));
        }

        public int GoNotReady(string tenantId, long campaignId, string agentId, string breakName)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, breakName={4}",
                DialerId, tenantId, campaignId, agentId, breakName);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.GoNotReady", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.GoNotReady(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                breakName)));
        }

        /// <summary>
        /// A function that sends the number to be dialed. Now containing group id for the call, 
        /// allowing to specify the dialing mode for the call, allowing to specify a timeout for 
        /// call aging, and allowing to specify whether the call should be recorded or not.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="diallingMode">New feature allow call to be made in a specific mode independent 
        ///   of campaign default dialing mode</param>
        /// <param name="groupId">Identifier for call group to which this call belongs</param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// </returns>
        public int SendNumber(string tenantId, long campaignId, DialingMode diallingMode, int groupId, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SendNumber",
                string.Format("DialerId = {0}, tenantId={1}, campaignId={2}, diallingMode={3}, groupId={4}, " +
                "contactId={5}, callId={6}, phoneNumber={7}, callAgingTimeout={8}, isRecording={9}",
                DialerId, tenantId, campaignId, diallingMode, groupId,
                contactId, callId, phoneNumber, callAgingTimeout, isRecording));

            //Is not supported
            return 0;
        }

        /// <summary>
        /// A function that sends a set of numbers to be dialed.
        /// </summary>
        /// <param name="requestId"> </param>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="campaignDiallingMode"></param>
        /// <param name="callList">List of CallInfo objects that contains numbers to be dialed</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_GROUP	0X81000022	Group ID does not belong to any existing group
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOTACTIVE	0x8100000f	Campaign is not active.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        public int SendNumbers(
            string requestId,
            string tenantId,
            long campaignId,
            DialingMode campaignDiallingMode,
            List<CallInfo> callList,
            int callAgingTimeout,
            bool isRecording)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, requestId = {1}, tenantId={2}, campaignId={3}, NumberOfCalls={4}, callAgingTimeout={5}, isRecording={6}",
                DialerId, requestId, tenantId, campaignId, callList.Count, callAgingTimeout, isRecording);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SendNumbers", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SendNumbers(
                requestId,
                int.Parse(tenantId),
                DialerId,
                campaignId,
                campaignDiallingMode,
                callList,
                callAgingTimeout)));
        }

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent. 
        /// Now allowing to specify dialing mode for the call.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="callDiallingMode"></param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <param name="respondentVariables">Info related to the Contact.</param>
        /// <returns></returns>
        public int SendNumberToAgent(string tenantId, long campaignId, string agentId, DialingMode callDiallingMode, int contactId, int callId, string phoneNumber, bool isRecording, string callerId, Dictionary<string, object> respondentVariables)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, diallingMode={4}, " +
                "contactId={5}, callId={6}, phoneNumber={7}, isRecording={8}, respondentVariables={9}",
                DialerId, tenantId, campaignId, agentId, callDiallingMode,
                contactId, callId, phoneNumber, isRecording, respondentVariables?.Stringify());

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SendNumberToAgent", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SendNumberToAgent(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                callDiallingMode,
                contactId,
                callId,
                phoneNumber,
                isRecording,
                callerId,
                respondentVariables)));
        }

        /// <summary>
        /// A function that initiates redialing by a specific agent. The phone number may be the same as 
        /// for previous dial or may be different.
        /// Note, it's dialer responsibility to do hangup if the agent is in call.
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="phoneNumber">The telephone number to dial.</param>
        /// <param name="isRecording"></param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        public int Redial(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, " +
                "contactId={4}, callId={5}, phoneNumber={6}, isRecording={7}, callerId={8}",
                DialerId, tenantId, campaignId, agentId,
                contactId, callId, phoneNumber, isRecording, callerId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.Redial", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.Redial(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                contactId,
                callId,
                phoneNumber,
                isRecording,
                callerId)));
        }

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent. 
        /// Now allowing to specify dialing mode for the call.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="diallingMode">The dilling mode for the particular call. 
        ///   It allows call to be made in a specific mode independent of campaign default dilling mode.</param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns></returns>
        public int SendNumberToAgentEx(string tenantId, long campaignId, string agentId, DialingMode diallingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SendNumberToAgentEx",
                string.Format("DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, diallingMode={4}, " +
                "contactId={5}, callId={6}, phoneNumber={7}, callAgingTimeout={8}, isRecording={9}",
                DialerId, tenantId, campaignId, agentId, diallingMode,
                contactId, callId, phoneNumber, callAgingTimeout, isRecording));

            // Is not supported
            return 0;
        }

        /// <summary>
        /// Drops respondent call on dialer
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant)</param>
        /// <param name="campaignId">The unique identifier of the Campaign</param>
        /// <param name="agentId">An Agent identifier</param>
        /// <param name="interviewId">The unique identifier of the  Interview</param>
        /// <param name="callId">The unique identifier of the  Call</param>
        /// <returns></returns>
        public int Hangup(string tenantId, long campaignId, string agentId, int interviewId, long callId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}",
                DialerId, tenantId, campaignId, agentId, interviewId, callId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.Hangup", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.Hangup(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                interviewId,
                callId)));
        }

        public int CompleteCall(string tenantId, long campaignId, string agentId, 
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, makeAgentReady={4}, breakName={5}, interviewId={6}, callId={7}",
                DialerId, tenantId, campaignId, agentId, makeAgentReady, makeAgentReady ? "NULL" : breakName, interviewId, callId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.CompleteCall", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.CompleteCall(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                interviewStatus,
                makeAgentReady,
                breakName,
                interviewId,
                callId)));
        }

        public int SetNextInterview(string tenantId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, currentCampaignId={2}, agentId={3}" +
                ", currentInterviewStatus={4}, nextCampaignId={5}, nextInterviewId={6}, nextCallId={7}",
                DialerId, tenantId, currentCampaignId, agentId,
                currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SetNextInterview", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SetNextInterview(
                int.Parse(tenantId),
                DialerId,
                currentCampaignId,
                int.Parse(agentId),
                currentInterviewStatus,
                nextCampaignId,
                nextInterviewId,
                nextCallId)));
        }

        public int StartCustomIvrInterview(string tenantId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}" +
                ", interviewId={4}, callId={5}, respondentSurveyLink={6}",
                DialerId, tenantId, campaignId, agentId,
                interviewId, callId, respondentSurveyLink);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StartCustomIvrInterview", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StartCustomIvrInterview(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                interviewId,
                callId,
                respondentSurveyLink)));
        }

        public int UpdateInterviewStatus(
            string companyId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            InterviewStatus interviewStatus)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, campaignId={1}, agentId={2}, interviewId={3}, callId={4}, interviewStatus={5}",
                companyId, campaignId, agentId, interviewId, callId, interviewStatus);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.UpdateInterviewStatus", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.UpdateInterviewStatus(
                int.Parse(companyId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                interviewId,
                callId,
                interviewStatus)));
        }


        /// <summary>
        /// A function that sets Predictive Dialing Engine tuning parameters. 
        /// If the input is set to -1 (the unsigned equivalent of) the parameter will be ignored and not updated.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="abandonTarget">The target abandonment rate threshold value, for example, 0.03 = 3%.</param>
        /// <param name="abandonDelay">The delay in seconds before a call will be abandoned.</param>
        /// <param name="estimatedTalkTime">The expected average talk time, measured in seconds (not used).</param>
        /// <param name="ringTimeoutOut">The time before a call is terminated as a no answer, measured in seconds.</param>
        /// <param name="previewTimeOut">The time out period for preview. If this is set, after this period a call will be automatically initiated.</param>
        /// <param name="restrainedDialling">Set the Campaign to run using restrained dialing mode. 
        ///   Restrained dialing mode is used in predictive dialing and ensures that the threshold target 
        ///   abandonment rate is never overstepped, not even temporarily. If restrained dialing is not used 
        ///   the threshold may be overstepped (in such situations the Predictive Dialing Engine will then 
        ///   dial conservatively until the rate falls back under the threshold value). 
        ///   When using restrained dialing the Predictive Dialing Engine basically has to wait for enough 
        ///   calls to have succeeded before trying to over dial, so there is a phase at the beginning of a 
        ///   Campaign or a dialing period where it will be slow to dial predictively; after this initial 
        ///   period there is little difference in the behavior of the two modes.
        /// </param>
        /// <returns>
        /// </returns>
        public int SetTuning(string tenantId, long campaignId, string abandonTarget, string abandonDelay, string estimatedTalkTime, string ringTimeoutOut, string previewTimeOut, string restrainedDialling)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SetTuning",
                string.Format("DialerId = {0}, tenantId={1}, campaignId={2}, abandonTarget={3}, abandonDelay={4}, " +
                "estimatedTalkTime={5}, ringTimeoutOut={6}, previewTimeOut={7}, restrainedDialling={8}",
                DialerId, tenantId, campaignId, abandonTarget, abandonDelay, estimatedTalkTime,
                ringTimeoutOut, previewTimeOut, restrainedDialling));

            //Not implemented now.
            //May be it can be implemented via calling PRO-T-S.StudyCfg...
            return 0;
        }

        /// <summary>
        /// A function that sets the groups that an agent can take calls for. This function 
        /// allows to change the group setting for an agent who is currently logged into a campaign. 
        /// This function is executed synchronously, the return code will indicate if the setting 
        /// happened successfully. 
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="groupIds">Array of GroupIDs. This is the new set of groups for that agent.</param>
        /// <returns>If the agent is not logged in or not logged into that campaign an unknown 
        /// agent error will be returned. There are also the usual default error messages.
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_AGENT	0x81000006	No Agent could be found for the given Agent ID.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        public int SetGroups(string tenantId, long campaignId, string agentId, int[] groupIds)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, groupIds=[{4}]",
                DialerId, tenantId, campaignId, agentId, string.Join(",", groupIds));

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SetGroups", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SetGroups(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId), groupIds)));
        }

        /// <summary>
        /// Removes the specified calls(numbers) from the dialer. The numbers 
        /// will be returned via NotifyOutcome with a CALL_FLUSHED outcome code. 
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="dialerIds">Ids of targets dialers</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="callsList">The list af calls to be flushed.</param>
        /// <returns>
        /// </returns>
        public int FlushNumbers(string tenantId, int[] dialerIds, long campaignId, List<CallInfo> callsList)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, tenantId={1}, dialerIds=[{2}], campaignId={3}, NumberOfCalls={4}",
                DialerId, tenantId, string.Join(", ", dialerIds), campaignId, callsList.Count);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.FlushNumbers", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.FlushNumbers(
                int.Parse(tenantId),
                dialerIds,
                campaignId,
                callsList)));
        }

        public int StartPlayback(string tenantId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, fileName={6}",
                DialerId, tenantId, campaignId, agentId, interviewId, callId, fileName);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StartPlayback", argumentsAsString);

            int outTimeOfPlayingInSeconds = 0;

            var result = (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StartPlayback(
                    int.Parse(tenantId),
                    DialerId,
                    campaignId,
                    int.Parse(agentId),
                    interviewId,
                    callId,
                    fileName,
                    out outTimeOfPlayingInSeconds)));

            timeOfPlayingInSeconds = outTimeOfPlayingInSeconds;

            return result;
        }

        public int StopPlayback(string tenantId, long campaignId, string agentId, int callId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, callId={4}",
                DialerId, tenantId, campaignId, agentId, callId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StopPlayback", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StopPlayback(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                callId)));
        }

        public int PauseOrResumePlayback(string tenantId, long campaignId, string agentId, int callId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3},  callId={4}",
                DialerId, tenantId, campaignId, agentId, callId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.PauseOrResumePlayback", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.PauseOrResumePlayback(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                callId)));
        }

        public int ToggleInterviewerListensToPlaybackOrRespondent(string tenantId, long campaignId, string agentId, int callId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3},  callId={4}",
               DialerId, tenantId, campaignId, agentId, callId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.ToggleInterviewerListensToPlaybackOrRespondent", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.ToggleInterviewerListensToPlaybackOrRespondent(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                callId)));
        }

        /// <summary>
        /// A function that starts monitoring Agent calls. This function will be executed synchronously, 
        /// i.e. success return code means that the call was placed on the switch (not connected yet!). 
        /// If the customer or Agent does not exist, or there is any other reason why the call cannot be 
        /// made at that point in time, an appropriate error message will be returned and the call discarded.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="agentId"></param>
        /// <param name="number">Supervisor’s telephone number</param>
        /// <param name="sessionId">If an initial monitor has not been performed SessionID should be empty 
        /// and its value will be returned in the return message. If an initial monitor has already been 
        /// performed the SessionID has to be specified and telephone number can be omitted.</param>
        /// <returns>
        /// </returns>
        public int StartMonitor(
            string tenantId,
            string agentId,
            string number,
            ref string sessionId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, agentId={2}, number={3}, sessionId={4}",
                DialerId, tenantId, agentId, number, sessionId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StartMonitor", argumentsAsString);

            if (sessionId.Length > 0)
            {
                //Stop the previous monitoring session first
                StopMonitor(tenantId, sessionId);
            }

            var outSessionId = string.Empty;

            //TODO: CODI changes: perform a new input parameter 'supervisorName' and receive it from the upper level
            const string supervisorName = "";

            //TODO: CODI changes: rename 'phoneNumber' parameter to 'supervisorConnectionString' here and on the upper level
            var supervisorConnectionString = number;

            //TODO: CODI changes: get real values of the resourceBindingType from the upper level
            const ResourceBindingType resourceBindingType = ResourceBindingType.PhoneNumber; //It is a fake stub for now.

            var result = (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StartMonitor(
                    int.Parse(tenantId),
                    DialerId,
                    int.Parse(agentId),
                    supervisorName,
                    supervisorConnectionString,
                    resourceBindingType,
                    ref outSessionId)));

            sessionId = outSessionId;

            return result;
        }

        ///  <summary>
        ///  A function that stops monitoring Agent calls. If the session does not exist, or there is any 
        ///  other reason why the call cannot be disconnected, an appropriate error message will be returned 
        ///  and the call discarded.
        ///  </summary>
        /// <param name="tenantId"></param>
        /// <param name="sessionId">Indicates which session should be disconnected.</param>
        ///  <returns>
        ///  ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        ///  ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        ///  ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        ///  ENGINE_RESULT_NOSERVICES	0x81000017	The engine is not running.
        ///  ENGINE_RESULT_UNKNOWN_SESSION	0x81000027	No SESSION could be found for the given SessionID.
        ///  </returns>
        public int StopMonitor(string tenantId, string sessionId)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, sessionId={2}", DialerId, tenantId, sessionId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StopMonitor", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StopMonitor(
                int.Parse(tenantId),
                DialerId,
                sessionId)));
        }

        public int SetMonitorMode(string tenantId, string sessionId, MonitorMode monitorMode)
        {
            var argumentsAsString = $"DialerId = {DialerId}, tenantId={tenantId}, sessionId={sessionId}, monitorMode={monitorMode}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SetMonitorMode", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SetMonitorMode(
                int.Parse(tenantId),
                DialerId,
                sessionId,
                monitorMode)));
        }

        public int StartRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, string label)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, contactId={4}, callId={5}, label={6}",
                DialerId, tenantId, campaignId, agentId, contactId, callId, label);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StartRecording", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StartRecording(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                contactId,
                callId,
                label)));
        }

        public int StopRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, campaignId={2}, agentId={3}, contactId={4}, callId={5}, stopRecordingMode={6}",
                DialerId, tenantId, campaignId, agentId, contactId, callId, stopRecordingMode);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.StopRecording", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.StopRecording(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                contactId,
                callId,
                stopRecordingMode)));
        }

        public bool IsPersonModeSupported(AgentTaskChoiceMode mode)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IsPersonModeSupported",
                string.Format("mode={0}. Returns true for SURVEY_ASSIGNMENT and MANUAL modes", mode));

            return _commonConfigurationParameters.SupportedPersonModes.Contains(mode);
        }

        public bool IsReloginNeededOnSurveyChange()
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IsReloginNeededOnSurveyChange",
                "Returns true");

            return _commonConfigurationParameters.IsReloginNeededOnCampaignChange;
        }

        public bool HasInternalHealthControl()
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.HasInternalHealthControl",
                "Returns false");
            return false;
        }

        /// <summary>
        /// Returns flag indicated is hang up option enabled for interviewer or not
        /// </summary>
        public bool IsHangUpSupported
        {
            get { return _commonConfigurationParameters.IsHangUpSupported; }
        }

        /// <summary>
        /// Returns flag indicating whether Pause/Resume playback command is enabled for interviewer or not
        /// </summary>
        public bool IsPauseOrResumePlaybackSupported
        {
            get { return _commonConfigurationParameters.IsPauseOrResumePlaybackSupported; }
        }

        /// <summary>
        /// Returns flag indicating whether toggle voice source command is enabled for interviewer or not
        /// </summary>
        public bool IsToggleInterviewerListensToPlaybackOrRespondentSupported
        {
            get { return _commonConfigurationParameters.IsToggleAgentListensToPlaybackOrRespondentSupported; }
        }

        public bool IsDynamicExtensionNumberAllowed(bool isAgentLocal)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IsDynamicExtensionNumberAllowed",
                string.Format("isLocalAgent = {0}", isAgentLocal));

            return isAgentLocal ?
                _commonConfigurationParameters.IsDynamicExtensionNumberAllowedForLocalAgents :
                _commonConfigurationParameters.IsDynamicExtensionNumberAllowedForRemoteAgents;
        }

        public DialerState GetState(int dialerId, string tenantId)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, tenantId={1}",
                DialerId, tenantId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.GetState", argumentsAsString);

            // Exceptions should not be logged inside of the GetState method. They are logged at the upper level.
            // There is corresponding GetStateShouldNotLogExceptions() test for checking this behavior.
            // See CR #56936 for details.
            // Because of this the GetState body is not wrapped with Execute()
            //TODO: propagate tenantId 'int' type to the upper level, and (maybe) rename it to companyId.
            return _codiWsCoreProxy.GetState(int.Parse(tenantId), DialerId);
        }

        /// <summary>
        /// Translate internal call outcome of a concrete dialer
        /// to the corresponding<code>CallOutcome</code> outcome
        /// </summary>
        /// <param name="outcome">
        /// call outcome of a concrete dialer (i.e. internal outcome of this intarface implementator)
        /// </param>
        /// <returns></returns>
        public CallOutcome TranslateOutcome(long outcome)
        {
            var translatedOutcome = (CallOutcome)outcome;

            if (!Enum.IsDefined(typeof(CallOutcome), translatedOutcome))
            {
                logger.Warning("DialerLibrary.TranslateOutcome",
                    "Unknown outcome {0} has been translated to 'TelephonyFailure' outcome.", outcome);

                return CallOutcome.TelephonyFailure;
            }

            if (UnexpectedOutcomes.Contains(translatedOutcome))
            {
                logger.Warning("DialerLibrary.TranslateOutcome",
                    "Unexpected outcome {0} has been translated to 'TelephonyFailure' outcome.", translatedOutcome);

                return CallOutcome.TelephonyFailure;
            }

            return translatedOutcome;
        }

        public int SetConfigurationParameters(string tenantId, string configurationParametersXml)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, configurationParametersXml={2}",
                DialerId, tenantId, configurationParametersXml);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SetConfigurationParameters", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SetConfigurationParameters(
                int.Parse(tenantId),
                configurationParametersXml)));
        }

        public int ValidateCampaignParameters(string surveyParametersXml)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.ValidateCampaignParameters",
                string.Format("DialerId = {0}, surveyParametersXml={1}",
                DialerId, surveyParametersXml));

            //if xml is incorrect then exception will occur during its parsing 
            //new SurveyParameters(surveyParametersXml);
            // TODO: Parameters validation

            return 0;
        }

        public int SetCampaignParameters(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string surveyParametersXml)
        {
            var argumentsAsString = string.Format(
                "DialerId = {0}, tenantId={1}, dialerIds=[{2}], campaignId={3}, dialingMode={4}, recordWholeInterview={5}, surveyParametersXml={6}",
                DialerId, tenantId, string.Join(", ", dialerIds), campaignId, dialingMode, recordWholeInterview, surveyParametersXml);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.SetCampaignParameters", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.SetCampaignParameters(
                    int.Parse(tenantId),
                    dialerIds,
                    campaignId,
                    dialingMode,
                    recordWholeInterview,
                    surveyParametersXml)));
        }

        public int GetTrunkLineStatesAndAlarms(string tenantId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            trunkLineStatesAndAlarms = new List<TrunkLineStateAndAlarms>();
            return 0;
        }

        public int TransferToIvr(string tenantId, long campaignId, string agentId, int interviewId, int callId, string endpoint,
                                 IEnumerable<KeyValuePair<string, string>> attributes)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, tenantId={1}, campaignId={2}, agentId={3}, interviewId={4}, callId={5}, " +
                "endpoint={6}, attributes=[{7}]",
                DialerId, tenantId, campaignId, agentId, interviewId, callId,
                endpoint, attributes.Aggregate("", (current, attribute) => current + attribute.ToString()));

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.TransferToIvr", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.TransferToIvr(
                int.Parse(tenantId),
                DialerId,
                campaignId,
                int.Parse(agentId),
                interviewId,
                callId,
                endpoint,
                attributes)));
        }

        public int IvrRenderVoiceXml(int companyId, long campaignId, int agentId, string voiceXml)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, companyId={1}, campaignId={2}, agentId={3}, voiceXml=[{4}]",
                DialerId, companyId, campaignId, agentId, voiceXml);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IvrRenderVoiceXml", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.IvrRenderVoiceXml(
                companyId,
                DialerId,
                campaignId,
                agentId,
                voiceXml)));    
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, companyId={1}, inboundDdiNumbers=[{2}]",
                DialerId, companyId, string.Join<InboundDdiNumber>(", ", inboundDdiNumbers));

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.ConfigureInboundDdiNumbers", argumentsAsString);

            return Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.ConfigureInboundDdiNumbers(
                companyId,
                DialerId,
                inboundDdiNumbers)),
                Enumerable.Repeat(DialerErrorCode.Exception, inboundDdiNumbers.Length).ToArray());
        }

        public int DropInboundCall(int companyId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, companyId={1}, inboundCallId={2}, audioMessageDescriptor={3}",
                DialerId, companyId, inboundCallId, audioMessageDescriptor.NullableToString());

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.DropInboundCall", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.DropInboundCall(
                companyId,
                DialerId,
                inboundCallId,
                audioMessageDescriptor)));
        }

        public int ConnectInboundCall(int companyId, long campaignId, string inboundCallId, CallInfo callInfo,
            long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, companyId={1}, campaignId={2}, inboundCallId={3}, callInfo={4}," +
                " campaignIdsToBorrowAgentsFrom=[{5}], audioMessageDescriptor={6}",
                DialerId, companyId, campaignId, inboundCallId, callInfo,
                campaignIdsToBorrowAgentsFrom != null ? string.Join(", ", campaignIdsToBorrowAgentsFrom) : "<NULL>",
                audioMessageDescriptor.NullableToString());

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.ConnectInboundCall", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.ConnectInboundCall(
                companyId,
                DialerId,
                campaignId,
                inboundCallId,
                callInfo,
                campaignIdsToBorrowAgentsFrom,
                audioMessageDescriptor)));
        }

        public int ConnectInboundCallToAgent(int companyId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            var argumentsAsString = string.Format(
                "DialerId={0}, companyId={1}, campaignId={2}, inboundCallId={3}, callInfo={4}, audioMessageDescriptor={5}",
                DialerId, companyId, campaignId, inboundCallId, callInfo, audioMessageDescriptor.NullableToString());

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.ConnectInboundCallToAgent", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.ConnectInboundCallToAgent(
                companyId,
                DialerId,
                campaignId,
                inboundCallId,
                callInfo,
                audioMessageDescriptor)));
        }

        public int TransferStart(int companyId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            var argumentsAsString =
                $"DialerId={DialerId}, companyId={companyId}, campaignId={campaignId}," +
                $" transferId={transferId}, agentId={agentId}, transferType={transferType}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.TransferStart", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.TransferStart(
                companyId,
                DialerId,
                campaignId,
                transferId,
                agentId,
                transferType)));
        }

        public int TransferSetTarget(int companyId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            var argumentsAsString =
                $"DialerId={DialerId}, companyId={companyId}, campaignId={campaignId}," +
                $" transferId={transferId}, targetType={targetType}, targetResource={targetResource}," +
                $" borrowAgentsFromAllCampaigns={borrowAgentsFromAllCampaigns}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.TransferSetTarget", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.TransferSetTarget(
                companyId,
                DialerId,
                campaignId,
                transferId,
                targetType,
                targetResource,
                borrowAgentsFromAllCampaigns)));
        }

        public int TransferSetConnectionState(int companyId, long campaignId, string transferId,
            ConnectionState state)
        {
            var argumentsAsString =
                $"DialerId={DialerId}, companyId={companyId}, campaignId={campaignId}," +
                $" transferId={transferId}, state={state}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.TransferSetConnectionState", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.TransferSetConnectionState(
                companyId,
                DialerId,
                campaignId,
                transferId,
                state)));
        }

        public int TransferComplete(int companyId, long campaignId, string transferId)
        {
            var argumentsAsString =
                $"DialerId={DialerId}, companyId={companyId}, campaignId={campaignId}, transferId={transferId}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.TransferComplete", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.TransferComplete(
                companyId,
                DialerId,
                campaignId,
                transferId)));
        }

        public int TransferCancel(int companyId, long campaignId, string transferId)
        {
            var argumentsAsString =
                $"DialerId={DialerId}, companyId={companyId}, campaignId={campaignId}, transferId={transferId}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.TransferCancel", argumentsAsString);

            return (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.TransferCancel(
                companyId,
                DialerId,
                campaignId,
                transferId)));
        }

        public IEnumerable<LogFileInfo> GetLogFiles()
        {
            var argumentsAsString = "";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerAPI.GetLogFiles", argumentsAsString);

            return ExecuteWithException(argumentsAsString, () => DoDialerServiceCall(() => _codiWsFacilitiesProxy.GetLogFiles()));
        }

        public byte[] GetLogFileBodyZipped(string fileName)
        {
            var argumentsAsString = $"fileName={fileName}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerAPI.GetLogFileBodyZipped", argumentsAsString);

            return ExecuteWithException(argumentsAsString, () => DoDialerServiceCall(() => _codiWsFacilitiesProxy.GetLogFileBodyZipped(fileName)));
        }

        public string GetDialerVersion()
        {
            var argumentsAsString = "";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerAPI.GetDialerVersion", argumentsAsString);

            return ExecuteWithException(argumentsAsString, () => DoDialerServiceCall(() => _codiVersionInfo.CodiFullVersion));
        }

        public void Initialize(string connectionParametersXml, string configurationParametersXml)
        {
            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerRecordingAPI.Initialize",
                string.Format("connectionParametersXml={0}, configurationParametersXml={1}",
                connectionParametersXml, configurationParametersXml));

            _connectionParameters = new ConnectionParameters(connectionParametersXml);

            string authorizationKeyForOutgoingRequests;

            using (var ecryptor = new DialerAuthorizationKeyEncryptor())
            {
                authorizationKeyForOutgoingRequests = ecryptor.DecryptString(_connectionParameters.AuthorizationKeyForOutgoingRequests);
                ecryptor.Clear();
            }

            CreateRecordingCodiWsProxy(authorizationKeyForOutgoingRequests);
        }

        /// <summary>
        /// Retrieves recordings URLs and returns them as a list of AudioRecordInfo objects.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="interviewId"></param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns>List of AudioRecordInfo objects. AudioRecordInfo.dateTime field contains UTC time</returns>
        public IEnumerable<AudioRecordInfo> GetAudioRecords(int companyId, long surveyId, int interviewId, int dialerId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, surveyId={1}, interviewId={2}, dialerId={3}",
                companyId, surveyId, interviewId, dialerId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerRecordingAPI.GetAudioRecords", argumentsAsString);

            return Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsRecordingProxy.GetAudioRecords(
                    companyId,
                    surveyId,
                    interviewId,
                    dialerId)),
                    new AudioRecordInfo[0]).ToList();
        }

        /// <summary>
        /// Gets the interview audio recording file
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="audioUrl">Url to the audio file which was returned by GetAudioRecords method</param>
        /// <returns>An object with the content of audio file</returns>
        public AudioFile GetAudioFile(int companyId, int dialerId, string audioUrl)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, dialerId={1}, audioUrl={2}",
                companyId, dialerId, audioUrl);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerRecordingAPI.GetAudioFile", argumentsAsString);

            return Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsRecordingProxy.GetAudioFile(
                    companyId,
                    dialerId,
                    audioUrl)),
                new AudioFile());
        }

        /// <summary>
        /// Gets audio records for given collection of interviews.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="interviewIndentities">Collection of interview identities.</param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns>Audio data.</returns>
        public BulkAudioResult GetBulkAudioRecords(int companyId, IEnumerable<CampaignInterviewIdentity> interviewIndentities, int dialerId)
        {
            var campaignInterviewIdentities = interviewIndentities as CampaignInterviewIdentity[] ??
                                              interviewIndentities.ToArray();

            var argumentsAsString = string.Format(
                "companyId={0}, interviewIdentities count={1}, dialerId={2}",
                companyId, campaignInterviewIdentities.Count(), dialerId);

            logger.WriteLine(
                TraceEventType.Verbose,
                "DialerLibrary.IDialerRecordingAPI.GetBulkAudioRecords", argumentsAsString);

            return Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsRecordingProxy.GetBulkAudioRecords(
                    companyId,
                    campaignInterviewIdentities,
                    dialerId)),
                    new BulkAudioResult());
        }

        /// <summary>
        /// Gets the list of boolean flags indicating whether there are some recordings are available for the specific interview ID.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <param name="surveyId">Campaign ID.</param>
        /// <param name="interviewIds">The list of interview IDs to determine whether recordings are available for.</param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns>
        /// The list of boolean flags. Flags count is always equal to the count of interview IDs list.
        /// </returns>
        public bool[] AreRecordsExists(int companyId, long surveyId, int[] interviewIds, int dialerId)
        {
            var argumentsAsString = string.Format(
                "companyId={0}, surveyId={1}, interviewIds=[{2}], dialerId={3}",
                companyId, surveyId, string.Join(", ", interviewIds.Select(s => s.ToString()).ToArray()), dialerId);

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.IDialerRecordingAPI.IDialerRecordingAPI.", argumentsAsString);

            return Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsRecordingProxy.AreRecordsExists(
                companyId,
                surveyId,
                interviewIds,
                dialerId)),
                Enumerable.Repeat(false, interviewIds.Count())).ToArray();
        }

        public int RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            var argumentsAsString = $"companyId={companyId}, dialerId={dialerId}, agentId={agentId}, agentName={agentName}";

            logger.WriteLine(TraceEventType.Verbose, "DialerLibrary.RegisterAgentSoftphone", argumentsAsString);

            var outLogin = "";
            var outPassword = "";
            var outHost = "";
            var outExtension = "";
            var outFrontendUrl = "";

            var result = (int)Execute(argumentsAsString, () => DoDialerServiceCall(() => _codiWsCoreProxy.RegisterAgentSoftphone(
                companyId, dialerId, agentId, agentName, out outLogin, out outPassword, out outHost, out outExtension, out outFrontendUrl)));

            login = outLogin;
            password = outPassword;
            host = outHost;
            extension = outExtension;
            frontendUrl = outFrontendUrl;

            return result;
        }
    }
}
