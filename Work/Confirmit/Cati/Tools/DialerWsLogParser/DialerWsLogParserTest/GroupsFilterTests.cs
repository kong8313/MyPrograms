using DialerWsLogParserLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerWsLogParserTest
{
    [TestClass]
    public class GroupFilterTests
    {
        [TestMethod]
        public void SetColumn()
        {
            var filter = new GroupsFilter(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty);

            filter.SetColumn("Name", "name");

            Assert.AreEqual(filter.Name, "name");
        }
    }
}
