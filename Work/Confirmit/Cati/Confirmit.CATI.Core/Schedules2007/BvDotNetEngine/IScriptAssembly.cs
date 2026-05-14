using System.CodeDom.Compiler;
using BvDotNetScript;
using BvDotNetScript.Interfaces;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace BvDotNetEngine
{
    public interface IScriptAssembly
    {
        CompilerErrorCollection Compile(
            ScriptAssemblyFileInfo fileInfo, 
            BvScheduleEntity schedule, 
            ExecuteSchedulingScriptEvent evt);

        CompilerErrorCollection Compile(
            ScriptAssemblyFileInfo fileInfo, 
            DnScript schedulingScript);

        void Execute(ScriptAssemblyFileInfo fileInfo, IEventSchedule bvEvent);
    }
}