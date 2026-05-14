using System;
using Confirmit.CATI.Core.EmailReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.EmailReports
{
    [TestClass]
    public class SurveyOverviewReportBuilderTest : BaseReportBuilderTest
    {
        private SurveyOverviewReportBuilder _builder;
        private SurveyPersonStatisticsDatedDurationCollectionDataReport _expected;

        [TestInitialize]
        public void TestInitialize()
        {
            BaseInitialize();

            _builder = new SurveyOverviewReportBuilder(
                SurveyRepositoryStub,
                LocalTimeProviderStub,
                SystemSettingsStub);

            _expected = new SurveyPersonStatisticsDatedDurationCollectionDataReport
            {
                Title = "Forsta CATI Daily Survey Overview Export",
                Name = "Survey Overview",

                StartDate = new DateTime(2014, 5, 10, 15, 30, 5),
                EndDate = new DateTime(2014, 10, 10, 23, 0, 0),

                ReportDate = LocalTime,
                PersonNames = "All",
                SurveyNames = "All",

                DbSurveyIds = "100,200",
                DbPersonIds = null,
                DbStateIds = "13",
                DbShowDialerAttempts = true,
                DbHideEmpty = true,
                DbStartDate = StartTime,
                DbEndDate = EndTime
            };
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BuildReport_ProperParameters_ObjectOfProperTypeIsReturned()
        {
            var result = _builder.BuildReport(DateTime.Now, DateTime.Now);

            Assert.IsInstanceOfType(result, typeof(SurveyPersonStatisticsDatedDurationCollectionDataReport));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BuildReport_ProperParameters_ProperObjectIsReturned()
        {
            var result = (SurveyPersonStatisticsDatedDurationCollectionDataReport)_builder.BuildReport(StartTime, EndTime);

            AssertReport(_expected, result);
        }

        private void AssertReport(
            SurveyPersonStatisticsDatedDurationCollectionDataReport expected,
            SurveyPersonStatisticsDatedDurationCollectionDataReport actual)
        {
            Assert.AreEqual(expected.Title, actual.Title, "Wrong report title");
            Assert.AreEqual(expected.Name, actual.Name, "Wrong report name");
            Assert.AreEqual(expected.StartDate, actual.StartDate, "Wrong report caption start date");
            Assert.AreEqual(expected.EndDate, actual.EndDate, "Wrong report caption end date");
            Assert.AreEqual(expected.ReportDate, actual.ReportDate, "Wrong report date");
            Assert.AreEqual(expected.PersonNames, actual.PersonNames, "Wrong report caption person list");
            Assert.AreEqual(expected.SurveyNames, actual.SurveyNames, "Wrong report caption survey list");
            Assert.AreEqual(expected.DbSurveyIds, actual.DbSurveyIds, "Wrong DbSurveyIds param");
            Assert.AreEqual(expected.DbPersonIds, actual.DbPersonIds, "Wrong DbPersonIds param");
            Assert.AreEqual(expected.DbStateIds, actual.DbStateIds, "Wrong DbStateIds param");
            Assert.AreEqual(expected.DbShowDialerAttempts, actual.DbShowDialerAttempts, "Wrong DbShowDialerAttempts param");
            Assert.AreEqual(expected.DbHideEmpty, actual.DbHideEmpty, "Wrong DbHideEmpty param");
            Assert.AreEqual(expected.DbStartDate, actual.DbStartDate, "Wrong DbStartDate param");
            Assert.AreEqual(expected.DbEndDate, actual.DbEndDate, "Wrong DbEndDate param");
        }
    }
}
