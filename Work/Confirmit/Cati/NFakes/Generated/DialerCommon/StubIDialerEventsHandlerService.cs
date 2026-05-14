using System;
using Confirmit.CATI.Telephony.DialerCommon;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerCommon.Fakes
{
    public class StubIDialerEventsHandlerService : IDialerEventsHandlerService 
    {
        private IDialerEventsHandlerService _inner;

        public StubIDialerEventsHandlerService()
        {
            _inner = null;
        }

        public IDialerEventsHandlerService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void NotifyUserStateInt32StringStringStringInt64StringStringDelegate(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string userState);
        public NotifyUserStateInt32StringStringStringInt64StringStringDelegate NotifyUserStateInt32StringStringStringInt64StringString;

        void IDialerEventsHandlerService.NotifyUserState(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string userState)
        {

            if (NotifyUserStateInt32StringStringStringInt64StringString != null)
            {
                NotifyUserStateInt32StringStringStringInt64StringString(dialerId, requestId, sessionId, companyId, surveyId, userId, userState);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyUserState(dialerId, requestId, sessionId, companyId, surveyId, userId, userState);
            }
        }

        public delegate void NotifyOutcomeInt32StringStringStringInt64StringStringInt32StringStringStringTimeSpanDictionaryOfStringStringStringDelegate(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string contactId, int callId, string outcomeType, string outcomeCode, string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata, string correlationId);
        public NotifyOutcomeInt32StringStringStringInt64StringStringInt32StringStringStringTimeSpanDictionaryOfStringStringStringDelegate NotifyOutcomeInt32StringStringStringInt64StringStringInt32StringStringStringTimeSpanDictionaryOfStringStringString;

        void IDialerEventsHandlerService.NotifyOutcome(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string contactId, int callId, string outcomeType, string outcomeCode, string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata, string correlationId)
        {

            if (NotifyOutcomeInt32StringStringStringInt64StringStringInt32StringStringStringTimeSpanDictionaryOfStringStringString != null)
            {
                NotifyOutcomeInt32StringStringStringInt64StringStringInt32StringStringStringTimeSpanDictionaryOfStringStringString(dialerId, requestId, sessionId, companyId, surveyId, userId, contactId, callId, outcomeType, outcomeCode, callerId, ringTime, callOutcomeMetadata, correlationId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyOutcome(dialerId, requestId, sessionId, companyId, surveyId, userId, contactId, callId, outcomeType, outcomeCode, callerId, ringTime, callOutcomeMetadata, correlationId);
            }
        }

        public delegate void NotifyInboundCallInt32Int32StringStringStringDelegate(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId);
        public NotifyInboundCallInt32Int32StringStringStringDelegate NotifyInboundCallInt32Int32StringStringString;

        void IDialerEventsHandlerService.NotifyInboundCall(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId)
        {

            if (NotifyInboundCallInt32Int32StringStringString != null)
            {
                NotifyInboundCallInt32Int32StringStringString(dialerId, companyId, ddiNumber, cliNumber, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyInboundCall(dialerId, companyId, ddiNumber, cliNumber, inboundCallId);
            }
        }

        public delegate void NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64Delegate(int dialerId, int companyId, long surveyId, int agentId, long callId);
        public NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64Delegate NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64;

        void IDialerEventsHandlerService.NotifyCallDroppedByRespondent(int dialerId, int companyId, long surveyId, int agentId, long callId)
        {

            if (NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64 != null)
            {
                NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64(dialerId, companyId, surveyId, agentId, callId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyCallDroppedByRespondent(dialerId, companyId, surveyId, agentId, callId);
            }
        }

        public delegate void NotifyInboundCallDroppedByRespondentInt32Int32StringDelegate(int dialerId, int companyId, string inboundCallId);
        public NotifyInboundCallDroppedByRespondentInt32Int32StringDelegate NotifyInboundCallDroppedByRespondentInt32Int32String;

        void IDialerEventsHandlerService.NotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId)
        {

            if (NotifyInboundCallDroppedByRespondentInt32Int32String != null)
            {
                NotifyInboundCallDroppedByRespondentInt32Int32String(dialerId, companyId, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyInboundCallDroppedByRespondent(dialerId, companyId, inboundCallId);
            }
        }

        public delegate void NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome);
        public NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome;

        void IDialerEventsHandlerService.NotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome)
        {

            if (NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome != null)
            {
                NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyCustomIvrInterviewEnd(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
            }
        }

        public delegate void ScreenPopInt32StringStringStringInt64StringStringInt32DialingModeDelegate(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string contactId, int callId, DialingMode callDialingMode);
        public ScreenPopInt32StringStringStringInt64StringStringInt32DialingModeDelegate ScreenPopInt32StringStringStringInt64StringStringInt32DialingMode;

        void IDialerEventsHandlerService.ScreenPop(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string contactId, int callId, DialingMode callDialingMode)
        {

            if (ScreenPopInt32StringStringStringInt64StringStringInt32DialingMode != null)
            {
                ScreenPopInt32StringStringStringInt64StringStringInt32DialingMode(dialerId, requestId, sessionId, companyId, surveyId, userId, contactId, callId, callDialingMode);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).ScreenPop(dialerId, requestId, sessionId, companyId, surveyId, userId, contactId, callId, callDialingMode);
            }
        }

        public delegate void NotifyDialerStateInt32StringDialerStateDelegate(int dialerId, string companyId, DialerState dialerState);
        public NotifyDialerStateInt32StringDialerStateDelegate NotifyDialerStateInt32StringDialerState;

        void IDialerEventsHandlerService.NotifyDialerState(int dialerId, string companyId, DialerState dialerState)
        {

            if (NotifyDialerStateInt32StringDialerState != null)
            {
                NotifyDialerStateInt32StringDialerState(dialerId, companyId, dialerState);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyDialerState(dialerId, companyId, dialerState);
            }
        }

        public delegate void RequestCallsInt32StringStringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32Delegate(int dialerId, string requestId, string sessionId, string companyId, long surveyId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount);
        public RequestCallsInt32StringStringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32Delegate RequestCallsInt32StringStringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32;

        void IDialerEventsHandlerService.RequestCalls(int dialerId, string requestId, string sessionId, string companyId, long surveyId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount)
        {

            if (RequestCallsInt32StringStringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32 != null)
            {
                RequestCallsInt32StringStringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32(dialerId, requestId, sessionId, companyId, surveyId, groupId, callsSelectionAlgorithm, callCount);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).RequestCalls(dialerId, requestId, sessionId, companyId, surveyId, groupId, callsSelectionAlgorithm, callCount);
            }
        }

        public delegate void NotifyLicenseExpirationInt32DateTimeDelegate(int dialerId, DateTime licenseExpirationDateTime);
        public NotifyLicenseExpirationInt32DateTimeDelegate NotifyLicenseExpirationInt32DateTime;

        void IDialerEventsHandlerService.NotifyLicenseExpiration(int dialerId, DateTime licenseExpirationDateTime)
        {

            if (NotifyLicenseExpirationInt32DateTime != null)
            {
                NotifyLicenseExpirationInt32DateTime(dialerId, licenseExpirationDateTime);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyLicenseExpiration(dialerId, licenseExpirationDateTime);
            }
        }

        public delegate void NotifyIvrSubmitInt32StringInt64StringArrayOfKeyValuePairOfStringStringDelegate(int dialerId, string companyId, long surveyId, string agentId, KeyValuePair<string, string>[] variables);
        public NotifyIvrSubmitInt32StringInt64StringArrayOfKeyValuePairOfStringStringDelegate NotifyIvrSubmitInt32StringInt64StringArrayOfKeyValuePairOfStringString;

        void IDialerEventsHandlerService.NotifyIvrSubmit(int dialerId, string companyId, long surveyId, string agentId, KeyValuePair<string, string>[] variables)
        {

            if (NotifyIvrSubmitInt32StringInt64StringArrayOfKeyValuePairOfStringString != null)
            {
                NotifyIvrSubmitInt32StringInt64StringArrayOfKeyValuePairOfStringString(dialerId, companyId, surveyId, agentId, variables);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyIvrSubmit(dialerId, companyId, surveyId, agentId, variables);
            }
        }

        public delegate void NotifyTransferStateInt32Int32StringTransferStateDelegate(int dialerId, int companyId, string transferId, TransferState transferState);
        public NotifyTransferStateInt32Int32StringTransferStateDelegate NotifyTransferStateInt32Int32StringTransferState;

        void IDialerEventsHandlerService.NotifyTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {

            if (NotifyTransferStateInt32Int32StringTransferState != null)
            {
                NotifyTransferStateInt32Int32StringTransferState(dialerId, companyId, transferId, transferState);
            } else if (_inner != null)
            {
                ((IDialerEventsHandlerService)_inner).NotifyTransferState(dialerId, companyId, transferId, transferState);
            }
        }

    }
}