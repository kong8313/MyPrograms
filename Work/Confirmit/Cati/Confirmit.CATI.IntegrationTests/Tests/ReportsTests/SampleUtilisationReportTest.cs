using System;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.AsyncOperations.Operations.RestoreSurvey;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.ReportsTests
{
    [TestClass]
    public class SampleUtilisationReportTest : BaseMockedIntegrationTest
    {
        private IInterviewService _interviewService;
        private readonly DateTime _baseDateTime = new DateTime(2000, 1, 1);    // this is just a time

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _interviewService = ServiceLocator.Resolve<IInterviewService>();
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void SampleWith5Records_2RecordsWith2CallAttemps_1RecordWith1_2Completes_1NotComplete_FCD2Records_OneRecordDeleted()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true, SchedulingScript = AllHoursSchedule.Name,
                        Forms = new[]
                        {
                            new SingleFormData() {Name = "q1", Precodes = new[] {"1", "2"}}
                        },
                        Quotas = new[]
                        {
                            new QuotaData()
                            {
                                Id = 1, Name = "quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData() {Id = 1, Values = "q1=1", Counter = 0, Limit = 2},
                                    new CellData() {Id = 2, Values = "q1=2", Counter = 0, Limit = 3},
                                }
                            }
                        },
                        Interviews = new[] {
                            new InterviewData() {Tag = "S1.I1", Data = "q1=1",ITS = CallOutcome.FreshSample,Call = new CallData() {Priority = 2}},
                            new InterviewData() {Tag = "S1.I2", Data = "q1=1",ITS = CallOutcome.FreshSample,Call = new CallData() {Priority = 2}},
                            new InterviewData() {Tag = "S1.I3", Data = "q1=1",ITS = CallOutcome.FreshSample,Call = new CallData()},
                            new InterviewData() {Tag = "S1.I4", Data = "q1=2",ITS = CallOutcome.FreshSample,Call = new CallData()},
                            new InterviewData() {Tag = "S1.I5", Data = "q1=2",ITS = CallOutcome.FreshSample,Call = new CallData()},
                        },
                        Assigns = new[] {"P1"}
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            ServiceLocator.Resolve<IReplicationService>().RunForceReplication();

            var survey = context.GetSurvey("S1");
            var quota = survey.GetQuota("quota1");
            var person = context.GetPerson("P1");

            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();

            var interview = console.StartInterview();
            var nextInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });
            console.FinishInterview(nextInterview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });

            CallTools.ActivateCalls(survey.Id, 3, CallStates.Scheduled, new int[] { }, (int)CallShiftType.None, BvCallEntity.TimeInsteadNowTimeToCall,
                false, new int[] { interview.Id, nextInterview.Id });

            interview = console.StartInterview();
            nextInterview = console.NextInterview(interview, new CompletedInterviewDetails { Its = "13", InterviewDuration = 10, Status = "Complete" });
            console.FinishInterview(nextInterview, new CompletedInterviewDetails { Its = "2", InterviewDuration = 10, Status = "Complete" });

            quota.CloseCellById(1);

            interview = console.StartInterview();
            console.FinishInterview(interview, new CompletedInterviewDetails { Its = "31", InterviewDuration = 10, Status = "Complete" });

            _interviewService.DeleteRespondents(survey.Id, new int[] { context.GetInterview("S1.I5").Id }, default);
            
            // Copy some rows from BvCallHistoryEx to BvCallHistory to check that BvSpSampleUtilisationReport gets data from both tables
            BackendTools.CopyCallHistoryExToCallHistory(6);
            
            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(survey.Id, "13,31", null, null).Single();

            Assert.AreEqual(5, result.InterviewsAdded);
            Assert.AreEqual(2, result.InterviewsCompleted, "Number of completed is incorrect");
            Assert.AreEqual(3, result.InterviewsAttempted, "Number of attempted is incorrect");
            Assert.AreEqual(4, result.InterviewsCurrent, "Current amount of records incorrect");
            Assert.AreEqual(1, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(1, result.BlockedAttemptedInterviews, "Number of blocked attempted is incorrect");
            Assert.AreEqual(0, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
            Assert.AreEqual(1.5, result.AvgAttemptsPerComplete, "AverageAttemptsPerComplete is incorrect");
            Assert.AreEqual(1.5, result.AttemptedInterviewsPerComplete, "Attempts per complete is incorrect");
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void TwoHistoryAttempts_LastHistoryAttemptAfterDisabledByFcdEvent_AttemptedAfterBlockedEqualToOne()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(3), ITS = CallOutcome.Busy, Person = "P1" }
                                },
                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(2),OperationType = OperationType.DisableByFcd}
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(1, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void TwoHistoryAttempts_LastHistoryAttemptAfterDeleteByFcdEvent_AttemptedAfterBlockedEqualToOne()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(3), ITS = CallOutcome.Busy, Person = "P1" }
                                },
                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(2),OperationType = OperationType.DeleteCallsByFcd}
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(1, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void TwoHistoryAttempts_BlockByFcdIsLastEvent_BlockedAttemptedEqualToOne()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(2), ITS = CallOutcome.Busy, Person = "P1" }
                                },
                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(3),OperationType = OperationType.DeleteCallsByFcd}
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(0, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
            Assert.AreEqual(0, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(1, result.BlockedAttemptedInterviews, "Number of blocked attempted is incorrect");
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void NoHistoryAttempts_BlockByFcdEvent_BlockedNotAttemptedEqualToOne()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,

                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(3),OperationType = OperationType.DeleteCallsByFcd}
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(0, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
            Assert.AreEqual(1, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(0, result.BlockedAttemptedInterviews, "Number of blocked attempted is incorrect");
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void MultipleHistoryAndCallHistoryEvents_BlockByFcdIsLastEvent_BlockedAttemptedEqualToOne()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(3), ITS = CallOutcome.Busy, Person = "P1" },
                                    new CallHistoryData {FiredTime = FiredTime(5), ITS = CallOutcome.Busy, Person = "P1" }

                                },
                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(2),OperationType = OperationType.DeleteCallsByFcd},
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(4),OperationType = OperationType.DeleteCallsByFcd},
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(6),OperationType = OperationType.DeleteCallsByFcd}
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(0, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
            Assert.AreEqual(0, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(1, result.BlockedAttemptedInterviews, "Number of blocked attempted is incorrect");
            Assert.AreEqual(1, result.InterviewsCurrent, "Current amount of records is incorrect");
        }


        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void FirstInterviewWithMultipleHistoryAndCallHistoryEvents_SecondWithOneDisabledFcd_BlockedAttemptedAndBlockedNotAttemptedEqualToOne()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(3), ITS = CallOutcome.Busy, Person = "P1" },
                                    new CallHistoryData {FiredTime = FiredTime(5), ITS = CallOutcome.Busy, Person = "P1" }
                                },
                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(2),OperationType = OperationType.DeleteCallsByFcd},
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(4),OperationType = OperationType.DeleteCallsByFcd},
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(6),OperationType = OperationType.DeleteCallsByFcd}
                                },
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I2", ITS = CallOutcome.Busy,
                                ExtendedCallHistory = new []
                                {
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(2),OperationType = OperationType.DeleteCallsByFcd},
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(4),OperationType = OperationType.DeleteCallsByFcd},
                                   new ExtendedCallHistoryData {FiredTime = FiredTime(6),OperationType = OperationType.DeleteCallsByFcd}
                                }
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(0, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
            Assert.AreEqual(1, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(1, result.BlockedAttemptedInterviews, "Number of blocked attempted is incorrect");
            Assert.AreEqual(2, result.InterviewsCurrent, "Current amount of records is incorrect");
        }

        [TestMethod, Owner(@"FIRM\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void MultipleHistoryAtempts_OneNotAttempted_NoCallHistoryEvents_BlockedAttemptedEqualToZero()
        {
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData()
                    {
                        Tag = "S1", IsUseDb = true,
                        Interviews = new[]
                        {
                            new InterviewData()
                            {
                                Tag = "S1.I1", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(3), ITS = CallOutcome.Busy, Person = "P1" },
                                    new CallHistoryData {FiredTime = FiredTime(5), ITS = CallOutcome.Busy, Person = "P1" }

                                },
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I2", ITS = CallOutcome.Busy,
                                CallHistory = new []
                                {
                                    new CallHistoryData {FiredTime = FiredTime(1), ITS = CallOutcome.Busy, Person = "P1"},
                                    new CallHistoryData {FiredTime = FiredTime(3), ITS = CallOutcome.Busy, Person = "P1" },
                                    new CallHistoryData {FiredTime = FiredTime(5), ITS = CallOutcome.Busy, Person = "P1" }

                                },
                            },
                            new InterviewData()
                            {
                                Tag = "S1.I3", ITS = CallOutcome.Busy,
                            },
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
            }.Create();

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Single();

            Assert.AreEqual(0, result.AttemptedAfterBlocked, "Number of attempted after blocked is incorrect");
            Assert.AreEqual(0, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(0, result.BlockedAttemptedInterviews, "Number of blocked attempted is incorrect");
            Assert.AreEqual(3, result.InterviewsCurrent, "Current amount of records is incorrect");
            Assert.AreEqual(2, result.InterviewsAttempted);
        }

        [TestMethod, Owner(@"Firm\LeonidS"), TestCategory(TestsCategoriesNames.SampleUtilisationReport)]
        public void AddSample_TwoTelNumbersAreInBlacklist_BlockedByBlacklistEqualsTwo()
        {
            var context = new TestData()
            {
                Surveys = new[] {new SurveyData()
                {
                    Tag = "S1", IsUseDb = true, IsSupportBlackList = true,
                    Forms = new[] {
                        new SingleFormData(){Name="q1", Precodes = new []{"1", "2"}}
                    },
                    Quotas = new [] {
                            new QuotaData(){ Id = 1, Name="quota1", Fields = new[] {"q1"},
                                Cells = new[]
                                {
                                    new CellData(){Id = 1, Values="q1=1", Counter=0, Limit=0},
                                    new CellData(){Id = 2, Values="q1=2", Counter=0, Limit=1},
                                }
                            }
                        }
                }},
                TelephoneBlacklist = new[] { "88001001010" }
            }.Create();

            var survey = context.GetSurvey("S1");

            var interviews = new[]
            {
                new InterviewData() {Tag = "S1.I1", TelephoneNumber = "88001001010", Data="q1="},
                new InterviewData() {Tag = "S1.I2", TelephoneNumber = "88001001010", Data="q1=1"},
                new InterviewData() {Tag = "S1.I3", TelephoneNumber = "88001001011", Data="q1="}
            };

            survey.AddSample(SchedulingMode.Simple, interviews);

            var result = BvSpSampleUtilisationReportAdapter.ExecuteEntityList(context.GetSurvey("S1").Id, "13", null, null).Last();

            Assert.AreEqual(0, result.BlockedExcludedAttemptedInterviews, "Number of blocked excluded attempted is incorrect");
            Assert.AreEqual(3, result.InterviewsCurrent, "Current amount of records is incorrect");
            Assert.AreEqual(2, result.BlockedByBlacklist);
        }

        private DateTime FiredTime(double minutes)
        {
            return _baseDateTime.AddMinutes(minutes);
        }
    }
}
