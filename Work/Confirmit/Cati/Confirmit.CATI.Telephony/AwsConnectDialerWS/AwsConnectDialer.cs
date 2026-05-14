using System;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.CallStatusConsumer;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.OutboundCall;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.AwsIvr.SurveyProvisioning;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Config;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Context;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage;
using Confirmit.CATI.Telephony.AwsConnectDialerWS.Storage.Model;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.AwsConnectDialerWS
{
    public class AwsConnectDialer : IDisposable
    {
        private readonly IDialerEvents _dialerEvents;
        private readonly ILogger _logger;
        private readonly RedisClient _redisClient;
        private DialerConfigurationParameters _dialerConfig;
        private SurveyProvisioningService _surveyProvisioningService;
        private OutboundCaller _outboundCaller;
        private CallStatusConsumer _callStatusConsumer;

        public AwsConnectDialer(IDialerEvents dialerEvents, ILogger logger, RedisClient redisClient)
        {
            _dialerEvents = dialerEvents;
            _logger = logger;
            _redisClient = redisClient;
        }

        public void Initialize(string configurationParametersXml)
        {
            _dialerConfig = ConfigReader.Read<DialerConfigurationParameters>(configurationParametersXml);
            ConfigValidator.ValidateConfig(_dialerConfig);
            
            var awsAccessOptions = new AwsAccessOptions
            {
                AccessKey = _dialerConfig.AwsAccessKey,
                SecretKey = _dialerConfig.AwsSecretKey,
                Region = _dialerConfig.AwsRegion,
            };
            
            _surveyProvisioningService = new SurveyProvisioningService(awsAccessOptions, _dialerConfig.AwsPublicApiUrl);
            _outboundCaller = new OutboundCaller(awsAccessOptions);
            _callStatusConsumer = new CallStatusConsumer(awsAccessOptions, _dialerConfig.AwsCallStatusQueueUrl, _logger);

            _callStatusConsumer.StartCallStatusConsumer(IsValidRespondent, OnCallDisconnected);
        }

        private bool IsValidRespondent(string respondentContext)
        {
            return !string.IsNullOrEmpty(respondentContext) && _redisClient.Get<RespondentInfo>(respondentContext) != null;
        }
        
        private void OnCallDisconnected(OnCallDisconnectedEventArgs args)
        {
            var respondentInfo = _redisClient.Get<RespondentInfo>(args.ContextId);
            if (respondentInfo == null)
                return;

            NotifyInterviewEnd(respondentInfo.Context, args.AnsweringMachineDetectionStatus);

            _redisClient.Remove(respondentInfo.Context);
        }

        public void CreateSurveySession(SurveyContext surveyContext, string campaignParametersXml)
        {
            var campaignConfig = ConfigReader.Read<DialerSurveyParameters>(campaignParametersXml);
            ConfigValidator.ValidateConfig(campaignConfig);
            
            var sourcePhoneNumber = campaignConfig.GetValue(DialerParameterKnownNames.SourcePhoneNumber);
            var callerId = campaignConfig.GetValue(DialerParameterKnownNames.CallerID);
            var ansMachineDetect = campaignConfig.GetBoolValue(DialerParameterKnownNames.AnsMachineDetect);

            var response = _surveyProvisioningService.RegisterSurveyIntegration(new SurveyProvisioningPayload
            {
                ProjectId = surveyContext.ProjectId,
                PhoneNumber = sourcePhoneNumber,
                CallerId = callerId
            }).ConfigureAwait(false).GetAwaiter().GetResult();
            
            _redisClient.Set(surveyContext, new SurveyInfo(surveyContext, sourcePhoneNumber, response.QueueId, ansMachineDetect));
        }

        public void UpdateSurveySession(SurveyContext surveyContext, string campaignParametersXml)
        {
            var campaignConfig = ConfigReader.Read<DialerSurveyParameters>(campaignParametersXml);
            ConfigValidator.ValidateConfig(campaignConfig);

            var surveyInfo = _redisClient.Get<SurveyInfo>(surveyContext);
            if (surveyInfo != null)
            {
                // we could update only optional fields which are not used in survey provisioning
                surveyInfo.EnableAnswerMachineDetection = campaignConfig.GetBoolValue(DialerParameterKnownNames.AnsMachineDetect);

                _redisClient.Set(surveyContext, surveyInfo);
            }
        }

        public void CreateRespondentSession(RespondentContext respondentContext, string phoneNumber)
        {
            ConfigValidator.ValidatePhoneNumber(phoneNumber);

            var safetyExpirationPeriod = TimeSpan.FromDays(30); // just for cleanup if NotifyInterviewEnd won't fire
            _redisClient.Set(respondentContext, new RespondentInfo(respondentContext, phoneNumber), safetyExpirationPeriod);
            
            NotifyOutcome(respondentContext, CallOutcome.Connected);
        }

        public void NotifyOutcome(RespondentContext respondentContext, CallOutcome callOutcome)
        {
            _dialerEvents.NotifyOutcome(
                respondentContext.CompanyId,
                respondentContext.DialerId,
                respondentContext.CampaignId,
                respondentContext.AgentId,
                respondentContext.InterviewId,
                respondentContext.CallId,
                callOutcome,
                null,
                TimeSpan.Zero,
                null,
                null);
        }
        
        private void NotifyInterviewEnd(RespondentContext respondentContext, string originalStatus)
        {
            var callOutcome = ConvertAnsweringMachineDetectionStatus(originalStatus);
            _logger.Info(nameof(NotifyInterviewEnd), $"{respondentContext} ==> {originalStatus} ==> {callOutcome}");
            _dialerEvents.NotifyCustomIvrInterviewEnd(
                respondentContext.CompanyId,
                respondentContext.DialerId,
                respondentContext.CampaignId,
                respondentContext.AgentId,
                respondentContext.InterviewId,
                respondentContext.CallId,
                callOutcome);
        }

        public void StartCall(RespondentContext respondentContext, string respondentSurveyLink)
        {
            var surveyInfo = _redisClient.Get<SurveyInfo>(respondentContext.ToSurveyContext());
            var sourcePhoneNumber = surveyInfo?.SourcePhoneNumber;
            var connectQueueId = surveyInfo?.ConnectQueueId;
            var enableAnswerMachineDetection = surveyInfo?.EnableAnswerMachineDetection ?? false;
            
            var respondentInfo = _redisClient.Get<RespondentInfo>(respondentContext);
            var destPhoneNumber = respondentInfo?.DestPhoneNumber;
            
            _logger.Verbose(nameof(StartCall), 
                $"Input. ctx: {respondentContext}, sourcePhoneNumber: {sourcePhoneNumber}, destPhoneNumber: {destPhoneNumber}, connectQueueId: {connectQueueId}, amd: {enableAnswerMachineDetection}");

            ConfigValidator.ValidatePhoneNumber(sourcePhoneNumber);
            ConfigValidator.ValidatePhoneNumber(destPhoneNumber);
            ConfigValidator.ValidateIsNotEmpty(connectQueueId, nameof(connectQueueId));
            ConfigValidator.ValidateIsValidUrl(respondentSurveyLink, nameof(respondentSurveyLink));

            var outboundCallOptions = new OutboundCallPayload
            {
                InstanceId = _dialerConfig.AwsConnectId,
                ContactFlowId = _dialerConfig.AwsContactFlowId,
                SourcePhoneNumber = sourcePhoneNumber,
                DestinationPhoneNumber = destPhoneNumber,
                SurveyId = respondentContext.ProjectId,
                SurveyUrl = respondentSurveyLink,
                QueueId = connectQueueId,
                ContextId = respondentContext,
                EnableAnswerMachineDetection = enableAnswerMachineDetection
            };

            var contactId = _outboundCaller.StartOutboundVoiceContact(outboundCallOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();
            _logger.Verbose(nameof(StartCall), $"Result. ctx: {respondentContext}, contact id: {contactId}");
        }

        // https://docs.aws.amazon.com/connect/latest/adminguide/contact-events.html#contact-events-data-model
        private static CallOutcome ConvertAnsweringMachineDetectionStatus(string disconnectStatus)
        {
            switch (disconnectStatus)
            {
                case "VOICEMAIL_BEEP":
                case "VOICEMAIL_NO_BEEP":
                    return CallOutcome.AnswerMachine;
                case "SIT_TONE_BUSY":
                case "SIT_TONE_DETECTED":
                    return CallOutcome.Busy;
                case "HUMAN_ANSWERED":
                case "AMD_UNRESOLVED":
                case "AMD_UNRESOLVED_SILENCE":
                    return CallOutcome.Connected;
                case "FAX_MACHINE_DETECTED":
                    return CallOutcome.Fax;
                case "AMD_UNANSWERED":
                case "AMD_NOT_APPLICABLE":
                    return CallOutcome.NoReply;
                case "SIT_TONE_INVALID_NUMBER":
                case "AMD_ERROR":
                    return CallOutcome.Error;
                default:
                    return CallOutcome.Unobtainable;
            }
        }
        
        public void RemoveSurveySession(SurveyContext surveyContext)
        {
            _redisClient.Remove(surveyContext);
        }

        public void Dispose()
        {
            _callStatusConsumer?.Dispose();
        }
    }
}