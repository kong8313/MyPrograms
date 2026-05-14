namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Dialer notification (dialer events) interface.
    /// Dialer notifies Confirmit CATI about different dialer/agent states, call outcomes etc.
    /// </summary>
    public interface IDialerEvents
    {
        /// <summary>
        /// Notifies Confirmit CATI about the dialer state
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="dialerState">State of dialer</param>
        void NotifyDialerState(
            int companyId, 
            int dialerId, 
            DialerState dialerState);

        /// <summary>
        /// Notifies Confirmit CATI about the agent state.
        /// Confirmit CATI (asynchronously) waits for this event after calling IDialerCoreApi.Login. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="agentState">Current agent state</param>
        void NotifyAgentState(
            int companyId, 
            int dialerId, 
            long campaignId, 
            int agentId, 
            AgentState agentState);

        /// <summary>
        /// Notifies Confirmit CATI about the call outcome.
        /// Confirmit CATI (asynchronously) waits for this event after calling the next methods:
        /// - IDialerCoreApi.SendNumberToAgent. 
        /// - IDialerCoreApi.SendNumbers
        /// - IDialerCoreApi.CompletePreview
        /// - IDialerCoreApi.FlushNumbers
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="outcome">The call outcome</param>
        /// <param name="dialerAccompanyingCallInfo">Some accompanying info received from dialer</param>
        void NotifyOutcome(
            int companyId, 
            int dialerId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            CallOutcome outcome, 
            string dialerAccompanyingCallInfo);

        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="callDialingMode">The call dialing mode</param>
        void ScreenPop(
            int companyId, 
            int dialerId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            DialingMode callDialingMode);

        /// <summary>
        /// Dialer requests calls for predicive dialing.
        /// Confirmit CATI answers to each RequestCalls request via 'IDialerCoreApi.SendNumbers' call.
        /// If Confirmit CATI does not have any calls ready for dialing it still answers via SendNumbers but with an empty call list.
        /// </summary>
        /// <param name="requestId">
        ///   Identity for the request. Confirmit CATI includes this id into the 'SendNumbers' answer.
        ///   So the dialer recognizes what request the 'SendNumbers' answer belongs to.
        /// </param>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="groupId">Identifier of the agent group which dialer requests calls for</param>
        /// <param name="callsSelectionAlgorithm">Confirmit CATI can select calls using two selection algorithms: 'by campaign' or 'by agent group'</param>
        /// <param name="callCount">Amount of calls the dialer requests for</param>
        void RequestCalls(
            string requestId, 
            int companyId, 
            int dialerId, 
            long campaignId, 
            int groupId, 
            CallsSelectionAlgorithm callsSelectionAlgorithm, 
            int callCount);
    }
}
