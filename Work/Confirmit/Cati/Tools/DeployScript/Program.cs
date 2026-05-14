using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using DeployScript.Interfaces;

namespace DeployScript
{
    public class Program
    {
        /// <summary>
        /// This is an utility to join pre-deployment, deployment and post-deployment scripts to one deployment script
        /// This utility also removes unused commands from the deployment script
        /// </summary>
        public static void Main(string[] args)
        {
            string currentFilePath = (new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath.Replace("%20", " ");
            ILogger logger = new FileAndConsoleLogger(currentFilePath + ".log");
            string sqlInstanceName = LocalEnvironment.GetLocalSqlInstanceName();

            IQueryExecutor queryExecutor = new QueryExecutor(logger, sqlInstanceName, "sa", "firm");
            try
            {
                new Program().Start(logger, queryExecutor, args, currentFilePath);
            }
            catch (Exception ex)
            {
                logger.WriteLog(true, TraceEventType.Error, ex.Message);
                logger.WriteLog(TraceEventType.Error, ex.ToString());
            }

            logger.WriteLog(true, "Execution has finished. See {0} log file for details", currentFilePath + ".log");
        }

        private void Start(ILogger logger, IQueryExecutor queryExecutor, string[] args, string currentFilePath)
        {
            var resultScript = GenerateSqlScript(logger, currentFilePath);            

            if (args.Length > 0 && args[0] == "/deploy")
            {
                logger.WriteLog("Start deploying script to ConfirmitCATIV15_BUILD database");

                string query = string.Format("IF EXISTS(SELECT name FROM sys.databases WHERE name='{0}') DROP DATABASE {0}; CREATE DATABASE {0}; ALTER DATABASE {0} SET TRUSTWORTHY ON", LocalEnvironment.CatiBuildDatabaseName);
                queryExecutor.ExecuteNonQuery("master", query);

                ExecuteSqlScript(logger, queryExecutor, resultScript, LocalEnvironment.CatiBuildDatabaseName);

                logger.WriteLog("Deploying has finished successfully");
            }            
        }

        private string GenerateSqlScript(ILogger logger, string currentFilePath)
        {
            logger.WriteLog("Start generation Confirmit.CATI.Database.sql scripts");

            string databasePath =Path.GetFullPath(Path.Combine(Path.GetDirectoryName(currentFilePath) ?? string.Empty, "..\\..\\Confirmit.CATI.Database.2012"));
            string resultScriptPath = Path.Combine(databasePath, @"DeployScript\Confirmit.CATI.Database.sql");
            string preDeployScriptPath = Path.Combine(databasePath, @"Confirmit.CATI.Database\sql\Scripts\Pre-Deployment\Script.PreDeployment.sql");
            string deployScriptPath = Path.Combine(databasePath, @"Confirmit.CATI.Database\sql\Confirmit.CATI.Database_Create.sql");
            string postDeployScriptPath = Path.Combine(databasePath, @"Confirmit.CATI.Database\sql\Scripts\Post-Deployment\Script.PostDeployment.sql");

            logger.WriteLog("databasePath=" + databasePath);
            logger.WriteLog("resultScriptPath=" + resultScriptPath);
            logger.WriteLog("preDeployScriptPath=" + preDeployScriptPath);
            logger.WriteLog("deployScriptPath=" + deployScriptPath);
            logger.WriteLog("postDeployScriptPath=" + postDeployScriptPath);

            var resultScript = new StringBuilder();
            resultScript.Append(File.ReadAllText(preDeployScriptPath));

            string deployScript = File.ReadAllText(deployScriptPath);

            int lineNumber = deployScript.LastIndexOf("ALTER DATABASE [$(DatabaseName)]", StringComparison.Ordinal);
            deployScript = deployScript.Substring(0, lineNumber);
            
            lineNumber = deployScript.LastIndexOf("ALTER DATABASE [$(DatabaseName)]", StringComparison.Ordinal);
            deployScript = deployScript.Substring(lineNumber);
            
            lineNumber = deployScript.IndexOf("PRINT N'Creating ", StringComparison.Ordinal);
            deployScript = deployScript.Substring(lineNumber);
            
            resultScript.Append(deployScript);

            resultScript.Append(File.ReadAllText(postDeployScriptPath));

            File.WriteAllText(resultScriptPath, resultScript.ToString());

            logger.WriteLog("Generation of Confirmit.CATI.Database.sql scripts has finished successfully");

            return resultScript.ToString();
        }

        public void ExecuteSqlScript(ILogger logger, IQueryExecutor queryExecutor, string sqlQuery, string databaseName)
        {
            logger.WriteLog("Execute script for database {0}", databaseName);

            string[] queries = sqlQuery.Split(new[] { "\r\nGO\r\n", "\r\ngo\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string query in queries)
            {
                if (string.IsNullOrEmpty(query.Trim(new[] { '\r', '\n' })))
                {
                    continue;
                }

                queryExecutor.ExecuteNonQuery(databaseName, query);                
            }

            logger.WriteLog("Script has executed successful");
        }
    }
}
