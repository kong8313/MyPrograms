namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
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
        /// Set flag to regenerate sheduling script after next launch
        /// </summary>
        /// <param name="databaseName">Database name</param>
        void UpdateRegenerateIsRequiredFlag(string databaseName);

        /// <summary>
        /// Get all database names from server
        /// </summary>
        /// <returns></returns>
        string[] GetAllDatabaseNames();
    }
}