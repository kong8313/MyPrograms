namespace RunTestParallelUtility.Interfaces
{
    public interface ITestExecutor
    {
        /// <summary>
        /// Run tests
        /// </summary>
        /// <param name="msTestParameterString">String with argument for MSTest</param>
        /// <param name="testResultDirectory">Test result directory</param>
        /// <param name="testsToRun">Command line with tests classes for executing for one process</param>
        /// <param name="threadNumber">Number of executed thread</param>
        void RunTests(string msTestParameterString, string testResultDirectory, string testsToRun, int threadNumber);
        
        /// <summary>
        /// Save output and error log files 
        /// </summary>
        /// <returns>
        /// </returns>
        int SaveErrorLogsAndCreateExitCode();
    }
}
