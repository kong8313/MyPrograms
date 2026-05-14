using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.DatabaseUpdateLibrary.Interfaces
{
    public interface IQueryExecutor
    {
        string CreateConnectionString(string databaseName = "master");

        string OutputOfLastExecution { get; }

        void ExecuteNonQuery(string databaseName, string query, params SqlParameter[] sqlParameters);

        void ExecuteNonQuery(string databaseName, string query, bool writeToLog, params SqlParameter[] sqlParameters);

        T ExecuteScalar<T>(string databaseName, string query);

        T ExecuteScalar<T>(string databaseName, string query, bool writeToLog);

        T ExecuteDataTable<T>(string databaseName, string query) where T : DataTable, new();
    }
}