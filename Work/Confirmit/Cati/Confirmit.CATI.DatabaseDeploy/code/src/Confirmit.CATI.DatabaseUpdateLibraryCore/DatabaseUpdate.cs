using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class DatabaseUpdate
    {
        private readonly IDatabaseUpdateEngine _databaseUpdateEngine;
        private readonly ILogger _logger;

        public DatabaseUpdate(IDatabaseUpdateEngine databaseUpdateEngine, ILogger logger)
        {
            _logger = logger;
            _databaseUpdateEngine = databaseUpdateEngine;
        }

        public void Validate(IValidator validator)
        {
            _logger.WriteLog("Start validation");

            validator.CheckUpdateScripts();

            validator.CheckDatabases(_databaseUpdateEngine.DatabasesForUpgrade);

            _logger.WriteLog("Validation has completed successfully");
        }

        /// <summary>
        /// Start database update process.
        /// Return values:
        /// 0 - success
        /// 1 - db update cannot be started
        /// 2 - db update failed (databases weren't restored)
        /// </summary>
        /// <returns></returns>
        public int Start(bool defaultCatiServer = true)
        {
            _logger.WriteLog("Start execution");

            int exitCode;
            bool startCommittingTransactions = false;

            string dbUpdateUtilityVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            string details;

            try
            {
                if (defaultCatiServer)
                {
                    _databaseUpdateEngine.CreateDefaultCatiDatabaseIfNeeded();

                    _databaseUpdateEngine.PopulateWithInitialSchemaIfNeeded();
                }

                _logger.WriteLog("Run update script verification (execution with rollback of transactions)");
                _databaseUpdateEngine.ApplyUpdates(dbUpdateUtilityVersion, false);

                // It looks logical not to run scripts again if no scripts were executed on the previous stage
                // but there is a special type of scripts which has to be executed only once (during execution stage with commiting)
                // so it is difficult to understand should we run execution with commiting or not
                startCommittingTransactions = true;
                _logger.WriteLog("Run applying of update scripts");
                _databaseUpdateEngine.ApplyUpdates(dbUpdateUtilityVersion, true);

                if (defaultCatiServer)
                {
                    _databaseUpdateEngine.OverrideSystemSettingsForContainerEnv();
                }

                _logger.WriteLog("Execution has completed successfully");

                details = "Successful update. No errors occurred";
                exitCode = 0;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(TraceEventType.Error, ex.ToString());

                if (startCommittingTransactions)
                {
                    exitCode = 2;
                    details = "Update fail. Database restoring are not supported, so databases are in an indefinite state. See other messages for details";
                }
                else
                {
                    exitCode = 1;
                    details = "An error occured during verification of possibility to apply update scripts. Databases are in the initial state. See other messages for details";
                }
            }

            _logger.WriteLog(details);

            return exitCode;
        }

        public void CreateDatabaseForCompany(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(databaseName));

            string dbUpdateUtilityVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

            var isNewDatabase = _databaseUpdateEngine.CreateCatiDatabaseForCompanyIfNeeded(databaseName);

            _databaseUpdateEngine.PopulateWithInitialSchemaIfNeeded(databaseName);

            _databaseUpdateEngine.ApplyUpdatesForDatabase(dbUpdateUtilityVersion, true, databaseName, new Dictionary<string, int>());

            if (isNewDatabase)
            {
                _databaseUpdateEngine.ApplyUpdateScriptToNewCompany(databaseName);
            }
        }

        public void Stop()
        {
            _databaseUpdateEngine.StopExecution();
        }
    }
}
