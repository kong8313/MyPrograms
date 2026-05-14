using System.Collections.Generic;

namespace RunTestParallelUtility.Interfaces
{
    public interface IAssemblyParser
    {
        /// <summary>
        /// Get all methods with Test attribute and without Ignoring and CannotWorkInParallel attributes
        /// </summary>
        /// <param name="testContainersNames">Dll files list</param>
        /// <returns></returns>
        Dictionary<string, TestClassInfo> GetActiveTests(IEnumerable<string> testContainersNames);
        

        /// <summary>
        /// Get all methods with Test and CannotWorkInParallel attributes and without Ignoring attribute
        /// </summary>
        /// <param name="testContainersNames">Dll files list</param>
        /// <returns></returns>
        Dictionary<string, TestClassInfo> GetCannotWorkInParallelTests(IEnumerable<string> testContainersNames);
    }
}
