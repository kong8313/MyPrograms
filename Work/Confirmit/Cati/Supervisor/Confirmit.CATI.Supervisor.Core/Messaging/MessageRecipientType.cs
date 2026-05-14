using System;

namespace Confirmit.CATI.Supervisor.Messaging
{    
    /// <summary>
    /// Represents message recipient type
    /// </summary>
    public enum MessageRecipientType
    {
        /// <summary>
        /// Means message will be sent to selected interviewers
        /// </summary>
        Interviewer,
        /// <summary>
        /// Means message will be sent to selected interviewer groups
        /// </summary>
        InterviewerGroup,
        /// <summary>
        /// Means message will be sent to selected surveys
        /// </summary>
        Survey        
    }    
}
