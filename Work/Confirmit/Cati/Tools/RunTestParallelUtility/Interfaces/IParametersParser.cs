using Confirmit.CATI.Installation.Common.Interfaces;

namespace RunTestParallelUtility.Interfaces
{
    public interface IParametersParser
    {
        /// <summary>
        /// Count of threads
        /// </summary>
        int ThreadCount { get; }

        /// <summary>
        /// String with argumetn for MSTest
        /// </summary>
        string MsTestParameterString { get; }

        /// <summary>
        /// Paths to dll files with testcontainer parameters
        /// </summary>
        string[] TestContainersNames { get; }

        /// <summary>
        /// Array of threads  to run
        /// </summary>
        int[] ValidThreadNumbers { get; }

        /// <summary>
        /// SQL instance name to run tests
        /// </summary>
        string SqlInstanceName { get; }

        /// <summary>
        ///  Add information about parsed parameters to the log
        /// </summary>
        /// <param name="logger"></param>
        void LogParsedParameters(ILogger logger);
    }
}
