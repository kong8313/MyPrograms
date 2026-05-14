using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Telephony.DialerCommon;

using ConfirmitDialerInterface;

namespace DialerIntegrationTests.Framework
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class TestDialerEventsHandlerService : IDialerEventsHandlerService
    {
        public static string ExpectedNotificationMethod { get; set; }

        public static string LastCalledMethod { get; private set; }

        public static AgentStateMsgs LastRecievedUserState { get; private set; }

        public static string LastRecievedOutcome { get; private set; }

        public static int LastReceivedNumberOfCalls { get; private set; }

        public static int? LastReceivedGroupIdForRequestCalls { get; private set; }

        public static int LastDialerState { get; private set; }    

        private static readonly ManualResetEvent notifyEvent = new ManualResetEvent(false);

        public static ManualResetEvent NotifyEvent
        {
            get
            {
                return notifyEvent;
            }
        }

        public TestDialerEventsHandlerService()
        {
            TraceInformation("TestDialerEventsHandlerService object is created");
        }

        /// <summary>
        /// This function is being used to pass back information about the status of the User. 
        /// It is especially needed for asynchronous functions like login, logout, go ready, etc. 
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="userState"></param>
        public void NotifyUserState(
            int dialerId, 
            string requestId, 
            string sessionId, 
            string companyId, 
            long surveyId, 
            string userId, 
            string userState)
        {
            var castedUserState = (AgentStateMsgs) Enum.Parse(typeof(AgentStateMsgs), userState);

            TraceInformation(string.Format("NotifyUserState /// " +
                "userState={0}({1}), dialerId={2}, requestId={3}, sessionId={4}, companyId={5}, surveyId={6}, userId={7}",
                userState, castedUserState, dialerId, requestId, sessionId, companyId, surveyId, userId));
            LastCalledMethod = MethodBase.GetCurrentMethod().Name;
            LastRecievedUserState = castedUserState;

            notifyEvent.Set();
        }

        /// <summary>
        /// This function is being used to pass back the outcome for a call.
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="outcomeType"></param>
        /// <param name="outcomeCode"></param>
        public void NotifyOutcome(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId,
            string contactId, int callId, string outcomeType, string outcomeCode,
            string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata, string correlationId)
        {
            TraceInformation(string.Format("NotifyOutcome /// " +
                                           "outcomeCode={0}, dialerId={1}, requestId={2}, sessionId={3}, companyId={4}, " +
                                           "surveyId={5}, userId={6}, contactId={7}, callId={8}, outcomeType={9}," +
                                           "callerId={10}, ringTime={11}, callOutcomeMetadata={12}, correlationId={13}",
                outcomeCode, dialerId, requestId, sessionId, companyId,
                surveyId, userId, contactId, callId, outcomeType,
                callerId, ringTime, callOutcomeMetadata?.Stringify(), correlationId));

            LastCalledMethod = MethodBase.GetCurrentMethod().Name;
            LastRecievedOutcome = outcomeCode;
            notifyEvent.Set();
        }

        public void NotifyInboundCall(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId)
        {
            throw new NotImplementedException();
        }

        public void NotifyCallDroppedByRespondent(int dialerId, int companyId, long surveyId, int agentId, long callId)
        {
            throw new NotImplementedException();
        }

        public void NotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId)
        {
            throw new NotImplementedException();
        }

        public void NotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId, int agentId, int interviewId,
            long callId, CallOutcome callOutcome)
        {
            throw new NotImplementedException();
        }

        public void ScreenPop(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string contactId, int callId, DialingMode callDialingMode)
        {
            TraceInformation(string.Format("ScreenPop /// " +
                "companyId={0} surveyId={1} userId={2} callId={3} callDialingMode={4}" +
                "dialerId={5}, requestId={6}, sessionId={7}, contactId={8}",
                companyId, surveyId, userId, callId, callDialingMode,
                dialerId, requestId, sessionId, contactId));

            LastCalledMethod = MethodBase.GetCurrentMethod().Name;            
            notifyEvent.Set();
        }

        /// <summary>
        /// This function is used to pass back dialer state
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="companyId"></param>
        /// <param name="dialerState"></param>
        public void NotifyDialerState(int dialerId, string companyId, DialerState dialerState)
        {
            TraceInformation(string.Format("NotifyDialerState /// " +
                "dialerState={0}, dialerId={1}, companyId={2}",
                dialerState, dialerId, companyId));

            LastCalledMethod = MethodBase.GetCurrentMethod().Name;
            LastDialerState = (int)dialerState;
            notifyEvent.Set();
        }

        /// <summary>
        /// Dialler requests for calls (predicive)
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="groupId"></param>
        /// <param name="callsSelectionAlgorithm"></param>
        /// <param name="callCount">amount of calls the dialler requests for</param>
        public void RequestCalls(int dialerId, string requestId, string sessionId, string companyId, long surveyId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount)
        {
            TraceInformation(string.Format("RequestCalls /// " +
                "callCount={0} dialerId={1}, requestId={2}, sessionId={3}, companyId={4}, surveyId={5}, " +
                "groupId={6}, callsSelectionAlgorithm={7}",
                callCount, dialerId, requestId, sessionId, companyId, surveyId,
                groupId, callsSelectionAlgorithm));

            if (notifyEvent.WaitOne(0))
            {
                // Already received the expected notification
                return;
            }

            LastCalledMethod = MethodBase.GetCurrentMethod().Name;
            LastReceivedNumberOfCalls = callCount;
            LastReceivedGroupIdForRequestCalls = groupId;

            FireNotifyEventIfNeeded();
        }


        private static void FireNotifyEventIfNeeded()
        {
            if (LastCalledMethod == ExpectedNotificationMethod)
            {
                notifyEvent.Set();
            }
        }

        public void NotifyLicenseExpiration(int dialerId, DateTime licenseExpirationDateTime)
        {
            Trace.TraceInformation("NotifyLicenseExpiration");
            LastCalledMethod = MethodBase.GetCurrentMethod().Name;
            notifyEvent.Set();
        }

        public void NotifyIvrSubmit(int dialerId, string companyId, long surveyId, string agentId, KeyValuePair<string, string>[] variables)
        {
            throw new NotImplementedException();
        }

        public void NotifyTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {
            throw new NotImplementedException();
        }

        private static string TimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static void TraceInformation(string message)
        {
            Trace.TraceInformation("{0}, TestDialerEventsHandlerService: {1}", TimeStamp(), message);
        }
    }
}
