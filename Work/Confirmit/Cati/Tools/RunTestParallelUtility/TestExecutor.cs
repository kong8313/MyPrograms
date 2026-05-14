using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Confirmit.CATI.Installation.Common.Interfaces;
using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class TestExecutor : ITestExecutor
    {
        protected readonly ILogger Logger;
        private readonly IEngine _engine;
        private readonly ITestResultFileEngine _testResultFileEngine;

        private readonly Dictionary<string, StringBuilder> _outputStrings = new Dictionary<string, StringBuilder>();
        private readonly Dictionary<string, StringBuilder> _errorStrings = new Dictionary<string, StringBuilder>();

        private readonly Dictionary<string, List<string>> _failedTests = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, bool> _passedTestsExistence = new Dictionary<string, bool>();
        private readonly Dictionary<string, bool> _summaryExistence = new Dictionary<string, bool>();

        private readonly object _lock = new object();        

        protected string MsTestPath;

        private readonly string _sqlInstanceName;

        public TestExecutor(ILogger logger, IEngine engine, IPathProvider pathProvider, ITestResultFileEngine testResultFileEngine, string sqlInstanceName)
        {
            Logger = logger;
            _engine = engine;
            _testResultFileEngine = testResultFileEngine;

            MsTestPath = pathProvider.GetPathToMsTest();

            var executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

            logger.WriteLog("MSTest location: " + MsTestPath);
            logger.WriteLog("Self location: " + executablePath);

            _sqlInstanceName = sqlInstanceName;
        }        

        private void ScriptProcessOutputDataReceived(object sender, DataReceivedEventArgs args)
        {
            lock (_lock)
            {
                string argsData = args.Data;
                if (string.IsNullOrEmpty(argsData))
                {
                    return;
                }

                Console.WriteLine(argsData);
                _outputStrings[((Process)sender).StartInfo.Arguments].Append(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.ms") + ": " + argsData + "\r\n");

                if (argsData.Length > 9 && (argsData.StartsWith("Failed   ") || argsData.StartsWith("Error   ")))
                {
                    _failedTests[((Process)sender).StartInfo.Arguments].Add(_engine.GetTestName(argsData));
                }
                else if (argsData.Length > 9 && argsData.StartsWith("Passed   "))
                {
                    _passedTestsExistence[((Process)sender).StartInfo.Arguments] = true;
                }
                else if (argsData == "Summary")
                {
                    _summaryExistence[((Process)sender).StartInfo.Arguments] = true;
                }
            }
        }

        private void ScriptProcessErrorDataReceived(object sender, DataReceivedEventArgs args)
        {
            lock (_lock)
            {
                string argsData = args.Data;
                if (string.IsNullOrEmpty(argsData) || argsData.Trim('\r', '\n', ' ').Length == 0)
                {
                    return;
                }

                if (!string.IsNullOrEmpty(argsData))
                {
                    Console.WriteLine("MSTest error:\r\n" + argsData);
                    _errorStrings[((Process)sender).StartInfo.Arguments].Append(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.ms") + ": " + argsData + "\r\n");
                }
            }
        }

        /// <summary>
        /// Invoke script with MsTest
        /// </summary>
        /// <param name="path">Path to program</param>
        /// <param name="args">Program arguments</param>
        /// <returns></returns>
        private Process InvokeProcess(string path, string args)
        {
            Logger.WriteLog("Path: " + path + "\r\nArguments: " + args);

            // 8191 is max length of a string of arguments. See https://support.microsoft.com/en-us/kb/830473
            // Lets check with stockpile
            if (args.Length > 8150)
            {
                throw new Exception("A string of arguments is overlong. Increase \"threadcount\" parameter to decrease it.");
            }

            var scriptProcess = new Process();

            InitializeOutputs(args);

            var pInfo = new ProcessStartInfo(path, args)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            _engine.CreateOrUpdateTheEnvironmentVariable(pInfo.EnvironmentVariables, "CATI_SQL_INSTANCE_NAME", _sqlInstanceName);
            _engine.CreateOrUpdateTheEnvironmentVariable(pInfo.EnvironmentVariables, "CONFIRMIT_SQL_INSTANCE_NAME", _sqlInstanceName);
            _engine.CreateOrUpdateTheEnvironmentVariable(pInfo.EnvironmentVariables, "ISRUNNINGINPARALLELMODE", "1");
            
            scriptProcess.StartInfo = pInfo;

            scriptProcess.OutputDataReceived += ScriptProcessOutputDataReceived;
            scriptProcess.ErrorDataReceived += ScriptProcessErrorDataReceived;
            
            scriptProcess.Start();

            scriptProcess.BeginOutputReadLine();
            scriptProcess.BeginErrorReadLine();

            scriptProcess.WaitForExit();

            return scriptProcess;
        }

        private void InitializeOutputs(string args)
        {
            _outputStrings.Add(args, new StringBuilder());
            _errorStrings.Add(args, new StringBuilder());
            _passedTestsExistence.Add(args, false);
            _summaryExistence.Add(args, false);
            _failedTests.Add(args, new List<string>());
        }

        /// <summary>
        /// Run tests
        /// </summary>
        /// <param name="msTestParameterString">String with argument for MSTest</param>
        /// <param name="testResultDirectory">Test result directory</param>
        /// <param name="testsToRun">Command line with tests classes for executing for one process</param>
        /// <param name="threadNumber">Number of executed thread</param>
        public void RunTests(string msTestParameterString, string testResultDirectory, string testsToRun, int threadNumber)
        {
            int repeatCnt = 0;
            string trxFilePath = string.Empty;
            string msTestArgs = string.Empty;
            bool processFinishedByException;

            // Repeat 3 times if there were no executed tests, no summary or invoking process failed with exception
            do
            {
                Logger.WriteLog($"Run process #{threadNumber}. Try #{repeatCnt + 1}");
                
                if (repeatCnt > 0)
                {
                    if (File.Exists(trxFilePath))
                    {
                        File.Delete(trxFilePath);
                    }

                    if (_failedTests.ContainsKey(msTestArgs))
                    {
                        _failedTests.Remove(msTestArgs);
                    }

                    Thread.Sleep(60000);
                }

                repeatCnt++;

                trxFilePath = _engine.GetTrxFilePath(testResultDirectory, "_" + repeatCnt);
                msTestArgs = $"{msTestParameterString} /resultsfile:\"{trxFilePath}\" {testsToRun}";
                
                try
                {
                    processFinishedByException = false;
                    InvokeProcess(MsTestPath, msTestArgs);
                }
                catch (Exception ex)
                {
                    processFinishedByException = true;
                    Logger.WriteLog(TraceEventType.Error, $"An error occurred during MsTest execution. Error:\r\n{ex}.");
                }
                finally
                {
                    _engine.SaveOutputLog(msTestArgs, _outputStrings);
                }
            } while (((!_passedTestsExistence[msTestArgs] && _failedTests[msTestArgs].Count == 0) ||
                      !_summaryExistence[msTestArgs] || processFinishedByException) && repeatCnt < 3);

            if ((!_passedTestsExistence[msTestArgs] && _failedTests[msTestArgs].Count == 0) ||
                 !_summaryExistence[msTestArgs] || processFinishedByException)
            {
                throw new Exception($"All attempts to run tests for thread #{threadNumber} has finished unsuccessfully");
            }

            var failedTestsCount = _failedTests[msTestArgs].Count;
            // Run failed tests again if in this thread is not too many failed tests
            if (failedTestsCount > 0 && failedTestsCount < 4)
            {
                _testResultFileEngine.RemoveFailedTestInfo(_failedTests[msTestArgs], trxFilePath);

                string repeatTrxFilePath = _engine.GetTrxFilePath(testResultDirectory, "_failed");

                string failedMsTestArgs = $"{msTestParameterString} /resultsfile:\"{repeatTrxFilePath}\" {_engine.GetCmdLineForFailedTests(_failedTests[msTestArgs])}";
                _failedTests.Remove(msTestArgs);

                try
                {
                    InvokeProcess(MsTestPath, failedMsTestArgs);
                }
                finally
                {
                    _engine.SaveOutputLog(failedMsTestArgs, _outputStrings);
                }
            }
        }

        /// <summary>
        /// Save error log files and generate exit code
        /// </summary>
        /// <returns>
        /// </returns>
        public int SaveErrorLogsAndCreateExitCode()
        {
            int countErrors = 0;
            foreach (string args in _errorStrings.Keys)
            {
                if (_errorStrings[args].Length == 0)
                {
                    continue;
                }

                string logFilePath = _engine.GetLogFilePathFromArgs(args, false);
                using (var sw = new StreamWriter(logFilePath, true))
                {
                    sw.Write(_errorStrings[args]);
                }

                countErrors++;
            }

            if (countErrors > 0)
            {
                Logger.WriteLog(true, "Tests were executed with {0} error messages", countErrors);
                return 1;
            }

            if (_failedTests.Any(x => x.Value.Count > 0))
            {
                Logger.WriteLog(true, "Tests were executed unsuccessfully. '{0}' tests are failed", _engine.GetAllFailedTestCount(_failedTests));
                return 1;
            }

            Logger.WriteLog(true, "Tests were executed successfully");
            return 0;
        }
    }
}
