using System.Diagnostics;
using Confirmit.CATI.Telephony.DialerCommon;

// !!! NOTE: It's just a test implementation of the IDialerEventsHandlerService interface
// It could be used for testing purposes

namespace Confirmit.CATI.Telephony.CommonDialerAPI.DialerEventsTestWcfService
{
    public class DialerEventsHandlerTestService : IDialerEventsHandlerService
    {
        /// <summary>
        /// This function is being used to pass back information about the status of the User. 
        /// It is especially needed for asynchronous functions like login, logout, go ready, etc. 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="userState"></param>
        public void NotifyUserState(string requestId,
                                    string sessionId,
                                    string companyId,
                                    string surveyId,
                                    string userId,
                                    string userState)
        {
            Trace.TraceInformation("requestId={0}, sessionId={1}, companyId={2}, surveyId={3}, userId={4}, userState={5}",
                requestId, sessionId, companyId, surveyId, userId, userState);
        }

        public void NotifyOutcome(string requestId,
                                  string sessionId,
                                  string companyId,
                                  string surveyId,
                                  string userId,
                                  string contactId,
                                  string callId,
                                  string outcomeType,
                                  string outcomeCode,
                                  string dialerAccompanyingCallInfo)
        {
            Trace.TraceInformation("requestId={0}, sessionId={1}, companyId={2}, surveyId={3}, userId={4}, contactId={5}" +
                                   "callId={6}, outcomeType={7}, outcomeCode={8}, dialerAccompanyingCallInfo={9}",
                                   requestId, sessionId, companyId, surveyId, userId, contactId,
                                   callId, outcomeType, outcomeCode, dialerAccompanyingCallInfo);
        }

        public void NotifyDialerState(string requestId,
                                      string sessionId,
                                      string companyId,
                                      string surveyId,
                                      string stateNotifycationType,
                                      string stateNotifycationCode,
                                      string stateNotifycationText)
        {
            Trace.TraceInformation("requestId={0}, sessionId={1}, companyId={2}, surveyId={3}" +
                                   "stateNotifycationType={4}, stateNotifycationCode={5}, stateNotifycationText={6}",
                                   requestId, sessionId, companyId, surveyId, 
                                   stateNotifycationType, stateNotifycationCode, stateNotifycationText);
        }

        public void RequestCalls(string requestId,
                                 string sessionId,
                                 string companyId,
                                 string surveyId,
                                 string groupId,
                                 int callCount)
        {
            Trace.TraceInformation("requestId={0}, sessionId={1}, companyId={2}, surveyId={3}" +
                                   "groupId={4}, callCount={5}",
                                   requestId, sessionId, companyId, surveyId,
                                   groupId, callCount);
        }
    }
}
