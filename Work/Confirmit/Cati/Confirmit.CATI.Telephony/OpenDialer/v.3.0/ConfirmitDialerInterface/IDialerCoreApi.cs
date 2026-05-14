using System.Collections.Generic;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// The main interface of Confirmit open dialer interface. 
    /// Confirmit CATI calls this interface functions to open campaigns, login agents, dial respondents…
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
        /// Initializes dialer to work with campaigns, agents, calls etc.
        /// Confirmit CATI does not call this interface methods until dialer is not initialized. 
        /// Note: actually some methods can still be called, see 'Workflow' chapter of 'Confirmit Open Dialer Interface' document for details.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id</param>
        /// <param name="configurationParametersXml">Dialer configuration parameters including obligatory parameters.
        ///   See chapter 'Dialer parameters' of 'Confirmit Open Dialer Interface' document for details. </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode Initialize(int companyId, int dialerId, string configurationParametersXml);

        /// <summary>
        /// Releases all dialer resources.
        /// This method is opposite to Initialize method.
        /// Confirmit CATI will not call any other methods after Release.
        /// Note: actually some methods can still be called, see 'Workflow' chapter of 'Confirmit Open Dialer Interface' document for details.
        /// </summary>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode Release();

        /// <summary>
        /// Load dialer driver state from a file
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
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
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="configurationParametersXml">XML statement that contains dialer configuration parameters</param>
        /// <exception cref="ParametersException"/>
        /// <seealso cref="Initialize"/>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml);

        /// <summary>
        /// Confirmit CATI calls this method regularly to know the dialer state. 
        /// DialerState.Available return value indicates that dialer is alive.
        /// If dialer returns DialerState.Unavailable or GetState call fails then Confirmit CATI tries to call GetState again and again during appropriate time.
        /// If GetState calls still return DialerState.Unavailable then Confirmit CATI makes dialer unavailable and finally calls IDialercoreApi.Release.
        /// Dialer can also use IDialerEvents.NotifyDialerState to notify Confirmit CATI about dialer state change. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <returns>Actual dialer state</returns>
        DialerState GetState(int companyId);

        /// <summary>
        /// Starts campiagn on dialer.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="campaignName"></param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <param name="recordWholeInterview">Is whole interview recording switched on for the campaign?</param>
        /// <param name="campaignParametersXml">The campaign parameters </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StartCampaign(
            int companyId, 
            long campaignId, 
            string campaignName, 
            DialingMode dialingMode, 
            bool recordWholeInterview, 
            string campaignParametersXml);

        /// <summary>
        /// Stops campiagn on dialer.
        /// Dialer does not interrupt active calls, it allows agents to complete them.
        /// In case of predictive campaign dialer must return predictive calls what are in queue back to Confirmit CATI 
        /// through IDialerEvents.NotifyOutcome with outcome 'ReturnedNotDialled'.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StopCampaign(int companyId, long campaignId, DialingMode dialingMode);

        /// <summary>
        /// Stops campiagn on dialer and forcibly terminates all active calls (conversations).
        /// In case of predictive campaign dialer must return predictive calls what are in queue back to Confirmit CATI 
        /// through IDialerEvents.NotifyOutcome with outcome 'ReturnedNotDialled',
        /// 'Stopped' outcome must be returned for interrupted calls.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode KillCampaign(int companyId, long campaignId, DialingMode dialingMode);

        /// <summary>
        /// Updates campaign dialer parameters for a concrete campaign.
        /// The function make sense for started campaigns only. New parameters must come into effect
        /// without restarting the campaign.
        /// Can throw DialerParametersException if parameters xml is incorrect or contains wrong parameters or some parameter values are incorrect, out of range etc.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <param name="recordWholeInterview">Is whole interview recording switched on for the campaign?</param>
        /// <param name="campaignParametersXml">Campaign parameters</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode SetCampaignParameters(int companyId, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml);

        /// <summary>
        /// Login agent to dialer.
        /// Dialer informs Confirmit CATI about the agent login result via IDialerEvents.NotifyAgentState event (if agent is not already logged in to dialer).
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">
        ///   The campaign unique identifier.
        ///   Confirmit CATI can pass campaignId=0. This means that agent will be logged in not to a concrete campaign and can work with many campaigns during one login session.
        ///   Note: Zero campaignId will be passed only for automatic task choice agents and only if dialer supports automatic task choice.
        ///   See 'SupportedPersonModes' parameter in 'Dialer parameters' chapter of 'Confirmit Open Dialer Interface' document for details.
        /// </param>
        /// <param name="agentId">The agent unique identifier</param>
        /// <param name="agentName">The agent name</param>
        /// <param name="agentConnectionString">The agent telephone extension or telephone number (depending on the resourceBindingType parameter value)</param>
        /// <param name="resourceBindingType">The agent resource binding type</param>
        /// <param name="isPredictive">Is the agent logging in for predictive calls?</param>
        /// <param name="agentAttributes">
        ///   Set of some additional attributes which can be applied to the agent at the login.
        ///   Each attribute is represented as a key-value pair.
        ///   At this time Confirmit CATI passes only one attribute with key 'Location' via this parameter,
        ///   'Location' attribute can be used by the dialer provider to select the correct agent resource (line)
        ///   according to the agent location.
        ///   Later more attributes could be supported.
        ///   Note: Values of the ‘agentAttributes’ parameter should be treated by the dialer as case insensitive.
        /// </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// DialerErrorCode.AgentAlreadyLoggedIn if the agent is already logged in.
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode Login(
            int companyId, 
            long campaignId, 
            int agentId, 
            string agentName, 
            string agentConnectionString, 
            ResourceBindingType resourceBindingType, 
            bool isPredictive, 
            IEnumerable<KeyValuePair<string, string>> agentAttributes);

        /// <summary>
        /// Changes current campaign for a logged in agent.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier </param>
        /// <param name="agentId">Dialer parameters for the campaign</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode SetCampaign(int companyId, long campaignId, int agentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier </param>
        /// <param name="agentId">The agent uniquew identifier</param>
        /// <param name="isPredictive">Has the agent been working predictively?</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode Logout(int companyId, long campaignId, int agentId, bool isPredictive);

        /// <summary>
        /// A function that forcibly logs an Agent out. The function does not wait for ongoing calls to complete.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode KillAgent(int companyId, long campaignId, int agentId);

        /// <summary>
        /// Makes agent ready to receive calls.
        /// Confirmit CATI always calls this method after login.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode GoReady(int companyId, long campaignId, int agentId);

        /// <summary>
        /// Makes agent not ready to receive calls. 
        /// Agent still stays logged in.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode GoNotReady(int companyId, long campaignId, int agentId);


        /// <summary>
        /// A function that sets the groups that an agent can take calls for. This function 
        /// allows to change the group setting for an agent who is currently logged into a campaign. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="agentGroups">array of agent groups identifiers</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode SetGroups(int companyId, long campaignId, int agentId, int[] agentGroups);

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent allowing to specify dialing mode for the call.
        /// Another name for this function could be 'Dial'.
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="companyId">Confirmit company id.</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="diallingMode">Dialling mode for the call</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode SendNumberToAgent(
            int companyId, 
            long campaignId, 
            int agentId, 
            DialingMode diallingMode, 
            int interviewId, 
            long callId, 
            string phoneNumber, 
            bool isRecording);

        /// <summary>
        /// A function that initiates redialing by a specific agent. The phone number may be the same as 
        /// for previous dial or may be different.
        /// Note, it's dialer responsibility to do hangup if the agent is in call.
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="companyId">Confirmit company id.</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode Redial(
            int companyId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording);

        /// <summary>
        /// Sends a set of calls to be dialed on a campaign.
        /// Confirmit CATI calls this method to answer the dialer 'IDialerEvents.RequestCalls' request.
        /// Confirmit CATI answers to each dialer 'IDialerEvents.RequestCalls' request.
        /// 
        /// Dialer informs Confirmit CATI about the dial operation results via 'IDialerEvents.NotifyOutcome' event for each call.
        /// Notes about the numbers handling sequence : 
        /// 1. Calls should be processed by the dialer in the same order as they come to this method (callList). 
        /// This is required to ensure that the call priority order is not lost when numbers are passed to the dialer.
        /// 2. If the next SendNumbers request comes to dialer while some previous calls are not handled by dialer 
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
        ///   Identity of the corresponding 'IDialerEvents.RequestCalls' request.
        ///   Confirmit includes it into the 'SendNumbers' answer.
        ///   So the dialer recognizes what request the 'SendNumbers' answer belongs to.
        /// </param>
        /// <param name="companyId">Confirmit company id</param>
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
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        /// <seealso cref="CallOutcome"/>
        DialerErrorCode SendNumbers(
            string requestId, 
            int companyId, 
            long campaignId, 
            DialingMode campaignDialingMode, 
            List<CallInfo> callList, 
            int callAgingTimeout);

        /// <summary>
        /// Respondent hangup, agent does not become ready for next calls.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode Hangup(int companyId, long campaignId, int agentId);

        /// <summary>
        /// Makes hangup (if respondent is off-hook) and completes the call for the agent.
        /// After this the agent can start working with another call (if ready).
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="makeAgentReady">true to make agent ready for next calls, false otherwise</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode CompleteCall(int companyId, long campaignId, int agentId, bool makeAgentReady);

        /// <summary>
        /// Send the interview extended status to dialer.
        /// This method is called for interviews on interview finish as soon as the final interview extended status is set,
        /// that is just after the interview scheduling process is finished.
        /// This methods is called for all calls in preview/automatic dialing mode, and for connected calls in predictive mode.
        /// Dialer can use this final extended status for different purposes: statistics, etc.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="interviewStatus">The interview status, <seealso cref="UpdateInterviewStatus"/>. </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode UpdateInterviewStatus(
            int companyId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            InterviewStatus interviewStatus);

        /// <summary>
        /// This method is used to complete preview stage of an interview dialed with preview dial mode inside a predictive campaign. 
        /// This is in fact dial as well.
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number to call.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode CompletePreview(
            int companyId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            string phoneNumber, 
            bool isRecording);

        /// <summary>
        /// Returns the calls for the specified campaign and/or group back from the dialer. 
        /// Dialer informs Confirmit CATI about the operation result via IDialerEvents.NotifyOutcome event.
        /// Dialer can answer with outcome 'ReturnedNotDialled' for calls in queue, or 'Stopped' outcome for interrupted calls.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="callList">List of calls to be flushed</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode FlushNumbers(int companyId, long campaignId, List<CallInfo> callList);

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        ///<param name="companyId">Confirmit company id</param>
        ///<param name="campaignId">The unique identifier of the Campaign.</param>
        ///<param name="agentId">The unique identifier of the Agent.</param>
        ///<param name="interviewId">The unique identifier of the interview connected to the call</param>
        ///<param name="callId">The unique identifier of the call</param>
        ///<param name="label">The label will be included into the audio record file name.</param>
        ///<returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StartRecording(
            int companyId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, string 
            label);

        /// <summary>
        /// Stops recording of the interview.
        /// </summary>
        ///<param name="companyId">Confirmit company id</param>
        ///<param name="campaignId">The unique identifier of the Campaign.</param>
        ///<param name="agentId">The unique identifier of the Agent.</param>
        ///<param name="interviewId">The unique identifier of the interview connected to the call</param>
        ///<param name="callId">The unique identifier of the call</param>
        ///<param name="stopRecordingMode">what to stop: whole interview recording, recording of a section, or both.</param>
        ///<returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StopRecording(
            int companyId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            StopRecordingMode stopRecordingMode);

        /// <summary>
        /// Starts playback of voice fragment (as wav file) for interview
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="fileName">file name without path</param>
        /// <param name="timeOfPlayingInSeconds"> returns time of playing in seconds</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StartPlayback(
            int companyId, 
            long campaignId, 
            int agentId, 
            int interviewId, 
            long callId, 
            string fileName, 
            out int timeOfPlayingInSeconds);

        /// <summary>
        /// Stops play voice fragment.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StopPlayback(
            int companyId, 
            long campaignId, 
            int agentId, 
            long callId);

        /// <summary>
        /// Pauses or Resumes paused playing.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode PauseOrResumePlayback(
            int companyId, 
            long campaignId, 
            int agentId, 
            long callId);

        /// <summary>
        /// Switchs agent from hearing respondent to hearing playing and back.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(
            int companyId, 
            long campaignId, 
            int agentId, 
            long callId);

        /// <summary>
        /// Starts audio monitoring of the agent calls. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="agentId">Identifier af the agent to be monitored</param>
        /// <param name="supervisorName">Supervisor’s name</param>
        /// <param name="supervisorConnectionString">Supervisor’s extension or telephone number (depending on the resourceBindingType parameter value)</param>
        /// <param name="resourceBindingType">The agent resource binding type</param>
        /// <param name="sessionId">If an initial monitor has not been performed SessionID should be empty 
        /// and its value will be returned in the return message. If an initial monitor has already been 
        /// performed the SessionId has to be specified and supervisorConnectionString can be omitted.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StartMonitor(
            int companyId, 
            int agentId, 
            string supervisorName, 
            string supervisorConnectionString, 
            ResourceBindingType resourceBindingType, 
            ref string sessionId);

        /// <summary>
        /// Stops monitoring session
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="sessionId">Identifier of monitoring session to be stopped</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode StopMonitor(int companyId, string sessionId);

        /// <summary>
        /// Returns list of instances TrunkLineStateAndAlarms class where is state (and alarms if exist) of each trunk line dialer owns
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="trunkLineStatesAndAlarms">The result list of states and alarms</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms);

        /// <summary>
        /// Transfers interview to Interactive Voice Response system (IVR).
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="endpoint">The IVR endpoint</param>
        /// <param name="attributes">Any attributes required for TransferToIvr in the form of key-value pairs.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        DialerErrorCode TransferToIvr(
            int companyId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes);
    }
}
