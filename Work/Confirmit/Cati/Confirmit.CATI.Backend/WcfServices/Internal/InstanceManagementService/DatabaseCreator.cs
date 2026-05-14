using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.DatabaseUpdateLibraryCore;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;
using Confirmit.Configuration;

namespace Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService
{
    public class DatabaseCreator
    {
        private readonly IServerSettings _serverSettings;

        public DatabaseCreator(IServerSettings serverSettings)
        {
            _serverSettings = serverSettings;
        }

        public string CreateCatiDatabaseForCompany(int companyId, string sqlServerConnectionString, string sqlDataPath, string sqlLogPath, bool isAzureSqlServer, string azureSqlServerEdition)
        {
            if (_serverSettings.CreateCompanyDatabasesFromBackup)
            {
                return BackendInstanceRegistrator.CreateDatabaseForInstance(companyId);
            }

            string dbName = MultimodeInstanceName.CompanyIdToDatabaseName(companyId);

            var connectionStringBuilder =
                new SqlConnectionStringBuilder(sqlServerConnectionString);

            IResources resources = new DatabaseUpdateLibraryCore.Resources();
            IConfiguration configuration =
                new DatabaseUpdateLibraryCore.Configuration(
                    connectionStringBuilder.DataSource,
                    connectionStringBuilder.UserID,
                    connectionStringBuilder.Password,
                    null,
                    null,
                    ConfirmitConfiguration.UseDefaultSqlServerPaths ? null : sqlDataPath,
                    ConfirmitConfiguration.UseDefaultSqlServerPaths ? null : sqlLogPath,
                    isAzureSqlServer ? azureSqlServerEdition : "");
            var consoleLogger = new ConsoleLogger();
            IQueryExecutor queryExecutor = new QueryExecutor(consoleLogger, configuration);
            IDatabaseWorker databaseWorker = new DatabaseWorker(consoleLogger, queryExecutor, configuration, resources);
            IUpdateScriptDatabaseWorker updateScriptDatabaseWorker = new UpdateScriptDatabaseWorker(consoleLogger, queryExecutor);
            IUpdateScriptsProvider updateScriptsProvider = new UpdateScriptsProvider(resources, updateScriptDatabaseWorker);
            IDatabaseUpdateEngine databaseUpdateEngine = new DatabaseUpdateEngine(consoleLogger, databaseWorker, configuration, updateScriptDatabaseWorker, updateScriptsProvider);

            var databaseUpdate = new DatabaseUpdate(databaseUpdateEngine, consoleLogger);
            databaseUpdate.CreateDatabaseForCompany(dbName);
            
            var scsb = new SqlConnectionStringBuilder(sqlServerConnectionString) {
                InitialCatalog = dbName
            };
            
            var dbEngine = new DatabaseEngine(scsb.ConnectionString);

            // We should clear BvSystemSettings because we should use default settings for new company\instance
            string query = string.Format(
                "ALTER DATABASE [{0}] SET READ_COMMITTED_SNAPSHOT ON WITH ROLLBACK IMMEDIATE\r\n" +
                "ALTER DATABASE [{0}] SET ALLOW_SNAPSHOT_ISOLATION ON\r\n" +
                "DELETE FROM [{0}].[dbo].[BvSystemSettings]\r\n" +
                "DELETE FROM [{0}].[dbo].[BvAppLocks]",
                dbName);
            dbEngine.ExecuteNonQuery(query, System.Data.CommandType.Text);

            return scsb.ConnectionString;
        }
    }
}