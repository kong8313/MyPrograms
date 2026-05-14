using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Build.Framework;
using TeamCityBuildEngine.CommonEngines;
using TeamCityBuildEngine.Interfaces;
using ILogger = TeamCityBuildEngine.Interfaces.ILogger;

namespace TeamCityBuildEngine
{
    public class RunTestInParallelModeEngine
    {
        private readonly ICopyist _copyist;
        private readonly ILogger _logger;
        private readonly string _solutionRoot;
        private readonly LogPathPresenter _logPathPresenter;
        private readonly string _threadCount;
        private readonly string _testResultFolderPath;
        private readonly string _testContainerMask;
        private readonly string _sqlInstanceName;
        private readonly ITaskItem[] _serversAndThreads;

        public RunTestInParallelModeEngine(
            ICopyist copyist, 
            ILogger logger, 
            string solutionRoot, 
            LogPathPresenter logPathPresenter,
            string threadCount, 
            ITaskItem[] serversAndThreads, 
            string testContainerMask,
            string sqlInstanceName)
        {
            _copyist = copyist;
            _logger = logger;
            _solutionRoot = solutionRoot;
            _logPathPresenter = logPathPresenter;
            _threadCount = threadCount;
            _serversAndThreads = serversAndThreads;
            _testContainerMask = testContainerMask;
            _sqlInstanceName = sqlInstanceName;
            _testResultFolderPath = Path.Combine(_solutionRoot, "TestResults");
        }

        public IEnumerable<Thread> PrepareFilesAndRunTestThreads(string testlabInstallerPassword)
        {
            _logger.WriteLog("Start PrepareFilesAndRunTestThreads");
            var threads = new List<Thread>();

            foreach (var serversAndThread in _serversAndThreads)
            {
                _logger.WriteLog("serversAndThread={0}", serversAndThread);

                string arguments;
                var serverName = GetServerName(serversAndThread.ItemSpec);
                var threadList = GetThreadList(serversAndThread.ItemSpec);

                _logger.WriteLog("serverName={0}\r\nthreadList={1}", serverName, threadList);

                if (serverName != "localhost")
                {
                    CopyNeededFiles(serverName);
                    const string runTestUtilityPath = @"C:\assemblies\assemblies\RunTestParallelUtility.exe";

                    arguments = $"/accepteula -u firm\\TestlabInstaller -p {testlabInstallerPassword} {@"\\" + serverName} \"{runTestUtilityPath}\" /threadcount:{_threadCount} /threadlist:{threadList} /sqlinstancename:{_sqlInstanceName}";

                    if (!string.IsNullOrEmpty(_testContainerMask))
                    {
                        arguments += " /testcontainers:" + _testContainerMask;
                    }

                    var thread = new Thread(AsyncRunTest);
                    threads.Add(thread);
                    thread.Start("PsExec\\PsExec.exe;;" + arguments);
                }
                else
                {
                    arguments = string.Format(
                        "/threadcount:{0} /threadlist:{1} /sqlinstancename:{2}",
                        _threadCount,
                        threadList,
                        _sqlInstanceName);

                    if (!string.IsNullOrEmpty(_testContainerMask))
                    {
                        arguments += " /testcontainers:" + _testContainerMask;
                    }

                    var thread = new Thread(AsyncRunTest);
                    threads.Add(thread);
                    thread.Start(Path.Combine(_solutionRoot, "assemblies\\RunTestParallelUtility.exe") + ";;" + arguments);
                }

                _logger.WriteLog("arguments={0}", arguments);
            }

            _logger.WriteLog("PrepareFilesAndRunTestThreads finished successfull");
            return threads;
        }

        /// <summary>
        /// Get information about server name from serversAndThread parameter
        /// </summary>
        /// <param name="serversAndThread">Parameter like this: server_name:0-4;7;8</param>
        /// <returns></returns>
        private static string GetServerName(string serversAndThread)
        {
            return serversAndThread.ToLower().Split(new[] { ':' })[0];
        }

        /// <summary>
        /// Get information about threads from serversAndThread parameter
        /// </summary>
        /// <param name="serversAndThread">Parameter like this: server_name:0-4;7;8</param>
        /// <returns></returns>
        private static string GetThreadList(string serversAndThread)
        {
            string[] temp = serversAndThread.Split(new[] { ':' }, 2);
            if (temp.Length == 2)
            {
                return temp[1];
            }

            throw new Exception("Wrong threads information: " + serversAndThread);
        }

        /// <summary>
        /// Copy needed files for running tests to remote server
        /// </summary>
        /// <param name="serverName">Remote server name</param>
        private void CopyNeededFiles(string serverName)
        {
            string remotePath = string.Format("\\\\{0}\\assemblies", serverName);

            _copyist.CopyDirectory(
                Path.Combine(_solutionRoot, "assemblies"),
                Path.Combine(remotePath, "assemblies"),
                new List<string> { Path.Combine(_solutionRoot, "assemblies\\Installation") });

            _copyist.RemoveDirectory(Path.Combine(remotePath, "TestResults"));
        }

