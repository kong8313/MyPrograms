using System.Data.SqlClient;

namespace DeployScript.Interfaces
{
    public interface IQueryExecutor
    {
         void ExecuteNonQuery(string databaseName, string query, params SqlParameter[] sqlParameters);

         void ExecuteNonQuery(string databaseName, string query, bool writeToLog, params SqlParameter[] sqlParameters);
    }
}