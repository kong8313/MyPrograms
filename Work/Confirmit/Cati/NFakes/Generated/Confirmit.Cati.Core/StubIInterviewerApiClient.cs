using System;
using Confirmit.CATI.Core.Services.Interfaces;
using ConfirmitDialerInterface;
using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInterviewerApiClient : IInterviewerApiClient 
    {
        private IInterviewerApiClient _inner;

        public StubIInterviewerApiClient()
        {
            _inner = null;
        }

        public IInterviewerApiClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void NotifySchedulingInt32Delegate(int companyId);
        public NotifySchedulingInt32Delegate NotifySchedulingInt32;

        void IInterviewerApiClient.NotifyScheduling(int companyId)
        {

            if (NotifySchedulingInt32 != null)
            {
                NotifySchedulingInt32(companyId);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyScheduling(companyId);
            }
        }

        public delegate void NotifyConsoleTerminatingInt32Int32NullableOfInt64Delegate(int companyId, int personId, long? monitoringSessionId);
        public NotifyConsoleTerminatingInt32Int32NullableOfInt64Delegate NotifyConsoleTerminatingInt32Int32NullableOfInt64;

        void IInterviewerApiClient.NotifyConsoleTerminating(int companyId, int personId, long? monitoringSessionId)
        {

            if (NotifyConsoleTerminatingInt32Int32NullableOfInt64 != null)
            {
                NotifyConsoleTerminatingInt32Int32NullableOfInt64(companyId, personId, monitoringSessionId);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyConsoleTerminating(companyId, personId, monitoringSessionId);
            }
        }

        public delegate void NotifyUpdatingLiveMonitoringStateBooleanInt32Int32Delegate(bool liveMonitoringStarted, int companyId, int personId);
        public NotifyUpdatingLiveMonitoringStateBooleanInt32Int32Delegate NotifyUpdatingLiveMonitoringStateBooleanInt32Int32;

        void IInterviewerApiClient.NotifyUpdatingLiveMonitoringState(bool liveMonitoringStarted, int companyId, int personId)
        {

            if (NotifyUpdatingLiveMonitoringStateBooleanInt32Int32 != null)
            {
                NotifyUpdatingLiveMonitoringStateBooleanInt32Int32(liveMonitoringStarted, companyId, personId);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyUpdatingLiveMonitoringState(liveMonitoringStarted, companyId, personId);
            }
        }

        public delegate void NotifyOutcomeInt32Int32StringInt64Int32StringInt64CallOutcomeStringInt32DictionaryOfStringStringDelegate(int companyId, int dialerId, string tenantId, long campaignId, int personId, string contactId, long callId, CallOutcome callOutcome, string dialerCallerId, int ringTime, Dictionary<string, string> callOutcomeMetadata);
        public NotifyOutcomeInt32Int32StringInt64Int32StringInt64CallOutcomeStringInt32DictionaryOfStringStringDelegate NotifyOutcomeInt32Int32StringInt64Int32StringInt64CallOutcomeStringInt32DictionaryOfStringString;

        void IInterviewerApiClient.NotifyOutcome(int companyId, int dialerId, string tenantId, long campaignId, int personId, string contactId, long callId, CallOutcome callOutcome, string dialerCallerId, int ringTime, Dictionary<string, string> callOutcomeMetadata)
        {

            if (NotifyOutcomeInt32Int32StringInt64Int32StringInt64CallOutcomeStringInt32DictionaryOfStringString != null)
            {
                NotifyOutcomeInt32Int32StringInt64Int32StringInt64CallOutcomeStringInt32DictionaryOfStringString(companyId, dialerId, tenantId, campaignId, personId, contactId, callId, callOutcome, dialerCallerId, ringTime, callOutcomeMetadata);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyOutcome(companyId, dialerId, tenantId, campaignId, personId, contactId, callId, callOutcome, dialerCallerId, ringTime, callOutcomeMetadata);
            }
        }

        public delegate void NotifyUpdatingAgentStateInt32Int32StringInt64Int32AgentStateMsgsDelegate(int companyId, int dialerId, string tenantId, long campaignId, int personId, AgentStateMsgs agentState);
        public NotifyUpdatingAgentStateInt32Int32StringInt64Int32AgentStateMsgsDelegate NotifyUpdatingAgentStateInt32Int32StringInt64Int32AgentStateMsgs;

        void IInterviewerApiClient.NotifyUpdatingAgentState(int companyId, int dialerId, string tenantId, long campaignId, int personId, AgentStateMsgs agentState)
        {

            if (NotifyUpdatingAgentStateInt32Int32StringInt64Int32AgentStateMsgs != null)
            {
                NotifyUpdatingAgentStateInt32Int32StringInt64Int32AgentStateMsgs(companyId, dialerId, tenantId, campaignId, personId, agentState);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyUpdatingAgentState(companyId, dialerId, tenantId, campaignId, personId, agentState);
            }
        }

        public delegate void NotifyScreenPopInt32Int32StringInt64Int32StringInt32DialingModeDelegate(int companyId, int dialerId, string customerId, long campaignId, int personId, string contactId, int callId, DialingMode callDialingMode);
        public NotifyScreenPopInt32Int32StringInt64Int32StringInt32DialingModeDelegate NotifyScreenPopInt32Int32StringInt64Int32StringInt32DialingMode;

        void IInterviewerApiClient.NotifyScreenPop(int companyId, int dialerId, string customerId, long campaignId, int personId, string contactId, int callId, DialingMode callDialingMode)
        {

            if (NotifyScreenPopInt32Int32StringInt64Int32StringInt32DialingMode != null)
            {
                NotifyScreenPopInt32Int32StringInt64Int32StringInt32DialingMode(companyId, dialerId, customerId, campaignId, personId, contactId, callId, callDialingMode);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyScreenPop(companyId, dialerId, customerId, campaignId, personId, contactId, callId, callDialingMode);
            }
        }

        public delegate void NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64Delegate(int companyId, int dialerId, long campaignId, int personId, long callId);
        public NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64Delegate NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64;

        void IInterviewerApiClient.NotifyCallDroppedByRespondent(int companyId, int dialerId, long campaignId, int personId, long callId)
        {

            if (NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64 != null)
            {
                NotifyCallDroppedByRespondentInt32Int32Int64Int32Int64(companyId, dialerId, campaignId, personId, callId);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyCallDroppedByRespondent(companyId, dialerId, campaignId, personId, callId);
            }
        }

        public delegate void NotifyUpdatingTransferStateInt32Int32StringConsoleTransferStateDelegate(int companyId, int dialerId, string transferId, ConsoleTransferState consoleTransferState);
        public NotifyUpdatingTransferStateInt32Int32StringConsoleTransferStateDelegate NotifyUpdatingTransferStateInt32Int32StringConsoleTransferState;

        void IInterviewerApiClient.NotifyUpdatingTransferState(int companyId, int dialerId, string transferId, ConsoleTransferState consoleTransferState)
        {

            if (NotifyUpdatingTransferStateInt32Int32StringConsoleTransferState != null)
            {
                NotifyUpdatingTransferStateInt32Int32StringConsoleTransferState(companyId, dialerId, transferId, consoleTransferState);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyUpdatingTransferState(companyId, dialerId, transferId, consoleTransferState);
            }
        }

        public delegate void NotifyTransferFinishedInt32Int32Int32StringDelegate(int companyId, int surveyId, int interviewId, string transferId);
        public NotifyTransferFinishedInt32Int32Int32StringDelegate NotifyTransferFinishedInt32Int32Int32String;

        void IInterviewerApiClient.NotifyTransferFinished(int companyId, int surveyId, int interviewId, string transferId)
        {

            if (NotifyTransferFinishedInt32Int32Int32String != null)
            {
                NotifyTransferFinishedInt32Int32Int32String(companyId, surveyId, interviewId, transferId);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyTransferFinished(companyId, surveyId, interviewId, transferId);
            }
        }

        public delegate void NotifyAutomaticSurveyChangedInt32Int32Int32Delegate(int companyId, int personId, int surveyId);
        public NotifyAutomaticSurveyChangedInt32Int32Int32Delegate NotifyAutomaticSurveyChangedInt32Int32Int32;

        void IInterviewerApiClient.NotifyAutomaticSurveyChanged(int companyId, int personId, int surveyId)
        {

            if (NotifyAutomaticSurveyChangedInt32Int32Int32 != null)
            {
                NotifyAutomaticSurveyChangedInt32Int32Int32(companyId, personId, surveyId);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyAutomaticSurveyChanged(companyId, personId, surveyId);
            }
        }

        public delegate void NotifyNewMessageInt32IEnumerableOfInt32StringStringDelegate(int companyId, IEnumerable<int> personIds, string message, string supervisorName);
        public NotifyNewMessageInt32IEnumerableOfInt32StringStringDelegate NotifyNewMessageInt32IEnumerableOfInt32StringString;

        void IInterviewerApiClient.NotifyNewMessage(int companyId, IEnumerable<int> personIds, string message, string supervisorName)
        {

            if (NotifyNewMessageInt32IEnumerableOfInt32StringString != null)
            {
                NotifyNewMessageInt32IEnumerableOfInt32StringString(companyId, personIds, message, supervisorName);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyNewMessage(companyId, personIds, message, supervisorName);
            }
        }

        public delegate void NotifyIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringStringDelegate(int dialerId, string companyId, long campaignId, long agentId, KeyValuePair<string, string>[] variables);
        public NotifyIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringStringDelegate NotifyIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringString;

        void IInterviewerApiClient.NotifyIvrSubmit(int dialerId, string companyId, long campaignId, long agentId, KeyValuePair<string, string>[] variables)
        {

            if (NotifyIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringString != null)
            {
                NotifyIvrSubmitInt32StringInt64Int64ArrayOfKeyValuePairOfStringString(dialerId, companyId, campaignId, agentId, variables);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyIvrSubmit(dialerId, companyId, campaignId, agentId, variables);
            }
        }

        public delegate void NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome);
        public NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcomeDelegate NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome;

        void IInterviewerApiClient.NotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId, int agentId, int interviewId, long callId, CallOutcome callOutcome)
        {

            if (NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome != null)
            {
                NotifyCustomIvrInterviewEndInt32Int32Int64Int32Int32Int64CallOutcome(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
            } else if (_inner != null)
            {
                ((IInterviewerApiClient)_inner).NotifyCustomIvrInterviewEnd(dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome);
            }
        }

    }
}