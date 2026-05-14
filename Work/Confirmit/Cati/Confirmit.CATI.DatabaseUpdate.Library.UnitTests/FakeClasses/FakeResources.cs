using System.Collections.Generic;
using Confirmit.CATI.DatabaseUpdateLibrary.Interfaces;

namespace Confirmit.CATI.DatabaseUpdateLibrary.UnitTests.FakeClasses
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
                new UpdateScriptInfo("_17_00_Test_00", "Description for _17_00_Test_00", false){ScriptText = "select _17_00_Test_00"}, 
                new UpdateScriptInfo("_17_00_Test_01", "Description for _17_00_Test_01", false){ScriptText = "select _17_00_Test_01"}, 
                new UpdateScriptInfo("_17_00_Test_02", "Description for _17_00_Test_02", false){ScriptText = "select _17_00_Test_02"}
            };
        }
    }
}