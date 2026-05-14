using System;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Telephony
{
    public interface IDialerEventsHandler
    {
        void OnDialerNotifyOutcome(
            int dialerId,
            string tenantId,
            long campaignId,
            long agentId,
            string contactId,
            long callId,
            long callOutcome,
            string callerId, 
            TimeSpan ringTime, 
            Dictionary<string, string> callOutcomeMetadata);

        void OnDialerNotifyAgentState(
            int dialerId,
            string tenantId,
            long campaignId,
            long agentId,
            string agentStateMsg);

        /// <summary>
        /// Used by the dialer engine to request calls for a specific group in a campaign
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"> </param>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign</param>
        /// <param name="groupId">ID of group for which calls are requested,
        ///   It can be null if dialer requests calls not by group but by campaign.
        ///   <see> CallsSelectionAlgorithm enum for possible call selection algorithms.</see> </param>
        /// <param name="callsSelectionAlgorithm"></param>
        /// <param name="callCount">The number of calls that have been requested</param>
        void OnDialerRequestCalls(
            int dialerId,
            string requestId,
            string tenantId,
            long campaignId,
            int? groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int callCount);

        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="customerId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="callDialingMode"></param>
        void OnDialerScreenPop(
            int dialerId,
            string customerId,
            long campaignId,
            int agentId,
            string contactId,
            int callId,
            DialingMode callDialingMode);

        void OnDialerNotifyInboundCall(
            int dialerId,
            int companyId,
            string ddiNumber,
            string cliNumber,
            string inboundCallId);

        void OnDialerNotifyCallDroppedByRespondent(
            int dialerId,
            string companyId,
            long campaignId,
            long agentId,
            long callId);

        void OnDialerNotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId);

        void OnDialerIvrSubmit(int dialerId, string companyId, long surveyId, long agentId, KeyValuePair<string, string>[] variables);

        void OnTransferState(int dialerId, int companyId, string transferId, TransferState transferState);
        
        void OnDialerNotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId,
            int agentId, int interviewId, long callId, CallOutcome callOutcome);
    }
}