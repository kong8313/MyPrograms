using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;
using Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using IntegrationTests.Tests.FilterAndPaging.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.FilterAndPaging.Tests
{
    [TestClass]
    public class AllModeFilterTest
    {
        private const int TotalInterviewCountForSurevy = 30;
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private int _surveyId;
        private readonly List<RespondentRecord> _respondentList = new List<RespondentRecord>();
        private DateTime _appointmentTime = DateTime.UtcNow.AddDays(1).CutMilliseconds();
        private BackendTools _backendTools;
        private FilterAndPagingTools _filterAndPagingTools;

        private readonly Dictionary<string, IEnumerable<string>> _listCfVariables = new Dictionary<string,IEnumerable<string>>
            {
                {"q1", new[] {"1", "2", "1", "3", "1"}},
                {"q2", new[] {"1", "1", "2", "2"}}
            };

        private readonly List<int> _interviewIdWithAppointment = new List<int>{2};

        private IEnumerable<RespondentRecord> GetRespondentRecords()
        {
            var rand = new Random(1000);

            for (int i = 1; i <= TotalInterviewCountForSurevy; ++i)
                _respondentList.Add(new RespondentRecord
                    {
                        Sid = i.ToString(CultureInfo.InvariantCulture),
                        InterviewId = i,
                        RespondentName = "resp" + (TotalInterviewCountForSurevy-i),
                        RespondentPhone = rand.Next().ToString(CultureInfo.InvariantCulture),
                        LastCallTime = null,
                        TotalDuration = rand.Next(1, 1000),
                        ExtensionNumber = rand.Next().ToString(CultureInfo.InvariantCulture),
                        DialAttempts = rand.Next(1, 10),
                        TimeZoneId = rand.Next(1, 50),
                        LastChannelId = (byte) i,
                        DialMode = rand.Next(0, 4),
                        Resource = i
                    });

            return _respondentList;
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
            ConfirmitTools.CreateResponseTable(_framework.DbEngine, new[]{"q1", "q2"}, "response0");

            ConfirmitTools.FillRespondentTable(_framework.DbEngine, GetRespondentRecords(), 1);
            ConfirmitTools.FillResponseTable(_framework.DbEngine, "response0", new[] { "q1", "q2" }, TotalInterviewCountForSurevy, _listCfVariables["q1"], _listCfVariables["q2"]);

            BackendTools.EnableChangeTracking(_framework.DbEngine, new[] { "respondent", "response0" }.Select(x => new TableInfo { Name = x }).ToArray());

            _surveyId = _filterAndPagingTools.CreateSurveyWithSample("p00012");
            BackendTools.AddAppointmentAndLinkItWithCall(2, _surveyId, _appointmentTime.AddDays(1));
            BackendTools.AddAppointmentAndLinkItWithCall(2, _surveyId, _appointmentTime);
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
            var respondent = _respondentList.SingleOrDefault(x => x.InterviewId == interviewId);
            var call = CallQueueService.GetCallAndNoLock(_surveyId, interviewId);

            Assert.AreEqual(interviewId, row["InterviewID"], "InterviewID, interview id = " + interviewId);

            if (call.TimeInShift == new DateTime(1899, 12, 30))
            {
                Assert.AreEqual(DBNull.Value, row["Time"], "Time, interview id = " + interviewId);
            }
            else
            {
                Assert.AreEqual(call.TimeInShift, row["Time"], "Time, interview id = " + interviewId);
            }

            Assert.AreEqual(DBNull.Value, row["ExpireTime"], "ExpireTime, interview id = " + interviewId);
            Assert.AreEqual(respondent.RespondentPhone, row["TelephoneNumber"], "TelephoneNumber, interview id = " + interviewId);
            Assert.AreEqual(respondent.RespondentName, row["RespondentName"], "RespondentName, interview id = " + interviewId);
            Assert.AreEqual(_interviewIdWithAppointment.Contains(interviewId) ? "Appointment" : "Fresh sample", row["StateName"], "StateName, interview id = " + interviewId);
            Assert.AreEqual(new DateTime(1899, 12, 30), row["LastCallTime"], "LastCallTime, interview id = " + interviewId);
            Assert.AreEqual(_interviewIdWithAppointment.Contains(interviewId)? (object)_appointmentTime : DBNull.Value, row["ApptTime"], "ApptTime, interview id = " + interviewId);
            Assert.AreEqual(respondent.DialAttempts, row["AttemptNumber"], "AttemptNumber, interview id = " + interviewId);
            Assert.AreEqual(_interviewIdWithAppointment.Contains(interviewId) ? (object)_appointmentTime.AddDays(1) : DBNull.Value, row["ExpTime"], "ExpTime, interview id = " + interviewId);
            Assert.AreEqual(TimezoneRepository.GetById(respondent.TimeZoneId).Name, row["TimezoneName"], "TimezoneName, interview id = " + interviewId);
            Assert.AreEqual(respondent.TimeZoneId, row["TimezoneID"], "TimezoneID, interview id = " + interviewId);
            Assert.AreEqual(String.Empty, row["ShiftType"], "ShiftType, interview id = " + interviewId);
            Assert.AreEqual(0, row["Shift_ID"], "Shift_ID, interview id = " + interviewId);
            Assert.AreEqual(String.Empty, row["Resource"], "Resource, interview id = " + interviewId);
            Assert.AreEqual(call.CallID, row["CallId"], "CallId, interview id = " + interviewId);

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

        public int Createinterview()
        {
            const int interviewId = TotalInterviewCountForSurevy + 1;
            var newInterview = new RespondentRecord
                {
                    Sid = interviewId.ToString(CultureInfo.InvariantCulture),
                    InterviewId = interviewId,
                    RespondentName = "resp" + interviewId,
                    RespondentPhone = interviewId.ToString(CultureInfo.InvariantCulture),
                    LastCallTime = null,
                    TotalDuration = interviewId,
                    ExtensionNumber = interviewId.ToString(CultureInfo.InvariantCulture),
                    DialAttempts = 0,
                    TimeZoneId = interviewId,
                    LastChannelId = interviewId,
                    Resource = interviewId,
                };

            _respondentList.Add(newInterview);

            ConfirmitTools.FillRespondentTable(_framework.DbEngine, new[] { newInterview }, 2);

            _backendTools.AddSample("p00012", 2, (int)SchedulingMode.Simple);

            return interviewId;
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_AllinterviewsHaveCallsAndTotalCountIsCorrect()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount);

            IsCorrectRecordSet(_respondentList.OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray(), actualRecordSet);
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_2Surveys_ReturnInterviewsFor1SurveyAndTotalCountIsCorrect()
        {
            _filterAndPagingTools.CreateSurveyWithSample("p000121");
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount);

            IsCorrectRecordSet(_respondentList.OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray(), actualRecordSet);
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_WithCFVariablesWithAliases()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount, new[] { "q1", "q2" });

            IsCorrectRecordSet(_respondentList.OrderBy(x => x.InterviewId).Select(x => x.InterviewId).ToArray(), actualRecordSet, "q1", "q2");
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_WithPagingAndOrderingByIdDesc()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(2, 2, "ID", false), out totalCount);
            IsCorrectRecordSet(_respondentList.OrderByDescending(x => x.InterviewId).Skip(1).Take(2).Select(x => x.InterviewId).ToArray(), actualRecordSet);
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetAllInterviews_OrderingByTimeToCallDescending()
        {
            var callsList = new List<BvCallEntity>();

            foreach (var respondentRecord in _respondentList)
            {
                var call = CallQueueService.GetCallAndNoLock(_surveyId, respondentRecord.InterviewId);
                callsList.Add(call);
            }

            var expectedResult = callsList.OrderByDescending(x => x.TimeInShift).Select(y => y.InterviewID).ToArray();

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All,
                new RangingArgs(1, _respondentList.Count, "Time", false),
                out totalCount);

            IsCorrectRecordSet(expectedResult, actualRecordSet);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void GetAllInterviews_FilteringByTimeToCall()
        {
            var callToChange = CallQueueService.GetCallAndNoLock(_surveyId, 10);
            callToChange.TimeInShift = new DateTime(2000, 5, 22);
            CallQueueService.UpdateCall(callToChange, 0);
            
            var filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.Or,
                new[]
                {
                    new FilterField(
                        TableTypes.Call,
                        "TimeInShift",
                        VariableTypes.Date,
                        FilterOperator.Equal,
                        new DateTime(2000, 5, 22),
                        false)
                });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.All,
                new RangingArgs(1, _respondentList.Count, "ID", false),
                out totalCount);

            Assert.AreEqual(1, totalCount);
            Assert.AreEqual(10, actualRecordSet.Rows[0]["InterviewID"]);
        }

        /// <summary>
        /// Intergration test for bug 36100.
        /// </summary>
        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_WithCallAttemptsEqualToZero()
        {
            var interviewId = Createinterview();

            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]
                { 
                    new FilterField(
                        TableTypes.Interview,
                        "AttemptNumber",
                        VariableTypes.Integer,
                        FilterOperator.Equal,
                        0,
                        false)
                }
            );

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.All, new RangingArgs(1, 20, "AttemptNumber", false), out totalCount);
            IsCorrectRecordSet(new[] { interviewId }, actualRecordSet);
            Assert.AreEqual(1, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_GetAllCallsWithRespondentNameLikeCondition_AllCallsAreReturned()
        {
            var respondent = _respondentList[0];
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]
                {
                    new FilterField(
                        TableTypes.Interview,
                        "RespondentName",
                        VariableTypes.String,
                        FilterOperator.Like,
                        respondent.RespondentName,
                        false
                    )
                }
            );

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.All, new RangingArgs(1, 20, "RespondentName", true), out totalCount);

            IsCorrectRecordSet(_respondentList.Where(x => x.RespondentName == respondent.RespondentName).Select(x => x.InterviewId).ToArray(), actualRecordSet);
            Assert.AreEqual(1, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void SampleStatusSummary_GetAllInterviewsWithSpecificResource_ResultAreCorrected()
        {
            var personName = "user1";
            var personId = PersonTools.CreatePerson(personName);
            var respondent = _respondentList[0];
            
            CallTools.AssignCalls(_surveyId, new[] {respondent.InterviewId}, personId);

            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]
                {
                    new FilterField(
                        TableTypes.Resource,
                        "Name",
                        VariableTypes.String,
                        FilterOperator.Equal,
                        personName,
                        false
                    )
                }
            );

            var result = SurveyService.GetSampleStatusSummary(_surveyId, filterId, null);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual((int)CallOutcome.FreshSample, result[0].id);
            Assert.AreEqual(1, result[0].count);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_OrderByStateName()
        {
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "StateName", true), out totalCount);

            IsCorrectRecordSet(_respondentList.Where(x => x.InterviewId == 2).Union(_respondentList).Select(x => x.InterviewId).ToArray(), actualRecordSet);
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_OrderByTimezoneNameFiterByApptTimeWithCfVariable()
        {
            BackendTools.AddAppointmentAndLinkItWithCall(_respondentList[0].InterviewId, _surveyId, _appointmentTime);
            BackendTools.AddAppointmentAndLinkItWithCall(_respondentList[TotalInterviewCountForSurevy-1].InterviewId, _surveyId, _appointmentTime);
            _interviewIdWithAppointment.Add(_respondentList[0].InterviewId);
            _interviewIdWithAppointment.Add(_respondentList[TotalInterviewCountForSurevy-1].InterviewId);

            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]
                    {
                        new FilterField(
                            TableTypes.Appointment,
                            "Time",
                            VariableTypes.Date,
                            FilterOperator.Equal,
                            _appointmentTime,
                            false)
                    });

            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.All, new RangingArgs(1, 20, "TimezoneName", true), out totalCount, "q2");

            IsCorrectRecordSet(_respondentList.Where(x => _interviewIdWithAppointment.Contains(x.InterviewId)).OrderBy(x => TimezoneRepository.GetById(x.TimeZoneId).Name).Select(x => x.InterviewId).
                Take(20).ToArray(), actualRecordSet, "q2");
            Assert.AreEqual(_interviewIdWithAppointment.Count, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_OrderByAttemptsNumber_CheckThatInterviewTableIsNotUsed()
        {
            BvInterviewAdapter.DeleteByCondition("1=1");
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(1, 20, "AttemptNumber", true), out totalCount);

            Assert.AreEqual(20, actualRecordSet.Rows.Count);
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_WithFilterByAppointmentExpTimeAndq2WithCfvariables_CheckThatInterviewTableIsNotUsed()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new[]
                    {
                        new FilterField(
                            TableTypes.Appointment,
                            "ExpTime",
                            VariableTypes.Date,
                            FilterOperator.Equal,
                            _appointmentTime.AddDays(1),
                            false),
                        new FilterField(
                            TableTypes.CFVariables,
                            "q2",
                            VariableTypes.Date,
                            FilterOperator.Equal,
                            1,
                            false)
                    });

            BvInterviewAdapter.DeleteByCondition("1=1");
            int totalCount;
            var actualRecordSet = CallManager.GetCallsRange(_surveyId, filterId, CallStates.All, new RangingArgs(1, 10, "ID", true), out totalCount, "q1");

            Assert.AreEqual(1, actualRecordSet.Rows.Count);
            IsCorrectRecordCfVariable(1, 2, actualRecordSet.Rows[0], new[] {"q1"});
            Assert.AreEqual(1, totalCount);
        }

        [TestMethod, Owner(@"FIRM\AlexanderL")]
        public void GetAllInterviews_NoRecordsInBvInterview()
        {
            BvInterviewAdapter.DeleteByCondition("1=1");
            int totalCount;
            var actualRecordset = CallManager.GetCallsRange(_surveyId, null, CallStates.All, new RangingArgs(1, 10, "ID", true), out totalCount);
            Assert.AreEqual(TotalInterviewCountForSurevy, totalCount);
            Assert.AreEqual(10, actualRecordset.Rows.Count);
            int index = 0;
            foreach (DataRow record in actualRecordset.Rows)
            {
                index += 1;

                Assert.AreEqual(index, record["InterviewID"], "InterviewID, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["Time"], "Time, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["ExpireTime"], "ExpireTime, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["TelephoneNumber"], "TelephoneNumber, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["RespondentName"], "RespondentName, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["StateName"], "StateName, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["LastCallTime"], "LastCallTime, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["ApptTime"], "ApptTime, interview id = " + index);
                Assert.AreEqual(_respondentList[index - 1].DialAttempts, record["AttemptNumber"], "AttemptNumber, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["ExpTime"], "ExpTime, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["TimezoneName"], "TimezoneName, interview id = " + index);
                Assert.AreEqual(String.Empty, record["ShiftType"], "ShiftType, interview id = " + index);
                Assert.AreEqual(String.Empty, record["Resource"], "Resource, interview id = " + index);
                Assert.AreEqual(DBNull.Value, record["CallId"], "CallId, interview id = " + index);
            }
        }

        [TestMethod, Owner("firm/vyacheslavb")]
        public void CallManagement_OrderAscByDialingMode_OrderSuccess()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new FilterField[] {}
                );

            int totalCount;
            DataTable table = CallManager.GetCallsRange(
                _surveyId, filterId, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount, "q1"
                );

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(
                _surveyId,
                new SelectedBatchParameters(new [] {
                    table.AsEnumerable().First().Field<int>("InterviewId"),
                    table.AsEnumerable().Last().Field<int>("InterviewId")}
                    ),
                DialingMode.SpecialDial
                );

            table = CallManager.GetCallsRange(
                _surveyId, filterId, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "DialingMode", true), out totalCount, "q1"
                );

            for (int i = 0, j = 1; i < table.Rows.Count && j < table.Rows.Count; i++, j++)
            {
                Assert.IsTrue(table.Rows[i].Field<byte>("DialingMode") <= table.Rows[j].Field<byte>("DialingMode"));
            }
        }

        [TestMethod, Owner("firm/vyacheslavb")]
        public void CallManagement_OrderDescByDialingMode_OrderSuccess()
        {
            int filterId = FilterAndPagingTools.CreateSimpleFilter(
                _surveyId,
                AndOrOperator.And,
                new FilterField[] { }
                );

            int totalCount;
            DataTable table = CallManager.GetCallsRange(
                _surveyId, filterId, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "ID", true), out totalCount, "q1"
                );

            new TestCallManagementOperationFactory().ChangeDialModeOfInterviews(
                _surveyId,
                new SelectedBatchParameters(new[] {
                    table.AsEnumerable().First().Field<int>("InterviewId"),
                    table.AsEnumerable().Last().Field<int>("InterviewId")}
                    ),
                DialingMode.SpecialDial
                );

            table = CallManager.GetCallsRange(
                _surveyId, filterId, CallStates.All, new RangingArgs(1, TotalInterviewCountForSurevy, "DialingMode", false), out totalCount, "q1"
                );

            for (int i = 0, j = 1; i < table.Rows.Count && j < table.Rows.Count; i++, j++)
            {
                Assert.IsTrue(table.Rows[i].Field<byte>("DialingMode") >= table.Rows[j].Field<byte>("DialingMode"));
            }
        }
    }
}
