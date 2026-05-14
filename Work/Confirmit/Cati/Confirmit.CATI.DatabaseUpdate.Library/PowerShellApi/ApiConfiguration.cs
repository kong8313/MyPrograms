using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.PowerShellApi
{
    public class ApiConfiguration
    {
        private readonly IConfiguration _configuration;
        public SqlConnection Connection { get; }
        public SqlTransaction Transaction { get; }

        public ApiConfiguration(IConfiguration configuration, SqlConnection connection, SqlTransaction transaction)
        {
            _configuration = configuration;
            Connection = connection;
            Transaction = transaction;
        }

        public string GetSurveyConnectionString(string projectId)
        {
            var confirmitDatabaseProvider = new ConfirmitDatabaseProvider();

            var dbName = confirmitDatabaseProvider.GetSurveyDatabaseName(projectId);
            var dataSource = confirmitDatabaseProvider.GetSqlServerName(projectId);

            var catiConnectionString = new SqlConnectionStringBuilder(Connection.ConnectionString);
            var surveyDbConnectionString = new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                InitialCatalog = dbName,
                UserID = _configuration.SqlUserName,
                Password = _configuration.SqlPassword,
                MaxPoolSize = catiConnectionString.MaxPoolSize
            };

            return surveyDbConnectionString.ToString();
        }

    }
}