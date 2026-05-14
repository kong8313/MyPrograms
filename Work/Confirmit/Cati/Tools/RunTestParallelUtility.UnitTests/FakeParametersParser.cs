using Confirmit.CATI.Installation.Common.Interfaces;
using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility.UnitTests
{
    class FakeParametersParser : IParametersParser
    {
        public int ThreadCount
        {
            get;
            set;
        }

        public string MsTestParameterString
        {
            get;
            set;
        }

        public string[] TestContainersNames
        {
            get;
            set;
        }

        public int[] ValidThreadNumbers
        {
            get;
            set;
        }

        public string PublishParameters
        {
            get;
            set;
        }

        public string SqlInstanceName
        {
            get;
            set;
        }

        public void LogParsedParameters(ILogger logger)
        {
            
        }
    }
}
