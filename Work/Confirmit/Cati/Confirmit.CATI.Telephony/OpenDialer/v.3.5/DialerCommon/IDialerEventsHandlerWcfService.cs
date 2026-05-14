using System;
using System.Collections.Generic;
using System.ServiceModel;

using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony.DialerCommon
{
    [ServiceContract(Name = "DialerEventsHandlerService", Namespace = "http://www.confirmit.com/DialerEventsHandlerService/21/09/2009")]
    public interface IDialerEventsHandlerService
    {
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
        [OperationContract]
        void NotifyUserState(
            int dialerId,
            string requestId, 
            string sessionId, 
            string companyId, 
            long surveyId,
            string userId, 
            string userState);

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
        /// <param name="dialerAccompanyingCallInfo">Some accompanying info received from dialer. PROTSInternalFlag in case of PROTS.</param>
        [OperationContract]
        void NotifyOutcome(
            int dialerId,
            string requestId, 
            string sessionId, 
            string companyId, 
            long surveyId, 
            string userId, 
            string contactId,
            int callId, 
            string outcomeType,
            string outcomeCode,
            string dialerAccompanyingCallInfo);

        [OperationContract]
        void NotifyInboundCall(
            int dialerId,
            int companyId,
            string ddiNumber,
            string cliNumber,
            string inboundCallId);

        [OperationContract]
        void NotifyInboundCallDroppedByRespondent(
            int dialerId,
            int companyId,
            string inboundCallId);

        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="callDialingMode"></param>
        [OperationContract]
        void ScreenPop(
            int dialerId,
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            string userId,
            string contactId,
            int callId,
            DialingMode callDialingMode);

        /// <summary>
        /// This function is used to pass back dialer state.
        /// Previously:
        /// Note! IsOneWay is switched off for this method because of we need it to be synchronous
        /// in order to know result of the call
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="companyId"></param>
        /// <param name="dialerState"></param>
        [OperationContract /*(IsOneWay = true)*/]
        void NotifyDialerState(int dialerId, string companyId, DialerState dialerState);

        /// <summary>
        /// Dialler requests for calls (predicive)
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="groupId"></param>
        /// <param name="callsSelectionAlgorithm"> </param>
        /// <param name="callCount">amount of calls the dialler requests for</param>
        [OperationContract]
        void RequestCalls(
            int dialerId, 
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            int? groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int callCount);

        /// <summary>
        /// Dialler license expiration date
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="licenseExpirationDateTime">date of the License expiration</param>
        [OperationContract]
        void NotifyLicenseExpiration(int dialerId, DateTime licenseExpirationDateTime);

        /// <summary>
        /// IVR form submission notification
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="agentId"></param>
        /// <param name="variables"></param>
        [OperationContract]
        void NotifyIvrSubmit(int dialerId, string companyId, long surveyId, string agentId, KeyValuePair<string, string>[] variables);
    }
}