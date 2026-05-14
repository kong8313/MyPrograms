using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Confirmit.CATI.Installation.Common.Interfaces;
using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class ParametersParser : IParametersParser
    {
        public const string CatiServerEnvironmentVariableName = "CATI_SQL_INSTANCE_NAME";

        public const string HelpString =
            "\r\n" +
            "Usage:  RunTestParallelUtility.exe /threadcount:<parallel process count> [/testcontainers:<test container masks>] [/threadlist:<valid thread numbers>] [/sqlinstancename:<sql instance name>]\r\n\r\n" +
            "parallel process count - a count of mstest parallel processes\r\n\r\n" +
            "test container masks   - semicolon delimited mask list. These masks will be used to select builds containing tests (test builds) (default *Integrationtest.dll)\r\n\r\n" +            
            "valid thread numbers   - comma delimited thread number list. Threads with these numbers should be run. Value must range from 0 to the total number number of threads. 0 - a thread for non-parallel tests (tests which couldn't be run in parallel). All threads are run by default. Example: 1-10,21,0,25\r\n" +
            "sql instance name      - SQL instance name to execute tests (if parameter isn't specified, the default sql instance will be used)\r\n" +
            "\r\n";

        /// <summary>
        /// Count of threads
        /// </summary>
        private readonly int _threadCount;

        /// <summary>
        /// Count of threads
        /// </summary>
        public int ThreadCount
        {
            get
            {
                return _threadCount;
            }
        }

        /// <summary>
        /// Array of threads  to run
        /// </summary>
        private readonly int[] _validThreadNumbers;

        /// <summary>
        /// Array of threads  to run
        /// </summary>
        public int[] ValidThreadNumbers
        {
            get
            {
                return _validThreadNumbers;
            }
        }

        /// <summary>
        /// SqlInstanceName
        /// </summary>
        public string SqlInstanceName { get; private set; }

        /// <summary>
        /// Parameter for testcontainer
        /// </summary>
        private StringBuilder _testContainerParameter;

        /// <summary>
        /// String with argumetn for MSTest
        /// </summary>
        public string MsTestParameterString
        {
            get
            {
                string runConfigPath = Path.Combine(Application.StartupPath, "..\\Temp.testsettings");
                return string.Format("/nologo /detail:duration /detail:owner /detail:errormessage /testsettings:\"{0}\" {1}", runConfigPath, _testContainerParameter);
            }
        }

        /// <summary>
        /// Names of dll files with tests
        /// </summary>
        private readonly List<string> _testContainersNames;

        /// <summary>
        /// Names of dll files with tests
        /// </summary>
        public string[] TestContainersNames
        {
            get
            {
                return _testContainersNames.ToArray();
            }
        }

        public ParametersParser(string[] args, IParameterVerifier parameterVerifier)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Wrong count of arguments");
            }

            _testContainersNames = new List<string>();
            
            SqlInstanceName = string.Empty;

            foreach (string arg in args)
            {
                string[] param = arg.Split(new[] { ':' }, 2);
                if (param.Length != 2 || (!param[0].StartsWith("/") && !param[0].StartsWith("-")))
                {
                    throw new ArgumentException("Wrong argument " + arg);
                }

                var testContainers = String.Empty;

                switch (param[0].ToLower().Substring(1))
                {
                    case "threadcount":
                        if (!int.TryParse(param[1], out _threadCount) || _threadCount <= 0)
                        {
                            throw new ArgumentException("/threadcount parameter must be an integer value greater than 0. Current value: " + param[1]);
                        }

                        break;
                    case "threadlist":
                        _validThreadNumbers = ParseThreadNumbers(param[1]);
                        break;
                    case "testcontainers":
                        testContainers = param[1];
                        break;
                    case "sqlinstancename":
                        SetSqlInstanceName(param[1]);                        
                        break;
                    default:
                        throw new ArgumentException("Unknown argument " + arg);
                }

                if (!String.IsNullOrEmpty(testContainers))
                    SetTestContainerProperties(testContainers, parameterVerifier);
            }

            if (string.IsNullOrEmpty(SqlInstanceName))
            {
                SqlInstanceName = Environment.GetEnvironmentVariable(CatiServerEnvironmentVariableName);
            }

            if (_testContainersNames.Count == 0)
            {
                SetTestContainerProperties("*IntegrationTests.dll", parameterVerifier);
            }

            if (_validThreadNumbers == null || _validThreadNumbers.Length == 0)
            {
                _validThreadNumbers = Enumerable.Range(0, _threadCount + 1).ToArray();
            }
            else if (!parameterVerifier.VerifyThreadNumbers(_validThreadNumbers, _threadCount))
            {
                throw new ArgumentException("Wrong valid thread numbers. All thread numbers must be between 0 and the thread count value.");
            }
        }


        /// <summary>
        ///  Add information about parsed parameters to the log
        /// </summary>
        /// <param name="logger"></param>
        public void LogParsedParameters(ILogger logger)
        {
            logger.WriteLog("-----------------------------------");
            logger.WriteLog("Parsed parameters are:");
            logger.WriteLog("Thread count: " + _threadCount);
            logger.WriteLog("Thread list: " +
                            _validThreadNumbers.Aggregate("",
                                                          (result, item) =>
                                                          result.Length > 0 ? result + ", " + item : item.ToString(CultureInfo.InvariantCulture)));
            logger.WriteLog("Test containers: " + TestContainersNames.Aggregate("", (result, item) => result + "\r\n" + item));
            logger.WriteLog("SQL instance name: " + SqlInstanceName);
            logger.WriteLog("-----------------------------------");
        }


        /// <summary>
        /// Set SqlInstanceName
        /// </summary>
        /// <param name="sqlInstanceName">Sql instance name</param>
        private void SetSqlInstanceName(string sqlInstanceName)
        {
            if (!string.IsNullOrWhiteSpace(sqlInstanceName))
            {
                SqlInstanceName = sqlInstanceName;
            }
        }


        /// <summary>
        /// Convert thread numbers from string to array
        /// </summary>
        /// <param name="threadNumbersStr">threadNumbersStr should be like this: 0;3-7;10</param>
        /// <returns></returns>
        private static int[] ParseThreadNumbers(string threadNumbersStr)
        {
            try
            {
                string[] threadNumbersArr = threadNumbersStr.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var threadNumbers = new List<int>();

                foreach (string str in threadNumbersArr)
                {
                    if (!str.Contains("-"))
                    {
                        threadNumbers.Add(Convert.ToInt32(str));
                        continue;
                    }

                    string[] rangeInfo = str.Split('-');
                    if (rangeInfo.Length != 2)
                    {
                        throw new Exception();
                    }

                    int first = Convert.ToInt32(rangeInfo[0]);
                    int second = Convert.ToInt32(rangeInfo[1]);
                    threadNumbers.AddRange(Enumerable.Range(first, second - first + 1));
                }

                return threadNumbers.ToArray();
            }
            catch
            {
                throw new ArgumentException("Wrong /threadlist parameter: " + threadNumbersStr);
            }
        }


        /// <summary>
        /// Set _testContainerParameter and _testContainersPaths parameters
        /// </summary>
        /// <param name="testContainerMasks">String like this: *.IntegrationTest.dll;*.Unit.dll;TestContainerName.dll</param>
        /// <param name="parameterVerifier">Startup path provider</param>
        private void SetTestContainerProperties(string testContainerMasks, IParameterVerifier parameterVerifier)
        {
            string[] masks = testContainerMasks.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            _testContainerParameter = new StringBuilder();

            foreach (string mask in masks)
            {
                // Получить список файлов по указанной маске
                foreach (string filePath in Directory.GetFiles(Application.StartupPath, mask))
                {
                    _testContainersNames.Add(filePath);
                    _testContainerParameter.AppendFormat("/testcontainer:\"{0}\" ", filePath);
                }
            }

            if (!parameterVerifier.VerifyTestContainersNames(_testContainersNames))
            {
                throw new ArgumentException("No files were found by this mask: " + testContainerMasks);
            }
        }
    }
}
