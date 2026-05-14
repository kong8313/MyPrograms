
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class SampleStatusSummaryByQuestionReportTest : BaseMockedIntegrationTest
    {

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleStatusSummaryByQuestionReport)]
        public void QuestionNotDefined_DoNotShowScheduledCalls_AllExtendedStatuses()
        {
            var context = TestDataContext(new[] {"1", "2"});

            var result = ExecuteReport(context.GetSurvey("S1").Id, null, null, null, null, false);

            var reportItems = result.AsEnumerable().Select(item =>
                new
                {
                    status = item.Field<string>("_column0"),
                    total = item.Field<int>("_column1")
                });

            Assert.AreEqual(2, result.Rows.Count, "Incorrect number of records");
            Assert.AreEqual("Completed", reportItems.First().status );
            Assert.AreEqual(2, reportItems.First().total);

            Assert.AreEqual("Fresh sample", reportItems.Last().status);
            Assert.AreEqual(3, reportItems.Last().total);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleStatusSummaryByQuestionReport)]
        public void QuestionDefinedWith2Answers_DoNotShowScheduledCalls_AllExtendedStatuses()
        {
            var context = TestDataContext(new[] {"1", "2"});

            var result = ExecuteReport(context.GetSurvey("S1").Id, null, "q1", "1,2", "_column4,_column5", false);

            var reportItems = result.AsEnumerable().Select(item =>
                new
                {
                    status = item.Field<string>("_column0"),
                    total = item.Field<int>("_column1"),
                    undefined = item.Field<int>("_column3"),
                    precode1 = item.Field<int>("_column4"),
                    precode2 = item.Field<int>("_column5")
                });

            Assert.AreEqual("Completed", reportItems.First().status);
            Assert.AreEqual(2, reportItems.First().total);
            Assert.AreEqual(0, reportItems.First().undefined);
            Assert.AreEqual(2, reportItems.First().precode1);
            Assert.AreEqual(0, reportItems.First().precode2);
            
            Assert.AreEqual("Fresh sample", reportItems.Last().status);
            Assert.AreEqual(3, reportItems.Last().total);
            Assert.AreEqual(1, reportItems.Last().undefined);
            Assert.AreEqual(1, reportItems.Last().precode1);
            Assert.AreEqual(1, reportItems.Last().precode2);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleStatusSummaryByQuestionReport)]
        public void QuestionDefinedWith2Answers_DoNotShowScheduledCalls_OnlyFreshSampleExtendedStatus()
        {
            var context = TestDataContext(new[] { "1", "2" });

            var result = ExecuteReport(context.GetSurvey("S1").Id, "16", "q1", "1,2", "_column4,_column5", false);

            var reportItems = result.AsEnumerable().Select(item =>
                new
                {
                    status = item.Field<string>("_column0"),
                    total = item.Field<int>("_column1"),
                    undefined = item.Field<int>("_column3"),
                    precode1 = item.Field<int>("_column4"),
                    precode2 = item.Field<int>("_column5")
                });

            Assert.AreEqual(1, result.Rows.Count, "Incorrect number of records");
            Assert.AreEqual("Fresh sample", reportItems.First().status);
            Assert.AreEqual(3, reportItems.Last().total);
            Assert.AreEqual(1, reportItems.Last().undefined);
            Assert.AreEqual(1, reportItems.Last().precode1);
            Assert.AreEqual(1, reportItems.Last().precode2);
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleStatusSummaryByQuestionReport)]
        public void QuestionDefinedWith2Answers_ShowScheduledCalls_AllExtendedStatuses()
        {
            var context = TestDataContext(new[] { "1", "2" });

            var result = ExecuteReport(context.GetSurvey("S1").Id, null, "q1", "1,2", "_column4,_column5", true);

            var reportItems = result.AsEnumerable().Select(item =>
                new
                {
                    status = item.Field<string>("_column0"),
                    total = item.Field<string>("_column1"),
                    undefined = item.Field<string>("_column3"),
                    precode1 = item.Field<string>("_column4"),
                    precode2 = item.Field<string>("_column5")
                });

            Assert.AreEqual("Completed", reportItems.First().status);
            Assert.AreEqual("2 (0)", reportItems.First().total);
            Assert.AreEqual("0 (0)", reportItems.First().undefined);
            Assert.AreEqual("2 (0)", reportItems.First().precode1);
            Assert.AreEqual("0 (0)", reportItems.First().precode2);

            Assert.AreEqual("Fresh sample", reportItems.Last().status);
            Assert.AreEqual("3 (3)", reportItems.Last().total);
            Assert.AreEqual("1 (1)", reportItems.Last().undefined, "Undefined count for Fresh sample is not correct");
            Assert.AreEqual("1 (1)", reportItems.Last().precode1, "Precode1 count for Fresh sample is not correct");
            Assert.AreEqual("1 (1)", reportItems.Last().precode2, "Precode2 count for Fresh sample is not correct");
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleStatusSummaryByQuestionReport)]
        public void QuestionDefinedWith2Answers_QuestionWithCharPrecodes_DoNotShowScheduledCalls_AllExtendedStatuses()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"M", "F"}, SqlType = SqlDataType.Char}
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=M", ITS = CallOutcome.Completed},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=M", ITS = CallOutcome.Completed},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=M", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=F", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData() {Tag = "S1.I5", ITS = CallOutcome.FreshSample, Call = new CallData()},
                        }
                    }
                }
            }.Create();

            var result = ExecuteReport(context.GetSurvey("S1").Id, null, "q1", "'M','F'", "_column4,_column5", false);

            var reportItems = result.AsEnumerable().Select(item =>
                new
                {
                    status = item.Field<string>("_column0"),
                    total = item.Field<int>("_column1"),
                    undefined = item.Field<int>("_column3"),
                    precode1 = item.Field<int>("_column4"),
                    precode2 = item.Field<int>("_column5")
                });

            Assert.AreEqual("Completed", reportItems.First().status);
            Assert.AreEqual(2, reportItems.First().total);
            Assert.AreEqual(0, reportItems.First().undefined);
            Assert.AreEqual(2, reportItems.First().precode1);
            Assert.AreEqual(0, reportItems.First().precode2);

            Assert.AreEqual("Fresh sample", reportItems.Last().status);
            Assert.AreEqual(3, reportItems.Last().total);
            Assert.AreEqual(1, reportItems.Last().undefined, "Undefined count for Fresh sample is not correct");
            Assert.AreEqual(1, reportItems.Last().precode1, "Precode1 count for Fresh sample is not correct");
            Assert.AreEqual(1, reportItems.Last().precode2, "Precode2 count for Fresh sample is not correct");
        }

        /// <summary>
        /// https://jiraosl.firmglobal.com/browse/CATI-2560
        /// </summary>
        [TestMethod, Owner(@"FIRM\EvgeniySu"), TestCategory(TestsCategoriesNames.SampleStatusSummaryByQuestionReport)]
        public void BuildReport_CorrectReportIfUseQuestionNameAsColumnNameFromBvSurveyTable()
        {
            var context = new TestData
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Forms = new FormData[]
                        {
                            new SingleFormData {Name = "Target"}
                        },
                        Interviews = new[]
                        {
                            new InterviewData {Tag = "S1.I1", Data = "Target=1", ITS = CallOutcome.Completed},
                            new InterviewData {Tag = "S1.I2", Data = "Target=2", ITS = CallOutcome.Completed},
                            new InterviewData {Tag = "S1.I3", Data = "Target=3", ITS = CallOutcome.FreshSample}
                        }
                    }
                }
            }.Create();

            var report = ExecuteReport(context.GetSurvey("S1").Id, null, "Target", null, null, false);

            var reportItems = report.AsEnumerable().Select(item =>
                new
                {
                    status = item.Field<string>("_column0"),
                    total = item.Field<int>("_column1")
                });

            Assert.AreEqual(2, report.Rows.Count, "Incorrect number of records");
            Assert.AreEqual("Completed", reportItems.First().status);
            Assert.AreEqual(2, reportItems.First().total);

            Assert.AreEqual("Fresh sample", reportItems.Last().status);
            Assert.AreEqual(1, reportItems.Last().total);
        }

        private TestDataContext TestDataContext(string[] precodes)
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        IsUseDb = true,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = precodes}
                        },
                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1", ITS = CallOutcome.Completed},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=1", ITS = CallOutcome.Completed},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=1", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=2", ITS = CallOutcome.FreshSample, Call = new CallData()},
                            new InterviewData() {Tag = "S1.I5", ITS = CallOutcome.FreshSample, Call = new CallData()},
                        },
                    }
                },
            }.Create();
            return context;
        }

        private DataTable ExecuteReport(int surveyId, string itsids, string questionId, string precodes, string answerTexts, bool showScheduled)
        {
            return new DatabaseEngine().ExecuteDataTable<DataTable>(
                "BvSpSampleStatusSummaryByQuestionReport", CommandType.StoredProcedure,
                new SqlParameter("@SurveyId", surveyId),
                new SqlParameter("@ITSIDs", itsids ?? (object) DBNull.Value),
                new SqlParameter("@QuestionId", questionId ?? (object)DBNull.Value),
                new SqlParameter("@Precodes", precodes ?? (object)DBNull.Value),
                new SqlParameter("@AnswerTexts", answerTexts ?? (object)DBNull.Value),
                new SqlParameter("@ShowScheduled", showScheduled));
        }
    }
}
