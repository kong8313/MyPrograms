using System;

namespace Confirmit.CATI.REST.SDK.Model
{
    /// <summary>
    /// Class representing information about the history of the interviewer session
    /// </summary>
    public class InterviewerSessionHistory
    {
        /// <summary>
        /// Unique identifier of the session
        /// </summary>
        public int SessionId { get; set; }

        /// <summary>
        /// Unique identifier of the call center
        /// </summary>
        public int CallCenterId { get; set; }

        /// <summary>
        /// Unique identifier of the interviewer
        /// </summary>
        public int InterviewerId { get; set; }

        /// <summary>
        /// Date and time of the login
        /// </summary>
        public DateTime LoginTime { get; set; }

        /// <summary>
        /// Date and time of the logout
        /// </summary>
        public DateTime? LogoutTime { get; set; }
    }
}