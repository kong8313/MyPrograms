using System.Collections.Generic;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.SystemSettings.Fakes;
using Confirmit.CATI.Core.SystemSettings.Toggle.Fakes;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Mocks;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class DialerNotifyDroppedByRespondentHandlerTest : BaseMockedIntegrationTest
    {
        public override void OnPostTestInitialize()
        {
            new VoiceXmlServiceController(TestingFramework, new[] { new VoiceXmlPageModel(), VoiceXmlServiceController.TransferPage(TransferType.InternalCold, "Live Inters") });

            var stubToggleSettings = TestingFramework.RegistryStub<IToggleSettings, StubIToggleSettings>();
            stubToggleSettings.CatiAgentGet = () => new StubICatiAgentSettings() { IvrThreadGet = () => false };
            stubToggleSettings.EnableIVRGet = () => true;
            stubToggleSettings.EnableDesktopConsoleLoginGet = () => true;
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForIvrAgent_TransferToLiveAgent_DropCall()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        Tag = "S1",
                        SchedulingScript = "SS1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        },
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1.Ivr" },
                    new PersonGroupData { Tag = "PG2.Live", Name = "Live Inters" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent, Memberships = "PG1.Ivr" },
                    new PersonData { Tag = "P2.Live", Type = AgentType.LiveAgent, Memberships = "PG2.Live" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1" }
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag ="SS1",
                        Script = new TestScript(CallOutcome.DroppedByRespondent, Framework.Tools.Action.Operation.SetNewITS, "31")
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, livePerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();


            dialer.SetNotificationReply(ReplyType.Postponed);

            //Login
            IvrConsoleController.ExecutePeriodicalWork();
            //Start interview
            IvrConsoleController.ExecutePeriodicalWork();

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.INTERVIEWING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            int callId = task.CallID.Value;

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.NO_CALLS, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(0, task.SurveySID);

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, 0, callId);
            Assert.AreEqual(31, context.GetInterview("S1.I1").Model.TransientState);
        }
        
        [TestMethod, Owner(@"Firm\EgorK")]
        public void StartInterviewForIvrAgent_TransferToLiveAgent_ChangeCallState_NotifyCallDropedByRespondent_NoExceptionExpected()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        Tag = "S1",
                        SchedulingScript = "SS1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        },
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1.Ivr" },
                    new PersonGroupData { Tag = "PG2.Live", Name = "Live Inters" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent, Memberships = "PG1.Ivr" },
                    new PersonData { Tag = "P2.Live", Type = AgentType.LiveAgent, Memberships = "PG2.Live" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1" }
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag ="SS1",
                        Script = new TestScript(CallOutcome.DroppedByRespondent, Framework.Tools.Action.Operation.SetNewITS, "31")
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, livePerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();


            dialer.SetNotificationReply(ReplyType.Postponed);

            //Login
            IvrConsoleController.ExecutePeriodicalWork();
            //Start interview
            IvrConsoleController.ExecutePeriodicalWork();

            dialer.ProcessAllPosponedNotification();

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            int callId = task.CallID.Value;

            dialer.ProcessAllPosponedNotification();

            CallQueueService.DeleteCall(context.GetSurvey("S1").Id, context.GetInterview("S1.I1").Id);//call dropped by interviewer
            
            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, 0, callId);
            //no exception expected
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForIvrAgent_WarmTransferToLiveAgent_DropCallAndCancelTransfer_NoException()
        {
            new VoiceXmlServiceController(TestingFramework, new[] { new VoiceXmlPageModel(), VoiceXmlServiceController.TransferPage(TransferType.InternalWarm, "Live Inters") });
            
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        Tag = "S1",
                        SchedulingScript = "SS1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        },
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1.Ivr" },
                    new PersonGroupData { Tag = "PG2.Live", Name = "Live Inters" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent, Memberships = "PG1.Ivr" },
                    new PersonData { Tag = "P2.Live", Type = AgentType.LiveAgent, Memberships = "PG2.Live" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1" }
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag ="SS1",
                        Script = new TestScript(CallOutcome.DroppedByRespondent, Framework.Tools.Action.Operation.SetNewITS, "31")
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, livePerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();
            var transferCancelParams = dialer.Behavior.Methods.TransferCancel.Init();
            
            dialer.SetNotificationReply(ReplyType.Postponed);

            //Login
            IvrConsoleController.ExecutePeriodicalWork();
            //Start interview
            IvrConsoleController.ExecutePeriodicalWork();

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.INTERVIEWING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            int callId = task.CallID.Value;

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);
            Assert.AreEqual(0, transferCancelParams.Count);
            
            task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.OUTGOING_TRANSFER, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, ivrPerson.Id, callId);
            Assert.AreEqual(1, transferCancelParams.Count);
            Assert.AreEqual(31, context.GetInterview("S1.I1").Model.TransientState);
            
            task = ivrConsole.Task;
            Assert.AreEqual((int)InterviewState.NO_CALLS, task.InterviewState);
        }
        
        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForIvrAgent_DropCall_CheckThatCallIsDropped()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        Tag = "S1",
                        SchedulingScript = "SS1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        },
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1.Ivr" },
                    new PersonGroupData { Tag = "PG2.Live", Name = "Live Inters" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent, Memberships = "PG1.Ivr" },
                    new PersonData { Tag = "P2.Live", Type = AgentType.LiveAgent, Memberships = "PG2.Live" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1" }
                },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag ="SS1",
                        Script = new TestScript(CallOutcome.DroppedByRespondent, Framework.Tools.Action.Operation.SetNewITS, "31")
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, livePerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();


            dialer.SetNotificationReply(ReplyType.Postponed);

            //Login
            IvrConsoleController.ExecutePeriodicalWork();
            //Start interview
            IvrConsoleController.ExecutePeriodicalWork();

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.INTERVIEWING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            int callId = task.CallID.Value;

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, ivrPerson.Id, callId);

            Assert.AreEqual(31, context.GetInterview("S1.I1").Model.TransientState);
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForIvrAgent_DropCallForWrongCallId_NothingHappened()
        {
            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        Tag = "S1",
                        SchedulingScript = "SS1",
                        DialMode = DialingMode.Automatic,
                        AssignsS = "P1.Ivr",
                        Interviews = new []
                        {
                            new InterviewData() { Tag ="S1.I1", Call = new CallData() },
                            new InterviewData() { Tag ="S1.I2", Call = new CallData() }
                        },
                    }
                },
                Persons = new[] { new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent } },
                Dialers = new[] { new DialerData { Tag = "D1" } },
                Scripts = new[]
                {
                    new ScriptData()
                    {
                        Tag ="SS1",
                        Script = new TestScript(CallOutcome.DroppedByRespondent, Framework.Tools.Action.Operation.SetNewITS, "31")
                    }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");

            dialer.SetNotificationReply(ReplyType.Postponed);

            IvrConsoleController.ExecutePeriodicalWork();

            dialer.ProcessAllPosponedNotification();

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.INTERVIEWING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            dialer.SendEventNotifyDropCallByRespondent(survey.Model.CampaignId, ivrPerson.Id, context.GetCall("S1.I2").Model.CallID);

            task = TaskRepository.GetByPerson(ivrPerson.Id);

            Assert.AreEqual((byte)CallConnectionState.Connected, task.CallConnectionState);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(16, context.GetInterview("S1.I1").Model.TransientState);
            Assert.AreEqual(16, context.GetInterview("S1.I2").Model.TransientState);
        }
    }
}
