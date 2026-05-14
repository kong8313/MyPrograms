using System.Runtime.Serialization;

namespace ConfirmitDialerInterface
{
    /// <summary>
    /// Possible ways to select calls for predictive dialing mode
    /// </summary>
    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/ConfirmitDialerInterface")]
    public enum CallsSelectionAlgorithm
    {
        /// <summary>
        /// Select calls by person groups
        /// </summary>
        [EnumMember]
        ByPersonGroup = 0,

        /// <summary>
        /// Select calls by campaign
        /// </summary>
        [EnumMember]
        ByCampaign = 1,

        /// <summary>
        /// Select calls assigned to campaign only,
        /// but not assigned neither to person groups nor to concrete agents)
        /// </summary>
        [EnumMember]
        CallsAssignedToCampaignOnly = 2,

        /// <summary>
        /// Select calls explicitly assigned to any agents
        /// </summary>
        [EnumMember]
        CallsAssignedToAgentsExplicitly = 3
    }
}
