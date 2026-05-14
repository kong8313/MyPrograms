using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;
using ConfirmitDialerInterface;
using DialerCommon;
using AgentTaskChoiceMode = ConfirmitDialerInterface.AgentTaskChoiceMode;
using CallInfo = ConfirmitDialerInterface.CallInfo;
using DialerState = ConfirmitDialerInterface.DialerState;
using DialingMode = ConfirmitDialerInterface.DialingMode;
using TrunkLineStateAndAlarms = ConfirmitDialerInterface.TrunkLineStateAndAlarms;

namespace Confirmit.CATI.Telephony
{
    //TODO: Replace int return type with the DialerErrorCode in all methods

    // ReSharper disable once InconsistentNaming
    public interface IDialerAPI
    {
        DialerInitializeResult Initialize(
            int dialerId,
            string tenantId,
            string connectionParametersXml,
            string configurationParametersXml,
            string surveyDefaultParametersXml,
            bool sendInitializeToWebService = true);
        int Release(int dialerId, int companyId);

        DialerFeatures GetFeatures(string tenantId);

        int StartCampaign(string tenantId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, bool recordWholeInterview, string surveyParametersXml);
        int StopCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode);
        int KillCampaign(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode);

        int Login(
            string tenantId,
            long campaignId,
            string agentId,
            string agentName,
            AgentType agentType,
            string agentExtension,
            string userId,
            bool isPredictive,
            bool isLocal,
            IEnumerable<KeyValuePair<string, string>> agentAttributes);

        // Note, the tenantId in other (older) methods is nothing else but the companyId. The older methods need to be refactored.
        int SetCampaign(int companyId, long campaignId, int agentId);

        int Logout(string tenantId, long campaignId, bool isPredictive, string agentId);

        /// <summary>
        /// A function that forcefully logs an Agent out. The function does not wait for ongoing calls to complete.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_AGENT	0x81000006	No Agent could be found for the given Agent ID.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_WRONG_AGENT_STATE	0x81000008	The Agent is not in the right state to execute this function.
        /// ENGINE_RESULT_NOTACTIVE	0x8100000f	Campaign is not active.
        /// ENGINE_RESULT_WRONG_CAMPAIGN_STATE	0x81000010	The Campaign is in the wrong state to run this function.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	The engine is not running.
        /// </returns>
        int KillAgent(string tenantId, long campaignId, string agentId);

        int GoReady(string tenantId, long campaignId, string agentId);

        int GoNotReady(string tenantId, long campaignId, string agentId, string breakName);

        /// <summary>
        /// A function that sends the number to be dialed. Now containing group id for the call, 
        /// allowing to specify the dialing mode for the call, allowing to specify a timeout for 
        /// call aging, and allowing to specify whether the call should be recorded or not.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="diallingMode">New feature allow call to be made in a specific mode independent 
        ///   of campaign default dialing mode</param>
        /// <param name="groupId">Identifier for call group to which this call belongs</param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_GROUP	0X81000022	Group ID does not correspond to any existing group
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOTACTIVE	0x8100000f	Campaign is not active.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        int SendNumber(string tenantId, long campaignId,
                       DialingMode diallingMode, int groupId, int contactId, int callId, string phoneNumber,
                       int callAgingTimeout, bool isRecording);

        /// <summary>
        /// A function that sends a set of numbers to be dialed.
        /// </summary>
        /// <param name="requestId"> </param>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="campaignDiallingMode"></param>
        /// <param name="callList">List of CallInfo objects that contains numbers to be dialed</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_GROUP	0X81000022	Group ID does not belong to any existing group
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOTACTIVE	0x8100000f	Campaign is not active.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        int SendNumbers(
            string requestId,
            string tenantId,
            long campaignId,
            DialingMode campaignDiallingMode,
            List<CallInfo> callList,
            int callAgingTimeout,
            bool isRecording);

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent. 
        /// Now allowing to specify dialing mode for the call.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="diallingMode">New feature allow call to be made in a specific mode independent</param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <param name="respondentVariables">A collection of custom contact fields that may be provided if they are configured for the company and are among the fields in the current campaign.</param>
        /// <returns></returns>
        int SendNumberToAgent(
            string tenantId,
            long campaignId,
            string agentId,
            DialingMode diallingMode,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording,
            string callerId,
            Dictionary<string, object> respondentVariables);

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent. 
        /// Now allowing to specify dialing mode for the call.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="dialingMode">The dilling mode for the particular call. 
        ///   It allows call to be made in a specific mode independent of campaign default dilling mode.</param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <returns></returns>
        int SendNumberToAgentEx(string tenantId, long campaignId, string agentId,
                                DialingMode dialingMode, int contactId, int callId, string phoneNumber,
                                int callAgingTimeout, bool isRecording);

