namespace Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces
{
    public interface IDatabaseWorker
    {
        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        string CreateConnectionString(string databaseName = "master");

        /// <summary>
        /// Kill all process for selected database
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        bool KillProcesses(string databaseName);

        /// <summary>
        /// Return true, if database exists, otherwise - false
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        bool IsDatabaseExists(string databaseName);

        /// <summary>
        /// Get user access for selected database
        /// </summary>
        /// <param name="databaseName">Database name</param>
        /// <returns></returns>
        DatabaseUserAccess GetUserAccess(string databaseName);

        /// <summary>
        /// Execute sql script from update script file on selected database
        /// </summary>
        /// <param name="sqlQuery">SQL query text</param>
        /// <param name="databaseName">Database name</param>
        string ExecuteSqlScript(string sqlQuery, string databaseName);
        
        /// <summary>
        /// Set flag to regenerate scheduling script after next launch
        /// </summary>
        /// <param name="databaseName">Database name</param>
        void UpdateRegenerateIsRequiredFlag(string databaseName);

        /// <summary>
        /// Get all database names from server
        /// </summary>
        /// <returns></returns>
        string[] GetAllDatabaseNames();

        /// <summary>
        /// Create database if there is no database with the specified name
        /// </summary>
        /// <param name="databaseName">Database name</param>
        void CreateDatabase(string databaseName);

        /// <summary>
        /// Apply base initial script to the specified database if the database is empty (there is no table)
        /// </summary>
        /// <param name="databaseName">Database name</param>
        void PopulateWithInitialSchemaIfNeeded(string databaseName);

        void ApplyNewCompanyUpdates(string databaseName);

        void OverrideSystemSettingsForContainerEnv();
        
        void ConfigureSqlServer();
    }
}