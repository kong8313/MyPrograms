namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
{
    public interface IDatabaseUpdateEngine
    {
        void SaveUpdateScriptEvents();

        /// <summary>
        /// Get list of CATI databases
        /// </summary>
        /// <returns></returns>
        string[] DatabasesForUpgrade { get; }

        /// <summary>
        /// Apply update scripts for all CATI databases
        /// </summary>
        void ApplyUpdates(string dbUpateUtilityVersion, string activeUser, bool commitTransaction);

        void StopExecution();
    }
}