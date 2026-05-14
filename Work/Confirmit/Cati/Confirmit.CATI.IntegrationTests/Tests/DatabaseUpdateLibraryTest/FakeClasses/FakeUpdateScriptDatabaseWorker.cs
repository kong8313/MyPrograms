using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses
{
    public class FakeUpdateScriptDatabaseWorker : IUpdateScriptDatabaseWorker
    {
        public int AddAppliedUpdateScriptInfoCount { get; set; }

        public List<UpdateScriptInfo> ReturnValueGetAppliedUpdateScriptInfos { get; set; }

        public FakeUpdateScriptDatabaseWorker()
        {
            ReturnValueGetAppliedUpdateScriptInfos = new List<UpdateScriptInfo>();

            AddAppliedUpdateScriptInfoCount = 0;
        }

        public UpdateScriptInfo[] GetAppliedUpdateScriptInfos(string databaseName)
        {
            return ReturnValueGetAppliedUpdateScriptInfos.ToArray();
        }

        public void AddAppliedUpdateScriptInfo(string databaseName, UpdateScriptInfo updateScriptInfo)
        {
            AddAppliedUpdateScriptInfoCount++;
        }
    }
}