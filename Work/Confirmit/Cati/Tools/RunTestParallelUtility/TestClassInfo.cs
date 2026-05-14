using System.Collections.Generic;
using System.Diagnostics;

namespace RunTestParallelUtility
{
    [DebuggerDisplay("TestCount={TestCount} TestListCnt={TestList.Length} TestList={ListToString}")]
    public class TestClassInfo
    {
        private string ListToString
        {
            get { return string.Join(", ", TestList); }
        }

        private int _testCount;

        public int TestCount
        {
            get
            {
                return _testCount;
            }
        }

        private readonly List<string> _testList;

        public string[] TestList
        {
            get
            {
                return _testList.ToArray();
            }
        }

        public TestClassInfo()
        {
            _testCount = 0;
            _testList = new List<string>();
        }

        public TestClassInfo(int testCnt, IEnumerable<string> testList)
        {
            _testCount = testCnt;
            _testList = new List<string>(testList);            
        }

        public void AddRange(TestClassInfo testClassInfo)
        {
            _testList.AddRange(testClassInfo.TestList);
            _testCount += testClassInfo.TestCount;
        }
    }
}
