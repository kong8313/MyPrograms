using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.ServiceRegistration;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace DatabaseMigrationUtility
{
    public class Migrator
    {
        public ILogger Logger { get; private set; }
        private IDatabaseWorker _databaseWorker;
        private IConfiguration _configuration;
        private IPowerShellScriptExecutor _powerShellScriptExecutor;


        public void Initialize()
        {
            var serviceLocator = new ServiceLocator();
            serviceLocator.Initialize();

            IServicesRegistryInitializer serviceRegistryInitializer = new ServicesRegistryInitializer(serviceLocator);

            serviceRegistryInitializer.RegisterRegistries(serviceRegistryInitializer.GetRegistries());

            var connectionStrings = ServiceLocator.Resolve<IConnectionStrings>();
            var csb = new SqlConnectionStringBuilder(connectionStrings.MasterConnectionString);

            Logger = new FileAndConsoleLogger(Path.Combine(Environment.CurrentDirectory,
                $"DatabaseMigrationUtility {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss")}.log"));

            _configuration = new Configuration(
                csb.DataSource,
                csb.UserID,
                csb.Password,
                connectionStrings.ConfirmlogConnectionString,
                Assembly.GetExecutingAssembly().GetName().Version,
                false);
            IQueryExecutor queryExecutor = new QueryExecutor(Logger, _configuration);
            _databaseWorker = new DatabaseWorker(Logger, queryExecutor, _configuration);
            _powerShellScriptExecutor = new PowerShellScriptExecutor(_configuration);
        }

        public void ExecuteScript(MigratorOptions options, UpdateScriptInfo scriptInfo)
        {
            var databaseNames = GetDatabaseNames(options);

            if (databaseNames.Length == 0)
            {
                Logger.WriteLog(true, "There is no any databases on server to update");
                return;
            }

            foreach (var databaseName in databaseNames)
            {
                Logger.WriteLog(true, $"Updating {databaseName} database...");

                using (new ConnectionScope(_databaseWorker.CreateConnectionString(databaseName)))
                {
                    switch (scriptInfo.Extension)
                    {
                        case "sql":
                            _databaseWorker.ExecuteSqlScript(scriptInfo.ScriptText, databaseName);
                            break;
                        case "ps1":
                            _powerShellScriptExecutor.Execute(Logger, scriptInfo.ScriptText);
                            break;
                        default:
                            throw new Exception($"Unknown script '{scriptInfo.Extension}' extension");
                    }
                }

                Logger.WriteLog(true, $"Database {databaseName} was updated...");
            }
        }

        private string[] GetDatabaseNames(MigratorOptions options)
        {
            using (new ConnectionScope(_databaseWorker.CreateConnectionString()))
            {
                string[] allDatabaseNames = _databaseWorker.GetAllDatabaseNames();
                var regEx = new Regex(_configuration.DatabaseNamePattern);

                return allDatabaseNames
                        .Where(databaseName => regEx.IsMatch(databaseName))
                        .Where( name => options.All || name.ToUpper() == $"CONFIRMITCATIV15_{options.CompanyId}" )
                        .ToArray();
            }
        }
    }
}
