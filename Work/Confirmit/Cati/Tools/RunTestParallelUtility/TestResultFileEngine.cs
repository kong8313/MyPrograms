using RunTestParallelUtility.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml;

namespace RunTestParallelUtility
{
    public class TestResultFileEngine : ITestResultFileEngine
    {
        public void RemoveFailedTestInfo(IEnumerable<string> failedTests, string trxFilePath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(trxFilePath);

            var nsMgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsMgr.AddNamespace("ns", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

            FixResultSummary(xmlDocument, nsMgr);

            foreach (string fullFailedTestName in failedTests)
            {                
                string testId = RemoveTestDefinitionsNodeForFailedTests(xmlDocument, nsMgr, fullFailedTestName);

                RemoveTestEntriesNodeForFailedTests(xmlDocument, nsMgr, testId);

                RemoveResultsNodeForFailedTest(xmlDocument, nsMgr, testId);
            }

            xmlDocument.Save(trxFilePath);            
        }        

        private void FixResultSummary(XmlDocument xmlDocument, XmlNamespaceManager nsMgr)
        {
            string passedTestsCount = GetAttribute(xmlDocument, nsMgr, "//ns:TestRun/ns:ResultSummary/ns:Counters", "passed");
            SetAttribute(xmlDocument, nsMgr, "//ns:TestRun/ns:ResultSummary/ns:Counters", "executed", passedTestsCount);
            SetAttribute(xmlDocument, nsMgr, "//ns:TestRun/ns:ResultSummary/ns:Counters", "total", passedTestsCount);
            SetAttribute(xmlDocument, nsMgr, "//ns:TestRun/ns:ResultSummary/ns:Counters", "failed", "0");

            SetAttribute(xmlDocument, nsMgr, "//ns:TestRun/ns:ResultSummary", "outcome", "Completed");
        }

        private string RemoveTestDefinitionsNodeForFailedTests(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string fullFailedTestName)
        {            
            ParseFullFailedTestName(fullFailedTestName, out string className, out string failedTestName);

            string testId = GetTestId(xmlDocument, nsMgr, $"//ns:TestRun/ns:TestDefinitions/ns:UnitTest[@name='{failedTestName}']", className, failedTestName);
            RemoveNode(xmlDocument, nsMgr, $"//ns:TestRun/ns:TestDefinitions/ns:UnitTest[@id='{testId}']");

            return testId;
        }

        private void RemoveTestEntriesNodeForFailedTests(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string testId)
        {
            RemoveNode(xmlDocument, nsMgr, $"//ns:TestRun/ns:TestEntries/ns:TestEntry[@testId='{testId}']");
        }

        private void RemoveResultsNodeForFailedTest(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string testId)
        {
            RemoveNode(xmlDocument, nsMgr, $"//ns:TestRun/ns:Results/ns:UnitTestResult[@testId='{testId}']");
        }

        private void RemoveNode(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string xpath)
        {
            XmlNode node2Remove = xmlDocument.SelectSingleNode(xpath, nsMgr);

            if (node2Remove != null && node2Remove.ParentNode != null)
            {
                node2Remove.ParentNode.RemoveChild(node2Remove);
            }
            else
            {
                throw new Exception($"Cannot remove node because a relaited to xpath '{xpath}' node is not found ");
            }
        }

        private void SetAttribute(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string xpath, string attributeName, string attributeValue)
        {
            var selectedNode = (XmlElement)xmlDocument.SelectSingleNode(xpath, nsMgr);

            if (selectedNode != null)
            {
                selectedNode.SetAttribute(attributeName, attributeValue);
            }
            else
            {
                throw new Exception($"Cannot set attribute '{attributeName}' because a relaited to xpath '{xpath}' node is not found ");
            }
        }

        private string GetAttribute(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string xpath, string attributeName)
        {
            var selectedNode = (XmlElement)xmlDocument.SelectSingleNode(xpath, nsMgr);

            if (selectedNode != null)
            {
                return selectedNode.GetAttribute(attributeName);
            }

            throw new Exception($"Cannot get attribute '{attributeName}' because a relaited to xpath '{xpath}' node is not found ");
        }

        private string GetTestId(XmlDocument xmlDocument, XmlNamespaceManager nsMgr, string xpath, string className, string testName)
        {
            var selectedNodes = xmlDocument.SelectNodes(xpath, nsMgr);
            foreach (XmlElement selectedNode in selectedNodes)
            {
                var testMethodNode = selectedNode.SelectSingleNode("ns:TestMethod", nsMgr);
                if (testMethodNode.Attributes["className"].Value.StartsWith(className))
                {
                    return selectedNode.Attributes["id"].Value;
                }
            }

            throw new Exception($"Cannot find id for failed test '{testName}' in class '{className}'");
        }

        private void ParseFullFailedTestName(string fullFailedTestName, out string className, out string testName)
        {
            int lastPointIndex = fullFailedTestName.LastIndexOf(".");
            className = fullFailedTestName.Substring(0, lastPointIndex);
            testName = fullFailedTestName.Substring(lastPointIndex + 1);
        }
    }
}
