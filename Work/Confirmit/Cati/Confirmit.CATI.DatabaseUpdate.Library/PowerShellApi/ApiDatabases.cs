using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.PowerShellApi
{
    public class ApiDatabases
    {
        private readonly ApiConfiguration _configuration;

        public ApiDatabases(ApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ApiSql Cati
        {
            get
            {
                return new ApiSql(() => new ConnectionProvider(_configuration.Connection, _configuration.Transaction), _configuration.Connection.ConnectionString);
            }
        }

        public ApiSql Survey(string projectId)
        {
            var connectionString = _configuration.GetSurveyConnectionString(projectId);
            return new ApiSql(() => new RemoteConnectionProvider(connectionString), connectionString);
        }
    }

    public class ConnectionProvider : IConnectionProvider
    {
        public ConnectionProvider(SqlConnection connection, SqlTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
        }

        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }

        public void Dispose(){}
    }
}