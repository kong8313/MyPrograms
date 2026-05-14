using System;
using System.Collections.Generic;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.Fakes
{
    public class StubIDialerEventsHandler : IDialerEventsHandler 
    {
        private IDialerEventsHandler _inner;

        public StubIDialerEventsHandler()
        {
            _inner = null;
        }

        public IDialerEventsHandler Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnDialerNotifyOutcomeInt32StringInt64Int64StringInt64Int64StringTimeSpanDictionaryOfStringStringDelegate(int dialerId, string tenantId, long campaignId, long agentId, string contactId, long callId, long callOutcome, string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata);
        public OnDialerNotifyOutcomeInt32StringInt64Int64StringInt64Int64StringTimeSpanDictionaryOfStringStringDelegate OnDialerNotifyOutcomeInt32StringInt64Int64StringInt64Int64StringTimeSpanDictionaryOfStringString;

        void IDialerEventsHandler.OnDialerNotifyOutcome(int dialerId, string tenantId, long campaignId, long agentId, string contactId, long callId, long callOutcome, string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata)
        {

            if (OnDialerNotifyOutcomeInt32StringInt64Int64StringInt64Int64StringTimeSpanDictionaryOfStringString != null)
            {
                OnDialerNotifyOutcomeInt32StringInt64Int64StringInt64Int64StringTimeSpanDictionaryOfStringString(dialerId, tenantId, campaignId, agentId, contactId, callId, callOutcome, callerId, ringTime, callOutcomeMetadata);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerNotifyOutcome(dialerId, tenantId, campaignId, agentId, contactId, callId, callOutcome, callerId, ringTime, callOutcomeMetadata);
            }
        }

        public delegate void OnDialerNotifyAgentStateInt32StringInt64Int64StringDelegate(int dialerId, string tenantId, long campaignId, long agentId, string agentStateMsg);
        public OnDialerNotifyAgentStateInt32StringInt64Int64StringDelegate OnDialerNotifyAgentStateInt32StringInt64Int64String;

        void IDialerEventsHandler.OnDialerNotifyAgentState(int dialerId, string tenantId, long campaignId, long agentId, string agentStateMsg)
        {

            if (OnDialerNotifyAgentStateInt32StringInt64Int64String != null)
            {
                OnDialerNotifyAgentStateInt32StringInt64Int64String(dialerId, tenantId, campaignId, agentId, agentStateMsg);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerNotifyAgentState(dialerId, tenantId, campaignId, agentId, agentStateMsg);
            }
        }

        public delegate void OnDialerRequestCallsInt32StringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32Delegate(int dialerId, string requestId, string tenantId, long campaignId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount);
        public OnDialerRequestCallsInt32StringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32Delegate OnDialerRequestCallsInt32StringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32;

        void IDialerEventsHandler.OnDialerRequestCalls(int dialerId, string requestId, string tenantId, long campaignId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount)
        {

            if (OnDialerRequestCallsInt32StringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32 != null)
            {
                OnDialerRequestCallsInt32StringStringInt64NullableOfInt32CallsSelectionAlgorithmInt32(dialerId, requestId, tenantId, campaignId, groupId, callsSelectionAlgorithm, callCount);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerRequestCalls(dialerId, requestId, tenantId, campaignId, groupId, callsSelectionAlgorithm, callCount);
            }
        }

        public delegate void OnDialerScreenPopInt32StringInt64Int32StringInt32DialingModeDelegate(int dialerId, string customerId, long campaignId, int agentId, string contactId, int callId, DialingMode callDialingMode);
        public OnDialerScreenPopInt32StringInt64Int32StringInt32DialingModeDelegate OnDialerScreenPopInt32StringInt64Int32StringInt32DialingMode;

        void IDialerEventsHandler.OnDialerScreenPop(int dialerId, string customerId, long campaignId, int agentId, string contactId, int callId, DialingMode callDialingMode)
        {

            if (OnDialerScreenPopInt32StringInt64Int32StringInt32DialingMode != null)
            {
                OnDialerScreenPopInt32StringInt64Int32StringInt32DialingMode(dialerId, customerId, campaignId, agentId, contactId, callId, callDialingMode);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerScreenPop(dialerId, customerId, campaignId, agentId, contactId, callId, callDialingMode);
            }
        }

        public delegate void OnDialerNotifyInboundCallInt32Int32StringStringStringDelegate(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId);
        public OnDialerNotifyInboundCallInt32Int32StringStringStringDelegate OnDialerNotifyInboundCallInt32Int32StringStringString;

        void IDialerEventsHandler.OnDialerNotifyInboundCall(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId)
        {

            if (OnDialerNotifyInboundCallInt32Int32StringStringString != null)
            {
                OnDialerNotifyInboundCallInt32Int32StringStringString(dialerId, companyId, ddiNumber, cliNumber, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerNotifyInboundCall(dialerId, companyId, ddiNumber, cliNumber, inboundCallId);
            }
        }

        public delegate void OnDialerNotifyCallDroppedByRespondentInt32StringInt64Int64Int64Delegate(int dialerId, string companyId, long campaignId, long agentId, long callId);
        public OnDialerNotifyCallDroppedByRespondentInt32StringInt64Int64Int64Delegate OnDialerNotifyCallDroppedByRespondentInt32StringInt64Int64Int64;

        void IDialerEventsHandler.OnDialerNotifyCallDroppedByRespondent(int dialerId, string companyId, long campaignId, long agentId, long callId)
        {

            if (OnDialerNotifyCallDroppedByRespondentInt32StringInt64Int64Int64 != null)
            {
                OnDialerNotifyCallDroppedByRespondentInt32StringInt64Int64Int64(dialerId, companyId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerNotifyCallDroppedByRespondent(dialerId, companyId, campaignId, agentId, callId);
            }
        }

        public delegate void OnDialerNotifyInboundCallDroppedByRespondentInt32Int32StringDelegate(int dialerId, int companyId, string inboundCallId);
        public OnDialerNotifyInboundCallDroppedByRespondentInt32Int32StringDelegate OnDialerNotifyInboundCallDroppedByRespondentInt32Int32String;

        void IDialerEventsHandler.OnDialerNotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId)
        {

            if (OnDialerNotifyInboundCallDroppedByRespondentInt32Int32String != null)
            {
                OnDialerNotifyInboundCallDroppedByRespondentInt32Int32String(dialerId, companyId, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerNotifyInboundCallDroppedByRespondent(dialerId, companyId, inboundCallId);
            }
        }

        public delegate void OnDialerIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringStringDelegate(int dialerId, string companyId, long surveyId, long agentId, KeyValuePair<string, string>[] variables);
        public OnDialerIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringStringDelegate OnDialerIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringString;

        void IDialerEventsHandler.OnDialerIvrSubmit(int dialerId, string companyId, long surveyId, long agentId, KeyValuePair<string, string>[] variables)
        {

            if (OnDialerIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringString != null)
            {
                OnDialerIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringString(dialerId, companyId, surveyId, agentId, variables);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerIvrSubmit(dialerId, companyId, surveyId, agentId, variables);
            }
        }

        public delegate void OnTransferStateInt32Int32StringTransferStateDelegate(int dialerId, int companyId, string transferId, TransferState transferState);
        public OnTransferStateInt32Int32StringTransferStateDelegate OnTransferStateInt32Int32StringTransferState;

        void IDialerEventsHandler.OnTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {

            if (OnTransferStateInt32Int32StringTransferState != null)
            {
                OnTransferStateInt32Int32StringTransferState(dialerId, companyId, transferId, transferState);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnTransferState(dialerId, companyId, transferId, transferState);
            }
        }

        public delegate void OnDialerNotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome);
        public OnDialerNotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate OnDialerNotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome;

        void IDialerEventsHandler.OnDialerNotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome)
        {

            if (OnDialerNotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome != null)
            {
                OnDialerNotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
            } else if (_inner != null)
            {
                ((IDialerEventsHandler)_inner).OnDialerNotifyCustomIvrInterviewEnd(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
            }
        }

    }
}