﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace DatabaseUpdateUtility
{
    class Program
    {
        static void Main()
        {
            ILogger logger = new FileAndConsoleLogger(Path.Combine(Application.StartupPath, "DatabaseUpdateUtility.log"));
            try
            {
                IResources resources = new Resources();
                IConfiguration configuration = new Configuration(
                    Properties.Resources.Default.SqlServerName,
                    Properties.Resources.Default.SqlUserName,
                    Properties.Resources.Default.SqlPassword,
                    Properties.Resources.Default.ConfirmLogConnectionString,
                    Assembly.GetExecutingAssembly().GetName().Version,
                    Properties.Resources.Default.IsDBCreation);
                IQueryExecutor queryExecutor = new QueryExecutor(logger, configuration);
                IDatabaseWorker databaseWorker = new DatabaseWorker(logger, queryExecutor, configuration);
                IUpdateScriptDatabaseWorker updateScriptDatabaseWorker = new UpdateScriptDatabaseWorker(logger, queryExecutor);
                IUpdateScriptsProvider updateScriptsProvider = new UpdateScriptsProvider(resources, updateScriptDatabaseWorker);
                IPowerShellScriptExecutor powerShellScriptExecutor = new PowerShellScriptExecutor(configuration);
                IDatabaseUpdateEngine databaseUpdateEngine = new DatabaseUpdateEngine(logger, databaseWorker, configuration, 
                    updateScriptDatabaseWorker, updateScriptsProvider, powerShellScriptExecutor);
                IValidator validator = new Validator(resources, databaseWorker, configuration);

                var databaseUpdate = new DatabaseUpdate(databaseUpdateEngine, logger, configuration);

                databaseUpdate.Validate(validator);

                databaseUpdate.Start();
            }
            catch (Exception ex)
            {
                logger.WriteLog(true, TraceEventType.Error, ex.Message);
                logger.WriteLog(TraceEventType.Error, ex.ToString());
            }
        }
    }
}