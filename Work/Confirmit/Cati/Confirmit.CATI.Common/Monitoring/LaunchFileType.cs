namespace Confirmit.CATI.Common.Monitoring
{
    /// <summary>
    /// Represents the type of file, which is used for player starting.
    /// </summary>
    public enum LaunchFileType
    {        
        /// <summary>
        /// Live monitoring start file.
        /// </summary>
        LiveMonitoring = 0,

        /// <summary>
        /// Deferred monitoring start file.
        /// </summary>
        DeferredMonitoring = 1,

        /// <summary>
        /// Offline monitoring start file.
        /// </summary>
        OfflineMonitoring = 2
    }
}