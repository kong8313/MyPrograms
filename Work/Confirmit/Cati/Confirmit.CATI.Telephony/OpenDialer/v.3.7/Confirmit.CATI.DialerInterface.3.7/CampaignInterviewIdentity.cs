using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Represents identity pair (campaign identifier, agent identifier).
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public struct CampaignInterviewIdentity
    {
        /// <summary>
        /// Gets or sets campaign identifier.
        /// </summary>
        [DataMember]
        public long CampaignId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets interview identifier.
        /// </summary>
        [DataMember]
        public int InterviewId
        {
            get;
            set;
        }
    }
}
