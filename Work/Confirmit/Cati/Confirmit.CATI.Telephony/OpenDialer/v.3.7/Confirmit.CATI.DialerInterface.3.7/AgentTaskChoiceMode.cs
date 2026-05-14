using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Defines task choice mode (AKA call delivery mode) for CATI interviewer.
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum AgentTaskChoiceMode
    {
        /// <summary>
        /// The system decides which campaign this interviewer should work with, and which call should be delivered to the
        /// interviewer at each moment.
        /// </summary>
        [EnumMember]
        Automatic = 0,

        /// <summary>
        /// The interviewer selects both a campaign and a call himself.
        /// </summary>
        [EnumMember]
        Manual = 1,

        /// <summary>
        /// The interviewer selects a campaign, and the system selects a call to work with.
        /// </summary>
        [EnumMember]
        CampaignAssignment = 2,

        /// <summary>
        /// Interviewer can choose task choice by himself.
        /// </summary>
        [EnumMember]
        Choice = 3
    }
}
