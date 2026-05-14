using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.IntegrationTests.Framework.Controllers;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Mocks;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using Confirmit.CATI.IntegrationTests.Framework.Tools;
using Confirmit.SurveyVoiceXml.Service.Client.Models;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using TaskChoiceMode = Confirmit.CATI.Backend.WebApiServices.Models.TaskChoiceMode;

namespace Confirmit.CATI.IntegrationTests.Tests.Telephony
{
    [TestClass]
    public class TransferTest : BaseMockedIntegrationTest
    {
        private VoiceXmlServiceController voiceXmlService = null;

        public override void OnPostTestInitialize()
        {
            voiceXmlService = new VoiceXmlServiceController(TestingFramework, new[]
            {
                new VoiceXmlPageModel(),
                VoiceXmlServiceController.TransferPage(TransferType.InternalCold, "Live Inters")
            });
        }

        // не предиктив
        // трансверинг от IVR к live  +
        // трансверинг от live к IVR  +
        // трансверинг от live к live +

        // предиктив
        // трансверинг от live к live

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForIvrAgent_TransferToLiveAgent_InterviewWasTransferredAndCompletedSuccessful()
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
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        }
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
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed}
                }
            }.Create();

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

            //Login and start
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

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            task = ivrConsole.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.NO_CALLS);
            Assert.AreEqual(task.PersonSID, ivrPerson.Id);
            Assert.AreEqual(task.InterviewID, 0);
            Assert.AreEqual(task.SurveySID, 0);

            var liveConsole = livePerson.Console.Login().LoginToDialer().Start().Wait().Check(interviewTag: "S1.I1");

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            liveConsole.WrapUp().Wait().Check(interviewState: InterviewState.NO_CALLS);

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(2, completeCallParams.Count);
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForLiveAgent_TransferToIvrAgent_InterviewWasTransferredAndCompletedSuccessful()
        {
            TransferFromLiveToIvrAgent(false);
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForLiveAgent_TransferToIvrAgentWithEmptyGroupName_InterviewWasTransferredAndCompletedSuccessful()
        {
            TransferFromLiveToIvrAgent(true);
        }

        public void TransferFromLiveToIvrAgent(bool useEmptyGroupName)
        {
            voiceXmlService.Scenario = new[] { new VoiceXmlPageModel() };

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
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG2.Live" }
                            }
                        },
                        Assigns = useEmptyGroupName ? new [] { "P1.Ivr" } : new string[0]
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1.Ivr", Name = "IVR Inters" },
                    new PersonGroupData { Tag = "PG2.Live", Name = "Live Inters" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent, Memberships = "PG1.Ivr" },
                    new PersonData { Tag = "P2.Live", Type = AgentType.LiveAgent, Memberships = "PG2.Live" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");
            var ivrPersonGroup = context.GetPersonGroup("PG1.Ivr");

            var sendNumberToAgentParams = dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, ivrPerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            var liveConsole = livePerson.Console.Login().LoginToDialer().Start().Wait().Check(interviewTag: "S1.I1");

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            if (useEmptyGroupName)
            {
                liveConsole.StartTransfer(ConsoleTransferType.InternalCold, null).CompleteTransfer();
            }
            else
            {
                liveConsole.StartTransfer(ConsoleTransferType.InternalCold, ivrPersonGroup.Model.Name).CompleteTransfer();
            }

            liveConsole.Wait().Check(interviewState: InterviewState.NO_CALLS);

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(2, completeCallParams.Count);

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;
            Assert.AreEqual((int)InterviewState.NO_CALLS, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
        }

        [TestMethod]
        public void Transfer_TwoInterviewsFromDifferentSurveysIsTransferingFromLiveToIvrAgent_Successed()
        {
            voiceXmlService.Scenario = new[] { new VoiceXmlPageModel() };

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true }
                },
                Surveys = new[] {
                    new SurveyData() { Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.Ivr",
                        Interviews = new [] { new InterviewData() { Tag ="S1.I1", Call = new CallData() { Resource = "PG2.Live" } } },
                    },
                    new SurveyData() { Tag = "S2", DialMode = DialingMode.Automatic, AssignsS = "P1.Ivr",
                        Interviews = new []
                        {
                            new InterviewData() { Tag ="S2.I1", Call = new CallData() { Resource = "PG3.Empty" } },
                            new InterviewData() { Tag ="S2.I2", Call = new CallData() { Resource = "PG2.Live" } }
                        },
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1.Ivr", Name = "IVR Inters" },
                    new PersonGroupData { Tag = "PG2.Live", Name = "Live Inters" },
                    new PersonGroupData { Tag = "PG3.Empty", Name = "Empty" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1.Ivr", Type = AgentType.IvrAgent, Memberships = "PG1.Ivr" },
                    new PersonData { Tag = "P2.Live", Type = AgentType.LiveAgent, Memberships = "PG2.Live" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed }
                }
            }.Create();

            var interview1 = context.GetInterview("S1.I1");
            var interview2 = context.GetInterview("S2.I2");

            Assert.AreNotEqual(interview1.Id, interview2.Id, "interviewIds should be different");

            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");
            var ivrPersonGroup = context.GetPersonGroup("PG1.Ivr");

            dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var setCampaignParams = dialer.Behavior.Methods.SetCampaign.Init();
            dialer.Behavior.Methods.IsReloginNeededOnSurveyChange.Init(false);

            var liveConsole = livePerson.Console.Login().LoginToDialer().Start().Wait().Check(interviewTag: "S1.I1");

            liveConsole.StartTransfer(ConsoleTransferType.InternalCold, ivrPersonGroup.Model.Name).CompleteTransfer();

            liveConsole.Wait().Check(interviewTag: "S2.I2");

            liveConsole.StartTransfer(ConsoleTransferType.InternalCold, ivrPersonGroup.Model.Name).CompleteTransfer();

            liveConsole.Wait().Check(interviewState: InterviewState.NO_CALLS);

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.DIALLING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(interview1.Survey.Id, task.SurveySID);
            Assert.AreEqual(interview1.Id, task.InterviewID);

            dialer.DialerHelper.SendEventConnected(interview1.Survey.Model.CampaignId, ivrPerson.Id, context.GetCall("S1.I1").Id);

            IvrConsoleController.ExecutePeriodicalWork();

            Assert.AreEqual((int)InterviewState.INTERVIEWING, ivrConsole.Task.InterviewState);
            var ivrSetCampaigns = setCampaignParams.Where(x => x.AgentId == ivrPerson.Id).ToList();
            Assert.AreEqual(1, ivrSetCampaigns.Count);
            Assert.AreEqual(interview1.Survey.Model.CampaignId, ivrSetCampaigns[0].CampaignId);

            dialer.ProcessAllPosponedNotification();
            IvrConsoleController.ExecutePeriodicalWork();

            task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.DIALLING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(interview2.Survey.Id, task.SurveySID);
            Assert.AreEqual(interview2.Id, task.InterviewID);

            dialer.DialerHelper.SendEventConnected(interview1.Survey.Model.CampaignId, ivrPerson.Id, context.GetCall("S2.I2").Model.CallID);

            IvrConsoleController.ExecutePeriodicalWork();

            ivrSetCampaigns = setCampaignParams.Where(x => x.AgentId == ivrPerson.Id).ToList();
            Assert.AreEqual(2, ivrSetCampaigns.Count);
            Assert.AreEqual(interview2.Survey.Model.CampaignId, ivrSetCampaigns[1].CampaignId);


            task = ivrConsole.Task;
            Assert.AreEqual((int)InterviewState.INTERVIEWING, ivrConsole.Task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(interview2.Survey.Id, task.SurveySID);
            Assert.AreEqual(interview2.Id, task.InterviewID);
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForLiveAgent_TransferToLiveAgentOnStartInterview_InterviewWasTransferredAndCompletedSuccessful()
        {
            TransferFromLiveToLiveAgentInStartInterview(false);
        }

        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartInterviewForLiveAgent_TransferToLiveAgentOnStartInterviewWithEmptyGroupName_InterviewWasTransferredAndCompletedSuccessful()
        {
            TransferFromLiveToLiveAgentInStartInterview(true);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void StartInterviewForLiveAgent_TransferToLiveAgentOnOnWrapUp_InterviewWasTransferredAndCompletedSuccessful()
        {
            TransferFromLiveToLiveAgentInWrapUp(false);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void StartInterviewForLiveAgent_TransferToLiveAgentOnWrapUpWithEmptyGroupName_InterviewWasTransferredAndCompletedSuccessful()
        {
            TransferFromLiveToLiveAgentInWrapUp(true);
        }

        public void TransferFromLiveToLiveAgentInWrapUp(bool useEmptyGroupName)
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
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData() { Tag ="S1.I1", Call = new CallData() { Resource = "PG1" }},
                            new InterviewData() { Tag ="S1.I2", Call = new CallData() { Resource = "PG2" }}
                        },
                        Assigns = useEmptyGroupName ? new [] { "P2" } : new string[0]
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "Live Inters1" },
                    new PersonGroupData { Tag = "PG2", Name = "Live Inters2" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Type = AgentType.LiveAgent, Memberships = "PG1" },
                    new PersonData { Tag = "P2", Type = AgentType.LiveAgent, Memberships = "PG2" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed }
                }
            }.Create();

            var dialer = context.GetDialer("D1");
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var personGroup2 = context.GetPersonGroup("PG2");

            dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, long.Parse(args.Target),
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });

            var initiator = person1.Login().LoginToDialer().Start().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            var target = person2.Login().LoginToDialer().Start().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(2, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.CompleteCall.History.Count);

            initiator.StartTransfer(ConsoleTransferType.InternalCold, useEmptyGroupName ? null : personGroup2.Model.Name).CompleteTransfer().Wait()
                .Check(interviewState: InterviewState.NO_CALLS);

            Assert.AreEqual(2, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.CompleteCall.History.Count);

            dialer.ProcessAllPosponedNotification();

            target.WrapUp().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(2, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(2, dialer.Behavior.Methods.CompleteCall.History.Count);

            target.WrapUp().Wait().Check(interviewState: InterviewState.NO_CALLS);

            Assert.AreEqual(2, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(3, dialer.Behavior.Methods.CompleteCall.History.Count);
        }

        public void TransferFromLiveToLiveAgentInStartInterview(bool useEmptyGroupName)
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
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData() { Tag ="S1.I1", Call = new CallData() { Resource = "PG1" }}
                        },
                        Assigns = useEmptyGroupName ? new [] { "P2" } : new string[0]
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "Live Inters1" },
                    new PersonGroupData { Tag = "PG2", Name = "Live Inters2" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Type = AgentType.LiveAgent, Memberships = "PG1" },
                    new PersonData { Tag = "P2", Type = AgentType.LiveAgent, Memberships = "PG2" },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed }
                }
            }.Create();

            var dialer = context.GetDialer("D1");
            var person1 = context.GetPerson("P1");
            var person2 = context.GetPerson("P2");
            var personGroup2 = context.GetPersonGroup("PG2");

            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, person2.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            var initiatorConsole = person1.Console.Login().LoginToDialer().Start().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            initiatorConsole.StartTransfer(ConsoleTransferType.InternalCold, useEmptyGroupName ? null : personGroup2.Model.Name)
                .CompleteTransfer().Wait().Check(interviewState: InterviewState.NO_CALLS);

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            var targetConsole = person2.Console.Login().LoginToDialer().Start().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            targetConsole.WrapUp();

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(2, completeCallParams.Count);
        }


        [TestMethod, Owner(@"Firm\GrigoryK")]
        public void StartPredictiveInterviewForLiveAgent_TransferToLiveAgentThroughGroupWithoutCrossSurveyTransferring_InterviewWasTransferredAndCompletedSuccessful()
        {
            voiceXmlService.Scenario = new[] { new VoiceXmlPageModel() };

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
                        DialMode = DialingMode.Predictive,
                        Assigns = new [] { "P1", "P2" },
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData()
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "Live Inters1" },
                    new PersonGroupData { Tag = "PG2", Name = "Live Inters2" }
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Type = AgentType.LiveAgent, Memberships = "PG1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2", Type = AgentType.LiveAgent, Memberships = "PG2", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed }
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var initiatorPerson = context.GetPerson("P1");
            var targetPerson = context.GetPerson("P2");
            var personGroup2 = context.GetPersonGroup("PG2");

            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init(DialerMethodBehaviors.SendEventScreenPop(targetPerson, context.GetCall("S1.I1")));
            var completePreviewParams = dialer.Behavior.Methods.CompletePreview.Init(DialerMethodBehaviors.SendOutcomeConnected);
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            var initiatorConsole = new PredictiveConsoleController(context, initiatorPerson, survey, dialer);
            var requestedCalls = initiatorConsole.LoginAndStart();
            var interview = initiatorConsole.WaitInterview(requestedCalls.CallList[0]);

            var targetConsole = new PredictiveConsoleController(context, targetPerson, survey, dialer);
            targetConsole.Login();
            targetConsole.LoginToDialer();
            targetConsole.StartInterview();

            Assert.AreEqual("S1.I1", interview.Tag);
            Assert.AreEqual(0, completePreviewParams.Count);
            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            initiatorConsole.TransferStart(personGroup2.Model.Name);
            initiatorConsole.TransferComplete();

            Assert.AreEqual(0, completePreviewParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(false, transferSetTargetParams[0].BorrowAgentsFromAllCampaigns);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            dialer.ProcessAllPosponedNotification();
            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual((int)InterviewState.WAITING, initiatorConsole.Task.InterviewState);

            interview = targetConsole.WaitInterview();
            Assert.AreEqual("S1.I1", interview.Tag);

            Assert.AreEqual((int)InterviewState.INTERVIEWING, targetConsole.Task.InterviewState);

            Assert.AreEqual(1, completePreviewParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);

            targetConsole.FinishInterview(interview);

            Assert.AreEqual(1, completePreviewParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(2, completeCallParams.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void StartPredictiveInterviewForLiveAgent_TransferToLiveAgentThroughGroupWithCrossSurveyTransferring_InterviewWasTransferredAndCompletedSuccessful()
        {
            voiceXmlService.Scenario = new[] { new VoiceXmlPageModel() };

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
                        DialMode = DialingMode.Predictive,
                        Assigns = new [] { "P1", "P2" },
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData()
                            }
                        }
                    }
                },
                PersonGroups = new[]
                {
                    new PersonGroupData { Tag = "PG1", Name = "Live Inters1" },
                    new PersonGroupData { Tag = "PG2", Name = "Live Inters2", TransferBehavior = TransferGroupBehavior.DeliverCallsFromOtherSurvey}
                },
                Persons = new[]
                {
                    new PersonData { Tag = "P1", Type = AgentType.LiveAgent, Memberships = "PG1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2", Type = AgentType.LiveAgent, Memberships = "PG2", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[]
                {
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed}
                }
            }.Create();

            var survey = context.GetSurvey("S1");
            var dialer = context.GetDialer("D1");
            var initiatorPerson = context.GetPerson("P1");
            var targetPerson = context.GetPerson("P2");
            var targetGroup = context.GetPersonGroup("PG2");

            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init(DialerMethodBehaviors.SendEventScreenPop(targetPerson, context.GetCall("S1.I1")));

            var initiatorConsole = new PredictiveConsoleController(context, initiatorPerson, survey, dialer);
            var requestedCalls = initiatorConsole.LoginAndStart();
            var interview = initiatorConsole.WaitInterview(requestedCalls.CallList[0]);

            var targetConsole = new PredictiveConsoleController(context, targetPerson, survey, dialer);
            targetConsole.Login();
            targetConsole.LoginToDialer();
            targetConsole.StartInterview();
            initiatorConsole.TransferStart(targetGroup.Model.Name);
            initiatorConsole.FinishInterview(interview);

            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(TargetType.AgentGroup, transferSetTargetParams[0].TargetType);
            Assert.AreEqual(targetGroup.Id.ToString(), transferSetTargetParams[0].Target);
            Assert.AreEqual(true, transferSetTargetParams[0].BorrowAgentsFromAllCampaigns);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void StartPredictiveInterviewForLiveAgent_TransferToExternalNumber_CallWasTransferredAndInterviewWasCompletedSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Predictive,  AssignsS = "P1",
                        Interviews = new [] { new InterviewData() { Tag ="S1.I1", Call = new CallData() } }
                }},
                Persons = new[] { new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } },
                ExternalNumbers = new[] { new ExternalNumberData() { Tag = "EN1", Phone = "111", Assigns = "S1" } }
            }.Create();

            var dialer = context.GetDialer("D1");
            var predictive = dialer.Predictive("S1");
            var console = context.GetPerson("P1").Console;

            var transferStartParams = dialer.Behavior.Methods.TransferStart.Init();
            var transferSetTargetParams = dialer.Behavior.Methods.TransferSetTarget.Init();
            var transferCompleteParams = dialer.Behavior.Methods.TransferComplete.Init();
            var transferSetConnectionStateParams = dialer.Behavior.Methods.TransferSetConnectionState.Init();
            var completeCallParams = dialer.Behavior.Methods.CompleteCall.Init();

            console.Login("S1").LoginToDialer().Start().Do(x => predictive.Request().Connect("S1.I1", console)).Wait();

            Assert.AreEqual(0, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(0, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            console.StartTransfer(ConsoleTransferType.ExternalCold, "111");

            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            Assert.AreEqual(false, transferSetTargetParams[0].BorrowAgentsFromAllCampaigns);
            Assert.AreEqual(TargetType.External, transferSetTargetParams[0].TargetType);
            Assert.AreEqual("111", transferSetTargetParams[0].Target);

            console.CompleteTransfer();

            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            console.WrapUp();

            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(1, transferSetTargetParams.Count);
            Assert.AreEqual(1, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(1, completeCallParams.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void TransferCallFromAgentToAgentInPredicitve_NotifyOutcomeComesDuringTransferTargetCall_CallWasTransferredAndInterviewWasNotDeliveredTwice()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1,P2",
                        Interviews = new [] { new InterviewData() { Tag ="S1.I1", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");
            var predictive = dialer.Predictive("S1");
            var console = context.GetPerson("P1").Console.Login("S1").LoginToDialer().Start();
            var console2 = context.GetPerson("P2").Console.Login("S1").LoginToDialer().Start();

            dialer.Behavior.Methods.TransferSetTarget.Init((behavior, ars) =>
            {
                DialerMethodBehaviors.SendEventScreenPop(context.GetPerson("P2"), context.GetCall("S1.I1"))(behavior, ars);
                while (dialer.ProcessAllPosponedNotification() > 0) ;
                return 0;
            });

            predictive.Request().Connect("S1.I1", console);
            console.Wait().InternalColdTransfer(null);
            console2.Wait();

            context.GetCall("S1.I1").Assert.AreEqual((int)CallState.InterviewInProgress, x => x.CallState, "Wrong call state");
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveWarmTransferFromAgentToAgent_CompleteTransferWithNotLaunchedSchedulingScript_TransferWasCompletedSuccessful()
        {
            var time = DateTimeMocker.StartNew();
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I,P2.T", ScreenRecording = true,
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup", Memberships = "P2.T" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1").Predictive("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();
            initiator.Wait().StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(TargetType.AgentGroup, dialer.Behavior.Methods.TransferSetTarget.Last?.TargetType);
            Assert.AreEqual(context.GetPersonGroup("PG1").Id.ToString(), dialer.Behavior.Methods.TransferSetTarget.Last?.Target);
            Assert.AreEqual(ConnectionState.InitiatorToTarget, dialer.Behavior.Methods.TransferSetConnectionState.Last?.State);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer()
                .Start().Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            initiator.CompleteTransfer().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            time.AddTime("00:00:30");

            target.WrapUp();

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveWarmTransferFromAgentToAgent_CompleteTransferWithLaunchedSchedulingScript_TransferWasCompletedSuccessful()
        {
            var time = DateTimeMocker.StartNew();
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { SchedulingScript = AllHoursSchedule.Name, Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I,P2.T", ScreenRecording = true,
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup", Memberships = "P2.T" } },
                Scripts = new[] { ScriptData.AllHours },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1").Predictive("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();
            initiator.Wait().StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(TargetType.AgentGroup, dialer.Behavior.Methods.TransferSetTarget.Last?.TargetType);
            Assert.AreEqual(context.GetPersonGroup("PG1").Id.ToString(), dialer.Behavior.Methods.TransferSetTarget.Last?.Target);
            Assert.AreEqual(ConnectionState.InitiatorToTarget, dialer.Behavior.Methods.TransferSetConnectionState.Last?.State);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer()
                .Start().Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            initiator.CompleteTransfer().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            time.AddTime("00:00:30");

            target.WrapUp();

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveWarmTransferFromAgentToAgent_CancelTransferAfterJoin_TransferWasCanceledSuccessful()
        {
            var timeMocker = new DateTimeMocker("2019-02-28T08:00:00");

            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", ProjectId = "p123", DialMode = DialingMode.Predictive, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup", Memberships = "P2.T" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");

            var predictive = dialer.Predictive("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start()
                            .Wait().Check(interviewTag: "S1.I1")
                            .StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                            .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);
            ;
            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                            .Wait(InterviewState.INCOMING_TRANSFER)
                            .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            timeMocker.AddTime("00:01:00");

            initiator.CancelTransfer().Wait(InterviewState.INTERVIEWING)
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            Assert.AreEqual(
                BackendTools.Format(context, @"
 DialState CallType Id Type DialerId DialerTelephoneNumber RespondentTelephoneNumber          StartTime         AnswerTime InboundCallId TransferId InitialSurveyId State SurveyId CampaignId InterviewId CallId MainPersonId
 Connected Outbound  1    0 {    D1}                <NULL>               01234567890 2/28/2019 08:00:00 2/28/2019 08:00:00        <NULL>     <NULL> {           S1}     1 {    S1}        123 {    S1.I1}      1 {      P1.I}
 Connected Outbound  2    0 {    D1}                <NULL>               01234567890 2/28/2019 08:01:00 2/28/2019 08:01:00        <NULL>     <NULL> {           S1}     1 {    S1}        123 {    S1.I2}      2 {      P2.T}"),
                GetAllActiveDial());
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void PredictiveWarmTransferFromAgentToAgent_CancelTransferOnWrapUp_TransferWasCanceledSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I3", Call = new CallData() }
                    }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup", Memberships = "P2.T" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");

            var predictive = dialer.Predictive("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            initiator.Wait().StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                .Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            initiator.WrapUp().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I3");

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void PredictiveWarmTransferFromAgentToExternal_CompleteTransfer_TransferWasCompletedSuccessful()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I",
                    Interviews = new [] {
                        new InterviewData { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[] {
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");
            var predictive = dialer.Predictive("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            var targetResource = "+7 909 808 77 66";
            initiator.Wait().StartTransfer(ConsoleTransferType.ExternalWarm, targetResource).Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(TargetType.External, dialer.Behavior.Methods.TransferSetTarget.Last.TargetType);
            Assert.AreEqual(targetResource, dialer.Behavior.Methods.TransferSetTarget.Last.Target);
            Assert.AreEqual(ConnectionState.InitiatorToTarget, dialer.Behavior.Methods.TransferSetConnectionState.Last.State);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            initiator.CompleteTransfer().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void PredictiveWarmTransferFromAgentToExternal_CancelTransferAfterJoin_TransferWasCanceledSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Predictive("S1").Auto();
            var targetResource = "+7 909 808 77 66";
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start()
                            .Wait().Check(interviewTag: "S1.I1")
                            .StartTransfer(ConsoleTransferType.ExternalWarm, targetResource).Wait()
                            .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            initiator.CancelTransfer().Wait(InterviewState.INTERVIEWING)
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void PredictiveWarmTransferFromAgentToExternal_CancelTransferOnWrapUp_TransferWasCanceledSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I3", Call = new CallData() }
                    }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Predictive("S1").Auto();
            var targetResource = "+7 909 808 77 66";
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            initiator.Wait().StartTransfer(ConsoleTransferType.ExternalWarm, targetResource).Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            initiator.WrapUp().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(1, dialer.Behavior.Methods.CompleteCall.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void PredictiveColdTransferFromAgentToExternal_CompleteTransfer_TransferWasCompletedSuccessful()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData { Tag = "S1", DialMode = DialingMode.Predictive, AssignsS = "P1.I",
                    Interviews = new [] {
                        new InterviewData { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[] {
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1");
            dialer.Predictive("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            var targetResource = "+7 909 808 77 66";
            initiator.Wait().StartTransfer(ConsoleTransferType.ExternalCold, targetResource).Wait()
                .CompleteTransfer().Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.INTERVIEWING);

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(TargetType.External, dialer.Behavior.Methods.TransferSetTarget.Last?.TargetType);
            Assert.AreEqual(targetResource, dialer.Behavior.Methods.TransferSetTarget.Last?.Target);
            Assert.AreEqual(ConnectionState.TargetToRespondent, dialer.Behavior.Methods.TransferSetConnectionState.Last?.State);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void AutomaticColdTransferFromAgentToExternal_CompleteTransfer_TransferWasCompletedSuccessful()
        {
            var context = new TestData
            {
                Surveys = new[] { new SurveyData { Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I",
                    Interviews = new [] {
                        new InterviewData { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[] {
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            var targetResource = "+7 909 808 77 66";
            initiator.Wait().StartTransfer(ConsoleTransferType.ExternalCold, targetResource).Wait()
                .CompleteTransfer().Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.INTERVIEWING);

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(TargetType.External, dialer.Behavior.Methods.TransferSetTarget.Last?.TargetType);
            Assert.AreEqual(targetResource, dialer.Behavior.Methods.TransferSetTarget.Last?.Target);
            Assert.AreEqual(ConnectionState.TargetToRespondent, dialer.Behavior.Methods.TransferSetConnectionState.Last?.State);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void AutomaticWarmTransferFromAgentToAgent_CompleteTransfer_TransferWasCompletedSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            initiator.Wait().StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(ConnectionState.InitiatorToTarget, dialer.Behavior.Methods.TransferSetConnectionState.Last?.State);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                .Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            initiator.CompleteTransfer().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(TargetType.Agent, dialer.Behavior.Methods.TransferSetTarget.Last?.TargetType);
            Assert.AreEqual(context.GetPerson("P2.T").Id.ToString(), dialer.Behavior.Methods.TransferSetTarget.Last?.Target);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void AutomaticWarmTransferFromAgentToAgent_CancelTransferAfterJoin_TransferWasCanceledSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() } }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start()
                            .Wait().Check(interviewTag: "S1.I1")
                            .StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                            .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);
            ;
            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                            .Wait(InterviewState.INCOMING_TRANSFER)
                            .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            initiator.CancelTransfer().Wait(InterviewState.INTERVIEWING)
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void AutomaticWarmTransferFromAgentToAgent_CancelTransferOnWrapUp_TransferWasCanceledSuccessful()
        {
            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { SchedulingScript = AllHoursSchedule.Name, Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I3", Call = new CallData() }
                    }
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },
                Scripts = new[] { ScriptData.AllHours },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start();

            initiator.Wait().StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                .Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            initiator.WrapUp(new CompletedInterviewDetails() { Its = "31" }).Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I3");

            Assert.AreEqual(2, dialer.Behavior.Methods.CompleteCall.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            context.GetInterview("S1.I1").Assert.AreEqual(31, x => x.TransientState);
        }

        [TestMethod, Owner(@"Firm\OlegZ")]
        public void AutomaticWarmTransferFromAgentToAgent_TerminateInitiator_TransferWasCanceledSuccessful()
        {
            var timeMocker = new DateTimeMocker("2019-02-28T08:00:00");

            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { IsUseDb = true, Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Data = "responsefield=1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Data = "responsefield=2", Call = new CallData() }
                    },
                    Forms = new[]
                    {
                    new SingleFormData() { Name = "responsefield", Precodes = new[] {"1", "2"} },
                    },
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            UpdateCustomFields(context);
            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start().Wait();

            timeMocker.AddTime("00:01:00");

            initiator.StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                .Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            timeMocker.AddTime("00:02:00");

            initiator.TerminateConsole();

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(2, dialer.Behavior.Methods.CompleteCall.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.Logout.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            Assert.AreEqual(
                BackendTools.Format(context, @"
 ID SurveyId TelephoneNumber          FiredTime InterviewId  ITS AppointmentID WaitingTime ConfirmitDuration Duration BatchId PersonSID RoleID CallCenterID OpenEndReviewDuration LinkedInterviewSessionId DisplayTime PreviewTime ConnectedTime WrapTime DialTypeId CallAttemptNumber SessionId       Custom1   Custom2 Custom3 Custom4 Custom5
  1 {    S1}     01234567890 2/28/2019 08:03:00 {    S1.I1}   26        <NULL>           0            <NULL>      180  <NULL> {   P1.I}      2            1                     0                   <NULL>      <NULL>      <NULL>        <NULL>   <NULL>          0                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>
  2 {    S1}     01234567890 2/28/2019 08:03:00 {    S1.I1} 1012             0           0                 0      120  <NULL> {   P2.T}      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>"),
                BackendTools.Format(BvHistoryAdapter.GetAll()));


        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticWarmTransferFromAgentToAgent_Hangup_TransferWasCanceledSuccessful()
        {
            var timeMocker = new DateTimeMocker("2019-02-28T08:00:00");

            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Data = "responsefield=1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Data = "responsefield=2", Call = new CallData() }
                    },
                    Forms = new[]
                    {
                        new SingleFormData() { Name = "responsefield", Precodes = new[] {"1", "2"} },
                    },
                    IsUseDb = true,
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } }
            }.Create();

            UpdateCustomFields(context);
            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start().Wait();

            timeMocker.AddTime("00:01:00");

            initiator.StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                .Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            timeMocker.AddTime("00:02:00");

            initiator.Hangup().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(1, dialer.Behavior.Methods.CompleteCall.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.Logout.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            Assert.AreEqual(
                BackendTools.Format(context, @"
 ID SurveyId TelephoneNumber          FiredTime InterviewId  ITS AppointmentID WaitingTime ConfirmitDuration Duration BatchId PersonSID RoleID CallCenterID OpenEndReviewDuration LinkedInterviewSessionId DisplayTime PreviewTime ConnectedTime WrapTime DialTypeId CallAttemptNumber SessionId       Custom1   Custom2 Custom3 Custom4 Custom5
  1 {    S1}     01234567890 2/28/2019 08:03:00 {    S1.I1} 1012             0           0                 0      120  <NULL> {   P2.T}      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>"),
                BackendTools.Format(BvHistoryAdapter.GetAll()));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticWarmTransferFromAgentToAgent_Redial_TransferWasCanceledSuccessful()
        {
            var timeMocker = new DateTimeMocker("2019-02-28T08:00:00");

            var context = new TestData()
            {
                Surveys = new[] { new SurveyData() { Tag = "S1", DialMode = DialingMode.Automatic, AssignsS = "P1.I,P2.T",
                    Interviews = new [] {
                        new InterviewData() { Tag = "S1.I1", Data = "responsefield=1", Call = new CallData() },
                        new InterviewData() { Tag = "S1.I2", Data = "responsefield=2", Call = new CallData() }
                    },
                    Forms = new[]
                    {
                        new SingleFormData() { Name = "responsefield", Precodes = new[] {"1", "2"} },
                    },
                    IsUseDb = true,
                }},
                Persons = new[]{
                    new PersonData { Tag = "P1.I", TaskChoice = TaskChoiceMode.SurveyAssignment },
                    new PersonData { Tag = "P2.T", TaskChoice = TaskChoiceMode.SurveyAssignment, Memberships = "PG1" }
                },
                PersonGroups = new[] { new PersonGroupData() { Tag = "PG1", Name = "TransferGroup" } },
                Dialers = new[] { new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed } },

            }.Create();


            UpdateCustomFields(context);
            var dialer = context.GetDialer("D1").Automatic("S1").Auto();
            var initiator = context.GetPerson("P1.I").Console.Login("S1").LoginToDialer().Start().Wait();

            timeMocker.AddTime("00:01:00");

            initiator.StartTransfer(ConsoleTransferType.InternalWarm, "TransferGroup").Wait()
                .Check(interviewTag: "S1.I1", interviewState: InterviewState.OUTGOING_TRANSFER);

            var target = context.GetPerson("P2.T").Console.Login("S1").LoginToDialer().Start()
                .Wait(InterviewState.INCOMING_TRANSFER)
                .Check(interviewState: InterviewState.INCOMING_TRANSFER, interviewTag: "S1.I1");

            timeMocker.AddTime("00:02:00");

            initiator.Dial().Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            target.Wait()
                .Check(interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I2");

            Assert.AreEqual(1, dialer.Behavior.Methods.CompleteCall.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.Logout.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);

            Assert.AreEqual(
                BackendTools.Format(context, @"
 ID SurveyId TelephoneNumber          FiredTime InterviewId  ITS AppointmentID WaitingTime ConfirmitDuration Duration BatchId PersonSID RoleID CallCenterID OpenEndReviewDuration LinkedInterviewSessionId DisplayTime PreviewTime ConnectedTime WrapTime DialTypeId CallAttemptNumber SessionId       Custom1   Custom2 Custom3 Custom4 Custom5
  1 {    S1}     01234567890 2/28/2019 08:03:00 {    S1.I1} 1012             0           0                 0      120  <NULL> {   P2.T}      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>"),
                BackendTools.Format(BvHistoryAdapter.GetAll()));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticWarmTransferFromIvrToAgent_AgentNotFound_TransferWasnotCanceledBeforeTransferTimeoutAreExceed()
        {
            var timeMocker = new DateTimeMocker("2019-04-28T08:00:00");

            voiceXmlService.Scenario = new[]
            {
                new VoiceXmlPageModel(),
                VoiceXmlServiceController.TransferPage(TransferType.InternalWarm, "Live Inters")
            };

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true },
                    { SystemSettingConstants.Ivr.TransferTimeout, "0.00:01:00" }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        Tag = "S1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        }
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
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed}
                }
            }.Create();

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

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            task = ivrConsole.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.OUTGOING_TRANSFER);
            Assert.AreEqual(task.PersonSID, ivrPerson.Id);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            timeMocker.AddTime("0.00:00:30");

            Assert.AreEqual(1, sendNumberToAgentParams.Count);
            Assert.AreEqual(1, transferStartParams.Count);
            Assert.AreEqual(0, transferSetTargetParams.Count);
            Assert.AreEqual(0, transferCompleteParams.Count);
            Assert.AreEqual(1, transferSetConnectionStateParams.Count);
            Assert.AreEqual(0, completeCallParams.Count);

            task = ivrConsole.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.OUTGOING_TRANSFER);
            Assert.AreEqual(task.PersonSID, ivrPerson.Id);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticWarmTransferFromIvrToAgent_AgentNotFound_TransferWasCanceledAndInterviewCompletedByIvr()
        {
            var timeMocker = new DateTimeMocker("2019-04-28T08:00:00");

            voiceXmlService.Scenario = new[] {
                new VoiceXmlPageModel(),
                VoiceXmlServiceController.TransferPage(TransferType.InternalWarm, "Live Inters"),
                new VoiceXmlPageModel(),
                new VoiceXmlPageModel(){IsLastPage = true, Its = "33", Status = "completed"} };

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true },
                    { SystemSettingConstants.Ivr.TransferTimeout, "0.00:01:00" }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        SchedulingScript = AllHoursSchedule.Name,
                        Tag = "S1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        }
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
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed}
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();

            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");

            dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            dialer.Behavior.Methods.TransferStart.Init();
            dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, livePerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                return 0;
            });

            //Login
            IvrConsoleController.ExecutePeriodicalWork();

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.CompleteCall.History.Count);

            var ivrConsole = new IvrConsoleController(context, ivrPerson);

            var task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.INTERVIEWING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            timeMocker.AddTime("0.00:00:30");

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.CompleteCall.History.Count);

            task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.OUTGOING_TRANSFER, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            timeMocker.AddTime("0.00:01:30");

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.CompleteCall.History.Count);

            task = ivrConsole.Task;

            Assert.AreEqual((int)InterviewState.INTERVIEWING, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(context.GetInterview("S1.I1").Id, task.InterviewID);
            Assert.AreEqual(context.GetSurvey("S1").Id, task.SurveySID);

            timeMocker.AddTime("0.00:00:30");

            dialer.ProcessAllPosponedNotification();

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.CompleteCall.History.Count);

            task = ivrConsole.Task;

            Assert.AreEqual(task.InterviewState, (int)InterviewState.NO_CALLS);
            Assert.AreEqual(task.PersonSID, ivrPerson.Id);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(0, task.SurveySID);

            context.GetInterview("S1.I1").Assert.AreEqual(33, x => x.TransientState, "Wrong transient state");

            Assert.AreEqual(
                BackendTools.Format(context, @"
 ID SurveyId TelephoneNumber          FiredTime InterviewId ITS AppointmentID WaitingTime ConfirmitDuration Duration BatchId PersonSID RoleID CallCenterID OpenEndReviewDuration LinkedInterviewSessionId DisplayTime PreviewTime ConnectedTime WrapTime DialTypeId CallAttemptNumber SessionId Custom1 Custom2 Custom3 Custom4 Custom5
  1 {    S1}     01234567890 4/28/2019 08:02:30 {    S1.I1}  33             0           0                 0      150  <NULL> { P1.Ivr}      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0  <NULL>  <NULL>  <NULL>  <NULL>  <NULL>"),
                BackendTools.Format(BvHistoryAdapter.GetAll()));
        }

        [TestMethod, Owner(@"Firm\MaximL")]
        public void AutomaticWarmTransferFromIvrToAgent_AgentNotFound_TransferWasCompleteddAndInterviewCompletedByLiveAgent()
        {
            var timeMocker = new DateTimeMocker("2019-04-28T08:00:00");

            voiceXmlService.Scenario = new[] {
                new VoiceXmlPageModel(),
                VoiceXmlServiceController.TransferPage(TransferType.InternalWarm, "Live Inters"),
                new VoiceXmlPageModel(),
                new VoiceXmlPageModel(){IsLastPage = true, Its = "33", Status = "completed"} };

            var context = new TestData()
            {
                SystemSettings = new Dictionary<string, object>
                {
                    { SystemSettingConstants.Toggle.EnableIVR, true },
                    { SystemSettingConstants.Ivr.TransferTimeout, "0.00:01:00" }
                },
                Surveys = new[] {
                    new SurveyData()
                    {
                        SchedulingScript = AllHoursSchedule.Name,
                        Tag = "S1",
                        DialMode = DialingMode.Automatic,
                        Interviews = new []
                        {
                            new InterviewData()
                            {
                                Tag ="S1.I1",
                                Data = "responsefield=1",
                                Call = new CallData() { Resource = "PG1.Ivr" }
                            }
                        },
                        Forms = new[]
                        {
                            new SingleFormData() { Name = "responsefield", Precodes = new[] {"1", "2"} },
                        },
                        IsUseDb = true,
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
                    new DialerData { Tag = "D1", ReplyType = ReplyType.Postponed}
                },
                Scripts = new[] { ScriptData.AllHours }
            }.Create();
            
            UpdateCustomFields(context);
            var dialer = context.GetDialer("D1");
            var ivrPerson = context.GetPerson("P1.Ivr");
            var livePerson = context.GetPerson("P2.Live");

            dialer.Behavior.Methods.SendNumberToAgent.Init(DialerMethodBehaviors.SendOutcomeConnected);
            dialer.Behavior.Methods.TransferStart.Init();
            dialer.Behavior.Methods.TransferSetTarget.Init((controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, livePerson.Id,
                    context.GetCall("S1.I1").Id, CallOutcome.Connected));
                controller.SendNotification(() => controller.SendEventTransferState(0, args.CompanyId, args.TransferId, new TransferState()
                {
                    ConnectionState = ConnectionState.InitiatorToTarget,
                    InitiatorState = InitiatorState.Connected,
                    TargetState = TargetState.Connected,
                    TargetType = TargetType.Agent,
                    InitiatorAgentId = ivrPerson.Id
                }));
                return 0;
            });


            IvrConsoleController.ExecutePeriodicalWork();//Inr agent should login and start interview
            dialer.ProcessAllPosponedNotification();
            timeMocker.AddTime("0.00:00:30");

            dialer.ProcessAllPosponedNotification();//Ivr agent should start transfer
            timeMocker.AddTime("0.00:00:15");

            var liveConsole = livePerson.Console.Login().LoginToDialer().Start().Wait().Check(
                interviewState: InterviewState.INTERVIEWING, interviewTag: "S1.I1");

            Assert.AreEqual(1, dialer.Behavior.Methods.SendNumberToAgent.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferStart.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetTarget.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferComplete.History.Count);
            Assert.AreEqual(0, dialer.Behavior.Methods.TransferCancel.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.TransferSetConnectionState.History.Count);
            Assert.AreEqual(1, dialer.Behavior.Methods.CompleteCall.History.Count);

            var task = new IvrConsoleController(context, ivrPerson).Task;

            Assert.AreEqual((int)InterviewState.NO_CALLS, task.InterviewState);
            Assert.AreEqual(ivrPerson.Id, task.PersonSID);
            Assert.AreEqual(0, task.InterviewID);
            Assert.AreEqual(0, task.SurveySID);

            timeMocker.AddTime("0.00:00:30");

            liveConsole.WrapUp(new CompletedInterviewDetails() { Its = "33" }).Wait();

            context.GetInterview("S1.I1").Assert.AreEqual(33, x => x.TransientState, "Wrong transient state");

            Assert.AreEqual(
                BackendTools.Format(context, @"
 ID SurveyId TelephoneNumber          FiredTime InterviewId  ITS AppointmentID WaitingTime ConfirmitDuration Duration BatchId PersonSID RoleID CallCenterID OpenEndReviewDuration LinkedInterviewSessionId DisplayTime PreviewTime ConnectedTime WrapTime DialTypeId CallAttemptNumber SessionId       Custom1   Custom2 Custom3 Custom4 Custom5
  1 {    S1}     01234567890 4/28/2019 08:00:45 {    S1.I1} 1010             0           0                 0       45  <NULL> { P1.Ivr}      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>
  2 {    S1}     01234567890 4/28/2019 08:01:15 {    S1.I1}   33             0           0                 0       30  <NULL> {P2.Live}      2            1                     0                   <NULL>      <NULL>           0             0        0     <NULL>                 0         0 RespOverriden 123123123       1  <NULL>  <NULL>"),
                BackendTools.Format(BvHistoryAdapter.GetAll()));
        }

        public string GetAllActiveDial()
        {
            return BackendTools.Format(BvActiveDialAdapter.GetAll().Select(x => new
            {
                x.DialState,
                x.CallType,
                x.Id,
                x.Type,
                x.DialerId,
                x.DialerTelephoneNumber,
                x.RespondentTelephoneNumber,
                x.StartTime,
                x.AnswerTime,
                x.InboundCallId,
                x.TransferId,
                x.InitialSurveyId,
                x.State,
                x.SurveyId,
                x.CampaignId,
                x.InterviewId,
                x.CallId,
                x.MainPersonId
            }));
        }

        private void UpdateCustomFields(TestDataContext context)
        {
            // Setup custom fields configuration in BvHistoryCustomFields (upsert)
            UpsertCustomField(1, CallHistoryCustomFieldSourceTable.Respondent, "RespondentName");
            UpsertCustomField(2, CallHistoryCustomFieldSourceTable.Respondent, "TelephoneNumber");
            UpsertCustomField(3, CallHistoryCustomFieldSourceTable.Response, "responsefield");

            var survey = context.GetSurvey("S1");
            var interview = context.GetInterview("S1.I1");

            // Use ISurveyDatabaseEngine for survey database operations
            var surveyDbEngine = ServiceLocator.Resolve<ISurveyDatabaseEngine>();

            // Set respondent field value
            var overridenRespName = "RespOverriden";
            var overridenTelNumber = "123123123";
            surveyDbEngine.ExecuteNonQuery(survey.Id,
                "UPDATE <Schema>.respondent SET RespondentName = @Value WHERE respID = @RespId",
                new SqlParameter("@Value", overridenRespName),
                new SqlParameter("@RespId", interview.Id));
            surveyDbEngine.ExecuteNonQuery(survey.Id,
                "UPDATE <Schema>.respondent SET TelephoneNumber = @Value WHERE respID = @RespId",
                new SqlParameter("@Value", overridenTelNumber),
                new SqlParameter("@RespId", interview.Id));
        }
        
        private void UpsertCustomField(int id, CallHistoryCustomFieldSourceTable sourceTable, string sourceFieldName)
        {
            var existingField = BvHistoryCustomFieldsAdapter.GetByCondition("Id = @Id", new SqlParameter("@Id", id)).FirstOrDefault();

            if (existingField == null)
            {
                // Insert new record
                var customField = new BvHistoryCustomFieldsEntity
                {
                    Id = id,
                    SourceTable = (int)sourceTable,
                    SourceFieldName = sourceFieldName,
                    IsActive = true
                };
                BvHistoryCustomFieldsAdapter.Insert(customField);
            }
            else
            {
                // Update existing record
                existingField.SourceTable = (int)sourceTable;
                existingField.SourceFieldName = sourceFieldName;
                existingField.IsActive = true;
                BvHistoryCustomFieldsAdapter.Update(existingField);
            }
        }
    }
}
