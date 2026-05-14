using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace ConfirmitDialerInterface
{
    /// <summary>
    /// The main interface of Forsta open dialer interface. 
    /// Forsta CATI calls this interface functions to open campaigns, login agents, dial respondents…
    /// </summary>
    public interface IDialerCoreApi
    {
        /// <summary>
        /// Dialer name to show in CATI supervisor
        /// Ex: "Pro-T-S", "TCI", ...
        /// </summary>
        /// <returns>Dialer name</returns>
        string GetName();

        /// <summary>
        /// Version of dialer provider's dialer to show it in CATI supervisor.
        /// Ex: "1.0", 
        /// </summary>
        /// <returns>Dialer version</returns>
        string GetVersion();

        /// <summary>
        /// Initializes dialer to work with campaigns, agents, calls, etc.
        /// Forsta CATI does not call this interface methods until dialer is not initialized. 
        /// Note: actually some methods can still be called, see 'Workflow' chapter of 'Forsta Open Dialer Interface' document for details.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="configurationParametersXml">Dialer configuration parameters including obligatory parameters.
        ///   See chapter 'Dialer parameters' of 'Forsta Open Dialer Interface' document for details. </param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml);

        /// <summary>
        /// Releases all dialer resources.
        /// This method is opposite to Initialize method.
        /// Forsta CATI will not call any other methods after <see cref="Release"/>.
        /// Note: actually some methods can still be called, see 'Workflow' chapter of 'Forsta Open Dialer Interface' document for details.
        /// </summary>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="companyId">Company id</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode Release(int dialerId, int companyId);

        /// <summary>
        /// Gets a list of features supported by the dialer. This is used by CATI to automatically enable / disable functionality that requires support by the dialer
        /// </summary>
        /// <param name="companyId">Company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <returns></returns>
        IDialerFeatures GetFeatures(int companyId, int dialerId);

        /// <summary>
        /// Load dialer driver state from a file
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="filename">filename with full path</param>
        /// <returns></returns>
        DialerErrorCode RestoreDialerDriverState(int companyId, string filename);

        /// <summary>
        /// Save dialer driver state to a file
        /// </summary>
        /// <param name="filename">filename with full path where to store</param>
        /// <returns></returns>
        DialerErrorCode SaveDialerDriverState(string filename);

        /// <summary>
        /// Updates dialer configuration parameters.
        /// Can throw DialerParametersException if parameters xml is incorrect or contains wrong parameters or some parameter values are incorrect, out of range etc.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="configurationParametersXml">XML statement that contains dialer configuration parameters</param>
        /// <exception cref="ParametersException"/>
        /// <seealso cref="Initialize"/>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml);

        /// <summary>
        /// Forsta CATI calls this method regularly to know the dialer state. 
        /// <see cref="DialerState.Available"/> return value indicates that dialer is alive.
        /// If dialer returns <see cref="DialerState.Unavailable"/> or GetState call fails then Forsta CATI tries to call GetState again and again during appropriate time.
        /// If GetState calls still return <see cref="DialerState.Unavailable"/> then Forsta CATI makes dialer unavailable and finally calls <see cref="Release"/>.
        /// Dialer can also use <see cref="IDialerEvents.NotifyDialerState"/> to notify Forsta CATI about dialer state change. 
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <returns>Actual dialer state</returns>
        DialerState GetState(int companyId, int dialerId);

        /// <summary>
        /// Starts campaign on dialer.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerIds">Ids of target dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="campaignName"></param>
        /// <param name="dialingMode">The campaign dialing mode</param>
        /// <param name="recordWholeInterview">Is whole interview recording switched on for the campaign?</param>
        /// <param name="campaignParametersXml">The campaign parameters </param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StartCampaign(
            int companyId,
            int[] dialerIds,
            long campaignId,
            string campaignName,
            DialingMode dialingMode,
            bool recordWholeInterview,
            string campaignParametersXml);

        /// <summary>
        /// Stops campaign on dialer.
        /// Dialer does not interrupt active calls, it allows agents to complete them.
        /// In case of predictive campaign dialer must return predictive calls what are in queue back to Forsta CATI 
        /// through <see cref="IDialerEvents.NotifyOutcome"/> with <see cref="CallOutcome.ReturnedNotDialled"/>.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerIds">Ids of target dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campaign dialing mode</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode);

        /// <summary>
        /// Stops campaign on dialer and forcibly terminates all active calls (conversations).
        /// In case of predictive campaign dialer must return predictive calls what are in queue back to Forsta CATI 
        /// through <see cref="IDialerEvents.NotifyOutcome"/> with <see cref="CallOutcome.ReturnedNotDialled"/>,
        /// <see cref="CallOutcome.Stopped"/> must be returned for interrupted calls.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerIds">Ids of target dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campaign dialing mode</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode);

        /// <summary>
        /// Updates campaign dialer parameters for a concrete campaign.
        /// The function makes sense for started campaigns only. New parameters must come into effect
        /// without restarting the campaign.
        /// Can throw DialerParametersException if parameters xml is incorrect or contains wrong parameters or some parameter values are incorrect, out of range etc.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerIds">Ids of target dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campaign dialing mode</param>
        /// <param name="recordWholeInterview">Is whole interview recording switched on for the campaign?</param>
        /// <param name="campaignParametersXml">Campaign parameters</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml);

        /// <summary>
        /// Login agent to dialer.
        /// Dialer informs Forsta CATI about the agent login result via <see cref="IDialerEvents.NotifyAgentState"/> event (if agent is not already logged in to dialer).
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">
        ///     The campaign unique identifier.
        ///     Forsta CATI can pass <paramref name="campaignId"/>=0. This means that agent will be logged in not to a concrete campaign and can work with many campaigns during one login session.
        ///     Note: Zero <paramref name="campaignId"/> will be passed only for automatic task choice agents and only if dialer supports automatic task choice.
        ///     See 'SupportedPersonModes' parameter in 'Dialer parameters' chapter of 'Forsta Open Dialer Interface' document for details.
        /// </param>
        /// <param name="agentId">The agent unique identifier</param>
        /// <param name="agentName">The agent name</param>
        /// <param name="agentType">The agent type</param>
        /// <param name="agentConnectionString">The agent telephone extension or telephone number (depending on the <paramref name="resourceBindingType"/> parameter value)</param>
        /// <param name="resourceBindingType">The agent resource binding type</param>
        /// <param name="isPredictive">Is the agent logging in for predictive calls?</param>
        /// <param name="agentAttributes">
        ///     Set of some additional attributes which can be applied to the agent at the login.
        ///     Each attribute is represented as a key-value pair.
        ///     At this time Forsta CATI passes only one attribute with key 'Location' via this parameter,
        ///     'Location' attribute can be used by the dialer provider to select the correct agent resource (line)
        ///     according to the agent location.
        ///     Later more attributes could be supported.
        ///     Note: Values of the ‘<paramref name="agentAttributes"/>’ parameter should be treated by the dialer as case insensitive.
        /// </param>
        /// <seealso cref="AgentType"/>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// <see cref="DialerErrorCode.AgentAlreadyLoggedIn"/> if the agent is already logged in.
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode Login(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            string agentName,
            AgentType agentType,
            string agentConnectionString,
            ResourceBindingType resourceBindingType,
            bool isPredictive,
            IEnumerable<KeyValuePair<string, string>> agentAttributes);

        /// <summary>
        /// Changes current campaign for a logged in agent.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The campaign unique identifier </param>
        /// <param name="agentId">Dialer parameters for the campaign</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// Logs out the specified agent.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The campaign unique identifier </param>
        /// <param name="agentId">The agent unique identifier</param>
        /// <param name="isPredictive">Has the agent been working predictively?</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive);

        /// <summary>
        /// Forcibly logs an agent out. The function does not wait for ongoing calls to complete.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// Makes agent ready to receive calls.
        /// Forsta CATI always calls this method after login.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// Makes agent not ready to receive calls. 
        /// Agent still stays logged in.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="breakName">The name of agent's break type</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId, string breakName);


        /// <summary>
        /// A function that sets the groups that an agent can take calls for. This function 
        /// allows to change the group setting for an agent who is currently logged into a campaign. 
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="agentGroups">array of agent groups identifiers</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups);

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent allowing to specify dialing mode for the call.
        /// Another name for this function could be 'Dial'.
        /// Dialer informs Forsta CATI about the dial result via <see cref="IDialerEvents.NotifyOutcome"/> event.
        /// Note, that if agent is already in a connected call with specified phone number - dialer should always reply with
        /// <see cref="IDialerEvents.NotifyOutcome"/> with <see cref="CallOutcome.Connected"/> and do not perform any other actions.
        /// </summary>
        /// <param name="companyId">Forsta company id.</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="diallingMode">Dialing mode for the call</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <param name="respondentVariables">A collection of custom contact fields that may be provided if they are configured for the company and are among the fields in the current campaign.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode SendNumberToAgent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            DialingMode diallingMode,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording,
            string callerId,
            Dictionary<string, object> respondentVariables);

        /// <summary>
        /// A function that initiates redialing by a specific agent. The phone number may be the same as 
        /// for previous dial or may be different.
        /// Note, it's dialer responsibility to do hangup if the agent is in call.
        /// Dialer informs Forsta CATI about the dial result via <see cref="IDialerEvents.NotifyOutcome"/> event.
        /// </summary>
        /// <param name="companyId">Forsta company id.</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode Redial(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording,
            string callerId);

        /// <summary>
        /// Sends a set of calls to be dialed on a campaign.
        /// Forsta CATI calls this method to answer the dialer <see cref="IDialerEvents.RequestCalls"/> request.
        /// Forsta CATI answers to each dialer <see cref="IDialerEvents.RequestCalls"/> request.
        /// 
        /// Dialer informs Forsta CATI about the dial operation results via <see cref="IDialerEvents.NotifyOutcome"/> event for each call.
        /// Notes about the numbers handling sequence:
        /// 1. Calls should be processed by the dialer in the same order as they come to this method (callList). 
        /// This is required to ensure that the call priority order is not lost when numbers are passed to the dialer.
        /// 2. If the next <see cref="SendNumbers"/> request comes to dialer while some previous calls are not handled by dialer 
        /// then those old calls must be handled by dialer first.
        /// 
        /// Note about explicitly assigned calls:
        /// Explicitly assigned calls (like CallInfo[agentId=72, … , diallingMode=Predictive, …] ) should be treated by dialer 
        /// as some sort of ‘automatic’. I.e. dialer shouldn’t involve ScreenPop notification in this case. 
        /// The algorithm should be like as follows:
        /// a.	Wait for corresponding Agent is free (in case if he/she is in an interview)
        /// b.	Dial the explicitly assigned call
        /// c.	Send the call outcome via NotifyOutcome
        /// The ScreenPop notification should be involved for so called hybrid (preview-in-predictive) mode calls 
        /// (like CallInfo[agentId=72, … , diallingMode=Preview, …]) only.
        /// </summary>
        /// <param name="requestId">
        ///   Identity of the corresponding <see cref="IDialerEvents.RequestCalls"/> request.
        ///   Forsta includes it into the <see cref="SendNumbers"/> answer.
        ///   So the dialer recognizes what request the <see cref="SendNumbers"/> answer belongs to.
        /// </param>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="campaignDialingMode">The campaign dialing mode</param>
        /// <param name="callList">List of CallInfo objects that contains numbers to be dialed</param>
        /// <param name="callAgingTimeout">
        ///   Time interval in minutes that tells the dialer how long it may keep calls in its queue.
        ///   Call should be returned with <see cref="CallOutcome.ReturnedDiallerExpired"/> outcome if it was not dialed before the timeout is expired.
        ///   Note: Calls that have a CallInfo.timeToCall value set should be dialed even in case they have been delivered later than their timeToCall value. 
        ///   However, at the same time, aging should be still in force and aged calls should not be dialed if that is exceeded.
        /// </param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        /// <seealso cref="CallOutcome"/>
        DialerErrorCode SendNumbers(
            string requestId,
            int companyId,
            int dialerId,
            long campaignId,
            DialingMode campaignDialingMode,
            List<CallInfo> callList,
            int callAgingTimeout);

        /// <summary>
        /// Respondent hangup, agent does not become ready for next calls.
        /// It should return <see cref="DialerErrorCode.Success"/> if the specified agent is not currently in a call.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if hangup operation was successful or if agent is not currently in a call.  
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId, int interviewId, long callId);

        /// <summary>
        /// Makes hangup (if respondent is off-hook) and completes the call for the agent.
        /// After this the agent can start working with another call (if <paramref name="makeAgentReady"/> is set to <see langword="true"/>).
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewStatus">The interview status, see <see cref="InterviewStatus"/> class for details.</param>
        /// <param name="makeAgentReady"><see langword="true"/> to make agent ready for next calls, <see langword="false"/> otherwise</param>
        /// <param name="breakName">The name of agent's break type</param>
        /// <param name="interviewId">The unique identifier of the Interview</param>
        /// <param name="callId">The unique identifier of the Call</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId, 
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId);

        /// <summary>
        /// Finishes current interview and switches to the next interview, keeping existing telephone connection on dialer
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="currentCampaignId">The unique identifier of the current campaign.</param>
        /// <param name="agentId">The unique identifier of the agent.</param>
        /// <param name="currentInterviewStatus">The interview status, see <see cref="InterviewStatus"/> class for details.</param>
        /// <param name="nextCampaignId">The unique identifier of the next campaign.</param>
        /// <param name="nextInterviewId">The unique identifier of the next interview.</param>
        /// <param name="nextCallId">The unique identifier of the next call.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode SetNextInterview(
            int companyId,
            int dialerId,
            long currentCampaignId,
            int agentId,
            InterviewStatus currentInterviewStatus,
            long nextCampaignId,
            int nextInterviewId,
            long nextCallId);

        /// <summary>
        /// For internal usage only
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the current campaign</param>
        /// <param name="agentId">The unique identifier of the agent</param>
        /// <param name="interviewId">The unique identifier of the interview</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="respondentSurveyLink">The URL link to the respondent survey</param>
        /// <returns></returns>
        DialerErrorCode StartCustomIvrInterview(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId, 
            string respondentSurveyLink);

        /// <summary>
        /// This method is used to complete preview stage of an interview dialed with <see cref="DialingMode.Preview"/> inside a predictive campaign. 
        /// This is in fact dial as well.
        /// Dialer informs Forsta CATI about the dial result via the <see cref="IDialerEvents.NotifyOutcome"/> event.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number to call.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode CompletePreview(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording);

        /// <summary>
        /// Returns the calls for the specified campaign and/or group back from the dialer. 
        /// Dialer informs Forsta CATI about the operation result via <see cref="IDialerEvents.NotifyOutcome"/> event.
        /// Dialer can answer with <see cref="CallOutcome.ReturnedNotDialled"/> for calls in queue, or <see cref="CallOutcome.Stopped"/> for interrupted calls.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerIds">Ids of target dialers</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="callList">List of calls to be flushed</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList);

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="label">The label to be included into the audio record file name.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StartRecording(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId, string
            label);

        /// <summary>
        /// Stops recording of the interview.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="stopRecordingMode">what to stop: whole interview recording, recording of a section, or both.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StopRecording(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            StopRecordingMode stopRecordingMode);

        /// <summary>
        /// Starts playback of voice fragment (as wav file) for interview
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="fileName">file name without path</param>
        /// <param name="timeOfPlayingInSeconds"> returns time of playing in seconds</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StartPlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string fileName,
            out int timeOfPlayingInSeconds);

        /// <summary>
        /// Stops play voice fragment.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StopPlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId);

        /// <summary>
        /// Pauses or Resumes paused playing.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode PauseOrResumePlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId);

        /// <summary>
        /// Switches agent from hearing respondent to hearing playing and back.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId);

        /// <summary>
        /// Starts audio monitoring of the agent calls. 
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="agentId">Identifier af the agent to be monitored</param>
        /// <param name="supervisorName">Supervisor’s name</param>
        /// <param name="supervisorConnectionString">Supervisor’s extension or telephone number (depending on the <paramref name="resourceBindingType"/> parameter value)</param>
        /// <param name="resourceBindingType">The agent resource binding type</param>
        /// <param name="sessionId">If an initial monitor has not been performed <paramref name="sessionId"/> should be empty 
        /// and its value will be returned in the return message. If an initial monitor has already been 
        /// performed the <paramref name="sessionId"/> has to be specified and <paramref name="supervisorConnectionString"/> can be omitted.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StartMonitor(
            int companyId,
            int dialerId,
            int agentId,
            string supervisorName,
            string supervisorConnectionString,
            ResourceBindingType resourceBindingType,
            ref string sessionId);

        /// <summary>
        /// Stops monitoring session
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="sessionId">Identifier of monitoring session to be stopped</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId);

        /// <summary>
        /// Set monitoring mode
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="sessionId">Identifier of monitoring session to be stopped</param>
        /// <param name="monitorMode">Monitoring mode</param>
        /// <returns></returns>
        DialerErrorCode SetMonitorMode(int companyId, int dialerId, string sessionId, MonitorMode monitorMode);

        /// <summary>
        /// Returns list of instances <see cref="TrunkLineStateAndAlarms"/> class where is state (and alarms if exist) of each trunk line dialer owns
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of target dialer</param>
        /// <param name="trunkLineStatesAndAlarms">The result list of states and alarms</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms);

        /// <summary>
        /// Transfers interview to Interactive Voice Response system (IVR).
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="endpoint">The IVR endpoint</param>
        /// <param name="attributes">Any attributes required for TransferToIvr in the form of key-value pairs.</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode TransferToIvr(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes);

        /// <summary>
        /// Passes voice XML statement to dialer 
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="agentId">The unique identifier of the Agent</param>
        /// <param name="voiceXml">A string containing the VoiceXML document</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode IvrRenderVoiceXml(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            string voiceXml);

        /// <summary>
        /// Configure and activate DDI numbers. It's assumed that all existing 
        /// numbers should be deactivated.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="inboundDdiNumbers">A list of inbound DDI numbers and their settings</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            int dialerId,
            InboundDdiNumber[] inboundDdiNumbers);

        /// <summary>
        /// Drop inbound call that is not yet routed to an agent 
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="inboundCallId">The unique identifier of the inbound call that has been generated by dialer and sent in <see cref="IDialerEvents.NotifyInboundCall"/></param>
        /// <param name="audioMessageDescriptor">Information about audio message that should be played before dropping the call or <see langword="null"/> if no audio should be played</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode DropInboundCall(
            int companyId,
            int dialerId,
            string inboundCallId,
            AudioMessageDescriptor audioMessageDescriptor);

        /// <summary>
        /// Connect inbound call to an available agent in predictive mode.
        /// The dialer should reply with <see cref="IDialerEvents.NotifyOutcome"/> with <see cref="CallOutcome.Connected"/> if agent was successfully connected to inbound call
        /// or with <see cref="CallOutcome.DroppedByRespondent"/> if respondent dropped the call after <see cref="ConnectInboundCall"/>.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="inboundCallId">The unique identifier of the inbound call that has been generated by dialer and sent in <see cref="IDialerEvents.NotifyInboundCall"/></param>
        /// <param name="callInfo">Information about the call</param>
        /// <param name="campaignIdsToBorrowAgentsFrom">A list of campaign IDs that the dialer can use to borrow agents from to handle the inbound call,
        /// or <see langword="null"/> indicating that agents logged it to any campaigns can be used  to handle the inbound call.</param>
        /// <param name="audioMessageDescriptor">Information about audio message that should be played
        ///     before connecting the call or null if no audio should be played</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode ConnectInboundCall(int companyId, int dialerId, long campaignId, string inboundCallId, CallInfo callInfo, long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor);

        /// <summary>
        /// Connect inbound call to an agent in non-predictive mode.
        /// The dialer should reply with <see cref="IDialerEvents.NotifyOutcome"/> with <see cref="CallOutcome.Connected"/> if agent was successfully connected to inbound call
        /// or with <see cref="CallOutcome.DroppedByRespondent"/> if respondent dropped the call after <see cref="ConnectInboundCallToAgent"/>.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="inboundCallId">The unique identifier of the inbound call that has been generated by dialer and sent in <see cref="IDialerEvents.NotifyInboundCall"/></param>
        /// <param name="callInfo">Information about the call</param>
        /// <param name="audioMessageDescriptor">Information about audio message that should be played
        /// before connecting the call or null if no audio should be played</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode ConnectInboundCallToAgent(
            int companyId,
            int dialerId,
            long campaignId,
            string inboundCallId,
            CallInfo callInfo,
            AudioMessageDescriptor audioMessageDescriptor);

        /// <summary>
        /// Creates transfer session, generates transfer ID and puts respondent on hold.
        /// Internally this should contain a call set ConnectionState.InitiatorToTarget, although we did not set a target yet.
        /// So by default initiator should be connected to target during dialing and when the connection has been established.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="transferId">A string containing unique transfer session ID</param>
        /// <param name="agentId">The unique identifier of the Agent</param>
        /// <param name="transferType">Indicates that type of the call transfer</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode TransferStart(
            int companyId,
            int dialerId,
            long campaignId,
            string transferId,
            int agentId,
            TransferType transferType);

        /// <summary>
        /// This should include the target, specified by <paramref name="targetType"/> and <paramref name="targetResource"/>, into the transfer session.
        /// The actual behaviour will depend on a target type.
        /// For <see cref="TargetType.External"/> this should start calling a telephone number specified in <paramref name="targetResource"/> and connect it to the transfer.
        /// For <see cref="TargetType.Agent"/> this should connect an agent (with ID specified in <paramref name="targetResource"/>) to the transfer.
        /// For <see cref="TargetType.AgentGroup"/> this should find an available agent in the group (group ID specified in <paramref name="targetResource"/>) and connect it to the transfer session.
        /// If (targetType==<see cref="TargetType.AgentGroup"/>) dialer should take an agent that belongs to the group specified in the targetResource parameter
        /// If (targetType==<see cref="TargetType.AgentGroup"/>) AND (<paramref name="targetResource"/>==<see langword="null"/>) dialer should take any agent logged into the campaign specified in the campaignId parameter
        /// If <paramref name="borrowAgentsFromAllCampaigns"/> is <see langword="true"/> dialer can take agents logged in to any campaign, otherwise only agents logged in the campaign specified in campaignId parameter
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="transferId">A string containing unique transfer session ID</param>
        /// <param name="targetType">Transfer target type</param>
        /// <param name="targetResource">A string containing telephone number for external transfer, agent ID or agent group ID for internal transfer</param>
        /// <param name="borrowAgentsFromAllCampaigns"><see langword="true"/> indicates that dialer can take agents logged in to any campaign, otherwise only agents logged in the campaign specified in campaignId parameter</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode TransferSetTarget(
            int companyId,
            int dialerId,
            long campaignId,
            string transferId,
            TargetType targetType,
            string targetResource,
            bool borrowAgentsFromAllCampaigns);

        /// <summary>
        /// Connects 2 out of 3 sides of transfer or starts a conference according to <paramref name="state"/> parameter.
        /// This method can be called at any point during transfer, even if we do not have a connected target.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="transferId">A string containing unique transfer session ID</param>
        /// <param name="state">A desired transfer connection state</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode TransferSetConnectionState(
            int companyId,
            int dialerId,
            long campaignId,
            string transferId,
            ConnectionState state);

        /// <summary>
        /// Connects target and respondent, disconnects initiator from transfer and removes transfer session.
        /// Following <see cref="CompleteCall"/> calls should not affect respondent after <see cref="TransferStart"/>
        /// <see cref="CompleteCall"/> may be called before or after the <see cref="TransferComplete"/>
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="transferId">A string containing unique transfer session ID</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode TransferComplete(
            int companyId,
            int dialerId,
            long campaignId,
            string transferId);

        /// <summary>
        /// Connects initiator and respondent, disconnects target from transfer session, releases transfer session
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="transferId">A string containing unique transfer session ID</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode TransferCancel(
            int companyId,
            int dialerId,
            long campaignId,
            string transferId);

        /// <summary>
        /// Ensures that agent is ready to use softphone (e.g. SIP or WebRTC) and gets softphone connection credentials.
        /// If enabled -can be invoked when agent has logged in into CATI interviewer UI
        /// and can be used for single sign-on in interviewer application and softphone.
        /// </summary>
        /// <param name="companyId">Forsta company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="agentId">The agent unique identifier</param>
        /// <param name="agentName">The agent name</param>
        /// <param name="login">User login name for softphone</param>
        /// <param name="password">User password for softphone</param>
        /// <param name="host">Host name for softphone connection</param>
        /// <param name="extension">Agent telephone extension</param>
        /// <param name="frontendUrl">Url to the softphone web application</param>
        /// <returns>
        /// <see cref="DialerErrorCode.Success"/> if succeeded
        /// Other <see cref="DialerErrorCode"/> if failed
        /// </returns>
        DialerErrorCode RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);
    }
}
