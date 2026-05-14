using System.Collections.Generic;
using System.ServiceModel;
using ConfirmitDialerInterface;
using DialerCommon.DialerExceptions;
using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Telephony.DialerService.Contract
{
    [ServiceContract]
    public interface IDialerServiceCore
    {
        /// <summary>
        /// Dialer name to show in CATI supervisor
        /// Ex: "Pro-T-S", "TCI", ...
        /// </summary>
        /// <returns>Dialer name</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        string GetName();

        /// <summary>
        /// Version of dialer provider's dialer to show it in CATI supervisor.
        /// Ex: "1.0", 
        /// </summary>
        /// <returns>Dialer version</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        string GetVersion();

        /// <summary>
        /// 0 - ConfirmitDialerInterface AssemblyVersion, 
        /// 1 - ConfirmitDialerInterface AssemblyInformationalVersion, 
        /// 2 - Dialer driver dll name and dll version
        /// </summary>
        /// <returns>Versions of ConfirmitDialerInterface assembly and dialer provider's dialer dll version</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        string[] Version();

        /// <summary>
        /// Initializes dialer to work with campaigns, agents, calls etc.
        /// Confirmit CATI does not call this interface methods until dialer is not initialized. 
        /// Note: actually some methods can still be called, see 'Workflow' chapter of 'Confirmit Open Dialer Interface' document for details.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId"></param>
        /// <param name="configurationParametersXml">Dialer configuration parameters including obligatory parameters.
        ///   See chapter 'Dialer parameters' of 'Confirmit Open Dialer Interface' document for details. </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode Release();

        /// <summary>
        /// Updates dialer configuration parameters.
        /// Can throw ParametersException if parameters xml is incorrect or contains wrong parameters or some parameter values are incorrect, out of range etc.
        /// </summary>
        /// <exception cref="ParametersException"/>
        /// <seealso cref="Initialize"/>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerParametersExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode SetConfigurationParameters(int companyId, string configurationParametersXml);

        /// <summary>
        /// Confirmit CATI calls this method regularly to know the dialer state. 
        /// DialerState.Available return value indicates that dialer is alive.
        /// If dialer returns DialerState.Unavailable or GetState call fails then Confirmit CATI tries to call GetState again and again during appropriate time.
        /// If GetState calls still return DialerState.Unavailable then Confirmit CATI makes dialer unavailable and finally calls IDialercoreApi.Release.
        /// Dialer can also use IDialerEvents.NotifyDialerState to notify Confirmit CATI about dialer state change. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <returns>Actual dialer state</returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerState GetState(int companyId, int dialerId);

        /// <summary>
        /// Starts campiagn on dialer.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerIds">Ids of targets dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="campaignName">The description super see in GUI </param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <param name="recordWholeInterview">Is whole interview recording switched on for the campaign?</param>
        /// <param name="campaignParametersXml">The campaign parameters </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerParametersExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode StartCampaign(int companyId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml);

        /// <summary>
        /// Stops campiagn on dialer.
        /// Dialer does not interrupt active calls, it allows agents to complete them.
        /// In case of predictive campaign dialer must return predictive calls what are in queue back to Confirmit CATI 
        /// through IDialerEvents.NotifyOutcome with outcome 'ReturnedNotDialled'.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerIds">Ids of targets dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode StopCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode);

        /// <summary>
        /// Stops campiagn on dialer and forcibly terminates all active calls (conversations).
        /// In case of predictive campaign dialer must return predictive calls what are in queue back to Confirmit CATI 
        /// through IDialerEvents.NotifyOutcome with outcome 'ReturnedNotDialled',
        /// 'Stopped' outcome must be returned for interrupted calls.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerIds">Ids of targets dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode KillCampaign(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode);

        /// <summary>
        /// Updates campaign dialer parameters for a concrete campaign.
        /// The function make sense for started campaigns only. New parameters must come into effect
        /// without restarting the campaign.
        /// Can throw ParametersException if parameters xml is incorrect or contains wrong parameters or some parameter values are incorrect, out of range etc.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerIds">Ids of target dialers</param>
        /// <param name="campaignId">The campaign unique identifier</param>
        /// <param name="dialingMode">The campiagn dialing mode</param>
        /// <param name="recordWholeInterview">Is whole interview recording switched on for the campaign?</param>
        /// <param name="campaignParametersXml">Campaign parameters</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerParametersExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode SetCampaignParameters(int companyId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string campaignParametersXml);

        /// <summary>
        /// Login agent to dialer.
        /// Dialer informs Confirmit CATI about the agent login result via IDialerEvents.NotifyAgentState event (if agent is not already logged in to dialer).
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">
        ///     The campaign unique identifier.
        ///     Confirmit CATI can pass campaignId=0. This means that agent will be logged in not to a concrete campaign and can work with many campaigns during one login session.
        ///     Note: Zero campaignId will be passed only for automatic task choice agents and only if dialer supports automatic task choice.
        ///     See 'SupportedPersonModes' parameter in 'Dialer parameters' chapter of 'Confirmit Open Dialer Interface' document for details.
        /// </param>
        /// <param name="agentId">The agent uniquew identifier</param>
        /// <param name="agentName">The agent name </param>
        /// <param name="agentConnectionString">Agent telephone extension or telephone number</param>
        /// <param name="isPredictive">Is the agent logging in for predictive calls?</param>
        /// <param name="resourceBindingType">The agent resource binding type Local, Name or PhoneNumber </param>
        /// <param name="agentAttributes">
        ///     Each attribute is represented as a key-value pair.
        ///     At this time Confirmit CATI passes only one attribute with key 'Location' via this parameter,
        ///     'Location' attribute can be used by the dialer provider to select the correct agent resource (line)
        ///     according to the agent location.
        ///     Later more attributes could be supported.
        /// </param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// DialerErrorCode.AgentAlreadyLoggedIn if the agent is already logged in.
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode Login(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            string agentName,
            string agentConnectionString,
            bool isPredictive,
            ResourceBindingType resourceBindingType,
            IEnumerable<KeyValuePair<string, string>> agentAttributes);

        /// <summary>
        /// Changes current campaign for a logged in agent.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The campaign unique identifier </param>
        /// <param name="agentId">Dialer parameters for the campaign</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode SetCampaign(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The campaign unique identifier </param>
        /// <param name="agentId">The agent uniquew identifier</param>
        /// <param name="isPredictive">Has the agent been working predictively?</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode Logout(int companyId, int dialerId, long campaignId, int agentId, bool isPredictive);

        /// <summary>
        /// A function that forcibly logs an Agent out. The function does not wait for ongoing calls to complete.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode KillAgent(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// Makes agent ready to receive calls.
        /// Confirmit CATI always calls this method after login.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode GoReady(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// Makes agent not ready to receive calls. 
        /// Agent still stays logged in.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode GoNotReady(int companyId, int dialerId, long campaignId, int agentId);


        /// <summary>
        /// A function that sets the groups that an agent can take calls for. This function 
        /// allows to change the group setting for an agent who is currently logged into a campaign. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="agentGroups">array of agent groups identifiers</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode SetGroups(int companyId, int dialerId, long campaignId, int agentId, int[] agentGroups);

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent allowing to specify dialing mode for the call.
        /// Another name for this function could be 'Dial'.
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="dialingMode">Dialling mode for the call</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode SendNumberToAgent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            DialingMode dialingMode,
            int interviewId,
            long callId,
            string phoneNumber,
            bool isRecording,
            string callerId);

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent allowing to specify dialing mode for the call.
        /// Another name for this function could be 'Dial'.
        /// Note, it's dialer responsibility to do hangup if the agent is in call.
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="callerId">Caller ID. Can be null or empty string if it is not defined.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        /// Dialer informs Confirmit CATI about the dial result via IDialerEvents.NotifyOutcome event for each call.
        /// </summary>
        /// <param name="requestId"> </param>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Id of the target dialer</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="campaignDialingMode">The campaign dialing mode</param>
        /// <param name="callList">List of CallInfo objects that contains numbers to be dialed</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialer.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode Hangup(int companyId, int dialerId, long campaignId, int agentId);

        /// <summary>
        /// Makes hangup (if respondent is off-hook) and completes the call for the agent.
        /// After this the agent can start working with another call (if ready).
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewStatus">The interview status, see ConfirmitDialerInterface.InterviewStatus class for details.</param>
        /// <param name="makeAgentReady">true to make agent ready for next calls, false otherwise</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode CompleteCall(int companyId, int dialerId, long campaignId, int agentId, InterviewStatus interviewStatus, bool makeAgentReady);

        /// <summary>
        /// Send the interview extended status to dialer.
        /// This method is called for each interview on interview finish as soon as the final interview extended status is set,
        /// that is just after the interview scheduling process is finished.
        /// Dialer can use this final extended status for different purposes: statistics, etc.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="interviewId">The unique identifier of the interview connected to the call</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <param name="interviewStatus">The interview status, see ConfirmitDialerInterface.InterviewStatus class for details.</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode UpdateInterviewStatus(
            int companyId,
            int dialerId,
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
        /// <param name="dialerId">Dialer id who perform the operation</param>
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
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        /// Dialer informs Confirmit CATI about the operation result via IDialerEvents.NotifyOutcome event.
        /// Dialer can answer with outcome 'ReturnedNotDialled' for calls in queue, or 'Stopped' outcome for interrupted calls.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerIds">Ids of targets dialers</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="callList">List of calls to be flushed</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode FlushNumbers(int companyId, int[] dialerIds, long campaignId, List<CallInfo> callList);

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        ///<param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        ///<param name="campaignId">The unique identifier of the Campaign.</param>
        ///<param name="agentId">The unique identifier of the Agent.</param>
        ///<param name="interviewId">The unique identifier of the interview connected to the call</param>
        ///<param name="callId">The unique identifier of the call</param>
        ///<param name="label">The label will be included into the audio record file name.</param>
        ///<returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode StartRecording(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string label);

        /// <summary>
        /// Stops recording of the interview.
        /// </summary>
        ///<param name="companyId">Confirmit company id</param>
        ///<param name="dialerId">Dialer id who perform the operation</param>
        ///<param name="campaignId">The unique identifier of the Campaign.</param>
        ///<param name="agentId">The unique identifier of the Agent.</param>
        ///<param name="interviewId">The unique identifier of the interview connected to the call</param>
        ///<param name="callId">The unique identifier of the call</param>
        /// <param name="stopRecordingMode"></param>
        ///<returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
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
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode StopPlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId);

        /// <summary>
        /// Pauses or Resumes paused playing.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode PauseOrResumePlayback(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId);

        /// <summary>
        /// Switchs agent from hearing respondent to hearing playing and back.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">The unique identifier of the Agent.</param>
        /// <param name="callId">The unique identifier of the call</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode ToggleInterviewerListensToPlaybackOrRespondent(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            long callId);

        /// <summary>
        /// Starts audio monitoring of the agent calls. 
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="agentId">Identifier af the agent to be monitored</param>
        /// <param name="resourceBindingType"></param>
        /// <param name="sessionId">If an initial monitor has not been performed SessionID should be empty 
        /// and its value will be returned in the return message. If an initial monitor has already been 
        /// performed the SessionId has to be specified and telephone number can be omitted.</param>
        /// <param name="supervisorName"></param>
        /// <param name="supervisorConnectionString"></param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
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
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
        /// <param name="sessionId">Identifier of monitoring session to be stopped</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode StopMonitor(int companyId, int dialerId, string sessionId);

        /// <summary>
        /// Returns list of instances TrunkLineStateAndAlarms class where is state (and alarms if exist) of each trunk line dialer owns
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Id of target dialer</param>
        /// <param name="trunkLineStatesAndAlarms">The result list of states and alarms</param>
        /// <returns>
        /// DialerErrorCode.Success if succeeded
        /// Other DialerErrorCode if failed
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode GetTrunkLineStatesAndAlarms(int companyId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms);

        /// <summary>
        /// Transfers interview to IVR.
        /// This means that Interactive Voice Response will continue the interview.
        /// Note: This method call does not rescind any other methods like Hangup or CompleteCall.
        /// CATI will still call any dialer methods pointed in the survey script and it will call CompleteCall as for usual interview.
        /// </summary>
        /// <param name="companyId">Confirmit company id</param>
        /// <param name="dialerId">Dialer id who perform the operation</param>
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
        [OperationContract]
        [FaultContract(typeof(DialerWsInvalidCredentialsExceptionDetails))]
        [FaultContract(typeof(DialerExceptionDetail))]
        DialerErrorCode TransferToIvr(
            int companyId,
            int dialerId,
            long campaignId,
            int agentId,
            int interviewId,
            long callId,
            string endpoint,
            IEnumerable<KeyValuePair<string, string>> attributes);
    }
}
