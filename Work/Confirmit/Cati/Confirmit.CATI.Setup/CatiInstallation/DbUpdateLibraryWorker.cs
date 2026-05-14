using BootstrapperLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace CatiInstallation
{
    public class DbUpdateLibraryWorker
    {
        private readonly ILogger _logger;
        private readonly string _sqlServerName;
        private readonly string _sqlUserName;
        private readonly string _sqlPassword;
        private readonly string _confirmlogConnectionString;
        private readonly string _confirmitLinkedServerName;

        private DatabaseUpdate _databaseUpdate;

        public DbUpdateLibraryWorker(ILogger logger, string sqlServerName, string sqlUserName, string sqlPassword, 
            string confirmlogConnectionString, string confirmitLinkedServerName)
        {
            _logger = logger;
            _sqlServerName = sqlServerName;
            _sqlUserName = sqlUserName;
            _sqlPassword = sqlPassword;
            _confirmlogConnectionString = confirmlogConnectionString;
            _confirmitLinkedServerName = confirmitLinkedServerName;
        }

        public int UpdateDatabases(bool isDbCreation)
        {
            IResources resources = new Resources();
            IConfiguration configuration = new Configuration(
                _sqlServerName,
                _sqlUserName,
                _sqlPassword,
                _confirmlogConnectionString,
                BootstrapperEngine.GetCurrentVersion(),
                isDbCreation);
            IQueryExecutor queryExecutor = new QueryExecutor(_logger, configuration);
            IDatabaseWorker databaseWorker = new DatabaseWorker(_logger, queryExecutor, configuration);
            IUpdateScriptDatabaseWorker updateScriptDatabaseWorker = new UpdateScriptDatabaseWorker(_logger, queryExecutor);
            IUpdateScriptsProvider updateScriptProvider = new UpdateScriptsProvider(resources, updateScriptDatabaseWorker);
            IPowerShellScriptExecutor powerShellScriptExecutor = new PowerShellScriptExecutor(configuration);
            IDatabaseUpdateEngine databaseUpdateEngine = new DatabaseUpdateEngine(_logger, databaseWorker, configuration, updateScriptDatabaseWorker, updateScriptProvider, powerShellScriptExecutor);

            _databaseUpdate = new DatabaseUpdate(databaseUpdateEngine, _logger, configuration);

            return _databaseUpdate.Start();
        }
    }
}