using System;
using System.Data;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.CATI.Supervisor.Core.Surveys;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptCustomScriptActionsTest : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Execute_CallExistsAndInterviewChanged_AtributesAreRestored(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new Action(Action.Operation.RestorePreviousCallState),
                new Shift(1, 1, "0.00:00:00", "6.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = (int)CallOutcome.FreshSample;

            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            BackendTools.CreateCall(call);

            interview.TransientState = (int)CallOutcome.ReturnedNotDialled;

            ServiceLocator.Resolve<IInterviewRepository>().Update(interview, new SchedulingScriptExecutionOptions());

            interview.TransientState = (int)CallOutcome.FreshSample;

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void Execute_CallNotExistsAndInterviewChanged_AtributesAreRestored(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new SubRule(new[]
                {
                    new Action(Action.Operation.SetNewCallPriority, "10"),
                    new Action(Action.Operation.RestorePreviousCallState)
                }),
                new Shift(1, 1, "0.00:00:00", "6.00:00:00"));

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = (int)CallOutcome.FreshSample;

            BackendTools.CreateInterview(interview);

            interview.TransientState = (int)CallOutcome.ReturnedNotDialled;

            ServiceLocator.Resolve<IInterviewRepository>().Update(interview, new SchedulingScriptExecutionOptions());

            interview.TransientState = (int)CallOutcome.FreshSample;

            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(interview.SurveySID, interview.ID));
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void ExecuteThroughCustomScript_CallNotExistsAndInterviewChanged_AtributesAreRestored(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new SubRule(new[]
                {
                    new Action(Action.Operation.SetNewCallPriority, "10"),
                    new Action(Action.Operation.RunCustomScript, "func")
                }),
                new Shift(1, 1, "0.00:00:00", "6.00:00:00"))
            {
                CustomScript = @"function func(){ ExecuteAction(Actions.RestorePreviousCallState); }"
            };

            int surveySID = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySID);
            interview.TransientState = (int)CallOutcome.FreshSample;

            BackendTools.CreateInterview(interview);

            interview.TransientState = (int)CallOutcome.ReturnedNotDialled;

            ServiceLocator.Resolve<IInterviewRepository>().Update(interview, new SchedulingScriptExecutionOptions());

            interview.TransientState = (int)CallOutcome.FreshSample;

            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(interview.SurveySID, interview.ID));
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptAssignMultipleGroups_AllGroupsAssigned(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "PersonGroup1" },
                    new PersonGroupData { Tag = "PG2", Name = "PersonGroup2" },
                    new PersonGroupData { Tag = "PG3", Name = "PersonGroup3" }
                },
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, 
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1"}}
                    }
                }
            }.Create();

            var g1 = context.GetPersonGroup("PG1").Id;
            var g2 = context.GetPersonGroup("PG2").Id;
            var g3 = context.GetPersonGroup("PG3").Id;

            var script = new TestScript(
            new[]{
                    new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                    new Action(Action.Operation.RunCustomScript, "CustomFunc1")

                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
                {
                    CustomScript = @" 
                                function CustomFunc () {
                                    var groups : String = '" + g1 + "," + g2 + @"' 
                                    ExecuteAction(Actions.AssignMultipleGroups,groups);
                                }

                                function CustomFunc1 () {
                                    var groups : String = '" + g3 + @"' 
                                    ExecuteAction(Actions.AssignMultipleGroups,groups);
                                }"
                };

            BackendToolsObject.LaunchScript(context.GetSurvey("S1").Id, script);

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var assignmentService = ServiceLocator.Resolve<IAssignmentService>();

            context.GetCall("S1.I1").Assert.IsTrue(x => x.Resource == assignmentService.GetAssignmentResourceId(new int[] { g1, g2, g3 }));
            var sql = @"SELECT COUNT(*) FROM [BvAssignmentResource]";
            Assert.AreEqual(2, new DatabaseEngine().ExecuteScalar<int>(sql, CommandType.Text));
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRemoveTwoGroups_GroupsRemoved(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            var context = new TestData()
            {
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "PersonGroup1" },
                    new PersonGroupData { Tag = "PG2", Name = "PersonGroup2" },
                    new PersonGroupData { Tag = "PG3", Name = "PersonGroup3" }
                },
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, 
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Call = new CallData(){Resource = "PG1,PG2,PG3"}}}
                    }
                }
            }.Create();

            var g1 = context.GetPersonGroup("PG1").Id.ToString();
            var g2 = context.GetPersonGroup("PG2").Id.ToString();

            var script = new TestScript(
                new[]
                {
                    new Action(Action.Operation.RunCustomScript, "CustomFunc")
                },
                new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
                {
                CustomScript = @" 
                                function CustomFunc() {
                                    var groups : String = '" + g1 + "," + g2 + @"' 
                                    ExecuteAction(Actions.DeassignMultipleGroups, groups);

                                };"
                };

            BackendToolsObject.LaunchScript(context.GetSurvey("S1").Id, script);

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x.Resource == context.GetPersonGroup("PG3").Id);
        }

        [Theory, Owner(@"FIRM\KirillV")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRecallAfterANumberOfMinutes_SummerTime_FirstShiftSelectedAndNewCallDateIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var testDate = new DateTime(2017, 8, 9, 9, 30, 0);
            new DateTimeMocker(testDate);

            var minutes = 10;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData{ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", TimeZoneId = "1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                            new Shift(1, 1, "3.10:00:00", "3.10:59:00")
                            )
                        {
                            CustomScript = @"function CustomFunc () { ExecuteAction(Actions.RecallAfterNumberOfMinutes,'" + minutes + "');}"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");
            var newCallDate = testDate.AddMinutes(minutes);
            Assert.AreEqual((int)CallShiftType.None, call.Model.ShiftID);
            Assert.AreNotEqual(null , call.Model.TimeInShift);
            Assert.AreEqual(newCallDate, call.Model.TimeInShift);
        }

        [Theory, Owner(@"FIRM\KirillV")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRecallAfterANumberOfMinutes_SummerTime_SecondShiftSelectedAndNewCallDateIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var testDate = new DateTime(2017, 8, 9, 9, 30, 0);
            new DateTimeMocker(testDate);

            var minutes = 60;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData{ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", TimeZoneId = "1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                            new Shift(1, 1, "3.10:00:00", "3.10:59:00"),
                            new Shift(2, 2, "3.11:00:00", "3.11:59:00")
                            )
                        {
                            CustomScript = @"function CustomFunc () { ExecuteAction(Actions.RecallAfterNumberOfMinutes,'" + minutes + "');}"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");
            var newCallDate = testDate.AddMinutes(minutes);
            Assert.AreEqual((int)CallShiftType.None, call.Model.ShiftID);
            Assert.AreNotEqual(null, call.Model.TimeInShift);
            Assert.AreEqual(newCallDate, call.Model.TimeInShift);
        }

        [Theory, Owner(@"FIRM\KirillV")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRecallAfterANumberOfMinutes_WinterTime_SecondShiftSelectedAndNewCallDateIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var testDate = new DateTime(2017, 1, 1, 10, 30, 0);
            new DateTimeMocker(testDate);

            var minutes = 60;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData{ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", TimeZoneId = "1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                            new Shift(1, 1, "0.10:00:00", "0.10:59:00"),
                            new Shift(2, 2, "0.11:00:00", "0.11:59:00")
                            )
                        {
                            CustomScript = @"function CustomFunc () { ExecuteAction(Actions.RecallAfterNumberOfMinutes,'" + minutes + "');}"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");
            var newCallDate = testDate.AddMinutes(minutes);
            Assert.AreEqual((int)CallShiftType.None, call.Model.ShiftID);
            Assert.AreNotEqual(null, call.Model.TimeInShift);
            Assert.AreEqual(newCallDate, call.Model.TimeInShift);
        }
        
        [Theory, Owner(@"FIRM\GrigoryK")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRecallAfterANumberOfMinutes_SpecificShiftTypeId_SecondShiftSelectedAndNewCallDateIsCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var testDate = new DateTime(2017, 1, 1, 10, 30, 0);
            new DateTimeMocker(testDate);

            var minutes = 60;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData{ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", TimeZoneId = "1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                            new Shift(1, 1, "0.10:00:00", "0.10:59:00"),
                            new Shift(2, 2, "0.11:00:00", "0.11:59:00")
                            )
                        {
                            CustomScript = @"function CustomFunc () { ExecuteAction(Actions.RecallAfterNumberOfMinutes,'" + minutes + "');}"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            int dbShiftTypeID1 = SurveyManager.GetShiftTypes(context.GetSurvey("S1").Id).Find(x => x.Id == 1).ObjectId;
            int dbShiftTypeID2 = SurveyManager.GetShiftTypes(context.GetSurvey("S1").Id).Find(x => x.Id == 2).ObjectId;
            
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");
            var newCallDate = testDate.AddMinutes(minutes);
            Assert.AreEqual((int)CallShiftType.None, call.Model.ShiftID);
            Assert.AreNotEqual(null, call.Model.TimeInShift);
            Assert.AreEqual(newCallDate, call.Model.TimeInShift);
           
            var newCall = call.Model.Copy();
            newCall.ShiftID = dbShiftTypeID1;
            CallQueueService.UpdateCall(newCall, 0);
            
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
           
            call = context.GetCall("S1.I1");
            Assert.AreEqual(dbShiftTypeID2, call.Model.ShiftID);
            
            newCall = call.Model.Copy();
            newCall.ShiftID = (int)CallShiftType.AnyValid;
            CallQueueService.UpdateCall(newCall, 0);
            
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
           
            call = context.GetCall("S1.I1");
            Assert.AreEqual((int)CallShiftType.AnyValid, call.Model.ShiftID);
        }
        
        [Theory, Owner(@"FIRM\KirillV")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRecallAfterNumberOfShifts_SecondShiftSelected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var testDate = new DateTime(2017, 1, 1, 10, 30, 0);
            new DateTimeMocker(testDate);

            var numberOfShifts = 1;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData{ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", TimeZoneId = "1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                            new Shift(1, 1, "0.10:00:00", "0.10:59:00"),
                            new Shift(2, 2, "0.11:00:00", "0.11:59:00")
                            )
                        {
                            CustomScript = @"function CustomFunc () { ExecuteAction(Actions.RecallAfterNumberOfShifts,'" + numberOfShifts + "');}"
                        }
                    }
                }
            }.Create();

            int dbShiftTypeID = SurveyManager.GetShiftTypes(context.GetSurvey("S1").Id).Find(x => x.Id == 2).ObjectId;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");
            Assert.AreEqual(dbShiftTypeID, call.Model.ShiftID, "Wrong shift was selected");
        }

        [Theory, Owner(@"FIRM\KirillV")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptRecallAfterNumberOfShifts_FirstShiftSelected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var testDate = new DateTime(2017, 1, 1, 11, 30, 0);
            new DateTimeMocker(testDate);

            var numberOfShifts = 2;

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData{ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData {Tag = "S1.I1", TimeZoneId = "1"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                            new Shift(1, 1, "0.10:00:00", "0.10:59:00"),
                            new Shift(2, 2, "0.11:00:00", "0.11:59:00"),
                            new Shift(3, 3, "0.12:00:00", "0.12:59:00")
                            )
                        {
                            CustomScript = @"function CustomFunc () { ExecuteAction(Actions.RecallAfterNumberOfShifts,'" + numberOfShifts + "');}"
                        }
                    }
                }
            }.Create();

            int dbShiftTypeID = SurveyManager.GetShiftTypes(context.GetSurvey("S1").Id).Find(x => x.Id == 1).ObjectId;

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");
            Assert.AreEqual(dbShiftTypeID, call.Model.ShiftID, "Wrong shift was selected");
        }
    }
}