using System;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.Core.Activity
{
    /// <summary>
    /// Represents single row of data for task activity view.
    /// </summary>
    public class TaskActivityInfo
    {
        private AlertStatus _lastSubmissionAlert = AlertStatus.Ok;
        private AlertStatus _keepAliveAlert = AlertStatus.Ok;
        private AlertStatus _noActivityAlert = AlertStatus.Ok;
        private AlertStatus _interviewDurationAlert = AlertStatus.Ok;
        private AlertStatus _breakDurationAlert = AlertStatus.Ok;

        /// <summary>
        /// Gets summary for alert statuses.
        /// </summary>
        public AlertStatus Alert
        {
            get
            {
                if (LastSubmissionAlert == AlertStatus.Error || KeepAliveAlert == AlertStatus.Error || NoActivityAlert == AlertStatus.Error || InterviewDurationAlert == AlertStatus.Error || BreakDurationAlert == AlertStatus.Error)
                    return AlertStatus.Error;
                if (LastSubmissionAlert == AlertStatus.Warning || KeepAliveAlert == AlertStatus.Warning || NoActivityAlert == AlertStatus.Warning || InterviewDurationAlert == AlertStatus.Warning || BreakDurationAlert == AlertStatus.Warning)
                    return AlertStatus.Warning;
                return AlertStatus.Ok;
            }
        }

        /// <summary>
        /// Gets or sets alert status.
        /// </summary>
        public AlertStatus LastSubmissionAlert
		{
            get { return _lastSubmissionAlert; }
            set { _lastSubmissionAlert = value; }
		}

        /// <summary>
        /// Gets or sets KeepAlive alert status.
        /// </summary>
        public AlertStatus KeepAliveAlert
        {
            get { return _keepAliveAlert; }
            set { _keepAliveAlert = value; }
        }

        /// <summary>
        /// Gets or sets State alert status.
        /// </summary>
        public AlertStatus NoActivityAlert
        {
            get { return _noActivityAlert; }
            set { _noActivityAlert = value; }
        }

        /// <summary>
        /// Gets or sets interview duration alert status.
        /// </summary>
        public AlertStatus InterviewDurationAlert
        {
            get { return _interviewDurationAlert; }
            set { _interviewDurationAlert = value; }
        }

        /// <summary>
        /// Gets or sets break duration alert status.
        /// </summary>
        public AlertStatus BreakDurationAlert
        {
            get { return _breakDurationAlert; }
            set { _breakDurationAlert = value; }
        }

        /// <summary>
        /// Gets or sets BvFEE sid of survey.
        /// </summary>
        public int SurveySID { get; set; }

	    /// <summary>
	    /// Gets or sets interview ID.
	    /// </summary>
	    public int InterviewID { get; set; }

	    /// <summary>
	    /// Gets or sets confirmit project id.
	    /// </summary>
	    public string ProjectId { get; set; }

	    /// <summary>
	    /// Gets or sets confirmit project name.
	    /// </summary>
	    public string ProjectName { get; set; }

        /// <summary>
        ///Gets or sets CallCenter name.
        /// </summary>
        public string CallCenterName { get; set; }

        /// <summary>
        /// Gets or sets BvFEE sid of interviewer.
        /// </summary>
        public int PersonSID { get; set; }

	    /// <summary>
	    /// Gets or sets BvFEE name of interviewer.
	    /// </summary>
	    public string InterviewerName { get; set; }

	    /// <summary>
	    /// Gets or sets time call delivered to interviewer.
	    /// </summary>
	    public DateTime? TimeCallDelivered { get; set; }

        /// <summary>
        /// Gets or sets the amount os seconds since time state was changed for selecting, no calls and waiting (for predictive dialing) operations. In fact it is start time for these operations.
        /// </summary>
        public int? SecondsSinceStateChanged { get; set; }

        /// <summary>
        /// Gets or sets interview status.
        /// </summary>
        public string State { get; set; }

	    /// <summary>
	    /// Gets or sets timezone name.
	    /// </summary>
	    public string TimezoneName { get; set; }

	    /// <summary>
	    /// Gets or sets timezone ID.
	    /// </summary>
	    public int TimezoneID { get; set; }

	    /// <summary>
	    /// Gets or sets dialling mode.
	    /// </summary>
        public DialingMode DiallingMode { get; set; }

	    /// <summary>
	    /// Gets or sets call outcome.
	    /// </summary>
	    public int CallOutcome { get; set; }

	    /// <summary>
	    /// Gets or sets logout status.
	    /// </summary>
	    public LoginState StatusLogout { get; set; }

	    /// <summary>
	    /// Seconds elapsed since last submission of data from interviewer.
	    /// </summary>
	    public int? SecondsElapsed { get; set; }

	    /// <summary>
	    /// Is task monitored.
	    /// </summary>
	    public bool IsMonitored { get; set; }

        /// <summary>
        /// Gets or sets the name of the supervisor performing a monitoring.
        /// </summary>
        public string SupervisorName { get; set; }

        /// <summary>
        /// Gets or sets the monitoring session ID.
        /// </summary>
        public long MonitoringSessionID { get; set; }

        /// <summary>
        /// Gets or sets the state of the interview.
        /// </summary>
        public InterviewState InterviewState { get; set; }

        /// <summary>
        /// Gets or sets the time of last keep alive message.
        /// </summary>
        public DateTime? LastKeepAliveTime { get; set; }

        /// <summary>
        /// Gets or sets the logged in to dialer state.
        /// </summary>
        public LoginState LoggedInToDialer { get; set; }

        /// <summary>
        /// Gets or sets the type of the problem.
        /// </summary>
        public int ProblemState { get; set; }

        /// <summary>
        /// Gets or sets station identifier
        /// </summary>
        public string StationIdentifier { get; set; }

        /// <summary>
        /// Gets or sets dial type
        /// </summary>
	    public string DialType { get; set; }

        /// <summary>
        /// Gets or sets the amount of seconds of  Open End review session.
        /// </summary>
        public int? OpenEndReviewInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the dialer id.
        /// </summary>
        public int DialerId { get; set; }

        /// <summary>
        /// Gets or sets the agent type.
        /// </summary>
        public AgentType AgentType { get; set; }

        /// <summary>
        /// Gets or sets the amount of seconds of an itnterview
        /// </summary>
        public int? InterviewDurationInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the amount of seconds of an break
        /// </summary>
        public int? BreakDurationInSeconds { get; set; }

        public CallTypes CallType { get; set; }

        public string LinkedChain { get; set; }

        public CallConnectionState CallConnectionState { get; set; }

        public string BreakTypeName { get; set; }

        public bool? InterviewScreenRecording { get; set; }

        public bool IsLiveMonitoringEnabled { get; set; }

        public bool IsWebConsole { get; set; }
    }
}