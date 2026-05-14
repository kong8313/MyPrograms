using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports.CustomInterviewerProductivityReport;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Core.SystemSettings;
using Telerik.Reporting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class InterviewerProductivityReportDataProviderTest
    {
        const string UserName = "testUser";
        const string Password = "password";
        const string ExtensionNumber = "101010";

        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private BvBreakTypeEntity _breakTypePaid;
        private BvBreakTypeEntity _breakTypeUnpaid;
        private InterviewerProductivityReportDataProvider _interviewerProductivityReportDataProvider;
        private InterviewerProductivityReportTemplate _defaultTemplate;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _backendTools = new BackendTools(_framework);

            _interviewerProductivityReportDataProvider = new InterviewerProductivityReportDataProvider();

            var breakTypeRepository = ServiceLocator.Resolve<IBreakTypeRepository>();

            breakTypeRepository.Insert(new BvBreakTypeEntity { Name = "InterviewerProductivityTestPaid", IsPaid = true });
            breakTypeRepository.Insert(new BvBreakTypeEntity { Name = "InterviewerProductivityTestUnpaid", IsPaid = false });
            _breakTypePaid = breakTypeRepository.GetAll().First(x => x.Name.Equals("InterviewerProductivityTestPaid"));
            _breakTypeUnpaid = breakTypeRepository.GetAll().First(x => x.Name.Equals("InterviewerProductivityTestUnpaid"));

            _defaultTemplate = MakeDefaultTemplate();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        private InterviewerProductivityReportTemplate MakeDefaultTemplate()
        {
            const string columnData = @"<Columns xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><ProductivityReportTemplateColumn><DisplayName>User ID</DisplayName><StandardColumnName>PersonId</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>User name</DisplayName><StandardColumnName>PersonName</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Log on time (hours)</DisplayName><StandardColumnName>LogOnHours</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Waiting time (hours)</DisplayName><StandardColumnName>WaitingHours</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Paid break time (hours)</DisplayName><StandardColumnName>BreakHoursPaid</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Unpaid break time (hours)</DisplayName><StandardColumnName>BreakHoursUnpaid</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Review Time (hours)</DisplayName><StandardColumnName>OpenEndReviewHours</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn xsi:type=""ProductivityReportTemplateColumnWithStatuses""><DisplayName>Interviews</DisplayName><StandardColumnName>DialingsCount</StandardColumnName><Visible>true</Visible><IsIncludeStatuses>false</IsIncludeStatuses><ExtendedStatuses /></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Interviews per log on hour</DisplayName><StandardColumnName>DialingsPerLogOnHours</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn xsi:type=""ProductivityReportTemplateColumnWithStatuses""><DisplayName>Completes</DisplayName><StandardColumnName>Completes</StandardColumnName><Visible>true</Visible><IsIncludeStatuses>true</IsIncludeStatuses><ExtendedStatuses><int>13</int></ExtendedStatuses></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Completes per log on hour</DisplayName><StandardColumnName>CompletesPerLogOnHours</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Interviews per complete</DisplayName><StandardColumnName>DialingsPerComplete</StandardColumnName></ProductivityReportTemplateColumn><ProductivityReportTemplateColumn><DisplayName>Average completed interview length (min)</DisplayName><StandardColumnName>AverageDuration</StandardColumnName></ProductivityReportTemplateColumn></Columns>";
            var serializer = new XmlSerializer(typeof(List<ProductivityReportTemplateColumn>), new XmlRootAttribute("Columns"));
            var stringReader = new StringReader(columnData);

            return new InterviewerProductivityReportTemplate()
            {
                Id = 1,
                Name = "System template",
                CreatorName = "System",
                CreatorLogin = "system",
                DateCreated = new DateTime(2019, 1, 1),
                IncludeBreakTimeInCalculations = false,
                IncludeZeroValues = false,
                IsDefault = true,
                IsPortrait = false,
                AccessType = 2,
                LastModified = new DateTime(2019, 1, 1),
                ShowDialerAttempts = false,
                Columns = (List<ProductivityReportTemplateColumn>)serializer.Deserialize(stringReader)
            };
        }

        

        private DataTable GetData(
            InterviewerProductivityReportTemplate template,
            string dbSurveyIds,
            string dbPersonIds = null,
            bool dbShowDialerAttempts = false,
            bool dbHideEmpty = true,
            string dbStartDate = null,
            string dbEndDate = null,
            bool dbCalcAllBreakHistory = true,
            string dbSurveyDataFilter = null,
            string dbStartShiftTime = null,
            string dbEndShiftTime = null,
            string surveyNames = "",
            int? callCenterId = null)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection()
            {
                new ReportParameter("SurveyNames", ReportParameterType.String, surveyNames),
                new ReportParameter("DbSurveyIds", ReportParameterType.String, dbSurveyIds),
                new ReportParameter("DbPersonIds", ReportParameterType.String, dbPersonIds),
                new ReportParameter("DbShowDialerAttempts", ReportParameterType.Boolean, dbShowDialerAttempts),
                new ReportParameter("DbHideEmpty", ReportParameterType.Boolean, dbHideEmpty),
                new ReportParameter("DbStartDate", ReportParameterType.DateTime, dbStartDate),
                new ReportParameter("DbEndDate", ReportParameterType.DateTime, dbEndDate),
                new ReportParameter("DbCalcAllBreakHistory", ReportParameterType.Boolean, dbCalcAllBreakHistory),
                new ReportParameter("DbSurveyDataFilter", ReportParameterType.String, dbSurveyDataFilter),
                new ReportParameter("DbStartShiftTime", ReportParameterType.DateTime, dbStartShiftTime),
                new ReportParameter("DbEndShiftTime", ReportParameterType.DateTime, dbEndShiftTime),
                new ReportParameter("DbCallCenterId", ReportParameterType.Integer, callCenterId)
            };

            return _interviewerProductivityReportDataProvider.GetData(template, reportParameters, out bool hasRecords);
        }

        private string GetValueFromDataTable(DataTable table, string columnName)
        {
            return table.Rows[0][columnName].ToString();
        }

        private void CompareDataTables(DataTable expected, DataTable actual, bool skipWaitingTime = true)
        {
            Assert.AreEqual(expected.Rows.Count, actual.Rows.Count);
            for (int i = 0; i < expected.Rows.Count; i++)
            {
                DataRow expectedRow = expected.Rows[i];
                DataRow actualRow = actual.Rows[i];
                Assert.AreEqual(expectedRow.ItemArray.Length, actualRow.ItemArray.Length);
                
                for (int j = 0; j < expectedRow.ItemArray.Length; j++)
                {
                    if (j == 9 && skipWaitingTime) /*skip WaitingTime parameter because it can be different*/
                    {
                        continue;
                    }

                    Assert.AreEqual(expectedRow[j].ToString(), actualRow[j].ToString(), $"Element {j} is different");
                }
            }
        }

        private DataTable GetDefaultResultTable()
        {
            var table = new DataTable();
            table.Columns.Add("PersonId");
            table.Columns.Add("PersonName");
            table.Columns.Add("DisplayName");
            table.Columns.Add("Attribute1");
            table.Columns.Add("Attribute2");
            table.Columns.Add("Attribute3");
            table.Columns.Add("Attribute4");
            table.Columns.Add("Attribute5");
            table.Columns.Add("LogOnTime");
            table.Columns.Add("WaitingTime");
            table.Columns.Add("OnBreakTimePaid");
            table.Columns.Add("OnBreakTimeUnpaid");
            table.Columns.Add("DialingsCount");
            table.Columns.Add("Completes");
            table.Columns.Add("AverageCompletedInterviewDuration");
            table.Columns.Add("OpenEndReviewDuration");
            table.Columns.Add("PreviewDuration");
            table.Columns.Add("WrapDuration");
            table.Columns.Add("ConnectedDuration");
            table.Columns.Add("InterviewDuration");

            return table;
        }

        private BvInterviewEntity CreateSurveyWithTwoCompletedInterviews(TestCati2 test, out DateTime startTime1,
            out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1,
            out DateTime timeCallDelivered2, out DateTime completedTime2)
        {
            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);


            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            startTime1 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            Thread.Sleep(1 * 1000);

            timeCallDelivered1 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;
            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvTask(after first call delivery)",
                "select * from BvTasks");

            int initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            completedTime1 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            interview = test.CompleteInterviewAndWaitNext_Preview(interview);
            Thread.Sleep(2 * 1000);

            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvTask(after second call delivery)",
                "select * from BvTasks");

            startTime2 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            timeCallDelivered2 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            completedTime2 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            test.CompleteInterviewAndWaitNext_Preview(interview);

            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvTask(after second complete)",
                "select * from BvTasks");

            return interview;
        }

        private void CreateThreeHistoryRecordsOneBreakRecord(int surveyId, int personId, DateTime now)
        {
            _backendTools.CreateHistoryRecords(surveyId, personId, new[] { now.AddMinutes(-360) }, 1, 100, 5);
            _backendTools.CreateHistoryRecords(surveyId, personId, new[] { now.AddMinutes(-240) }, 2, 200, 10);
            _backendTools.CreateHistoryRecords(surveyId, personId, new[] { now.AddMinutes(-120) }, 3, 300, 15);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity
            {
                Duration = 16 * 60,
                InterviewerId = personId,
                StartTime = now.AddMinutes(-(240 + 8)),
                SurveyId = surveyId
            };

            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterviewAndLogout_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);
            
            PersonTools.UpdateAttributesAndFullName(
                test.PersonSID, "Display name", new[] { "Attribute 1", "Attribute2", "Attribute3", "Attribute4", "Attribute5" });
                
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(2));

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(4));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;

            test.CompleteInterviewWithLogout_Progressive(interview);

            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "Display name",                                         /*DisplayName*/
                "Attribute 1",                                          /*Attribute1*/
                "Attribute2",                                          /*Attribute2*/
                "Attribute3",                                          /*Attribute3*/
                "Attribute4",                                          /*Attribute4*/
                "Attribute5",                                          /*Attribute5*/
                TimeDiff.Seconds(startTime, timeCallDelivered) +
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                0,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*AverageCompletedInterviewDuration*/
                0,                                                      /*OpenEndReviewDuration*/
                0,                                                      /*PreviewDuration*/
                0,                                                      /*WrapDuration*/
                0,                                                      /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),/*InterviewDuration*/
                1,                                                      /*DialingsCount*/
                1);                                                     /*Completes*/

            CompareDataTables(expected, actual);
        }
        
        [TestMethod, Owner(@"FIRM\egork"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_IncludeOpenEndReviewTimeInInterviewDurationIsEnabled_ReportIsCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = true;
            
            var currentUtcTime = DateTime.UtcNow;
            currentUtcTime.AddMilliseconds(-currentUtcTime.Millisecond);
            new DateTimeMocker(_framework).MockDate(currentUtcTime);
            
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic, openEndReview: 1);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).AddTime(new TimeSpan(0,0,10));
            
            test.GetFroceOpenEndReview();
            var OpenEndReviewStart = TaskRepository.GetByPerson(test.PersonSID).OpenEndReviewStartTime.Value;
            new DateTimeMocker(_framework).AddTime(new TimeSpan(0,0,5));
            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;

            test.CompleteInterviewWithLogout_Progressive(interview);
          
            
            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "",                                                     /*DisplayName*/
                null,                                                   /*Attribute1*/
                null,                                                   /*Attribute2*/
                null,                                                   /*Attribute3*/
                null,                                                   /*Attribute4*/
                null,                                                   /*Attribute5*/
                TimeDiff.Seconds(startTime, timeCallDelivered) +
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                0,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*AverageCompletedInterviewDuration*/
                TimeDiff.Seconds(OpenEndReviewStart, completedTime),  /*OpenEndReviewDuration*/
                0,                                                          /*PreviewDuration*/
                0,                                                          /*WrapDuration*/
                0,                                                          /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),/*InterviewDuration*/
                1,                                                      /*DialingsCount*/
                1);                                                     /*Completes*/

            CompareDataTables(expected, actual);
        }
        
          [TestMethod, Owner(@"FIRM\egork"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_IncludeOpenEndReviewTimeInInterviewDurationIsDisabled_ReportIsCorrect()
        {
            ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration = false;
            
            var currentUtcTime = DateTime.UtcNow;
            currentUtcTime.AddMilliseconds(-currentUtcTime.Millisecond);
            new DateTimeMocker(_framework).MockDate(currentUtcTime);
            
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic, openEndReview: 1);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).AddTime(new TimeSpan(0,0,10));
            
            test.GetFroceOpenEndReview();
            var OpenEndReviewStart = TaskRepository.GetByPerson(test.PersonSID).OpenEndReviewStartTime.Value;
            new DateTimeMocker(_framework).AddTime(new TimeSpan(0,0,5));
            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;

            test.CompleteInterviewWithLogout_Progressive(interview);
          
            
            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                             /*PersonId*/
                UserName,                                                   /*PersonName*/
                "",                                                         /*DisplayName*/
                null,                                                       /*Attribute1*/
                null,                                                       /*Attribute2*/
                null,                                                       /*Attribute3*/
                null,                                                       /*Attribute4*/
                null,                                                       /*Attribute5*/
                TimeDiff.Seconds(startTime, timeCallDelivered) +
                TimeDiff.Seconds(timeCallDelivered, completedTime)
                +TimeDiff.Seconds(OpenEndReviewStart, completedTime),  /*LogOnTime*/
                null,                                                       /*WaitingTime*/
                0,                                                          /*OnBreakTimePaid*/
                0,                                                          /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),/*AverageCompletedInterviewDuration*/
                TimeDiff.Seconds(OpenEndReviewStart, completedTime),   /*OpenEndReviewDuration*/
                0,                                                          /*PreviewDuration*/
                0,                                                          /*WrapDuration*/
                0,                                                          /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),/*InterviewDuration*/
                1,                                                          /*DialingsCount*/
                1);                                                         /*Completes*/

            CompareDataTables(expected, actual);
        }
        
        private void UpdateLogOnTime(DateTime login, DateTime logout, int interviewerId)
        {
            var sql = @"
            UPDATE CatiInterviewerSessionHistory 
            SET [LoginTime] = @LgoInTime, [LogoutTime] = @LogOutTime
            WHERE [InterviewerId] = @InterviewerId";

            var parameters = new SqlParameter[] {
                new SqlParameter("InterviewerId", interviewerId),
                new SqlParameter("LgoInTime", login),
                new SqlParameter("LogOutTime", logout)
            };

            _framework.DbEngine.ExecuteDataTable<DataTable>(sql, CommandType.Text, parameters.ToArray());
        }

        private void CreateSurveyWithTwoCompletedInterviews_AndMockedTime(TestCati2 test, out DateTime startTime1,
           out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1,
           out DateTime timeCallDelivered2, out DateTime completedTime2)
        {
            var currentUtcTime = DateTime.UtcNow;
            currentUtcTime = currentUtcTime.AddMilliseconds(-currentUtcTime.Millisecond);
            new DateTimeMocker(_framework).MockDate(currentUtcTime);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);

            startTime1 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            new DateTimeMocker(_framework).AddTime(new TimeSpan(0, 0, 2));

            timeCallDelivered1 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            int initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            new DateTimeMocker(_framework).AddTime(new TimeSpan(0, 0, 8));
            interview = test.CompleteInterviewAndWaitNext_Preview(interview);
            completedTime1 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;

            startTime2 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            new DateTimeMocker(_framework).AddTime(new TimeSpan(0, 0, 2));
            timeCallDelivered2 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).AddTime(new TimeSpan(0, 0, 18));
            test.CompleteInterviewAndWaitNext_Preview(interview);
            completedTime2 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
        }

        [TestMethod, Owner(@"FIRM\egorK"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterviewAndLogout_LogOnDataTakenFromSessions()
        {
            PrepareSessionDatabase();

            var test = new TestCati2(true, false, _backendTools);

            CreateSurveyWithTwoCompletedInterviews_AndMockedTime(test, out DateTime startTime1, out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1, out DateTime timeCallDelivered2, out DateTime completedTime2);

            UpdateLogOnTime(startTime1, completedTime2, test.PersonSID);

            //include whole interview
            var dbStartDate = startTime1.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            var dbEndtDate = completedTime2.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            var actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);
            var expected = GetDefaultResultTable();
            expected.Rows.Add(
               test.PersonSID,                                              /*PersonId*/
               UserName,                                                    /*PersonName*/
               "",                                                          /*DisplayName*/
               null,                                                        /*Attribute1*/
               null,                                                        /*Attribute2*/
               null,                                                        /*Attribute3*/
               null,                                                        /*Attribute4*/
               null,                                                        /*Attribute5*/
               TimeDiff.Seconds(startTime1, completedTime2),           /*LogOnTime*/
               null,                                                        /*WaitingTime*/
               0,                                                           /*OnBreakTimePaid*/
               0,                                                           /*OnBreakTimeUnpaid*/
               (TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
                TimeDiff.Seconds(timeCallDelivered2, completedTime2)) / 2,  /*AverageCompletedInterviewDuration*/
               0,                                                           /*OpenEndReviewDuration*/
               0,                                                           /*PreviewDuration*/
               0,                                                           /*WrapDuration*/
               0,                                                           /*ConnectedDuration*/
               TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
               TimeDiff.Seconds(timeCallDelivered2, completedTime2),/*InterviewDuration*/
               2,                                                           /*DialingsCount*/
               2);                                                          /*Completes*/                                                    /*Completes*/
            CompareDataTables(expected, actual);


            //does not include end of the interview
            dbStartDate = startTime1.AddSeconds(0).ToString("yyyy-MM-dd HH:mm:ss.fff");
            dbEndtDate = completedTime2.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);
            expected = GetDefaultResultTable();
            expected.Rows.Add(
               test.PersonSID,                                              /*PersonId*/
               UserName,                                                    /*PersonName*/
               "",                                                          /*DisplayName*/
               null,                                                        /*Attribute1*/
               null,                                                        /*Attribute2*/
               null,                                                        /*Attribute3*/
               null,                                                        /*Attribute4*/
               null,                                                        /*Attribute5*/
               TimeDiff.Seconds(startTime1, completedTime2.AddSeconds(-1)), /*LogOnTime*/
               null,                                                        /*WaitingTime*/
               0,                                                           /*OnBreakTimePaid*/
               0,                                                           /*OnBreakTimeUnpaid*/
               TimeDiff.Seconds(timeCallDelivered1, completedTime1),  /*AverageCompletedInterviewDuration*/
               0,                                                           /*OpenEndReviewDuration*/
               0,                                                           /*PreviewDuration*/
               0,                                                           /*WrapDuration*/
               0,                                                           /*ConnectedDuration*/
               TimeDiff.Seconds(timeCallDelivered1, completedTime1),/*InterviewDuration*/
               1,                                                           /*DialingsCount*/
               1);                                                          /*Completes*/                                                    /*Completes*/
            CompareDataTables(expected, actual);

            //does not include First interview
            dbStartDate = startTime1.AddSeconds(12).ToString("yyyy-MM-dd HH:mm:ss.fff");
            dbEndtDate = completedTime2.AddSeconds(0).ToString("yyyy-MM-dd HH:mm:ss.fff");
            actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);
            expected = GetDefaultResultTable();
            expected.Rows.Add(
               test.PersonSID,                                              /*PersonId*/
               UserName,                                                    /*PersonName*/
               "",                                                          /*DisplayName*/
               null,                                                        /*Attribute1*/
               null,                                                        /*Attribute2*/
               null,                                                        /*Attribute3*/
               null,                                                        /*Attribute4*/
               null,                                                        /*Attribute5*/
               TimeDiff.Seconds(startTime1.AddSeconds(12), completedTime2), /*LogOnTime*/
               null,                                                        /*WaitingTime*/
               0,                                                           /*OnBreakTimePaid*/
               0,                                                           /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered2, completedTime2),  /*AverageCompletedInterviewDuration*/
               0,                                                           /*OpenEndReviewDuration*/
               0,                                                           /*PreviewDuration*/
               0,                                                           /*WrapDuration*/
               0,                                                           /*ConnectedDuration*/
               TimeDiff.Seconds(timeCallDelivered2, completedTime2),/*InterviewDuration*/
               1,                                                           /*DialingsCount*/
               1);                                                          /*Completes*/                                                  /*Completes*/
            CompareDataTables(expected, actual);

            //does not include start and end of the interview
            dbStartDate = startTime1.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss.fff");
            dbEndtDate = completedTime2.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);
            expected = GetDefaultResultTable();
            expected.Rows.Add(
               test.PersonSID,                                              /*PersonId*/
               UserName,                                                    /*PersonName*/
               "",                                                          /*DisplayName*/
               null,                                                        /*Attribute1*/
               null,                                                        /*Attribute2*/
               null,                                                        /*Attribute3*/
               null,                                                        /*Attribute4*/
               null,                                                        /*Attribute5*/
               TimeDiff.Seconds(startTime1.AddSeconds(2), completedTime2.AddSeconds(-1)),/*LogOnTime*/
               null,                                                        /*WaitingTime*/
               0,                                                           /*OnBreakTimePaid*/
               0,                                                           /*OnBreakTimeUnpaid*/
               (
                TimeDiff.Seconds(timeCallDelivered1, completedTime1)),  /*AverageCompletedInterviewDuration*/
               0,                                                           /*OpenEndReviewDuration*/
               0,                                                           /*PreviewDuration*/
               0,                                                           /*WrapDuration*/
               0,                                                           /*ConnectedDuration*/
               TimeDiff.Seconds(timeCallDelivered1, completedTime1),/*InterviewDuration*/
               1,                                                           /*DialingsCount*/
               1);                                                          /*Completes*/
            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\egork"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterview_SessionNotFinished_reportIsCorrect()
        {
            PrepareSessionDatabase();

            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_ManualOrPreview(null, 0);
            var startTime1 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            Thread.Sleep(1 * 1000);

            var timeCallDelivered1 = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            int initiator = 0;
            test.Dial(interview, initiator, true, CallOutcome.Connected);
            test.Hangup(interview, initiator);

            var completedTime1 = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            interview = test.CompleteInterviewAndWaitNext_Preview(interview);
            Thread.Sleep(2 * 1000);

            var dbStartDate = startTime1.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var dbEndtDate = startTime1.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss.fff");
            var actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);

            var logOnTime1 = (int)actual.Rows[0].ItemArray[8];
            Assert.IsTrue(logOnTime1 > TimeDiff.Seconds(startTime1, completedTime1));

            Thread.Sleep(1 * 1000);
            actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);

            var logOnTime2 = (int)actual.Rows[0].ItemArray[8];
            Assert.IsTrue(logOnTime2 - logOnTime1 >= 1);
        }
        
        [TestMethod, Owner(@"FIRM\egork"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterview_AddPreviewConnectedWrapTime_ReportIsCorrect()
        {
            PrepareSessionDatabase();

            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(2);

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime);
            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);
            
            
            int initiator = 0;
            //start interview
            BvInterviewEntity interview1 = test.StartInterview_Progressive(null, 0);
            var startTime1 = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;
            test.Dial(interview1, initiator, true, CallOutcome.Connected);
            test.Hangup(interview1, initiator);

            BvInterviewEntity interview2 = test.CompleteInterviewAndWaitNext_Manual(interview1);
            test.Dial(interview2, initiator, true, CallOutcome.Connected);
            test.Hangup(interview2, initiator);
            test.CompleteInterviewAndWaitNext_Manual(interview2);
            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(20));
            UpdateBvHistoryRecord(test.SurveySID, interview1.ID, 1, 2, 3, 7);
            UpdateBvHistoryRecord(test.SurveySID, interview2.ID, 4, 5, 6, 9);

            UpdateLogOnTime(startTime1, startTime1.AddSeconds(20), test.PersonSID);
            var dbStartDate = startTime1.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var dbEndtDate = startTime1.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss.fff");
            var actual = GetData(_defaultTemplate, test.SurveySID.ToString(), surveyNames: "All", dbStartDate: dbStartDate, dbEndDate: dbEndtDate);

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "",                                                     /*DisplayName*/
                null,                                                   /*Attribute1*/
                null,                                                   /*Attribute2*/
                null,                                                   /*Attribute3*/
                null,                                                   /*Attribute4*/
                null,                                                   /*Attribute5*/
                20,                                                     /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                0,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                8,                                                      /*AverageCompletedInterviewDuration*/
                0,                                                      /*OpenEndReviewDuration*/
                5,                                                      /*PreviewDuration*/
                7,                                                      /*WrapDuration*/
                9,                                                      /*ConnectedDuration*/
                16,                                                     /*InterviewDuration*/
                2,                                                      /*DialingsCount*/
                2);                                                     /*Completes*/

            CompareDataTables(expected, actual);
        }

        private void UpdateBvHistoryRecord(int surveyId, int interviewId, int previewTime, int wrapTime, int connectedTime, int duration)
        {
            var entity = BvHistoryAdapter.GetByCondition("SurveyID = @SurveyId AND InterviewId = @InterviewId", 
                new SqlParameter("SurveyId", surveyId), new SqlParameter("InterviewId", interviewId)).Single();

            entity.PreviewTime = previewTime;
            entity.ConnectedTime = connectedTime;
            entity.WrapTime = wrapTime;
            entity.Duration = duration;
            ServiceLocator.Resolve<IHistoryRepository>().Update(entity);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_TerminateWhileWaitingForInterview_TimingsAreCalculatedCorrectly()
        {
            int minInterviewingTime = 1;

            var test = new TestCati2(true, false, _backendTools);

            test.CreateSurveyWithPerson(DialingMode.Preview, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);
            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer("1234");

            //Sorry cannot avoid Sleep here
            Thread.Sleep(minInterviewingTime * 1000);

            Assert.IsTrue(test.TerminateTask(TestCati2.TerminateCalled.FromSupervisor, test.PersonSID));

            var result = GetData(_defaultTemplate, test.SurveySID.ToString());

            Assert.IsTrue(result.Rows.Count == 1);
            Assert.IsTrue(Convert.ToInt32(GetValueFromDataTable(result, "WaitingTime")) >= minInterviewingTime);
            Assert.IsTrue(Convert.ToInt32(GetValueFromDataTable(result, "Completes")) == 0);
            test.CheckLogout();
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterviewAndLogout_OpenEndReviewEnabled_ReportIsCorrect()
        {
            const int OpenEndReviewDurationInSec = 2;

            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            var survey = SurveyRepository.GetById(test.SurveySID);
            survey.ForceOpnRev = 1;
            SurveyRepository.Update(survey);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);

            Thread.Sleep(2 * 1000);

            test.ReplyOnInterview_Progressive(interview);
            test.WS.GetForceOpenendReview(1);

            Thread.Sleep(OpenEndReviewDurationInSec * 1000);

            test.CompleteInterviewWithLogout_Progressive(interview);

            var result = GetData(_defaultTemplate, test.SurveySID.ToString());

            Assert.IsTrue(Convert.ToInt32(GetValueFromDataTable(result, "OpenEndReviewDuration")) >= OpenEndReviewDurationInSec);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_StartEndEndTimeIsSpecified_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            var newUtcTime = new TestTimeService(DateTime.UtcNow.AddSeconds(2));
            new DateTimeMocker(_framework).MockDate(newUtcTime.GetUtcNow());

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(newUtcTime.GetUtcNow().AddSeconds(2));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            test.CompleteInterviewWithLogout_Progressive(interview);

            var startDate = DateTime.UtcNow.Date;
            var endDate = DateTime.UtcNow.Date.AddDays(1);
            var actual = GetData(
                _defaultTemplate,
                test.SurveySID.ToString(),  /*surveysids*/
                null,                       /*personsids*/
                false,                      /*use dialer*/
                true,                       /*hide empty*/
                startDate.ToString(),       /*start time*/
                endDate.ToString());        /*end time*/

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "",                                                     /*DisplayName*/
                null,                                                   /*Attribute1*/
                null,                                                   /*Attribute2*/
                null,                                                   /*Attribute3*/
                null,                                                   /*Attribute4*/
                null,                                                   /*Attribute5*/
                TimeDiff.Seconds(startTime, timeCallDelivered) +
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                0,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*AverageCompletedInterviewDuration*/
                0,                                                      /*OpenEndReviewDuration*/
                0,                                                      /*PreviewDuration*/
                0,                                                      /*WrapDuration*/
                0,                                                      /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered, completedTime), /*InterviewDuration*/
                1,                                                      /*DialingsCount*/
                1);                                                     /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteTwoInterviews_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);

            CreateSurveyWithTwoCompletedInterviews(test, out DateTime startTime1, out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1, out DateTime timeCallDelivered2, out DateTime completedTime2);

            var actual = GetData(
                _defaultTemplate,
                test.SurveySID.ToString(),                  /*surveysids*/
                test.PersonSID.ToString(),                  /*personsids*/
                false,                                      /*use dialer*/
                false,                                      /*hide empty*/
                startTime1.ToString(),                      /*start time*/
                DateTime.UtcNow.AddSeconds(10).ToString()); /*end time*/

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
               test.PersonSID,                                              /*PersonId*/
               UserName,                                                    /*PersonName*/
               "",                                                          /*DisplayName*/
               null,                                                        /*Attribute1*/
               null,                                                        /*Attribute2*/
               null,                                                        /*Attribute3*/
               null,                                                        /*Attribute4*/
                null,                                                       /*Attribute5*/
               TimeDiff.Seconds(startTime1, timeCallDelivered1) +
                TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
                TimeDiff.Seconds(startTime2, timeCallDelivered2) +
                TimeDiff.Seconds(timeCallDelivered2, completedTime2),       /*LogOnTime*/
               null,                                                        /*WaitingTime*/
               0,                                                           /*OnBreakTimePaid*/
               0,                                                           /*OnBreakTimeUnpaid*/
               (TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
                TimeDiff.Seconds(timeCallDelivered2, completedTime2)) / 2,  /*AverageCompletedInterviewDuration*/
               0,                                                           /*OpenEndReviewDuration*/
               0,                                                           /*PreviewDuration*/
               0,                                                           /*WrapDuration*/
               0,                                                           /*ConnectedDuration*/
               TimeDiff.Seconds(timeCallDelivered1, completedTime1) +
               TimeDiff.Seconds(timeCallDelivered2, completedTime2),/*InterviewDuration*/
               2,                                                           /*DialingsCount*/
               2);                                                          /*Completes*/

            Trace.TraceInformation("startTime1 = {0}", startTime1.ToString("o"));
            Trace.TraceInformation("timeCallDelivered1 = {0}", timeCallDelivered1.ToString("o"));
            Trace.TraceInformation("completedTime1 = {0}", completedTime1.ToString("o"));
            Trace.TraceInformation("startTime2 = {0}", startTime2.ToString("o"));
            Trace.TraceInformation("timeCallDelivered2 = {0}", timeCallDelivered2.ToString("o"));
            Trace.TraceInformation("completedTime2 = {0}", completedTime2.ToString("o"));

            BackendTools.TraceQuery(IntegrationTestingFramework.Instance.DbEngine, "BvHistory", "select * from BvHistory");

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_FilterByDataInReplicationTable_CorrectNumbeOfRecordsReturned()
        {
            var test = new TestCati2(true, false, _backendTools);
            var interview = CreateSurveyWithTwoCompletedInterviews(test, out DateTime startTime1, out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1, out DateTime timeCallDelivered2, out DateTime completedTime2);

            //Replicated table consists of two records and CallAttemptCount = NULL

            var actual = GetData(
               _defaultTemplate,
               test.SurveySID.ToString(),                   /*surveysids*/
               test.PersonSID.ToString(),                   /*personsids*/
               false,                                       /*use dialer*/
               false,                                       /*hide empty*/
               startTime1.ToString(),                       /*start time*/
               DateTime.UtcNow.AddSeconds(10).ToString(),   /*end time*/
               true,                                        /*calcAllBreakHistory*/
               "CFInterview.[CallAttemptCount]=1");         /*surveyDataFilter*/

            Assert.AreEqual(0, actual.Rows.Count);

            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            actual = GetData(
               _defaultTemplate,
               test.SurveySID.ToString(),                   /*surveysids*/
               test.PersonSID.ToString(),                   /*personsids*/
               false,                                       /*use dialer*/
               false,                                       /*hide empty*/
               startTime1.ToString(),                       /*start time*/
               DateTime.UtcNow.AddSeconds(10).ToString(),   /*end time*/
               true,                                        /*calcAllBreakHistory*/
               "CFInterview.[CallAttemptCount]=1");         /*surveyDataFilter*/

            Assert.AreEqual(1, actual.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_TwoCompletes_OneCompleteIsFilteredBySurveyData_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);

            var interview = CreateSurveyWithTwoCompletedInterviews(test, out DateTime startTime1, out DateTime timeCallDelivered1, out DateTime startTime2, out DateTime completedTime1, out DateTime timeCallDelivered2, out DateTime completedTime2);
            BackendTools.UpdateFieldInReplicatedTable(interview, "CallAttemptCount", "1");

            var actual = GetData(
               _defaultTemplate,
               test.SurveySID.ToString(),                   /*surveysids*/
               test.PersonSID.ToString(),                   /*personsids*/
               false,                                       /*use dialer*/
               false,                                       /*hide empty*/
               startTime1.ToString(),                       /*start time*/
               DateTime.UtcNow.AddSeconds(10).ToString(),   /*end time*/
               true,                                        /*calcAllBreakHistory*/
               "CFInterview.[CallAttemptCount]=1");         /*surveyDataFilter*/

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "",                                                     /*DisplayName*/
                null,                                                   /*Attribute1*/
                null,                                                   /*Attribute2*/
                null,                                                   /*Attribute3*/
                null,                                                   /*Attribute4*/
                null,                                                   /*Attribute5*/
                TimeDiff.Seconds(startTime2, timeCallDelivered2) +
                TimeDiff.Seconds(timeCallDelivered2, completedTime2),   /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                0,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered2, completedTime2),   /*AverageCompletedInterviewDuration*/
                0,                                                      /*OpenEndReviewDuration*/
                0,                                                      /*PreviewDuration*/
                0,                                                      /*WrapDuration*/
                0,                                                      /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered2, completedTime2),/*InterviewDuration*/
                1,                                                      /*DialingsCount*/
                1);                                                     /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterviewAndLogoutInSurveySelectionMode_LogoutTimeShouldBeCalced()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);
            test.CreateInterviewsWithCalls(1);

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, true);
            test.LoginToDialer(ExtensionNumber);

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(1));

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(2));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            // Not sure this is correct ... but now completed time equals logout time ( do not take dialler into account)
            // LS :  need to discuss this test
            var logoutTime = completedTime;

            test.CompleteInterviewWithLogout_Progressive(interview);

            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "",                                                     /*DisplayName*/
                null,                                                   /*Attribute1*/
                null,                                                   /*Attribute2*/
                null,                                                   /*Attribute3*/
                null,                                                   /*Attribute4*/
                null,                                                   /*Attribute5*/
                TimeDiff.Seconds(startTime, timeCallDelivered) +
                TimeDiff.Seconds(timeCallDelivered, completedTime) +
                TimeDiff.Seconds(completedTime, logoutTime),            /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                0,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*AverageCompletedInterviewDuration*/
                0,                                                      /*OpenEndReviewDuration*/
                0,                                                      /*PreviewDuration*/
                0,                                                      /*WrapDuration*/
                0,                                                      /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),/*InterviewDuration*/
                1,                                                      /*DialingsCount*/
                1);                                                     /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_SeveralTimeBreaks_TimeBreakIsDisplayed()
        {
            var testCati = new TestCati2(false, _backendTools);

            var surveyId = _backendTools.CreateSurvey("p00123");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = personId, StartTime = DateTime.UtcNow, BreakTypeId = _breakTypePaid.Id };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            BvTimeBreaksHistoryAdapter.Insert(new BvTimeBreaksHistoryEntity { Duration = 10, InterviewerId = personId, StartTime = DateTime.UtcNow, BreakTypeId = _breakTypeUnpaid.Id });

            BvHistoryAdapter.Insert(
                new BvHistoryEntity
                {
                    SurveyId = surveyId,
                    Duration = 1,
                    FiredTime = DateTime.UtcNow,
                    InterviewId = 1,
                    WaitingTime = 0,
                    PersonSID = personId,
                    ITS = 13,
                    RoleID = 2
                });

            var actual = GetData(_defaultTemplate, surveyId.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                personId,   /*PersonId*/
                UserName,   /*PersonName*/
                "",         /*DisplayName*/
                null,       /*Attribute1*/
                null,       /*Attribute2*/
                null,       /*Attribute3*/
                null,       /*Attribute4*/
                null,       /*Attribute5*/
                14,         /*LogOnTime*/
                null,       /*WaitingTime*/
                3,          /*OnBreakTimePaid*/
                10,         /*OnBreakTimeUnpaid*/
                1,          /*AverageCompletedInterviewDuration*/
                0,          /*OpenEndReviewDuration*/
                0,          /*PreviewDuration*/
                0,          /*WrapDuration*/
                0,          /*ConnectedDuration*/
                1,          /*InterviewDuration*/
                1,          /*DialingsCount*/
                1);         /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CompleteInterviewAndMakeSeveralBreaks_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);
            test.CreateInterviewsWithCalls(1);

            var currentUtcTime = DateTime.UtcNow;
            new DateTimeMocker(_framework).MockDate(currentUtcTime);

            test.Login(UserName, Password, AgentTaskChoiceMode.Automatic, true);
            test.LoginToDialer(ExtensionNumber);

            //start interview
            BvInterviewEntity interview = test.StartInterview_Progressive(null, 0);
            var startTime = TaskRepository.GetByPerson(test.PersonSID).StartTime.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(1));

            test.ReplyOnInterview_Progressive(interview);
            var timeCallDelivered = TaskRepository.GetByPerson(test.PersonSID).TimeCallDelivered.Value;

            new DateTimeMocker(_framework).MockDate(currentUtcTime.AddSeconds(2));

            var completedTime = TaskRepository.GetByPerson(test.PersonSID).CurrentUtcTime.Value;
            test.CompleteInterviewWithLogout_Progressive(interview);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = test.PersonSID, StartTime = DateTime.UtcNow };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,                                         /*PersonId*/
                UserName,                                               /*PersonName*/
                "",                                                     /*DisplayName*/
                null,                                                   /*Attribute1*/
                null,                                                   /*Attribute2*/
                null,                                                   /*Attribute3*/
                null,                                                   /*Attribute4*/
                null,                                                   /*Attribute5*/
                TimeDiff.Seconds(startTime, timeCallDelivered) +
                TimeDiff.Seconds(timeCallDelivered, completedTime) + 3, /*LogOnTime*/
                null,                                                   /*WaitingTime*/
                3,                                                      /*OnBreakTimePaid*/
                0,                                                      /*OnBreakTimeUnpaid*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),     /*AverageCompletedInterviewDuration*/
                0,                                                      /*OpenEndReviewDuration*/
                0,                                                      /*PreviewDuration*/
                0,                                                      /*WrapDuration*/
                0,                                                      /*ConnectedDuration*/
                TimeDiff.Seconds(timeCallDelivered, completedTime),/*InterviewDuration*/
                1,                                                      /*DialingsCount*/
                1);                                                      /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CalculateAllBreaks_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = test.PersonSID, StartTime = DateTime.UtcNow };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            timeBreaksHistoryEntity.SurveyId = test.SurveySID;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            var actual = GetData(_defaultTemplate, test.SurveySID.ToString(), null, false, false);

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,     /*PersonId*/
                UserName,           /*PersonName*/
                "",                 /*DisplayName*/
                null,               /*Attribute1*/
                null,               /*Attribute2*/
                null,               /*Attribute3*/
                null,               /*Attribute4*/
                null,               /*Attribute5*/
                3,                  /*LogOnTime*/
                null,               /*WaitingTime*/
                3,                  /*OnBreakTimePaid*/
                0,                  /*OnBreakTimeUnpaid*/
                0,                  /*AverageCompletedInterviewDuration*/
                0,                  /*OpenEndReviewDuration*/
                0,                  /*PreviewDuration*/
                0,                  /*WrapDuration*/
                0,                  /*ConnectedDuration*/
                0,                  /*InterviewDuration*/
                0,                  /*DialingsCount*/
                0);                 /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_CalculateSurveyBreaks_ReportIsCorrect()
        {
            var test = new TestCati2(true, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.Automatic);

            var timeBreaksHistoryEntity = new BvTimeBreaksHistoryEntity { Duration = 1, InterviewerId = test.PersonSID, StartTime = DateTime.UtcNow };
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            timeBreaksHistoryEntity.Duration = 2;
            timeBreaksHistoryEntity.SurveyId = test.SurveySID;
            BvTimeBreaksHistoryAdapter.Insert(timeBreaksHistoryEntity);

            var actual = GetData(
                _defaultTemplate,
                test.SurveySID.ToString(),    /*surveysids*/
                null,                         /*personsids*/
                false,                        /*use dialer*/
                false,                        /*hide empty*/
                null,                         /*start time*/
                null,                         /*end time*/
                false);                       /*calcAllBreakHistory*/

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                test.PersonSID,     /*PersonId*/
                UserName,           /*PersonName*/
                "",                 /*DisplayName*/
                null,               /*Attribute1*/
                null,               /*Attribute2*/
                null,               /*Attribute3*/
                null,               /*Attribute4*/
                null,               /*Attribute5*/
                2,                  /*LogOnTime*/
                null,               /*WaitingTime*/
                2,                  /*OnBreakTimePaid*/
                0,                  /*OnBreakTimeUnpaid*/
                0,                  /*AverageCompletedInterviewDuration*/
                0,                  /*OpenEndReviewDuration*/
                0,                  /*PreviewDuration*/
                0,                  /*WrapDuration*/
                0,                  /*ConnectedDuration*/
                0,                  /*InterviewDuration*/
                0,                  /*DialingsCount*/
                0);                 /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_NoCallsButHaveBreaksHideZero_StartInterviewNotCalled_ReportIsEmpty()
        {
            var test = new TestCati2(false, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, false);

            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);
            Thread.Sleep(1000);
            test.WS.ContinueWorkAfterBreak(1);

            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            Assert.AreEqual(0, actual.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_InterviewStartedNoCallsGoOnBreak_ReportIsNotEmpty()
        {
            int minWaitingTime = 1;

            var test = new TestCati2(false, false, _backendTools);
            test.CreateSurveyWithPerson(DialingMode.Automatic, UserName, Password, AgentTaskChoiceMode.CampaignAssignment);

            test.Login(UserName, Password, AgentTaskChoiceMode.CampaignAssignment, false);
            test.StartInterview_ManualOrPreview(test.SurveyName, 0);

            Thread.Sleep(minWaitingTime * 1000);
            test.WS.SetPendingBreakStatus(PendingBreakStatus.Break, 1);

            Assert.IsTrue(BvHistoryAdapter.GetAll().Count == 1);

            var actual = GetData(_defaultTemplate, test.SurveySID.ToString());

            Assert.AreEqual(1, actual.Rows.Count);
            Assert.IsTrue(Convert.ToInt32(GetValueFromDataTable(actual, "WaitingTime")) >= minWaitingTime);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_ThreeHistoryRecords_OneFitsInShift_BreakTimeCutByTheEndOfShift_OneRecordReturned()
        {
            var now = new DateTime(2014, 11, 23, 12, 0, 0);
            var testCati = new TestCati2(false, _backendTools);
            var surveyId = _backendTools.CreateSurvey("p010203123");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;

            CreateThreeHistoryRecordsOneBreakRecord(surveyId, personId, now);

            var actual = GetData(
                _defaultTemplate,
                surveyId.ToString(),                       /*surveysids*/
                null,                                      /*personsids*/
                false,                                     /*use dialer*/
                true,                                      /*hide empty*/
                null,                                      /*start time*/
                null,                                      /*end time*/
                true,                                      /*calcAllBreakHistory*/
                null,                                      /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)).ToString(), /* Shift start times */
                now.AddMinutes(-240).ToString()            /* Shift end times */);

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                personId,               /*PersonId*/
                UserName,               /*PersonName*/
                "",                     /*DisplayName*/
                null,                   /*Attribute1*/
                null,                   /*Attribute2*/
                null,                   /*Attribute3*/
                null,                   /*Attribute4*/
                null,                   /*Attribute5*/
                200 + 16 * 60 / 2 + 10, /*LogOnTime*/    //Duration+BreakTimePaid+WaitingTime
                null,                   /*WaitingTime*/
                16 * 60 / 2,            /*OnBreakTimePaid*/
                0,                      /*OnBreakTimeUnpaid*/
                200,                    /*AverageCompletedInterviewDuration*/
                0,                      /*OpenEndReviewDuration*/
                0,                      /*PreviewDuration*/
                0,                      /*WrapDuration*/
                0,                      /*ConnectedDuration*/
                200,                    /*InterviewDuration*/
                1,                      /*DialingsCount*/
                1);                     /*Completes*/

            CompareDataTables(expected, actual);
        }

        [TestMethod, Owner(@"FIRM\grigoryk"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_ThreeHistoryRecords_TwoFitsInShift_StartShiftGreaterEndShift_OvernightShift_TwoRecordsReturned()
        {
            var now = new DateTime(2014, 11, 23, 03, 0, 0);
            var testCati = new TestCati2(false, _backendTools);
            var surveyId = _backendTools.CreateSurvey("p010203124");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;

            CreateThreeHistoryRecordsOneBreakRecord(surveyId, personId, now);

            var actual = GetData(
                _defaultTemplate,
                surveyId.ToString(),                                       /*surveysids*/
                null,                                           /*personsids*/
                false,                                  /*use dialer*/
                true,                                          /*hide empty*/
                null,                                           /*start time*/
                null,                                           /*end time*/
                true,                                   /*calcAllBreakHistory*/
                null,                                     /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)).ToString(), /* Shift start times */
                now.AddMinutes(-120 + 1).ToString()         /* Shift end times */);

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                personId,                       /*PersonId*/
                UserName,                       /*PersonName*/
                "",                             /*DisplayName*/
                null,                           /*Attribute1*/
                null,                           /*Attribute2*/
                null,                           /*Attribute3*/
                null,                           /*Attribute4*/
                null,                           /*Attribute5*/
                200 + 300 + 16 * 60 + 10 + 15,  /*LogOnTime*/    //Duration+BreakTimePaid+WaitingTime
                null,                           /*WaitingTime*/
                16 * 60,                        /*OnBreakTimePaid*/
                0,                              /*OnBreakTimeUnpaid*/
                250,                            /*AverageCompletedInterviewDuration*/
                0,                              /*OpenEndReviewDuration*/
                0,                              /*PreviewDuration*/
                0,                              /*WrapDuration*/
                0,                              /*ConnectedDuration*/
                500,                            /*InterviewDuration*/
                2,                              /*DialingsCount*/
                2);                             /*Completes*/

            CompareDataTables(expected, actual);
        }
        
          [TestMethod, Owner(@"FIRM\EgorK"), TestCategory(TestsCategoriesNames.InterviewersProductivityReport)]
        public void InterviewProductivityReportDataProvider_TwoCallCenters_correctFilteringByCallCenter()
        {
            var now = new DateTime(2014, 11, 23, 03, 0, 0);
            var testCati = new TestCati2(false, _backendTools);
            var surveyId = _backendTools.CreateSurvey("p010203124");

            testCati.CreatePerson(UserName, Password, AgentTaskChoiceMode.Automatic);
            var personId = testCati.PersonSID;
            
            var callCenter2 = CallCenterTools.Create();
            var userName2 = UserName + "2";
            testCati.CreatePerson(userName2, Password, AgentTaskChoiceMode.Automatic, callCenter2.ID);
            var person2Id = testCati.PersonSID;
            
            CreateThreeHistoryRecordsOneBreakRecord(surveyId, personId, now);
            CreateThreeHistoryRecordsOneBreakRecord(surveyId, person2Id, now);

            var actual = GetData(
                _defaultTemplate,
                surveyId.ToString(),                                       /* surveysids */
                null,                                           /* personsids */
                false,                                  /* use dialer */
                true,                                          /* hide empty */
                null,                                           /* start time */
                null,                                            /* end time */
                true,                                   /* calcAllBreakHistory */
                null,                                     /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)).ToString(),  /* Shift start times */
                now.AddMinutes(-120 + 1).ToString(),         /* Shift end times */
                callCenterId:CallCenterTools.DefaultId);

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                personId,                       /*PersonId*/
                UserName,                       /*PersonName*/
                "",                             /*DisplayName*/
                null,                           /*Attribute1*/
                null,                           /*Attribute2*/
                null,                           /*Attribute3*/
                null,                           /*Attribute4*/
                null,                           /*Attribute5*/
                200 + 300 + 16 * 60 + 10 + 15,  /*LogOnTime*/    //Duration+BreakTimePaid+WaitingTime
                null,                           /*WaitingTime*/
                16 * 60,                        /*OnBreakTimePaid*/
                0,                              /*OnBreakTimeUnpaid*/
                250,                            /*AverageCompletedInterviewDuration*/
                0,                              /*OpenEndReviewDuration*/
                0,                              /*PreviewDuration*/
                0,                              /*WrapDuration*/
                0,                              /*ConnectedDuration*/
                500,                            /*InterviewDuration*/
                2,                              /*DialingsCount*/
                2);                             /*Completes*/

            CompareDataTables(expected, actual);
            
            var actual2 = GetData(
                _defaultTemplate,
                surveyId.ToString(),                       /*surveysids*/
                null,                                      /*personsids*/
                false,                                     /*use dialer*/
                true,                                      /*hide empty*/
                null,                                      /*start time*/
                null,                                      /*end time*/
                true,                                      /*calcAllBreakHistory*/
                null,                                      /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)).ToString(), /* Shift start times */
                now.AddMinutes(-120 + 1).ToString(),        /* Shift end times */
                callCenterId:callCenter2.ID);

            var expected2 = GetDefaultResultTable();
            expected2.Rows.Add(
                person2Id,                       /*PersonId*/
                userName2,                       /*PersonName*/
                "",                             /*DisplayName*/
                null,                           /*Attribute1*/
                null,                           /*Attribute2*/
                null,                           /*Attribute3*/
                null,                           /*Attribute4*/
                null,                           /*Attribute5*/
                200 + 300 + 16 * 60 + 10 + 15,  /*LogOnTime*/    //Duration+BreakTimePaid+WaitingTime
                null,                           /*WaitingTime*/
                16 * 60,                        /*OnBreakTimePaid*/
                0,                              /*OnBreakTimeUnpaid*/
                250,                            /*AverageCompletedInterviewDuration*/
                0,                              /*OpenEndReviewDuration*/
                0,                              /*PreviewDuration*/
                0,                              /*WrapDuration*/
                0,                              /*ConnectedDuration*/
                500,                            /*InterviewDuration*/
                2,                              /*DialingsCount*/
                2);                             /*Completes*/

            CompareDataTables(expected2, actual2);
            
            var actual3 = GetData(
                _defaultTemplate,
                surveyId.ToString(),                       /*surveysids*/
                null,                                      /*personsids*/
                false,                                     /*use dialer*/
                true,                                      /*hide empty*/
                null,                                      /*start time*/
                null,                                      /*end time*/
                true,                                      /*calcAllBreakHistory*/
                null,                                      /* surveyDataFilter */
                now.AddMinutes(-(240 + 8 + 2)).ToString(), /* Shift start times */
                now.AddMinutes(-120 + 1).ToString()       /* Shift end times */);

            Assert.AreEqual(2, actual3.Rows.Count);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void InterviewProductivityReportDataProvider_ExcludeItsesFromDialingCount_DialingCountIsCorrect()
        {
            var timeMocker = new DateTimeMocker(_framework);

            timeMocker.MockDate(DateTime.UtcNow.TrimMiliseconds());

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData()}
                        },
                        Assigns = new [] {"P1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomeBehaviors((call) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(10));
                return CallOutcome.Connected;
            });

            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(20));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "1" });

            Assert.AreEqual("S1.I2", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "2" });

            Assert.AreEqual("S1.I3", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(40));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "3" });

            Assert.AreEqual("S1.I4", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(50));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "13" });

            Assert.IsNull(interview);

            var column = (ProductivityReportTemplateColumnWithStatuses)_defaultTemplate.Columns.FirstOrDefault(x => x.StandardColumnName == "DialingsCount");
            column.ExtendedStatuses.AddRange(new[] { 2, 3 });

            var actual = GetData(_defaultTemplate, survey.Id.ToString());

            var expected = GetDefaultResultTable();
            expected.Rows.Add(
                person.Id,              /*PersonId*/
                person.Data.Name,       /*PersonName*/
                "",                     /*DisplayName*/
                null,                   /*Attribute1*/
                null,                   /*Attribute2*/
                null,                   /*Attribute3*/
                null,                   /*Attribute4*/
                null,                   /*Attribute5*/
                20 + 30 + 40 + 50 + 40, /*LogOnTime*/
                40,                     /*WaitingTime*/
                0,                      /*OnBreakTimePaid*/
                0,                      /*OnBreakTimeUnpaid*/
                50,                     /*AverageCompletedInterviewDuration*/
                0,                      /*OpenEndReviewDuration*/
                0,                      /*PreviewDuration*/
                0,                      /*WrapDuration*/
                0,                      /*ConnectedDuration*/
                140,                    /*InterviewDuration*/
                2,                      /*DialingsCount*/
                1);                     /*Completes*/

            CompareDataTables(expected, actual, false);
        }

        [TestMethod, Owner(@"FIRM\grigoryk")]
        public void InterviewProductivityReportDataProvider_Add2AdditinalColumnsWithStatus_NewColumnsAreReturned()
        {
            var timeMocker = new DateTimeMocker(_framework);

            timeMocker.MockDate(DateTime.UtcNow.TrimMiliseconds());

            var context = new TestData()
            {
                Surveys = new[] {
                    new SurveyData(){ Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData() {Tag = "S1.I1", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I2", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I3", Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Call = new CallData()}
                        },
                        Assigns = new [] {"P1"}
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData() { Tag = "D1" } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var dialer = context.GetDialer("D1");

            var console = new AutomaticConsoleController(context, person, survey, dialer);
            console.Login();
            console.LoginToDialer();

            dialer.SetOutcomeBehaviors((call) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(10));
                return CallOutcome.Connected;
            });

            var interview = console.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(20));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "1" });

            Assert.AreEqual("S1.I2", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(30));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "2" });

            Assert.AreEqual("S1.I3", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(40));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "3" });

            Assert.AreEqual("S1.I4", interview.Tag);

            timeMocker.AddTime(TimeSpan.FromSeconds(50));

            interview = console.NextInterview(interview, new CompletedInterviewDetails() { Its = "13" });

            Assert.IsNull(interview);

            AddColumnWithStatusesToDefaultTemplate(2);
            AddColumnWithStatusesToDefaultTemplate(3);

            var actual = GetData(_defaultTemplate, survey.Id.ToString());

            var expected = GetDefaultResultTable();
            expected.Columns.Add("Status2");
            expected.Columns.Add("Status3");

            expected.Rows.Add(
                person.Id,              /*PersonId*/
                person.Data.Name,       /*PersonName*/
                "",                     /*DisplayName*/
                null,                   /*Attribute1*/
                null,                   /*Attribute2*/
                null,                   /*Attribute3*/
                null,                   /*Attribute4*/
                null,                   /*Attribute5*/
                20 + 30 + 40 + 50 + 40, /*LogOnTime*/
                40,                     /*WaitingTime*/
                0,                      /*OnBreakTimePaid*/
                0,                      /*OnBreakTimeUnpaid*/
                50,                     /*AverageCompletedInterviewDuration*/
                0,                      /*OpenEndReviewDuration*/
                0,                      /*PreviewDuration*/
                0,                      /*WrapDuration*/
                0,                      /*ConnectedDuration*/
                140,                    /*InterviewDuration*/
                4,                      /*DialingsCount*/
                1,                      /*Completes*/
                1,                      /*Status2*/
                1);                     /*Status3*/

            CompareDataTables(expected, actual, false);
        }

        private void AddColumnWithStatusesToDefaultTemplate(int its)
        {
            var column = new ProductivityReportTemplateColumnWithStatuses()
            {
                DisplayName = $"Status{its}",
                IsIncludeStatuses = true,
                ExtendedStatuses = new List<int>() { its }
            };

            _defaultTemplate.Columns.Add(column);
        }

        private void PrepareSessionDatabase()
        {
            _framework.DbEngine.ExecuteNonQuery(
                @"	CREATE TABLE [dbo].[CatiInterviewerSessionHistory]
	            (
		            [SessionId] INT IDENTITY(1,1),
		            [CompanyId] INT NOT NULL,
		            [CallCenterId] INT NOT NULL,
		            [InterviewerId] INT NOT NULL,
		            [LoginTime] DATETIME NOT NULL,
		            [LogoutTime] DATETIME,
		            CONSTRAINT [PK_CatiInterviewerSessionHistory] PRIMARY KEY CLUSTERED([SessionId] ASC)
	            )", CommandType.Text);

            BackendInstance.Current.ConfirmlogConnectionString = _framework.DbEngine.ConnectionString;

            _framework.RegistryStub<IPersonSessionHistoryRepository, PersonSessionHistoryRepository>();
        }
    }
}
