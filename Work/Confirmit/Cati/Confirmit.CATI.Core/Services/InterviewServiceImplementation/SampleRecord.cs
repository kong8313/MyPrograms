using System;

namespace Confirmit.CATI.Core.Services.InterviewServiceImplementation
{
    /// <summary>
    /// Represents a single sample record used during sample upload.
    /// </summary>
    public class RespondentRecord
    {
        /// <summary>
        /// Gets or sets the Confirmit SID of the sample record.
        /// </summary>
        /// <example>LLQRMVFA</example>
        public string Sid { get; set; }

        /// <summary>
        /// Gets or sets the interview id. Actually it contains values of <c>respid</c> column in respondent table.
        /// </summary>
        public int InterviewId { get; set; }

        /// <summary>
        /// Gets or sets the name of the respondent.
        /// </summary>
        public string RespondentName { get; set; }

        /// <summary>
        /// Gets or sets the respondent phone number.
        /// </summary>
        public string RespondentPhone { get; set; }

        /// <summary>
        /// Gets or sets the last call time.
        /// </summary>
        public DateTime? LastCallTime { get; set; }

        /// <summary>
        /// Gets or sets total interview duration.
        /// </summary>
        public int TotalDuration { get; set; }

        /// <summary>
        /// Gets or sets the extension number. (Interviewer telephone number or some ID for local interviewers and dialers).
        /// </summary>
        public string ExtensionNumber { get; set; }

        /// <summary>
        /// Gets or sets the dial attempts.
        /// </summary>
        public int DialAttempts { get; set; }

        /// <summary>
        /// Gets or sets the time zone ID. 0 - no timezone specified.
        /// </summary>
        public int TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the last channel ID (CATI, CAWI, etc).
        /// </summary>
        public byte LastChannelId { get; set; }

        /// <summary>
        /// Gets or sets the resource SID (CATI interviewer or group) taken from CatiInterviewerId column.
        /// It is used in simple scheduling mode to assign interview in specific resource.
        /// </summary>
        public int Resource { get; set; }

        /// <summary>
        /// Gets or sets the time to call. Used in simple scheduling mode to set it to call created for current sample record.
        /// </summary>
        public string CatiCallTime { get; set; }

        /// <summary>
        /// Gets or sets call expiration date/time. Used in simple scheduling mode to set it to call created for current sample record.
        /// </summary>
        public string CatiCallExpirationTime { get; set; }

        /// <summary>
        /// Gets or sets the extended status. Used in simple scheduling mode to set it to interview created for current sample record.
        /// </summary>
        public string CatiExtendedStatus { get; set; }
        
        /// <summary>
        /// Gets or sets the call priority. Used in simple scheduling mode to set it to call created for current sample record.
        /// </summary>
        public string CatiCallPriority { get; set; }
        
        /// <summary>
        /// Gets or sets the call shift type. Used in simple scheduling mode to set it to call created for current sample record.
        /// </summary>
        public string CatiShiftType  { get; set; }
        
        /// <summary>
        /// Gets or sets the call state. Used in simple scheduling mode to set it to call created for current sample record.
        /// </summary>
        public string CatiCallState  { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this record hist the closed quota cell.
        /// </summary>
        public bool IsClosedCell { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether telephone number get into telephone black list or not.
        /// </summary>
        public bool IsTelephoneInBlackList { get; set; }

        /// <summary>
        /// Gets or sets the dial mode. Used for initialization of Dialing mode for our interview.
        /// </summary>
        public int DialMode { get; set; }

        public int ClusteredCellId { get; set; }

        /// <summary>
        /// Get or sets groups ids separated by comma or person id for multiple interview assignment.
        /// </summary>
        public string ResourceIds { get; set; }

        /// <summary>
        /// Get or set sample type id.
        /// </summary>
        public byte DialTypeId { get; set; }

        /// <summary>
        /// Get or set transient state aka ITS aka ExtendedStatus.
        /// </summary>
        public int TransientState { get; set; }
    }
}
