using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers
{
    public static class PredictiveControllerExtensions
    {
        public static PredictiveDialerController Auto(this PredictiveDialerController controller) //where T : PredictiveDialerController
        {
            Request(controller);

            controller.Dialer.Behavior.Methods.Login.Init((behavior, args) => Login(controller, behavior, args));
            controller.Dialer.Behavior.Methods.Logout.Init((behavior, args) => Logout(controller, behavior, args));
            controller.Dialer.Behavior.Methods.GoReady.Init((behavior, args) => GoReady(controller, behavior, args));
            controller.Dialer.Behavior.Methods.GoNotReady.Init((behavior, args) => GoNotReady(controller, behavior, args));
            controller.Dialer.Behavior.Methods.CompleteCall.Init((behavior, args) => CompleteCall(controller, behavior, args));
            controller.Dialer.Behavior.Methods.SetCampaign.Init(0);
            controller.Dialer.Behavior.Methods.TransferStart.Init((behavior, args) => TransferStart(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferSetTarget.Init((behavior, args) => TransferSetTarget(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferCancel.Init((behavior, args) => TransferCancel(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferComplete.Init((behavior, args) => TransferComplete(controller, behavior, args));

            return controller;
        }

        private static int Login<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.LoginParams args) 
            where T : PredictiveDialerController
        {
            controller.Agents.Add(new PredictiveDialerController.Agent()
            {
                Id = int.Parse(args.AgentId),
                Ready = false,
                Call = null,
                CampaignId = args.CampaignId,
                Name = args.AgentName,
            });

            return DialerMethodBehaviors.SendLoggedAgentState(behavior, args);
        }

        private static int Logout<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.LogoutParams args)
            where T : PredictiveDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == args.AgentId);
            controller.Agents.Remove(agent);

            return 0;
        }

        private static int GoNotReady<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.GoNotReadyParams args)
            where T : PredictiveDialerController
        {
            controller.Agents.Single(x => x.Id == int.Parse(args.AgentId)).Ready = false;
            return 0;
        }

        private static int GoReady<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.GoReadyParams args)
            where T : PredictiveDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == int.Parse(args.AgentId));

            agent.Call = controller.Calls.FirstOrDefault();
            if (agent.Call == null)
                return 0;

            controller.Calls.Remove(agent.Call);

            if (agent.Call.diallingMode == DialingMode.Preview)
            {
                behavior.SendNotification(() => behavior.SendEventScreenPop(args.CampaignId, int.Parse(args.AgentId), agent.Call.interviewId, (int)agent.Call.callId,
                    agent.Call.diallingMode));
            }
            else
            {
                behavior.SendNotification(() => behavior.SendEventNotifyOutcome(args.CampaignId, int.Parse(args.AgentId), (int)agent.Call.callId,
                    CallOutcome.Connected));
            }

            return 0;
        }

        private static int CompleteCall<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.CompleteCallParams args)
            where T : PredictiveDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == int.Parse(args.AgentId));
            agent.Call = null;
            agent.Ready = args.MakeAgentReady;

            if (!agent.Ready)
                return 0;

            agent.Call = controller.Calls.FirstOrDefault();
            if (agent.Call == null)
                return 0;

            controller.Calls.Remove(agent.Call);

            if (agent.Call.diallingMode == DialingMode.Preview)
            {
                behavior.SendNotification( () => 
                    behavior.SendEventScreenPop(agent.CampaignId, agent.Id, agent.Call.interviewId, (int)agent.Call.callId,
                    agent.Call.diallingMode));
            }
            else
            {
                behavior.SendNotification( () => behavior.SendEventNotifyOutcome(agent.CampaignId, agent.Id, (int)agent.Call.callId,
                    CallOutcome.Connected));
            }

            return 0;
        }

        private static int TransferStart<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferStartParams args)
            where T : PredictiveDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == args.AgentId);
            var state = new TransferState
            {
                InitiatorAgentId = args.AgentId,
                InitiatorState = InitiatorState.Connected,
                ConnectionState = ConnectionState.InitiatorToTarget,
                TargetState = TargetState.NotDefined,
                TargetType = TargetType.NotDefined,
                TargetOutcome = TargetOutcome.NotDefined,
                TargetResource = null
            };
            controller.Transfers.Add(new PredictiveDialerController.Transfer()
            {
                Id = args.TransferId,
                InitiatorId = args.AgentId,
                Call = agent.Call,
                TransferState = state
            });
            behavior.SendNotification(() => behavior.SendEventTransferState(controller.Dialer.Id, args.CompanyId, args.TransferId, state));
            return 0;
        }

        private static int TransferSetTarget<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferSetTargetParams args)
            where T : PredictiveDialerController
        {
            var transfer = controller.Transfers.Single(x => x.Id == args.TransferId);

            transfer.TransferState.TargetOutcome = TargetOutcome.Connected;
            transfer.TransferState.TargetState = TargetState.Connected;
            transfer.TransferState.TargetType = args.TargetType;
            transfer.TransferState.TargetResource = args.Target;

            if (args.TargetType == TargetType.AgentGroup)
            {
                transfer.Call.diallingMode = DialingMode.Preview;
                controller.Calls.Insert(0, transfer.Call);
            }

            behavior.SendNotification(() => behavior.SendEventTransferState(controller.Dialer.Id, args.CompanyId, args.TransferId, transfer.TransferState));
            return 0;
        }

        private static int TransferComplete<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferCompleteParams args)
            where T : PredictiveDialerController
        {
            var transfer = controller.Transfers.Single(x => x.Id == args.TransferId);

            controller.Transfers.Remove(transfer);

            return 0;
        }

        private static int TransferCancel<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferCancelParams args)
            where T : PredictiveDialerController
        {
            var transfer = controller.Transfers.Single(x => x.Id == args.TransferId);

            controller.Transfers.Remove(transfer);
            controller.Calls.Remove(transfer.Call);

            return 0;
        }

        public static T Request<T>(this T controller, int count = 10, CallsSelectionAlgorithm algorithm = CallsSelectionAlgorithm.ByPersonGroup, int groupId = 0)
            where T : PredictiveDialerController
        {
            var calls = controller.Dialer.RequestCalls(controller.Survey, count, algorithm, groupId).CallList;

            controller.Calls.AddRange(calls);

            return controller;
        }

        public static T Connect<T>(this T controller, string interviewTag, ConsoleController console) where T : PredictiveDialerController
        {
            var interview = controller.Dialer.Context.GetInterview(interviewTag);

            Assert.AreEqual(controller.Survey, interview.Survey, "Interview from wrong survey");

            var call = controller.Calls.Single(x => x.interviewId == interview.Id);
            controller.Calls.Remove(call);

            controller.Dialer.SendPredicitveConnectedCall(controller.CampaignId, call, console.Person);

            return controller;
        }

        public static T Preview<T>(this T controller, string interviewTag, ConsoleController console) where T : PredictiveDialerController
        {
            var interview = controller.Dialer.Context.GetInterview(interviewTag);

            Assert.AreEqual(controller.Survey, interview.Survey, "Interview from wrong survey");

            var call = controller.Calls.Single(x => x.interviewId == interview.Id);
            controller.Calls.Remove(call);

            controller.Dialer.SendPredicitvePreviewCall(controller.CampaignId, call, console.Person);

            return controller;
        }

        public static T Busy<T>(this T controller, string interviewTag) where T : PredictiveDialerController
        {
            return Notify(controller, interviewTag, null, CallOutcome.Busy);
        }

        public static T Notify<T>(this T controller, string interviewTag, ConsoleController console, CallOutcome outcome) where T : PredictiveDialerController
        {
            var interview = controller.Dialer.Context.GetInterview(interviewTag);

            Assert.AreEqual(controller.Survey, interview.Survey, "Interview from wrong survey");

            var call = controller.Calls.Single(x => x.interviewId == interview.Id);
            controller.Calls.Remove(call);

            controller.Dialer.Behavior.SendEventNotifyOutcome(controller.CampaignId, console?.Person.Id ?? 0, (int)call.callId, outcome);

            return controller;
        }

    }
}
