using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.RoutineMaintenance.Framework.Interfaces.Enums;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Fakes;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data.Fakes;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.RoutineMaintenance.Actions;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.Test.Common.Attributes;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Confirmit.CATI.IntegrationTests.XUnit.Tests.Scheduling
{
    [Collection(TestConstants.CollectionName)]
    [Trait(TestConstants.TraitName, TestConstants.Trait1)]
    public class ScriptCustomScriptTest : BaseMockedIntegrationTest
    {
        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CreateCallWithIncrementPriority_CallCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.IncrementPriority, "2"),
                    new Action(Action.Operation.RunCustomScript, "IncrementPriority"),
                    new Action(Action.Operation.IncrementPriority, "2")
                },
                    new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
function IncrementPriority()
{
    CallShouldBeCreated();
    Scheduling.NewCall.Priority += 2;
}"
            };

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Priority = 10;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            call.Priority += 6;

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CreateCallWithIncrementPriorityThroughUsingParametrisedFunction_CallCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.IncrementPriority, "2"),
                    new Action(Action.Operation.RunCustomScript, "IncrementPriority(4)"),
                    new Action(Action.Operation.IncrementPriority, "2")
                },
                    new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
function IncrementPriority( count: int )
{
    CallShouldBeCreated();
    Scheduling.NewCall.Priority += count;
}"
            };

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Priority = 10;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            call.Priority += 8;

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CreateCallWithDoubleIncrementPriority_CallCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.IncrementPriority, "2"),
                    new Action(Action.Operation.RunCustomScript, "IncrementPriority"),
                    new Action(Action.Operation.RunCustomScript, "IncrementPriority"),
                    new Action(Action.Operation.IncrementPriority, "2")
                },
                    new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
function IncrementPriority()
{
    CallShouldBeCreated();
    Scheduling.NewCall.Priority += 2;
}"
            };

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Priority = 10;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            call.Priority += 8;

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL"), Cr(42612)]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_TwoRunCostomScriptActionsOnSameFunction_CallCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.RunCustomScript, "IncrementPriority"),
                    new Action(Action.Operation.RunCustomScript, "IncrementPriority")
                },
                    new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
function IncrementPriority()
{
    CallShouldBeCreated();
    Scheduling.NewCall.Priority += 2;
}"
            };

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);
            call.Priority = 10;
            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            call.Priority += 4;

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_UpdateITS_CallNotCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.RunCustomScript, "SetITS")
                },
                    new Shift(1, 1, "0.00:00:00", "0.00:00:00"))
            {
                CustomScript = @"
function SetITS()
{
    Scheduling.Interview.TransientState = 10;
}"
            };

            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            interview.TransientState = 10;

            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(call.SurveySID, call.InterviewID));
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_SetTimeToCall_CallCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.RunCustomScript, "SetTimeToCall")
                },
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(1, 1, "1.00:00:00", "0.00:00:00"));

            var time = DateTime.Parse("2010-07-23 12:00:00");

            script.CustomScript =
                @"
