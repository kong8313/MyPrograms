using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Describes the current interview/interviewer state.
    /// </summary>
    public class State
    {
        /// <summary>
        /// The empty constructor.
        /// (Required for serialization needs.)
        /// </summary>
        public State()
        {
        }

        /// <summary>
        /// Creates State object and initializes it by parameters.
        /// </summary>
        public State(string surveyId,
                      string surveyDescription,
                      int interviewId,
                      string interviewURL,
                      Timezone respondentTimezone,
                      int interviewState,
                      int callOutcome,
                      int interviewerLoginState,
                      int interviewerLoginToDialerState,
                      int problemState,
                      CallTypes callType,
                      int projectId,
                      bool isSurveyRecorded,
                      DateTime? breakStartTime,
                      int deferredRecordId,
                      int? languageVariableValue,
                      bool isNewSurvey,
                      ConsoleTransferState transferState,
                      ExternalTransferType externalTransferType,
                      InternalTransferType internalTransferType,
                      TransferOptions transferOptions)
        {
            this.surveyId = surveyId;
            this.surveyDescription = surveyDescription;
            this.interviewId = interviewId;
            this.interviewURL = interviewURL;
            this.respondentTimezone = respondentTimezone;
            this.interviewState = interviewState;
            this.callOutcome = callOutcome;
            this.interviewerLoginState = interviewerLoginState;
            this.interviewerLoginToDialerState = interviewerLoginToDialerState;
            this.problemState = problemState;
            this.callType = callType;
            this.projectId = projectId;
            this.isSurveyRecorded = isSurveyRecorded;
            this.breakStartTime = breakStartTime;
            this.deferredRecordId = deferredRecordId;
            this.languageVariableValue = languageVariableValue;
            this.isNewSurvey = isNewSurvey;
            this.transferState = transferState;
            this.externalTransferType = externalTransferType;
            this.internalTransferType = internalTransferType;
            this.transferOptions = transferOptions;
        }

        public State(
            string surveyId,
            string surveyDescription,
            int interviewId,
            string interviewURL,
            Timezone respondentTimezone,
            int interviewState,
            int callOutcome,
            int interviewerLoginState,
            int interviewerLoginToDialerState,
            int problemState,
            int projectId,
            bool isSurveyRecorded)
            : this(surveyId,
                      surveyDescription,
                      interviewId,
                      interviewURL,
                      respondentTimezone,
                      interviewState,
                      callOutcome,
                      interviewerLoginState,
                      interviewerLoginToDialerState,
                      problemState,
                      (int)CallTypes.Outbound,
                      projectId,
                      isSurveyRecorded,
                      null,
                      0,
                      null,
                      false,
                      null,
                      ExternalTransferType.Cold,
                      InternalTransferType.Off,
                      null)
        {
        }

        /// <summary>
        /// The interview owner survey id.
        /// Interview is uniquely identified by the pair (surveyId, interviewId).
        /// Confirmit survey ID like pNNNNNNN is used.
        /// </summary>
        public string surveyId;

        /// <summary>
        /// Confirmit survey name.
        /// </summary>
        public string surveyDescription;

        /// <summary>
        /// The interview id.
        /// Interview is uniquely identified by the pair (surveyId, interviewId).
        /// </summary>
        public int interviewId;

        /// <summary> 
        /// The interview URL. 
        /// CATI console uses this URL in order to start the interview at Confirmit. 
        /// </summary> 
        /// <remarks>
        /// URL format: http://ConfirmitServer/ConfirmitProjectID.aspx?r=InterviewId&s=SecurityKey
        /// where 
        ///     ConfirmitServer is a Confirmit server site, 
        ///     ConfirmitProjectID is a surveyId (Confirmit format us used),
        ///     InterviewId identifies an interview,
        ///     SecurityKey provides access control.
        /// </remarks>
        /// <example>
        /// http://localhost/confirm/p1234567.aspx?r=1&s=ABCDEF
        /// </example>
        public string interviewURL;

        /// <summary>
        /// The interview state
        /// </summary>
        public int interviewState;

        /// <summary>
        /// The result of dialing operation
        /// </summary>
        public int callOutcome;

        /// <summary>
        /// The respondent time zone.
        /// </summary>
        public Timezone respondentTimezone;

        /// <summary>
        /// Interviewer login state
        /// </summary>
        public int interviewerLoginState;

        /// <summary>
        /// Interviewer login to dialer state
        /// </summary>
        public int interviewerLoginToDialerState;

        /// <summary>
        /// The problem state.
        /// </summary>
        public int problemState;

        /// <summary>
        /// The call type: outbound, inbound, transfer or incoming transfer.
        /// </summary>
        public CallTypes callType;

        /// <summary>
        /// Backend survey identifier.
        /// </summary>
        public int projectId;

        /// <summary>
        /// Survey IsRecording flag.
        /// </summary>
        public bool isSurveyRecorded;

        /// <summary>
        /// Time when break was started
        /// </summary>
        public DateTime? breakStartTime;

        public int deferredRecordId;

        public int? languageVariableValue;

        public bool isNewSurvey;

        public ConsoleTransferState transferState;

        public ExternalTransferType externalTransferType;

        public InternalTransferType internalTransferType;

        public TransferOptions transferOptions;
    }
}