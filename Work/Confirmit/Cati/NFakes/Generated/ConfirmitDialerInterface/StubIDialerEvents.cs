using System;
using ConfirmitDialerInterface;
using System.Collections.Generic;

namespace ConfirmitDialerInterface.Fakes
{
    public class StubIDialerEvents : IDialerEvents 
    {
        private IDialerEvents _inner;

        public StubIDialerEvents()
        {
            _inner = null;
        }

        public IDialerEvents Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void NotifyDialerStateInt32Int32DialerStateDelegate(int companyId, int dialerId, DialerState dialerState);
        public NotifyDialerStateInt32Int32DialerStateDelegate NotifyDialerStateInt32Int32DialerState;

        void IDialerEvents.NotifyDialerState(int companyId, int dialerId, DialerState dialerState)
        {

            if (NotifyDialerStateInt32Int32DialerState != null)
            {
                NotifyDialerStateInt32Int32DialerState(companyId, dialerId, dialerState);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyDialerState(companyId, dialerId, dialerState);
            }
        }

        public delegate void NotifyAgentStateInt32Int32Int64Int32AgentStateDelegate(int companyId, int dialerId, long campaignId, int agentId, AgentState agentState);
        public NotifyAgentStateInt32Int32Int64Int32AgentStateDelegate NotifyAgentStateInt32Int32Int64Int32AgentState;

        void IDialerEvents.NotifyAgentState(int companyId, int dialerId, long campaignId, int agentId, AgentState agentState)
        {

            if (NotifyAgentStateInt32Int32Int64Int32AgentState != null)
            {
                NotifyAgentStateInt32Int32Int64Int32AgentState(companyId, dialerId, campaignId, agentId, agentState);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyAgentState(companyId, dialerId, campaignId, agentId, agentState);
            }
        }

        public delegate void NotifyOutcomeInt32Int32Int64Int32Int32Int64CallOutcomeStringTimeSpanDictionaryOfStringStringStringDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, CallOutcome outcome, string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata, string correlationId);
        public NotifyOutcomeInt32Int32Int64Int32Int32Int64CallOutcomeStringTimeSpanDictionaryOfStringStringStringDelegate NotifyOutcomeInt32Int32Int64Int32Int32Int64CallOutcomeStringTimeSpanDictionaryOfStringStringString;

        void IDialerEvents.NotifyOutcome(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, CallOutcome outcome, string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata, string correlationId)
        {

            if (NotifyOutcomeInt32Int32Int64Int32Int32Int64CallOutcomeStringTimeSpanDictionaryOfStringStringString != null)
            {
                NotifyOutcomeInt32Int32Int64Int32Int32Int64CallOutcomeStringTimeSpanDictionaryOfStringStringString(companyId, dialerId, campaignId, agentId, interviewId, callId, outcome, callerId, ringTime, callOutcomeMetadata, correlationId);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyOutcome(companyId, dialerId, campaignId, agentId, interviewId, callId, outcome, callerId, ringTime, callOutcomeMetadata, correlationId);
            }
        }

