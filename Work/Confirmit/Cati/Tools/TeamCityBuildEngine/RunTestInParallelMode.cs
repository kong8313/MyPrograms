using System;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TeamCityBuildEngine.CommonEngines;
using TeamCityBuildEngine.Interfaces;
using ILogger = TeamCityBuildEngine.Interfaces.ILogger;

namespace TeamCityBuildEngine
{
    public class RunTestInParallelMode : Task
    {
        public static string ThreadsExitCode;

        [Output]
        public string ExitCode
        {
            get { return ThreadsExitCode; }
        }

        [Required]
        public string SolutionRoot { private get; set; }

        /// <summary>
        /// Server names and thread number lists.
        /// Example:
        /// <ServersWithThreads>
        ///    <AdditionalBuildServer1>
        ///      <Threads Include="5-8"/>
        ///    </AdditionalBuildServer1>   
        ///    <Localhost>
        ///      <Threads Include="1-4;0"/>
        ///    </Localhost>
        ///  </ServersWithThreads>
        /// localhost = build server
        /// </summary>
        [Required]
        public ITaskItem[] ServersAndThreads { private get; set; }

        [Required]
        public string ThreadCount { private get; set; }

        [Required] 
        public string TestlabInstallerPassword { private get; set; }
        
        public string TestContainerMask { private get; set; }

        public string SqlInstanceName { private get; set; }

        public override bool Execute()
        {
            var logPathPresenter = new LogPathPresenter(SolutionRoot);
            ILogger logger = new FileLogger(logPathPresenter.GetLogPath("RunTestInSelectedComputers.log"));
            ICopyist copyist = new Copyist(logger);

            logger.WriteLog(
                "SolutionRoot={0}\r\nThreadCount={1}\r\nSqlInstanceName={2}\r\n",
                SolutionRoot,
                ThreadCount,
                SqlInstanceName);

            if (!String.IsNullOrEmpty(TestContainerMask))
            {
                logger.WriteLog("TestContainerMask=" + TestContainerMask);
            }

            if (ServersAndThreads == null)
            {
                logger.WriteLog("ServersAndThreads is null");
            }
            else
            {
                logger.WriteLog("ServersAndThreads length is " + ServersAndThreads.Length);
            }

            new FileCleaner(logger).CleanOldDatabaseFiles(SqlInstanceName);

            var runTestsEngine = new RunTestInParallelModeEngine(copyist, logger, SolutionRoot, 
                logPathPresenter, ThreadCount, ServersAndThreads, TestContainerMask, SqlInstanceName);

            try
            {
                ThreadsExitCode = "0";

                runTestsEngine.WaitAllThreads(runTestsEngine.PrepareFilesAndRunTestThreads(TestlabInstallerPassword));
                runTestsEngine.CopyLogsAndTrxFilesFromRemoteServers();
                runTestsEngine.CopyInformationToStandardLog(new StandardLogger(Log));
            }
            catch (Exception ex)
            {
                logger.WriteLog(TraceEventType.Error, ex.ToString());
                ThreadsExitCode = "2";
            }
            
            return true;
        }        
    }
}
