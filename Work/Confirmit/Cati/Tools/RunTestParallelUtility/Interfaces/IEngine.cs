using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace RunTestParallelUtility.Interfaces
{
    public interface IEngine
    {
        /// <summary>
        /// Drop all databases are started by "test_confirmlog"
        /// </summary>
        void DropTestConfirmlogDatabases();

        /// <summary>
        /// Get log file path from arguments line
        /// </summary>
        /// <param name="args">String with arguments</param>
        /// <param name="isOutput">true - return path to output log file, false - to error log file</param>
        /// <returns></returns>
        string GetLogFilePathFromArgs(string args, bool isOutput);

        /// <summary>
        /// Get count of failed tests from all processes
        /// </summary>
        /// <param name="failedTests"></param>
        /// <returns></returns>
        int GetAllFailedTestCount(Dictionary<string, List<string>> failedTests);

        void SaveOutputLog(string args, Dictionary<string, StringBuilder> outputStrings);

        string GetCmdLineForFailedTests(List<string> failedTests);

        string GetTestName(string argsData);

        void CreateOrUpdateTheEnvironmentVariable(StringDictionary environmentVariables, string environmentVariable, string value);

        string GetTrxFilePath(string testResultDirectory, string additionalFileNamePart = "");
    }
}
