using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using Confirmit.CATI.Installation.Common.Interfaces;

namespace Confirmit.CATI.Installation.Common
{    
    public class ExternalInvoker : IExternalInvoker
    {
        private readonly ILogger _logger;
        private readonly int _successCode;

        public ExternalInvoker(ILogger logger)
            :this (logger, 0)
        {

        }

        public ExternalInvoker(ILogger logger, int successCode)
        {
            _logger = logger;
            _successCode = successCode;
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        public string Invoke(string scriptNameOrPath, string args)
        {
            return Invoke(scriptNameOrPath, args, null, -1, true);
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="delay">Delay for waiting</param>
        public string Invoke(string scriptNameOrPath, string args, int delay)
        {
            return Invoke(scriptNameOrPath, args, null, delay, true);
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        ///  <param name="isNeedToWait">Is need to wait until execution finish </param>
        public string Invoke(string scriptNameOrPath, string args, bool isNeedToWait)
        {
            return Invoke(scriptNameOrPath, args, null, -1, isNeedToWait);
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="tempFolderPath">Temp folder path</param>
        public string Invoke(string scriptNameOrPath, string args, string tempFolderPath)
        {
            return Invoke(scriptNameOrPath, args, tempFolderPath, -1, true);
        }

        /// <summary>
        /// Run external script
        /// </summary>
        /// <param name="scriptNameOrPath">The name or path of the external script</param>
        /// <param name="args">Thread arguments</param>
        /// <param name="tempFolderPath">Temp folder path</param>
        /// <param name="delay">Delay for waiting</param>
        /// <param name="isNeedToWait">Is need to wait until execution finish</param>
        /// <param name="doNotVerifyExitCode">Do not verify exit code</param>
        public string Invoke(string scriptNameOrPath, string args, string tempFolderPath, int delay, bool isNeedToWait, bool doNotVerifyExitCode = false)
        {
            _logger.WriteLog("Begin Invoke");

            _logger.WriteLog("Path={0}\r\nArguments={1}\r\ntempFolderPath={2}", scriptNameOrPath, args, tempFolderPath);
            using (var scriptProcess = new Process())
            {
                var pinfo = new ProcessStartInfo(scriptNameOrPath, args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                if (isNeedToWait)
                { 
                    pinfo.RedirectStandardOutput = true;
                    pinfo.RedirectStandardError = true;
                }

                if (!string.IsNullOrEmpty(tempFolderPath))
                {
                    CreateOrUpdateTheEnvironmentVariable(pinfo.EnvironmentVariables, "TEMP", tempFolderPath);
                    CreateOrUpdateTheEnvironmentVariable(pinfo.EnvironmentVariables, "TMP", tempFolderPath);
                }

                scriptProcess.StartInfo = pinfo;
                scriptProcess.Start();

                if (!isNeedToWait)
                {
                    return null;
                }

                var outputTask = new Task<string>(() => scriptProcess.StandardOutput.ReadToEnd());
                var errorTask = new Task<string>(() => scriptProcess.StandardError.ReadToEnd());

                outputTask.Start();
                errorTask.Start();

                _logger.WriteLog("Before WaitForExit");
                if (delay > 0)
                {
                    scriptProcess.WaitForExit(delay);
                }
                else
                {
                    scriptProcess.WaitForExit();
                }

                _logger.WriteLog("After WaitForExit");

                _logger.WriteLog("{0} return the exit code: {1}\r\nOutput information:\r\n{2}\r\nError information:\r\n{3}", scriptNameOrPath, scriptProcess.ExitCode, outputTask.Result, errorTask.Result);
                if (!doNotVerifyExitCode && _successCode != scriptProcess.ExitCode)
                {
                    throw new Exception(string.Format("Error executing {0}\r\nExit code {1} is wrong\r\nInternal error:\r\n{2}", 
                        scriptNameOrPath, 
                        scriptProcess.ExitCode,
                        string.IsNullOrEmpty(errorTask.Result) ? "No information" : errorTask.Result));
                }

                _logger.WriteLog("End Invoke");
                return outputTask.Result;
            }
        }

        private void CreateOrUpdateTheEnvironmentVariable(StringDictionary environmentVariables, string environmentVariable, string tempFolderPath)
        {
            if (environmentVariables.ContainsKey(environmentVariable))
            {
                environmentVariables[environmentVariable] = tempFolderPath;
            }
            else
            {
                environmentVariables.Add(environmentVariable, tempFolderPath);
            }
        }  
    }
}