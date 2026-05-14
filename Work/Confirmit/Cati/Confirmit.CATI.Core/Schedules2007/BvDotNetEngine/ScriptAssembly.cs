using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Microsoft.JScript;
using System.IO;
using System.Runtime.Remoting;

using BvDotNetScript;
using BvDotNetScript.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Threading;
using Confirmit.CATI.Core.Misc;

namespace BvDotNetEngine
{
    public class ScriptAssembly : IScriptAssembly
    {
        public bool CompileInDebugMode {
            get { return true;}
            set { }
        }

        public CompilerErrorCollection Compile(ScriptAssemblyFileInfo fileInfo, BvScheduleEntity schedule, ExecuteSchedulingScriptEvent evt)
        {
            if (String.IsNullOrEmpty(schedule.ScriptSource))
            {
                throw new InvalidOperationException("Scheduling script source has not been generated.");
            }

            //
            // We need avoid concurrent compilation for the same
            // scheduling script but in the same time different
            // scheduling scripts can be compiled concurrently.
            // So, we cannot use simple lock(...) here
            // as it will block compilation for all scheduling scripts.
            // Instead of lock(...) we're using Mutex
            // as it has name that we can customise for every scheduling script.
            //
            using (var scheduleCompilationLock = new Mutex(
                false,
                string.Format(
                    "Schedule {0} Lock Company Instance {1}",
                    schedule.ScheduleID,
                    BackendInstance.Current.CompanyId)))
            {
                evt.AddTiming("CreateMutex");

                scheduleCompilationLock.WaitOne();
                evt.AddTiming("WaitForMutex");

                try
                {
                    bool isAssemblyExists = File.Exists(fileInfo.AssemblyFilePath);
                    evt.AddTiming("CheckIsAssemblyExists2");

                    if (isAssemblyExists)
                    {
                        return new CompilerErrorCollection();
                    }

                    DnScript schedulingScript = ScheduleScriptExecutor.DeserializeScript(schedule.ScriptSource);
                    evt.AddTiming("DeserializeScript");

                    var compileErrors = Compile(fileInfo, schedulingScript);
                    evt.AddTiming("Compile");

                    return compileErrors;
                }
                finally
                {
                    scheduleCompilationLock.ReleaseMutex();
                    evt.AddTiming("ReleaseMutex");
                }
            }
        }

        public CompilerErrorCollection Compile(ScriptAssemblyFileInfo fileInfo, DnScript schedulingScript)
        {
            if (schedulingScript.SourceFiles.Count <= 0)
                throw new InvalidOperationException(
                    "Schedule contains empty scheduling script");

            var compilerParameters = new CompilerParameters();

            compilerParameters.GenerateExecutable = false;
            compilerParameters.GenerateInMemory = false;
            compilerParameters.OutputAssembly = fileInfo.AssemblyFilePath;
            if (CompileInDebugMode)
            {
                compilerParameters.CompilerOptions = "/debug+ /fast+";
                compilerParameters.IncludeDebugInformation = true;
                Directory.CreateDirectory(fileInfo.SourcesPath);
            }
            else
            {
                compilerParameters.CompilerOptions = "/fast+";
            }

            foreach (BvDotNetScript.DnReference refer in schedulingScript.References)
            {
                compilerParameters.ReferencedAssemblies.Add(ResolveReferencePath(refer.Path));
            }

            // fileNamesOrSources depends of IsDebugMode. For debug mode is files, otherwise sources
            List<string> fileNamesOrSources = new List<string>();
            if (CompileInDebugMode)
            {
                foreach (var file in schedulingScript.SourceFiles)
                {
                    string fileName = Path.Combine(fileInfo.SourcesPath, file.Name);
                    File.WriteAllText(fileName, file.Source);
                    fileNamesOrSources.Add(fileName);
                }
            }
            else
            {
                foreach (var file in schedulingScript.SourceFiles)
                {
                    fileNamesOrSources.Add(file.Source);
                }
            }

            CompilerResults result = null;
            using (var provider = new JScriptCodeProvider() )
            {
                if (CompileInDebugMode)
                {
                    result = provider.CompileAssemblyFromFile(
                            compilerParameters,
                            fileNamesOrSources.ToArray());
                }
                else
                {
                    result = provider.CompileAssemblyFromSource(
                            compilerParameters,
                            fileNamesOrSources.ToArray());
                }
            }


            if (result.NativeCompilerReturnValue != 0)
            {
                result.Errors.Add((new CompilerError(
                    "",
                    0,
                    0,
                    result.NativeCompilerReturnValue.ToString(),
                    "error execute compiler. exit code: " + result.NativeCompilerReturnValue.ToString())));
                return result.Errors;
            }

            return result.Errors;
        }

        private string ResolveReferencePath(string refFile )
        {
            return refFile.Replace("{BackEndPath}", Config.Inst.FusionPath.TrimEnd('\\')).
                           Replace("{DotNetFrameworkPath}", Config.Inst.DotNetFrameworkPath);
        }

        public void Execute(ScriptAssemblyFileInfo fileInfo, IEventSchedule bvEvent)
        {
            string entryPoint = "Interpreter.Initializer";

            ObjectHandle obj = AppDomain.CurrentDomain.CreateInstanceFrom(
                fileInfo.AssemblyFilePath,
                entryPoint);

            ISchedulingScript schedule = (ISchedulingScript)obj.Unwrap();

            schedule.Execute(bvEvent);
        }
    }
}
