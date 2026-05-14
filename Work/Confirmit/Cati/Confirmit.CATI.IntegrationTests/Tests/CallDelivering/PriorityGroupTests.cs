using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.Test.Common.Attributes;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.IntegrationTests.Tests.AsyncOperations;

using ConfirmitDialerInterface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.CallDelivering
{
    [TestClass]
    public class PriorityGroupTests : BaseMockedIntegrationTest
    {
        private const string User = "user";
        private const string Password = "pwd";

        private ISurveyStateService _surveyStateService;

        [TestInitialize]
        public new void TestInitialize()
        {
            base.TestInitialize();
            _surveyStateService = ServiceLocator.Resolve<ISurveyStateService>();
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullScheduling_CallGroupHasDifferentPriorityForITS_CallIsDeliveredOnlyForAssingITSs()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewCallPriority, "1" ),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID % 3 == 1"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID % 3 == 2"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID % 3 == 0")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 6},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 5}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 1, 4, 7, 10, 2, 5, 8 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeliveryCallsWithDifferentAssignemtnsOnCalls_PersonIsIncludedToSubsetOfGroups_SubsetCallsAreDeliveredInCorrectOrder()
        {
            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    Tag="S1", IsOpen = true, IsCallGroupEnabled = true,
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(){ Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "PG1"}},
                        new InterviewData(){ Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "PG2"}},
                        new InterviewData(){ Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I5", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P2"}},
                        new InterviewData(){ Tag="S1.I6", ITS=CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(){ Tag="S1.I7", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "PG1"}},
                        new InterviewData(){ Tag="S1.I8", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "PG2"}},
                        new InterviewData(){ Tag="S1.I9", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I10", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "P2"}},
                    },
                }},
                Persons = new[]{
                    new PersonData { Tag="P1", Memberships = "PG1", CallGroup="CG1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData { Tag="P2", Memberships = "PG2", CallGroup="CG1", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                PersonGroups = new[]{
                    new PersonGroupData(){Tag="PG1"},
                    new PersonGroupData(){Tag="PG2"}
                },
                CallGroups = new[] { new CallGroupData() { Tag = "CG1", ITS = new[] { CallOutcome.Busy } } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var expected = context.GetInterviewsInOrder("S1.I7", "S1.I9");

            var actual = console.ProcessAllInterviews();

            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray());
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void DeliveryCallsWithDifferentAssignemtnsOnCallsAndSurvey_PersonIsIncludedToSubsetOfGroups_SubsetCallsAreDeliveredInCorrectOrder()
        {
            var context = new TestData()
            {
                Surveys = new[]{ new SurveyData()
                {
                    Tag="S1", IsOpen = true, IsCallGroupEnabled = true,
                    Assigns = new []{"PG1"},
                    Interviews = new[] {
                        new InterviewData(){ Tag="S1.I1", ITS=CallOutcome.FreshSample, Call = new CallData()},
                        new InterviewData(){ Tag="S1.I2", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "PG1"}},
                        new InterviewData(){ Tag="S1.I3", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "PG2"}},
                        new InterviewData(){ Tag="S1.I4", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I5", ITS=CallOutcome.FreshSample, Call = new CallData(){Resource = "P2"}},
                        new InterviewData(){ Tag="S1.I6", ITS=CallOutcome.Busy, Call = new CallData()},
                        new InterviewData(){ Tag="S1.I7", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "PG1"}},
                        new InterviewData(){ Tag="S1.I8", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "PG2"}},
                        new InterviewData(){ Tag="S1.I9", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "P1"}},
                        new InterviewData(){ Tag="S1.I10", ITS=CallOutcome.Busy, Call = new CallData(){Resource = "P2"}},
                    },
                }},
                Persons = new[]{
                    new PersonData { Tag="P1", Memberships = "PG1", CallGroup="CG1", TaskChoice = TaskChoiceMode.SurveyAssignment},
                    new PersonData { Tag="P2", Memberships = "PG2", CallGroup="CG1", TaskChoice = TaskChoiceMode.SurveyAssignment}
                },
                PersonGroups = new[]{
                    new PersonGroupData(){Tag="PG1"},
                    new PersonGroupData(){Tag="PG2"}
                },
                CallGroups = new[] { new CallGroupData() { Tag = "CG1", ITS = new[] { CallOutcome.Busy } } }
            }.Create();

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var console = new AutomaticConsoleController(context, person, survey);

            console.Login();
            var expected = context.GetInterviewsInOrder("S1.I7", "S1.I9", "S1.I6");

            var actual = console.ProcessAllInterviews();

            CollectionAssert.AreEqual(
                expected.Select(x => x.Id).ToArray(),
                actual.Select(x => x.Id).ToArray());
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullScheduling_CallGroupHasEqualsPriorityForITS_CallIsDeliveredOnlyForAssingITSs()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewCallPriority, "1" ),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 3"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 3 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 10")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 5}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 1, 4, 2, 5, 3, 6 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullScheduling_CallGroupHasDifferentPriorityForITS_CallIsDeliveredInPriorityOrderTakeIntoAccountRoundRobin()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewCallPriority, "1" ),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewITS, "4", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 8"),
                    new Action(Action.Operation.SetNewITS, "5", "Scheduling.Interview.ID > 8 && Scheduling.Interview.ID  <= 10")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 3, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 6},
                    new BvCallGroupConditionEntity{ConditionValue = 5, ConditionPriority = 6}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 7, 9, 8, 10, 1, 3, 5, 2, 4, 6 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullScheduling_CallGroupAndCallsHaveDifferentPriority_CallIsDeliveredInPriorityOrderTakeIntoAccountRoundRobin()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewCallPriority, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewCallPriority, "1", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewCallPriority, "1", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewITS, "4", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 8"),
                    new Action(Action.Operation.SetNewCallPriority, "2", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 8"),
                    new Action(Action.Operation.SetNewITS, "5", "Scheduling.Interview.ID > 8 && Scheduling.Interview.ID  <= 10"),
                    new Action(Action.Operation.SetNewCallPriority, "2", "Scheduling.Interview.ID > 8 && Scheduling.Interview.ID  <= 10")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 6},
                    new BvCallGroupConditionEntity{ConditionValue = 3, ConditionPriority = 6},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 4},
                    new BvCallGroupConditionEntity{ConditionValue = 5, ConditionPriority = 5}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 9, 10, 7, 8, 3, 5, 4, 6, 1, 2 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullScheduling_GetCallForSurveyAssignMode_CallIsDeliveredByCallPriority()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewCallPriority, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewCallPriority, "1", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewCallPriority, "1", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewITS, "4", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 8"),
                    new Action(Action.Operation.SetNewCallPriority, "2", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 8"),
                    new Action(Action.Operation.SetNewITS, "5", "Scheduling.Interview.ID > 8 && Scheduling.Interview.ID  <= 10"),
                    new Action(Action.Operation.SetNewCallPriority, "2", "Scheduling.Interview.ID > 8 && Scheduling.Interview.ID  <= 10")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var personId = CreatePerson(survey.SID, null);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 7, 8, 9, 10, 1, 2, 3, 4, 5, 6 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithSimpleScheduling_MoveCallToSpecificITS_CallIsDeliveredOnlyForSpecificITSOrderTakeIntoAccountRoundRobin()
        {
            BackendToolsObject.LaunchAllHoursScript();

            var survey = CreateSurvey(BackendTools.GetAllHoursID(), 4, SchedulingMode.Simple);

            CallTools.MoveCalls(survey.SID, new[] { 2, 4 }, 1);

            var conditions =
                new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 6}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 2, 4 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithSimpleScheduling_MoveCallToSpecificITS_CallIsDeliveredOnlyForFreshSampleOrderTakeIntoAccountRoundRobin()
        {
            BackendToolsObject.LaunchAllHoursScript();

            var survey = CreateSurvey(BackendTools.GetAllHoursID(), 4, SchedulingMode.Simple);

            CallTools.MoveCalls(survey.SID, new[] { 1, 4 }, 1);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity { ConditionValue = 16, ConditionPriority = 5 },
                    new BvCallGroupConditionEntity { ConditionValue = 2, ConditionPriority = 6 }
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 2, 3 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithSimpleScheduling_MoveAndRescheduleCallToSpecificITS_CallIsDeliveredOnlyForSpecificITSOrderTakeIntoAccountRoundRobin()
        {
            var scheduleScriptId = new TestScript(new SubRule(new[]{
                        new Action(Action.Operation.SetNewITS, "1"),
                        new Action(Action.Operation.SetNewCallPriority, "1")
                    })
            {
                ItsId = (int)CallOutcome.Busy
            },
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 4, SchedulingMode.Simple);

            CallTools.MoveAndRescheduleCalls(survey.SID, new[] { 2, 4 }, (int)CallOutcome.Busy);

            var conditions =
                new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 6}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 2, 4 }, deliveredInterviewIds, String.Format("delivered ids are : {0}", String.Join(",", deliveredInterviewIds)));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithSimpleScheduling_MoveAndRescheduleCallToSpecificITS_CallIsDeliveredOnlyForFreshSampleOrderTakeIntoAccountRoundRobin()
        {
            var scheduleScriptId = new TestScript(new[]{
                        new Action(Action.Operation.SetNewITS, "1"),
                        new Action(Action.Operation.SetNewCallPriority, "1")
                    },
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 4, SchedulingMode.Simple);

            CallTools.MoveAndRescheduleCalls(survey.SID, new[] { 1, 4 }, 5);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity { ConditionValue = 16, ConditionPriority = 5 },
                    new BvCallGroupConditionEntity { ConditionValue = 2, ConditionPriority = 6 }
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 2, 3 }, deliveredInterviewIds, String.Format("delivered ids are : {0}", String.Join(",", deliveredInterviewIds)));
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithoutCalls_ActivateCalls_CallIsDeliveredOnlyForFreshSampleOrderTakeIntoAccountRoundRobin()
        {
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewITS, "4", "Scheduling.Interview.ID > 6 && Scheduling.Interview.ID  <= 8"),
                    new Action(Action.Operation.SetNewITS, "5", "Scheduling.Interview.ID > 8 && Scheduling.Interview.ID  <= 10")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 4, SchedulingMode.Full);

            new TestCallManagementOperationFactory().CreateActivateCallsSelected(survey.SID, new[] { 1, 2, 3, 4, 5, 6 }, 1, 0, (int)CallShiftType.AnyValid, CallStates.All, false);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity { ConditionValue = 1, ConditionPriority = 5 },
                    new BvCallGroupConditionEntity { ConditionValue = 2, ConditionPriority = 5 }
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 1, 3, 2, 4 }, deliveredInterviewIds, String.Format("delivered ids are : {0}", String.Join(",", deliveredInterviewIds)));
        }

        [TestMethod, Owner(@"FIRM\MaximL"), Bug(65374)]
        public void AddSampleWithFullScheduling_CallGroupHasDifferentPriorityForITS_CallIsNotDeliveredForSurveyWithoutAssignment()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewCallPriority, "1" ),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID % 3 == 1"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID % 3 == 2"),
                    new Action(Action.Operation.SetNewITS, "3", "Scheduling.Interview.ID % 3 == 0")
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 6},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 5}
                };

            var personId = CreatePerson(survey.SID, conditions);

            var interviews = BvInterviewAdapter.GetAll();

            var test = new TestCati2(false, BackendToolsObject);

            test.InitializeWithExistsSurveyAndPerson(survey, personId, interviews);

            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, false);

            var interview = test.StartInterview_ManualOrPreview(survey.Name, 0);

            Assert.AreEqual(1, interview.ID);

            BackendTools.DeassignCatiPersonFromSurvey(survey.SID, personId);

            interview = test.CompleteInterviewAndWaitNext_Manual(interview);

            Assert.IsNull(interview);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void AddSampleWithFullScheduling_CallGroupInterviewsHasDifferentTimeInShift_CallIsDeliveredInPriorityOrderTakeIntoAccountRoundRobinAndTimeInShift()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewCallPriority, "1" ),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.RecallAfterANumberOfMinutes, "60", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
           new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = this.CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var conditions = new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 6},
                };

            var personId = CreatePerson(survey.SID, conditions);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 5, 6, 3, 4 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void PersonWithoutAssignmentOnCallGroup_InterviewsHasDifferentTimeInShift_CallIsDeliveredTakeIntoAccountTimeInShiftAndPriority()
        {
            //create scheduling script
            var scheduleScriptId = new TestScript(new[]
            {
                new SubRule(new []
                {
                    new Action(Action.Operation.SetNewCallPriority, "1" ),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.RecallAfterANumberOfMinutes, "60", "Scheduling.Interview.ID > 0 && Scheduling.Interview.ID  <= 2"),
                    new Action(Action.Operation.SetNewITS, "1", "Scheduling.Interview.ID > 2 && Scheduling.Interview.ID  <= 4"),
                    new Action(Action.Operation.SetNewITS, "2", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                    new Action(Action.Operation.SetNewCallPriority, "2", "Scheduling.Interview.ID > 4 && Scheduling.Interview.ID  <= 6"),
                })
                {
                    ItsId = (int)CallOutcome.FreshSample
                }
            },
            new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
            new Shift(2, 1, "1.00:00:00", "0.00:00:00")).Create("TestScript");

            var survey = this.CreateSurvey(scheduleScriptId, 10, SchedulingMode.Full);

            var personId = CreatePerson(survey.SID, null);

            var deliveredInterviewIds = GetDeliveredInterviewIds(personId, survey);

            CollectionAssert.AreEqual(new[] { 5, 6, 3, 4, 7, 8, 9, 10 }, deliveredInterviewIds);
        }

        [TestMethod, Owner(@"FIRM\MaximL")]
        public void TwoSurvey_DeliveryCall_RoundRobinIsSpecificPerSurvey()
        {
            BackendToolsObject.LaunchAllHoursScript();

            BackendTools.ResetInterviewId();
            var survey1 = CreateSurveyWithInterview(BackendTools.GetAllHoursID(), 8, "p000000010");
            BackendTools.ResetInterviewId();
            var survey2 = CreateSurveyWithInterview(BackendTools.GetAllHoursID(), 8, "p000000020");

            CallTools.MoveCalls(survey1.SID, new[] { 1, 2 }, 4);
            CallTools.MoveCalls(survey1.SID, new[] { 3, 4 }, 3);
            CallTools.MoveCalls(survey1.SID, new[] { 5, 6 }, 2);
            CallTools.MoveCalls(survey1.SID, new[] { 7, 8 }, 1);
            CallTools.MoveCalls(survey2.SID, new[] { 1, 2 }, 4);
            CallTools.MoveCalls(survey2.SID, new[] { 3, 4 }, 3);
            CallTools.MoveCalls(survey2.SID, new[] { 5, 6 }, 2);
            CallTools.MoveCalls(survey2.SID, new[] { 7, 8 }, 1);

            var conditions =
                new[]
                {
                    new BvCallGroupConditionEntity{ConditionValue = 1, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 2, ConditionPriority = 5},
                    new BvCallGroupConditionEntity{ConditionValue = 4, ConditionPriority = 5}
                };

            var @group = CreateCallGroup(conditions);

            var personId1 = CreatePerson(survey1.SID, @group, "user1");
            var personId2 = CreatePerson(survey2.SID, @group, "user2");

            var interviews = BvInterviewAdapter.GetAll();

            var test1 = new TestCati2(false, BackendToolsObject);
            var test2 = new TestCati2(false, BackendToolsObject);

            test1.InitializeWithExistsSurveyAndPerson(survey1, personId1, interviews);
            test2.InitializeWithExistsSurveyAndPerson(survey2, personId2, interviews);

            test1.Login("user1", Password, AgentTaskChoiceMode.CampaignAssignment, false);
            test2.Login("user2", Password, AgentTaskChoiceMode.CampaignAssignment, false);

            var interview1 = test1.StartInterview_ManualOrPreview(survey1.Name, 0);
            var interview2 = test2.StartInterview_ManualOrPreview(survey2.Name, 0);

            Assert.AreEqual(7, interview1.ID);
            Assert.AreEqual(7, interview2.ID);

            interview1 = test1.CompleteInterviewAndWaitNext_Manual(interview1);
            interview2 = test2.CompleteInterviewAndWaitNext_Manual(interview2);

            Assert.AreEqual(8, interview1.ID);
            Assert.AreEqual(8, interview2.ID);

            interview1 = test1.CompleteInterviewAndWaitNext_Manual(interview1);
            interview2 = test2.CompleteInterviewAndWaitNext_Manual(interview2);

            Assert.AreEqual(5, interview1.ID);
            Assert.AreEqual(5, interview2.ID);

            interview1 = test1.CompleteInterviewAndWaitNext_Manual(interview1);
            interview2 = test2.CompleteInterviewAndWaitNext_Manual(interview2);

            Assert.AreEqual(1, interview1.ID);
            Assert.AreEqual(1, interview2.ID);

            interview1 = test1.CompleteInterviewAndWaitNext_Manual(interview1);
            interview2 = test2.CompleteInterviewAndWaitNext_Manual(interview2);

            Assert.AreEqual(6, interview1.ID);
            Assert.AreEqual(6, interview2.ID);

            interview1 = test1.CompleteInterviewAndWaitNext_Manual(interview1);
            interview2 = test2.CompleteInterviewAndWaitNext_Manual(interview2);

            Assert.AreEqual(2, interview1.ID);
            Assert.AreEqual(2, interview2.ID);

            interview1 = test1.CompleteInterviewAndWaitNext_Manual(interview1);
            interview2 = test2.CompleteInterviewAndWaitNext_Manual(interview2);

            Assert.IsNull(interview1);
            Assert.IsNull(interview2);
        }

        private BvSurveyEntity CreateSurvey(int scheduleScriptId, int sampleSize, SchedulingMode sampleMode)
        {
            // create and setup survey
            var surveyId = BackendToolsObject.CreateSurvey("p000000010");
            var survey = SurveyRepository.GetById(surveyId);
            survey.SurveySchedulingMode = (short)SurveySchedulingMode.CallGroup;
            survey.ScheduleID = scheduleScriptId;
            SurveyRepository.Update(survey);

            BackendToolsObject.AddSample(survey.Name, 1, (int)sampleMode, 1, sampleSize, null);

            _surveyStateService.Open(surveyId);
            return survey;
        }

        private BvSurveyEntity CreateSurveyWithInterview(int scheduleScriptId, int sampleSize, string name)
        {
            // create and setup survey
            var surveyId = BackendToolsObject.CreateSurvey(name);
            var survey = SurveyRepository.GetById(surveyId);
            survey.SurveySchedulingMode = (short)SurveySchedulingMode.CallGroup;
            survey.ScheduleID = scheduleScriptId;
            SurveyRepository.Update(survey);

            List<BvInterviewEntity> interviews;
            List<BvCallEntity> calls;
            BackendTools.CreateInterviewsWithCalls(surveyId, sampleSize, out interviews, out calls);

            _surveyStateService.Open(surveyId);
            return survey;
        }

        private int CreatePerson(int surveyId, IEnumerable<BvCallGroupConditionEntity> conditions)
        {
            var @group = CreateCallGroup(conditions);

            return CreatePerson(surveyId, @group, null);
        }

        private int CreatePerson(int surveyId, BvCallGroupEntity @group, string user)
        {
            var personId = PersonTools.CreatePerson(user ?? User, Password, AgentTaskChoiceMode.CampaignAssignment);
            BackendTools.AssignCatiPersonToSurvey(surveyId, personId);

            if (@group != null)
            {
                ServiceLocator.Resolve<ICallGroupService>().SetPersonsAssignment(new List<int> { personId }, @group.Id);
            }

            return personId;
        }

        private static BvCallGroupEntity CreateCallGroup(IEnumerable<BvCallGroupConditionEntity> conditions)
        {
            if (conditions == null)
                return null;

            var group = new BvCallGroupEntity { Name = "Group" };

            var service = ServiceLocator.Resolve<ICallGroupService>();
            var repository = ServiceLocator.Resolve<ICallGroupRepository>();

            repository.Insert(@group);
            service.SetListOfCondition(@group.Id, conditions);
            return @group;
        }

        private List<int> GetDeliveredInterviewIds(int personId, BvSurveyEntity survey)
        {
            var interviews = BvInterviewAdapter.GetAll();
            var test = new TestCati2(false, BackendToolsObject);

            test.InitializeWithExistsSurveyAndPerson(survey, personId, interviews);

            var deliveredInterviewIds = new List<int>();

            test.Login(User, Password, AgentTaskChoiceMode.CampaignAssignment, false);

            var interview = test.StartInterview_ManualOrPreview(survey.Name, 0);

            while (interview != null)
            {
                deliveredInterviewIds.Add(interview.ID);
                interview = test.CompleteInterviewAndWaitNext_Manual(interview);
            }
            return deliveredInterviewIds;
        }
    }
}
