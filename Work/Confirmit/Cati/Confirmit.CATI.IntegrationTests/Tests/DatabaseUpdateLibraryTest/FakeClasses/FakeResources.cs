using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.IntegrationTests.Tests.DatabaseUpdateLibraryTest.FakeClasses
{
    public class FakeResources : IResources
    {
        public UpdateScriptInfo[] UpdateScriptInfos 
        {
            get
            {
                return FakeUpdateScriptInfos.ToArray();
            }
        }
        public List<UpdateScriptInfo> FakeUpdateScriptInfos { get; set; }

        public FakeResources()
        {
            FakeUpdateScriptInfos = new List<UpdateScriptInfo> 
            { 
                new UpdateScriptInfo("_2017-01-01_01_01_01", "Applied script", false)
                {
                    ScriptText = "select 1"
                }
            };
        }

        public void AddFakeScript(string name, bool isUnsafe)
        {
            FakeUpdateScriptInfos.Add(new UpdateScriptInfo(name, "Description for " + name, isUnsafe) { ScriptText = $"select '{name}'"});
        }

        public void AddFakePsScript(string name, bool isUnsafe)
        {
            FakeUpdateScriptInfos.Add(new UpdateScriptInfo(name, "Description for " + name, isUnsafe) { ScriptText = $"Write-Host '{name}'", Extension = "ps1" });
        }
    }
}