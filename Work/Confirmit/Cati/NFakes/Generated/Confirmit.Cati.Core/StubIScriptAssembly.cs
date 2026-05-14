using System;
using BvDotNetEngine;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using System.CodeDom.Compiler;
using BvDotNetScript;
using BvDotNetScript.Interfaces;

namespace BvDotNetEngine.Fakes
{
    public class StubIScriptAssembly : IScriptAssembly 
    {
        private IScriptAssembly _inner;

        public StubIScriptAssembly()
        {
            _inner = null;
        }

        public IScriptAssembly Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate CompilerErrorCollection CompileScriptAssemblyFileInfoBvScheduleEntityExecuteSchedulingScriptEventDelegate(ScriptAssemblyFileInfo fileInfo, BvScheduleEntity schedule, ExecuteSchedulingScriptEvent evt);
        public CompileScriptAssemblyFileInfoBvScheduleEntityExecuteSchedulingScriptEventDelegate CompileScriptAssemblyFileInfoBvScheduleEntityExecuteSchedulingScriptEvent;

        CompilerErrorCollection IScriptAssembly.Compile(ScriptAssemblyFileInfo fileInfo, BvScheduleEntity schedule, ExecuteSchedulingScriptEvent evt)
        {


            if (CompileScriptAssemblyFileInfoBvScheduleEntityExecuteSchedulingScriptEvent != null)
            {
                return CompileScriptAssemblyFileInfoBvScheduleEntityExecuteSchedulingScriptEvent(fileInfo, schedule, evt);
            } else if (_inner != null)
            {
                return ((IScriptAssembly)_inner).Compile(fileInfo, schedule, evt);
            }

            return default(CompilerErrorCollection);
        }

        public delegate CompilerErrorCollection CompileScriptAssemblyFileInfoDnScriptDelegate(ScriptAssemblyFileInfo fileInfo, DnScript schedulingScript);
        public CompileScriptAssemblyFileInfoDnScriptDelegate CompileScriptAssemblyFileInfoDnScript;

        CompilerErrorCollection IScriptAssembly.Compile(ScriptAssemblyFileInfo fileInfo, DnScript schedulingScript)
        {


            if (CompileScriptAssemblyFileInfoDnScript != null)
            {
                return CompileScriptAssemblyFileInfoDnScript(fileInfo, schedulingScript);
            } else if (_inner != null)
            {
                return ((IScriptAssembly)_inner).Compile(fileInfo, schedulingScript);
            }

            return default(CompilerErrorCollection);
        }

        public delegate void ExecuteScriptAssemblyFileInfoIEventScheduleDelegate(ScriptAssemblyFileInfo fileInfo, IEventSchedule bvEvent);
        public ExecuteScriptAssemblyFileInfoIEventScheduleDelegate ExecuteScriptAssemblyFileInfoIEventSchedule;

        void IScriptAssembly.Execute(ScriptAssemblyFileInfo fileInfo, IEventSchedule bvEvent)
        {

            if (ExecuteScriptAssemblyFileInfoIEventSchedule != null)
            {
                ExecuteScriptAssemblyFileInfoIEventSchedule(fileInfo, bvEvent);
            } else if (_inner != null)
            {
                ((IScriptAssembly)_inner).Execute(fileInfo, bvEvent);
            }
        }

    }
}