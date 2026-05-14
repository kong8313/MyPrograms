using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.DatabaseTests;
using ConfirmitDialerInterface;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class QuotaProgressReportTest : BaseMockedIntegrationTest
    {
        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.QuotaProgressReport)]
        public void QuotaProgressReport_4CellsQuota_Day1Day7TargetDateHaveOneCompletedRecordForCells2And3_ResultsCorrect()
        {
            var targetDate = new DateTime(2017, 3, 30, 8, 8, 0);
            RunTest(targetDate);
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.QuotaProgressReport)]
        public void QuotaProgressReport_4CellsQuota_Day1Day7TargetDateHaveOneCompletedRecordForCells2And3_CurrentTimeIsAtTheEndOfDay_ResultsCorrect()
        {
            var targetDate = new DateTime(2017, 3, 30, 23, 0, 0);
        }

        private void RunTest(DateTime targetDate)
        {                      
            new DateTimeMocker(TestingFramework).MockDate(targetDate);
            var context = new TestData
            {
                Surveys = new[]{ 
                    new SurveyData { Tag="S1", IsUseDb = true,
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}},
                            new SingleFormData{Name="q2", Precodes = new []{"A", "B"}, SqlType = SqlDataType.Char},
                        },
                        Quotas = new[]{ 
                            new QuotaData(){ Id = 1, Name="Quota1", Fields = new[] {"q1", "q2"}, 
                                Cells = new[] 
                                {
                                    new CellData(){Id = 1, Values="q1=1,q2=A", Counter=0, Limit=10},
                                    new CellData(){Id = 2, Values="q1=1,q2=B", Counter=2, Limit=10},
                                    new CellData(){Id = 3, Values="q1=2,q2=A", Counter=3, Limit=10},
                                    new CellData(){Id = 4, Values="q1=2,q2=B", Counter=0, Limit=10},
                                }
                            }},
                        Interviews = new []
                        {
                            new InterviewData(){Tag="S1.I1", Data="q1=1,q2=A", ITS=CallOutcome.FreshSample},
                            new InterviewData()
                            {
                                Tag="S1.I1", Data="q1=1,q2=B", ITS=CallOutcome.Completed, 
                                CallHistory = new[]  { new CallHistoryData {Tag = "S1.C1", FiredTime = targetDate, Person = "P1", ITS=CallOutcome.Completed} }
                            },
                            new InterviewData()
                            {
                                Tag="S1.I1", Data="q1=1,q2=B", ITS=CallOutcome.Refusal, 
                                CallHistory = new[]  { new CallHistoryData {Tag = "S1.C1", FiredTime = targetDate.AddDays(-1), Person = "P1", ITS=CallOutcome.Refusal} }
                            },

                            new InterviewData()
                            {
                                Tag="S1.I1", Data="q1=1,q2=B", ITS=CallOutcome.Refusal, 
                                CallHistory = new[]  { new CallHistoryData {Tag = "S1.C1", FiredTime = targetDate.AddDays(-7), Person = "P1", ITS=CallOutcome.Refusal} }
                            },
                            
                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=A", ITS=CallOutcome.FreshSample},
                            new InterviewData()
                            {
                                Tag="S1.I1", Data="q1=2,q2=A", ITS=CallOutcome.Completed, 
                                CallHistory = new[]  { new CallHistoryData {Tag = "S1.C1", Person = "P1", FiredTime = targetDate, ITS=CallOutcome.Completed} }
                            },

                            new InterviewData()
                            {
                                Tag="S1.I1", Data="q1=2,q2=A", ITS=CallOutcome.Completed, 
                                CallHistory = new[]  { new CallHistoryData {Tag = "S1.C1", FiredTime = targetDate.AddDays(-1), Person = "P1", ITS=CallOutcome.Completed} }
                            },

                            new InterviewData()
                            {
                                Tag="S1.I1", Data="q1=2,q2=A", ITS=CallOutcome.Refusal, 
                                CallHistory = new[]  { new CallHistoryData {Tag = "S1.C1", FiredTime = targetDate.AddDays(-7), Person = "P1", ITS=CallOutcome.Refusal} }
                            },

                            new InterviewData(){Tag="S1.I1", Data="q1=2,q2=B", ITS=CallOutcome.FreshSample},
                       },                 
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1"} }
            }.Create();

            var surveyId = context.GetSurvey("S1").Id;

            var result = ExecuteReport(surveyId, "13,5", "Quota1", "q1*q2", targetDate);

            var reportItems = result.AsEnumerable().Select(item =>
                                    new
                                    {
                                        cells = item.Field<string>("_column0"),
                                        day1 = item.Field<int>("_column1"),
                                        day2 = item.Field<int>("_column2"),
                                        day3 = item.Field<int>("_column3"),
                                        day4 = item.Field<int>("_column4"),
                                        day5 = item.Field<int>("_column5"),
                                        day6 = item.Field<int>("_column6"),
                                        day7 = item.Field<int>("_column7"),
                                        avg7days = item.Field<decimal>("_column8"),
                                        targetDate = item.Field<int>("_column9"),
                                        progress = item.Field<string>("_column10"),
                                        EstimatedCompletion = item.Field<object>("_column11") == null ? (decimal?) null : item.Field<decimal>("_column11")
                                    }).ToArray();

            Assert.AreEqual(0, reportItems[0].day1, "Day1 for first cell should be equal to 0");

            Assert.AreEqual(1, reportItems[1].day1, "Day1 for second cell should be equal to 1");
            Assert.AreEqual(0, reportItems[1].day2, "Day2 for second cell should be equal to 0");
            Assert.AreEqual(1, reportItems[1].day7, "Day7 for second cell should be equal to 1");
            Assert.AreEqual(0.285714m, reportItems[1].avg7days, "Avg 7 days for second cell should be equal to 0.285714");
            Assert.AreEqual(1, reportItems[1].targetDate, "Target date for second cell should be equal to 1");
            Assert.AreEqual("2 of 10", reportItems[1].progress, "Pregress is not correct");
            Assert.AreEqual(28.000028000028000028m, reportItems[1].EstimatedCompletion, "Estimated completion is not coretct");

            Assert.AreEqual(1, reportItems[2].day1, "Day1 for third cell should be equal to 1");
            Assert.AreEqual(0, reportItems[2].day2, "Day2 for , third cell should be equal to 0");
            Assert.AreEqual(1, reportItems[2].day7, "Day7 for third cell should be equal to 1");
            Assert.AreEqual(0.285714m, reportItems[2].avg7days, "Avg 7 days for third cell should be equal to 0.285714");
            Assert.AreEqual(1, reportItems[2].targetDate, "Target date should be equal to 1");
            Assert.AreEqual("3 of 10", reportItems[2].progress, "Pregress is not correct");
            Assert.AreEqual(24.500024500024500024m, reportItems[2].EstimatedCompletion, "Estimated completion is not coretct");

            Assert.AreEqual(0, reportItems[3].day1, "Day1 for fourth cell should be equal to 0");

        }

        private DataTable ExecuteReport(int surveyId, string itsids, string quotaName, string quotaFields, DateTime targetDate)
        {
            var report = new QuotaProgressReport();
            report.ReportParameters["DbSurveyId"].Value = surveyId;
            report.ReportParameters["QuotaName"].Value = quotaName;
            report.ReportParameters["DbStateIds"].Value = itsids;
            report.ReportParameters["DbQuotaName"].Value = quotaName;
            report.ReportParameters["DbQuotaFields"].Value = quotaFields;
            report.ReportParameters["DbTargetDate"].Value = targetDate;

            return report.GetReportData();
        }
    }
}
