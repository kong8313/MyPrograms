using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RunTestParallelUtility.Interfaces;

namespace RunTestParallelUtility
{
    public class AssemblyParser : IAssemblyParser
    {
        /// <summary>
        /// Get all methods with Test attribute and without Ignoring attribute
        /// </summary>
        /// <param name="testContainersNames">Dll files list</param>
        /// <param name="getCannotWorkInParallelTests">if true - return tests with CannotWorkInParallel attribute only, false - return tests without CannotWorkInParallel attribute</param>
        /// <returns></returns>
        private static Dictionary<string, TestClassInfo> GetTests(IEnumerable<string> testContainersNames, bool getCannotWorkInParallelTests)
        {
            var tests = new Dictionary<string, TestClassInfo>();

            foreach (string testAssemblyName in testContainersNames)
            {
                Assembly assembly = Assembly.LoadFile(testAssemblyName);

                foreach (Type type in assembly.GetTypes())
                {
                    IEnumerable<string> typeAttrs = CustomAttributeData.GetCustomAttributes(type).Select(x => x.ToString()).ToArray();
                    if (typeAttrs.Any(x => x.Contains("IgnoreAttribute")))
                    {
                        continue;
                    }

                    bool classHasCannotWorkInParallelAttribute = typeAttrs.Any(x => x.Contains("CannotWorkInParallel"));

                    var testsFromOneClass = new List<string>();
                    bool isWeUseAllTestsFromClass = true;
                    int testCnt = 0;
                    foreach (MethodInfo methodInfo in type.GetMethods())
                    {
                        IEnumerable<string> methodAttrs = CustomAttributeData.GetCustomAttributes(methodInfo).Select(x => x.ToString()).ToArray();

                        if (!methodAttrs.Any(x => x.Contains("TestMethodAttribute")))
                        {
                            continue;
                        }

                        bool hasCannotWorkInParallelAttribute = classHasCannotWorkInParallelAttribute || methodAttrs.Any(x => x.Contains("CannotWorkInParallel"));

                        if ((getCannotWorkInParallelTests && !hasCannotWorkInParallelAttribute) ||
                            (!getCannotWorkInParallelTests && hasCannotWorkInParallelAttribute) ||
                            methodAttrs.Any(x => x.Contains("IgnoreAttribute")))
                        {
                            isWeUseAllTestsFromClass = false;
                            continue;
                        }

                        testCnt++;
                        testsFromOneClass.Add(methodInfo.Name);
                    }

                    if (testCnt > 0)
                    {
                        // if we use all tests from class - change all tests to class name with ".*" at the end
                        if (isWeUseAllTestsFromClass)
                        {
                            testsFromOneClass = new List<string> { type.FullName + ".*" };
                        }

                        tests.Add(type.FullName, new TestClassInfo(testCnt, testsFromOneClass));
                    }
                }
            }

            return tests;
        }


        /// <summary>
        /// Get all methods with Test attribute and without Ignoring and CannotWorkInParallel attributes
        /// </summary>
        /// <param name="testContainersNames">Dll files list</param>
        /// <returns></returns>
        public Dictionary<string, TestClassInfo> GetActiveTests(IEnumerable<string> testContainersNames)
        {
            return GetTests(testContainersNames, false);
        }


        /// <summary>
        /// Get all methods with Test and CannotWorkInParallel attributes and without Ignoring attribute
        /// </summary>
        /// <param name="testContainersNames">Dll files list</param>
        /// <returns></returns>
        public Dictionary<string, TestClassInfo> GetCannotWorkInParallelTests(IEnumerable<string> testContainersNames)
        {
            return GetTests(testContainersNames, true);
        }
    }
}
