using System.Collections.Generic;

namespace RunTestParallelUtility.Interfaces
{
    public interface IParameterVerifier
    {
        bool VerifyTestContainersNames(List<string> testContainersNames);

        bool VerifyThreadNumbers(int[] tempThreadNumbers, int threadCount);
    }
}
