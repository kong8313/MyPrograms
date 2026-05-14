using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Confirmit.CATI.DatabaseUpdateLibraryCore.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibraryCore
{
    public class DatabaseUpdateEngine : IDatabaseUpdateEngine
    {
        private readonly ILogger _logger;

        private readonly IDatabaseWorker _databaseWorker;

        private readonly IConfiguration _configuration;

        private readonly IUpdateScriptDatabaseWorker _updateScriptDatabaseWorker;

        private readonly IUpdateScriptsProvider _updateScriptsProvider;
        
        private string _currentUpdatedDatabase;

        public string[] DatabasesForUpgrade { get; private set; }

        private bool _stopExecution;

        public DatabaseUpdateEngine(
            ILogger logger,
            IDatabaseWorker databaseWorker,
            IConfiguration configuration,
            IUpdateScriptDatabaseWorker updateScriptDatabaseWorker,
            IUpdateScriptsProvider updateScriptsProvider)
        {
            _logger = logger;
            _databaseWorker = databaseWorker;
            _configuration = configuration;
            _updateScriptDatabaseWorker = updateScriptDatabaseWorker;
            _updateScriptsProvider = updateScriptsProvider;

            FindDatabasesForUpgrade();
        }

        private void FindDatabasesForUpgrade()
        {
            using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString()))
            {
                string[] allDatabaseNames = _databaseWorker.GetAllDatabaseNames();
                var regEx = new Regex(_configuration.DatabaseNamePattern);

                DatabasesForUpgrade = allDatabaseNames.Where(databaseName => regEx.IsMatch(databaseName)).ToArray();
            }
        }

        /// <summary>
        /// Apply generated update files for all necessary databases
        /// </summary>
        public void ApplyUpdates(string dbUpdateUtilityVersion, bool commitTransaction)
        {
            _logger.WriteLog("Start applying updates");

            _stopExecution = false;

            var appliedScriptsCount = new Dictionary<string, int>();

            foreach (string database in DatabasesForUpgrade)
            {
                ApplyUpdatesForDatabase(dbUpdateUtilityVersion, commitTransaction, database, appliedScriptsCount);
            }

            LogInfoAfterUpdate(commitTransaction, appliedScriptsCount);
        }

        public void ApplyUpdatesForDatabase(string dbUpdateUtilityVersion, bool commitTransaction, string database,
            Dictionary<string, int> appliedScriptsCount)
        {
            List<UpdateScriptInfo> scriptsToExecute;

            using (var _ = new ConnectionScope(_databaseWorker.CreateConnectionString(database)))
            using (var txScope = new DatabaseTransactionScope("DatabaseUpdate", null))
            {
                _currentUpdatedDatabase = database;
                string actionInfo = commitTransaction ? "Apply" : "Verify";
                _logger.WriteLog("{0} update scripts for '{1}' database", actionInfo, _currentUpdatedDatabase);

                scriptsToExecute = commitTransaction
                    ? _updateScriptsProvider.GetScriptsToApply(_currentUpdatedDatabase)
                    : _updateScriptsProvider.GetScriptsToValidate(_currentUpdatedDatabase);

                ExecuteUpdateScripts(commitTransaction, scriptsToExecute, dbUpdateUtilityVersion);

                _databaseWorker.UpdateRegenerateIsRequiredFlag(_currentUpdatedDatabase);

                _logger.WriteLog($"An applying of update scripts for {_currentUpdatedDatabase} has ended successful");

                appliedScriptsCount.Add(_currentUpdatedDatabase, scriptsToExecute.Count);

                if (commitTransaction)
                {
                    _logger.WriteLog("Start commiting changes for '{0}' database", _currentUpdatedDatabase);
                    txScope.Commit();
                    _logger.WriteLog("Successful");
                }
            }

            if (commitTransaction && scriptsToExecute.Any(x => x.HasSqlScriptUnsafeType))
            {
                using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString(_currentUpdatedDatabase)))
                {
                    ShrinkDatabaseLog();
                }
            }
        }

        private void LogInfoAfterUpdate(bool commitTransaction, Dictionary<string, int> appliedScriptsCount)
        {
            if (!DatabasesForUpgrade.Any())
            {
                _logger.WriteLog("No CATI databases found \r\n");
                return;
            }
            
            if (commitTransaction)
            {
                int firstAppliedScriptNumber = appliedScriptsCount[DatabasesForUpgrade[0]];
                if (appliedScriptsCount.Any(x => x.Value != firstAppliedScriptNumber))
                {
                    _logger.WriteLog(TraceEventType.Warning, "WARNING: Update scripts have been applied successfully but count of applied scripts has to be the same for all databases but it is different.");
                }
                else
                {
                    if (firstAppliedScriptNumber > 0)
                    {
                        _logger.WriteLog("{0} update scripts have been applied successfully for each {1} database(s) \r\n", firstAppliedScriptNumber, DatabasesForUpgrade.Length);
                    }
                    else
                    {
                        _logger.WriteLog("All databases were in actual state. No update scripts were applied\r\n");
                    }
                }
            }
            else
            {
                _logger.WriteLog("All update scripts have been verified");
            }

            foreach (var key in appliedScriptsCount.Keys)
            {
                _logger.WriteLog("{0} - {1}", key, appliedScriptsCount[key]);
            }
        }

        private void ShrinkDatabaseLog()
        {
            _logger.WriteLog("At least one unsafe script was executed for database '{0}' so start shrink process for log file of this database", _currentUpdatedDatabase);

            string output = _databaseWorker.ExecuteSqlScript(
                @"DECLARE @Name NVARCHAR(MAX) = ( select top(1) name from sys.database_files WHERE Type = 1 )
                  EXEC( 'DBCC SHRINKFILE(' + @Name + ' )')  ", 
                _currentUpdatedDatabase);

            _logger.WriteLog("Query with shrink operation has finished successfully. Output:\r\n{0}", output);
        }

        private void ExecuteUpdateScripts(
            bool commitTransaction,
            List<UpdateScriptInfo> scriptsToExecute,
            string dbUpdateUtilityVersion)
        {
            foreach (UpdateScriptInfo updateScriptInfo in scriptsToExecute)
            {
                StopExecutionIfNeeded();

                var swatch = Stopwatch.StartNew();

                ExecutionResult result;

                switch (updateScriptInfo.Extension)
                {
                    case "sql":
                        result = ExecuteSqlScript(updateScriptInfo);
                        break;
                    case "ps1":
                        // Do nothing on kubernetes with power shell scripts
                        result = new ExecutionResult { Script = updateScriptInfo.ScriptText, Output = "THIS POWER SHELL SCRIPT WAS SKIPPED" };
                        break;
                    default:
                        throw new Exception($"Unknown script '{updateScriptInfo.Extension}' extension");
                }

                swatch.Stop();

                StopExecutionIfNeeded();

                if (commitTransaction)
                {
                    var newUpdateScriptInfo = new UpdateScriptInfo(
                        updateScriptInfo.Name, updateScriptInfo.Extension, updateScriptInfo.Description, updateScriptInfo.HasSqlScriptUnsafeType, DateTime.Now, (int)swatch.ElapsedMilliseconds, result.Script, result.Output, _configuration.IsDbCreation, dbUpdateUtilityVersion);
                    _updateScriptDatabaseWorker.AddAppliedUpdateScriptInfo(_currentUpdatedDatabase, newUpdateScriptInfo);
                }
            }
        }

        class ExecutionResult
        {
            public string Script;
            public string Output;
        }

        private ExecutionResult ExecuteSqlScript(UpdateScriptInfo updateScriptInfo)
        {
            var scriptText = updateScriptInfo.ScriptText;

            _logger.WriteLog("Execute '{0}' {1}update script", updateScriptInfo.Name,
                updateScriptInfo.HasSqlScriptUnsafeType ? "UNSAFE " : "");
            var scriptOutput = _databaseWorker.ExecuteSqlScript(scriptText, _currentUpdatedDatabase);

            return new ExecutionResult { Script = scriptText, Output = scriptOutput };
        }

        private void StopExecutionIfNeeded()
        {
            if (_stopExecution)
            {
                _currentUpdatedDatabase = null;
                throw new Exception("Update is stopping by user");
            }
        }

        public void StopExecution()
        {
            _logger.WriteLog(TraceEventType.Warning, "Stop execution because a user has pressed 'Cancel' button");

            _stopExecution = true;

            Thread.Sleep(1000);

            if (string.IsNullOrEmpty(_currentUpdatedDatabase))
            {
                _logger.WriteLog(TraceEventType.Warning, "Cannot kill execution for 'current' database because it isn't defined");
                return;
            }

            try
            {
                using (new ConnectionScope(_databaseWorker.CreateConnectionString()))
                {
                    _logger.WriteLog("Before KillProcesses");
                    _databaseWorker.KillProcesses(_currentUpdatedDatabase);
                    _logger.WriteLog("The execution of sql script has been broken.");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(TraceEventType.Error, "Error during the breaking of processes: " + ex.Message);
            }
        }

        public void CreateDefaultCatiDatabaseIfNeeded()
        {
            if (!_databaseWorker.IsDatabaseExists(_configuration.DefaultDatabaseName))
            {
                _databaseWorker.ConfigureSqlServer();
                
                _databaseWorker.CreateDatabase(_configuration.DefaultDatabaseName);
                _configuration.IsDbCreation = true;
                FindDatabasesForUpgrade();
            }
        }

        public void PopulateWithInitialSchemaIfNeeded(string databaseName = null)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                databaseName = _configuration.DefaultDatabaseName;
            }

            _databaseWorker.PopulateWithInitialSchemaIfNeeded(databaseName);
        }

        public void ApplyUpdateScriptToNewCompany(string databaseName)
        {
            _databaseWorker.ApplyNewCompanyUpdates(databaseName);
        }

        public bool CreateCatiDatabaseForCompanyIfNeeded(string databaseName)
        {
            if (!_databaseWorker.IsDatabaseExists(databaseName))
            {
                _databaseWorker.CreateDatabase(databaseName);
                _configuration.IsDbCreation = true;
                return true;
            }
            return false;
        }
        
        public void OverrideSystemSettingsForContainerEnv()
        {
            _databaseWorker.OverrideSystemSettingsForContainerEnv();
        }
    }
}