        /// <summary>
        /// Function for asynchronius running tests
        /// </summary>
        /// <param name="parameters">2 parameters, separated by double semicolon</param>
        private void AsyncRunTest(object parameters)
        {
            ILogger logger = new FileLogger(_logPathPresenter.GetLogPath("RunTestInSelectedComputers_" + Guid.NewGuid() + ".log"));
            IExternalExecutor externalInvoker = new ExternalExecutor(logger);

            try
            {
                string[] parametersArray = ((string)parameters).Split(new[] { ";;" }, StringSplitOptions.None);
                if (parametersArray.Length != 2)
                {
                    throw new Exception("Wrong parameter object: " + parameters);
                }

                string scriptPathOrName = parametersArray[0];
                string arguments = parametersArray[1];

                externalInvoker.Invoke(scriptPathOrName, arguments);
            }
            catch (Exception ex)
            {
                logger.WriteLog(TraceEventType.Error, ex.ToString());
                RunTestInParallelMode.ThreadsExitCode = externalInvoker.ExitCode.ToString();
            }
        }

        /// <summary>
        /// Waitm while all threads finish its work
        /// </summary>
        /// <param name="threads">Thread list</param>
        public void WaitAllThreads(IEnumerable<Thread> threads)
        {
            _logger.WriteLog("Start WaitAllThreads");
            foreach (var thread in threads)
            {
                thread.Join();
            }

            _logger.WriteLog("WaitAllThreads finishes successfull");
        }

        /// <summary>
        /// Copy logs and tests results files from remote servers
        /// </summary>
        public void CopyLogsAndTrxFilesFromRemoteServers()
        {
            _logger.WriteLog("Start CopyLogsAndTrxFilesFromRemoteServers");

            foreach (var serversAndThread in _serversAndThreads)
            {
                var serverName = GetServerName(serversAndThread.ItemSpec);                

                if (serverName == "localhost")
                {
                    continue;
                }

                var remoteTestResultPath = string.Format("\\\\{0}\\assemblies\\TestResults", serverName);
                if (Directory.Exists(remoteTestResultPath))
                {
                    foreach (var filePath in
                        Directory.GetFiles(remoteTestResultPath).Where(filePath => Path.GetExtension(filePath) == ".txt" || Path.GetExtension(filePath) == ".trx"))
                    {
                        _copyist.CopyFile(filePath, Path.Combine(_testResultFolderPath, "_" + serverName + "_" + Path.GetFileName(filePath)));
                    }
                }

                var remoteRunTestParallelUtilityLogLocation = string.Format("\\\\{0}\\assemblies\\assemblies\\RunTestParallelUtilityLog.txt", serverName);
                if (File.Exists(remoteRunTestParallelUtilityLogLocation))
                {
                    _copyist.CopyFile(remoteRunTestParallelUtilityLogLocation, Path.Combine(_testResultFolderPath, "_" + serverName + "_" + Path.GetFileName(remoteRunTestParallelUtilityLogLocation)));
                }
            }

            _logger.WriteLog("CopyLogsAndTrxFilesFromRemoteServers finished successfull");
        }
        
        /// <summary>
        /// Copy information from different log files to general build log
        /// from BuildType
        /// RunTestInSelectedComputers.log
        /// RunTestInSelectedComputers_GUID.log
        /// ...
        /// RunTestInSelectedComputers_GUID.log
        ///         
        /// From Sources\TestResult
        /// SOMETHING_RunTestParallelUtilityLog.txt
        /// ...
        /// SOMETHING_RunTestParallelUtilityLog.txt
        /// 
        /// From Sources\assemblies
        /// RunTestParallelUtilityLog.txt
        /// 
        /// From Sources\TestResult
        /// SOMETHING_Output.txt
        /// ...
        /// SOMETHING_Output.txt
        /// </summary>
        /// <param name="standardLogger">Logger to log to standard general log file of build</param>
        public void CopyInformationToStandardLog(StandardLogger standardLogger)
        {
            string buildTypeFolderPath = _logPathPresenter.GetLogPath(string.Empty);
            
            foreach (string logFilePath in Directory.GetFiles(buildTypeFolderPath).Where(filePath => Path.GetFileName(filePath).StartsWith("RunTestInSelectedComputers")))
            {
                CopyFileContentToStandardLog(logFilePath, standardLogger);
            }

            string runTestParallelUtilityLogFilePath = Path.Combine(_solutionRoot, @"assemblies\RunTestParallelUtilityLog.txt");
            CopyFileContentToStandardLog(runTestParallelUtilityLogFilePath, standardLogger);

            foreach (string logFilePath in Directory.GetFiles(_testResultFolderPath).Where(filePath => filePath.EndsWith("_RunTestParallelUtilityLog.txt")))
            {
                CopyFileContentToStandardLog(logFilePath, standardLogger);
            }

            foreach (string logFilePath in Directory.GetFiles(_testResultFolderPath).Where(filePath => filePath.EndsWith("_Output.txt")))
            {
                CopyFileContentToStandardLog(logFilePath, standardLogger);
            }
        }

        private void CopyFileContentToStandardLog(string logFilePath, StandardLogger standardLogger)
        {
            standardLogger.WriteLog("CONTENT OF \"" + logFilePath + "\" FILE");
            standardLogger.WriteLog(File.ReadAllText(logFilePath));
            standardLogger.WriteLog("\r\n\r\n\r\n");
        }
    }
}
