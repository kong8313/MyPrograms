using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Describes an CATI appointment.
    /// </summary>
    public class Appointment
    {
        /// <summary>
        /// The appointment unique identifier.
        /// </summary>
        public int id;

        /// <summary>
        /// The appointment interview identifier.
        /// </summary>
        public int InterviewId;

        /// <summary>
        /// The respondent contact name.
        /// It is supposed the interviewer uses this name to address to the respondent by phone.
        /// </summary>
        public string contactName;

        /// <summary>
        /// The appointment time.
        /// </summary>
        public DateTime time;

        /// <summary>
        /// The appointment expiration time.
        /// <value>null</value> expitationTime means that the appointment never expire.
        /// </summary>
        public DateTime? expirationTime;

        /// <summary>
        /// Confirmit project ID in a format p(n), where (n) is decimal ID padded by 0's from the left up to site-wide limit.
        /// </summary>
        public string projectID;

        /// <summary>
        /// Name of confirmit project or survey name in BE
        /// </summary>
        public string projectName;

        /// <summary>
        /// Timezone of appointment (the same as at interview)
        /// </summary>
        public Timezone appointmentTimeZone;

        /// <summary>
        /// State of appointment
        /// </summary>
        public int? state;
    }
}