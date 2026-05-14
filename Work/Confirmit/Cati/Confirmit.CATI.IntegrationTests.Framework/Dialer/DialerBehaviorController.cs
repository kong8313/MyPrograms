using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.IntegrationTests.Framework.Dialer
{
    public class DialerBehaviorController : TestDialerHelper
    {
        public DialerMethodsBehaviorController Methods;
        public TestDataContext Context { get; }
        public DialerBehaviorController(ITestDialer testDialer, TestDataContext context)
            : base(testDialer)
        {
            Methods = new DialerMethodsBehaviorController(this);
            Context = context;
        }
        
    }

    public class DialerMethodsBehaviorController
    {
        public DialerMethodBehaviorController<TestDialerHelper.StartCampaignParams, int> StartCampaign;
        public DialerMethodBehaviorController<TestDialerHelper.SetGroupsParams, int> SetGroups;
        public DialerMethodBehaviorController<TestDialerHelper.KillAgentParams, int> KillAgent;
        public DialerMethodBehaviorController<TestDialerHelper.LogoutParams, int> Logout;
        public DialerMethodBehaviorController<TestDialerHelper.CompleteCallParams, int> CompleteCall;
        public DialerMethodBehaviorController<TestDialerHelper.CompletePreviewParams, int> CompletePreview;
        public DialerMethodBehaviorController<TestDialerHelper.SendNumberToAgentParams, int> SendNumberToAgent;
        public DialerMethodBehaviorController<TestDialerHelper.RedialParams, int> Redial;
        public DialerMethodBehaviorController<TestDialerHelper.LoginParams, int> Login;
        public DialerMethodBehaviorController<TestDialerHelper.ConnectInboundCallParams, int> ConnectInboundCall;
        public DialerMethodBehaviorController<TestDialerHelper.ConnectInboundCallToAgentParams, int> ConnectInboundCallToAgent;
        public DialerMethodBehaviorController<TestDialerHelper.DropInboundCallParams, int> DropInboundCall;
        public DialerMethodBehaviorController<TestDialerHelper.SendNumbersParams, int> SendNumbers;
        public DialerMethodBehaviorController<TestDialerHelper.GoNotReadyParams, int> GoNotReady;
        public DialerMethodBehaviorController<TestDialerHelper.GoReadyParams, int> GoReady;
        public DialerMethodBehaviorController<TestDialerHelper.SetCampaignParams, int> SetCampaign;
        public DialerMethodBehaviorController<TestDialerHelper.SetNextInterviewParams, int> SetNextInterview;
        public DialerMethodBehaviorController<TestDialerHelper.TransferStartParams, int> TransferStart;
        public DialerMethodBehaviorController<TestDialerHelper.TransferCompleteParams, int> TransferComplete;
        public DialerMethodBehaviorController<TestDialerHelper.TransferCancelParams, int> TransferCancel;
        public DialerMethodBehaviorController<TestDialerHelper.TransferSetConnectionStateParams, int> TransferSetConnectionState;
        public DialerMethodBehaviorController<TestDialerHelper.TransferSetTargetParams, int> TransferSetTarget;
        public DialerMethodBehaviorController<TestDialerHelper.IvrRenderVoiceXmlParams, int> IvrRenderVoiceXml;
        public DialerMethodBehaviorController<TestDialerHelper.IsReloginNeededOnSurveyChangeParams, bool> IsReloginNeededOnSurveyChange;
        public DialerMethodBehaviorController<TestDialerHelper.EmptyParams, string> GetDialerVersion;
        public DialerMethodBehaviorController<TestDialerHelper.GetFeaturesParams, DialerFeatures> GetFeatures;
        public DialerMethodBehaviorController<TestDialerHelper.GetStateParams, DialerState> GetState;


        public DialerMethodsBehaviorController(DialerBehaviorController dbc)
        {
            StartCampaign = new DialerMethodBehaviorController<TestDialerHelper.StartCampaignParams, int>(dbc, dbc.SetBehaviorForStartCampaign);
            SetGroups = new DialerMethodBehaviorController<TestDialerHelper.SetGroupsParams, int>(dbc, dbc.SetBehaviorForSetGroups);
            KillAgent = new DialerMethodBehaviorController<TestDialerHelper.KillAgentParams, int>(dbc, dbc.SetBehaviorForKillAgent);
            Logout = new DialerMethodBehaviorController<TestDialerHelper.LogoutParams, int>(dbc, dbc.SetBehaviorForLogout);
            CompleteCall = new DialerMethodBehaviorController<TestDialerHelper.CompleteCallParams, int>(dbc, dbc.SetBehaviorForCompleteCall);
            CompletePreview = new DialerMethodBehaviorController<TestDialerHelper.CompletePreviewParams, int>(dbc, dbc.SetBehaviorForCompletePreview);
            SendNumberToAgent = new DialerMethodBehaviorController<TestDialerHelper.SendNumberToAgentParams, int>(dbc, dbc.SetBehaviorForSendNumberToAgent);
            Redial = new DialerMethodBehaviorController<TestDialerHelper.RedialParams, int>(dbc, dbc.SetBehaviorForRedial);
            Login = new DialerMethodBehaviorController<TestDialerHelper.LoginParams, int>(dbc, dbc.SetBehaviorForLogin);
            ConnectInboundCall = new DialerMethodBehaviorController<TestDialerHelper.ConnectInboundCallParams, int>(dbc, dbc.SetBehaviorForConnectInboundCall);
            ConnectInboundCallToAgent = new DialerMethodBehaviorController<TestDialerHelper.ConnectInboundCallToAgentParams, int>(dbc, dbc.SetBehaviorForConnectInboundCallToAgent);
            DropInboundCall = new DialerMethodBehaviorController<TestDialerHelper.DropInboundCallParams, int>(dbc, dbc.SetBehaviorForDropInboundCall);
            SendNumbers = new DialerMethodBehaviorController<TestDialerHelper.SendNumbersParams, int>(dbc, dbc.SetBehaviorForSendNumbers);
            GoNotReady = new DialerMethodBehaviorController<TestDialerHelper.GoNotReadyParams, int>(dbc, dbc.SetBehaviorForGoNotReady);
            GoReady = new DialerMethodBehaviorController<TestDialerHelper.GoReadyParams, int>(dbc, dbc.SetBehaviorForGoReady);
            SetCampaign = new DialerMethodBehaviorController<TestDialerHelper.SetCampaignParams, int>(dbc, dbc.SetBehaviorForSetCampaign);
            SetNextInterview = new DialerMethodBehaviorController<TestDialerHelper.SetNextInterviewParams, int>(dbc, dbc.SetBehaviorForNextInterview);
            IvrRenderVoiceXml = new DialerMethodBehaviorController<TestDialerHelper.IvrRenderVoiceXmlParams, int>(dbc, dbc.SetBehaviorForIvrRenderVoiceXml);
            IsReloginNeededOnSurveyChange = new DialerMethodBehaviorController<TestDialerHelper.IsReloginNeededOnSurveyChangeParams, bool>(dbc, dbc.SetBehaviorForIsReloginNeededOnSurveyChange);

            TransferStart = new DialerMethodBehaviorController<TestDialerHelper.TransferStartParams, int>(dbc, dbc.SetBehaviorForTransferStart);
            TransferComplete = new DialerMethodBehaviorController<TestDialerHelper.TransferCompleteParams, int>(dbc, dbc.SetBehaviorForTransferComplete);
            TransferCancel = new DialerMethodBehaviorController<TestDialerHelper.TransferCancelParams, int>(dbc, dbc.SetBehaviorForTransferCancel);
            TransferSetConnectionState = new DialerMethodBehaviorController<TestDialerHelper.TransferSetConnectionStateParams, int>(dbc, dbc.SetBehaviorForTransferSetConnectionState);
            TransferSetTarget = new DialerMethodBehaviorController<TestDialerHelper.TransferSetTargetParams, int>(dbc, dbc.SetBehaviorForTransferSetTarget);
            GetDialerVersion = new DialerMethodBehaviorController<TestDialerHelper.EmptyParams, string>(dbc, dbc.SetBehaviorForGetDialerVersion);
            GetFeatures = new DialerMethodBehaviorController<TestDialerHelper.GetFeaturesParams, DialerFeatures>(dbc, dbc.SetBehaviorForGetFeatures);
            GetState = new DialerMethodBehaviorController<TestDialerHelper.GetStateParams, DialerState>(dbc, dbc.SetBehaviorForGetState);
        }
    }

    public class DialerMethodBehaviorController<TParams, TResult>
    {
        private readonly DialerBehaviorController _controller;
        private readonly Action<Func<TParams, TResult>> _setBehavior;
        public List<TParams> History { get; } = new List<TParams>();

        public DialerMethodBehaviorController(DialerBehaviorController controller, Action<Func<TParams, TResult>> setBehavior)
        {
            _controller = controller;
            _setBehavior = setBehavior;
        }

        public List<TParams> Init(Func<DialerBehaviorController, TParams, TResult> behavior)
        {
            var result = new List<TParams>();

            _setBehavior((args) =>
            {
                result.Add(args);
                History.Add(args);
                return behavior(_controller, args);
            });

            return result;
        }

        public List<TParams> Init()
        {
            return Init((controller, args) => default(TResult));
        }

        public List<TParams> Init(TResult result)
        {
            return Init((controller, args) => result);
        }

        public TParams Last => History.LastOrDefault();
    }

    public class DialerMethodBehaviors
    {
        public static Func<DialerBehaviorController, TestDialerHelper.ConnectInboundCallParams, int> SendOutcomeConnected(
            Func<TestDialerHelper.ConnectInboundCallParams, int> behavior)
        {
            return (controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, behavior(args),
                    (int)args.CallInfo.callId, CallOutcome.Connected));
                return 0;
            };
        }

        public static int SendOutcomeConnected(DialerBehaviorController controller, TestDialerHelper.SendNumberToAgentParams args)
        {
            controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, args.AgentId,
                (int)args.CallId, CallOutcome.Connected));
            return 0;
        }

        public static int SendOutcomeNotConnected(DialerBehaviorController controller, TestDialerHelper.SendNumberToAgentParams args, CallOutcome callOutcome)
        {
            controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, args.AgentId,
                (int)args.CallId, callOutcome));
            return 0;
        }

        public static int SendOutcomeConnected(DialerBehaviorController controller, TestDialerHelper.ConnectInboundCallToAgentParams args)
        {
            controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, args.CallInfo.agentId,
                (int)args.CallInfo.callId, CallOutcome.Connected));
            return 0;
        }

        public static int SendOutcomeConnected(DialerBehaviorController controller, ConsoleController console, CallRef call)
        {
            controller.SendNotification(() => controller.SendEventNotifyOutcome(call.Interview.Survey.Model.CampaignId, console.Person.Id,
                call.Model.CallID, CallOutcome.Connected));
            return 0;
        }

        public static int SendOutcomeConnected(DialerBehaviorController controller, TestDialerHelper.CompletePreviewParams args)
        {
            controller.SendNotification(() => controller.SendEventNotifyOutcome(args.CampaignId, (int)args.AgentId,
                args.CallId, CallOutcome.Connected));
            return 0;
        }

        public static int SendLoggedAgentState(DialerBehaviorController controller, TestDialerHelper.LoginParams args )
        {
            controller.SendEventNotifyAgentState(
                args.CampaignId,
                int.Parse(args.AgentId),
                "1");
            return 0;
        }

        public static int SendAgentState(DialerBehaviorController controller, ConsoleController console, AgentStateMsgs state)
        {
            controller.SendEventNotifyAgentState(
                console.Survey.Model.CampaignId,
                console.Person.Id,
                ((int)state).ToString());
            return 0;
        }

        public static Func<DialerBehaviorController, TestDialerHelper.TransferSetTargetParams, int> SendEventScreenPop(PersonController targetPerson, CallRef call)
        {
            return (controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventScreenPop(args.CampaignId, targetPerson.Id,
                    call.Interview.Id, call.Model.CallID, DialingMode.Preview));
                return 0;
            };
        }

        public static int SendIvrSubmit(DialerBehaviorController controller, TestDialerHelper.IvrRenderVoiceXmlParams args)
        {
            var variables = IvrConsoleService.GetVariablesFromVoiceXml(args.VoiceXml);

            var survey = ServiceLocator.Resolve<ISurveyRepository>().GetByCampaignId(args.CampaignId);

            controller.SendNotification(() => controller.SendEventIvrSubmit(args.CompanyId, survey.CampaignId, args.AgentId, variables));

            return 0;
        }

        public static Func<DialerBehaviorController, TestDialerHelper.ConnectInboundCallParams, int> SendScreenPop(PersonController targetPerson)
        {
            return (controller, args) =>
            {
                controller.SendNotification(() => controller.SendEventScreenPop(args.CampaignId, targetPerson.Id,
                    args.CallInfo.interviewId, (int)args.CallInfo.callId, DialingMode.Preview));
                return 0;
            };
        }
        
        public static DialerState SendDialerStateAvailable(DialerBehaviorController controller, TestDialerHelper.GetStateParams args)
        {
            controller.SendNotification(() => controller.SendEventNotifyDialerState(DialerState.Available));

            return DialerState.Available;
        }
        
        public static DialerState SendDialerStateNotificationUnavailable(DialerBehaviorController controller, TestDialerHelper.GetStateParams args)
        {
            controller.SendNotification(() => controller.SendEventNotifyDialerState(DialerState.Unavailable));

            return DialerState.Available;
        }
    }
}