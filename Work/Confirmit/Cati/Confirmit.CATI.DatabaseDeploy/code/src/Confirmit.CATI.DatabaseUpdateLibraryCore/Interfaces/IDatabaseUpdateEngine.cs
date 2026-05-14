using System.Collections.Generic;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IDatabaseUpdateEngine
    {
        /// <summary>
        /// Get list of CATI databases
        /// </summary>
        /// <returns></returns>
        string[] DatabasesForUpgrade { get; }

        /// <summary>
        /// Apply update scripts for all CATI databases
        /// </summary>
        void ApplyUpdates(string dbUpdateUtilityVersion, bool commitTransaction);

        void StopExecution();

        void CreateDefaultCatiDatabaseIfNeeded();
        
        void OverrideSystemSettingsForContainerEnv();
        
        bool CreateCatiDatabaseForCompanyIfNeeded(string databaseName);

        void PopulateWithInitialSchemaIfNeeded(string databaseName = null); 

        void ApplyUpdateScriptToNewCompany(string databaseName);

        void ApplyUpdatesForDatabase(string dbUpdateUtilityVersion, bool commitTransaction, string database,
            Dictionary<string, int> appliedScriptsCount);
    }
}