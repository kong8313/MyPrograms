using System.Linq;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers
{
    public static class AutomaticControllerExtensions
    {
        public static AutomaticDialerController Auto(this AutomaticDialerController controller) //where T : AutomaticDialerController
        {
            controller.Dialer.Behavior.Methods.Login.Init((behavior, args) => Login(controller, behavior, args));
            controller.Dialer.Behavior.Methods.Logout.Init((behavior, args) => Logout(controller, behavior, args));
            controller.Dialer.Behavior.Methods.SendNumberToAgent.Init((behavior, args) => SendNumberToAgent(controller, behavior, args));
            controller.Dialer.Behavior.Methods.Redial.Init((behavior, args) => Redial(controller, behavior, args));
            controller.Dialer.Behavior.Methods.ConnectInboundCallToAgent.Init((behavior, args) => ConnectInboundCallToAgent(controller, behavior, args));
            controller.Dialer.Behavior.Methods.CompleteCall.Init((behavior, args) => CompleteCall(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferStart.Init((behavior, args) => TransferStart(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferSetTarget.Init((behavior, args) => TransferSetTarget(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferCancel.Init((behavior, args) => TransferCancel(controller, behavior, args));
            controller.Dialer.Behavior.Methods.TransferComplete.Init((behavior, args) => TransferComplete(controller, behavior, args));

            return controller;
        }

        private static int Login<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.LoginParams args)
            where T : AutomaticDialerController
        {
            controller.Agents.Add(new AutomaticDialerController.Agent()
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
            where T : AutomaticDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == args.AgentId);
            controller.Agents.Remove(agent);

            return 0;
        }

        private static int SendNumberToAgent<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.SendNumberToAgentParams args) where T : AutomaticDialerController
        {
            var call = new CallInfo
            {
                agentId = (int)args.AgentId,
                diallingMode = args.DiallingMode,
                interviewId = args.InterviewId,
                callId = args.CallId,
                phoneNumber = args.PhoneNumber,
                isRecording = args.IsRecording,
                callerId = args.CallerId
            };
            UpdateActiveCall(controller, behavior, call, args.AgentId, args.CampaignId);
            return 0;
        }

        private static int Redial<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.RedialParams args) where T : AutomaticDialerController
        {
            var call = new CallInfo
            {
                agentId = (int)args.AgentId,
                interviewId = args.InterviewId,
                callId = args.CallId,
                phoneNumber = args.PhoneNumber,
                isRecording = args.IsRecording,
                callerId = args.CallerId

            };

            UpdateActiveCall(controller, behavior, call, args.AgentId, args.CampaignId);
            return 0;
        }

        private static int ConnectInboundCallToAgent<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.ConnectInboundCallToAgentParams args) where T : AutomaticDialerController
        {
            UpdateActiveCall(controller, behavior, args.CallInfo, args.CallInfo.agentId, args.CampaignId);
            return 0;
        }

        private static void UpdateActiveCall<T>(T controller, DialerBehaviorController behavior,
            CallInfo call, long agentId, long campaignId) where T : AutomaticDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == agentId);
            // ActiveCall = call;
            agent.Call = call;
            behavior.SendNotification(() => behavior.SendEventNotifyOutcome(campaignId, agentId,
                (int) call.callId, CallOutcome.Connected));
        }

        private static int CompleteCall<T>(T controller, DialerBehaviorController behavior, TestDialerHelper.CompleteCallParams args) where T : AutomaticDialerController
        {
            var agent = controller.Agents.Single(x => x.Id == int.Parse(args.AgentId));
            if (agent.Call == null) return 0;
            agent.Call = null;
            return 0;
        }

        private static int TransferStart<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferStartParams args)
            where T : AutomaticDialerController
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
            controller.Transfers.Add(new AutomaticDialerController.Transfer()
            {
                Id = args.TransferId,
                InitiatorId = args.AgentId,
                CampaignId = args.CampaignId,
                Call = agent.Call,
                TransferState = state
            });
            
            behavior.SendNotification(() => behavior.SendEventTransferState(controller.Dialer.Id, args.CompanyId, args.TransferId, state));
            return 0;
        }

        private static int TransferSetTarget<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferSetTargetParams args)
            where T : AutomaticDialerController
        {
            var transfer = controller.Transfers.Single(x => x.Id == args.TransferId);

            transfer.TransferState.TargetOutcome = TargetOutcome.Connected;
            transfer.TransferState.TargetState = TargetState.Connected;
            transfer.TransferState.TargetType = args.TargetType;
            transfer.TransferState.TargetResource = args.Target;

            if (args.TargetType == TargetType.Agent)
            {
                var agent = controller.Agents.FirstOrDefault(x => x.Id == int.Parse(args.Target));
                Assert.IsNotNull(agent, $"There is no target agent with id = {args.Target}");
                behavior.SendNotification(() => behavior.SendEventNotifyOutcome(
                    transfer.CampaignId,
                    int.Parse(args.Target), 
                    (int) transfer.Call.callId, 
                    CallOutcome.Connected));
            }

            behavior.SendNotification(() => behavior.SendEventTransferState(controller.Dialer.Id, args.CompanyId, args.TransferId, transfer.TransferState));
            return 0;
        }

        private static int TransferComplete<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferCompleteParams args)
            where T : AutomaticDialerController
        {
            var transfer = controller.Transfers.Single(x => x.Id == args.TransferId);

            controller.Transfers.Remove(transfer);

            return 0;
        }

        private static int TransferCancel<T>(T controller, DialerBehaviorController behavior,
            TestDialerHelper.TransferCancelParams args)
            where T : AutomaticDialerController
        {
            var transfer = controller.Transfers.Single(x => x.Id == args.TransferId);

            controller.Transfers.Remove(transfer);

            return 0;
        }
    }
}
