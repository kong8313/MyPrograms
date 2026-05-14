using System;
using System.Data;
using System.IO;
using System.Text;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.Tools
{
    public class Comparer
    {
        private readonly string _testDeploymentDir;
        private readonly string _sqlPackageUtilityPath;
        private readonly IExternalInvoker _externalInvoker;
        private readonly IQueryExecutor _queryExecutor;

        public Comparer(string testDeploymentDir, string sqlPackageUtilityPath, IExternalInvoker externalInvoker, IQueryExecutor queryExecutor)
        {
            _testDeploymentDir = testDeploymentDir;
            _sqlPackageUtilityPath = sqlPackageUtilityPath;
            _externalInvoker = externalInvoker;
            _queryExecutor = queryExecutor;
        }

        public void CompareSchema(string dacPacFilePath, string scriptDatabaseName, IConfiguration configuration)
        {
            string resultSqlScriptFilePath = Path.Combine(_testDeploymentDir, "test.sql");
            // SqlPackage.exe /a:Script /tcs:"Data Source=localhost;User ID=sa;Password=firm;Database=ConfirmitCATIV15_xxxx;Encrypt=False" /sf:"c:\test.dacpac" /op:"c:\test.sql" /p:DropObjectsNotInSource=True  /v:master=master
            _externalInvoker.Invoke(_sqlPackageUtilityPath, string.Format(
                "/a:Script /tcs:\"Data Source={0};User ID={1};Password={2};Database={3};Encrypt=False\" /sf:\"{4}\" /op:\"{5}\" /p:DropObjectsNotInSource=True  /v:master=master",
                configuration.SqlServerName,
                configuration.SqlUserName,
                configuration.SqlPassword,
                scriptDatabaseName,
                dacPacFilePath,
                resultSqlScriptFilePath));

            var sqlScriptContent = new StringBuilder(File.ReadAllText(resultSqlScriptFilePath));

            RemoveVarChecks(sqlScriptContent);
            RemoveAlterAssemblys(sqlScriptContent);
            RemoveFulltextDisabling(sqlScriptContent);
            RemovePrintCommands(sqlScriptContent);
            RemoveBvSpGetVersionFunction(sqlScriptContent);
            RemoveRevokeViewCommand(sqlScriptContent);

            string resultString = RemoveGoCommandsAndEmptyLines(sqlScriptContent);

            Assert.AreEqual(string.Empty, resultString, "Database schema is different for two databases. SQL script contains some commands.");
        }

        private void RemoveRevokeViewCommand(StringBuilder sqlScriptContent)
        {
            RemoveScript(sqlScriptContent, "revoke view any column encryption key definition to public cascade");
            RemoveScript(sqlScriptContent, "revoke view any column master key definition to public cascade");
        }

        private string RemoveGoCommandsAndEmptyLines(StringBuilder sqlScriptContent)
        {
            string[] lines = sqlScriptContent.ToString().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var resultString = new StringBuilder();
            foreach (string str in lines)
            {
                if (!string.IsNullOrWhiteSpace(str) && str.ToLower() != "go")
                {
                    resultString.AppendLine(str);
                }
            }

            return resultString.ToString();
        }

        private void RemoveBvSpGetVersionFunction(StringBuilder sqlScriptContent)
        {
            RemoveScript(sqlScriptContent, " alter procedure [dbo].[bvspgetversion]");
        }

        private void RemovePrintCommands(StringBuilder sqlScriptContent)
        {
            RemoveScript(sqlScriptContent, "print");
        }

        private void RemoveFulltextDisabling(StringBuilder sqlScriptContent)
        {
            RemoveScript(sqlScriptContent, "if fulltextserviceproperty(n'isfulltextinstalled') = 1");
        }

        private void RemoveAlterAssemblys(StringBuilder sqlScriptContent)
        {
            RemoveScript(sqlScriptContent, "alter assembly");
        }

        private void RemoveScript(StringBuilder sqlScriptContent, string marker)
        {
            while (sqlScriptContent.ToString().ToLower().Contains("go\r\n" + marker))
            {
                int startIndex = sqlScriptContent.ToString().ToLower().IndexOf("go\r\n" + marker, StringComparison.Ordinal);
                int endIndex = sqlScriptContent.ToString().ToLower().IndexOf("go\r\n", startIndex + 2, StringComparison.Ordinal);
                sqlScriptContent.Remove(startIndex, endIndex - startIndex);
            }
        }

        private void RemoveVarChecks(StringBuilder sqlScriptContent)
        {
            const string marker = "if fulltextserviceproperty(n'isfulltextinstalled') = 1";
            int index = sqlScriptContent.ToString().ToLower().IndexOf(marker, StringComparison.Ordinal);
            sqlScriptContent.Remove(0, index - 4);
        }

        public void CompareData(string databaseName, string scriptDatabaseName)
        {
            using (var cnScope = new ConnectionScope(_queryExecutor.CreateConnectionString(databaseName)))
            {
                string query = "SELECT name FROM sys.Tables";
                var dtTables = _queryExecutor.ExecuteDataTable<DataTable>(databaseName, query);

                foreach (DataRow tableRow in dtTables.Rows)
                {
                    string tableName = tableRow[0].ToString();

                    if (tableName == "BvVersionHistory")
                    {
                        continue;
                    }

                    string queryDt = string.Format("SELECT * FROM [{0}].[dbo].[{1}]", databaseName, tableName);
                    string queryScriptDt = string.Format("SELECT * FROM [{0}].[dbo].[{1}]", scriptDatabaseName, tableName);

                    var dt = _queryExecutor.ExecuteDataTable<DataTable>(databaseName, queryDt);
                    var scriptDt = _queryExecutor.ExecuteDataTable<DataTable>(scriptDatabaseName, queryScriptDt);

                    Assert.AreEqual(dt.Rows.Count, scriptDt.Rows.Count, string.Format("Count of rows is different in '{0}' table", tableName));

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow row = dt.Rows[i];
                        DataRow scriptRow = scriptDt.Rows[i];

                        for (int j = 0; j < row.ItemArray.Length; j++)
                        {
                            // Skip 'CreateDate' and 'ModifyDate' and RegenerateIsRequired column from 'BvSchedule' table
                            // Skip 'Value' column from 'BvSystemSettings' table
                            if ((tableName == "BvSchedule" && (j == 5 || j == 6 || j == 7)) ||
                                (tableName == "BvSystemSettings" && j == 6))
                            {
                                continue;
                            }

                            Assert.AreEqual(row[j], scriptRow[j], string.Format("Found different values in '{0}' table", tableName));
                        }
                    }
                }
            }
        }        
    }
}
