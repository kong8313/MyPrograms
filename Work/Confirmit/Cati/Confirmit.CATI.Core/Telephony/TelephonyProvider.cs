using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using BvCallHandlerLibrary;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;
using DialerCommon.DialerExceptions;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging.DialerActivityLogging;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Core.Telephony
{
    public class TelephonyProvider : ITelephony
    {
        private readonly Lazy<IDialerCollection> _dialerCollection;

        private readonly Lazy<IInstanceInfo> _instanceInfo;
        private readonly Lazy<IProblemStateSetter> _problemStateSetter;
        private readonly Lazy<IDialerAvailabilityManager> _dialerAvailabilityManager;
        private readonly Lazy<IDialerRecordingWrapper> _dialerRecordingWrapper;
        private readonly Lazy<IDialerOperationalStateNotificator> _dialerOperationalStateNotificator;
        private readonly Lazy<ISurveyRepository> _surveyRepository;
        private readonly Lazy<IDialerSettings> _dialerSettings;

        public TelephonyProvider()
        {
            _instanceInfo = new Lazy<IInstanceInfo>(() => ServiceLocator.Resolve<IInstanceInfo>());
            _problemStateSetter = new Lazy<IProblemStateSetter>(() => ServiceLocator.Resolve<IProblemStateSetter>());
            _dialerAvailabilityManager = new Lazy<IDialerAvailabilityManager>(() => ServiceLocator.Resolve<IDialerAvailabilityManager>());
            _dialerCollection = new Lazy<IDialerCollection>(() => ServiceLocator.Resolve<IDialerCollection>());
            _dialerRecordingWrapper = new Lazy<IDialerRecordingWrapper>(() => ServiceLocator.Resolve<IDialerRecordingWrapper>());
            _dialerOperationalStateNotificator = new Lazy<IDialerOperationalStateNotificator>(() => ServiceLocator.Resolve<IDialerOperationalStateNotificator>());
            _surveyRepository = new Lazy<ISurveyRepository>(() => ServiceLocator.Resolve<ISurveyRepository>());
            _dialerSettings = new Lazy<IDialerSettings>(() => ServiceLocator.Resolve<IDialerSettings>());
        }

        private void EnsureIsExecutedInBackendInstance()
        {
            if (!_instanceInfo.Value.IsExecutedInBackendInstance)
            {
                throw new InvalidOperationException("Telephony can't be used out of the backend service context.");
            }
        }

        //TODO: The only usage of this method is from BvDialersTrigger. Call the _dialerCollection.Value.UpdateCollection() from there directly?
        public void UpdateDialersCollection()
        {
            EnsureIsExecutedInBackendInstance();

            _dialerCollection.Value.InitializeCollection();
        }

        public void InitializeDialers()
        {
            EnsureIsExecutedInBackendInstance();

            _dialerCollection.Value.InitializeCollection();
        }

        public void UninitializeDialers(bool releaseDialerWs)
        {
            EnsureIsExecutedInBackendInstance();

            if (!_dialerCollection.Value.InitializedDialerExists())
            {
                return;
            }

            foreach (var dialer in _dialerCollection.Value.GetDialers())
            {
                if (_dialerCollection.Value.IsDialerInitialized(dialer.DialerId))
                {
                    dialer.Uninitialize(releaseDialerWs);
                }
                else
                {
                    Trace.TraceWarning(
                        "TelephonyProvider.UninitializeDialers: Dialer[{0}] is unavailable, so Uninitialize() command is not called.",
                        dialer.DialerId);
                }
            }
        }

        public DialerErrorCode DoDialerCall(Func<int> delegatedCall, IDialerInstance dialer, string debugInfo,
            DialerActivityEvent logEvent = null)
        {
            logEvent?.Parameters(debugInfo);

            try
            {
                VerifyDialerInitialized(dialer, delegatedCall.Method.ToString());

                EventDetailsScope.Current.AddTiming("DoDialerCall:IsDialerInitialized");

                if (dialer.Api == null)
                {
                    throw new InternalErrorException(
                        $"TelephonyProvider.DoDialerCall: Dialer [id={dialer.DialerId}, tenantId={dialer.TenantId}] interface is [null] on {delegatedCall.Method} call");
                }

                var result = (DialerErrorCode)delegatedCall();

                EventDetailsScope.Current.AddTiming("DoDialerCall:delegatedCall");

                if (result != DialerErrorCode.Success)
                {
                    TraceDialerErrorCode(dialer.DialerId, delegatedCall.Method, result, debugInfo);
                    EventDetailsScope.Current.AddTiming("DoDialerCall:TraceDialerErrorCode");

                    logEvent?.LogError(result);
                }
                else
                {
                    logEvent?.LogInfo(result);
                }

                return result;
            }
            catch (DialerWsNotInitializedException ex)
            {
                Trace.TraceError("TelephonyProvider.DoDialerCall<{0}>: {1} /// dialerId={2}",
                    delegatedCall.Method, ex, dialer.DialerId);

                EventDetailsScope.Current.AddTiming("DoDialerCall:TraceError DialerWsNotInitializedException");

                _dialerAvailabilityManager.Value.DisableDialer(dialer.DialerId);

                EventDetailsScope.Current.AddTiming("DoDialerCall:DisableDialer");

                logEvent?.LogError(DialerErrorCode.Exception, ex);

                return DialerErrorCode.Exception;
            }
            catch (DialerParametersException ex)
            {
                Trace.TraceWarning("TelephonyProvider.DoDialerCall<{0}>: {1} /// dialerId={2}",
                    delegatedCall.Method, ex, dialer.DialerId);

                EventDetailsScope.Current.AddTiming("DoDialerCall:TraceWarning DialerParametersException");

                logEvent?.LogError(DialerErrorCode.Exception, ex);

                throw;
            }
            catch (Exception ex)
            {
                Trace.TraceError("TelephonyProvider.DoDialerCall<{0}>: {1} /// dialerId={2}",
                    delegatedCall.Method, ex, dialer.DialerId);

                EventDetailsScope.Current.AddTiming("DoDialerCall:TraceError Unexpected exception");

                logEvent?.LogError(DialerErrorCode.Exception, ex);

                // TODO: We need to switch later from error codes to exception handling
                // But currently we should return error codes. The only exception is 
                // the DialerParametersException - see above
                //                 throw new InternalErrorException(strError);
                return DialerErrorCode.Exception;
            }
        }

        private static void TraceDialerErrorCode(int dialerId, MethodInfo method, DialerErrorCode dialerErrorCode, string debugInfo)
        {
            if (DialerErrorSeverityProvider.IsWarning(dialerErrorCode))
            {
                Trace.TraceWarning(
                    "IDialerAPI.{0}. Call is failed with error code: {1} /// dialerId={2}, {3}", method, dialerErrorCode, dialerId, debugInfo);
            }
            else
            {
                Trace.TraceError(
                    "IDialerAPI.{0}. Call is failed with error code: {1} /// dialerId={2}, {3}", method, dialerErrorCode, dialerId, debugInfo);
            }
        }

        public TResult DoDialerCall<TResult>(Func<TResult> delegatedCall, IDialerAPI dialerApi,
            DialerActivityEvent logEvent = null)
        {
            if (dialerApi == null)
            {
                var ex = new InternalErrorException(
                    $"Dialer is not configured properly or Telephony is not enabled for Company. Dialer interface is [null] on {delegatedCall.Method} call");

                logEvent?.LogError(ex);

                throw ex;
            }

            try
            {
                var result = delegatedCall();

                if ((object)result is DialerErrorCode resCode)
                {
                    if (resCode != DialerErrorCode.Success)
                    {
                        logEvent?.LogError(resCode);

                        return result;
                    }
                }

                logEvent?.LogInfo(result);

                return result;
            }
            catch (DialerParametersException ex)
            {
                var strError =
                    $"TelephonyProvider.DoDialerCall: DialerParametersException exception on {delegatedCall.Method} dialer call";
                Trace.TraceWarning("{0}: {1}", strError, ex);

                logEvent?.LogError(ex);

                throw;
            }
            catch (Exception ex)
            {
                var strError = $"Unexpected exception on {delegatedCall.Method} dialer call";
                Trace.TraceError("{0}: {1}", strError, ex);

                logEvent?.LogError(ex);

                throw new InternalErrorException(strError);
            }
        }

        public CallOutcome TranslateOutcome(long outcome)
        {
            var dialerApi = _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.TranslateOutcome(outcome), dialerApi);
        }

        public bool IsHangUpSupported()
        {
            EventDetailsScope.Current.AddTiming("Begin IsHangUpSupported");

            var dialerApi = _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.IsHangUpSupported, dialerApi);
        }

        public bool IsPersonModeSupported(AgentTaskChoiceMode personMode, int? dialerId = null)
        {
            EventDetailsScope.Current.AddTiming("Begin IsPersonModeSupported");

            var dialerApi = dialerId.HasValue
                ? _dialerCollection.Value.GetDialerById(dialerId.Value).Api
                : _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.IsPersonModeSupported(personMode), dialerApi);
        }

        public bool IsReloginNeededOnSurveyChange(int? dialerId = null)
        {
            EventDetailsScope.Current.AddTiming("Begin IsReloginNeededOnSurveyChange");

            var dialerApi = dialerId.HasValue
                ? _dialerCollection.Value.GetDialerById(dialerId.Value).Api
                : _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.IsReloginNeededOnSurveyChange(), dialerApi);
        }

        public bool IsDynamicExtensionNumberAllowed(bool isAgentLocal, int? dialerId = null)
        {
            EventDetailsScope.Current.AddTiming("Begin IsDynamicExtensionNumberAllowed");

            var dialerApi = dialerId.HasValue
                ? _dialerCollection.Value.GetDialerById(dialerId.Value).Api
                : _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.IsDynamicExtensionNumberAllowed(isAgentLocal), dialerApi);
        }

        public DialerErrorCode SetConfigurationParameters(int dialerId, string configurationParametersXml)
        {
            EventDetailsScope.Current.AddTiming("Begin SetConfigurationParameters");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, configurationParametersXml={configurationParametersXml}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SetConfigurationParameters(
                    dialerTenantId,
                    configurationParametersXml),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialerId)
            );
        }

        public DialerErrorCode ValidateCampaignParameters(string surveyParametersXml)
        {
            EventDetailsScope.Current.AddTiming("Begin ValidateCampaignParameters");

            var dialerApi = _dialerCollection.Value.FirstLoadedDialerApi;

            return (DialerErrorCode)DoDialerCall(
                () => dialerApi.ValidateCampaignParameters(surveyParametersXml), dialerApi);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campaignId"></param>
        /// <param name="dialingMode"></param>
        /// <param name="surveyParametersXml">
        ///   Note! 
        ///   The same parameters go to the all dialer instances. And the same parameters check procedure is applied.
        ///   So in case of DialerParametersException there is no sense to continue the foreach loop below. 
        /// </param>
        public void SetCampaignParameters(long campaignId, DialingMode dialingMode, string surveyParametersXml)
        {
            SetCampaignParameters(DialType.Landline, campaignId, dialingMode, surveyParametersXml);
            SetCampaignParameters(DialType.Cellphone, campaignId, DialingMode.Preview, surveyParametersXml);
            SetCampaignParameters(DialType.Assisted, campaignId, dialingMode, surveyParametersXml);
        }

        private void SetCampaignParameters(DialType dialType, long campaignId, DialingMode dialingMode, string surveyParametersXml)
        {
            EventDetailsScope.Current.AddTiming("Begin SetCampaignParameters");

            var dialerIds = _dialerCollection.Value.GetDialerIds(dialType);

            if (!dialerIds.Any())
                return;

            var initializedDialers = _dialerCollection.Value.GetInitializedDialers(dialType).ToList();

            if (!initializedDialers.Any())
            {
                Trace.TraceWarning(
                    "TelephonyProvider.SetCampaignParameters: Campaign [id={0}] parameters are not sent to dialer. There is no initialized dialer. " +
                    "/// dialingMode={1}, surveyParametersXml=[{2}]",
                    campaignId, dialingMode, surveyParametersXml);

                EventDetailsScope.Current.AddTiming("End SetCampaignParameters");
                return;
            }

            if (_dialerSettings.Value.OpenSurveysOnDialersIndividually)
            {
                foreach (var initializedDialer in initializedDialers)
                {
                    SetCampaignParametersForDialer(initializedDialer, new[] { initializedDialer.DialerId }, campaignId, dialingMode, surveyParametersXml);
                }
            }
            else
            {
                var initializedDialer = initializedDialers.First();
                SetCampaignParametersForDialer(initializedDialer, dialerIds, campaignId, dialingMode, surveyParametersXml);
            }

            EventDetailsScope.Current.AddTiming("End SetCampaignParameters");
        }

        private void SetCampaignParametersForDialer(IDialerInstance initializedDialer, int[] dialerIds, long campaignId, DialingMode dialingMode, string surveyParametersXml)
        {
            var tenantId = initializedDialer.TenantId;

            var argumentsAsString =
                $"targetDialerId={initializedDialer.DialerId}, targetTenantId={tenantId}, dialerIds=[{string.Join(", ", dialerIds)}], " +
                $"campaignId={campaignId}, dialingMode={dialingMode}, surveyParametersXml={surveyParametersXml}";

            var surveyEntity = _surveyRepository.Value.GetByCampaignId(campaignId);

            var result = DoDialerCall(
                () => initializedDialer.Api.SetCampaignParameters(
                    tenantId,
                    dialerIds,
                    campaignId,
                    dialingMode,
                    surveyEntity.RecWholeInt > 0,
                    surveyParametersXml),
                initializedDialer,
                argumentsAsString,
                new DialerActivityEvent(initializedDialer.DialerId).Survey(surveyEntity.ProjectId, surveyEntity.SID));

            if (result != DialerErrorCode.Success)
            {
                Trace.TraceWarning(
                    "TelephonyProvider.SetCampaignParameters: Campaign ['{0}', id={1}, sid={2}] set parameters error: [{3}]. " +
                    "/// Dialer=['{4}', (id={5})], campaignId={6}, dialerIds=[{7}], dialingMode={8}, surveyParametersXml={9}",
                    surveyEntity.Name, campaignId, surveyEntity.SID, result,
                    initializedDialer.DialerName, initializedDialer.DialerId, campaignId, string.Join(", ", dialerIds), dialingMode, surveyParametersXml);
            } 
        }

        /// <summary>
        /// </summary>
        /// <param name="campaignId">
        /// </param>
        /// <param name="campaignName">
        /// </param>
        /// <param name="dialingMode">
        /// </param>
        /// <param name="campaignType">
        /// </param>
        /// <param name="surveyParametersXml">
        ///   Note! 
        ///   The same parameters go to the all dialer instances. And the same parameters check procedure is applied.
        ///   So in case of DialerParametersException there is no sense to continue the foreach loop below. 
        /// </param>
        /// <returns>
        /// Array of dialer erros, it is empty if there rea no errors.
        /// </returns>
        public ICollection<DialerStartCampaignResult> StartCampaign(long campaignId, string campaignName, DialingMode dialingMode, string campaignType, string surveyParametersXml)
        {
            var result = new List<DialerStartCampaignResult>();

            result.AddRange(StartCampaign(DialType.Landline, campaignId, campaignName, dialingMode, campaignType, surveyParametersXml));
            result.AddRange(StartCampaign(DialType.Cellphone, campaignId, campaignName, DialingMode.Preview, campaignType, surveyParametersXml));
            result.AddRange(StartCampaign(DialType.Assisted, campaignId, campaignName, dialingMode, campaignType, surveyParametersXml));

            return result;
        }

        private ICollection<DialerStartCampaignResult> StartCampaign(DialType dialType, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, string surveyParametersXml)
        {
            EventDetailsScope.Current.AddTiming("Begin StartCampaign");

            var result = new List<DialerStartCampaignResult>();

            var dialerIds = _dialerCollection.Value.GetDialerIds(dialType);

            if (!dialerIds.Any()) 
                return result;

            var initializedDialers = new List<IDialerInstance>();

            foreach (var dialerInstance in _dialerCollection.Value.GetDialers(dialType))
            {
                if (_dialerCollection.Value.IsDialerInitialized(dialerInstance.DialerId))
                {
                    initializedDialers.Add(dialerInstance);
                }
                else
                {
                    result.Add(new DialerStartCampaignResult() {
                        ErrorCode = DialerErrorCode.NotAvailable, DialerId = dialerInstance.DialerId, DialerName = dialerInstance.DialerName
                    });
                }
            }

            if (!initializedDialers.Any())
            {
                Trace.TraceWarning("TelephonyProvider.StartCampaign: Campaign ['{0}', id={1}] is not started on dialer. There is no initialized dialer." + "/// dialingMode={2}, campaignType={3}, surveyParametersXml=[{4}]", 
                    campaignName, campaignId, dialingMode, campaignType, surveyParametersXml);

                EventDetailsScope.Current.AddTiming("End StartCampaign");

                return result;
            }

            if (_dialerSettings.Value.OpenSurveysOnDialersIndividually)
            {
                foreach (var initializedDialer in initializedDialers)
                {
                    result.AddRange(StartCampaignForDialer(initializedDialer, new[] { initializedDialer.DialerId }, campaignId, campaignName, dialingMode, campaignType, surveyParametersXml));
                }
            }
            else
            {
                var initializedDialer = initializedDialers.Last();
                result.AddRange(StartCampaignForDialer(initializedDialer, dialerIds, campaignId, campaignName, dialingMode, campaignType, surveyParametersXml));
            }

            EventDetailsScope.Current.AddTiming("End StartCampaign");

            return result;
        }

        private List<DialerStartCampaignResult> StartCampaignForDialer(IDialerInstance initializedDialer, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, string surveyParametersXml)
        {
            var result = new List<DialerStartCampaignResult>();

            var tenantId = initializedDialer.TenantId;

            var argumentsAsString = $"targetDialerId={initializedDialer.DialerId}, targetDialerTenantId={tenantId}, dialerIds=[{string.Join(", ", dialerIds)}], " + $"campaignId={campaignId}, dialingMode={dialingMode}, campaignType={campaignType}, surveyParametersXml={surveyParametersXml}";

            var surveyEntity = _surveyRepository.Value.GetByCampaignId(campaignId);

            var startCampaignResult =
                DoDialerCall(() => initializedDialer.Api.StartCampaign(
                        tenantId,
                        dialerIds,
                        campaignId,
                        campaignName,
                        dialingMode,
                        campaignType,
                        surveyEntity.RecWholeInt > 0,
                        surveyParametersXml),
                    initializedDialer,
                    argumentsAsString,
                    new DialerActivityEvent(initializedDialer.DialerId).Survey(surveyEntity.ProjectId, surveyEntity.SID));

            result.Add(new DialerStartCampaignResult() {
                ErrorCode = startCampaignResult, DialerId = initializedDialer.DialerId, DialerName = initializedDialer.DialerName
            });

            return result;
        }

        public void StopCampaign(long campaignId, DialingMode dialingMode)
        {
            StopCampaign(DialType.Landline, campaignId, dialingMode);
            StopCampaign(DialType.Cellphone, campaignId, DialingMode.Preview);
            StopCampaign(DialType.Assisted, campaignId, dialingMode);
        }

        private void StopCampaign(DialType dialType, long campaignId, DialingMode dialingMode)
        {
            EventDetailsScope.Current.AddTiming("Begin StopCampaign");

            var dialerIds = _dialerCollection.Value.GetDialerIds(dialType);

            if (!dialerIds.Any()) return;

            var initializedDialers = _dialerCollection.Value.GetInitializedDialers(dialType).ToList();

            if (!initializedDialers.Any())
            {
                Trace.TraceWarning("TelephonyProvider.StopCampaign: Campaign [id={0}] is not stopped on dialer. There is no initialized dialer." + "/// dialingMode={1}", 
                    campaignId, dialingMode);

                EventDetailsScope.Current.AddTiming("End StopCampaign");
                return;
            }

            if (_dialerSettings.Value.OpenSurveysOnDialersIndividually)
            {
                foreach (var initializedDialer in initializedDialers)
                {
                    StopCampaignForDialer(initializedDialer, new[] { initializedDialer.DialerId }, campaignId, dialingMode);
                }
            }
            else
            {
                var initializedDialer = initializedDialers.First();
                StopCampaignForDialer(initializedDialer, dialerIds, campaignId, dialingMode);
            }

            EventDetailsScope.Current.AddTiming("End StopCampaign");
        }

        private void StopCampaignForDialer(IDialerInstance initializedDialer, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            var dialerTenantId = initializedDialer.TenantId;

            var argumentsAsString = $"targetDialerId={initializedDialer.DialerId}, targetDialerTenantId={dialerTenantId}, " + $"dialerIds=[{string.Join(", ", dialerIds)}], campaignId={campaignId}, dialingMode={dialingMode}";

            var stopCampaignResult = DoDialerCall(
                () => initializedDialer.Api.StopCampaign(
                    dialerTenantId, 
                    dialerIds, 
                    campaignId, 
                    dialingMode), 
                    initializedDialer, 
                    argumentsAsString, 
                    new DialerActivityEvent(initializedDialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId))
                );

            if (stopCampaignResult != DialerErrorCode.Success)
            {
                Trace.TraceWarning("TelephonyProvider.StopCampaign: Campaign [id={0}] stop error: [{1}]. /// Dialer=['{2}', (id={3})], dialingMode={4}.", 
                    campaignId, 
                    stopCampaignResult, 
                    initializedDialer.DialerName, 
                    initializedDialer.DialerId, 
                    dialingMode);
            }
        }

        public void KillCampaign(long campaignId, DialingMode dialingMode)
        {
            KillCampaign(DialType.Landline, campaignId, dialingMode);
            KillCampaign(DialType.Cellphone, campaignId, DialingMode.Preview);
            KillCampaign(DialType.Assisted, campaignId, dialingMode);
        }
        
        public void KillCampaign(DialType dialType, long campaignId, DialingMode dialingMode)
        {
            EventDetailsScope.Current.AddTiming("Begin KillCampaign");

            var dialerIds = _dialerCollection.Value.GetDialerIds(dialType);

            if (!dialerIds.Any())
                return;

            var initializedDialers = _dialerCollection.Value.GetInitializedDialers(dialType).ToList();

            if (!initializedDialers.Any())
            {
                Trace.TraceWarning(
                    "TelephonyProvider.KillCampaign: Campaign [id={0}] is not killed on dialer. There is no initialized dialer." +
                    "/// dialingMode={1}",
                    campaignId, dialingMode);

                EventDetailsScope.Current.AddTiming("End KillCampaign");
                return;
            }

            if (_dialerSettings.Value.OpenSurveysOnDialersIndividually)
            {
                foreach (var initializedDialer in initializedDialers)
                {
                    KillCampaignForDialer(initializedDialer, new[] { initializedDialer.DialerId }, campaignId, dialingMode);
                }
            }
            else
            {
                var initializedDialer = initializedDialers.First();
                KillCampaignForDialer(initializedDialer, dialerIds, campaignId, dialingMode);
            }

            EventDetailsScope.Current.AddTiming("End KillCampaign");
        }

        private void KillCampaignForDialer(IDialerInstance initializedDialer, int[] dialerIds, long campaignId, DialingMode dialingMode)
        {
            var dialerTenantId = initializedDialer.TenantId;

            var argumentsAsString =
                $"targetDialerId={initializedDialer.DialerId}, targetDialerTenantId={dialerTenantId}, " +
                $"dialerIds=[{string.Join(", ", dialerIds)}], campaignId={campaignId}, dialingMode={dialingMode}";

            var killCampaignResult = DoDialerCall(
                () => initializedDialer.Api.KillCampaign(
                    dialerTenantId,
                    dialerIds,
                    campaignId,
                    dialingMode),
                initializedDialer,
                argumentsAsString,
                new DialerActivityEvent(initializedDialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)));

            if (killCampaignResult != DialerErrorCode.Success)
            {
                Trace.TraceWarning(
                    "TelephonyProvider.KillCampaign: Campaign [id={0}] kill error: [{1}]. /// Dialer=['{2}', (id={3})], dialingMode={4}.",
                    campaignId,
                    killCampaignResult,
                    initializedDialer.DialerName,
                    initializedDialer.DialerId,
                    dialingMode);
            }
        }

        public DialerErrorCode Login(
            int dialerId,
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
            EventDetailsScope.Current.AddTiming("Begin Login");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"agentName={agentName}, agentType={agentType}, agentExtension={agentExtension}, userId={userId}, isPredictive={isPredictive}, isLocal={isLocal}, " +
                $"agentAttributes={agentAttributes.Aggregate("", (current, agentAttribute) => current + agentAttribute.ToString())}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.Login(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    agentName,
                    agentType,
                    agentExtension,
                    userId,
                    isPredictive,
                    isLocal,
                    agentAttributes),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );
        }

        /// <summary>
        /// Switches campaign on dialer
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <returns>Either return DialerErrorCode.Success or throws Exception</returns>
        public DialerErrorCode SetCampaign(int dialerId, long campaignId, int agentId)
        {
            EventDetailsScope.Current.AddTiming("Begin SetCampaign");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var companyId = int.Parse(dialer.TenantId); //TODO: Needs to be refactored - TenantId should be an 'int'

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={companyId}, campaignId={campaignId}, agentId={agentId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            var result = DoDialerCall(
                () => dialer.Api.SetCampaign(
                    companyId,
                    campaignId,
                    agentId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );

            // In order to simplify error processing at upper levels
            if (result != DialerErrorCode.Success)
            {
                throw new DialerException(result, $"SetCampaign is failed with error code [{result}]");
            }

            return result;
        }

        public DialerErrorCode Logout(int dialerId, long campaignId, bool isPredictive, string agentId)
        {
            EventDetailsScope.Current.AddTiming("Begin Logout");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, isPredictive={isPredictive}, agentId={agentId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.Logout(
                    dialerTenantId,
                    campaignId,
                    isPredictive,
                    agentId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );
        }

        public DialerErrorCode KillAgent(int dialerId, long campaignId, string agentId)
        {
            EventDetailsScope.Current.AddTiming("Begin KillAgent");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.KillAgent(
                    dialerTenantId,
                    campaignId,
                    agentId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );
        }

        public DialerErrorCode GoReady(int dialerId, long campaignId, string agentId)
        {
            EventDetailsScope.Current.AddTiming("Begin GoReady");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.GoReady(
                    dialerTenantId,
                    campaignId,
                    agentId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );
        }

        public DialerErrorCode GoNotReady(int dialerId, long campaignId, string agentId, string breakName)
        {
            EventDetailsScope.Current.AddTiming("Begin GoNotReady");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, breakName={breakName}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.GoNotReady(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    breakName),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );
        }

        public DialerErrorCode SendNumber(int dialerId, long campaignId, string agentId, DialingMode dialingMode,
            int groupId, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {
            EventDetailsScope.Current.AddTiming("Begin SendNumber");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, dialingMode={dialingMode}, groupId={groupId}, " +
                $"contactId={contactId}, callId={callId}, phoneNumber={phoneNumber}, callAgingTimeout={callAgingTimeout}, isRecording={isRecording}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SendNumber(
                    dialerTenantId,
                    campaignId,
                    dialingMode,
                    groupId,
                    contactId,
                    callId,
                    phoneNumber,
                    callAgingTimeout,
                    isRecording),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(contactId)
            );
        }

        public DialerErrorCode SendNumberToAgent(
            int dialerId,
            long campaignId,
            string agentId,
            DialingMode dialingMode,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording,
            string callerId,
            Dictionary<string, object> respondentVariables)
        {
            EventDetailsScope.Current.AddTiming("Begin SendNumberToAgent");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, dialingMode={dialingMode}, " +
                $"contactId={contactId}, callId={callId}, phoneNumber={phoneNumber}, isRecording={isRecording}, callerId = {callerId}, respondentVariables = {respondentVariables?.Stringify()}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SendNumberToAgent(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    dialingMode,
                    contactId,
                    callId,
                    phoneNumber,
                    isRecording,
                    callerId,
                    respondentVariables),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(contactId)
            );
        }

        public DialerErrorCode Redial(
            int dialerId,
            long campaignId,
            string agentId,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording,
            string callerId)
        {
            EventDetailsScope.Current.AddTiming("Begin Redial");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"contactId={contactId}, callId={callId}, phoneNumber={phoneNumber}, isRecording={isRecording}, callerId = {callerId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.Redial(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    contactId,
                    callId,
                    phoneNumber,
                    isRecording,
                    callerId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(contactId)
            );
        }

        public DialerErrorCode SendNumberToAgentEx(int dialerId, long campaignId, string agentId, DialingMode dialingMode, int contactId, int callId, string phoneNumber, int callAgingTimeout, bool isRecording)
        {
            EventDetailsScope.Current.AddTiming("Begin SendNumberToAgentEx");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, dialingMode={dialingMode}, " +
                $"contactId={contactId}, callId={callId}, phoneNumber={phoneNumber}, callAgingTimeout={callAgingTimeout}, isRecording={isRecording}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SendNumberToAgentEx(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    dialingMode,
                    contactId,
                    callId,
                    phoneNumber,
                    callAgingTimeout,
                    isRecording),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(contactId)
            );
        }

        public DialerErrorCode SendNumbers(
            int dialerId,
            string requestId,
            long campaignId,
            DialingMode campaignDiallingMode,
            List<CallInfo> callList,
            int callAgingTimeout,
            bool isRecording)
        {
            EventDetailsScope.Current.AddTiming("Begin SendNumbers");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, requestId={requestId}, tenantId={dialerTenantId}, campaignId={campaignId}, " +
                $"campaignDiallingMode={campaignDiallingMode}, numberOfCalls={callList.Count}, " +
                $"callAgingTimeout={callAgingTimeout}, isRecording={isRecording}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SendNumbers(
                    requestId,
                    dialerTenantId,
                    campaignId,
                    campaignDiallingMode,
                    callList,
                    callAgingTimeout,
                    isRecording),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId))
                    .Parameters(argumentsAsString)
                    .Detail(nameof(callList), callList)
            );
        }

        public DialerErrorCode Hangup(int dialerId, long campaignId, string agentId, int contactId, long callId)
        {
            EventDetailsScope.Current.AddTiming("Begin Hangup");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, interviewId={contactId}, callId={callId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.Hangup(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    contactId,
                    callId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode CompleteCall(int dialerId, long campaignId, string agentId, int contactId, 
            bool makeAgentReady, string breakName, InterviewStatus its, long callId)
        {
            EventDetailsScope.Current.AddTiming("Begin CompleteCall");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, makeAgentReady={makeAgentReady}, " +
                $"breakName={(makeAgentReady ? "NULL" : breakName)}, interviewId={contactId}, callId={callId}, its={its}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.CompleteCall(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    its,
                    makeAgentReady,
                    breakName,
                    contactId,
                    callId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode SetNextInterview(int dialerId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            EventDetailsScope.Current.AddTiming("Begin SetNextInterview");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, currentCampaignId={currentCampaignId}, agentId={agentId}, " +
                $"currentInterviewStatus={currentInterviewStatus}, nextCampaignId={nextCampaignId}, nextInterviewId={nextInterviewId}, nextCallId={nextCallId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SetNextInterview(
                    dialerTenantId,
                    currentCampaignId,
                    agentId,
                    currentInterviewStatus,
                    nextCampaignId,
                    nextInterviewId,
                    nextCallId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(nextCampaignId)).Interviewer(agentId).Interview(nextInterviewId)
            );
        }

        public DialerErrorCode StartCustomIvrInterview(int dialerId, long campaignId, string agentId, int interviewId, long callId, string respondentSurveyLink)
        {
            EventDetailsScope.Current.AddTiming("Begin StartCustomIvrInterview");
            
            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;
            
            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"interviewId={interviewId}, callId={callId}, respondentSurveyLink={respondentSurveyLink}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.StartCustomIvrInterview(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    respondentSurveyLink),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(interviewId)
            );
        }

        public DialerErrorCode UpdateInterviewStatus(
            int dialerId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            InterviewStatus interviewStatus)
        {
            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"interviewId={interviewId}, callId={callId}, interviewStatus={interviewStatus}";

            return DoDialerCall(
                () => dialer.Api.UpdateInterviewStatus(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    interviewStatus),
                    dialer,
                    argumentsAsString
                );
        }

        public DialerErrorCode SetGroups(int dialerId, long campaignId, string agentId, int[] agentGroups)
        {
            EventDetailsScope.Current.AddTiming("Begin SetGroups");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, " +
                $"agentId={agentId}, agentGroups=[{string.Join(",", agentGroups)}]";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SetGroups(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    agentGroups),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                );
        }

        public void FlushNumbers(long campaignId, List<CallInfo> callsList)
        {
            EventDetailsScope.Current.AddTiming("Begin FlushNumbers");

            var dialerIds = _dialerCollection.Value.GetDialerIds(DialType.Landline);

            if (!dialerIds.Any())
                return;

            var initializedDialer = _dialerCollection.Value.GetFirstInitializedDialer(DialType.Landline);

            if (initializedDialer == null)
            {
                Trace.TraceWarning(
                    "TelephonyProvider.FlushNumbers: There is no initialized dialer." +
                    "/// campaignId={0}, numberOfCalls={1}",
                    campaignId, callsList.Count);

                EventDetailsScope.Current.AddTiming("End FlushNumbers");
                return;
            }

            var dialerTenantId = initializedDialer.TenantId;


            var argumentsAsString =
                $"targetDialerId={initializedDialer.DialerId}, targetDialerTenantId={dialerTenantId}, " +
                $"dialerIds=[{string.Join(", ", dialerIds)}], campaignId={campaignId}, numberOfCalls={callsList.Count}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            var flushNumbersResult = DoDialerCall(
                () => initializedDialer.Api.FlushNumbers(
                    dialerTenantId,
                    dialerIds,
                    campaignId,
                    callsList),
                initializedDialer,
                argumentsAsString,
                new DialerActivityEvent(initializedDialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)));

            if (flushNumbersResult != DialerErrorCode.Success)
            {
                Trace.TraceWarning(
                    "TelephonyProvider.FlushNumbers: FlushNumbers error: [{0}]. /// Dialer=['{1}', (id={2})], dialerIds=[{3}], campaignId={4}, numberOfCalls={5}.",
                    flushNumbersResult,
                    initializedDialer.DialerName,
                    initializedDialer.DialerId,
                    string.Join(", ", dialerIds),
                    campaignId,
                    callsList.Count);
            }

            EventDetailsScope.Current.AddTiming("End FlushNumbers");
        }

        public DialerErrorCode StartRecording(int dialerId, long campaignId, string agentId, int contactId, int callId, string label)
        {
            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, " +
                $"agentId={agentId}, contactId={contactId}, callId={callId}, label={label}";

            return DoDialerCall(
                () => dialer.Api.StartRecording(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    contactId,
                    callId,
                    label),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode StopRecording(int dialerId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode)
        {
            EventDetailsScope.Current.AddTiming("Begin StopRecording");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"contactId={contactId}, callId={callId}, stopRecordingMode={stopRecordingMode}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.StopRecording(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    contactId,
                    callId,
                    stopRecordingMode),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode StartMonitor(int dialerId, string agentId, string phoneNumber, ref string sessionId)
        {
            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, agentId={agentId}, phoneNumber={phoneNumber}, " +
                $"sessionId={sessionId}";

            string resultSessionId = sessionId;

            DialerErrorCode result = DoDialerCall(
                () => dialer.Api.StartMonitor(
                    dialerTenantId,
                    agentId,
                    phoneNumber,
                    ref resultSessionId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Interviewer(agentId)
                );

            sessionId = resultSessionId;
            Trace.TraceInformation("TelephonyProvider.StartMonitor: resultSessionId is {0} /// {1}", resultSessionId, argumentsAsString);

            return result;
        }

        public DialerErrorCode StopMonitor(int dialerId, string agentId, int contactId, string sessionId)
        {
            EventDetailsScope.Current.AddTiming("Begin StopMonitor");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString = $"dialerId={dialerId}, tenantId={dialerTenantId}, sessionId={sessionId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.StopMonitor(dialerTenantId, sessionId),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode SetMonitorMode(int dialerId, string agentId, string sessionId, MonitorMode monitorMode)
        {
            EventDetailsScope.Current.AddTiming("Begin SetMonitorMode");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString = $"dialerId={dialerId}, tenantId={dialerTenantId}, sessionId={sessionId}, monitorMode={monitorMode}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.SetMonitorMode(dialerTenantId, sessionId, monitorMode),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Interviewer(agentId)
                );
        }

        public DialerErrorCode CompletePreview(int dialerId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {
            EventDetailsScope.Current.AddTiming("Begin CompletePreview");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"contactId={contactId}, callId={callId}, phoneNumber={phoneNumber}, isRecording={isRecording}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.CompletePreview(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    contactId,
                    callId,
                    phoneNumber,
                    isRecording),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode TransferToIvr(
            int dialerId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes)
        {
            EventDetailsScope.Current.AddTiming("Begin TransferToIvr");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"interviewId={interviewId}, callId={callId}, endpoint={endpoint}, " +
                $"attributes=[{attributes.Aggregate("", (current, attribute) => current + attribute.ToString())}]";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.TransferToIvr(
                    dialerTenantId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    endpoint,
                    attributes),
                    dialer,
                    argumentsAsString);
        }

        public DialerErrorCode IvrRenderVoiceXml(int dialerId, int companyId, long campaignId, int agentId, int contactId, string voiceXml)
        {
            EventDetailsScope.Current.AddTiming("Begin IvrRenderVoiceXml");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, voiceXml=[{voiceXml}]";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.IvrRenderVoiceXml(
                    int.Parse(dialerTenantId),
                    campaignId,
                    agentId,
                    voiceXml),
                    dialer,
                    argumentsAsString,
                    new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
                );
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            EventDetailsScope.Current.AddTiming("Begin ConfigureInboundDdiNumbers");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            VerifyDialerInitialized(dialer, "ConfigureInboundDdiNumbers");

            EventDetailsScope.Current.AddTiming("DoDialerCall:IsDialerInitialized");

            return DoDialerCall(
                () => dialer.Api.ConfigureInboundDdiNumbers(
                    int.Parse(dialerTenantId),
                    inboundDdiNumbers),
                dialer.Api,
                new DialerActivityEvent(dialerId).Parameters($"{nameof(dialerId)}={dialerId}, {nameof(inboundDdiNumbers)}={inboundDdiNumbers.ToJson()}")
            );
        }

        public DialerErrorCode DropInboundCall(
            int dialerId,
            string inboundCallId,
            AudioMessageDescriptor audioMessageDescriptor)
        {
            EventDetailsScope.Current.AddTiming("Begin DropInboundCall");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, inboundCallId={inboundCallId}, audioMessageDescriptor={audioMessageDescriptor.NullableToString()}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.DropInboundCall(
                    int.Parse(dialerTenantId),
                    inboundCallId,
                    audioMessageDescriptor),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialerId)
            );
        }

        public DialerErrorCode ConnectInboundCall(
            int dialerId,
            long campaignId,
            int agentId,
            int contactId,
            string inboundCallId,
            CallInfo callInfo,
            long[] campaignIdsToBorrowAgentsFrom,
            AudioMessageDescriptor audioMessageDescriptor)
        {
            EventDetailsScope.Current.AddTiming("Begin ConnectInboundCall");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, inboundCallId={inboundCallId}, callInfo={callInfo}, " +
                $"campaignIdsToBorrowAgentsFrom=[{(campaignIdsToBorrowAgentsFrom != null ? string.Join(", ", campaignIdsToBorrowAgentsFrom) : "<NULL>")}], " +
                $"audioMessageDescriptor={audioMessageDescriptor.NullableToString()}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.ConnectInboundCall(
                    int.Parse(dialerTenantId),
                    campaignId,
                    inboundCallId,
                    callInfo,
                    campaignIdsToBorrowAgentsFrom,
                    audioMessageDescriptor),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode ConnectInboundCallToAgent(
            int dialerId,
            long campaignId,
            int agentId,
            int contactId,
            string inboundCallId,
            CallInfo callInfo,
            AudioMessageDescriptor audioMessageDescriptor)
        {
            EventDetailsScope.Current.AddTiming("Begin ConnectInboundCallToAgent");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, inboundCallId={inboundCallId}, " +
                $"callInfo={callInfo}, audioMessageDescriptor={audioMessageDescriptor.NullableToString()}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.ConnectInboundCallToAgent(
                    int.Parse(dialerTenantId),
                    campaignId,
                    inboundCallId,
                    callInfo,
                    audioMessageDescriptor),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode TransferStart(int dialerId, long campaignId, string transferId, int agentId, int contactId, TransferType transferType)
        {
            EventDetailsScope.Current.AddTiming("Begin TransferStart");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, " +
                $"transferId={transferId}, agentId={agentId}, transferType={transferType}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.TransferStart(
                    int.Parse(dialerTenantId),
                    campaignId,
                    transferId,
                    agentId,
                    transferType),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode TransferSetTarget(int dialerId, long campaignId, string transferId, int agentId, int contactId, TargetType targetType,
            string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            EventDetailsScope.Current.AddTiming("Begin TransferSetTarget");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, " +
                $"transferId={transferId}, targetType={targetType}, targetResource={targetResource}, " +
                $"borrowAgentsFromAllCampaigns={borrowAgentsFromAllCampaigns}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.TransferSetTarget(
                    int.Parse(dialerTenantId),
                    campaignId,
                    transferId,
                    targetType,
                    targetResource,
                    borrowAgentsFromAllCampaigns),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode TransferSetConnectionState(int dialerId, long campaignId, string transferId, int agentId, int contactId, ConnectionState state)
        {
            EventDetailsScope.Current.AddTiming("Begin TransferSetConnectionState");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, " +
                $"transferId={transferId}, state={state}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.TransferSetConnectionState(
                    int.Parse(dialerTenantId),
                    campaignId,
                    transferId,
                    state),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode TransferComplete(int dialerId, long campaignId, string transferId, int agentId, int contactId)
        {
            EventDetailsScope.Current.AddTiming("Begin TransferComplete");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, " +
                $"transferId={transferId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.TransferComplete(
                    int.Parse(dialerTenantId),
                    campaignId,
                    transferId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode TransferCancel(int dialerId, long campaignId, string transferId, int agentId, int contactId)
        {
            EventDetailsScope.Current.AddTiming("Begin TransferCancel");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var dialerTenantId = dialer.TenantId;

            var argumentsAsString =
                $"dialerId={dialerId}, companyId={dialerTenantId}, campaignId={campaignId}, " +
                $"transferId={transferId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.TransferCancel(
                    int.Parse(dialerTenantId),
                    campaignId,
                    transferId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId).Interview(contactId)
            );
        }

        public DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            var outLogin = "";
            var outPassword = "";
            var outHost = "";
            var outExtension = "";
            var outFrontendUrl = "";


            var argumentsAsString = $"companyId={companyId}, dialerId={dialerId}, agentId={agentId}, agentName={agentName}";

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var result = DoDialerCall(
                () => dialer.Api.RegisterAgentSoftphone(
                    companyId, dialerId, agentId, agentName, out outLogin, out outPassword, out outHost, out outExtension, out outFrontendUrl),
                dialer,
                argumentsAsString
            );
            var logEvent = new DialerActivityEvent(dialer.DialerId).Interviewer(agentId).Parameters($"{argumentsAsString}, login={outLogin}, password={outPassword}, host={outHost}, extension={outExtension}, frontendUrl={outFrontendUrl}");
            logEvent.LogInfo(result);
            login = outLogin;
            password = outPassword;
            host = outHost;
            frontendUrl = outFrontendUrl;
            extension = outExtension;
            
            return result;
        }

        public void InitializeRecording()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AudioRecordInfo> GetAudioRecords(int surveyId, int interviewId)
        {
            EventDetailsScope.Current.AddTiming("Begin GetAudioRecords");

            var result = new List<AudioRecordInfo>();

            foreach (var dialer in _dialerCollection.Value.GetDialers())
            {
                //TODO: How to to take records from unavailable dialers: (see Bug 66359 as well)
                //<The simplest way> to achive this is to comment next four lines. But in this case we will depend on the hack rule:
                //"Records are available on dialer if the dialer was successfully initialized and BE was not restarted then,
                //even if the dialer became unavailable later."
                //<The correct way> to achive this could be redesign DialerInstance object to have to Initialize methods - one for dialer, other for dialer recording.
                //But it is not applicable at the moment of Minipatch 001 for 17.5.
                //So it is decided use <The simplest way>.
                /*if (!dialer.IsDialerInitialized)
                {
                    continue;
                }*/

                var logEvent = new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefById(surveyId))
                    .Interview(interviewId)
                    .Parameters($"surveyId={surveyId}, interviewId={interviewId}");

                try
                {
                    result.AddRange(_dialerRecordingWrapper.Value.GetInterviewRecordings(dialer.DialerId, dialer.TenantIdInt, surveyId, interviewId));

                    logEvent.LogInfo(result);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning(
                        "TelephonyProvider.GetAudioRecords: {0} /// surveyId = {1}, interviewId = {2}, dialerId = {3}, dialerName = {4}, tenantId = {5}",
                        ex,
                        surveyId,
                        interviewId,
                        dialer.DialerId,
                        dialer.DialerName,
                        dialer.TenantId);

                    logEvent.LogError(ex);
                }
            }

            EventDetailsScope.Current.AddTiming("End GetAudioRecords");

            return result;
        }
        
        public AudioFile GetAudioFile(int dialerId, string audioUrl)
        {
            EventDetailsScope.Current.AddTiming("Begin GetAudioRecords");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);
            var result = new AudioFile();
            
            var logEvent = new DialerActivityEvent(dialerId);

            try
            {
                result = _dialerRecordingWrapper.Value.GetAudioFile(dialer.TenantIdInt, dialerId, audioUrl);

                logEvent.LogInfo(new 
                {
                    result.FileName, result.CreationTime, ContentLength = result.Content?.Length
                });
            }
            catch (Exception ex)
            {
                Trace.TraceWarning(
                    "TelephonyProvider.GetAudioFile: {0} /// companyId = {1}, dialerId = {2}, audioUrl = {3}",
                    ex,
                    dialer.TenantIdInt,
                    dialerId,
                    audioUrl);

                logEvent.LogError(ex);
            }

            EventDetailsScope.Current.AddTiming("End GetAudioRecords");

            return result;
        }

        public bool[] AreRecordsExists(int surveyId, int[] interviewIds)
        {
            EventDetailsScope.Current.AddTiming("Begin AreRecordsExists");

            var result = new bool[interviewIds.Length];

            foreach (var dialer in _dialerCollection.Value.GetDialers())
            {
                var surveyEntity = _surveyRepository.Value.GetById(surveyId);
                var logEvent = new DialerActivityEvent(dialer.DialerId).Survey(surveyEntity.ProjectId, surveyEntity.SID)
                    .Parameters($"surveyId={surveyId}, interviewIds={interviewIds.ToJson()}");

                try
                {
                    JoinResults(ref result, _dialerRecordingWrapper.Value.AreRecordsExists(dialer.DialerId, dialer.TenantIdInt, surveyId, interviewIds));

                    var logResult = new Dictionary<int, bool>();
                    for (var i = 0; i < interviewIds.Length && i < result.Length; i++)
                        logResult[interviewIds[i]] = result[i];

                    logEvent.LogInfo(logResult);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(
                        "Exception on AreRecordsExists, surveyId = {0}, interviewIds = {1}, dialerid = {2}, dialerName = {3}, tenantId = {4} : {5}",
                        surveyId,
                        interviewIds.Select(s => s.ToString(CultureInfo.InvariantCulture)).ToArray(),
                        dialer.DialerId,
                        dialer.DialerName,
                        dialer.TenantId,
                        ex);

                    logEvent.LogError(ex);
                }
            }

            EventDetailsScope.Current.AddTiming("End AreRecordsExists");

            return result;
        }

        private static void JoinResults(ref bool[] result, bool[] ar)
        {
            var count = result.Length;
            for (var i = 0; i < count; i++)
            {
                result[i] = result[i] || ar[i];
            }
        }

        public DialerErrorCode StartPlayback(
            int dialerId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            string fileName,
            out int timeOfPlayingInSeconds)
        {
            var dialer = _dialerCollection.Value.GetDialerById(dialerId);
            var dialerTenantId = dialer.TenantId;
            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, " +
                $"interviewId={interviewId}, callId={callId}, fileName={fileName}";

            int timeOfPlaying = 0;
            var result = DoDialerCall(
                () => dialer.Api.StartPlayback(
                    dialerTenantId, campaignId, agentId, interviewId, callId, fileName, out timeOfPlaying),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(interviewId)
            );
            timeOfPlayingInSeconds = timeOfPlaying;

            return result;
        }

        public DialerErrorCode StopPlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {
            EventDetailsScope.Current.AddTiming("Begin StopPlayback");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);
            var dialerTenantId = dialer.TenantId;
            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, callId={callId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.StopPlayback(dialerTenantId, campaignId, agentId, callId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(interviewId)
            );
        }

        public bool IsPauseOrResumePlaybackSupported(int? dialerId = null)
        {
            EventDetailsScope.Current.AddTiming("Begin IsPauseOrResumePlaybackSupported");

            var dialerApi = dialerId.HasValue
                ? _dialerCollection.Value.GetDialerById(dialerId.Value).Api
                : _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.IsPauseOrResumePlaybackSupported, dialerApi);
        }

        public bool IsToggleInterviewerListensToPlaybackOrRespondentSupported(int? dialerId = null)
        {
            EventDetailsScope.Current.AddTiming("Begin IsToggleInterviewerListensToPlaybackOrRespondentSupported");

            var dialerApi = dialerId.HasValue
                ? _dialerCollection.Value.GetDialerById(dialerId.Value).Api
                : _dialerCollection.Value.FirstLoadedDialerApi;

            return DoDialerCall(
                () => dialerApi.IsToggleInterviewerListensToPlaybackOrRespondentSupported, dialerApi);
        }

        public DialerErrorCode PauseOrResumePlayback(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {
            EventDetailsScope.Current.AddTiming("Begin PauseOrResumePlayback");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);
            var dialerTenantId = dialer.TenantId;
            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, callId={callId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.PauseOrResumePlayback(dialerTenantId, campaignId, agentId, callId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(interviewId)
            );
        }

        public DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(int dialerId, long campaignId, string agentId, int interviewId, int callId)
        {
            EventDetailsScope.Current.AddTiming("Begin ToggleInterviewerListensToPlaybackOrRespondent");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);
            var dialerTenantId = dialer.TenantId;
            var argumentsAsString =
                $"dialerId={dialerId}, tenantId={dialerTenantId}, campaignId={campaignId}, agentId={agentId}, callId={callId}";

            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.ToggleInterviewerListensToPlaybackOrRespondent(dialerTenantId, campaignId, agentId, callId),
                dialer,
                argumentsAsString,
                new DialerActivityEvent(dialer.DialerId).Survey(GetSurveyRefByCampaign(campaignId)).Interviewer(agentId)
                    .Interview(interviewId)
            );
        }

        public void SendGoReady(int dialerId, long campaignId, long agentId, Func<string> logInfoFunc)
        {
            //TODO: GoNotReady can throw DialerNotFoundException. It should be handled here in the same way as in the SendSetGroups below.
            var result = GoReady(
                dialerId,
                campaignId,
                agentId.ToString(CultureInfo.InvariantCulture));
            EventDetailsScope.Current.AddTiming("GoReady");

            if (result == DialerErrorCode.Success)
            {
                return;
            }

            _problemStateSetter.Value.SetProblemState(agentId, result);

            throw new Exception(
                $"GoReady error [{result}] /// " + logInfoFunc());
        }

        public void SendGoNotReady(int dialerId, long campaignId, string agentId, string breakName,
            Func<string> logInfoFunc)
        {
            //TODO: GoNotReady can throw DialerNotFoundException. It should be handled here in the same way as in the SendSetGroups below.
            var result = GoNotReady(
                dialerId,
                campaignId,
                agentId,
                breakName);

            EventDetailsScope.Current.AddTiming("GoNotReady");

            if (result == DialerErrorCode.Success)
            {
                return;
            }

            _problemStateSetter.Value.SetProblemState(long.Parse(agentId), result);

            throw new Exception(
                $"GoNotReady error [{result}] /// " + logInfoFunc());
        }

        public void SendSetGroups(int dialerId, long campaignId, long agentId, int[] userGroups)
        {
            var result = DialerErrorCode.Exception;

            try
            {
                result = SetGroups(
                    dialerId,
                    campaignId,
                    agentId.ToString(CultureInfo.InvariantCulture),
                    userGroups);
                EventDetailsScope.Current.AddTiming("SetGroups");

                if (result != DialerErrorCode.Success)
                {
                    throw new Exception(
                        $"SetGroups is failed with error code [{result}]");
                }

            }
            catch (Exception)
            {
                _problemStateSetter.Value.SetProblemState(agentId, result);

                throw;
            }
        }

        public IEnumerable<LogFileInfo> GetLogFiles(int dialerId)
        {
            EventDetailsScope.Current.AddTiming("Begin GetLogFiles");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var argumentsAsString = $"{nameof(dialerId)}={dialerId}";
            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.GetLogFiles(),
                dialer.Api,
                new DialerActivityEvent(dialerId).Parameters(argumentsAsString)
            );
        }

        public byte[] GetLogFileBodyZipped(int dialerId, string fileName)
        {
            EventDetailsScope.Current.AddTiming("Begin GetLogFileBodyZipped");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var argumentsAsString = $"{nameof(dialerId)}={dialerId}, {nameof(fileName)}={fileName}";
            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.GetLogFileBodyZipped(fileName),
                dialer.Api,
                new DialerActivityEvent(dialerId)
                    .Parameters(argumentsAsString)
                    .ResultFormatter(r => ((byte[])r).Length.ToString())
            );
        }

        public string GetDialerVersion(int dialerId)
        {
            EventDetailsScope.Current.AddTiming("Begin GetDialerVersion");

            var dialer = _dialerCollection.Value.GetDialerById(dialerId);

            var argumentsAsString = $"{nameof(dialerId)}={dialerId}";
            EventDetailsScope.Current.AddTiming(argumentsAsString);

            return DoDialerCall(
                () => dialer.Api.GetDialerVersion(),
                dialer.Api,
                new DialerActivityEvent(dialerId).Parameters(argumentsAsString)
            );
        }

        private DialerActivityEvent.SurveyRef GetSurveyRefByCampaign(long campaignId)
        {
            var result = new DialerActivityEvent.SurveyRef
            {
                ProjectId = "",
                SurveySid = -1
            };

            try
            {
                var surveyEntity = _surveyRepository.Value.GetByCampaignId(campaignId);
                result.SurveySid = surveyEntity.SID;
                result.ProjectId = surveyEntity.ProjectId;
            }
            catch (Exception) { /*ignored*/ }

            return result;
        }

        private DialerActivityEvent.SurveyRef GetSurveyRefById(int surveyId)
        {
            var result = new DialerActivityEvent.SurveyRef
            {
                ProjectId = "",
                SurveySid = -1
            };

            try
            {
                var surveyEntity = _surveyRepository.Value.GetById(surveyId);
                result.SurveySid = surveyEntity.SID;
                result.ProjectId = surveyEntity.ProjectId;
            }
            catch (Exception) { /*ignored*/ }

            return result;
        }

        private void VerifyDialerInitialized(IDialerInstance dialer, string methodName)
        {
            if (!_dialerCollection.Value.IsDialerInitialized(dialer.DialerId))
            {
                if (dialer.DialerOperationalState) 
                {
                    try
                    {
                        dialer.Create(); //init dialer instance without calling Initialize method if dialer api
                        dialer.IsDialerInitialized = true;
                    }
                    catch (Exception ex)
                    {
                        _dialerOperationalStateNotificator.Value.SendDialerOperationalStateNotification(dialer.DialerId, false);
                        throw new InternalErrorException(
                            $@"TelephonyProvider.DoDialerCall: Dialer [id={dialer.DialerId}, tenantId={dialer.TenantId}] is [not available] on {methodName} call.
                                Dialer should be in operational state but initialization failed with exception", ex);
                    }
                }
                else
                {
                    throw new InternalErrorException(
                        $@"TelephonyProvider.DoDialerCall: Dialer [id={dialer.DialerId}, tenantId={dialer.TenantId}] is [not available] on {methodName} call; 
                        Dialer is in not operational state, it should be activated in the supervisor interface");
                }
            }
        }
    }
}
