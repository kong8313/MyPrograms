using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace RunTestParallelUtility
{
    public class TestItem
    {
        [XmlText]
        public string Name { get; set; }

        [XmlAttribute]
        public bool Run { get; set; }
    }

    public class TestClassItem
    {
        [XmlAttribute]
        public bool Run { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement(ElementName = "Test")]
        public TestItem[] TestsItemList
        {
            get;
            set;
        }
    }

    public class TestProjectItem
    {
        [XmlAttribute]
        public bool Run { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement(ElementName = "TestClass")]
        public TestClassItem[] TestsClasses
        {
            get;
            set;
        }
    }

    public class TestToRun
    {
        [XmlElement(ElementName = "RunBeforeAnyTests")]
        public TestToRun RunBeforeAnyTests
        {
            get;
            set;
        }

        [XmlElement(ElementName = "TestProject")]
        public TestProjectItem[] TestsProjects
        {
            get;
            set;
        }

        public IList<string> GetTestToRun()
        {
            return GetTestNamesToRun().Select(item => "/test:" + item).ToList();
        }

        public IList<string> GetTestToRun(int groupCount)
        {
            var tests = GetTestToRun();
            var groups = new List<StringBuilder>();

            for (int i = 0; i < groupCount; i++ )
            {
                groups.Add(new StringBuilder());
            }
            
            int j = 0;
            foreach (var test in tests)
            {
                groups[j].Append(" ").Append(test);
                j = j + 1 == groupCount ? 0 : j + 1;
            }

            return groups.Select(group => group.ToString()).ToList();
        }

        public IList<string> GetTestNamesToRun()
        {
            if (TestsProjects == null)
                return null;

            var names = new List<string>();

            if (RunBeforeAnyTests != null)
            {
               names = new List<string>(names.Concat(RunBeforeAnyTests.GetTestNamesToRun()));
            }

            foreach (var project in TestsProjects)
            {
                if (project.TestsClasses == null)
                    continue;

                if (!project.Run)
                    continue;

                foreach (var testClass in project.TestsClasses)
                {
                    if (!testClass.Run)
                        continue;

                    if (testClass.TestsItemList == null)
                    {
                        names.Add(project.Name + "." + testClass.Name + ".*");
                        continue;
                    }

                    foreach (var testItem in testClass.TestsItemList)
                    {
                        if (!testItem.Run)
                            continue;

                        names.Add(project.Name + "." + testClass.Name + "." + testItem.Name);
                    }
                }
            }

            return names;
        }
    }
}
