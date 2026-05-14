using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class SuspendedModeFilterTest
    {
        private const int TotalInterviewCountForSurevy = 30;
        
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private FilterAndPagingTools _filterAndPagingTools;
        private readonly List<RespondentRecord> _respondentlist = new List<RespondentRecord>();
        private int _surveyId;
        private readonly Dictionary<string, IEnumerable<string>> _listCfVariables = new Dictionary<string, IEnumerable<string>>
            {
                {"q1", new[] {"1", "2", "1", "3", "1", "23"}},
                {"q2", new[] {"1", "1", "2", "2"}}
            };

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
            BvSvyScheduleAdapter.DeleteByCondition("ID%2 = 0");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
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

            Assert.AreEqual(interviewId, row["InterviewID"], "InterviewID, interview id = " + interviewId);
            Assert.AreEqual(respondent.RespondentPhone, row["TelephoneNumber"], "TelephoneNumber, interview id = " + interviewId);
            Assert.AreEqual(respondent.RespondentName, row["RespondentName"], "RespondentName, interview id = " + interviewId);
            Assert.AreEqual(new DateTime(1899, 12, 30), row["LastCallTime"], "LastCallTime, interview id = " + interviewId);
            Assert.AreEqual(respondent.TimeZoneId, row["TimezoneId"], "TimezoneId, interview id = " + interviewId);
            Assert.AreEqual(respondent.DialAttempts, row["AttemptNumber"], "AttemptNumber, interview id = " + interviewId);
            Assert.AreEqual(TimezoneRepository.GetById(respondent.TimeZoneId).Name, row["TimezoneName"], "TimezoneName, interview id = " + interviewId);
            Assert.AreEqual("Fresh sample", row["StateName"], "StateName, interview id = " + interviewId);

            Assert.AreEqual(DBNull.Value, row["Priority"], "Priority, interview id = " + interviewId);
            Assert.AreEqual(DBNull.Value, row["Time"], "Time, interview id = " + interviewId);
            Assert.AreEqual(DBNull.Value, row["ExpireTime"], "ExpireTime, interview id = " + interviewId);
            Assert.AreEqual(DBNull.Value, row["ApptTime"], "ApptTime, interview id = " + interviewId);
            Assert.AreEqual(DBNull.Value, row["ExpTime"], "ExpTime, interview id = " + interviewId);
            Assert.AreEqual(0, row["CallId"], "CallId, interview id = " + interviewId);
            Assert.AreEqual(String.Empty, row["ShiftType"], "ShiftType, interview id = " + interviewId);
            Assert.AreEqual(String.Empty, row["Resource"], "Resource, interview id = " + interviewId);
            Assert.AreEqual(0, row["Shift_ID"], "Shift_ID, interview id = " + interviewId);

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

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetSuspendedInterviews_EverySecondInterviewHasCall_TotalCountIsCorrect()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.Suspended, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount);
            var expected =
                _respondentlist.Where(x => x.InterviewId%2 == 0)
                              .OrderBy(x => x.InterviewId)
                              .Select(x => x.InterviewId)
                              .ToArray();
            IsCorrectRecordSet(expected, actualRecordSet);
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetSuspendedInterviews_WithCFVariablesWithoutAliases()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.Suspended, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount, "q1");
            var expected =
                _respondentlist.Where(x => x.InterviewId % 2 == 0)
                              .OrderBy(x => x.InterviewId)
                              .Select(x => x.InterviewId)
                              .ToArray();
            IsCorrectRecordSet(expected, actualRecordSet, "q1");
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetSuspendedInterviews_FilteredStringCannotStartWithAnything()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(_surveyId,
                                                                    AndOrOperator.And,
                                                                    new[]
                                                                        {
                                                                            new FilterField(TableTypes.CFVariables,
                                                                                            "q1",
                                                                                            VariableTypes.String,
                                                                                            FilterOperator.Like,
                                                                                            "3",
                                                                                            false)
                                                                        });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.Suspended, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount, "q1");
            var expected = _respondentlist.Where(x => x.InterviewId == 4).Select(x => x.InterviewId).ToArray();
            IsCorrectRecordSet(expected, actualRecordSet, "q1");
            Assert.AreEqual(expected.Length, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetSuspendedInterviews_GetScheduledCallIDsWithNotExistentCfVariables_UserMessageException()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.Or,
                new[]{ new FilterField(TableTypes.CFVariables,
                                                   "NotExistentCFVariableName",
                                                   VariableTypes.String,
                                                   FilterOperator.Equal,
                                                   "value",
                                                   false) });

            int totalCount;
            TestAssert.InvokeMethodAndVerifyExceptionThrown<UserMessageException>(() => CallManager.GetCallsRange(_surveyId, filterId, CallStates.Suspended, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount, "q1"));
        }
    }
}
