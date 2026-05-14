using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary
{
    public class DatabaseUpdate
    {
        private readonly IDatabaseUpdateEngine _databaseUpdateEngine;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public DatabaseUpdate(IDatabaseUpdateEngine databaseUpdateEngine, ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _databaseUpdateEngine = databaseUpdateEngine;
            _configuration = configuration;
        }

        public void Validate(IValidator validator)
        {
            _logger.WriteLog(true, "Start validation");

            validator.CheckUpdateScripts();

            validator.CheckDatabases(_databaseUpdateEngine.DatabasesForUpgrade);

            _logger.WriteLog(true, "Validation has completed successfully");
        }

        /// <summary>
        /// Start database update process.
        /// Retrun values:
        /// 0 - success
        /// 1 - db update cannot be started
        /// 2 - db update failed (databases weren't restored)
        /// </summary>
        /// <returns></returns>
        public int Start()
        {
            _logger.WriteLog(true, "Start execution");

            int exitCode;
            bool startCommittingTransactions = false;

            string dbUpateUtilityVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();

            string activeUser = windowsIdentity != null ? windowsIdentity.Name : "Unknown user";
            string details;

            var swatch = new Stopwatch();
            swatch.Start();

            try
            {
                _logger.WriteLog(true, "Run update script verification (execution with rollback of transactions)");
                _databaseUpdateEngine.ApplyUpdates(dbUpateUtilityVersion, activeUser, false);

                startCommittingTransactions = true;
                _logger.WriteLog(true, "Run applying of update scripts");
                _databaseUpdateEngine.ApplyUpdates(dbUpateUtilityVersion, activeUser, true);

                _logger.WriteLog(true, "Execution has completed successfully");

                details = "Successful update. No errors occurred";
                exitCode = 0;
            }
            catch (Exception ex)
            {
                _logger.WriteLog(true, TraceEventType.Error, ex.Message);
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

                _logger.WriteLog(true, details);
            }

            swatch.Stop();

            new DatabaseUpdateFinishEvent(DateTime.UtcNow, -1, Environment.MachineName, activeUser, (int)swatch.ElapsedMilliseconds, details).Save(_configuration.ConfirmlogConnectionString);

            _databaseUpdateEngine.SaveUpdateScriptEvents();
           
            return exitCode;
        }

        public void Stop()
        {
            _databaseUpdateEngine.StopExecution();
        }
    }
}
