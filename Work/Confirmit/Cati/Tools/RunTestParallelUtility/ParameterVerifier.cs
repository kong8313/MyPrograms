using System.Collections.Generic;
using System.Linq;

using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class ParameterVerifier : IParameterVerifier
    {
        public bool VerifyTestContainersNames(List<string> testContainersNames)
        {
            return testContainersNames.Count != 0;
        }

        public bool VerifyThreadNumbers(int[] tempThreadNumbers, int threadCount)
        {
            return tempThreadNumbers.All(number => number >= 0 && number <= threadCount);
        }
    }
}
