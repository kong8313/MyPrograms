using Confirmit.CATI.Backend.WebApiServices.Models;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Fakes;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Core.Telephony.Fakes;
using Confirmit.CATI.IntegrationTests.Framework;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Action = Confirmit.CATI.IntegrationTests.Framework.Tools.Action;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class DialerNotifyInboundCallHandlerTest : BaseMockedIntegrationTest
    {
        [TestMethod]
        public void Handler_should_dropCall_if_inbound_disabled()
        {
            // arrange
            var inboundCallNumber = Guid.NewGuid().ToString();
            var telephonyDropExecuted = false;
            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                DropInboundCallInt32StringAudioMessageDescriptor = (id, callId, audio) => {
                    telephonyDropExecuted = true;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, false}
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                
            }.Create();

            // act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyInboundCall(inboundCallNumber, "456", "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsTrue(telephonyDropExecuted);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemInboundDisabled, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
        }

        [TestMethod]
        public void Handler_should_dropCall_if_interview_missing()
        {
            // arrange
            var inboundCallNumber = Guid.NewGuid().ToString();
            var telephonyDropExecuted = false;

            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                DropInboundCallInt32StringAudioMessageDescriptor = (id, callId, audio) => {
                    telephonyDropExecuted = true;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData() { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyInboundCall(inboundCallNumber, "456", "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsTrue(telephonyDropExecuted);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemInterviewNotFound, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.AreEqual(context.GetSurvey("S1").Id, inboundCallsHistory.SurveyId);
        }

        [TestMethod]
        public void Handler_should_dropCall_if_call_state_inactive()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var telephonyDropExecuted = false;

            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                DropInboundCallInt32StringAudioMessageDescriptor = (id, callId, audio) => {
                    telephonyDropExecuted = true;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData { CallState = -1 }, TelephoneNumber = callerNumber },
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber }
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData {
                    Tag = "SS1", Script = new TestScript(new SubRule(new []
                        {
                            new Action(Action.Operation.AcceptInboundCall, string.Empty),
                        }),
                        Shift.Week)} }
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsTrue(telephonyDropExecuted);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemWrongCallState, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
        }

        [TestMethod]
        public void Handler_should_dropCall_if_survey_closed()
        {
            // arrange
            var inboundCallNumber = Guid.NewGuid().ToString();
            var telephonyDropExecuted = false;

            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                DropInboundCallInt32StringAudioMessageDescriptor = (id, callId, audio) => {
                    telephonyDropExecuted = true;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = false, DialMode = DialingMode.Predictive,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyInboundCall(inboundCallNumber, "456", "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsTrue(telephonyDropExecuted);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemSurveyIsNotOpened, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.AreEqual(context.GetSurvey("S1").Id, inboundCallsHistory.SurveyId);
        }

        [TestMethod, Owner(@"Firm\alm")]
        public void Handler_predictive_should_dropCall_if_no_agents_logged_in()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var telephonyDropExecuted = false;

            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                DropInboundCallInt32StringAudioMessageDescriptor = (id, callId, audio) => {
                    telephonyDropExecuted = true;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1" }, TelephoneNumber = callerNumber },
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new []{
                        new SubRule( new Action(Action.Operation.AcceptInboundCall, string.Empty),(int)CallOutcome.InboundCall, 0, 0, null, false ),
                        new SubRule( new Action(Action.Operation.SetNewITS, "40"),(int)CallOutcome.InterruptedBySystem, 0, 0, null, false )
                    },
                    Shift.Week)} }
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            var history = BvDialHistoryAdapter.GetAll().SingleOrDefault();

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsTrue(telephonyDropExecuted);
            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Active dial table isn't empty");
            Assert.IsNotNull(history, "Dial history record wasn't created");
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemNoAgentsAvailable, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == 40);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod, Owner(@"Firm\alm")]
        public void Handler_automatic_should_dropCall_if_no_agents_logged_in()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            var telephonyDropExecuted = false;

            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                DropInboundCallInt32StringAudioMessageDescriptor = (id, callId, audio) => {
                    telephonyDropExecuted = true;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1" }, TelephoneNumber = callerNumber },
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new []{
                        new SubRule( new Action(Action.Operation.AcceptInboundCall, string.Empty),(int)CallOutcome.InboundCall, 0, 0, null, false ),
                        new SubRule( new Action(Action.Operation.SetNewITS, "40"),(int)CallOutcome.InterruptedBySystem, 0, 0, null, false )
                    },
                    Shift.Week)} }
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsTrue(telephonyDropExecuted);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemNoAgentsAvailable, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == 40);
            context.GetCall("S1.I1").Assert.IsNull();
        }

        [TestMethod]
        public void Handler_predictive_survey_should_execute_ConnectInboundCall()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1" }, TelephoneNumber = callerNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new [] { new ScriptData { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall, string.Empty),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            //act
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);
            var interview = context.GetInterview("S1.I1");
            var call = context.GetCall("S1.I1");
            var callHistory = BvCallHistoryExAdapter.GetByCondition("SurveyId = @SurveyId AND InterviewID = @InterviewID",
                new SqlParameter("@SurveyId", interview.Survey.Id), new SqlParameter("@InterviewId", interview.Id)).ToList();
            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.IsNotNull(callHistory);
            Assert.AreEqual(InboundHandlerOperationType.SendToDialer, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNotNull(connectInboundCallParams); // connect was executed
            Assert.AreEqual(connectInboundCallParams.CallInfo.interviewId, interview.Id);
            Assert.AreEqual(CallState.LoadedToDialerPredictively, (CallState)call.Model.CallState);
        }

        [TestMethod]
        public void Handler_nonpredictive_survey_disabled_state_in_schedulingScript_should_drop_call()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Manual, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = callerNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new SubRule(new [] { new Action(Action.Operation.AcceptInboundCall, string.Empty), new Action(Action.Operation.DisableCall, string.Empty)}),
                    Shift.Week)} } // change call state to disabled
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            //act
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);
            var call = context.GetCall("S1.I1");

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemWrongCallState, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
            Assert.AreEqual(CallState.DisabledByUser, (CallState)call.Model.CallState); // result call state should change
        }

        [TestMethod]
        public void Handler_nonpredictive_scheduled_state_should_pass_and_create_history()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = callerNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall, string.Empty),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            //act
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);
            var call = context.GetCall("S1.I1");

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.PlacedInQueue, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
            Assert.AreEqual(CallState.Scheduled, (CallState)call.Model.CallState); // result call state should change
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Handler_new_nonpredictive_calls_should_not_be_accepted_from_fresh_sample_recond()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Manual, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1_NotEmptyTelNumbner", Call = new CallData(), TelephoneNumber = "123" },
                            new InterviewData { Tag = "S1.I2_NotFreshSample", Call = new CallData(), ITS=CallOutcome.Busy, TelephoneNumber = "" },
                            new InterviewData { Tag = "S1.I3_WithoutCall", TelephoneNumber = "" },
                            new InterviewData { Tag = "S1.I4_WithWrongCallState", Call = new CallData {CallState = (int)CallState.DisabledByUser}, TelephoneNumber = "" },
                            new InterviewData { Tag = "S1.I5_WithWrongCallState", Call = new CallData {CallState = (int)CallState.ToBeDeleted}, TelephoneNumber = "" },
                            new InterviewData { Tag = "S1.I6_WithWrongCallState", Call = new CallData {CallState = (int)CallState.InterviewInProgress}, TelephoneNumber = "" },
                            new InterviewData { Tag = "S1.I7_WithWrongCallState", Call = new CallData {CallState = (int)CallState.LoadedToDialerPredictively}, TelephoneNumber = "" },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall, string.Empty),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            //act
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.DropBySystemInterviewNotFound, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Handler_nonpredictive_call_should_pass_clinumber_to_scheduling_script_and_accept_call()
        {
            var ddiNumber = Guid.NewGuid().ToString();
            var cliNumber = "1234";
            var startTime = DateTime.Parse("2018-03-13 12:00:00");

            var timeMocker = new DateTimeMocker(startTime);

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = cliNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = ddiNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(
                    new Action(Action.Operation.AcceptInboundCall, string.Empty, "Scheduling.CliNumber == '1234'"),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            var call = context.GetCall("S1.I1");
            
            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            var originalCall = call.Model;

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            //act
            dialer.SendNotifyInboundCall(ddiNumber, cliNumber, "ICID_2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == ddiNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.PlacedInQueue, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
            Assert.AreEqual(CallState.Scheduled, (CallState)call.Model.CallState); // result call state should change

            var activeDial = BvActiveDialAdapter.GetAll().Single();
            Assert.AreEqual((byte)CallTypes.Inbound, activeDial.Type, "Wrong call type");
            Assert.AreEqual((byte)DialState.Pending, activeDial.State, "Wrong state");
            Assert.AreEqual(startTime, activeDial.StartTime, "Wrong startTime");
            Assert.IsNull(activeDial.AnswerTime, "Wrong AnswerTime");
            Assert.AreEqual(ddiNumber, activeDial.DialerTelephoneNumber, "Wrong ddi numner");
            Assert.AreEqual(cliNumber, activeDial.RespondentTelephoneNumber, "Wrong caller numner");
            Assert.AreEqual(originalCall.SurveySID, activeDial.SurveyId, "Wrong survey id");
            Assert.AreEqual(originalCall.InterviewID, activeDial.InterviewId, "Wrong interview id");
            Assert.AreEqual(originalCall.CallID, activeDial.CallId, "Wrong call id");
            Assert.AreEqual(dialer.Id, activeDial.DialerId, "Wrong dialer id");
            Assert.AreEqual("ICID_2", activeDial.InboundCallId, "Wrong inbound call id");
            Assert.AreEqual(0, activeDial.MainPersonId, "Wrong person id");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Handler_nonpredictive_call_should_pass_clinumber_to_scheduling_script_and_reject_inbound_call()
        {
            var ddiNumber = Guid.NewGuid().ToString();
            var cliNumber = "1234";
            var startTime = DateTime.Parse("2018-03-13 12:00:00");

            var timeMocker = new DateTimeMocker(startTime);

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var originalTelephony = ServiceLocator.Resolve<ITelephony>();
            var telephony = TestingFramework.RegistryStub<ITelephony, StubITelephony>();
            telephony.Inner = originalTelephony;
            telephony.DropInboundCallInt32StringAudioMessageDescriptor = (dialerId, inboundCallid, msg) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(10));
                return DialerErrorCode.Success;
            };

            
            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = cliNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = ddiNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(
                    new Action(Action.Operation.AcceptInboundCall, string.Empty, "Scheduling.CliNumber == 'wrong_number'"),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            var call = context.GetCall("S1.I1");

            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            
            //act
            dialer.SendNotifyInboundCall(ddiNumber, cliNumber, "ICID_2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == ddiNumber);
            

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.DropBySchedulingScript, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
            Assert.IsNull(call.Model); // result call state should change

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Active dial table isn't empty");
            var history = BvDialHistoryAdapter.GetAll().SingleOrDefault();
            Assert.IsNotNull(history, "dial history record wasn't created");
            Assert.AreEqual((byte)CallTypes.Inbound, history.Type, "Wrong type of dial history record");
            Assert.AreEqual(ddiNumber, history.DialerTelephoneNumber, "Wrong ddi number of dial history record");
            Assert.AreEqual(cliNumber, history.RespondentTelephoneNumber, "Wrong TelephoneNumber of dial history record");
            Assert.AreEqual("ICID_2", history.InboundCallId, "Wrong InboundCallId of dial history record");
            Assert.AreEqual(dialer.Id, history.DialerId, "Wrong dialer id of dial history record");
            Assert.AreEqual(startTime, history.StartTime, "Wrong StartTime of dial history record");
            Assert.AreEqual(startTime.AddSeconds(10), history.FinishTime, "Wrong FinishTime of dial history record");
            Assert.IsNull(history.AnswerTime, "Wrong DialingTimeInSec of dial history record");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Handler_nonpredictive_call_should_pass_ddinumber_to_scheduling_script_and_accept_call()
        {
            var ddiNumber = "DDI-1234";
            var cliNumber = "CLI-5678";
            var startTime = DateTime.Parse("2018-03-13 12:00:00");

            var timeMocker = new DateTimeMocker(startTime);

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = cliNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = ddiNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(
                    new Action(Action.Operation.AcceptInboundCall, string.Empty, "Scheduling.DdiNumber == 'DDI-1234'"),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            var call = context.GetCall("S1.I1");

            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            var originalCall = call.Model;

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            //act
            dialer.SendNotifyInboundCall(ddiNumber, cliNumber, "ICID_2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == ddiNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.PlacedInQueue, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
            Assert.AreEqual(CallState.Scheduled, (CallState)call.Model.CallState); // result call state should change

            var activeDial = BvActiveDialAdapter.GetAll().Single();
            Assert.AreEqual((byte)CallTypes.Inbound, activeDial.Type, "Wrong call type");
            Assert.AreEqual((byte)DialState.Pending, activeDial.State, "Wrong state");
            Assert.AreEqual(startTime, activeDial.StartTime, "Wrong startTime");
            Assert.IsNull(activeDial.AnswerTime, "Wrong AnswerTime");
            Assert.AreEqual(ddiNumber, activeDial.DialerTelephoneNumber, "Wrong ddi numner");
            Assert.AreEqual(cliNumber, activeDial.RespondentTelephoneNumber, "Wrong caller numner");
            Assert.AreEqual(originalCall.SurveySID, activeDial.SurveyId, "Wrong survey id");
            Assert.AreEqual(originalCall.InterviewID, activeDial.InterviewId, "Wrong interview id");
            Assert.AreEqual(originalCall.CallID, activeDial.CallId, "Wrong call id");
            Assert.AreEqual(dialer.Id, activeDial.DialerId, "Wrong dialer id");
            Assert.AreEqual("ICID_2", activeDial.InboundCallId, "Wrong inbound call id");
            Assert.AreEqual(0, activeDial.MainPersonId, "Wrong person id");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Handler_nonpredictive_call_should_be_created_with_lock()
        {
            var ddiNumber = "DDI-1234";
            var callState = (int?)null;

            var stubIRespondentsClient = TestingFramework.RegistryStub<IRespondentsClient, StubIRespondentsClient>();
            var originalInterviewService = ServiceLocator.Resolve<IInterviewService>();
            var stubInterviewService = TestingFramework.RegistryStub<IInterviewService, StubIInterviewService>();
            stubInterviewService.Inner = originalInterviewService;
            stubInterviewService.AddRespondentBvSurveyEntityInt32SchedulingScriptExecutionOptions =
                (survey, interviewId, options) =>
                {
                    var result = originalInterviewService.AddRespondent(survey, interviewId, options);
                    using (IDataReader dr = BvSpCall_GetAdapter.ExecuteReader(survey.SID, interviewId, (int)CallLockMode.NoLock, (int)CallMode.Live))
                    {
                        callState = BvCallAdapter.Read(dr)?.CallState;
                    }
                    return result;
                };

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Automatic, SchedulingScript = "SS1",
                        InboundBehavior = InboundSurveyBehavior.CreateOnly, Assigns = new [] {"P1"}, IsUseDb = true,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = ddiNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1" } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(
                    new []{ new Action(Action.Operation.SetTimeToNOW, "0"), new Action(Action.Operation.AcceptInboundCall)},
                    Shift.Week)} }
            }.Create();

            stubIRespondentsClient.AddRespondentStringRespondentsInfo =
                (projectId, args) =>
                {
                    using (new TestConnectionUnscope())
                    {
                        var interviewData = new InterviewData { TelephoneNumber = args.Values["TelephoneNumber"].ToString() };
                        var survey = context.Surveys.Single();
                        var respId = survey.Database.AddInterview(1, "16", interviewData);

                        var interview = new InterviewController("S1.I1", context, survey, respId, survey.Database, interviewData);
                        context.Interviews.Add(interview);
                        context.Calls.Add(new CallRef(interview));

                        return respId;
                    }
                };

            var dialer = context.GetDialer("D1");

            var connectInboundCallToAgents = dialer.Behavior.Methods.ConnectInboundCallToAgent.Init();

            context.GetPerson("P1").Console.Login().LoginToDialer().Start().Wait();

            dialer.SendNotifyInboundCall(ddiNumber, "CLI-5678", "ICID_2");

            Assert.AreEqual(-1, callState);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void Handler_nonpredictive_call_should_pass_ddinumber_to_scheduling_script_and_reject_inbound_call()
        {
            var ddiNumber = "DDI-1234";
            var cliNumber = "CLI-5678";
            var startTime = DateTime.Parse("2018-03-13 12:00:00");

            var timeMocker = new DateTimeMocker(startTime);

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var originalTelephony = ServiceLocator.Resolve<ITelephony>();
            var telephony = TestingFramework.RegistryStub<ITelephony, StubITelephony>();
            telephony.Inner = originalTelephony;
            telephony.DropInboundCallInt32StringAudioMessageDescriptor = (dialerId, inboundCallid, msg) =>
            {
                timeMocker.AddTime(TimeSpan.FromSeconds(10));
                return DialerErrorCode.Success;
            };


            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1"}, TelephoneNumber = cliNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = ddiNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(
                    new Action(Action.Operation.AcceptInboundCall, string.Empty, "Scheduling.DdiNumber == 'wrong_number'"),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            var call = context.GetCall("S1.I1");

            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });


            //act
            dialer.SendNotifyInboundCall(ddiNumber, cliNumber, "ICID_2");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == ddiNumber);


            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(InboundHandlerOperationType.DropBySchedulingScript, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
            Assert.IsNull(connectInboundCallParams); // connect was not executed
            Assert.IsNull(call.Model); // result call state should change

            Assert.AreEqual(0, BvActiveDialAdapter.GetAll().Count, "Active dial table isn't empty");
            var history = BvDialHistoryAdapter.GetAll().SingleOrDefault();
            Assert.IsNotNull(history, "dial history record wasn't created");
            Assert.AreEqual((byte)CallTypes.Inbound, history.Type, "Wrong type of dial history record");
            Assert.AreEqual(ddiNumber, history.DialerTelephoneNumber, "Wrong ddi number of dial history record");
            Assert.AreEqual(cliNumber, history.RespondentTelephoneNumber, "Wrong TelephoneNumber of dial history record");
            Assert.AreEqual("ICID_2", history.InboundCallId, "Wrong InboundCallId of dial history record");
            Assert.AreEqual(dialer.Id, history.DialerId, "Wrong dialer id of dial history record");
            Assert.AreEqual(startTime, history.StartTime, "Wrong StartTime of dial history record");
            Assert.AreEqual(startTime.AddSeconds(10), history.FinishTime, "Wrong FinishTime of dial history record");
            Assert.IsNull(history.AnswerTime, "Wrong DialingTimeInSec of dial history record");
        }

        [TestMethod, Owner(@"Firm\alm")]
        public void Handler_ConnectInboundCall_should_pass_null_audio_descriptor_to_dialer()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            var connectInboundCallExecuted = false;
            var actualAudioMessageDescriptor = new AudioMessageDescriptor();

            var telephony = ServiceLocator.Resolve<ITelephony>();
            var stubTelephony = new StubITelephony
            {
                ConnectInboundCallInt32Int64Int32Int32StringCallInfoArrayOfInt64AudioMessageDescriptor = 
                    (id, campaignId, agentId, connectionId, callId, info, from, descriptor) =>
                {
                    connectInboundCallExecuted = true;
                    actualAudioMessageDescriptor = descriptor;
                    return DialerErrorCode.Success;
                }
            };
            Stubs.ExtendExistingITelephonyStub(telephony, stubTelephony);

            TestDialerHelper.ConnectInboundCallParams connectInboundCallParams = null;

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    {SystemSettingConstants.Toggle.EnableInbound, true}
                },
                Surveys = new[] {
                    new SurveyData { Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        Interviews = new []
                        {
                            new InterviewData {Tag = "S1.I1", Call = new CallData { CallState = 1, Tag = "S1.I1" }, TelephoneNumber = callerNumber },
                        },
                        Assigns = new [] {"P1"},
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber}
                        }
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData { Tag = "SS1", Script = new TestScript(new Action(Action.Operation.AcceptInboundCall, string.Empty),
                    Shift.Week)} }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Helper.SetBehaviorForConnectInboundCall(callParams =>
            {
                connectInboundCallParams = callParams;
                return (int)DialerErrorCode.Success;
            });

            var survey = context.GetSurvey("S1");
            var person = context.GetPerson("P1");

            var console = new PredictiveConsoleController(context, person, survey, dialer);
            console.LoginAndStart();

            //act
            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            
            // assert
            Assert.IsTrue(connectInboundCallExecuted, "ConnectInboundCall method call was expected, but the method is not executed.");
            Assert.IsNull(actualAudioMessageDescriptor, "It's expected that the audioMessageDescriptor parameter has to be null.");
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Handler_should_dropCall_if_no_acceptInboundCall_subrule_for_inbounf_calls()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();

            var respondentsClient = new StubIRespondentsClient();
            ServiceLocator.RegisterInstance<IRespondentsClient>(respondentsClient);

            var context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        InboundBehavior = InboundSurveyBehavior.MatchAndCreate,
                        Interviews = new []
                        {
                            new InterviewData { Tag = "S1.I1", Call = new CallData(), TelephoneNumber = callerNumber },
                        },
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber }
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData
                {
                    Tag = "SS1", Script = new TestScript(new Action(Action.Operation.IncrementPriority, "1"), Shift.Week)
                }}
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();

            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(1, dropInboundCallParams.Count);
            context.GetCall("S1.I1").Assert.IsTrue(x => x.CallState == (int)CallState.Scheduled);
            Assert.AreEqual(InboundHandlerOperationType.DropBySchedulingScript, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
        }

        [TestMethod, Owner(@"firm\grigoryk")]
        public void Handler_should_have_no_call_if_no_interview_and_no_acceptInboundCall_subrule_for_inbounf_calls()
        {
            var inboundCallNumber = Guid.NewGuid().ToString();
            var callerNumber = Guid.NewGuid().ToString();
            TestDataContext context = null;

            TestingFramework.RegistryStub<IRespondentsClient, StubIRespondentsClient>().AddRespondentStringRespondentsInfo =
                (projectId, args) =>
                {
                    using (TestConnectionUnscope testConnectionUnscope = new TestConnectionUnscope())
                    {
                        var interviewData = new InterviewData { TelephoneNumber = args.Values["TelephoneNumber"].ToString() };
                        var survey = context.Surveys.Single();
                        var respId = survey.Database.AddInterview(1, "16", interviewData);

                        var interview = new InterviewController("S1.I1", context, survey, respId, survey.Database, interviewData);
                        context.Interviews.Add(interview);
                        context.Calls.Add(new CallRef(interview));

                        return respId;
                    }
                };

            context = new TestData
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableInbound, true }
                },
                Surveys = new[] {
                    new SurveyData
                    {
                        Tag = "S1", IsOpen  = true, DialMode = DialingMode.Predictive, SchedulingScript = "SS1",
                        InboundBehavior = InboundSurveyBehavior.MatchAndCreate, IsUseDb = true,
                        InboundTelephoneNumbers = new []
                        {
                            new InboundTelephoneNumberData { Dialer = "D1", TelephoneNumber = inboundCallNumber }
                        }
                    }
                },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[] { new ScriptData
                {
                    Tag = "SS1", Script = new TestScript(new Action(Action.Operation.SetNewITS, "31"), Shift.Week)
                }}
            }.Create();

            //act
            var dialer = context.GetDialer("D1");
            var dropInboundCallParams = dialer.Behavior.Methods.DropInboundCall.Init();

            dialer.SendNotifyInboundCall(inboundCallNumber, callerNumber, "1");
            var inboundCallsHistory = BvInboundCallsHistoryAdapter.GetAll().FirstOrDefault(x => x.InboundTelNumber == inboundCallNumber);

            // assert
            Assert.IsNotNull(inboundCallsHistory);
            Assert.AreEqual(1, dropInboundCallParams.Count);
            context.GetInterview("S1.I1").Assert.IsTrue(x => x.TransientState == 31);
            context.GetCall("S1.I1").Assert.IsNull();
            Assert.AreEqual(InboundHandlerOperationType.DropBySchedulingScript, (InboundHandlerOperationType)inboundCallsHistory.OperationType);
        }
    }
}