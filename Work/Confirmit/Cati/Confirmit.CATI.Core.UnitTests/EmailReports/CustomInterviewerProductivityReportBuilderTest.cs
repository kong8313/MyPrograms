using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Framework.Interfaces.Fakes;
using Confirmit.CATI.Core.EmailReports;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.Fakes;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.Core.UnitTests.EmailReports
{
    [TestClass]
    public class CustomInterviewerProductivityReportBuilderTest : BaseReportBuilderTest
    {
        private CustomInterviewerProductivityReportBuilder _builder;
        private CustomInterviewerProductivityReport _expected;

        [TestInitialize]
        public void TestInitialize()
        {
            BaseInitialize();

            ServiceLocator.StaticCleanup();
            ServiceLocator.StaticInitialize();
            var databaseEngineFactoryStub = new StubIDatabaseEngineFactory();
            var companyInfoStub = new StubICompanyInfo();
            ServiceLocator.RegisterInstance<IDatabaseEngineFactory>(databaseEngineFactoryStub);
            ServiceLocator.RegisterInstance<ICompanyInfo>(companyInfoStub);
            ServiceLocator.RegisterInstance<ISystemSettings>(new StubISystemSettings());
            ServiceLocator.RegisterInstance<IConnectionStrings>(new StubIConnectionStrings());

            _builder = new CustomInterviewerProductivityReportBuilder(
                SurveyRepositoryStub,
                LocalTimeProviderStub,
                SystemSettingsStub,
                SupervisorApiClientStub);

            _expected = new CustomInterviewerProductivityReport
            {
                Title = "Forsta CATI Daily Interviewer Productivity Export",
                Name = "Interviewer Productivity",

                StartDate = StartTime.AddHours(TimezoneHoursOffset),
                EndDate = EndTime.AddHours(TimezoneHoursOffset),

                ReportDate = LocalTime,
                PersonNames = "All",
                SurveyNames = "All",

                IncludeBreaksInAverages = false,

                DbSurveyIds = "100,200",
                DbPersonIds = null,
                DbStateIds = "13",
                DbShowDialerAttempts = true,
                DbCalcAllBreakHistory = true,
                DbHideEmpty = true,
                DbStartDate = StartTime,
                DbEndDate = EndTime
            };
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BuildReport_ProperParameters_ObjectOfProperTypeIsReturned()
        {
            var result = _builder.BuildReport(DateTime.Now, DateTime.Now);

            Assert.IsInstanceOfType(result, typeof(CustomInterviewerProductivityReport));
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void BuildReport_ProperParameters_ProperObjectIsReturned()
        {
            var result = (CustomInterviewerProductivityReport)_builder.BuildReport(StartTime, EndTime);

            AssertReport(_expected, result);
        }

        private void AssertReport(CustomInterviewerProductivityReport expected, CustomInterviewerProductivityReport actual)
        {
            Assert.AreEqual(expected.Title, actual.Title, "Wrong report title");
            Assert.AreEqual(expected.Name, actual.Name, "Wrong report name");
            Assert.AreEqual(expected.StartDate, actual.StartDate, "Wrong report caption start date");
            Assert.AreEqual(expected.EndDate, actual.EndDate, "Wrong report caption end date");
            Assert.AreEqual(expected.ReportDate, actual.ReportDate, "Wrong report date");
            Assert.AreEqual(expected.PersonNames, actual.PersonNames, "Wrong report caption person list");
            Assert.AreEqual(expected.SurveyNames, actual.SurveyNames, "Wrong report caption survey list");
            Assert.AreEqual(expected.IncludeBreaksInAverages, actual.IncludeBreaksInAverages, "Wrong IncludeBreaksInAverages param");
            Assert.AreEqual(expected.DbSurveyIds, actual.DbSurveyIds, "Wrong DbSurveyIds param");
            Assert.AreEqual(expected.DbPersonIds, actual.DbPersonIds, "Wrong DbPersonIds param");
            Assert.AreEqual(expected.DbStateIds, actual.DbStateIds, "Wrong DbStateIds param");
            Assert.AreEqual(expected.DbShowDialerAttempts, actual.DbShowDialerAttempts, "Wrong DbShowDialerAttempts param");
            Assert.AreEqual(expected.DbHideEmpty, actual.DbHideEmpty, "Wrong DbHideEmpty param");
            Assert.AreEqual(expected.DbStartDate, actual.DbStartDate, "Wrong DbStartDate param");
            Assert.AreEqual(expected.DbEndDate, actual.DbEndDate, "Wrong DbEndDate param");
            Assert.AreEqual(expected.DbCalcAllBreakHistory, actual.DbCalcAllBreakHistory, "Wrong DbCalcAllBreakHistory param");
        }
    }
}
