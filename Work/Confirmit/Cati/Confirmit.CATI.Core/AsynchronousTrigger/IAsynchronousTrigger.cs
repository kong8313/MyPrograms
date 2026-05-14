using Confirmit.CATI.Core.AsynchronousTrigger.Messages;

namespace Confirmit.CATI.Core.AsynchronousTrigger
{
    public interface IAsynchronousTrigger
    {
        /// <summary>
        /// Gets subscriber name. Used in different logging operations.
        /// </summary>
        string TrigerName { get; }

        /// <summary>
        /// Gets a table name subscriber created for.
        /// Must be an existing table name.
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// Called once only during process initialization.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called once only during process de initialization.
        /// </summary>
        void Uninitialize();

        /// <summary>
        /// Called when message received.
        /// </summary>
        /// <param name="triggerMessage">Message to process</param>
        void OnTableChanged(TriggerMessage triggerMessage);
    }
}
