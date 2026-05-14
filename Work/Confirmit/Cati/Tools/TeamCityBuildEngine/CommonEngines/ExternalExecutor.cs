using System;
using System.Diagnostics;
using TeamCityBuildEngine.Interfaces;

namespace TeamCityBuildEngine.CommonEngines
{
    public class ExternalExecutor : IExternalExecutor
    {
        private readonly ILogger _logger;

        private int _exitCode;
        public int ExitCode { get { return _exitCode; } }

        public ExternalExecutor(ILogger logger)
        {
            _logger = logger;
            _exitCode = 2;
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="delay">Max delay (millisecond)</param>
        public void Invoke(string scriptNameOrPath, string args, int delay = -1)
        {
            _logger.WriteLog("Begin Invoke");

            try
            {
                _logger.WriteLog("Path={0}\r\nArguments={1}", scriptNameOrPath, args);
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
                        _logger.WriteLog("Before WaitForExit({0})", delay);
                        scriptProcess.WaitForExit(delay);
                        _logger.WriteLog("After WaitForExit({0})", delay);
                    }
                    else
                    {
                        _logger.WriteLog("Before WaitForExit");
                        scriptProcess.WaitForExit();
                        _logger.WriteLog("After WaitForExit");
                    }

                    _exitCode = scriptProcess.ExitCode;
                    _logger.WriteLog("{0} return the exit code: {1}", scriptNameOrPath, scriptProcess.ExitCode);
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
                _logger.WriteLog("End Invoke");
            }
        }
    }
}
