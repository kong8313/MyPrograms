using System.IO;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace BvDotNetEngine
{
    public class ScriptAssemblyFileInfo
    {
        public string BaseScriptName { get; private set; }
        public string SourcesPath { get; private set; }
        public string AssemblyFileName { get; private set; }
        public string AssemblyFilePath { get; private set; }

        public ScriptAssemblyFileInfo(string baseScriptName)
        {
            Init(baseScriptName);
        }

        public ScriptAssemblyFileInfo(BvScheduleEntity schedule) 
        {
            var baseScriptName = schedule.ScheduleID.ToString() + " " +
                                 schedule.ModifyDate.Year.ToString() + "." +
                                 schedule.ModifyDate.Month.ToString() + "." +
                                 schedule.ModifyDate.Day.ToString() + " " +
                                 schedule.ModifyDate.Hour.ToString() + "." +
                                 schedule.ModifyDate.Minute.ToString() + "." +
                                 schedule.ModifyDate.Second.ToString() + "." +
                                 schedule.ModifyDate.Millisecond;

            Init(baseScriptName);
        }

        private void Init(string baseScriptName)
        {
            BaseScriptName = baseScriptName;

            SourcesPath = Path.Combine(
                Config.Inst.BaseScriptPath,
                BaseScriptName);

            AssemblyFileName = BaseScriptName + ".dll";

            AssemblyFilePath = Path.Combine(Config.Inst.BaseScriptPath, AssemblyFileName);
        }

        
    }
}