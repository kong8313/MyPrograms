using System;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class CallAttemptsReportTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private static TestDataContext _context;
        private static AssertParameters _callAttemptParameters1;
        private static AssertParameters _callAttemptParameters2;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _framework.TestCleanup();
        }

        

        private static void PrepareTestData()
        {
            _context = new TestData
            {
                Supervisors = new[]
                {
                    new SupervisorData()
                    {
                        Tag = "SV",
                        Name = "Supervisor",
                        Surveys = new[] {"S1"}
                    }
                },
                Persons = new[]
                {
                    new PersonData()
                    {
                        Tag = "P1",
                        Name = "Interviewer"
                    }
                },
                Surveys = new[]
                {
                    new SurveyData() {
                        Tag = "S1",
                        AssignsS = "P1",
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T09:00:00", ITS = CallOutcome.Busy},
                                    new InterviewHisotryData(){Time = "2019-02-01T09:00:00", Person = "P1", ITS = CallOutcome.Appointment, Duration = 120},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", Person = "P1", ITS = CallOutcome.Completed, Duration = 130}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I2",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T09:00:00", Person = "P1", ITS = CallOutcome.AnswerMachine, Duration = 210},
                                    new InterviewHisotryData(){Time = "2019-02-01T09:00:00", ITS = CallOutcome.NoReply},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", Person = "P1", ITS = CallOutcome.Terminated, Duration = 230}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I3",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T09:00:00", Person = "P1", ITS = CallOutcome.InternalTransfer, Duration = 310},
                                    new InterviewHisotryData(){Time = "2019-02-01T09:00:00", Person = "P1", ITS = CallOutcome.InboundCall, Duration = 320},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", ITS = CallOutcome.Busy}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I4",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T09:00:00", ITS = CallOutcome.Busy},
                                    new InterviewHisotryData(){Time = "2019-02-01T09:00:00", Person = "P1", ITS = CallOutcome.SurveyScriptError, Duration = 420},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", Person = "P1", ITS = CallOutcome.InterruptedByInterviewer, Duration = 430}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I5",
                                TelephoneNumber = "1234567",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T15:00:00", Person = "P1", ITS = CallOutcome.InterruptedBySystem, Duration = 510, TelephoneNumber = "1234567"},
                                    new InterviewHisotryData(){Time = "2019-01-01T10:00:00", ITS = CallOutcome.NoReply},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", Person = "P1", ITS = CallOutcome.CanceledTransfer, Duration = 530}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I6",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T09:00:00", Person = "P1", ITS = CallOutcome.Congestion, Duration = 610},
                                    new InterviewHisotryData(){Time = "2019-02-01T09:00:00", Person = "P1", ITS = CallOutcome.QuotaFail, Duration = 620},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", ITS = CallOutcome.Busy}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I7",
                                TelephoneNumber = "7654321",
                                History = new []
                                {
                                    new InterviewHisotryData(){Time = "2019-01-01T12:00:00", ITS = CallOutcome.Busy, TelephoneNumber = "7654321"},
                                    new InterviewHisotryData(){Time = "2019-02-01T09:00:00", Person = "P1", ITS = CallOutcome.Error, Duration = 720},
                                    new InterviewHisotryData(){Time = "2019-03-01T10:00:00", Person = "P1", ITS = CallOutcome.Screened, Duration = 730}
                                }
                            }
                        }
                    }
                }
            }.Create();

            var survey = _context.GetSurvey("S1");
            var interview5 = _context.GetInterview("S1.I5");
            var interview7 = _context.GetInterview("S1.I7");

            _callAttemptParameters1 = new AssertParameters()
            {
                SurveySID = survey.Model.SID,
                SurveyName = survey.Model.Name,
                InterviewerName = "Interviewer",
                InterviewId = interview5.Model.ID,
                TelephoneNumber = interview5.Model.TelephoneNumber,
                ExtendedStatus = 26,
                CallDuration = 510
            };

            _callAttemptParameters2 = new AssertParameters()
            {
                SurveySID = survey.Model.SID,
                SurveyName = survey.Model.Name,
                InterviewerName = "Not Applicable",
                InterviewId = interview7.Model.ID,
                TelephoneNumber = interview7.Model.TelephoneNumber,
                ExtendedStatus = 2,
                CallDuration = 0
            };
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void CallAttemptsReport_ExcludeDisposedByDialerAttempts()
        {
            PrepareTestData();

            var report = BvSpGetCallAttemptsReport_ListPageAdapter.ExecuteEntityList("Supervisor", 1, 50, "EventDate", 0, "", false,
                out var callAttempts);
            Assert.AreEqual(14, callAttempts);

            var calAttempt = report.FirstOrDefault(x => x.EventDate == new DateTime(2019, 1, 1, 15, 0, 0));
            AssertCallAttempt(calAttempt, _callAttemptParameters1);

            var disposedByDialerAttempt = report.FirstOrDefault(x => x.EventDate == new DateTime(2019, 1, 1, 12, 0, 0));
            Assert.IsNull(disposedByDialerAttempt);
        }

        [TestMethod, Owner(@"FIRM\LiubovK")]
        public void CallAttemptsReport_IncludeDisposedByDialerAttempts()
        {
            PrepareTestData();

            var report = BvSpGetCallAttemptsReport_ListPageAdapter.ExecuteEntityList("Supervisor", 1, 50, "EventDate", 0, "", true,
                out var callAttempts);
            Assert.AreEqual(21, callAttempts);

            var disposedByDialerAttempt = report.FirstOrDefault(x => x.EventDate == new DateTime(2019, 1, 1, 12, 0, 0));
            AssertCallAttempt(disposedByDialerAttempt, _callAttemptParameters2);

            var calAttempt = report.FirstOrDefault(x => x.EventDate == new DateTime(2019, 1, 1, 15, 0, 0));
            AssertCallAttempt(calAttempt, _callAttemptParameters1);
        }

        private static void AssertCallAttempt(BvSpGetCallAttemptsReport_ListPageEntity attemptToAssert, AssertParameters parameters)
        {
            Assert.AreEqual(parameters.SurveySID, attemptToAssert.SurveySID);
            Assert.AreEqual(parameters.SurveyName, attemptToAssert.ProjectID);
            Assert.AreEqual(parameters.InterviewerName, attemptToAssert.InterviewerName);
            Assert.AreEqual(parameters.InterviewId, attemptToAssert.InterviewID);
            Assert.AreEqual(parameters.TelephoneNumber, attemptToAssert.TelephoneNumber);
            Assert.AreEqual(parameters.ExtendedStatus, (int)attemptToAssert.ExtendedStatus);
            Assert.AreEqual(parameters.CallDuration, attemptToAssert.CallDuration);
        }
    }

    public class AssertParameters
    {
        public int SurveySID;
        public string SurveyName;
        public string InterviewerName;
        public int InterviewId;
        public string TelephoneNumber;
        public int ExtendedStatus;
        public int CallDuration;
    }
}
