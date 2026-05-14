using System.Collections.Generic;

using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility.UnitTests
{
    public class TestParameterVerifier : IParameterVerifier
    {
        public bool VerifyTestContainersNames(List<string> testContainersNames)
        {
            return true;
        }

        public bool VerifyRunConfigPath(string runConfigPath)
        {
            return true;
        }

        public bool VerifyThreadNumbers(int[] tempThreadNumbers, int threadCount)
        {
            return true;
        }
    }
}
