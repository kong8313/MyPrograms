using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses
{
    public class FakeDatabaseWorker : DatabaseWorker
    {
        public int ExecutedSqlScriptsCnt { get; set; }
        public List<string> ExecutedSqlScripts { get; set; }
        public int UpdateRegenerateIsRequiredFlagCnt { get; set; }

        public FakeDatabaseWorker(ILogger logger, IQueryExecutor queryExecutor, IConfiguration configuration) 
            : base(logger, queryExecutor, configuration)
        {
            ExecutedSqlScriptsCnt = 0;
            ExecutedSqlScripts = new List<string>();
        }

        public override string ExecuteSqlScript(string sqlQuery, string databaseName)
        {
            ExecutedSqlScriptsCnt++;
            ExecutedSqlScripts.Add(sqlQuery);
            return "Fake ExecuteSqlScript was executed with " + sqlQuery;
        }
        
        public override void UpdateRegenerateIsRequiredFlag(string databaseName)
        {
            UpdateRegenerateIsRequiredFlagCnt++;
        }
    }
}