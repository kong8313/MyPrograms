using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging.DialerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerInitializer : IDialerInitializer
    {
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerSettings _dialerSettings;
        private readonly IDialerType _dialerType;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly IToggleSettings _toggleSettings;
        private readonly IInboundAudioMessages _inboundAudioMessages;
        private readonly ISurveyRepository _surveyRepository;

        public DialerInitializer(
            IDialersRepository dialersRepository,
            IDialerSettings dialerSettings,
            IDialerType dialerType,
            IInboundTelephoneNumberRepository inboundTelephoneNumberRepository,
            IToggleSettings toggleSettings,
            IInboundAudioMessages inboundAudioMessages,
            ISurveyRepository surveyRepository)
        {
            _dialersRepository = dialersRepository;
            _dialerSettings = dialerSettings;
            _dialerType = dialerType;
            _inboundTelephoneNumberRepository = inboundTelephoneNumberRepository;
            _toggleSettings = toggleSettings;
            _inboundAudioMessages = inboundAudioMessages;
            _surveyRepository = surveyRepository;
        }

        public IDialerAPI CreateInstance()
        {
            try
            {
                return _dialerType.CreateInstance<IDialerAPI>();
            }
            catch (Exception ex)
            {
                Trace.TraceError("DialerInitializer.CreateInstance: {0}", ex);
            }

            return null;
        }

        public IDialerAPI InitializeDialer(int dialerId, IDialerAPI dialerApi, bool sendInitializeToWebService, out int tenantId, out string name, out DialType dialType)
        {
            var dialerEntity = _dialersRepository.GetById(dialerId);

            tenantId = dialerEntity.TenantId;
            name = dialerEntity.Name;
            dialType = (DialType) dialerEntity.DialTypeId;

            if (0 == tenantId)
            {
                throw new DialerIsNotConfiguredException(
                    "Attempt to initialize dialer but Tenant ID is zero. /// Dialer name is [" + name + "]");
            }

            if (dialerApi == null)
            {
                dialerApi = CreateInstance();
            }

            if (dialerApi == null)
            {
                throw new DialerIsNotConfiguredException(
                    "Dialer instance (api) is not created. /// Dialer name is [" + name + "]");
            }

            DialerActivityEvent activityEvent = null;
            
            if (sendInitializeToWebService)
            {
                activityEvent = new DialerActivityEvent(dialerId, nameof(dialerApi.Initialize))
                    .Detail(nameof(dialerEntity.ConnectionParameters), dialerEntity.ConnectionParameters)
                    .Detail(nameof(dialerEntity.ConfigurationParameters), dialerEntity.ConfigurationParameters)
                    .Detail(nameof(_dialerSettings.DefaultSurveyParameters), _dialerSettings.DefaultSurveyParameters);
            }
            
            DialerInitializeResult initializeResult;
            var cntRetry = 0;
            while(true)
            {
                initializeResult = dialerApi.Initialize(
                    dialerEntity.Id,
                    tenantId.ToString(CultureInfo.InvariantCulture),
                    dialerEntity.ConnectionParameters,
                    dialerEntity.ConfigurationParameters,
                    _dialerSettings.DefaultSurveyParameters,
                    sendInitializeToWebService);

                
                if (initializeResult.DialerErrorCode == DialerErrorCode.Success || cntRetry > 3)
                    break;

                cntRetry++;
                Thread.Sleep(1000);
            }

            if (initializeResult.DialerErrorCode != DialerErrorCode.Success)
            {
                var message = $"Failed to initialize Dialer [{dialerEntity.Id}: {dialerEntity.Name}] after {cntRetry} attempts. Error code: [{initializeResult.DialerErrorCode}].";
                if (!string.IsNullOrEmpty(initializeResult.ErrorMessage))
                {
                    message += " Error message: " + initializeResult.ErrorMessage;
                }

                var exception = new InternalErrorException(message);
                
                activityEvent?.LogError(initializeResult.DialerErrorCode, exception);
                
                throw exception;
            }

            if (sendInitializeToWebService)
            {
                activityEvent?.LogInfo(initializeResult);

                UpdateCodiVersionInfoInDatabase(dialerApi, dialerEntity);
                UpdateDialerFeaturesInDatabase(dialerApi, tenantId.ToString(CultureInfo.InvariantCulture), dialerEntity);

                if (_toggleSettings.EnableInbound)
                {
                    ConfigureDdiNumbers(dialerId, tenantId, dialerApi);
                }
            }

            return dialerApi;
        }

        private void UpdateCodiVersionInfoInDatabase(IDialerAPI dialerApi, BvDialersEntity dialerEntity)
        {
            var codiVersionInfo = dialerApi.GetCodiVersionInfo();
            var dialerNameAndVersion = codiVersionInfo.DialerDriverNameAndVersion.Split('#');

            if (dialerEntity.DialerInterfaceVersion == codiVersionInfo.CodiFullVersion &&
                dialerEntity.DialerDriver == dialerNameAndVersion[0] &&
                (dialerNameAndVersion.Length < 1 || dialerEntity.DialerDriverVersion == dialerNameAndVersion[1]))
            {
                return;
            }

            dialerEntity.DialerInterfaceVersion = codiVersionInfo.CodiFullVersion;
            dialerEntity.DialerDriver = dialerNameAndVersion[0];
            if (dialerNameAndVersion.Length > 1)
            {
                dialerEntity.DialerDriverVersion = dialerNameAndVersion[1];
            }
            
            _dialersRepository.Update(dialerEntity, false);
        }
        private void UpdateDialerFeaturesInDatabase(IDialerAPI dialerApi, string tenantId, BvDialersEntity dialerEntity)
        {
            var features = GetFeatures(dialerApi, tenantId, dialerEntity.Id);
            string serializedFeatures = JsonSerializer.Serialize(features);
            dialerEntity.Features = serializedFeatures;

            _dialersRepository.Update(dialerEntity, false);
        }
        
        private DialerFeatures GetFeatures(IDialerAPI dialerApi, string tenantId, int dialerId)
        {
            var activityEvent = new DialerActivityEvent(dialerId);
            try
            {
                var features = dialerApi.GetFeatures(tenantId);
                activityEvent.LogInfo(features);
                return features;
            }
            catch (Exception e)
            {
                activityEvent.LogError(e);
                throw;
            }
        }

        internal void ConfigureDdiNumbers(int dialerId, int tenantId, IDialerAPI dialerApi)
        {
            var inboundDdiNumbers = _inboundTelephoneNumberRepository.GetValidByDialerId(dialerId);
            
            var ddiNumbers = inboundDdiNumbers.Select(x => new InboundDdiNumber
            {
                Number = x.TelephoneNumber,
                AudioMessages = _inboundAudioMessages.DdiNumbersMessages(x)
            }).ToArray();

            var activityEvent = new DialerActivityEvent(dialerId, nameof(dialerApi.ConfigureInboundDdiNumbers))
                .Detail(nameof(ddiNumbers), ddiNumbers);
            try
            {
                var result = dialerApi.ConfigureInboundDdiNumbers(tenantId, ddiNumbers);
                activityEvent.LogInfo(result);
            }
            catch (Exception e)
            {
                activityEvent.LogError(e);
                throw;
            }

        }
    }
}
