using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Utilities;

using StaticTeamCityBuildEngine.Interfaces;

namespace StaticTeamCityBuildEngine.CommonEngines
{
    public class ExternalExecutor : IExternalExecutor
    {
        private readonly TaskLoggingHelper _logger;

        public ExternalExecutor(TaskLoggingHelper logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="delay">Max delay (millisecond)</param>
        public void Invoke(string scriptNameOrPath, string args, int delay = -1)
        {
            _logger.LogMessage("Begin Invoke");

            try
            {
                _logger.LogMessage("Path={0}\r\nArguments={1}", scriptNameOrPath, args);
                using (var scriptProcess = new Process())
                {
                    var pinfo = new ProcessStartInfo(scriptNameOrPath, args)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                    scriptProcess.StartInfo = pinfo;
                    scriptProcess.Start();

                    if (delay > 0)
                    {
                        _logger.LogMessage("Before WaitForExit({0})", delay);
                        scriptProcess.WaitForExit(delay);
                        _logger.LogMessage("After WaitForExit({0})", delay);
                    }
                    else
                    {
                        _logger.LogMessage("Before WaitForExit");
                        scriptProcess.WaitForExit();
                        _logger.LogMessage("After WaitForExit");
                    }

                    _logger.LogMessage("{0} return the exit code: {1}", scriptNameOrPath, scriptProcess.ExitCode);
                    if (scriptProcess.ExitCode != 0)
                    {
                        throw new Exception(string.Format(
                            "Error executing {0}\r\nExit code {1} is wrong.",
                            scriptNameOrPath,
                            scriptProcess.ExitCode));
                    }
                }
            }
            finally
            {
                _logger.LogMessage("End Invoke");
            }
        }
        
        private string GetPathToGit()
        {
            string pathsVariable = Environment.GetEnvironmentVariable("Path") ?? string.Empty;

            string[] paths = pathsVariable.Split(';');
            string gitCmdPath = paths.FirstOrDefault(x => x.Contains(@"\Git\")); // something like this: C:\Program Files (x86)\Git\cmd

            if (string.IsNullOrEmpty(gitCmdPath))
            {
                throw new Exception("Git path was not found among Path environment variables");
            }

            return Path.Combine(Directory.GetParent(gitCmdPath).FullName, @"bin\git.exe");
        }

        public string ExecuteGitUtility(string command)
        {
            string gitPath = GetPathToGit();

            if (!File.Exists(gitPath))
            {
                throw new Exception("File " + gitPath + " doesn't exist");
            }

            _logger.LogMessage("ExecuteGitUtility:\r\ngitPath={0}\r\ncommand={1}", gitPath, command);

            var scriptProcess = new Process();

            var pinfo = new ProcessStartInfo(gitPath, command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            scriptProcess.StartInfo = pinfo;
            scriptProcess.Start();
            scriptProcess.WaitForExit();

            string errorStr = scriptProcess.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(errorStr))
            {
                throw new Exception("An error occured during work with git.exe utility: " + errorStr);
            }

            return scriptProcess.StandardOutput.ReadToEnd();
        }
    }
}
