using System.Collections.Generic;
using DialerWsLogParserLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialerWsLogParserTest
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void SetColumnsFilter()
        {
            var settings = new Settings();

            settings.SetColumnsFilter("Name", "2019-06-25 09:26:51.883", "2019-06-25 09:26:51.885", "CompanyId", "DialerId", "CampaignId",
                "AgentId", "CallId", "InterviewId", "Duration", "AllInfo");

            Assert.AreEqual(settings.Filter.Name, "Name");
            Assert.AreEqual(settings.Filter.StartTime, "2019-06-25 09:26:51.883");
            Assert.AreEqual(settings.Filter.FinishTime, "2019-06-25 09:26:51.885");
            Assert.AreEqual(settings.Filter.CompanyId, "CompanyId");
            Assert.AreEqual(settings.Filter.DialerId, "DialerId");
            Assert.AreEqual(settings.Filter.CampaignId, "CampaignId");
            Assert.AreEqual(settings.Filter.AgentId, "AgentId");
            Assert.AreEqual(settings.Filter.CallId, "CallId");
            Assert.AreEqual(settings.Filter.InterviewId, "InterviewId");
            Assert.AreEqual(settings.Filter.Duration, "Duration");
            Assert.AreEqual(settings.Filter.AllInfo, "AllInfo");
        }

        [TestMethod]
        public void SetColumnsVisibility()
        {
            var settings = new Settings();
            settings.SetColumnsVisibility(false, true, false, true, false, true, false, true, false);

            Assert.AreEqual(settings.ColumnHandler.StartTime, false);
            Assert.AreEqual(settings.ColumnHandler.FinishTime, true);
            Assert.AreEqual(settings.ColumnHandler.CompanyId, false);
            Assert.AreEqual(settings.ColumnHandler.DialerId, true);
            Assert.AreEqual(settings.ColumnHandler.CampaignId, false);
            Assert.AreEqual(settings.ColumnHandler.AgentId, true);
            Assert.AreEqual(settings.ColumnHandler.CallId, false);
            Assert.AreEqual(settings.ColumnHandler.InterviewId, true);
            Assert.AreEqual(settings.ColumnHandler.Duration, false);
        }

        [TestMethod]
        public void SetRecentFiles()
        {
            var settings = new Settings();

            var files = new List<string>() { "file1", "file2", "file3" };
            settings.SetRecentFiles(files);

            CollectionAssert.AreEqual(settings.RecentFiles, files);
        }

        [TestMethod]
        public void IsParametersMatchCondition_True()
        {
            var settings = new Settings();

            settings.SetColumnsFilter("Name", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            EventsGroup group = new EventsGroup(0, "Name", "2019-06-25 09:26:51.883", "2019-06-25 09:26:51.885");
            Assert.AreEqual(settings.IsParametersMatchCondition(group), true);
        }

        [TestMethod]
        public void IsParametersMatchCondition_False()
        {
            var settings = new Settings();

            settings.SetColumnsFilter("Name1", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            EventsGroup group = new EventsGroup(0, "Name", "2019-06-25 09:26:51.883", "2019-06-25 09:26:51.885");
            Assert.AreEqual(settings.IsParametersMatchCondition(group), false);
        }

        [TestMethod]
        public void SetConditionalOperatorAnd()
        {
            var settings = new Settings();

            settings.SetConditionalOperatorAnd();
            Assert.AreEqual(settings.IsConditionalOperatorAnd, true);
        }

        [TestMethod]
        public void SetConditionalOperatorOr()
        {
            var settings = new Settings();

            settings.SetConditionalOperatorOr();
            Assert.AreEqual(settings.IsConditionalOperatorAnd, false);
        }

        [TestMethod]
        public void SetCoincidenceOperatorPos()
        {
            var settings = new Settings();

            settings.SetCoincidenceOperatorPos();
            Assert.AreEqual(settings.IsCoincidenceOperatorPositive, true);
        }

        [TestMethod]
        public void SetCoincidenceOperatorNeg()
        {
            var settings = new Settings();

            settings.SetCoincidenceOperatorNeg();
            Assert.AreEqual(settings.IsCoincidenceOperatorPositive, false);
        }
    }
}
