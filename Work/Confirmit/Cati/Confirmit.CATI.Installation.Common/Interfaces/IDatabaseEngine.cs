using System.Data;
using System.Data.SqlClient;

namespace Confirmit.CATI.Installation.Common.Interfaces
{
    public interface IDatabaseEngine
    {
        string ServerName { get; }
        string Login { get; }
        string Password { get; }

        void ValidateConnection(string databaseName);

        void ExecuteNonQuery(string databaseName, string commandText, params SqlParameter[] parameters);
        T ExecuteScalar<T>(string commandText, params SqlParameter[] parameters);
        T ExecuteScalar<T>(string databaseName, string commandText, params SqlParameter[] parameters);
        T ExecuteDataTable<T>(string databaseName, string query) where T : DataTable, new();
    }
}
