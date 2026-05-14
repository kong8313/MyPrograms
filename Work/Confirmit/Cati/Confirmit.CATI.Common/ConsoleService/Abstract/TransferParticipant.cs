namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Participant of interview transfer.
    /// </summary>
    public class TransferParticipant
    {
        /// <summary>
        /// Type of participant - external or internal.
        /// </summary>
        public ParticipantType ParticipantType { get; set; }

        /// <summary>
        /// Contains telephone number or agentId depended by ParticipantType.
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// State of dialing to participant.
        /// </summary>
        public DialingState DialingState { get; set; }

        /// <summary>
        /// Outcome state of dialing to participant.
        /// </summary>
        public DialingStateOutcome DialingStateOutcome { get; set; }
    }

    /// <summary>
    /// Type of participant.
    /// </summary>
    public enum ParticipantType
    {
        /// <summary>
        /// Unspecified resource.
        /// </summary>
        NotDefined = 0,

        /// <summary>
        /// External, determinate by telephone number.
        /// </summary>
        External = 1,

        /// <summary>
        /// Agent, internal, determinate by agentId.
        /// </summary>
        Agent = 2,

        /// <summary>
        /// Agent group, internal.
        /// </summary>
        AgentGroup = 3
    }

    /// <summary>
    /// State of dialing.
    /// </summary>
    public enum DialingState
    {
        NotDefined = 0,
        Dialing = 1,
        Connected = 2,
        NotConnected = 3,
        Waiting = 4,
        Hold = 5
    }

    /// <summary>
    /// Outcome state of dialing.
    /// </summary>
    public enum DialingStateOutcome
    {
        NotDefined = 1,
        Connected = 2,
        Busy = 3,
        NoReply = 4
    }
}