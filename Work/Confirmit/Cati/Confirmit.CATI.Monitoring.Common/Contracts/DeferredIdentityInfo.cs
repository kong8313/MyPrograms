using System;

namespace Confirmit.CATI.Monitoring.Common.Contracts
{
    /// <summary>
    /// Represents identity object for a deferred record. Used for starting playing deferred record.
    /// </summary>
    [Serializable]
    public class DeferredIdentityInfo
    {
        /// <summary>
        /// Gets/sets company identifier.
        /// </summary>
        public int CompanyID { get; set; }

        /// <summary>
        /// Gets/sets company alias.
        /// </summary>
        public string CompanyAlias { get; set; }

        /// <summary>
        /// Gets/sets deferred record identifier.
        /// </summary>
        public int DeferredRecordID { get; set; }

        /// <summary>
        /// Gets/sets collection of audio records identities.
        /// </summary>
        public AudioIdentityObjectCollection AudioRecords { get; set; }

        /// <summary>
        /// Gets/sets interviewer ID.
        /// </summary>
        public int InterviewerID { get; set; }

        /// <summary>
        /// Gets/sets interviewer name.
        /// </summary>
        public string InterviewerName { get; set; }

        /// <summary>
        /// Gets/sets supervisor name.
        /// </summary>
        public string SupervisorName { get; set; }
    }
}
