using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class AttemptsByDespositionReportTest
    {
        private readonly IntegrationTestingFramework _framework = IntegrationTestingFramework.Instance;
        private BackendTools _backendTools;
        private static TestDataContext _context;

        [TestInitialize]
        public void TestInitialize()
        {
            _framework.TestInitialize();
            _framework.BackendInitialize();
            _backendTools = new BackendTools(_framework);
            _backendTools.LaunchAllHoursScript();
            PrepareTestData();
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
                        Name = "Interviewer",
                        CallCenter = "CC1"
                    },
                    new PersonData()
                    {
                        Tag = "P2",
                        Name = "Interviewer_2",
                        CallCenter = "CC2"
                    }
                },
                CallCenters = new[] {
                    new CallCenterData() { Tag = "CC1" }, 
                    new CallCenterData() { Tag = "CC2" }
                },
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1",
                        AssignsS = "P1",
                        CallCenters = new[] { "CC1" },
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1",
                                History = new[]
                                {
                                    new InterviewHisotryData() {Time = "2022-01-01T09:00:00", ITS = CallOutcome.Busy},
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1", ITS = CallOutcome.Appointment,
                                        Duration = 120
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1", ITS = CallOutcome.Completed,
                                        Duration = 130
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I2",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T09:00:00", Person = "P1", ITS = CallOutcome.AnswerMachine,
                                        Duration = 210
                                    },
                                    new InterviewHisotryData()
                                        {Time = "2022-01-02T09:00:00", ITS = CallOutcome.NoReply},
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1", ITS = CallOutcome.Terminated,
                                        Duration = 230
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I3",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T09:00:00", Person = "P1", ITS = CallOutcome.InternalTransfer,
                                        Duration = 310
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1", ITS = CallOutcome.InboundCall,
                                        Duration = 320
                                    },
                                    new InterviewHisotryData() {Time = "2022-01-03T10:00:00", ITS = CallOutcome.Busy}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I4",
                                History = new[]
                                {
                                    new InterviewHisotryData() {Time = "2022-01-01T09:00:00", ITS = CallOutcome.Busy},
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1",
                                        ITS = CallOutcome.SurveyScriptError, Duration = 420
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1",
                                        ITS = CallOutcome.InterruptedByInterviewer, Duration = 430
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I5",
                                TelephoneNumber = "1234567",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T15:00:00", Person = "P1",
                                        ITS = CallOutcome.InterruptedBySystem, Duration = 510,
                                        TelephoneNumber = "1234567"
                                    },
                                    new InterviewHisotryData()
                                        {Time = "2022-01-01T10:00:00", ITS = CallOutcome.NoReply},
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1", ITS = CallOutcome.CanceledTransfer,
                                        Duration = 530
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I6",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T09:00:00", Person = "P1", ITS = CallOutcome.Congestion,
                                        Duration = 610
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1", ITS = CallOutcome.QuotaFail,
                                        Duration = 620
                                    },
                                    new InterviewHisotryData() {Time = "2022-01-03T10:00:00", ITS = CallOutcome.Busy}
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I7",
                                TelephoneNumber = "7654321",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1", ITS = CallOutcome.Error,
                                        Duration = 720
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1", ITS = CallOutcome.Screened,
                                        Duration = 730
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I8",
                                TelephoneNumber = "7654321",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:10:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:20:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:30:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:40:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T12:00:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T12:00:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I9",
                                TelephoneNumber = "7654321",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T12:00:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:00:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:10:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:20:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:30:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:40:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:45:00", ITS = CallOutcome.Modem,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:50:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T12:55:00", ITS = CallOutcome.Busy,
                                        TelephoneNumber = "7654321"
                                    },
                                }
                            }

                        }
                    },
                     new SurveyData()
                    {
                        Tag = "S2",
                        AssignsS = "P1,P2",
                        CallCenters = new[] { "CC1", "CC2" },
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1", ITS = CallOutcome.Appointment, 
                                        Duration = 120
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1", ITS = CallOutcome.Completed,
                                        Duration = 130
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I2",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T09:00:00", Person = "P1", ITS = CallOutcome.AnswerMachine,
                                        Duration = 210
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1", ITS = CallOutcome.Terminated,
                                        Duration = 230
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I3",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T09:00:00", Person = "P1", ITS = CallOutcome.InternalTransfer,
                                        Duration = 310
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1", ITS = CallOutcome.InboundCall,
                                        Duration = 320
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I4",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P1",
                                        ITS = CallOutcome.SurveyScriptError, Duration = 420
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P1",
                                        ITS = CallOutcome.InterruptedByInterviewer, Duration = 430
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I5",
                                TelephoneNumber = "1234567",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T15:00:00", Person = "P2",
                                        ITS = CallOutcome.InterruptedBySystem, Duration = 510,
                                        TelephoneNumber = "1234567"
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-03T10:00:00", Person = "P2", ITS = CallOutcome.CanceledTransfer,
                                        Duration = 530
                                    }
                                }
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I6",
                                History = new[]
                                {
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-01T09:00:00", Person = "P2", ITS = CallOutcome.Congestion,
                                        Duration = 610
                                    },
                                    new InterviewHisotryData()
                                    {
                                        Time = "2022-01-02T09:00:00", Person = "P2", ITS = CallOutcome.QuotaFail,
                                        Duration = 620
                                    }
                                }
                            }
                        }
                    }
                }
            }.Create();
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void AttemptsByDespositionReport_HideEmpty()
        {
            var startDate = new DateTime(2022, 1, 2, 0, 0, 0);
            var endDate = DateTime.Now;

            List<int> surveys = new List<int>();
            List<int> its = new List<int>();
            bool hideEmpty = true;

            var survey = _context.GetSurvey("S1");
            surveys.Add(survey.Id);

            (CallOutcome it, int[] attemts)[] expected = new[]
            {
                (
                    CallOutcome.Appointment,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Busy,
                    new[] { 1,3,1,1,1,1,0,0,1,1,0}
                ),
                (
                    CallOutcome.Completed,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Screened,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.InterruptedByInterviewer,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Error,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.InboundCall,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.QuotaFail,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Terminated,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.SurveyScriptError,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                )
                ,
                (
                    CallOutcome.CanceledTransfer,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Modem,
                    new[] { 1,1,1,1,1,1,2,2,0,0,0 }
                ),
                (
                    CallOutcome.NoReply,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                )
            };

            var report = ReportManager.GetAttemptsByDispositionReportData(survey.Id, its, hideEmpty, startDate, endDate);

            Assert.IsNotNull(report);
            Assert.AreEqual(expected.Length, report.Count);
            AssertIts(expected, report);
        }
        
        [TestMethod, Owner(@"FIRM\EgorK")]
        public void AttemptsByDespositionReport_FilterByCallCenter()
        {
            var startDate = new DateTime(2022, 1, 2, 0, 0, 0);
            var endDate = DateTime.Now;

            List<int> surveys = new List<int>();
            List<int> its = new List<int>();
            bool hideEmpty = true;

            var survey1 = _context.GetSurvey("S2");
            surveys.Add(survey1.Id);

            (CallOutcome it, int[] attemts)[] expected1 = new[]
            {
                (
                    CallOutcome.Appointment,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Completed,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.InterruptedByInterviewer,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.InboundCall,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Terminated,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.SurveyScriptError,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                )
            };

            var report1 = ReportManager.GetAttemptsByDispositionReportData(survey1.Id, its, hideEmpty, startDate, endDate, _context.GetCallCenter("CC1").Id);

            Assert.IsNotNull(report1);
            Assert.AreEqual(expected1.Length, report1.Count);
            AssertIts(expected1, report1);
            
            (CallOutcome it, int[] attemts)[] expected2 = new[]
            {
                (
                    CallOutcome.QuotaFail,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.CanceledTransfer,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                )
            };
            
            var report2 = ReportManager.GetAttemptsByDispositionReportData(survey1.Id, its, hideEmpty, startDate, endDate, _context.GetCallCenter("CC2").Id);
            Assert.IsNotNull(report2);
            Assert.AreEqual(expected2.Length, report2.Count);
            AssertIts(expected2, report2);
        }

        [TestMethod, Owner(@"FIRM\EvgeniiL")]
        public void AttemptsByDespositionReport_ShowEmpty_WithItsList_CountBusy11plus()
        {
            var startDate = new DateTime(2022, 1, 1, 0, 0, 0);
            var endDate = DateTime.Now;

            List<int> surveys = new List<int>();
            List<int> its = new List<int>();
            bool hideEmpty = false;

            var survey = _context.GetSurvey("S1");
            surveys.Add(survey.Id);

            (CallOutcome it, int[] attemts)[] expected = new[]
            {
                (
                    CallOutcome.Appointment,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                    ),
                (
                    CallOutcome.Busy,
                    new[] { 5,2,4,2,2,2,2,2,1,1,2 }
                    ),
                (
                    CallOutcome.Completed,
                    new[] { 0,0,1,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Congestion,
                    new[] { 1,0,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.InterruptedByInterviewer,
                    new[] { 0,0,1,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.InterruptedBySystem,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.NoReply,
                    new[] { 1,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.QuotaFail,
                    new[] { 0,1,0,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.Terminated,
                    new[] { 0,0,1,0,0,0,0,0,0,0,0 }
                ),
                (
                    CallOutcome.FreshSample,
                    new[] { 0,0,0,0,0,0,0,0,0,0,0 }
                )
            };

            its.AddRange(expected.Select(e => (int)e.it));

            var report = ReportManager.GetAttemptsByDispositionReportData(survey.Id, its, hideEmpty, startDate, endDate);

            Assert.IsNotNull(report);
            Assert.AreEqual(its.Count, report.Count);
            AssertIts(expected, report);
        }

        public void AssertIts((CallOutcome it, int[] attempts)[] extected,
            List<AttemptsByDispositionReportItem> report)
        {
            foreach (var item in extected)
            {
                var itFromReport = report.FirstOrDefault(it => it.Code == (int)item.it);
                if (itFromReport != null)
                {
                    Assert.AreEqual(item.attempts[0], itFromReport.Attempts1);
                    Assert.AreEqual(item.attempts[1], itFromReport.Attempts2);
                    Assert.AreEqual(item.attempts[2], itFromReport.Attempts3);
                    Assert.AreEqual(item.attempts[3], itFromReport.Attempts4);
                    Assert.AreEqual(item.attempts[4], itFromReport.Attempts5);
                    Assert.AreEqual(item.attempts[5], itFromReport.Attempts6);
                    Assert.AreEqual(item.attempts[6], itFromReport.Attempts7);
                    Assert.AreEqual(item.attempts[7], itFromReport.Attempts8);
                    Assert.AreEqual(item.attempts[8], itFromReport.Attempts9);
                    Assert.AreEqual(item.attempts[9], itFromReport.Attempts10);
                    Assert.AreEqual(item.attempts[10], itFromReport.Attempts11AndMore);
                }
            }
        }
    }

}
