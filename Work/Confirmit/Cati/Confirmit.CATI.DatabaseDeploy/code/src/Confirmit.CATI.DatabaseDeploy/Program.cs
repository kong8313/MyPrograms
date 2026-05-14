using System;
using System.Diagnostics;
using Confirmit.CATI.DatabaseUpdateLibraryCore;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;
using Confirmit.Configuration;
using Microsoft.Extensions.Configuration;

namespace Confirmit.CATI.DatabaseDeploy
{
    class Program
    {
        static int Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            int exitCode = 0;

            try
            {
                var systemConfiguration = new ConfigurationBuilder()
                    .AddEnvironmentVariables()
                    .AddKeyPerFile("/etc/confirmit/secrets", true)
                    .AddCommandLine(args)
                    .Build();

                new ConfigurationLoader().LoadConfiguration();

                var resultCode = UpdateAllDatabasesOnServer(systemConfiguration, logger,
                    ConfigSettings.CatiSqlServerName,
                    systemConfiguration["Confirmit:Database:Server:Cati:DataPath"],
                    systemConfiguration["Confirmit:Database:Server:Cati:LogPath"],
                    true);
                exitCode = Math.Max(exitCode, resultCode);

                foreach (var serverData in ConfirmitConfiguration.CatiServers)
                {
                    var server = serverData.Value;

                    if (server.SqlServerName == ConfigSettings.CatiSqlServerName)
                        continue;

                    resultCode = UpdateAllDatabasesOnServer(systemConfiguration, logger, server.SqlServerName, server.SqlServerDataPath, server.SqlServerLogPath, false);
                    exitCode = Math.Max(exitCode, resultCode);
                }
            }
            catch (Exception ex)
            {
                logger.WriteLog(TraceEventType.Error, ex.ToString());
                exitCode = Math.Max(exitCode, 1);
            }

            return exitCode;
        }

        private static int UpdateAllDatabasesOnServer(IConfigurationRoot systemConfiguration, ILogger logger, string serverName, string sqlDataPath, string sqlLogPath, bool defaultCatiServer)
        {
            IResources resources = new Resources();
            DatabaseUpdateLibraryCore.Interfaces.IConfiguration configuration = new DatabaseUpdateLibraryCore.Configuration(
                serverName,
                systemConfiguration["Confirmit:Database:User:DeployC:Name"],
                systemConfiguration["Confirmit:Database:User:DeployC:Password"],
                systemConfiguration["Confirmit:Database:User:SystemAdmin:Name"],
                systemConfiguration["Confirmit:Database:User:SystemAdmin:Password"],
                ConfirmitConfiguration.UseDefaultSqlServerPaths ? null : sqlDataPath,
                ConfirmitConfiguration.UseDefaultSqlServerPaths ? null : sqlLogPath);
            IQueryExecutor queryExecutor = new QueryExecutor(logger, configuration);
            IDatabaseWorker databaseWorker = new DatabaseWorker(logger, queryExecutor, configuration, resources);
            IUpdateScriptDatabaseWorker updateScriptDatabaseWorker = new UpdateScriptDatabaseWorker(logger, queryExecutor);
            IUpdateScriptsProvider updateScriptsProvider = new UpdateScriptsProvider(resources, updateScriptDatabaseWorker);
            IDatabaseUpdateEngine databaseUpdateEngine = new DatabaseUpdateEngine(logger, databaseWorker, configuration, updateScriptDatabaseWorker, updateScriptsProvider);
            IValidator validator = new Validator(resources, databaseWorker, configuration);

            var databaseUpdate = new DatabaseUpdate(databaseUpdateEngine, logger);
            databaseUpdate.Validate(validator);
            return databaseUpdate.Start(defaultCatiServer);
        }
    }
}