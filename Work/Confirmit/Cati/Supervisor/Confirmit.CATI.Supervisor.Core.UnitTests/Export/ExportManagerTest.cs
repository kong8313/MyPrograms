using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using Confirmit.CATI.Supervisor.Core.Export;
using Confirmit.CATI.Supervisor.Core.Export.Tools;

namespace Confirmit.CATI.Supervisor.Core.UnitTests.Export
{
    /// <summary>
    /// Summary description for Export
    /// </summary>
    [TestClass]
    public class ExportManagerTest
    {
        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetColumnValue_CorrectValue_Success()
        {
            string columnName, columnStyle;

            XElement columnElement = new XElement(
                "c",
                new XAttribute("r", "A1"),                
                new XElement("v", "Column value")
            );

            string value = OpenXmlHelper.GetColumnValue(columnElement, "", new string[0], out columnName, out columnStyle);

            Assert.AreEqual("Column value", value);
            Assert.AreEqual("A", columnName);
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetColumnValue_CorrectValueFromSharedStrings_Success()
        {                     
            string columnName, columnStyle;
            string[] sharedStrings = new string[] { "Column value" };

            XElement columnElement = new XElement(
                "c",
                new XAttribute("r", "A1"),
                new XAttribute("t", "s"),
                new XElement("v", 0)
            );

            string value = OpenXmlHelper.GetColumnValue(columnElement, "", sharedStrings, out columnName, out columnStyle);

            Assert.AreEqual("Column value", value);
            Assert.AreEqual("A", columnName);            
        }

        [TestMethod, Owner(@"FIRM\AlexanderZh")]
        public void GetColumnValue_CorrectInlineStringValue_Success()
        {
            string columnName, columnStyle;
            
            XElement columnElement = new XElement(
                "c",
                new XAttribute("r", "A1"),
                new XAttribute("t", "inlineStr"),
                new XElement("is", new XElement("t", "Column value"))
            );

            string value = OpenXmlHelper.GetColumnValue(columnElement, "", new string[0], out columnName, out columnStyle);

            Assert.AreEqual("Column value", value);
            Assert.AreEqual("A", columnName);            
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetMaxValueForPageRange_GetCorrectMaxValue()
        {
            Assert.AreEqual(1565, ExportManager.GetMaxValueForPageRange(20, 31290));
            Assert.AreEqual(1565, ExportManager.GetMaxValueForPageRange(20, 31300));
            Assert.AreEqual(84, ExportManager.GetMaxValueForPageRange(20, 1667));
            Assert.AreEqual(1, ExportManager.GetMaxValueForPageRange(0, 0));
            Assert.AreEqual(25, ExportManager.GetMaxValueForPageRange(0, 25));
            Assert.AreEqual(1, ExportManager.GetMaxValueForPageRange(1, 0));
        }
    }
}

