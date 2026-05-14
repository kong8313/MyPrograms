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
            long callOutcome);

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
        /// Is being sent on MN_HTTP_RESULT_INVALID_TENANT_TOKEN error (0x8000f00A,	This Tenant Token does not exist)
        /// from MN dialer. The error usually means that the dialer has been restarted and we need to 
        /// reinitialize everything.
        /// </summary>
        void OnDialerRestart(int dialerId);

        /// <summary>
        /// Is being called on each LiveMonitorStatus or LiveMonitorError event received
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="sessionId">The unique identifier of the Session.</param>
        /// <param name="isError">If true, status code contains an error code.</param>
        /// <param name="statusCode">Current monitoring status including success and error codes</param>
        void OnDialerMonitorStatus(int dialerId, string sessionId, bool isError, int statusCode);

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
    }
}