using System;
using Confirmit.CATI.Core.EmailReports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.EmailReports
{
    [TestClass]
    public class SurveyProductivityReportBuilderTest : BaseReportBuilderTest
    {
        private SurveyProductivityReportBuilder _builder;
        private SurveyProductivityReport _expected;

        [TestInitialize]
        public void TestInitialize()
        {
            BaseInitialize();

            _builder = new SurveyProductivityReportBuilder(
                SurveyRepositoryStub,
                LocalTimeProviderStub,
                SystemSettingsStub);

            _expected = new SurveyProductivityReport
            {
                Title = "Forsta CATI Daily Survey Productivity Export",
                Name = "Survey Productivity",

                StartDate = StartTime.AddHours(TimezoneHoursOffset),
                EndDate = EndTime.AddHours(TimezoneHoursOffset),

                ReportDate = LocalTime,
                PersonNames = "All",
                SurveyNames = "All",

                ITSNames = "All",
                IncludePercentage = false,

                DbSurveyIds = "100,200",
                DbPersonIds = null,
                DbStateIds = null,
                DbStartDate = StartTime,
                DbEndDate = EndTime
            };
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BuildReport_ProperParameters_ObjectOfProperTypeIsReturned()
        {
            var result = _builder.BuildReport(DateTime.Now, DateTime.Now);

            Assert.IsInstanceOfType(result, typeof(SurveyProductivityReport));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BuildReport_ProperParameters_ProperObjectIsReturned()
        {
            var result = (SurveyProductivityReport)_builder.BuildReport(StartTime, EndTime);

            AssertReport(_expected, result);
        }

        private void AssertReport(SurveyProductivityReport expected, SurveyProductivityReport actual)
        {
            Assert.AreEqual(expected.Title, actual.Title, "Wrong report title");
            Assert.AreEqual(expected.Name, actual.Name, "Wrong report name");
            Assert.AreEqual(expected.StartDate, actual.StartDate, "Wrong report caption start date");
            Assert.AreEqual(expected.EndDate, actual.EndDate, "Wrong report caption end date");
            Assert.AreEqual(expected.ReportDate, actual.ReportDate, "Wrong report date");
            Assert.AreEqual(expected.PersonNames, actual.PersonNames, "Wrong report caption person list");
            Assert.AreEqual(expected.SurveyNames, actual.SurveyNames, "Wrong report caption survey list");
            Assert.AreEqual(expected.ITSNames, actual.ITSNames, "Wrong ITSNames param");
            Assert.AreEqual(expected.IncludePercentage, actual.IncludePercentage, "Wrong IncludePercentage param");
            Assert.AreEqual(expected.DbSurveyIds, actual.DbSurveyIds, "Wrong DbSurveyIds param");
            Assert.AreEqual(expected.DbPersonIds, actual.DbPersonIds, "Wrong DbPersonIds param");
            Assert.AreEqual(expected.DbStateIds, actual.DbStateIds, "Wrong DbStateIds param");
            Assert.AreEqual(expected.DbStartDate, actual.DbStartDate, "Wrong DbStartDate param");
            Assert.AreEqual(expected.DbEndDate, actual.DbEndDate, "Wrong DbEndDate param");
        }
    }
}
