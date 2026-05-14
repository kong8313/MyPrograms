namespace Confirmit.CATI.Core.ActivityLogging
{
    /// <summary>
    /// Provides the common methods for all activity log events.
    /// </summary>
    public interface IActivityEvent
    {
        /// <summary>
        /// Finishes the activity event and stops the event duration counter.
        /// </summary>
        void Finish();

        /// <summary>
        /// Saves the activity event details to DB.
        /// </summary>
        void Save();

        /// <summary>
        /// Determines whether this event is currently measuring the duration.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </returns>
        bool IsRunning();
    }
}