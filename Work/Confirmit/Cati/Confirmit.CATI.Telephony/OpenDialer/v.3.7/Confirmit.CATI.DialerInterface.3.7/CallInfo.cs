using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

// ReSharper disable once CheckNamespace
namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Information about a call.
    /// This info is passed to dialer.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public class CallInfo
    {
        /// <summary> The unique identifier of the call. </summary>
        [DataMember]
        public long callId;

        /// <summary> The unique identifier of the interview. </summary>
        [DataMember]
        public int interviewId;

        /// <summary> Respondent telephone number. </summary>
        [DataMember]
        public string phoneNumber;

        /// <summary> Identifier for the agent this call should be assigned to. </summary>
        [DataMember]
        public int agentId;

        /// <summary> The interviewer group for the call. </summary>
        [DataMember]
        public int agentGroupId;

        /// <summary> The time when the call is to be scheduled to. </summary>
        [DataMember]
        public DateTime? timeToCall;

        /// <summary> The call dialing mode.
        /// This feature allows call to be made in a specific mode independent of campaign 
        /// default dialing mode. </summary>
        [DataMember]
        public DialingMode diallingMode;

        /// <summary>
        /// Was the call previously abandoned? 
        /// if this field is true and dialling mode is predictive then the call was abandoned last time 
        /// and as such will not be dialed until an interviewer is available on this attempt.
        /// Default setting: false.
        /// </summary>
        [DataMember]
        public bool wasAbandoned;

        /// <summary>
        /// The number of times this number has been previously dialed.
        /// In predictive mode this value is used in the predictive calling algorithm.
        /// Default setting: 0.
        /// </summary>
        [DataMember]
        public int dialingAttemptsMade;

        /// <summary>
        /// If this number was dialed previously did it ever yield a connect:
        /// a value of 0 means never and a positive value means the number of previous connects,
        /// In predictive mode this value is used in the predictive calling algorithm.
        /// Default value: 0.
        /// </summary>
        [DataMember]
        public int previousConnects;

        /// <summary>
        /// The number of times this number has been previously dispositioned as a No Answer.
        /// Initial value: 0.
        /// </summary>
        [DataMember]
        public int numberOfNoAnswer;

        /// <summary>
        /// Any dialer specific call accompany info. 
        /// Dialer itself knows what to put/get into it and how to handle it.
        /// (PRO-T-S dialer uses this field for PRO-T-S internal flag).
        /// Default: ""
        /// </summary>
        [DataMember]
        public string dialerSpecificAccompanyInfo;

        /// <summary>
        /// Flag which triggers call recording for this call ("False":don’t record, "True":record)
        /// </summary>
        [DataMember]
        public bool isRecording;

        /// <summary>
        ///   Time interval in minutes that tells dialer how long it may keep call in its queue.
        ///   Call should be returned with <see cref="CallOutcome.ReturnedDiallerExpired"/> outcome if it was not dialed before the timeout is expired.
        /// </summary>
        [DataMember]
        public int agingTimeout;

        /// <summary>
        /// Caller ID.
        /// Each number dialed could be given a different caller ID.
        /// Caller ID can be null or empty string if it is not defined.
        /// </summary>
        [DataMember]
        public string callerId;

        /// <summary>
        /// A collection of custom contact fields that may be provided if they are configured for the company and are among the fields in the current campaign.
        /// </summary>
        [DataMember] 
        public Dictionary<string, object> respondentVariables;
        
        /// <summary>
        /// Empty constructor
        /// </summary>
        public CallInfo()
        {
        }

        /// <summary>
        /// Constructor with all parameters
        /// </summary>
        /// <param name="agentId">Identifier for the agent this call should be assigned to.</param>
        /// <param name="interviewId">The unique identifier of the interview.</param>
        /// <param name="callId">The unique identifier of the call.</param>
        /// <param name="agentGroupId">The interviewer group for the call.</param>
        /// <param name="phoneNumber">Respondent telephone number.</param>
        /// <param name="timeToCall">The time when the call is to be scheduled to.</param>
        /// <param name="diallingMode">
        ///   The call dialing mode.
        ///   This feature allows call to be made in a specific mode independent of campaign default dialing mode.
        /// </param>
        /// <param name="wasAbandoned">
        ///   Was the call previously abandoned?
        ///   if this field is true and dialing mode is predictive then the call was abandoned last time 
        ///   and as such will not be dialed until an interviewer is available on this attempt.
        ///   Default setting: false.
        /// </param>
        /// <param name="dialingAttemptsMade">
        ///   The number of times this number has been previously dialed.
        ///   In predictive mode this value is used in the predictive calling algorithm.
        ///   Default setting: 0.
        /// </param>
        /// <param name="previousConnects">
        ///   If this number was dialed previously did it ever yield a connect:
        ///   a value of 0 means never and a positive value means the number of previous connects,
        ///   In predictive mode this value is used in the predictive calling algorithm.
        ///   Default value: 0.
        /// </param>
        /// <param name="numberOfNoAnswer">
        ///   The number of times this number has been previously dispositioned as a No Answer.
        ///   Initial value: 0.
        /// </param>
        /// <param name="dialerSpecificAccompanyInfo">
        ///   Any dialer specific call accompony info. 
        ///   Dialler itself knows what to put/get into it and how to handle it.
        ///   (PRO-T-S dialler uses this field for PRO-T-S internal flag).
        ///   Default: ""
        /// </param>
        /// <param name="isRecording"></param>
        /// <param name="agingTimeout">
        ///   Time interval in minutes that tells dialer how long it may keep calls in its queue.
        ///   Call should be returned with <see cref="CallOutcome.ReturnedDiallerExpired"/> outcome if it was not dialed before the timeout is expired.
        /// </param>
        /// <param name="callerId">
        ///   Caller ID. Can be null or empty string if it is not defined.
        /// </param>
        /// <param name="respondentVariables">
        /// A collection of custom contact fields that may be provided if they are configured for the company and are among the fields in the current campaign.
        /// </param>
        public CallInfo(
            int agentId, 
            int interviewId, 
            long callId, 
            int agentGroupId, 
            string phoneNumber, 
            DateTime? timeToCall, 
            DialingMode diallingMode, 
            bool wasAbandoned, 
            int dialingAttemptsMade, 
            int previousConnects, 
            int numberOfNoAnswer, 
            string dialerSpecificAccompanyInfo,
            bool isRecording,
            int agingTimeout,
            string callerId,
            Dictionary<string, object> respondentVariables)
        {
            this.agentId = agentId;
            this.interviewId = interviewId;
            this.callId = callId;
            this.agentGroupId = agentGroupId;
            this.phoneNumber = phoneNumber;
            this.timeToCall = timeToCall;
            this.diallingMode = diallingMode;
            this.wasAbandoned = wasAbandoned;
            this.dialingAttemptsMade = dialingAttemptsMade;
            this.previousConnects = previousConnects;
            this.numberOfNoAnswer = numberOfNoAnswer;
            this.dialerSpecificAccompanyInfo = dialerSpecificAccompanyInfo;
            this.isRecording = isRecording;
            this.agingTimeout = agingTimeout;
            this.callerId = callerId;
            this.respondentVariables = respondentVariables;
        }

        /// <summary>
        /// String representation of the call with information about all call parameter values
        /// </summary>
        /// <returns>String representation of the call</returns>
        public override string ToString()
        {
            return string.Format(
                "CallInfo[agentId={0}, interviewId={1}, callId={2}, agentGroupId={3}, phoneNumber={4}, " +
                "timeToCall={5}, diallingMode={6}, wasAbandoned={7}, dialingAttemptsMade={8}, previousConnects={9}, " +
                "numberOfNoAnswer={10}, isRecording={11},  agingTimeout={12}, dialerSpecificAccompanyInfo=({13}), callerId = {14}, respondentVariables = {15}]",
                agentId, interviewId, callId, agentGroupId, phoneNumber,
                timeToCall, diallingMode, wasAbandoned, dialingAttemptsMade, previousConnects,
                numberOfNoAnswer, isRecording, agingTimeout, dialerSpecificAccompanyInfo, callerId, $"{{ {(respondentVariables != null ? string.Join(", ", respondentVariables.Select(x => $"{{{x.Key}:{x.Value}}}")) : "")} }}");
        }
    }
}
