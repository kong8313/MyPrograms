using System.Collections.Generic;
using DialerWsLogParserLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerWsLogParserTest
{
    [TestClass]
    public class FileReaderTests
    {
        [TestMethod]
        public void SetRecentFileNames()
        {
            var fileReader = new FileReader();
            var fileNames = new List<string> { "file1", "file2", "file3" };

            fileReader.SetRecentFileNames(fileNames);
            CollectionAssert.AreEqual(fileReader.RecentFileNames, fileNames);
        }

        [TestMethod]
        public void Clean()
        {
            var fileReader = new FileReader();
            fileReader.Clean();

            Assert.AreEqual(fileReader.FileNames.Count, 0);
            Assert.AreEqual(fileReader.Text.Count, 0);
        }
    }
}
