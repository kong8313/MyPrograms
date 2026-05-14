using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common;
using Confirmit.CATI.Installation.Common.Interfaces;
using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class Program
    {
        private readonly IParametersParser _parametersParser;
        private readonly IAssemblyParser _assemblyParser;
        
        private readonly IEngine _engine;
        private readonly ITestExecutor _testExecutor;
        private readonly ILogger _logger;

        private Program(IAssemblyParser assemblyParser, IParametersParser parametersParser, IEngine engine, ITestExecutor testExecutor, ILogger logger)
        {
            _parametersParser = parametersParser;
            _assemblyParser = assemblyParser;
            _engine = engine;
            _testExecutor = testExecutor;
            _logger = logger;
        }

        /// <summary>
        /// Create strings with name of tests for command line in case of Integration tests
        /// </summary>
        /// <returns></returns>
        private string[] GetStringsToRunInMsTest()
        {
            string[] cmdLines = CreateStringsWithExecutedTestsForMsTestCommandLine();

            _logger.WriteLog("-----------------------------------");
            _logger.WriteLog("Tests to run:");
            foreach (var cmdLine in cmdLines)
            {
                _logger.WriteLog(cmdLine);
            }
            _logger.WriteLog("-----------------------------------");

            return cmdLines;
        }

        /// <summary>
        /// Create strings with name of tests for command line in case of Integration tests
        /// </summary>
        /// <returns></returns>
        private string[] CreateStringsWithExecutedTestsForMsTestCommandLine()
        {
            if (_parametersParser.ThreadCount == 1)
            {
                return new string[1];
            }

            var testLists = new TestClassInfo[_parametersParser.ThreadCount];
            for (int i = 0; i < testLists.Length; i++)
            {
                testLists[i] = new TestClassInfo();
            }

            Dictionary<string, TestClassInfo> tests = _assemblyParser.GetActiveTests(_parametersParser.TestContainersNames);

            var orderedTests = tests.OrderByDescending(x => x.Value.TestCount);

            foreach (KeyValuePair<string, TestClassInfo> pair in orderedTests)
            {
                var currentList = testLists.OrderBy(x => x.TestCount).First();
                currentList.AddRange(pair.Value);
            }

            return testLists.Where(x => x.TestList.Length > 0).Select(x => " /test:" + string.Join(" /test:", x.TestList)).ToArray();
        }

        private async Task RunTestsAndLog(string testResultDirectory, string testsToRun, int treadNumber)
        {
            await Task.Factory.StartNew(() => _testExecutor.RunTests(_parametersParser.MsTestParameterString, testResultDirectory, testsToRun, treadNumber), TaskCreationOptions.LongRunning);
            _logger.WriteLog($"Process #{treadNumber} is over");
        }

        /// <summary>
        /// Run standard tests in parallel mode
        /// </summary>
        /// <param name="testResultDirectory">Directory for test results</param>
        /// <param name="testsToRunList">Command line with tests classes for executing for all parallel processes</param>
        private void RunTestsInParallelMode(string testResultDirectory, string[] testsToRunList)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < _parametersParser.ThreadCount; i++)
            {
                if (!_parametersParser.ValidThreadNumbers.Contains(i + 1))
                {
                    continue;
                }

                _logger.WriteLog($"Run process #{i + 1}");

                tasks.Add(RunTestsAndLog(testResultDirectory, testsToRunList[i], i + 1));

                Thread.Sleep(10000);
            }

            Task.WaitAll(tasks.ToArray());
            _logger.WriteLog("All processes has finished.");
        }


        /// <summary>
        /// Run Tests with CannotWorkInParallel attribute
        /// </summary>
        /// <param name="testResultDirectory">Directory for test results</param>
        private void RunCannotWorkInParallelTests(string testResultDirectory)
        {
            Dictionary<string, TestClassInfo> tests = _assemblyParser.GetCannotWorkInParallelTests(_parametersParser.TestContainersNames);

            string testsToRun = tests.Keys.Select(x => tests[x]).SelectMany(methods => methods.TestList).Aggregate(string.Empty, (current, method) => current + (" /test:" + method));

            if (!string.IsNullOrEmpty(testsToRun))
            {
                Task task = Task.Run(() => _testExecutor.RunTests(_parametersParser.MsTestParameterString, testResultDirectory, testsToRun, 0));

                task.Wait();
            }
        }

        private void LogEnvironmentInformation()
        {
            _logger.WriteLog("-----------------------------------");
            _logger.WriteLog("ENVIRONMENT INFORMATION");
            _logger.WriteLog("Current user:");
            _logger.WriteLog(WindowsIdentity.GetCurrent().Name);
            _logger.WriteLog("Current machine name:");
            _logger.WriteLog(Environment.MachineName);
            _logger.WriteLog("Current directory:");
            _logger.WriteLog(Environment.CurrentDirectory);
            _logger.WriteLog("Executing assembly location:");
            _logger.WriteLog(Assembly.GetExecutingAssembly().Location);
            _logger.WriteLog("Executing assembly location directory:");
            _logger.WriteLog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "");
            _logger.WriteLog("Application startup path:");
            _logger.WriteLog(Path.GetDirectoryName(Application.StartupPath));
            _logger.WriteLog("-----------------------------------");
        }


        /// <summary>
        /// General function of utility
        /// </summary>
        /// <returns></returns>
        private int StartProgram()
        {
            var startTime = DateTime.Now;
            try
            {
                LogEnvironmentInformation();

                string[] cmdLines = GetStringsToRunInMsTest();

                string testResultDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? string.Empty, @"..\TestResults\");
                if (!Directory.Exists(testResultDirectory))
                {
                    Directory.CreateDirectory(testResultDirectory);
                }

                if (_parametersParser.TestContainersNames.Any(testContainersName => !testContainersName.Contains("Unit")))
                {
                    _engine.DropTestConfirmlogDatabases();
                }

                RunTestsInParallelMode(testResultDirectory, cmdLines);

                if (_parametersParser.ThreadCount > 1 && _parametersParser.ValidThreadNumbers.Contains(0))
                {
                    _logger.WriteLog(true, "I'm running tests that cannot work in parallel.");
                    RunCannotWorkInParallelTests(testResultDirectory);
                }

                return _testExecutor.SaveErrorLogsAndCreateExitCode();
            }
            finally
            {
                var diffSpan = DateTime.Now - startTime;
                string timeInfoString = diffSpan.Hours.ToString("D2") + ":" + diffSpan.Minutes.ToString("D2") + ":" + diffSpan.Seconds.ToString("D2");
                _logger.WriteLog(true, "Elapsed time: " + timeInfoString);
            }
        }


        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">Program arguments</param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            if (args.Length == 1 &&
               (args[0] == "/?" || args[0] == "-?" || args[0] == "/help" ||
                args[0] == "-help" || args[0] == "/h" || args[0] == "-h"))
            {
                Console.WriteLine(ParametersParser.HelpString);
                return 1;
            }

            ILogger logger;
            try
            {
                logger = new FileAndConsoleLogger(Path.Combine(Application.StartupPath, "RunTestParallelUtilityLog.txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Logging error:\r\n" + ex);
                return 2;
            }

            try
            {
                IAssemblyParser assemblyParser = new AssemblyParser();
                IParametersParser parametersParser;

                try
                {
                    parametersParser = new ParametersParser(args, new ParameterVerifier());
                    parametersParser.LogParsedParameters(logger);
                }
                catch (ArgumentException ex)
                {
                    logger.WriteLog(true, ex.Message + ParametersParser.HelpString);
                    return 2;
                }

                IEngine engine = new Engine(logger, parametersParser);
                ITestResultFileEngine testResultFileEngine = new TestResultFileEngine();
                IPathProvider pathProvider = new PathProvider();

                ITestExecutor testExecutor = new TestExecutor(logger, engine, pathProvider, testResultFileEngine,
                    parametersParser.SqlInstanceName);

                return new Program(assemblyParser, parametersParser, engine, testExecutor, logger).StartProgram();
            }
            catch (ReflectionTypeLoadException ex)
            {
                logger.WriteLog("Loader exceptions:\r\n");
                foreach (var le in ex.LoaderExceptions)
                {
                    logger.WriteLog(true, le.Message);
                }
                return 2;
            }
            catch (Exception ex)
            {
                logger.WriteLog(true, ex.Message);

                logger.WriteLog("Global error:\r\n" + ex);
                return 2;
            }
        }
    }
}
