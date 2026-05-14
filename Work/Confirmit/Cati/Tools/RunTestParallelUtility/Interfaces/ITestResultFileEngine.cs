using System.Collections.Generic;

namespace RunTestParallelUtility.Interfaces
{
    public interface ITestResultFileEngine
    {
        void RemoveFailedTestInfo(IEnumerable<string> failedTests, string trxFilePath);
    }
}