        public delegate void NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome);
        public NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome;

        void IDialerEvents.NotifyCustomIvrInterviewEnd(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome)
        {

            if (NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome != null)
            {
                NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome(companyId, dialerId, campaignId, agentId, interviewId, callId, callOutcome);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyCustomIvrInterviewEnd(companyId, dialerId, campaignId, agentId, interviewId, callId, callOutcome);
            }
        }

        public delegate void NotifyInboundCallInt32Int32StringStringStringDelegate(int companyId, int dialerId, string ddiNumber, string cliNumber, string inboundCallId);
        public NotifyInboundCallInt32Int32StringStringStringDelegate NotifyInboundCallInt32Int32StringStringString;

        void IDialerEvents.NotifyInboundCall(int companyId, int dialerId, string ddiNumber, string cliNumber, string inboundCallId)
        {

            if (NotifyInboundCallInt32Int32StringStringString != null)
            {
                NotifyInboundCallInt32Int32StringStringString(companyId, dialerId, ddiNumber, cliNumber, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyInboundCall(companyId, dialerId, ddiNumber, cliNumber, inboundCallId);
            }
        }

        public delegate void NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64Delegate(int companyId, int dialerId, long campaignId, int agentId, long callId);
        public NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64Delegate NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64;

        void IDialerEvents.NotifyCallDroppedByRespondent(int companyId, int dialerId, long campaignId, int agentId, long callId)
        {

            if (NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64 != null)
            {
                NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64(companyId, dialerId, campaignId, agentId, callId);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyCallDroppedByRespondent(companyId, dialerId, campaignId, agentId, callId);
            }
        }

        public delegate void NotifyInboundCallDroppedByRespondentInt32Int32StringDelegate(int companyId, int dialerId, string inboundCallId);
        public NotifyInboundCallDroppedByRespondentInt32Int32StringDelegate NotifyInboundCallDroppedByRespondentInt32Int32String;

        void IDialerEvents.NotifyInboundCallDroppedByRespondent(int companyId, int dialerId, string inboundCallId)
        {

            if (NotifyInboundCallDroppedByRespondentInt32Int32String != null)
            {
                NotifyInboundCallDroppedByRespondentInt32Int32String(companyId, dialerId, inboundCallId);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyInboundCallDroppedByRespondent(companyId, dialerId, inboundCallId);
            }
        }

        public delegate void ScreenPopInt32Int32Int64Int32Int32Int64DialingModeDelegate(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, DialingMode callDialingMode);
        public ScreenPopInt32Int32Int64Int32Int32Int64DialingModeDelegate ScreenPopInt32Int32Int64Int32Int32Int64DialingMode;

        void IDialerEvents.ScreenPop(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId, DialingMode callDialingMode)
        {

            if (ScreenPopInt32Int32Int64Int32Int32Int64DialingMode != null)
            {
                ScreenPopInt32Int32Int64Int32Int32Int64DialingMode(companyId, dialerId, campaignId, agentId, interviewId, callId, callDialingMode);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).ScreenPop(companyId, dialerId, campaignId, agentId, interviewId, callId, callDialingMode);
            }
        }

        public delegate void RequestCallsStringInt32Int32Int64Int32CallsSelectionAlgorithmInt32Delegate(string requestId, int companyId, int dialerId, long campaignId, int groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount);
        public RequestCallsStringInt32Int32Int64Int32CallsSelectionAlgorithmInt32Delegate RequestCallsStringInt32Int32Int64Int32CallsSelectionAlgorithmInt32;

        void IDialerEvents.RequestCalls(string requestId, int companyId, int dialerId, long campaignId, int groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount)
        {

            if (RequestCallsStringInt32Int32Int64Int32CallsSelectionAlgorithmInt32 != null)
            {
                RequestCallsStringInt32Int32Int64Int32CallsSelectionAlgorithmInt32(requestId, companyId, dialerId, campaignId, groupId, callsSelectionAlgorithm, callCount);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).RequestCalls(requestId, companyId, dialerId, campaignId, groupId, callsSelectionAlgorithm, callCount);
            }
        }

        public delegate void NotifyIvrSubmitInt32Int32Int64Int32ArrayOfKeyValuePairOfStringStringDelegate(int companyId, int dialerId, long campaignId, int agentId, KeyValuePair<string, string>[] variables);
        public NotifyIvrSubmitInt32Int32Int64Int32ArrayOfKeyValuePairOfStringStringDelegate NotifyIvrSubmitInt32Int32Int64Int32ArrayOfKeyValuePairOfStringString;

        void IDialerEvents.NotifyIvrSubmit(int companyId, int dialerId, long campaignId, int agentId, KeyValuePair<string, string>[] variables)
        {

            if (NotifyIvrSubmitInt32Int32Int64Int32ArrayOfKeyValuePairOfStringString != null)
            {
                NotifyIvrSubmitInt32Int32Int64Int32ArrayOfKeyValuePairOfStringString(companyId, dialerId, campaignId, agentId, variables);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyIvrSubmit(companyId, dialerId, campaignId, agentId, variables);
            }
        }

        public delegate void NotifyTransferStateInt32Int32StringTransferStateDelegate(int companyId, int dialerId, string transferId, TransferState transferState);
        public NotifyTransferStateInt32Int32StringTransferStateDelegate NotifyTransferStateInt32Int32StringTransferState;

        void IDialerEvents.NotifyTransferState(int companyId, int dialerId, string transferId, TransferState transferState)
        {

            if (NotifyTransferStateInt32Int32StringTransferState != null)
            {
                NotifyTransferStateInt32Int32StringTransferState(companyId, dialerId, transferId, transferState);
            } else if (_inner != null)
            {
                ((IDialerEvents)_inner).NotifyTransferState(companyId, dialerId, transferId, transferState);
            }
        }

    }
}