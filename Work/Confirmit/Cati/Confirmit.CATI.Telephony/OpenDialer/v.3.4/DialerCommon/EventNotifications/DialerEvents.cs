using System;
using System.Collections.Generic;
using System.Linq;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    public enum DialerEventPriority
    {
        LowPriority,
        HighPriority
    }

    /// <summary>
    /// Base Event class
    /// </summary>
    public abstract class DialerEvent
    {
        public int CompanyId { get; protected set; }

        public int DialerId { get; protected set; }

        public long SurveyId { get; protected set; }

        public DialerEventPriority Priority { get; private set; }

        public bool KeepInQueueOnCommunicationFailure { get; protected set; }

        protected DialerEvent(DialerEventPriority eventPriority, int companyId, int dialerId, long surveyId) :
            this(eventPriority, companyId, dialerId, surveyId, true)
        {
        }

        protected DialerEvent(
            DialerEventPriority eventPriority,
            int companyId,
            int dialerId,
            long surveyId,
            bool keepInQueueOnCommunicationFailure)
        {
            Priority = eventPriority;
            CompanyId = companyId;
            DialerId = dialerId;
            SurveyId = surveyId;
            KeepInQueueOnCommunicationFailure = keepInQueueOnCommunicationFailure;
        }

        public void SetDialerIdIfEmpty(int dialerId)
        {
            if (DialerId == 0)
            {
                DialerId = dialerId;
            }
        }

        public abstract void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient);

        public override string ToString()
        {
            return string.Format(
                "{0}[priority={1}, companyId={2}, dialerId={3}, surveyId={4}",
                GetType().Name, Priority.ToString().Substring(0, 2), CompanyId, DialerId, SurveyId);
        }
    }

    public class DialerEventScreenPop : DialerEvent
    {
        public int AgentId { get; private set; }
        public long CallId { get; private set; }

        public DialerEventScreenPop(DialerEventPriority eventPriority, int companyId, long surveyId, int agentId, long callId)
            : this(eventPriority, companyId, 0, surveyId, agentId, callId)
        {
        }

        public DialerEventScreenPop(DialerEventPriority eventPriority, int companyId, int dialerId, long surveyId, int agentId, long callId)
            : base(eventPriority, companyId, dialerId, surveyId)
        {
            AgentId = agentId;
            CallId = callId;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.ScreenPop(
                DialerId,
                string.Empty, // requestId parameter is not currently used
                string.Empty, // sessionId parameter is not currently used
                CompanyId.ToString(), //TODO CODI changes: propagate int for CompanyId into DialerEventsServiceClient
                SurveyId,
                AgentId.ToString(), //TODO CODI changes: propagate int for AgentId into DialerEventsServiceClient
                string.Empty, // contactId parameter is not currently used
                (int)CallId, //TODO CODI changes: propagate long for CallId into DialerEventsServiceClient
                DialingMode.Preview);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, agentId={1}, callId={2}]",
                base.ToString(), AgentId, CallId);
        }
    }

    public class DialerEventNotifyOutcome : DialerEvent
    {
        public int AgentId { get; private set; }
        public long CallId { get; private set; }
        public int OutcomeCode { get; set; }
        public string DialerAccompanyingCallInfo { get; private set; }

        public DialerEventNotifyOutcome(DialerEventPriority eventPriority, int companyId, long surveyId,
            int agentId, long callId, int outcomeCode, string dialerAccompanyingCallInfo)
            : this(eventPriority, companyId, 0, surveyId, agentId, callId, outcomeCode, dialerAccompanyingCallInfo)
        {
        }

        public DialerEventNotifyOutcome(DialerEventPriority eventPriority, int companyId, int dialerId, long surveyId, int agentId, long callId, int outcomeCode, string dialerAccompanyingCallInfo)
            : base(eventPriority, companyId, dialerId, surveyId)
        {
            AgentId = agentId;
            CallId = callId;
            OutcomeCode = outcomeCode;
            DialerAccompanyingCallInfo = dialerAccompanyingCallInfo;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyOutcome(
                DialerId,
                string.Empty, // requestId parameter is not currently used
                string.Empty, // sessionId parameter is not currently used
                CompanyId.ToString(), //TODO CODI changes: propagate int for CompanyId into DialerEventsServiceClient
                SurveyId,
                AgentId.ToString(), //TODO CODI changes: propagate int for AgentId into DialerEventsServiceClient
                string.Empty, // contactId parameter is not currently used
                (int)CallId, //TODO CODI changes: propagate long for CallId into DialerEventsServiceClient
                "1", // outcomeType is not currently used in the Common Dialer API. 
                     // But "1" is hardcoded here to make it similar the MN dialer API
                OutcomeCode.ToString(),
                DialerAccompanyingCallInfo);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, agentId={1}, callId={2}, outcomeCode={3}]",
                base.ToString(), AgentId, CallId, OutcomeCode);
        }
    }

    public class DialerEventNotifyInboundCall : DialerEvent
    {
        public string InboundLinePhoneNumber { get; private set; }
        public string CallerPhoneNumber { get; private set; }
        public int InboundCallId { get; private set; }

        public DialerEventNotifyInboundCall(DialerEventPriority eventPriority, int companyId, int dialerId, string inboundLinePhoneNumber, string callerPhoneNumber, int inboundCallId)
            : base(eventPriority, companyId, dialerId, 0)
        {
            InboundLinePhoneNumber = inboundLinePhoneNumber;
            CallerPhoneNumber = callerPhoneNumber;
            InboundCallId = inboundCallId;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyInboundCall(
                DialerId,
                CompanyId,
                InboundLinePhoneNumber,
                CallerPhoneNumber,
                InboundCallId);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, InboundLinePhoneNumber={1}, CallerPhoneNumber={2}, InboundCallId={3}]",
                base.ToString(), InboundLinePhoneNumber, CallerPhoneNumber, InboundCallId);
        }
    }

    public class DialerEventRequestCalls : DialerEvent
    {
        public int CallCount { get; private set; }
        private readonly CallsSelectionAlgorithm _callsSelectionAlgorithm;
        private readonly string _requestId;
        private readonly int _groupId;

        public DialerEventRequestCalls(
            DialerEventPriority eventPriority,
            string requestId,
            int companyId,
            long surveyId,
            int groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int callCount)
            : this(eventPriority, requestId, companyId, 0, surveyId, groupId, callsSelectionAlgorithm, callCount)
        {
        }

        public DialerEventRequestCalls(
            DialerEventPriority eventPriority, 
            string requestId,
            int companyId,
            int dialerId, 
            long surveyId,
            int groupId, 
            CallsSelectionAlgorithm callsSelectionAlgorithm, 
            int callCount)
            : base(eventPriority, companyId, dialerId, surveyId, false)
        {
            _requestId = requestId;
            _groupId = groupId;
            _callsSelectionAlgorithm = callsSelectionAlgorithm;
            CallCount = callCount;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.RequestCalls(
                DialerId,
                _requestId,
                string.Empty, // sessionId parameter is not currently used
                CompanyId.ToString(), //TODO CODI changes: propagate int for CompanyId into DialerEventsServiceClient
                SurveyId,
                _groupId,
                _callsSelectionAlgorithm,
                CallCount);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, requestId={1}, groupId={2}, callsSelectionAlgorithm={3}({4}), callCount={5}]",
                base.ToString(), _requestId, _groupId, _callsSelectionAlgorithm, (int)_callsSelectionAlgorithm, CallCount);
        }
    }

    public class DialerEventNotifyUserState : DialerEvent
    {
        private readonly int _agentId;
        private readonly AgentState _agentState;

        public DialerEventNotifyUserState(DialerEventPriority eventPriority, int companyId, long surveyId,
            int agentId, AgentState agentState)
            : this(eventPriority, companyId, 0, surveyId, agentId, agentState)
        {
        }

        public DialerEventNotifyUserState(DialerEventPriority eventPriority, int companyId, int dialerId, long surveyId, int agentId, AgentState agentState)
            : base(eventPriority, companyId, dialerId, surveyId)
        {
            _agentId = agentId;
            _agentState = agentState;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyUserState(
                DialerId,
                string.Empty, // requestId parameter is not currently used
                string.Empty, // sessionId parameter is not currently used
                CompanyId.ToString(), //TODO CODI changes: propagate int for CompanyId into DialerEventsServiceClient
                SurveyId,
                _agentId.ToString(), //TODO CODI changes: propagate int for AgentId into DialerEventsServiceClient
                ((int)_agentState).ToString()); //TODO: AgentState should be used?
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, agentId={1}, agentState={2}({3})]",
                base.ToString(), _agentId, _agentState, (int)_agentState);
        }
    }

    public class DialerEventNotifyDialerState : DialerEvent
    {
        private readonly DialerState _dialerState;

        public DialerEventNotifyDialerState(DialerEventPriority eventPriority, int companyId, int dialerId, DialerState dialerState)
            : base(eventPriority, companyId, dialerId, 0, false)
        {
            _dialerState = dialerState;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyDialerState(
                DialerId,
                CompanyId.ToString(), //TODO CODI changes: propagate int for CompanyId into DialerEventsServiceClient
                _dialerState);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, dialerState={1}]",
                base.ToString(), _dialerState);
        }
    }

    public class DialerEventNotifyLicenseExpiration : DialerEvent
    {
        private readonly DateTime _licenseExpirationDateTime;

        public DialerEventNotifyLicenseExpiration(DialerEventPriority eventPriority, DateTime licenseExpirationDateTime)
            : this(eventPriority, 0, licenseExpirationDateTime)
        {
        }

        public DialerEventNotifyLicenseExpiration(DialerEventPriority eventPriority, int dialerId, DateTime licenseExpirationDateTime)
            : base(eventPriority, 0, dialerId, 0, false)
        {
            _licenseExpirationDateTime = licenseExpirationDateTime;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyLicenseExpiration(DialerId, _licenseExpirationDateTime);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, licenseExpirationDateTime={1}]",
                base.ToString(), _licenseExpirationDateTime.ToLongDateString());
        }
    }

    public class DialerEventNotifyIvrSubmit : DialerEvent
    {
        public int AgentId { get; private set; }
        public KeyValuePair<string, string>[] Variables { get; private set; }

        public DialerEventNotifyIvrSubmit(DialerEventPriority eventPriority, int companyId, int dialerId, long surveyId, int agentId, KeyValuePair<string, string>[] variables)
            : base(eventPriority, companyId, dialerId, surveyId)
        {
            AgentId = agentId;
            Variables = variables;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyIvrSubmit(
                DialerId,
                CompanyId.ToString(), //TODO CODI changes: propagate int for CompanyId into DialerEventsServiceClient,
                SurveyId,
                AgentId.ToString(), //TODO CODI changes: propagate int for AgentId into DialerEventsServiceClient
                Variables);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, Variables=[{1}]",
                base.ToString(), string.Join(", ", Variables.Select(x => x.Key + ": " + x.Value)));
        }
    }
}
