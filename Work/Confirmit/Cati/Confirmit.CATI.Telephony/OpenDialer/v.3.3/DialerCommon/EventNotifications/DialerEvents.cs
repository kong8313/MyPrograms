using System;
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

        public long SurveyId { get; protected set; }

        public DialerEventPriority Priority { get; private set; }

        public bool KeepInQueueOnCommunicationFailure { get; protected set; }

        protected DialerEvent(DialerEventPriority eventPriority, int companyId, long surveyId) :
            this(eventPriority, companyId, surveyId, true)
        {
        }

        protected DialerEvent(
            DialerEventPriority eventPriority, 
            int companyId, 
            long surveyId, 
            bool keepInQueueOnCommunicationFailure)
        {
            Priority = eventPriority;
            CompanyId = companyId;
            SurveyId = surveyId;
            KeepInQueueOnCommunicationFailure = keepInQueueOnCommunicationFailure;
        }

        public abstract void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient);

        public override string ToString()
        {
            return string.Format(
                "{0}[priority={1}, companyId={2}, surveyId={3}",
                GetType().Name, Priority.ToString().Substring(0, 2), CompanyId, SurveyId);
        }
    }

    public class DialerEventScreenPop : DialerEvent
    {
        public int AgentId { get; private set; }
        public long CallId { get; private set; }

        public DialerEventScreenPop(DialerEventPriority eventPriority, int companyId, long surveyId, int agentId, long callId)
            : base(eventPriority, companyId, surveyId)
        {
            AgentId = agentId;
            CallId = callId;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.ScreenPop(
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

        public DialerEventNotifyOutcome(DialerEventPriority eventPriority, int companyId, long surveyId, int agentId, long callId, int outcomeCode, string dialerAccompanyingCallInfo)
            : base(eventPriority, companyId, surveyId)
        {
            AgentId = agentId;
            CallId = callId;
            OutcomeCode = outcomeCode;
            DialerAccompanyingCallInfo = dialerAccompanyingCallInfo;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyOutcome(
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
            : base(eventPriority, companyId, surveyId, false)
        {
            _requestId = requestId;
            _groupId = groupId;
            _callsSelectionAlgorithm = callsSelectionAlgorithm;
            CallCount = callCount;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.RequestCalls(
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

        public DialerEventNotifyUserState(DialerEventPriority eventPriority, int companyId, long surveyId, int agentId, AgentState agentState)
            : base(eventPriority, companyId, surveyId)
        {
            _agentId = agentId;
            _agentState = agentState;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyUserState(
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

        public DialerEventNotifyDialerState(DialerEventPriority eventPriority, DialerState dialerState)
            : base(eventPriority, 0, 0, false)
        {
            _dialerState = dialerState;
        }

        public DialerEventNotifyDialerState(DialerEventPriority eventPriority, DialerState dialerState, int companyId)
            : base(eventPriority, companyId, 0, false)
        {
            _dialerState = dialerState;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyDialerState(
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
            : base(eventPriority, 0, 0, false)
        {
            _licenseExpirationDateTime = licenseExpirationDateTime;
        }

        public override void SendEventNotification(DialerEventsServiceClient dialerEventsHandlerServiceClient)
        {
            dialerEventsHandlerServiceClient.NotifyLicenseExpiration(_licenseExpirationDateTime);
        }

        public override string ToString()
        {
            return string.Format(
                "{0}, licenseExpirationDateTime={1}]",
                base.ToString(), _licenseExpirationDateTime.ToLongDateString());
        }
    }
}