        /// <summary>
        /// A function that initiates redialing by a specific agent. The phone number may be the same as 
        /// for previous dial or may be different.
        /// Note, it's dialer responsibility to do hangup if the agent is in call.
        /// Dialer informs Forsta CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording"></param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <returns></returns>
        int Redial(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId);

        int Hangup(string tenantId, long campaignId, string agentId, int interviewId, long callId);

        int CompleteCall(string tenantId, long campaignId, string agentId, 
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId);

        int SetNextInterview(
            string tenantId,
            long currentCampaignId,
            string agentId,
            InterviewStatus currentInterviewStatus,
            long nextCampaignId,
            int nextInterviewId,
            long nextCallId);

        int StartCustomIvrInterview(
            string tenantId,
            long campaignId,
            string agentId,
            int interviewId,
            long callId,
            string respondentSurveyLink);
        
        /// <summary>
        /// Send the interview extended status to dialer.
        /// This method is called for interviews on interview finish as soon as the final interview extended status is set,
        /// that is just after the interview scheduling process is finished.
        /// This methods is called for all calls in preview/automatic dialing mode, and for connected calls in predictive mode.
        /// Dialer can use this final extended status for different purposes: statistics, etc.
        /// </summary>
        /// <param name="tenantId">Forsta company id</param>
        /// <param name="campaignId">The unique identifier of the Campaign</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="interviewStatus">The interview status, see ConfirmitDialerInterface.InterviewStatus class for details.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        int UpdateInterviewStatus(
            string tenantId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            InterviewStatus interviewStatus);

        /// <summary>
        /// A function that sets Predictive Dialing Engine tuning parameters. 
        /// If the input is set to -1 (the unsigned equivalent of) the parameter will be ignored and not updated.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).
        ///   when the tenant is first created.</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="abandonTarget">The target abandonment rate threshold value, for example, 0.03 = 3%.</param>
        /// <param name="abandonDelay">The delay in seconds before a call will be abandoned.</param>
        /// <param name="estimatedTalkTime">The expected average talk time, measured in seconds (not used).</param>
        /// <param name="ringTimeoutOut">The time before a call is terminated as a no answer, measured in seconds.</param>
        /// <param name="previewTimeOut">The time out period for preview. If this is set, after this period a call will be automatically initiated.</param>
        /// <param name="restrainedDialling">Set the Campaign to run using restrained dialing mode. 
        ///   Restrained dialing mode is used in predictive dialing and ensures that the threshold target 
        ///   abandonment rate is never overstepped, not even temporarily. If restrained dialing is not used 
        ///   the threshold may be overstepped (in such situations the Predictive Dialing Engine will then 
        ///   dial conservatively until the rate falls back under the threshold value). 
        ///   When using restrained dialing the Predictive Dialing Engine basically has to wait for enough 
        ///   calls to have succeeded before trying to over dial, so there is a phase at the beginning of a 
        ///   Campaign or a dialing period where it will be slow to dial predictively; after this initial 
        ///   period there is little difference in the behavior of the two modes.
        /// </param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOTACTIVE	0x8100000f	Campaign is not active.
        /// ENGINE_RESULT_WRONG_CAMPAIGN_STATE	0x81000010	The Campaign is in the wrong state to run this function.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        int SetTuning(string tenantId, long campaignId,
                      string abandonTarget, string abandonDelay, string estimatedTalkTime,
                      string ringTimeoutOut, string previewTimeOut, string restrainedDialling);

        /// <summary>
        /// A function that sets the groups that an agent can take calls for. This function 
        /// allows to change the group setting for an agent who is currently logged into a campaign. 
        /// This function is executed synchronously, the return code will indicate if the setting 
        /// happened successfully. 
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="agentGroups">Array of GroupIDs. This is the new set of groups for that agent.</param>
        /// <returns>If the agent is not logged in or not logged into that campaign an unknown 
        /// agent error will be returned. There are also the usual default error messages.
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_AGENT	0x81000006	No Agent could be found for the given Agent ID.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        int SetGroups(string tenantId, long campaignId, string agentId, int[] agentGroups);

        /// <summary>
        /// Removes the specified calls(numbers) from the dialer. The numbers 
        /// will be returned via NotifyOutcome with a CALL_FLUSHED outcome code. 
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="dialerIds">Ids of targets dialers</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="callsList">The list af calls to be flushed.</param>
        /// <returns>
        /// </returns>
        int FlushNumbers(string tenantId, int[] dialerIds, long campaignId, List<CallInfo> callsList);

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        ///<param name="tenantId"></param>
        ///<param name="campaignId"></param>
        ///<param name="agentId"></param>
        ///<param name="contactId">The respondent ID (interview ID in CATI).</param>
        ///<param name="callId"></param>
        ///<param name="label">The label will be included in the audio record file name.</param>
        int StartRecording(
            string tenantId,
            long campaignId,
            string agentId,
            int contactId,
            int callId,
            string label);

        ///  <summary>
        ///  Stops recording of the interview
        ///  </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="stopRecordingMode"></param>
        /// <returns></returns>
        int StopRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, StopRecordingMode stopRecordingMode);

        /// <summary>
        /// Start play of voice fragment (as wav file) for interview
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="interviewId"></param>
        /// <param name="callId"></param>
        /// <param name="fileName">file name without path</param>
        /// <param name="timeOfPlayingInSeconds"> returns time of playing in seconds</param>
        /// <returns></returns>
        int StartPlayback(
            string tenantId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            string fileName,
            out int timeOfPlayingInSeconds);

        /// <summary>
        /// Stop play voice fragment
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        int StopPlayback(
            string tenantId,
            long campaignId,
            string agentId,
            int callId);

        /// <summary>
        /// Pause or Resume paused playing
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        int PauseOrResumePlayback(
            string tenantId,
            long campaignId,
            string agentId,
            int callId);

        /// <summary>
        /// Switch agent from hear respondent to hear playing and back
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="callId"></param>
        /// <returns></returns>
        int ToggleInterviewerListensToPlaybackOrRespondent(
            string tenantId,
            long campaignId,
            string agentId,
            int callId);

        /// <summary>
        /// A function that starts monitoring Agent calls. This function will be executed synchronously, 
        /// i.e. success return code means that the call was placed on the switch (not connected yet!). 
        /// If the customer or Agent does not exist, or there is any other reason why the call cannot be 
        /// made at that point in time, an appropriate error message will be returned and the call discarded.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="agentId"></param>
        /// <param name="phoneNumber">Supervisor’s telephone number</param>
        /// <param name="sessionId">If an initial monitor has not been performed SessionID should be empty 
        /// and its value will be returned in the return message. If an initial monitor has already been 
        /// performed the SessionID has to be specified and telephone number can be omitted.</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_AGENT	0x81000006	No Agent could be found for the given Agent ID.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	The engine is not running.
        /// </returns>
        int StartMonitor(string tenantId, string agentId, string phoneNumber, ref string sessionId);

        /// <summary>
        /// A function that stops monitoring Agent calls. If the session does not exist, or there is any 
        /// other reason why the call cannot be disconnected, an appropriate error message will be returned 
        /// and the call discarded.
        /// </summary>
        /// <param name="tenantId"> </param>
        /// <param name="sessionId">Indicates which session should be disconnected.</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	The engine is not running.
        /// ENGINE_RESULT_UNKNOWN_SESSION	0x81000027	No SESSION could be found for the given SessionID.
        /// </returns>
        int StopMonitor(string tenantId, string sessionId);

        /// <summary>
        /// Set monitoring mode
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant)</param>
        /// <param name="sessionId">Identifier of monitoring session/param>
        /// <param name="monitorMode">Monitoring mode</param>
        /// <returns></returns>
        int SetMonitorMode(string tenantId, string sessionId, MonitorMode monitorMode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="isRecording"></param>
        /// <returns></returns>
        int CompletePreview(
            string tenantId,
            long campaignId,
            string agentId,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording);

        bool IsPersonModeSupported(AgentTaskChoiceMode mode);
        bool IsReloginNeededOnSurveyChange();
        bool HasInternalHealthControl();
        bool IsDynamicExtensionNumberAllowed(bool isAgentLocal);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        DialerState GetState(int dialerId, string tenantId);

        /// <summary>
        /// Translates a dialer specific call outcome
        /// to the corresponding Open Dialer API <code>CallOutcome</code>
        /// </summary>
        /// <param name="outcome">
        /// Dialer specific call outcome (i.e. internal outcome of this intarface implementator)
        /// </param>
        /// <returns></returns>
        CallOutcome TranslateOutcome(long outcome);

        /// <summary>
        /// Returns flag indicated is hang up option enabled for interviewer or not
        /// </summary>
        bool IsHangUpSupported
        {
            get;
        }

        /// <summary>
        /// Returns flag indicating whether Pause/Resume playback command is enabled for interviewer or not
        /// </summary>
        bool IsPauseOrResumePlaybackSupported
        {
            get;
        }

        /// <summary>
        /// Returns flag indicating whether toggle voice source command is enabled for interviewer or not
        /// </summary>
        bool IsToggleInterviewerListensToPlaybackOrRespondentSupported
        {
            get;
        }

        /// <summary>
        /// Updates dialer configuration parameters.
        /// Not currently used, reserved for future.
        /// </summary>
        /// <exception cref="DialerParametersException"/>
        /// <seealso cref="Initialize"/>
        int SetConfigurationParameters(string tenantId, string configurationParametersXml);

        /// <summary>
        /// Validates campaign dialer parameters.
        /// </summary>
        /// <exception cref="DialerParametersException"/>
        /// <seealso cref="Initialize"/>
        /// <seealso cref="StartCampaign"/>
        int ValidateCampaignParameters(string surveyParametersXml);

        /// <summary>
        /// Updates campaign dialer parameters for a concrete campaign.
        /// The function make sense for started campaigns only. New parameters must come into effect
        /// without restarting the campaign.
        /// </summary>
        int SetCampaignParameters(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string surveyParametersXml);

        /// <summary>
        /// Returns list of instances TrunkLineStateAndAlarms class where is state (and alarms if exist) of each trunk line dialer owns
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="dialerId"></param>
        /// <param name="trunkLineStatesAndAlarms"></param>
        /// <returns></returns>
        int GetTrunkLineStatesAndAlarms(string tenantId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms);

        /// <summary>
        /// Transfers current respondent call to an IVR endpoint
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="interviewId"></param>
        /// <param name="callId"></param>
        /// <param name="endpoint"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        int TransferToIvr(
            string tenantId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes);

        /// <summary>
        /// Passes voice XML statement to dialer 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="voiceXml"></param>
        int IvrRenderVoiceXml(
            int companyId,
            long campaignId,
            int agentId,
            string voiceXml);

        /// <summary>
        /// Configure and activate DDI numbers.It's assumed that all existing 
        /// numbers should be deactivated.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="inboundDdiNumbers"></param>
        /// <returns></returns>
        DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            InboundDdiNumber[] inboundDdiNumbers);

        /// <summary>
        /// Drop inbound call that is not yet routed to an agent 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="inboundCallId"></param>
        /// <param name="audioMessageDescriptor"></param>
        int DropInboundCall(
            int companyId,
            string inboundCallId,
            AudioMessageDescriptor audioMessageDescriptor);

        /// <summary>
        /// Connect inbound call to an available agent in predictive mode
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="campaignId"></param>
        /// <param name="inboundCallId"></param>
        /// <param name="callInfo"></param>
        /// <param name="campaignIdsToBorrowAgentsFrom"></param>
        /// <param name="audioMessageDescriptor"></param>
        int ConnectInboundCall(
            int companyId,
            long campaignId,
            string inboundCallId,
            CallInfo callInfo,
            long[] campaignIdsToBorrowAgentsFrom,
            AudioMessageDescriptor audioMessageDescriptor);

        /// <summary>
        /// Connect inbound call to an agent in non-predictive mode
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="campaignId"></param>
        /// <param name="inboundCallId"></param>
        /// <param name="callInfo"></param>
        /// <param name="audioMessageDescriptor"></param>
        int ConnectInboundCallToAgent(
            int companyId,
            long campaignId,
            string inboundCallId,
            CallInfo callInfo,
            AudioMessageDescriptor audioMessageDescriptor);

        int TransferStart(
            int companyId,
            long campaignId,
            string transferId,
            int agentId,
            TransferType transferType);

        int TransferSetTarget(
            int companyId,
            long campaignId,
            string transferId,
            TargetType targetType,
            string targetResource,
            bool borrowAgentsFromAllCampaigns);

        int TransferSetConnectionState(
            int companyId,
            long campaignId,
            string transferId,
            ConnectionState state);

        int TransferComplete(
            int companyId,
            long campaignId,
            string transferId);

        int TransferCancel(
            int companyId,
            long campaignId,
            string transferId);

        int RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl);

        /// <summary>
        /// Get list of all files from logs folder.
        /// </summary>
        /// <returns>List of file info.</returns>
        IEnumerable<LogFileInfo> GetLogFiles();

        /// <summary>
        /// Get zipped body of specified file from logs folder.
        /// </summary>
        /// <param name="fileName">File name with extension in logs folder.</param>
        /// <returns>Zip archive contained one specified file.</returns>
        byte[] GetLogFileBodyZipped(string fileName);

        /// <summary>
        /// Get dialer product full version.
        /// </summary>
        /// <returns>Version presented in string</returns>
        string GetDialerVersion();

        /// <summary>
        /// Get all information about dialer and dialer WS: versions and name 
        /// </summary>
        /// <returns></returns>
        CodiVersionInfoCommon GetCodiVersionInfo();
    }
}
