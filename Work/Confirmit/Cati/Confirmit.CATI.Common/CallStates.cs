namespace Confirmit.CATI.Common
{
    /// <summary>
    /// Represents the enumeration of states of call.
    /// TODO: Describe negative state values
    /// </summary>
     public enum CallStates
    {
        /// <summary>
        /// Scheduled calls.
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// Suspended calls.
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// All interviews.
        /// </summary>
        All = 3,

        /// <summary>
        /// High priority calls per group including calls sent to dialer
        /// </summary>i
        HighPriority = 4,

        /// <summary>
        /// Calls sent to dialer
        /// </summary>
        SentToDialer = 5,
        
        /// <summary>
        /// Only calls which are available to dial currently will be shown 
        /// </summary>
        CallsAvailableNow = 1000
    }
}
