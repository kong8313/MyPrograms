
namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// State of interview transfer.
    /// </summary>
    public class ConsoleTransferState
    {
        public ConsoleConnectionState ConnectionState { get; set; }
        /// <summary>
        /// Transfer initiator agent.
        /// </summary>
        public TransferParticipant Initiator { get; set; }

        /// <summary>
        /// Respondent of interview.
        /// </summary>
        public TransferParticipant Respondent { get; set; }

        /// <summary>
        /// Target of transfer interview.
        /// </summary>
        public TransferParticipant Target { get; set; }
    }
}