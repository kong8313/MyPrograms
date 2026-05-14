using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Telephony;
using DialerCommon;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Dialer
{
    /// <summary>
    /// Implementation of <see cref="IDialerAPI"/> to use in tests.
    /// </summary>
    public class TestDialer : ITestDialer
    {
        private const int TimeOfPlayingInSeconds = 3;
        public DialType DialType { get; set; }

        /// <summary>
        /// Queue of dialer calls currently waiting to be called by a test dialer. Consists of method names.
        /// </summary>
        private readonly Queue<string> _dialerRequests = new Queue<string>();

        /// <summary>
        /// Actions to run during calls of a test dialer methods. Dictionary key - method name. Value - action to run.
        /// </summary>
        private readonly Dictionary<string, Action> _actionsToRun = new Dictionary<string, Action>();

        /// <summary>
        /// Default actions to run during calls of a test dialer methods. Dictionary key - method name. Value - action to run.
        /// </summary>
        private readonly Dictionary<string, Func<object[], object>> _defaultActionsToRun = new Dictionary<string, Func<object[], object>>();

        public int[] GroupsSentWithLastSetGroups { get; private set; }

        // ReSharper disable InconsistentNaming
        protected bool _hasInternalHealthControlValue = true;
        // ReSharper restore InconsistentNaming

        public virtual bool HasInternalHealthControlValue
        {
            set { _hasInternalHealthControlValue = value; }
            get { return _hasInternalHealthControlValue; }
        }

        public TestDialer(DialType dialType = DialType.Landline)
        {
            DialType = dialType;
            SetDefaultRequestBehavior(nameof(IDialerAPI.FlushNumbers));
            SetDefaultRequestBehavior(nameof(IDialerAPI.IsPersonModeSupported), args => true);
            SetDefaultRequestBehavior(nameof(IDialerAPI.IsReloginNeededOnSurveyChange), args => false);
        }

        /// <summary>
        /// Checks that there are no expected requests in the expectations queue.
        /// </summary>
        public void CheckNoExpectedRequests()
        {
            Assert.AreEqual(
                0,
                _dialerRequests.Count,
                "Test dialer. There shouldn't be any expected requests, but the following methods are remain in the expectations queue: {0}",
                String.Join(", ", _dialerRequests.ToArray()));
        }

        /// <summary>
        /// Makes the method call to be expected by test dialer.
        /// </summary>
        /// <param name="methodName">The expected method name.</param>
        /// <param name="action">The action to execute during call of the method.</param>
        public void AddExpectedRequest(string methodName, Action action = null)
        {
            _dialerRequests.Enqueue(methodName);

            Trace.TraceInformation($"Test dialer. Expected method '{methodName}' has been registered.");

            if (action != null)
            {
                _actionsToRun[methodName] = action;
            }
        }

        public void SetDefaultRequestBehavior(string methodName, Func<object[], object> action = null)
        {
            if (action == null)
                action = args => null;

            Trace.TraceInformation($"Test dialer. Default action for method '{methodName}' has been registered.");

            _defaultActionsToRun[methodName] = action;
        }

        private object ProcessMethodCall(string currentMethodName, params object[] args)
        {
            if (_defaultActionsToRun.ContainsKey(currentMethodName))
            {
                Trace.TraceInformation("Test dialer. Start invoking action for method '{0}'.", currentMethodName);
                var result = _defaultActionsToRun[currentMethodName](args);
                Trace.TraceInformation("Test dialer. Finish invoking action for method '{0}'.", currentMethodName);
                return result;
            }

            Assert.IsTrue(
                _dialerRequests.Any(),
                "Test dialer. There are no expected calls, but method '{0}' has been called. Thread ID {1}.",
                currentMethodName,
                Thread.CurrentThread.ManagedThreadId);

            string expectedRequestMethodName = _dialerRequests.Dequeue();

            Assert.AreEqual(
                expectedRequestMethodName,
                currentMethodName,
                "Test dialer. Method '{0}' has been called, but method '{1}' was expected. Thread ID {2}.",
                currentMethodName,
                expectedRequestMethodName,
                Thread.CurrentThread.ManagedThreadId);

            Trace.TraceInformation(
                "Test dialer. Expected method '{0}' has been called. Thread ID {1}.",
                currentMethodName,
                Thread.CurrentThread.ManagedThreadId);

            if (_actionsToRun.ContainsKey(currentMethodName))
            {
                Trace.TraceInformation("Test dialer. Start invoking action for method '{0}'.", currentMethodName);
                _actionsToRun[currentMethodName]();
                Trace.TraceInformation("Test dialer. Finish invoking action for method '{0}'.", currentMethodName);
                _actionsToRun.Remove(currentMethodName);
            }

            return 0;
        }

        public DialerInitializeResult Initialize(
            int dialerId,
            string tenantId,
            string connectionParametersXml,
            string configurationParametersXml,
            string surveyDefaultParametersXml,
            bool sendInitializeToWebService = true)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return new DialerInitializeResult(DialerErrorCode.Success);
        }

        public int Release(int dialerId, int companyId)
        {
            return 0;
        }

        public DialerFeatures GetFeatures(string tenantId)
        {
            return (DialerFeatures)ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId);
        }

        public int StartCampaign(string tenantId, int[] dialerIds, long campaignId, string campaignName, DialingMode dialingMode, string campaignType, bool recordWholeInterview, string surveyParametersXml)
        {
            return (int)ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, dialerIds, campaignId, campaignName, dialingMode, campaignType, recordWholeInterview, surveyParametersXml);
        }

        public int StopCampaign(string TenantId, int[] dialerIds, long CampaignId, DialingMode Mode)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int KillCampaign(string TenantId, int[] dialerIds, long CampaignId, DialingMode Mode)
        {
            return 0;
        }

        public bool IsReloginNeededOnSurveyChange()
        {
            return (bool)ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
        }

        public virtual bool HasInternalHealthControl()
        {
            return HasInternalHealthControlValue;
        }

        /// <summary>
        /// Returns flag indicated is hang up option enabled for interviewer or not
        /// </summary>
        public bool IsHangUpSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Returns flag indicating whether Pause/Resume playback command is enabled for interviewer or not
        /// </summary>
        public bool IsPauseOrResumePlaybackSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Returns flag indicating whether toggle voice source command is enabled for interviewer or not
        /// </summary>
        public bool IsToggleInterviewerListensToPlaybackOrRespondentSupported
        {
            get { return false; }
        }

        public bool IsDynamicExtensionNumberAllowed(bool isAgentLocal)
        {
            return false;
        }

        public DialerState GetState(int dialerId, string tenantId)
        {
            return (DialerState)ProcessMethodCall(
                MethodBase.GetCurrentMethod().Name,
                tenantId);
        }

        public int Login(
            string tenantId,
            long campaignId,
            string agentId,
            string agentName,
            AgentType agentType,
            string agentExtension,
            string userId,
            bool isPredictive,
            bool isLocal,
            IEnumerable<KeyValuePair<string, string>> agentAttributes)
        {
            ProcessMethodCall(
                MethodBase.GetCurrentMethod().Name,
                tenantId,
                campaignId,
                agentId,
                agentName,
                agentType,
                agentExtension,
                userId,
                isPredictive,
                isLocal,
                agentAttributes);

            return 0;
        }

        public int SetCampaign(int companyId, long campaignId, int agentId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, companyId, campaignId, agentId);
            return 0;
        }

        public int Logout(string TenantId, long CampaignId, bool isPredictive, string AgentId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, TenantId, CampaignId, isPredictive, AgentId);
            return 0;
        }

        /// <summary>
        /// A function that forcefully logs an Agent out. The function does not wait for ongoing calls to complete.
        /// </summary>
        /// <param name="TenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="CampaignId">The unique identifier of the Campaign.</param>
        /// <param name="AgentId">The unique identifier of the Agent.</param>
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
        public int KillAgent(string TenantId, long CampaignId, string AgentId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, TenantId, CampaignId, AgentId);
            return 0;
        }

        public int GoReady(string TenantId, long CampaignId, string AgentId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, TenantId, CampaignId, AgentId);
            return 0;
        }

        public int GoNotReady(string TenantId, long CampaignId, string AgentId, string breakName)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        /// <summary>
        /// A function that sends the number to be dialed. Now containing group id for the call, 
        /// allowing to specify the dialing mode for the call, allowing to specify a timeout for 
        /// call aging, and allowing to specify whether the call should be recorded or not.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="groupId">Identifier for call group to which this call belongs</param>
        /// <param name="contactId">The unique identifier of the Contact.</param>
        /// <param name="callId">The unique identifier of the telephone number.</param>
        /// <param name="phoneNumber">The telephone number.</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="diallingMode">New feature allow call to be made in a specific mode independent 
        /// of campaign default dialing mode</param>
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
        public int SendNumber(
            string tenantId,
            long campaignId,
            DialingMode diallingMode,
            int groupId, int contactId,
            int callId,
            string phoneNumber,
            int callAgingTimeout,
            bool isRecording)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int CompletePreview(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, campaignId, agentId, contactId, callId, phoneNumber, isRecording);
            return 0;
        }

        /// <summary>
        /// A function that sends a set of numbers to be dialed.
        /// </summary>
        /// <param name="requestId"> </param>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="callList">List of CallInfo objects that contains numbers to be dialed</param>
        /// <param name="callAgingTimeout">Call aging, will unload call after specified time (in minutes). 
        ///   Passing in 0 will mean that the call will not age, i.e. stay in the dialler.</param>
        /// <param name="isRecording">Flag which triggers call recording for this call ("False":don’t record, "True":record)</param>
        /// <param name="campaignDiallingMode">New feature allow call to be made in a specific mode independent 
        /// of campaign default dialing mode</param>
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
        public int SendNumbers(
            string requestId,
            string tenantId,
            long campaignId,
            DialingMode campaignDiallingMode,
            List<CallInfo> callList,
            int callAgingTimeout,
            bool isRecording)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, requestId, tenantId, campaignId, campaignDiallingMode, callList, callAgingTimeout, isRecording);
            return 0;
        }

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent. 
        /// Now allowing to specify dialing mode for the call.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">
        ///     The unique identifier of the Campaign.
        /// </param>
        /// <param name="agentId">
        ///     An Agent identifier.
        /// </param>
        /// <param name="diallingMode">
        ///     New feature allow call to be made in a specific mode independent of campaign default dilling mode
        /// </param>
        /// <param name="contactId">
        ///     The unique identifier of the Contact.
        /// </param>
        /// <param name="callId">
        ///     The unique identifier of the telephone number.
        /// </param>
        /// <param name="phoneNumber">
        ///     The telephone number.
        /// </param>
        /// <param name="isRecording">
        ///     Flag which triggers call recording for this call ("False": don’t record, "True": record)
        /// </param>
        /// <param name="callerId">Caller ID</param>
        /// <param name="respondentVariables"></param>
        /// <returns></returns>
        public int SendNumberToAgent(
            string tenantId,
            long campaignId,
            string agentId,
            DialingMode diallingMode,
            int contactId,
            int callId,
            string phoneNumber,
            bool isRecording,
            string callerId,
            Dictionary<string, object> respondentVariables)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, tenantId, campaignId, agentId, diallingMode, contactId, callId, phoneNumber, isRecording, callerId, respondentVariables);
        }

        /// <summary>
        /// A function that sends the number to be dialed, by a specific agent. 
        /// Now allowing to specify dialing mode for the call.
        /// </summary>
        /// <param name="tenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="campaignId">The unique identifier of the Campaign.</param>
        /// <param name="agentId">An Agent identifier.</param>
        /// <param name="diallingMode">
        ///     The dilling mode for the particular call.
        ///     It allows call to be made in a specific mode independent of campaign default dilling mode.
        /// </param>
        /// <param name="contactId">
        ///     The unique identifier of the Contact.
        /// </param>
        /// <param name="callId">
        ///     The unique identifier of the telephone number.
        /// </param>
        /// <param name="phoneNumber">
        ///     The telephone number.
        /// </param>
        /// <param name="callAgingTimeout">
        ///     Call aging, will unload call after specified time (in minutes).
        ///     Passing in 0 will mean that the call will not age, i.e. stay in the dialler.
        /// </param>
        /// <param name="isRecording">
        ///     Flag which triggers call recording for this call (&quot;False&quot;:don’t record, &quot;True&quot;:record)
        /// </param>
        /// <returns></returns>
        public int SendNumberToAgentEx(
            string tenantId,
            long campaignId,
            string agentId,
            DialingMode diallingMode,
            int contactId,
            int callId,
            string phoneNumber,
            int callAgingTimeout,
            bool isRecording)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int Redial(string tenantId, long campaignId, string agentId, int contactId, int callId, string phoneNumber, bool isRecording, string callerId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int Hangup(string tenantId, long campaignId, string agentId, int interviewId, long callId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int CompleteCall(string tenantId, long campaignId, string agentId, 
            InterviewStatus interviewStatus, bool makeAgentReady, string breakName, int interviewId, long callId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, campaignId, agentId, interviewStatus, makeAgentReady, breakName);
            return 0;
        }

        public int SetNextInterview(string tenantId, long currentCampaignId, string agentId, InterviewStatus currentInterviewStatus, long nextCampaignId, int nextInterviewId, long nextCallId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, currentCampaignId, agentId, currentInterviewStatus, nextCampaignId, nextInterviewId, nextCallId);
            return 0;
        }

        public int StartCustomIvrInterview(string tenantId, long campaignId, string agentId, int interviewId, long callId,
            string respondentSurveyLink)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, campaignId, agentId, interviewId, callId, respondentSurveyLink);
            return 0;
        }

        public bool IsPersonModeSupported(AgentTaskChoiceMode mode)
        {
            return (bool)ProcessMethodCall(MethodBase.GetCurrentMethod().Name, mode);
        }

        public int UpdateInterviewStatus(
            string tenantId,
            long campaignId,
            string agentId,
            int interviewId,
            int callId,
            InterviewStatus interviewStatus)
        {
            return (int)DialerErrorCode.Success;
        }

        /// <summary>
        /// A function that sets Predictive Dialing Engine tuning parameters. 
        /// If the input is set to -1 (the unsigned equivalent of) the parameter will be ignored and not updated.
        /// </summary>
        /// <param name="TenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="CampaignId">The unique identifier of the Campaign.</param>
        /// <param name="AbandonTarget">The target abandonment rate threshold value, for example, 0.03 = 3%.</param>
        /// <param name="AbandonDelay">The delay in seconds before a call will be abandoned.</param>
        /// <param name="EstimatedTalkTime">The expected average talk time, measured in seconds (not used).</param>
        /// <param name="RingTimeoutOut">The time before a call is terminated as a no answer, measured in seconds.</param>
        /// <param name="PreviewTimeOut">The time out period for preview. If this is set, after this period a call will be automatically initiated.</param>
        /// <param name="RestrainedDialling">Set the Campaign to run using restrained dialing mode. 
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
        public int SetTuning(string TenantId, long CampaignId, string AbandonTarget, string AbandonDelay, string EstimatedTalkTime, string RingTimeoutOut, string PreviewTimeOut, string RestrainedDialling)
        {
            return 0;
        }

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
        public int SetGroups(string tenantId, long campaignId, string agentId, int[] agentGroups)
        {
            GroupsSentWithLastSetGroups = (int[])agentGroups.Clone();
            return (int)ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, campaignId, agentId, agentGroups);
        }

        /// <summary>
        /// Will remove all numbers for the specified campaign and/or group. The numbers 
        /// will be returned via NotifyOutcome with a CALL_FLUSHED outcome code. 
        /// </summary>
        /// <param name="tenantId">
        ///     The unique identifier of the Product Customer (Tenant).
        /// </param>
        /// <param name="dialerIds"></param>
        /// <param name="campaignId">
        ///     The unique identifier of the Campaign.
        /// </param>
        /// <param name="callsList"></param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_UNKNOWN_TENANT	0x81000004	No Product Customer (Tenant) could be found for the given code.
        /// ENGINE_RESULT_UNKNOWN_CAMPAIGN	0x81000005	No Campaign could be found for the given code.
        /// ENGINE_RESULT_WRONG_GROUP	0x81000021	Group doesn’t exist.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	Failure to connect to service. Transport problem. Most likely problem of communication between Engine and CTI.
        /// </returns>
        public int FlushNumbers(string tenantId, int[] dialerIds, long campaignId, List<CallInfo> callsList)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, campaignId, callsList);
            return 0;
        }

        /// <summary>
        /// Starts open-end or sectional audio recording of the interview.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="campaignId"></param>
        /// <param name="agentId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public int StartRecording(string tenantId, long campaignId, string agentId, int contactId, int callId, string label)
        {
            return 0;
        }

        ///  <summary>
        ///  Stops recording of the interview
        ///  </summary>
        /// <param name="TenantId"></param>
        /// <param name="CampaignId"></param>
        /// <param name="AgentId"></param>
        /// <param name="ContactId"></param>
        /// <param name="CallId"></param>
        /// <param name="stopRecordingMode"></param>
        /// <returns></returns>
        public int StopRecording(string TenantId, long CampaignId, string AgentId, int ContactId, int CallId, StopRecordingMode stopRecordingMode)
        {
            return 0;
        }

        public int StartPlayback(string tenantId, long campaignId, string agentId, int interviewId, int callId, string fileName, out int timeOfPlayingInSeconds)
        {
            timeOfPlayingInSeconds = TimeOfPlayingInSeconds;
            return 0;
        }

        public int StopPlayback(string tenantId, long campaignId, string agentId, int callId)
        {
            return 0;
        }

        public int PauseOrResumePlayback(string tenantId, long campaignId, string agentId, int callId)
        {
            return 0;
        }

        public int ToggleInterviewerListensToPlaybackOrRespondent(string tenantId, long campaignId, string agentId, int callId)
        {
            return 0;
        }

        /// <summary>
        /// A function that starts monitoring Agent calls. This function will be executed synchronously, 
        /// i.e. success return code means that the call was placed on the switch (not connected yet!). 
        /// If the customer or Agent does not exist, or there is any other reason why the call cannot be 
        /// made at that point in time, an appropriate error message will be returned and the call discarded.
        /// </summary>
        /// <param name="TenantId">The unique identifier of the Product Customer (Tenant).</param>
        /// <param name="AgentId"></param>
        /// <param name="Number">Supervisor’s telephone number</param>
        /// <param name="SessionId">If an initial monitor has not been performed SessionID should be empty 
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
        public int StartMonitor(string TenantId, string AgentId, string Number, ref string SessionId)
        {
            return 0;
        }

        /// <summary>
        /// A function that stops monitoring Agent calls. If the session does not exist, or there is any 
        /// other reason why the call cannot be disconnected, an appropriate error message will be returned 
        /// and the call discarded.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="sessionId">Indicates which session should be disconnected.</param>
        /// <returns>
        /// ENGINE_RESULT_SUCCESS	0x81000000	Function terminated successfully.
        /// ENGINE_RESULT_EXCEPTION	0x81000001	Function terminated with an exception.
        /// ENGINE_RESULT_GET_LOCK_FAILED	0x81000007	The function failed to acquire a lock.
        /// ENGINE_RESULT_NOSERVICES	0x81000017	The engine is not running.
        /// ENGINE_RESULT_UNKNOWN_SESSION	0x81000027	No SESSION could be found for the given SessionID.
        /// </returns>
        public int StopMonitor(string tenantId, string sessionId)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name);
            return 0;
        }

        public int SetMonitorMode(string tenantId, string sessionId, MonitorMode monitorMode)
        {
            ProcessMethodCall(MethodBase.GetCurrentMethod().Name, tenantId, sessionId, monitorMode);
            return 0;
        }

        /// <summary>
        /// Translates a dialer specific call outcome
        /// to the corresponding Open Dialer API <code>CallOutcome</code>
        /// </summary>
        /// <param name="outcome">
        /// Dialer specific call outcome (i.e. internal outcome of this intarface implementator)
        /// </param>
        /// <returns></returns>
        public CallOutcome TranslateOutcome(long outcome)
        {
            return (CallOutcome)outcome;
        }

        public int SetConfigurationParameters(string tenantId, string configurationParametersXml)
        {
            return 0;
        }

        public int ValidateCampaignParameters(string surveyParametersXml)
        {
            return 0;
        }

        public int SetCampaignParameters(string tenantId, int[] dialerIds, long campaignId, DialingMode dialingMode, bool recordWholeInterview, string surveyParametersXml)
        {
            return 0;
        }

        public int GetTrunkLineStatesAndAlarms(string tenantId, int dialerId, out IEnumerable<TrunkLineStateAndAlarms> trunkLineStatesAndAlarms)
        {
            trunkLineStatesAndAlarms = new List<TrunkLineStateAndAlarms>();
            return 0;
        }

        public int TransferToIvr(string tenantId, long campaignId, string agentId, int interviewId, int callId, string endpoint,
                                 IEnumerable<KeyValuePair<string, string>> attributes)
        {
            return 0;
        }

        public int IvrRenderVoiceXml(int companyId, long campaignId, int agentId, string voiceXml)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, agentId, voiceXml);
        }

        public DialerErrorCode[] ConfigureInboundDdiNumbers(
            int companyId,
            InboundDdiNumber[] inboundDdiNumbers)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (DialerErrorCode[])ProcessMethodCall(methodName, companyId, inboundDdiNumbers);
        }

        public int DropInboundCall(int companyId, string inboundCallId, AudioMessageDescriptor audioMessageDescriptor)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, inboundCallId, audioMessageDescriptor);
        }

        public int ConnectInboundCall(int companyId, long campaignId, string inboundCallId, CallInfo callInfo,
            long[] campaignIdsToBorrowAgentsFrom, AudioMessageDescriptor audioMessageDescriptor)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, inboundCallId, callInfo, campaignIdsToBorrowAgentsFrom, audioMessageDescriptor);
        }

        public int ConnectInboundCallToAgent(int companyId, long campaignId, string inboundCallId, CallInfo callInfo, AudioMessageDescriptor audioMessageDescriptor)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, inboundCallId, callInfo, audioMessageDescriptor);
        }

        public int TransferStart(int companyId, long campaignId, string transferId, int agentId,
            TransferType transferType)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, transferId, agentId, transferType);
        }

        public int TransferSetTarget(int companyId, long campaignId, string transferId,
            TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, transferId, targetType, targetResource, borrowAgentsFromAllCampaigns);
        }

        public int TransferSetConnectionState(int companyId, long campaignId, string transferId,
            ConnectionState state)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, transferId, state);
        }

        public int TransferComplete(int companyId, long campaignId, string transferId)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, transferId);
        }

        public int TransferCancel(int companyId, long campaignId, string transferId)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (int)ProcessMethodCall(methodName, companyId, campaignId, transferId);
        }

        public IEnumerable<LogFileInfo> GetLogFiles()
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (IEnumerable<LogFileInfo>)ProcessMethodCall(methodName);
        }

        public byte[] GetLogFileBodyZipped(string fileName)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (byte[])ProcessMethodCall(methodName, fileName);
        }

        public string GetDialerVersion()
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (string)ProcessMethodCall(methodName);
        }

        public CodiVersionInfoCommon GetCodiVersionInfo()
        {
            var methodName = MethodBase.GetCurrentMethod().Name;

            return (CodiVersionInfoCommon)ProcessMethodCall(methodName);
        }

        public int RegisterAgentSoftphone(int companyId, int dialerId, int agentId, string agentName, out string login, out string password, out string host, out string extension, out string frontendUrl)
        {
            login = "login";
            password = "password";
            host = "host";
            extension = "ext";
            frontendUrl = "frontendUrl";

            return 0;
        }
    }
}