using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.Surveys;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class ScheduledModeFilterTest
    {
        private const int TotalInterviewCountForSurevy = 30;
        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private FilterAndPagingTools _filterAndPagingTools;
        private readonly List<RespondentRecord> _respondentlist = new List<RespondentRecord>();
        private int _surveyId;

        private readonly Dictionary<string, IEnumerable<string>> _listCfVariables = new Dictionary<string, IEnumerable<string>>
            {
                {"q1", new[] {"1", "2", "1", "3", "1"}},
                {"q2", new[] {"1", "1", "2", "2"}}
            };

        private readonly DateTime _timeToCall1 = DateTime.UtcNow.AddDays(1).CutMilliseconds();
        private readonly DateTime _timeToCall2 = DateTime.UtcNow.AddDays(1).CutMilliseconds().AddMinutes(1);
        private readonly DateTime _timeToCall3 = DateTime.UtcNow.AddDays(1).CutMilliseconds().AddMinutes(2);
        private const int Priority1 = 13;
        private const int Priority2 = 14;
        private const int Priority3 = 15;
        private DateTime _appointmentTime = DateTime.UtcNow.AddDays(1).CutMilliseconds();

        private IEnumerable<RespondentRecord> GetRespondentRecords()
        {
            var rand = new Random(1000);

            for (int i = 1; i <= TotalInterviewCountForSurevy; ++i)
                _respondentlist.Add(new RespondentRecord
                {
                    Sid = i.ToString(CultureInfo.InvariantCulture),
                    InterviewId = i,
                    RespondentName = "resp" + (TotalInterviewCountForSurevy - i),
                    RespondentPhone = rand.Next().ToString(CultureInfo.InvariantCulture),
                    LastCallTime = null,
                    TotalDuration = rand.Next(1, 1000),
                    ExtensionNumber = rand.Next().ToString(CultureInfo.InvariantCulture),
                    DialAttempts = rand.Next(1, 10),
                    TimeZoneId = rand.Next(1, 50),
                    LastChannelId = (byte)i,
                    Resource = i
                });

            return _respondentlist;
        }

        public void IsCorrectRecordSet(int[] expectedInterviewIds, DataTable actualRecordSet, params string[] cfVariables)
        {
            Assert.AreEqual(expectedInterviewIds.Length, actualRecordSet.Rows.Count, "records count");

            for (int i = 0; i < expectedInterviewIds.Length; ++i)
            {
                IsCorrectRecord(expectedInterviewIds[i], actualRecordSet.Rows[i], cfVariables);
            }
        }

        public void IsCorrectRecord(int interviewId, DataRow row, params string[] cfVariables)
        {
            var respondent = _respondentlist.SingleOrDefault(x => x.InterviewId == interviewId);
            int priority = 1;
            DateTime dateTime;
            string resource;
            //var shiftType = "None";
            if (interviewId <= 6)
            {
                priority = Priority1;
                dateTime = TimezoneManager.ConvertToUTC(respondent.TimeZoneId, _timeToCall1);
                resource = "p1";
                //shiftType = "None";
            }
            else if (interviewId <= 11)
            {
                priority = Priority2;
                dateTime = TimezoneManager.ConvertToUTC(respondent.TimeZoneId, _timeToCall2);
                resource = "p1";
                //shiftType = "Any Valid";
            }
            else
            {
                priority = Priority3;
                dateTime = TimezoneManager.ConvertToUTC(respondent.TimeZoneId, _timeToCall3);
                resource = "p2";
                //shiftType = "None";
            }

            Assert.AreEqual(interviewId, row["InterviewID"], "InterviewID, interview id = " + interviewId);
            Assert.AreEqual(interviewId == 2 ? 1000: priority, row["Priority"], "Priority, interview id = " + interviewId);
            Assert.AreEqual(dateTime, row["Time"], "Time, interview id = " + interviewId);
            Assert.AreEqual(interviewId == 2 ? (object)dateTime.AddDays(1) : DBNull.Value, row["ExpireTime"], "ExpireTime, interview id = " + interviewId);
            Assert.AreEqual(respondent.RespondentPhone, row["TelephoneNumber"], "TelephoneNumber, interview id = " + interviewId);
            Assert.AreEqual(respondent.RespondentName, row["RespondentName"], "RespondentName, interview id = " + interviewId);
            Assert.AreEqual(new DateTime(1899, 12, 30), row["LastCallTime"], "LastCallTime, interview id = " + interviewId);
            Assert.AreEqual(respondent.DialAttempts, row["AttemptNumber"], "AttemptNumber, interview id = " + interviewId);
            Assert.AreEqual(TimezoneRepository.GetById(respondent.TimeZoneId).Name, row["TimezoneName"], "TimezoneName, interview id = " + interviewId);
            Assert.AreEqual(interviewId == 2 ? "Appointment" : "Fresh sample", row["StateName"], "StateName, interview id = " + interviewId);
            Assert.AreEqual(interviewId == 2 ? (object)_appointmentTime : DBNull.Value, row["ApptTime"], "ApptTime, interview id = " + interviewId);
            Assert.AreEqual(interviewId == 2 ? (object)_appointmentTime.AddDays(1) : DBNull.Value, row["ExpTime"], "ExpTime, interview id = " + interviewId);
            Assert.AreEqual(interviewId, row["CallId"], "CallId, interview id = " + interviewId);
            //TODO:
            //Assert.AreEqual(shiftType, row["ShiftType"], "ShiftType, interview id = " + interviewId);
            Assert.AreEqual(resource, row["Resource"], "Resource, interview id = " + interviewId);

            IsCorrectRecordCfVariable(interviewId - 1, interviewId, row, cfVariables);
        }

        public void IsCorrectRecordCfVariable(int index, int interviewId, DataRow row, params string[] cfVariables)
        {
            foreach (var cfVariable in cfVariables)
            {
                var variable = (row["Var" + cfVariable] != DBNull.Value ? ((byte)row["Var" + cfVariable]).ToString(CultureInfo.InvariantCulture) : null);
                Assert.AreEqual(_listCfVariables[cfVariable].ElementAtOrDefault(index), variable, cfVariable + ", interviewId = " + interviewId);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _filterAndPagingTools = new FilterAndPagingTools(_framework, _backendTools);

            _backendTools.LaunchAllHoursScript();
            ConfirmitTools.CreateRespondentTable(_framework.DbEngine);
            ConfirmitTools.CreateResponseTable(_framework.DbEngine, new[] { "q1", "q2" }, "response0");

            ConfirmitTools.FillRespondentTable(_framework.DbEngine, GetRespondentRecords(), 1);
            ConfirmitTools.FillResponseTable(_framework.DbEngine, "response0", new[] { "q1", "q2" }, TotalInterviewCountForSurevy, _listCfVariables["q1"], _listCfVariables["q2"]);

            BackendTools.EnableChangeTracking(_framework.DbEngine, new[] { "respondent", "response0" }.Select(x => new TableInfo { Name = x }).ToArray());

            _surveyId = _filterAndPagingTools.CreateSurveyWithSample("p00012");
            BvSvyScheduleAdapter.DeleteByCondition("ID = 1");

            var personId1 = PersonTools.CreatePerson("p1");
            var personId2 = PersonTools.CreatePerson("p2");

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                _surveyId, _respondentlist.Skip(1).Take(5).Select(x => x.InterviewId).ToArray(), Priority1, personId1, (int)CallShiftType.None, _timeToCall1, CallStates.All, false, "super");

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                _surveyId, _respondentlist.Skip(6).Take(5).Select(x => x.InterviewId).ToArray(), Priority2, personId1, (int)CallShiftType.AnyValid, _timeToCall2, CallStates.All, false, "super");

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(
                _surveyId, _respondentlist.Skip(11).Select(x => x.InterviewId).ToArray(), Priority3, personId2, 1, _timeToCall3, CallStates.All, false, "super");

            BackendTools.AddAppointmentAndLinkItWithCall(2, _surveyId, _appointmentTime.AddDays(1));
            BackendTools.AddAppointmentAndLinkItWithCall(2, _surveyId, _appointmentTime);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetScheduledInterviews_EverySecondInterviewHasCall_TotalCountIsCorrect()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.Scheduled, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount);
            var expected = _respondentlist.Skip(1).OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet);
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetScheduledInterviews_WithShiftTypeAnyValidAndNone()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.Or,
                new[]{ 
                    new FilterField(TableTypes.ShiftType,
                    "Name",
                    VariableTypes.String,
                    FilterOperator.Equal,
                    "[None]",
                    false), new FilterField(TableTypes.ShiftType,
                    "Name",
                    VariableTypes.String,
                    FilterOperator.Equal,
                    "[Any Valid]",
                    false) });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Scheduled, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount);

            var expected = _respondentlist.Skip(1).Take(10).OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet);
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetScheduledInterviews_WithDefaultShiftType()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]{new FilterField(TableTypes.ShiftType,
                    "Name",
                    VariableTypes.String,
                    FilterOperator.Equal,
                    "Default",
                    false) });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Scheduled, new RangingArgs(1, 7, "ID", true), out totalCount);

            var expected = _respondentlist.Skip(11).Take(7).OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet);
            Assert.AreEqual(_respondentlist.Skip(11).Count(), totalCount);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScheduledInterviews_FilterWithEmptyLastInterviewerName_AllRecordsAreReturned()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]{new FilterField(TableTypes.Person,
                    "Name",
                    VariableTypes.String,
                    FilterOperator.Equal,
                    "",
                    false) });

            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Scheduled, new RangingArgs(1, 7, "ID", true), out int totalCount);

            var expected = _respondentlist.Skip(1).Take(7).OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet);
            Assert.AreEqual(_respondentlist.Skip(1).Count(), totalCount);
        }

        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScheduledInterviews_FilterWithNonEmptyLastInterviewerName_NoRecordsAreReturned()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]{new FilterField(TableTypes.Person,
                    "Name",
                    VariableTypes.String,
                    FilterOperator.Equal,
                    "p1",
                    false) });

            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Scheduled, new RangingArgs(1, 7, "ID", true), out int totalCount);
            
            Assert.AreEqual(0, actualRecordSet.Rows.Count);
            Assert.AreEqual(0, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetScheduledInterviews_WithFilterByAppointmentExpiredTime()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.Or,
                new[]{ new FilterField(TableTypes.Appointment,
                                                   "ExpTime",
                                                   VariableTypes.Date,
                                                   FilterOperator.Equal,
                                                   _appointmentTime.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),
                                                   false) });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Scheduled, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount);

            var expected = _respondentlist.Where(x => x.InterviewId == 2).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet);
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetScheduledInterviews_WithCfVariablesOrderByPriority()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.Scheduled, new RangingArgs(1, TotalInterviewCountForSurevy, "Priority", true), out totalCount, "q1");

            var expected = _respondentlist.Skip(2).Select(x => x.InterviewId).Union(new[]{2}).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet, "q1");
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL"), Cr(48074)]
        public void GetScheduledInterviews_FilterTest_EmptyFilterWithSearching()
        {
            try
            {
                _framework.SetTestHttpContextCurrentWithSupervisorPrincipal();

                int filterId = FilterAndPagingTools.CreateSimpleFilter(new FilterField[0]);

                var searchParameterCollection = new SearchParameterCollection();
                var additionalParameter = new SearchParameter
                {
                    ColumnName = "Priority",
                    ColumnType = SearchColumnType.Number,
                    Operator = SearchOperator.LessThanOrEqual,
                    Value = Priority1
                };
                searchParameterCollection.Add(additionalParameter);

                var pagingArgs = new PagingArgs(1, 1, "Priority", true, searchParameterCollection);

                int totalCount;
                var actualRecordSet = CallHelper.GetCallsPage(
                    _surveyId,
                    filterId,
                    1,
                    CallStates.Scheduled,
                    pagingArgs,
                    out totalCount,
                    ShowTimeMode.Interviewer,
                    false,
                    new string[0]);

                var expected = _respondentlist.Skip(2).Take(1).Select(x => x.InterviewId).ToArray();
                IsCorrectRecordSet(expected, actualRecordSet);
                Assert.AreEqual(4, totalCount);
            }
            finally
            {
                _framework.ClearTestHttpContextCurrent();
            }
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetScheduledInterviews_GetSortedPageOfScheduledCallsWithFilterByCF()
        {
            var attemptNumer = _respondentlist[3].DialAttempts;
            int filterId = FilterAndPagingTools.CreateSimpleFilter(_surveyId,
                                                                    AndOrOperator.And,
                                                                    new[]
                                                                        {
                                                                            new FilterField(TableTypes.Interview,
                                                                                            "AttemptNumber",
                                                                                            VariableTypes.Integer,
                                                                                            FilterOperator.Equal,
                                                                                            attemptNumer,
                                                                                            false),
                                                                            new FilterField(TableTypes.CFVariables,
                                                                                            "q1",
                                                                                            VariableTypes.Integer,
                                                                                            FilterOperator.LessEqual,
                                                                                            3,
                                                                                            false)
                                                                        });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Scheduled, new RangingArgs(1, 20, "Priority", false), out totalCount, "q1");

            var expected = _respondentlist.Skip(1).Take(4).Where(x => x.DialAttempts == attemptNumer).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet, "q1");
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\SergeyC")]
        public void GetScheduledInterviews_OrderByCallState()
        {
            var disabledInterviewsCount = _respondentlist.Count / 2;
            var enableCallsResult = new TestCallManagementOperationFactory().CreateEnableCallsSelected(
                _surveyId,
                _respondentlist.Where(i => i.InterviewId < disabledInterviewsCount).Select(i => i.InterviewId).ToArray(), false, true);
            Assert.AreEqual(AsyncOperationState.Completed, enableCallsResult.State, "Unable to disable calls");

            var disabledInterviewIds = _respondentlist.Skip(1).Where(i => i.InterviewId < disabledInterviewsCount).Select(i => i.InterviewId).ToArray();
            var enabledInterviewIds = _respondentlist.Where(i => i.InterviewId >= disabledInterviewsCount).Select(i => i.InterviewId).ToArray();
            var expectedResult = enabledInterviewIds.Union(disabledInterviewIds).ToArray();

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.Scheduled, 
                                                            new RangingArgs(1, _respondentlist.Count, "CallState", false),
                                                            out totalCount);

            IsCorrectRecordSet(expectedResult, actualRecordSet);
        }
        
        [TestMethod, Owner(@"FIRM\GrigoryK")]
        public void GetScheduledInterviews_GetOnlyAvailableNowCalls()
        {
            var dbEngine = new DatabaseEngine();
            dbEngine.ExecuteNonQuery($"INSERT INTO BvActiveShiftTypeZone VALUES (-1, {_surveyId}, 0)", CommandType.Text);
            dbEngine.ExecuteNonQuery($"UPDATE BvSvySchedule SET TimeInShift = '1899-12-30 00:00:00.000' WHERE SurveySID = {_surveyId} AND ID > 2", CommandType.Text);
            dbEngine.ExecuteNonQuery($"UPDATE BvSvySchedule SET CallState = 3 WHERE SurveySID = {_surveyId} AND ID = 3", CommandType.Text);
            dbEngine.ExecuteNonQuery($"UPDATE BvSvySchedule SET ShiftTypeID = -1 WHERE SurveySID = {_surveyId};" +
                                     $"UPDATE BvSvySchedule SET ShiftTypeID = 1 WHERE SurveySID = {_surveyId}  AND ID = 4", CommandType.Text);
            
            var expectedResult = _respondentlist.Skip(4).Select(i => i.InterviewId).ToArray();

            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.CallsAvailableNow, 
                new RangingArgs(1, _respondentlist.Count, "CallState", false),
                out var totalCount);

            Assert.AreEqual(expectedResult.Length, actualRecordSet.Rows.Count);
            Assert.AreEqual(totalCount, actualRecordSet.Rows.Count);
            
            for (int i = 0; i < expectedResult.Length; ++i)
            {
                DataRow row = actualRecordSet.Rows[i];
                var interviewId = expectedResult[i];
                var respondent = _respondentlist.SingleOrDefault(x => x.InterviewId == interviewId);
                
                Assert.AreEqual(interviewId, row["InterviewID"], "InterviewID, interview id = " + interviewId);
                Assert.AreEqual(respondent.RespondentPhone, row["TelephoneNumber"], "TelephoneNumber, interview id = " + interviewId);
                Assert.AreEqual(respondent.RespondentName, row["RespondentName"], "RespondentName, interview id = " + interviewId);
                
            }
        }
    }
}