function SetTimeToCall()
{
    CallShouldBeCreated();

    var shift = Scheduling.Shifts.GetMatchingShift( DateTime.Parse(" + "\"" + time + "\"" + @"), 1 );
                
    Scheduling.NewCall.ShiftID = shift.ShiftTypeID;
    Scheduling.NewCall.TimeInShift = shift.StartDate;
}";
            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            call.ShiftID = script.GetShiftTypeWorkID(1);
            call.TimeInShift = DateTime.Parse("2010-07-18 23:00:00");

            BackendTools.CheckInterview(interview);
            BackendTools.CheckCall(call);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CreateAppointment_AppointmentCreatedCorrect(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var script = new TestScript(
                new[]{
                    new Action(Action.Operation.RunCustomScript, "AddAppointment")
                },
                    new Shift(1, 1, "0.00:00:00", "1.00:00:00"),
                    new Shift(1, 1, "1.00:00:00", "0.00:00:00"));

            var time = DateTime.Parse("2010-07-23 12:00:00");

            script.CustomScript =
                @"
function AddAppointment()
{
    var appt : Confirmit.CATI.Common.ConsoleService.Abstract.Appointment = new Confirmit.CATI.Common.ConsoleService.Abstract.Appointment();
    appt.time = DateTime.Parse(" + "\"" + time + "\"" + @");
    appt.expirationTime = DateTime.Parse(" + "\"" + time + "\"" + @").AddDays(1);
    appt.contactName = Scheduling.Interview.ID.ToString();

    var appts : Confirmit.CATI.Common.ConsoleService.Abstract.Appointment[] = new Confirmit.CATI.Common.ConsoleService.Abstract.Appointment[1];
    appts[0] = appt;

    (new Confirmit.CATI.Core.Services.InterviewService).AddAppointments(Scheduling.Interview.SurveySID, Scheduling.Interview.ID, 0, appts);
}";
            int surveySid = BackendToolsObject.CreateSurvey(script);

            var interview = BackendTools.NewInterview(surveySid);
            BackendTools.CreateInterview(interview);

            var call = BackendTools.NewCall(interview);

            BackendTools.CreateCall(call);

            BackendTools.FireEvent(interview);

            call.ShiftID = script.GetShiftTypeWorkID(1);
            call.TimeInShift = DateTime.Parse("2010-07-18 23:00:00");

            BackendTools.CheckInterview(interview);
            Assert.IsFalse(BackendTools.IsCallExists(call.SurveySID, call.InterviewID));
            Assert.AreEqual(BvAppointmentAdapter.GetAll().Count, 1);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromActionFilter_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc()"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" function FilterFunc() : Boolean {return true; }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptWithZeroDialingAttempts_NoErrorsExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1",ITS = CallOutcome.FreshSample}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() {
                        Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new[] {
                                new Action(Action.Operation.RunCustomScript, "CustomFunction"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00")) {
                            CustomScript = @" 
                                function CustomFunction()		
                                {
                                   LogMessage('Custom Log Message');
                                }						
                            "
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");
            var survey = context.GetSurvey("S1");
            
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions {
                    ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false,
                    DialingAttempts = Array.Empty<CatiDialingAttempt>()
                });
            
            var logRepository = ServiceLocator.Resolve<ISchedulingScriptLogRepository>();
          

            var logs = logRepository.GetByInterviewId(survey.Id, interview.Id);
            Assert.AreEqual(1, logs.Count);
            var message = logs[0].LogMessages;
            Assert.IsTrue(message.Contains("Custom Log Message"));
        }

        [Theory, Owner(@"FIRM\AlexanderM")]
        [ClassData(typeof(TestDataGenerator))]
        public void CallHistoryContainsOriginalTelephoneNumberIfCustomScriptChangesIt(SecurityMode mode)
        {
            SetSecurityMode(mode);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData
                    {
                        Tag = "S1", IsUseDb = false,
                        SchedulingScript = "ChangeNumberScript",
                        Interviews = new[]
                        {
                            new InterviewData
                            {
                                Tag = "S1.I1", ITS = CallOutcome.FreshSample, TelephoneNumber = "999",
                            },
                        },
                    }
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag = "ChangeNumberScript",
                        Script = new TestScript(CallOutcome.Appointment, Action.Operation.RunCustomScript,
                            "ChangeNumber")
                        {
                            CustomScript = @"function ChangeNumber() { Scheduling.Interview.TelephoneNumber = '111'; }"
                        }
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions
                {
                    ITS = 1,
                    ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = true,
                    DialingAttempts = Array.Empty<CatiDialingAttempt>()
                });

            var historyEntities = BvHistoryAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", survey.Id),
                new SqlParameter("@InterviewId", context.GetInterview("S1.I1").Id));

            var history = historyEntities.Single();

            Assert.AreEqual("999", history.TelephoneNumber);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromSubRuleFilter_FilterIsFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new Action(Action.Operation.IncrementPriority, "10")
                            {
                                Filter = "CustomScript.FilterFunc()",
                                FilterEnabled = true
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" function FilterFunc() : Boolean {return false; }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsNull();
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromSubRuleFilter_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new Action(Action.Operation.IncrementPriority, "10")
                            {
                                Filter = "CustomScript.FilterFunc()",
                                FilterEnabled = true
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" function FilterFunc() : Boolean {return true; }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromActionFilter_FilterIsFalse(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = false, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc()"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" function FilterFunc() : Boolean {return false; }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsNull();
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromActionFilter_FrFunctionInsideCustomCode_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}}
                        },

                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc() == 2"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" function FilterFunc() : int {return fr('q1').get(); }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromActionFilter_FilterAccessGlobalVar_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}}
                        },

                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CustomFunction(2)"),
                                new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc() == 2")
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                var Var1 : int;
                                function FilterFunc() : int {
                                    return Var1; 
                                }
                                function CustomFunction( i : int) {
                                    Var1 = i;
                                }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_FrFunctionResultAsParameterToCustomFunctionToBeSavedInGloballVar_GlobalVarValueIsReturnedInNextActionFilter_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}}
                        },

                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CustomFunc(fr('q1').get())"),
                                new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc() == 2")
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                var Var1 : int;
                                function FilterFunc() : int {
                                    return Var1;
                                }

                                function CustomFunc( i : int) {
                                    Var1 = i;
                                }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CustomFunctionUseFrFunctionToSetGlobalVar_GlobalVarValueIsReturnedInNextActionFilter_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}}
                        },

                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CustomFunc"),
                                new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc() == 2")
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                var Var1 : int;
                                function FilterFunc() : int {
                                    return Var1; 
                                }
                                function CustomFunc() {
                                    Var1 = fr('q1').get();
                                }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }


        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_RunCustomScriptFunctionFromActionFilter_FrFunctionOutsideCustomCode_FilterIsTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2"}}
                        },

                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2" }}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.IncrementPriority, "10", "CustomScript.FilterFunc(fr('q1').get())"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" function FilterFunc(value : int) : Boolean {return value==2 }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            context.GetCall("S1.I1").Assert.IsTrue(x => x != null);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void AssignFunctionCallResultToVariable_q1AssignFunc_ValueUpdated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new FormData(){Name="q1"}
                        },
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", Data="q1=2"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.AssignFunctionCallResultToVariable, "q1=func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){return f('q1').get() - 1;}"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var q1 = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int?>(
                survey.Id,
                @"SELECT q1 FROM <Schema>.response0 WHERE respid = @respId",
            new SqlParameter("@respId", interview.Id));

            Assert.AreEqual(1, q1);
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void DirectInsertToSurveyDatabaseEnabled_RunCustomScript_CorrectDataExpected(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new FormData[]
                        {
                            new FormData(){Name="q1"}
                        },
                        Interviews = new InterviewData[] {
                            new InterviewData(){Tag = "S1.I1"},
                            new InterviewData() {Tag = "S1.I2", Data="q1=1"}
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new Action(Action.Operation.RunCustomScript, "func"),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function func(){ f('q1').setValue('2');}"
                        }
                    }
                }
            }.Create();

            ServiceLocator.Register<ISurveyDataRowsWebServiceUpdater, StubISurveyDataRowsWebServiceUpdater>();
            ServiceLocator.Resolve<ISchedulingScriptSettings>().UseDirectDbAccess = true;
            ServiceLocator.Resolve<IToggleSettings>().DirectlyInsertResponses = true;
            
            var survey = context.GetSurvey("S1");
            var interview1 = context.GetInterview("S1.I1");
            var interview2 = context.GetInterview("S1.I2");

            InterviewRepository.Update(interview1.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
            InterviewRepository.Update(interview2.Model, new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var responseId1 = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(
                survey.Id,
                @"SELECT responseId FROM <Schema>.response_control WHERE respid = @respId",
                new SqlParameter("@respId", interview1.Id));
            
            var responseId2 = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(
                survey.Id,
                @"SELECT responseId FROM <Schema>.response_control WHERE respid = @respId",
                new SqlParameter("@respId", interview2.Id));
            
            var i1_q1 = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(
                survey.Id,
                @"SELECT q1 FROM <Schema>.response0 WHERE respid = @respId AND responseId=@responseId",
                new SqlParameter("@respId", interview1.Id), new SqlParameter("@responseId", responseId1));
            
            var i2_q1 = ServiceLocator.Resolve<ISurveyDatabaseEngine>().ExecuteScalar<int>(
                survey.Id,
                @"SELECT q1 FROM <Schema>.response0 WHERE respid = @respId AND responseId=@responseId",
                new SqlParameter("@respId", interview2.Id), new SqlParameter("@responseId", responseId2));

            Assert.AreEqual(2, i1_q1);
            Assert.AreEqual(2, i2_q1);
        }


        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_FunctionResultAsParameterToCustomFunctionToBeSavedInGloballVar_GlobalVarValueIsReturnedInNextActionFilter_CorrectValuesReturnedForEachtIntviewer_FiltersAreTrue(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },

                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1" },
                            new InterviewData() {Tag = "S1.I2", Data="q1=2" },
                            new InterviewData() {Tag = "S1.I3", Data="q1=3" },
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CustomFunc(fr('q1').get())"),
                                new Action(Action.Operation.AssignResource, "14", "CustomScript.FilterFunc() != 9999")
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                var Var1 : int;
                                function FilterFunc() : int {
                                    return Var1;
                                }

                                function CustomFunc( i : int) {
                                    CallShouldBeCreated();
                                    if (Var1 == 0)
                                    {
                                        Scheduling.NewCall.Priority = i;
                                        Var1 = i;
                                    }
                                    else
                                    {   
                                        Scheduling.NewCall.Priority = 9999;
                                        Var1 = 9999;
                                    }
                                }"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I1").Assert.IsTrue(x => x.Priority == 1);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Resource == 14);

            interview = context.GetInterview("S1.I2");
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I2").Assert.IsTrue(x => x.Priority == 2);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Resource == 14);

            interview = context.GetInterview("S1.I3");
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            context.GetCall("S1.I3").Assert.IsTrue(x => x.Priority == 3);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.Resource == 14);

        }


        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CustomFunction_CreateAppointment_AppointmentIsCreated(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimezoneManager.AddTimezone(16);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },

                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1", TimeZoneId = "16"},
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CreateAppointment"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                function CreateAppointment()		
                                {		
                                  CallShouldBeCreated();				
                                  var time : DateTime = DateTime.Parse('2017-05-15 13:00');						
                                  var tzId : int = Scheduling.Interview.TimezoneID;						
						
                                  var tzInfo = TimezoneService.GetTimezoneInfo(tzId);						
                                  time = TimeZoneInfo.ConvertTimeToUtc(time, tzInfo);						
   						
                                  var appt = new BvAppointmentEntity()						
						
                                  appt.ID = 0;						
                                  appt.SurveySID = Scheduling.Interview.SurveySID;						
                                  appt.InterviewSID = Scheduling.Interview.ID;						
                                  appt.Time = time;						
                                  appt.ExpTime = null;						
                                  appt.State = 0;						
                                  appt.TZID = tzId;						
						
                                  var ApptId = AppointmentRepository.InsertUpdate(appt);		
                                  Scheduling.NewCall.ShiftID = CallShiftType.None;
                                  Scheduling.NewCall.TimeToExpire = null;
                                  Scheduling.NewCall.TimeInShift = time;
                                  Scheduling.NewCall.ApptID = ApptId;				
                                }						
                            "
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");

            call.Assert.IsTrue(x => x != null);
            call.Assert.IsTrue(x => x.TimeInShift == DateTime.Parse("2017-05-15 10:00"));
            call.Assert.IsTrue(x => x.ApptID > 0);
            Assert.AreEqual(1, AppointmentRepository.GetAppointmentForInterview(context.GetSurvey("S1").Id, interview.Id, AppointmentState.ActiveWithCall).State);
        }

        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CustomFunction_GetInterviewerAndCreateNewAppointment_CorrectInterviewerDataAndInterviewerId(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimezoneManager.AddTimezone(16);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },

                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1", TimeZoneId = "16"},
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CreateAppointment"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                function AssertTrue(condition) 
                                {
                                    if (!condition)
                                        throw Error('Assert failed: ');
                                }

                                function CreateAppointment()		
                                {		
                                  CallShouldBeCreated();				
                                  var time : DateTime = DateTime.Parse('2017-05-15 13:00');						
                                  var tzId : int = Scheduling.Interview.TimezoneID;						
						
                                  var tzInfo = TimezoneService.GetTimezoneInfo(tzId);						
                                  time = TimeZoneInfo.ConvertTimeToUtc(time, tzInfo);						
   						
                                  var appt = new BvAppointmentEntity()						
						
                                  appt.ID = 0;						
                                  appt.SurveySID = Scheduling.Interview.SurveySID;						
                                  appt.InterviewSID = Scheduling.Interview.ID;						
                                  appt.Time = time;						
                                  appt.ExpTime = null;						
                                  appt.State = 0;						
                                  appt.TZID = tzId;						
						
                                  var ApptId = AppointmentRepository.InsertUpdate(appt);		
                                  Scheduling.NewCall.ShiftID = CallShiftType.None;
                                  Scheduling.NewCall.TimeToExpire = null;
                                  Scheduling.NewCall.TimeInShift = time;
                                  Scheduling.NewCall.ApptID = ApptId;

                                  var interviewer = GetInterviewerByName('TestPerson123');
                                  var interviewer2 = GetInterviewerById(interviewer.Id);

                                  AssertTrue(interviewer.Name == 'TestPerson123');
                                  AssertTrue(interviewer.Description == 'Interviewer for testing');
                                  AssertTrue(interviewer.Location == 'location for testing');

                                  AssertTrue(interviewer2.Name == 'TestPerson123');
                                  AssertTrue(interviewer2.Description == 'Interviewer for testing');
                                  AssertTrue(interviewer2.Location == 'location for testing');

                                  AssertTrue(interviewer.Id == interviewer2.Id);

                                  Scheduling.NewCall.Resource = interviewer2.Id;
                                }	
                            "
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", Name = "TestPerson123", Description = "Interviewer for testing", Location = "location for testing" } }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });
           
            interview = context.GetInterview("S1.I1");
            Assert.IsTrue(interview.Model.TransientState != (int)CallOutcome.Error);
            var call2 = BvSvyScheduleAdapter.GetAll().Single();
            Assert.AreEqual(context.GetPerson("P1").Id, call2.ExplicitSID);
        }

        [Theory, Owner(@"FIRM\LeonidS")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CustomFunction_RecallAfterSpecifiedNumberOfShifts_CallRescheduledCorrectly(SecurityMode mode)
        {
            SetSecurityMode(mode);

            var firstShiftDate = new DateTime(2017, 5, 15, 13, 0, 0);
            var secondShiftDate = new DateTime(2017, 5, 16, 13, 0, 0);

            TimezoneManager.AddTimezone(16);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },

                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1", TimeZoneId = "16"},
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "RecallAfterNextShifOfSpecifiedType"),
                            }),
                            new Shift(1, 1, "1.12:00:00", "1.22:00:00" ),
                            new Shift(2, 2, "2.12:00:00", "2.22:00:00" ),
                            new Shift(3, 1, "3.12:00:00", "3.22:00:00" ),
                            new Shift(4, 2, "4.12:00:00", "4.22:00:00" ),
                            new Shift(5, 1, "5.12:00:00", "5.22:00:00" )
                            )
                        {
                            CustomScript = @" 
                                function RecallAfterNextShifOfSpecifiedType()						
                                {						
                                    var timeZone : int;						
                                    var lastCallTime : DateTime;				
                                  
                                    timeZone = GetTimezoneId(); 						
                                    lastCallTime = Scheduling.Time;	
                                    //These two lines just for testing only
                                    lastCallTime = TimezoneService.ConvertTimeFromUtc(timeZone, lastCallTime);				
                                    lastCallTime = TimezoneService.ConvertTimeToUtc(timeZone, lastCallTime);				


                                    var shift = Scheduling.Shifts.GetMatchingShift(lastCallTime, timeZone); 						
                                    if (shift.ShiftTypeID == Scheduling.Shifts.GetShiftTypeWorkID(1))						
                                    {						
                                        RecallAfterNumberOfShiftOfSpecifiedType(1,2);						
                                    }						

                                    if (shift.ShiftTypeID == Scheduling.Shifts.GetShiftTypeWorkID(2))						
                                    {						
                                         RecallAfterNumberOfShiftOfSpecifiedType(1,1);						
                                    }         						
                                }						

                                function RecallAfterNumberOfShiftOfSpecifiedType( numberOfShifts : int, shiftTypeId : int )						
                                {						
                                    var timeZone : int;						
                                    var lastCallTime : DateTime;				
                                  
                                        timeZone = GetTimezoneId(); 						
                                        lastCallTime = Scheduling.Time;						

                                        var shift : ShiftService.MatchingShift = Scheduling.Shifts.GetNextShiftOfSpecifiedType(lastCallTime,timeZone,shiftTypeId);						
         						
                                        while( numberOfShifts-- > 0 )						
                                        {						
                                             shift = Scheduling.Shifts.GetNextShiftOfSpecifiedType(shift.FinishDate,timeZone,shiftTypeId);						
                                        }						
						
                                        CallShouldBeCreated();						
						
                                        Scheduling.NewCall.ShiftID = shift.ShiftTypeID;						
                                        Scheduling.NewCall.TimeInShift = shift.StartDate;						
                                }	

                                //Get a interview record timezone id or site timezone id
                                function GetTimezoneId() : int
                                {
                                    if (Scheduling.Interview.TimezoneID.HasValue)
                                         return Scheduling.Interview.TimezoneID.Value;
      
                                     return TimezoneManager.GetDefaultCallCenterTimezoneId();
                                }					

                            "
                        }
                    }
                }
            }.Create();

            new DateTimeMocker(firstShiftDate);

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var call = context.GetCall("S1.I1");

            call.Assert.IsTrue(x => x != null);
            call.Assert.IsTrue(x => x.TimeInShift == DateTime.Parse("2017-05-18 09:00"));

            new DateTimeMocker(secondShiftDate);

            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            call = context.GetCall("S1.I1");

            call.Assert.IsTrue(x => x != null);
            call.Assert.IsTrue(x => x.TimeInShift == DateTime.Parse("2017-05-19 09:00"));
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SetTimezoneId_CreateCall_CallIsCreatedInNewTimezone(SecurityMode mode)
        {
            SetSecurityMode(mode);
            TimezoneManager.AddTimezone(16);
            TimezoneManager.AddTimezone(39);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", TimeZoneId = "16"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new []
                            {
                                new Action(Action.Operation.RunCustomScript, "setTz"),
                                new Action(Action.Operation.SetShiftType, "0"/*Any valid*/),
                                new Action(Action.Operation.SetNewCallPriority, "20")
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function setTz(){Scheduling.Interview.TimezoneID = 39;}"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions());

            var internalCall = BvSvyScheduleAdapter.GetAll().Single();

            Assert.AreEqual(2, internalCall.CallState);
            Assert.AreEqual(-39, internalCall.ShiftTypeID);
        }

        [Theory, Owner(@"FIRM\MaximL")]
        [ClassData(typeof(TestDataGenerator))]
        public void SetTimezoneId_CreateCallToSpecificTime_CallIsCreatedInWithCorrectUtcTime(SecurityMode mode)
        {
            var startTime = DateTime.Parse("2018-10-09T08:00:00");
            var timeMocker = new DateTimeMocker(startTime);

            SetSecurityMode(mode);
            TimezoneManager.AddTimezone(16);
            TimezoneManager.AddTimezone(39);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Interviews = new[] {new InterviewData() {Tag = "S1.I1", TimeZoneId = "16"}}
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new []
                            {
                                new Action(Action.Operation.RunCustomScript, "setTz"),
                                new Action(Action.Operation.RecallAfterANumberOfShifts, "1")
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @"function setTz(){Scheduling.Interview.TimezoneID = 39;}"
                        }
                    }
                }
            }.Create();

            var interview = context.GetInterview("S1.I1");

            InterviewRepository.Update(interview.Model, new SchedulingScriptExecutionOptions());

            context.GetCall("S1.I1").Assert
                .AreEqual(2, x => x.CallState, "Wrong call state")
                .AreEqual(DateTime.Parse("2018-10-13T15:00:00"), x => x.TimeInShift, "Wrong time in shift");
        }

        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CustomFunction_GettingError_ErrorLoggedToDatabase(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimezoneManager.AddTimezone(16);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },

                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1", TimeZoneId = "16"},
                            new InterviewData() {Tag = "S1.I2", Data="q1=1", TimeZoneId = "16"},
                            new InterviewData() {Tag = "S1.I3", Data="q1=1", TimeZoneId = "16"},
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CustomFunction"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                function CustomFunction()		
                                {		
                                  f('customVariable').setValue('customValue');			
                                }						
                            "
                        }
                    }
                }
            }.Create();

            ServiceLocator.Resolve<ISchedulingScriptSettings>().ErrorLogSize = 2;
            var errorRepository = ServiceLocator.Resolve<IScheduleErrorRepository>();
            var survey = context.GetSurvey("S1");
            var schedule = ServiceLocator.Resolve<ISurveyService>().GetSchedule(survey.Id);

            var interview = context.GetInterview("S1.I1");
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            var errors = errorRepository.GetByScheduleId(schedule.ScheduleID);
            Assert.AreEqual(1, errors.Count, "Error was not stored");
            var firstError = errors[0];
            Assert.AreEqual(interview.Id, firstError.InterviewId);
            Assert.AreEqual(schedule.ScheduleID, firstError.ScheduleID);
            Assert.AreEqual(survey.Id, firstError.SurveySid);
            Assert.AreEqual(SchedulingScriptExecutionReasonConverter.ConvertToString(SchedulingScriptExecutionReason.MovedAndRescheduled), firstError.TriggeredBy);

            interview = context.GetInterview("S1.I2");
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            errors = errorRepository.GetByScheduleId(schedule.ScheduleID);
            Assert.AreEqual(2, errors.Count, "Second error was not stored");
            Assert.IsTrue(errors.Contains(firstError));
            var secondError = errors[1];
            Assert.AreNotEqual(firstError, secondError);

            interview = context.GetInterview("S1.I3");
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });

            errors = errorRepository.GetByScheduleId(schedule.ScheduleID);
            var thirdError = errors[1];
            Assert.AreEqual(2, errors.Count, "First Error should be deleted");
            Assert.IsTrue(errors.Contains(secondError));
            Assert.IsFalse(errors.Contains(firstError));
            Assert.AreNotEqual(secondError, thirdError);
        }

        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void CustomScript_CustomFunction_ExcecutionStepsLoggedToDatabase_CleanupWorks(SecurityMode mode)
        {
            SetSecurityMode(mode);

            TimezoneManager.AddTimezone(16);
            var context = new TestData()
            {
                Surveys = new[]
                {
                    new SurveyData(){ Tag = "S1", IsUseDb = true, SchedulingScript = "SS1",
                        Forms = new []
                        {
                            new SingleFormData{Name="q1", Precodes = new []{"1", "2", "3"}}
                        },

                        Interviews = new[]
                        {
                            new InterviewData() {Tag = "S1.I1", Data="q1=1", TimeZoneId = "16"},
                            new InterviewData() {Tag = "S1.I2", Data="q1=1", TimeZoneId = "16"},
                            new InterviewData() {Tag = "S1.I3", Data="q1=1", TimeZoneId = "16"},
                        }
                    }
                },
                Scripts = new[]
                {
                    new ScriptData() { Tag = "SS1",
                        Script = new TestScript(
                            new SubRule ( new []
                            {
                                new Action(Action.Operation.RunCustomScript, "CustomFunction"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00"))
                        {
                            CustomScript = @" 
                                function CustomFunction()		
                                {
                                  LogMessage('Custom Log Message');
                                  f('customVariable').setValue('customValue');			
                                }						
                            "
                        }
                    }
                }
            }.Create();


            var logRepository = ServiceLocator.Resolve<ISchedulingScriptLogRepository>();
            var survey = context.GetSurvey("S1");

            var interview = context.GetInterview("S1.I1");
            InterviewRepository.Update(
                interview.Model,
                new SchedulingScriptExecutionOptions { ExecutionReason = SchedulingScriptExecutionReason.MovedAndRescheduled, IsLogToHistory = false });


            var logs = logRepository.GetByInterviewId(survey.Id, interview.Id);
            Assert.AreEqual(1, logs.Count);
            var message = logs[0].LogMessages;
            Assert.IsTrue(message.Contains($"Starting scheduling script execution. Triggered by: \"{SchedulingScriptExecutionReasonConverter.ConvertToString(SchedulingScriptExecutionReason.MovedAndRescheduled)}\", Extended Status: \"Fresh sample\""));
            Assert.IsTrue(message.Contains("Executing rule 1.1"));
            Assert.IsTrue(message.Contains("Custom Log Message"));
            Assert.IsTrue(message.Contains("Executing action: \"Run custom script CustomFunction\""));
            Assert.IsTrue(message.Contains("Run custom script"));
            Assert.IsTrue(message.Contains("Error: Survey variable 'customVariable' was not found."));
            Assert.IsTrue(message.Contains("Finishing scheduling script execution"));

            ServiceLocator.Resolve<ISchedulingScriptLogTableCleanupSettings>().ExpirationPeriod = new TimeSpan(120, 0, 0, 0);
            ServiceLocator.Resolve<CleanSchedulingScriptLogTableAction>().Execute(RoutineMaintenanceShiftType.None);
            logs = logRepository.GetByInterviewId(survey.Id, interview.Id);
            Assert.AreEqual(1, logs.Count);

            ServiceLocator.Resolve<ISchedulingScriptLogTableCleanupSettings>().ExpirationPeriod = new TimeSpan(0, 0, 0, 0);
            ServiceLocator.Resolve<CleanSchedulingScriptLogTableAction>().Execute(RoutineMaintenanceShiftType.None);
            logs = logRepository.GetByInterviewId(survey.Id, interview.Id);
            Assert.AreEqual(0, logs.Count);
        }

        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void DialingPredictive_NotConnectedCallOutcome_CallOutcomeMetadataIsAvailableInCustomScript(SecurityMode mode)
        {
            SetSecurityMode(mode);
            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() {
                        Tag = "S1", IsUseDb = true, IsOpen = true, AssignsS = "P1", DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new[] {
                            new InterviewData() { Tag = "S1.I1", DialMode = "0", TelephoneNumber  = "1234", Call = new CallData() { Resource = "P1" } },
                            new InterviewData() { Tag = "S1.I2", DialMode = "0", TelephoneNumber  = "4321", Call = new CallData() { Resource = "P1" } }
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] {
                    new DialerData() { Tag = "D1" }
                },
                Scripts = new[] {
                    new ScriptData() {
                        Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new[] {
                                new Action(Action.Operation.RunCustomScript, "CustomFunction"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00")) {
                            CustomScript = @" 
                                function CustomFunction()		
                                {
                                  LogMessage('DialId: ' + Scheduling.LastDialingAttempt.DialId);
                                  LogMessage('TelephoneNumber: ' + Scheduling.LastDialingAttempt.TelephoneNumber);
                                  LogMessage('DialerCallerId: ' + Scheduling.LastDialingAttempt.DialerCallerId);
                                  LogMessage('RingTime: ' + Scheduling.LastDialingAttempt.RingTime);
                                  LogMessage('Custom1: ' + Scheduling.LastDialingAttempt.GetMetadata('Custom1'));	
                                  LogMessage('Custom3: ' + Scheduling.LastDialingAttempt.GetMetadata('Custom3'));	
                                  LogMessage('DialerCallOutcome: ' + Scheduling.LastDialingAttempt.DialerCallOutcome);	
                                  LogMessage('LastCallDialingAttempts.Length: ' + Scheduling.LastCallDialingAttempts.Length);
                                  LogMessage('LastCallDialingAttempts[0].RingTime: ' + Scheduling.LastCallDialingAttempts[0].RingTime);	
                                }						
                            "
                        }
                    }
                }
            }.Create();

            var dialer = context.GetDialer("D1");
            var logRepository = ServiceLocator.Resolve<ISchedulingScriptLogRepository>();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            var call1 = context.GetCall("S1.I1");
            var call2 = context.GetCall("S1.I2");
            var interview1 = context.GetInterview("S1.I1");
            var interview2 = context.GetInterview("S1.I2");
            var callerId = "TestDialerCallerId123";
            var ringTimeSeconds = 64;
            var callOutcomeMetadata = new Dictionary<string, string>() {
                { "Custom1", "123" },
                { "Custom3", "88" }
            };
            var console = new PredictiveConsoleController(context, person, survey, dialer);
            var callsRequest = console.LoginAndStart(10, CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly, person.Id);


            dialer.Helper.SendEventNotifyOutcome(survey.Model.CampaignId, person.Id, call1.Id, CallOutcome.ReturnedNotDialled, callerId, ringTimeSeconds, callOutcomeMetadata);
            var logs = logRepository.GetByInterviewId(survey.Id, interview1.Id);
            Assert.AreEqual(1, logs.Count);
            var message = logs[0].LogMessages;
            Assert.IsTrue(message.Contains("Returned not dialled"));
            Assert.IsTrue(message.Contains("DialerCallerId: TestDialerCallerId123"));
            Assert.IsTrue(message.Contains("TelephoneNumber: 1234"));
            Assert.IsTrue(message.Contains("RingTime: 64"));
            Assert.IsTrue(message.Contains("Custom1: 123"));
            Assert.IsTrue(message.Contains("Custom3: 88"));
            Assert.IsTrue(message.Contains("DialerCallOutcome: 15"));

            dialer.Helper.SendEventNotifyOutcome(survey.Model.CampaignId, person.Id, call2.Id, CallOutcome.ReturnedDiallerExpired, callerId, ringTimeSeconds, callOutcomeMetadata);
            logs = logRepository.GetByInterviewId(survey.Id, interview2.Id);
            Assert.AreEqual(1, logs.Count);
            message = logs[0].LogMessages;
            Assert.IsTrue(message.Contains("Returned dialler expired"));
            Assert.IsTrue(message.Contains("DialerCallerId: TestDialerCallerId123"));
            Assert.IsTrue(message.Contains("TelephoneNumber: 4321"));
            Assert.IsTrue(message.Contains("RingTime: 64"));
            Assert.IsTrue(message.Contains("Custom1: 123"));
            Assert.IsTrue(message.Contains("DialerCallOutcome: 25"));
        }
        
        [Theory, Owner(@"FIRM\EgorK")]
        [ClassData(typeof(TestDataGenerator))]
        public void ExcecuteSchedulingScript_CallHistoryIsAvailableInCustomScript(SecurityMode mode)
        {
            SetSecurityMode(mode);
            
            TimezoneManager.AddTimezone(16);
            var context = new TestData() {
                Surveys = new[] {
                    new SurveyData() {
                        Tag = "S1", IsUseDb = true, IsOpen = true, AssignsS = "P1", DialMode = DialingMode.Manual, SchedulingScript = "SS1",
                        Interviews = new[] {
                            new InterviewData() {
                                Tag = "S1.I1", DialMode = "0", TelephoneNumber  = "1234", Call = new CallData() { Resource = "P1" },
                                TimeZoneId = "16"
                            }
                        }
                    }
                },
                Persons = new[] { new PersonData() { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Scripts = new[] {
                    new ScriptData() {
                        Tag = "SS1",
                        Script = new TestScript(
                            new SubRule(new[] {
                                new Action(Action.Operation.RunCustomScript, "CustomFunction"),
                            }),
                            new Shift(1, 1, "0.00:00:00", "1.00:00:00")) {
                            CustomScript = @" 
                                function CustomFunction()		
                                {
                                    CallShouldBeCreated();
                                    var callAttempts = Scheduling.GetCallHistory(); 
                                    LogMessage('call attempts count: ' + callAttempts.Length);
                                    
                                    var callAttempt = callAttempts[0];

                                    LogMessage('AaporCode: ' + callAttempt.AaporCode);
                                    LogMessage('AttemptNumber: ' + callAttempt.AttemptNumber);
                                    LogMessage('CallCenterId: ' + callAttempt.CallCenterId);
                                    LogMessage('ConnectedTime: ' + callAttempt.ConnectedTime);
                                    LogMessage('Duration: ' + callAttempt.Duration);
                                    LogMessage('EndTimeRespondent: ' + callAttempt.EndTimeRespondent);
                                    LogMessage('EndTimeUtc: ' + callAttempt.EndTimeUtc);
                                    LogMessage('ExtendedStatus: ' + callAttempt.ExtendedStatus);
                                    LogMessage('InterviwerId: ' + callAttempt.InterviwerId);
                                    LogMessage('OpenEndReviewDuration: ' + callAttempt.OpenEndReviewDuration);
                                    LogMessage('PreviewTime: ' + callAttempt.PreviewTime);
                                    LogMessage('StartTimeRespondent: ' + callAttempt.StartTimeRespondent);
                                    LogMessage('StartTimeUtc: ' + callAttempt.StartTimeUtc);
                                    LogMessage('TelephoneNumber: ' + callAttempt.TelephoneNumber);
                                    LogMessage('WaitingTime: ' + callAttempt.WaitingTime);
                                    LogMessage('WrapTime: ' + callAttempt.WrapTime);
                                    LogMessage('Busy attempts: ' + Scheduling.GetCallHistory(ExtendedStatus.Busy).Length);
                                    LogMessage('Completed attempts: ' + Scheduling.GetCallHistory(ExtendedStatus.Completed, 1).Length);
                                    LogMessage('Telephone number attempts: ' + Scheduling.GetCallHistory('1234').Length);
                                }				
                            "
                        }
                    }
                }
            }.Create();
            
            var startTimeUtc = DateTime.UtcNow;
            var endTimeUtc = startTimeUtc.AddSeconds(40);
            var startTimeRespondent = startTimeUtc.AddHours(3);
            var endTimeRespondent = endTimeUtc.AddHours(3);
            
            var surveyDataService = ServiceLocator.Resolve<ISurveyDatabaseService>();
            var dateTimeMocker = new DateTimeMocker(TestingFramework);
            dateTimeMocker.MockDate(endTimeUtc);
            var stub = TestingFramework.RegistryStub<IInterviewTimings, StubIInterviewTimings>();
            stub.GetInterviewTimingsBvTasksEntityBvSurveyEntity = (task, sv) => new BvInterviewTimings() {
                CallCenterID = 1,
                PreviewTime = 8,
                WaitingTime = 7,
                OpenEndReviewDurationTime = 17,
                ConnectedTime = 21,
                WrapTime = 3,
                InterviewDurationTime = 40,
                TimeCallDelivered = startTimeUtc
            };
            
            var logRepository = ServiceLocator.Resolve<ISchedulingScriptLogRepository>();
            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");
            
            var console = new AutomaticConsoleController(context, person, survey);
            console.Login();


            var interview = console.StartInterview();
            surveyDataService.IncrementCallAttemptCount(survey.Id, interview.Id);
            console.FinishInterview(interview, new CompletedInterviewDetails {
                Its = "2",
                Status = "Complete",
                InterviewDuration = 45,
            });

            interview = console.StartInterview();
            surveyDataService.IncrementCallAttemptCount(survey.Id, interview.Id);
            console.FinishInterview(interview, new CompletedInterviewDetails {
                Its = "13",
                Status = "Complete",
            });
            
            var logs = logRepository.GetByInterviewId(survey.Id, interview.Id);
            Assert.AreEqual(2, logs.Count);
            var message1 = logs[0].LogMessages;
            Assert.IsTrue(message1.Contains("call attempts count: 1"));
            Assert.IsTrue(message1.Contains("AaporCode: 3.121"));
            Assert.IsTrue(message1.Contains("AttemptNumber: 1"));
            Assert.IsTrue(message1.Contains("CallCenterId: 1"));
            Assert.IsTrue(message1.Contains("ConnectedTime: 21"));
            Assert.IsTrue(message1.Contains("Duration: 40"));
            Assert.IsTrue(message1.Contains($"EndTimeRespondent: {endTimeRespondent}"));
            Assert.IsTrue(message1.Contains($"EndTimeUtc: {endTimeUtc}"));
            Assert.IsTrue(message1.Contains("ExtendedStatus: Busy"));
            Assert.IsTrue(message1.Contains("InterviwerId: "));
            Assert.IsTrue(message1.Contains("OpenEndReviewDuration: 17"));
            Assert.IsTrue(message1.Contains("PreviewTime: 8"));
            Assert.IsTrue(message1.Contains($"StartTimeRespondent: {startTimeRespondent}"));
            Assert.IsTrue(message1.Contains($"StartTimeUtc: {startTimeUtc}"));
            Assert.IsTrue(message1.Contains("TelephoneNumber: 1234"));
            Assert.IsTrue(message1.Contains("WaitingTime: 7"));
            Assert.IsTrue(message1.Contains("WrapTime: 3"));
            Assert.IsTrue(message1.Contains("Busy attempts: 1"));
            Assert.IsTrue(message1.Contains("Completed attempts: 0"));
            Assert.IsTrue(message1.Contains("Telephone number attempts: 1"));
            
            var message2 = logs[1].LogMessages;
            Assert.IsTrue(message2.Contains("call attempts count: 2"));
            Assert.IsTrue(message2.Contains("AaporCode: 1.1"));
            Assert.IsTrue(message2.Contains("ExtendedStatus: Completed"));
            Assert.IsTrue(message2.Contains("AttemptNumber: 2"));
            Assert.IsTrue(message2.Contains("Busy attempts: 1"));
            Assert.IsTrue(message2.Contains("Completed attempts: 1"));
            Assert.IsTrue(message2.Contains("Telephone number attempts: 2"));
        }
    }
}