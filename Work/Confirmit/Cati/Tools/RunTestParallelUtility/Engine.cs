using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class Engine : IEngine
    {
        private readonly ILogger _logger;
        private readonly IParametersParser _parametersParser;

        private const string TestConfirmlogBaseName = "test_confirmlog";

        public Engine(ILogger logger, IParametersParser parametersParser)
        {
            _logger = logger;
            _parametersParser = parametersParser;
        }

        /// <summary>
        /// Get Confirmlog connection string from registry
        /// </summary>
        /// <returns></returns>
        private string GetConfirmlogConnectionString()
        {
            string confirmlogServerName = Environment.MachineName;
            if (!string.IsNullOrEmpty(_parametersParser.SqlInstanceName))
            {
                confirmlogServerName = _parametersParser.SqlInstanceName;
            }

            var confirmlogDatabaseConnectionString = new SqlConnectionStringBuilder
            {
                DataSource = confirmlogServerName,
                InitialCatalog = "master",
                IntegratedSecurity = false,
                UserID = "sa",
                Password = "firm"
            };

            return confirmlogDatabaseConnectionString.ToString();
        }


        /// <summary>
        /// Drop all databases are started by "test_confirmlog"
        /// </summary>
        public void DropTestConfirmlogDatabases()
        {
            string connectionString = GetConfirmlogConnectionString();

            using (var cn = new SqlConnection(connectionString))
            {
                cn.Open();

                var sc = new ServerConnection(cn);

                var srv = new Server(sc);

                var databases = new Database[srv.Databases.Count];
                srv.Databases.CopyTo(databases, 0);
                foreach (Database database in databases)
                {
                    if (database.Name.StartsWith(TestConfirmlogBaseName))
                    {
                        try
                        {
                            database.Drop();
                        }
                        catch (Exception ex)
                        {
                            _logger.WriteLog("Can't drop the database " + database.Name + ".\r\nException: " + ex);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Get log file path from arguments line
        /// </summary>
        /// <param name="args">String with arguments</param>
        /// <param name="isOutput">true - return path to output log file, false - to error log file</param>
        /// <returns></returns>
        public string GetLogFilePathFromArgs(string args, bool isOutput)
        {
            string[] argsArr = args.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string arg in argsArr)
            {
                if (arg.Trim().StartsWith("resultsfile") || arg.Trim().StartsWith("publishresultsfile"))
                {
                    string[] resFileInfo = arg.Trim().Split(new[] { ':' }, 2);

                    string folderPath = Path.GetDirectoryName(resFileInfo[1].Trim(new[] { '"', ' ' }));
                    string fileName = Path.GetFileNameWithoutExtension(resFileInfo[1].Trim(new[] { '"', ' ' }));

                    if (isOutput)
                    {
                        return Path.Combine(folderPath ?? string.Empty, fileName + "_Output.txt");
                    }

                    return Path.Combine(folderPath ?? string.Empty, fileName + "_Error.txt");
                }
            }

            throw new Exception("Arguments for mstest don't contain 'resultsfile' information");
        }

        public int GetAllFailedTestCount(Dictionary<string, List<string>> failedTests)
        {
            int count = 0;
            foreach (var key in failedTests.Keys)
            {
                count += failedTests[key].Count;
            }

            return count;
        }

        public void SaveOutputLog(string args, Dictionary<string, StringBuilder> outputStrings)
        {
            string logFilePath = GetLogFilePathFromArgs(args, true);
            using (var sw = new StreamWriter(logFilePath, true))
            {
                sw.Write(outputStrings[args]);
            }
        }

        public string GetCmdLineForFailedTests(List<string> failedTests)
        {
            return "/test:" + string.Join(" /test:", failedTests.ToArray());
        }

        /// <summary>
        /// Get test name from output string
        /// </summary>
        /// <param name="argsData">Something like 'Failed Confirmit.CATI.IntegrationTests.Tests.FCDSpecificTests.FcdFilteringTests.ActivateCalls_ClassWithDifferentItses_CallsWithFilteredItsesAreDeleted'</param>
        /// <returns></returns>
        public string GetTestName(string argsData)
        {
            int firstIndex = argsData.IndexOf(" ");
            while (argsData[firstIndex++] == ' ') ;

            int secondIndex = argsData.IndexOf("\n");
            if (secondIndex == -1)
                secondIndex = argsData.Length;

            return argsData.Substring(firstIndex - 1, secondIndex - firstIndex + 1);
        }

        public void CreateOrUpdateTheEnvironmentVariable(StringDictionary environmentVariables, string environmentVariable, string value)
        {
            if (environmentVariables.ContainsKey(environmentVariable))
            {
                environmentVariables[environmentVariable] = value;
            }
            else
            {
                environmentVariables.Add(environmentVariable, value);
            }
        }

        public string GetTrxFilePath(string testResultDirectory, string additionalFileNamePart = "")
        {
            string trxFileName = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss") + additionalFileNamePart + ".trx";
            return Path.Combine(testResultDirectory, trxFileName);
        }

    }
}
