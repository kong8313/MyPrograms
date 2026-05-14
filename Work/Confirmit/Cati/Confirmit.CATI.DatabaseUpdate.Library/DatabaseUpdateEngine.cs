using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using ILogger = Confirmit.CATI.Installation.Common.Interfaces.ILogger;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class DatabaseUpdateEngine : IDatabaseUpdateEngine
    {
        private readonly ILogger _logger;

        private readonly IDatabaseWorker _databaseWorker;

        private readonly IPowerShellScriptExecutor _powerShellScriptExecutor;

        private readonly IConfiguration _configuration;

        private readonly IUpdateScriptDatabaseWorker _updateScriptDatabaseWorker;

        private readonly IUpdateScriptsProvider _updateScriptsProvider;
        
        private string _currentUpdatedDatabase;
        public string[] DatabasesForUpgrade { get; private set; }

        private readonly List<DatabaseUpdateScriptApplyingEvent> _appliedUpdateScriptEvents;

        private bool _stopExecution;
        
        public DatabaseUpdateEngine(
            ILogger logger,
            IDatabaseWorker databaseWorker,
            IConfiguration configuration,
            IUpdateScriptDatabaseWorker updateScriptDatabaseWorker,
            IUpdateScriptsProvider updateScriptsProvider,
            IPowerShellScriptExecutor powerShellScriptExecutor)
        {
            _logger = logger;
            _databaseWorker = databaseWorker;
            _configuration = configuration;
            _updateScriptDatabaseWorker = updateScriptDatabaseWorker;
            _updateScriptsProvider = updateScriptsProvider;
            _powerShellScriptExecutor = powerShellScriptExecutor;

            _appliedUpdateScriptEvents = new List<DatabaseUpdateScriptApplyingEvent>();
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
        public void ApplyUpdates(string dbUpateUtilityVersion, string activeUser, bool commitTransaction)
        {
            _logger.WriteLog("Start applying updates");

            _stopExecution = false;

            var appliedScriptsCount = new Dictionary<string, int>();

            foreach (var databaseForUpgrade in DatabasesForUpgrade)
            {
                var applyedUpdateScriptEventsForCurrentDatabase = new List<DatabaseUpdateScriptApplyingEvent>();

                List<UpdateScriptInfo> scriptsToExecute;
                using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString(databaseForUpgrade)))
                using (var txScope = new DatabaseTransactionScope("DatabaseUpdate", null))
                {
                    _currentUpdatedDatabase = databaseForUpgrade;
                    string actionInfo = commitTransaction ? "Apply" : "Verify";
                    _logger.WriteLog(true, "{0} update scripts for '{1}' database", actionInfo, _currentUpdatedDatabase);

                    scriptsToExecute = commitTransaction 
                        ? _updateScriptsProvider.GetScriptsToApply(_currentUpdatedDatabase)
                        : _updateScriptsProvider.GetScriptsToValidate(_currentUpdatedDatabase);

                    ExecuteUpdateScripts(commitTransaction, scriptsToExecute, applyedUpdateScriptEventsForCurrentDatabase, activeUser, dbUpateUtilityVersion);

                    _databaseWorker.UpdateRegenerateIsRequiredFlag(_currentUpdatedDatabase);

                    _logger.WriteLog($"An applying of update scripts for {_currentUpdatedDatabase} has ended successful");

                    appliedScriptsCount.Add(_currentUpdatedDatabase, scriptsToExecute.Count);

                    if (commitTransaction)
                    {
                        _logger.WriteLog("Start commiting changes for '{0}' database", _currentUpdatedDatabase);
                        txScope.Commit();
                        _logger.WriteLog("Successful");

                        _appliedUpdateScriptEvents.AddRange(applyedUpdateScriptEventsForCurrentDatabase);
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

            LogInfoAfterUpdate(commitTransaction, appliedScriptsCount);
        }

        private void LogInfoAfterUpdate(bool commitTransaction, Dictionary<string, int> appliedScriptsCount)
        {
            if (commitTransaction)
            {
                int firstAppliedScriptNumber = appliedScriptsCount[DatabasesForUpgrade[0]];
                if (appliedScriptsCount.Any(x => x.Value != firstAppliedScriptNumber))
                {
                    _logger.WriteLog(true, TraceEventType.Warning, "WARNING: Update scripts have been applied successfully but count of applied scripts has to be the same for all databases but it is different.");
                }
                else
                {
                    if (firstAppliedScriptNumber > 0)
                    {
                        _logger.WriteLog(true, "{0} update scripts have been applied successfully for each {1} database(s) \r\n", firstAppliedScriptNumber, DatabasesForUpgrade.Length);
                    }
                    else
                    {
                        _logger.WriteLog(true, "All databases were in actual state. No update scripts were applied\r\n");
                    }
                }
            }
            else
            {
                _logger.WriteLog(true, "All update scripts have been verified");
            }

            foreach (var key in appliedScriptsCount.Keys)
            {
                _logger.WriteLog(true, "{0} - {1}", key, appliedScriptsCount[key]);
            }
        }

        private void ShrinkDatabaseLog()
        {
            _logger.WriteLog(true, "At least one unsafe script was executed for database '{0}' so start shrink process for log file of this database", _currentUpdatedDatabase);

            string output = _databaseWorker.ExecuteSqlScript(
                @"DECLARE @Name NVARCHAR(MAX) = ( select top(1) name from sys.database_files WHERE Type = 1 )
                  EXEC( 'DBCC SHRINKFILE(' + @Name + ' )')  ", 
                _currentUpdatedDatabase);

            _logger.WriteLog(true, "Query with shrink operation has finished successfully. Output:\r\n{0}", output);
        }

        private void ExecuteUpdateScripts(
            bool commitTransaction,
            List<UpdateScriptInfo> scriptsToExecute,
            List<DatabaseUpdateScriptApplyingEvent> applyedUpdateScriptEventsForCurrentDatabase,
            string activeUser,
            string dbUpateUtilityVersion)
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
                        result = ExecutePsScript(updateScriptInfo);
                        break;
                    default:
                        throw new Exception($"Unknown script '{updateScriptInfo.Extension}' extension");
                }

                swatch.Stop();

                StopExecutionIfNeeded();

                if (commitTransaction)
                {
                    var newUpdateScriptInfo = new UpdateScriptInfo(
                        updateScriptInfo.Name, updateScriptInfo.Extension, updateScriptInfo.Description, updateScriptInfo.HasSqlScriptUnsafeType, DateTime.Now, (int)swatch.ElapsedMilliseconds, result.Script, result.Output, _configuration.IsDBCreation, dbUpateUtilityVersion, activeUser);
                    _updateScriptDatabaseWorker.AddAppliedUpdateScriptInfo(_currentUpdatedDatabase, newUpdateScriptInfo);

                    var newEvent = new DatabaseUpdateScriptApplyingEvent(
                        DateTime.UtcNow, GetNumberFromDatabaseName(_currentUpdatedDatabase), Environment.MachineName, activeUser, (int)swatch.ElapsedMilliseconds, newUpdateScriptInfo.ToDatabaseUpdateScriptApplyingParameters());
                    applyedUpdateScriptEventsForCurrentDatabase.Add(newEvent);
                }
            }
        }

        class ExecutionResult
        {
            public string Script;
            public string Output;
        }

        private ExecutionResult ExecutePsScript(UpdateScriptInfo updateScriptInfo)
        {
            _logger.WriteLog(true, "Execute '{0}' powershell script", updateScriptInfo.Name);

            var output = _powerShellScriptExecutor.Execute(_logger, updateScriptInfo.ScriptText);

            return new ExecutionResult { Script = updateScriptInfo.ScriptText, Output = output };
        }

        private ExecutionResult ExecuteSqlScript(UpdateScriptInfo updateScriptInfo)
        {
            _logger.WriteLog(true, "Execute '{0}' {1}update script", updateScriptInfo.Name,
                updateScriptInfo.HasSqlScriptUnsafeType ? "UNSAFE " : "");
            var scriptOutput = _databaseWorker.ExecuteSqlScript(updateScriptInfo.ScriptText, _currentUpdatedDatabase);
            
            return new ExecutionResult { Script = updateScriptInfo.ScriptText, Output = scriptOutput };
        }

        public void SaveUpdateScriptEvents()
        {
            _logger.WriteLog("Start saving of update script events");

            foreach (var applyedUpdateScriptEvent in _appliedUpdateScriptEvents)
            {
                applyedUpdateScriptEvent.Save(_configuration.ConfirmlogConnectionString);
            }

            _logger.WriteLog("All update script events have been saved");
        }

        private void StopExecutionIfNeeded()
        {
            if (_stopExecution)
            {
                _currentUpdatedDatabase = null;
                throw new Exception("Update is stopping by user");
            }
        }

        private int GetNumberFromDatabaseName(string databaseName)
        {
            string[] databaseNameParts = databaseName.Split('_');
            if (databaseNameParts.Length == 1)
            {
                return 0;
            }

            if (!int.TryParse(databaseNameParts[1], out var number))
            {
                throw new Exception("Database name '{databaseName}' has unknown format. It should be like 'ConfirmitCATIV15_xxx'");
            }

            return number;
        }

        public void StopExecution()
        {
            _logger.WriteLog(true, TraceEventType.Warning, "Stop execution because a user has pressed 'Cancel' button");

            _stopExecution = true;

            Thread.Sleep(1000);

            if (string.IsNullOrEmpty(_currentUpdatedDatabase))
            {
                _logger.WriteLog(TraceEventType.Warning, "Cannot kill execution for 'current' database because it isn't defined");
                return;
            }

            try
            {
                using (var cnScope = new ConnectionScope(_databaseWorker.CreateConnectionString()))
                {
                    _logger.WriteLog("Before KillProcesses");
                    _databaseWorker.KillProcesses(_currentUpdatedDatabase);
                    _logger.WriteLog(true, "The execution of sql script has been broken.");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLog(true, TraceEventType.Error, "Error during the breaking of processes: " + ex.Message);
            }
        }
    }
}