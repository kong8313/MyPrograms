using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony;
using ConfirmitDialerInterface;
using DialerCommon;

namespace Confirmit.CATI.IntegrationTests.Framework.Dialer
{
    /// <summary>
    /// Wrapper for <see cref="TestDialer"/> class. Contains methods to simplify adding expectations to test dialer.
    /// Also contains methods to emulate sending events notification to backend from dialer.
    /// </summary>
    public class TestDialerHelper
    {
        private int _fakeDialerId = 1;
        private const string FakeTenantId = "1";

        /// <summary>
        /// Gets or sets the test dialer to be used by helper class.
        /// </summary>
        public ITestDialer FakeDialer { get; private set; }

        private readonly IDialerEventsHandler _dialerEventsHandler;

        public TestDialerHelper(ITestDialer testDialer)
        {
            FakeDialer = testDialer;
            NotificationReplyType = ReplyType.Async;
            _dialerEventsHandler = ServiceLocator.Resolve<IDialerEventsHandler>();
        }

        public void SetFakeDialerId(int id)
        {
            _fakeDialerId = id;
        }

        public void AddRequestLogin()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.Login));
        }

        public void AddRequestGoReady()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.GoReady));
        }

        public void AddRequestGoNotReady(Action action)
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.GoNotReady), action);
        }

        public void AddRequestSendNumber(Action action = null)
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.SendNumberToAgent), action);
        }

        public void AddRequestSendNumbers()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.SendNumbers));
        }

        public void AddRequestCompleteCall()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.CompleteCall));
        }

        public void AddRequestStartCampaign()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.StartCampaign));
        }

        public void AddRequestLogout(Action action = null)
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.Logout), action);
        }

        public void AddRequestSetGroups()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.SetGroups));
        }

        public void SendEventScreenPop(long campaignId, int agentId, int interviewId, int callId, DialingMode mode)
        {
            _dialerEventsHandler.OnDialerScreenPop(
                _fakeDialerId,
                FakeTenantId,
                campaignId,
                agentId,
                interviewId.ToString(CultureInfo.InvariantCulture),
                callId,
                mode);
        }

        public void AddRequestCompletePreview()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.CompletePreview));
        }

        public void AddRequestHangup()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.Hangup));
        }

        public ReplyType NotificationReplyType { get; set; }

        private List<Action> _postponedNotification = new List<Action>();

        public int ProcessAllPosponedNotification()
        {
            var items = _postponedNotification;
            _postponedNotification = new List<Action>();

            items.ForEach(x => x());

            return items.Count;
        }

        public void SendNotification(Action action)
        {
            switch (NotificationReplyType)
            {
                case ReplyType.Sync:
                    action();
                    break;
                case ReplyType.Async:
                    Task.Factory.StartNew(action);
                    break;
                case ReplyType.Postponed:
                    _postponedNotification.Add(action);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void SendEventNotifyAgentState(long campaignId, long agentId, string agentStateMsg)
        {
            _dialerEventsHandler.OnDialerNotifyAgentState(
                _fakeDialerId,
                FakeTenantId,
                campaignId,
                agentId,
                agentStateMsg);
        }

        public void SendEventConnected(long campaignId, long agentId, int callId, string callerId = null, int ringTimeSeconds = 0, Dictionary<string, string> callOutcomeMetadata = null)
        {
            _dialerEventsHandler.OnDialerNotifyOutcome(
                _fakeDialerId,
                FakeTenantId,
                campaignId,
                agentId,
                "",
                callId,
                (long)CallOutcome.Connected,
                callerId,
                TimeSpan.FromSeconds(ringTimeSeconds),
                callOutcomeMetadata);
        }

        public void SendEventIvrSubmit(int companyId, long surveyId, int agentId, KeyValuePair<string, string>[] variables)
        {
            _dialerEventsHandler.OnDialerIvrSubmit(
                _fakeDialerId,
                companyId.ToString(),
                surveyId,
                agentId,
                variables);
        }

        public void SendEventNotifyOutcome(long campaignId, long agentId, int callId, CallOutcome callOutcome, string callerId = null, int ringTimeSeconds = 0, Dictionary<string, string> callOutcomeMetadata = null)
        {
            _dialerEventsHandler.OnDialerNotifyOutcome(
                _fakeDialerId,
                FakeTenantId,
                campaignId,
                agentId,
                "",
                callId,
                (long)callOutcome,
                callerId,
                TimeSpan.FromSeconds(ringTimeSeconds),
                callOutcomeMetadata);
        }

        public void SendEventTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {
            _dialerEventsHandler.OnTransferState(dialerId, companyId, transferId, transferState);
        }

        public void SendEventRequestCalls(long campaignId, int groupId, int callCount, CallsSelectionAlgorithm algorithm = CallsSelectionAlgorithm.ByPersonGroup)
        {
            _dialerEventsHandler.OnDialerRequestCalls(
                _fakeDialerId,
                null,
                FakeTenantId,
                campaignId,
                groupId,
                algorithm,
                callCount);
        }

        internal void SendEventNotifyInboundCall(string inboundCallNumber, string callerPhoneNumber, string inboundCallId)
        {
            _dialerEventsHandler.OnDialerNotifyInboundCall(
                _fakeDialerId,
                IntegrationTestingFramework.CompanyId,
                inboundCallNumber,
                callerPhoneNumber,
                inboundCallId
                );
        }

        internal void SendEventNotifyDropInboundCall(string inboundCallId)
        {
            _dialerEventsHandler.OnDialerNotifyInboundCallDroppedByRespondent(
                _fakeDialerId,
                IntegrationTestingFramework.CompanyId,
                inboundCallId
            );
        }

        internal void SendEventNotifyDropCallByRespondent(long campainnId, long agentId, long callId)
        {
            _dialerEventsHandler.OnDialerNotifyCallDroppedByRespondent(
                _fakeDialerId,
                IntegrationTestingFramework.CompanyId.ToString(),
                campainnId,
                agentId,
                callId
            );
        }

        internal void SendEventNotifyDialerState(DialerState state)
        {
            ServiceLocator.Resolve<IDialerCollection>().GetDialerById(_fakeDialerId).OnDialerState(state);
        }

        public void CheckAllExpectedRequestsAreSentToDialer()
        {
            FakeDialer.CheckNoExpectedRequests();
        }

        public class StartCampaignParams
        {
            public string TenantId;
            public int[] DialerIds;
            public long CampaignId;
            public string CampaignName;
            public DialingMode DialingMode;
            public string CampaignType;
            public bool RecordWholeInterview;
            public string SurveyParametersXml;
        }

        public void SetBehaviorForStartCampaign(Func<StartCampaignParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.StartCampaign),
                args =>
                {
                    var parameters = new StartCampaignParams()
                    {
                        TenantId = (string)args[0],
                        DialerIds = (int[])args[1],
                        CampaignId = (long)args[2],
                        CampaignName = (string)args[3],
                        DialingMode = (DialingMode)args[4],
                        CampaignType = (string)args[5],
                        RecordWholeInterview = (bool)args[6],
                        SurveyParametersXml = (string)args[7]
                    };

                    return behavior(parameters);
                });
        }

        public class SetGroupsParams
        {
            public string TenantId;
            public long CampaignId;
            public string AgentId;
            public int[] AgentGroups;
        }

        public void SetBehaviorForSetGroups(Func<SetGroupsParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.SetGroups),
                args =>
                {
                    var parameters = new SetGroupsParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (string)args[2],
                        AgentGroups = (int[])args[3]
                    };

                    return behavior(parameters);
                });
        }

        public class SendNumbersParams
        {
            public string RequestId;
            public string TenantId;
            public long CampaignId;
            public DialingMode CampaignDiallingMode;
            public List<CallInfo> CallList;
            public int CallAgingTimeout;
            public bool IsRecording;
        }


        public void SetBehaviorForSendNumbers(Func<SendNumbersParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.SendNumbers),
                args =>
                {
                    var parameters = new SendNumbersParams()
                    {
                        RequestId = (string)args[0],
                        TenantId = (string)args[1],
                        CampaignId = (long)args[2],
                        CampaignDiallingMode = (DialingMode)args[3],
                        CallList = (List<CallInfo>)args[4],
                        CallAgingTimeout = (int)args[5],
                        IsRecording = (bool)args[6]
                    };

                    return behavior(parameters);
                });
        }

        public class IsReloginNeededOnSurveyChangeParams { }

        public void SetBehaviorForIsReloginNeededOnSurveyChange(Func<IsReloginNeededOnSurveyChangeParams, bool> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.IsReloginNeededOnSurveyChange),
                args =>
                {
                    var parameters = new IsReloginNeededOnSurveyChangeParams();
                    return behavior(parameters);
                });
        }

        public class CompleteCallParams
        {
            public string TenantId;
            public long CampaignId;
            public string AgentId;
            public InterviewStatus InterviewStatus;
            public bool MakeAgentReady;
            public string BreakName;
        }

        public void SetBehaviorForCompleteCall(Func<CompleteCallParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.CompleteCall),
                args =>
                {
                    var parameters = new CompleteCallParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (string)args[2],
                        InterviewStatus = (InterviewStatus)args[3],
                        MakeAgentReady = (bool)args[4],
                        BreakName = (string)args[5]
                    };

                    return behavior(parameters);
                });
        }

        public class SetNextInterviewParams
        {
            public string TenantId;
            public long CampaignId;
            public string AgentId;
            public InterviewStatus InterviewStatus;
            public long NextCampaingId;
            public int NextInterviewId;
            public long NextCallId;
        }

        public void SetBehaviorForNextInterview(Func<SetNextInterviewParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.SetNextInterview),
                args =>
                {
                    var parameters = new SetNextInterviewParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (string)args[2],
                        InterviewStatus = (InterviewStatus)args[3],
                        NextCampaingId = (long)args[4],
                        NextInterviewId = (int)args[5],
                        NextCallId = (long)args[6]
                    };

                    return behavior(parameters);
                });
        }

        public class IsPersonModeSupportedParams
        {
            public int mode;
        }

        public void SetBehaviorForIsPersonModeSupported(Func<IsPersonModeSupportedParams, bool> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.IsPersonModeSupported),
                args =>
                {
                    var parameters = new IsPersonModeSupportedParams()
                    {
                        mode = (int)args[0]
                    };

                    return behavior(parameters);
                });
        }

        public class TransferStartParams
        {
            public int CompanyId;
            public long CampaignId;
            public string TransferId;
            public int AgentId;
            public TransferType Type;
        }

        public void SetBehaviorForTransferStart(Func<TransferStartParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.TransferStart),
                args =>
                {
                    var parameters = new TransferStartParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        TransferId = (string)args[2],
                        AgentId = (int)args[3],
                        Type = (TransferType)args[4]
                    };

                    return behavior(parameters);
                });
        }

        public class IvrRenderVoiceXmlParams
        {
            public int CompanyId;
            public long CampaignId;
            public int AgentId;
            public string VoiceXml;
        }

        public void SetBehaviorForIvrRenderVoiceXml(Func<IvrRenderVoiceXmlParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.IvrRenderVoiceXml),
                args =>
                {
                    var parameters = new IvrRenderVoiceXmlParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (int)args[2],
                        VoiceXml = (string)args[3]
                    };

                    return behavior(parameters);
                });
        }

        public class TransferCompleteParams
        {
            public int CompanyId;
            public long CampaignId;
            public string TransferId;
        }

        public void SetBehaviorForTransferComplete(Func<TransferCompleteParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.TransferComplete),
                args =>
                {
                    var parameters = new TransferCompleteParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        TransferId = (string)args[2]
                    };

                    return behavior(parameters);
                });
        }

        public class TransferCancelParams
        {
            public int CompanyId;
            public long CampaignId;
            public string TransferId;
        }

        public void SetBehaviorForTransferCancel(Func<TransferCancelParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.TransferCancel),
                args =>
                {
                    var parameters = new TransferCancelParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        TransferId = (string)args[2]
                    };

                    return behavior(parameters);
                });
        }

        public class TransferSetTargetParams
        {
            public int CompanyId;
            public long CampaignId;
            public string TransferId;
            public TargetType TargetType;
            public string Target;
            public bool BorrowAgentsFromAllCampaigns;
        }

        public void SetBehaviorForTransferSetTarget(Func<TransferSetTargetParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.TransferSetTarget),
                args =>
                {
                    var parameters = new TransferSetTargetParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        TransferId = (string)args[2],
                        TargetType = (TargetType)args[3],
                        Target = (string)args[4],
                        BorrowAgentsFromAllCampaigns = (bool)args[5]
                    };

                    return behavior(parameters);
                });
        }

        public class TransferSetConnectionStateParams
        {
            public int CompanyId;
            public long CampaignId;
            public string TransferId;
            public ConnectionState State;
        }

        public void SetBehaviorForTransferSetConnectionState(Func<TransferSetConnectionStateParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.TransferSetConnectionState),
                args =>
                {
                    var parameters = new TransferSetConnectionStateParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        TransferId = (string)args[2],
                        State = (ConnectionState)args[3]
                    };

                    return behavior(parameters);
                });
        }

        public class ConnectInboundCallParams
        {
            public int CompanyId;
            public long CampaignId;
            public string InboundCallId;
            public CallInfo CallInfo;
            public long[] CampaignIdsToBorrowAgentsFrom;
            public AudioMessageDescriptor AudioMessageDescriptor;
        }

        public void SetBehaviorForConnectInboundCall(Func<ConnectInboundCallParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.ConnectInboundCall),
                args =>
                {
                    var parameters = new ConnectInboundCallParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        InboundCallId = (string)args[2],
                        CallInfo = (CallInfo)args[3],
                        CampaignIdsToBorrowAgentsFrom = (long[])args[4],
                        AudioMessageDescriptor = (AudioMessageDescriptor)args[5]
                    };

                    return behavior(parameters);
                });
        }

        public class ConnectInboundCallToAgentParams
        {
            public int CompanyId;
            public long CampaignId;
            public string InboundCallId;
            public CallInfo CallInfo;
            public AudioMessageDescriptor AudioMessageDescriptor;
        }

        public void SetBehaviorForConnectInboundCallToAgent(Func<ConnectInboundCallToAgentParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.ConnectInboundCallToAgent),
                args =>
                {
                    var parameters = new ConnectInboundCallToAgentParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        InboundCallId = (string)args[2],
                        CallInfo = (CallInfo)args[3],
                        AudioMessageDescriptor = (AudioMessageDescriptor)args[4]
                    };

                    return behavior(parameters);
                });
        }

        public class DropInboundCallParams
        {
            public int CompanyId;
            public string InboundCallId;
            public AudioMessageDescriptor AudioMessageDescriptor;
        }

        public void SetBehaviorForDropInboundCall(Func<DropInboundCallParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.DropInboundCall),
                args =>
                {
                    var parameters = new DropInboundCallParams()
                    {
                        CompanyId = (int)args[0],
                        InboundCallId = (string)args[1],
                        AudioMessageDescriptor = (AudioMessageDescriptor)args[2]
                    };

                    return behavior(parameters);
                });
        }

        public class LoginParams
        {
            public string TenantId;
            public long CampaignId;
            public string AgentId;
            public string AgentName;
            public AgentType AgentType;
            public string AgentExtension;
            public string UserId;
            public bool IsPredictive;
            public bool IsLocal;
            public IEnumerable<KeyValuePair<string, string>> AgentAttributes;
        }

        public void SetBehaviorForLogin(Func<LoginParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.Login),
                args =>
                {
                    var parameters = new LoginParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (string)args[2],
                        AgentName = (string)args[3],
                        AgentType = (AgentType)args[4],
                        AgentExtension = (string)args[5],
                        UserId = (string)args[6],
                        IsPredictive = (bool)args[7],
                        IsLocal = (bool)args[8],
                        AgentAttributes = (IEnumerable<KeyValuePair<string, string>>)args[9],
                    };

                    return behavior(parameters);
                });
        }

        public class LogoutParams
        {
            public string TenantId;
            public long CampaignId;
            public bool IsPredicitve;
            public long AgentId;
        }

        public void SetBehaviorForLogout(Func<LogoutParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.Logout),
                args =>
                {
                    var parameters = new LogoutParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        IsPredicitve = (bool)args[2],
                        AgentId = long.Parse((string)args[3]),
                    };

                    return behavior(parameters);
                });
        }

        public class ConfigureInboundDdiNumbersParams
        {
            public int TenantId;
            public InboundDdiNumber[] InboundDDINumbers;
        }

        public void SetBehaviorForConfigureInboundDdiNumbers(Func<ConfigureInboundDdiNumbersParams, DialerErrorCode[]> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.ConfigureInboundDdiNumbers),
                args =>
                {
                    var parameters = new ConfigureInboundDdiNumbersParams()
                    {
                        TenantId = (int)args[0],
                        InboundDDINumbers = (InboundDdiNumber[])args[1]
                    };

                    return behavior(parameters);
                });
        }

        public class KillAgentParams
        {
            public string TenantId;
            public long CampaignId;
            public long AgentId;
        }

        public void SetBehaviorForKillAgent(Func<KillAgentParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.KillAgent),
                args =>
                {
                    var parameters = new KillAgentParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = long.Parse((string)args[2]),
                    };

                    return behavior(parameters);
                });
        }

        public class GoReadyParams
        {
            public string TenantId;
            public long CampaignId;
            public string AgentId;
        }

        public void SetBehaviorForGoReady(Func<GoReadyParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GoReady),
                args =>
                {
                    var parameters = new GoReadyParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (string)args[2],
                    };

                    return behavior(parameters);
                });
        }

        public class GoNotReadyParams
        {
            public string TenantId;
            public long CampaignId;
            public string AgentId;
            public string BreakName;
        }

        public void SetBehaviorForGoNotReady(Func<GoNotReadyParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GoNotReady),
                args =>
                {
                    var parameters = new GoNotReadyParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (string)args[2],
                        BreakName = (string)args[3]
                    };

                    return behavior(parameters);
                });
        }

        public class SetCampaignParams
        {
            public int CompanyId;
            public long CampaignId;
            public long AgentId;
        }

        public void SetBehaviorForSetCampaign(Func<SetCampaignParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.SetCampaign),
                args =>
                {
                    var parameters = new SetCampaignParams()
                    {
                        CompanyId = (int)args[0],
                        CampaignId = (long)args[1],
                        AgentId = (int)args[2],
                    };

                    return behavior(parameters);
                });
        }


        public class SendNumberToAgentParams
        {
            public string TenantId;
            public long CampaignId;
            public long AgentId;
            public DialingMode DiallingMode;
            public int InterviewId;
            public int CallId;
            public string PhoneNumber;
            public bool IsRecording;
            public string CallerId;
            public Dictionary<string, object> RespondentVariables;
        }

        public void SetBehaviorForSendNumberToAgent(Func<SendNumberToAgentParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.SendNumberToAgent),
                args =>
                {
                    var parameters = new SendNumberToAgentParams() {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = long.Parse((string)args[2]),
                        DiallingMode = (DialingMode)args[3],
                        InterviewId = (int)args[4],
                        CallId = (int)args[5],
                        PhoneNumber = (string)args[6],
                        IsRecording = (bool)args[7],
                        CallerId = (string)args[8],
                        RespondentVariables = (Dictionary<string, object>)args[9]
                    };

                    return behavior(parameters);
                });
        }

        public class RedialParams
        {
            public string TenantId;
            public long CampaignId;
            public long AgentId;
            public int InterviewId;
            public int CallId;
            public string PhoneNumber;
            public bool IsRecording;
            public string CallerId;
        }

        public void SetBehaviorForRedial(Func<RedialParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.Redial),
                args =>
                {
                    var parameters = new RedialParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = long.Parse((string)args[2]),
                        InterviewId = (int)args[3],
                        CallId = (int)args[4],
                        PhoneNumber = (string)args[5],
                        IsRecording = (bool)args[6],
                        CallerId = (string)args[7],
                    };

                    return behavior(parameters);
                });
        }

        public class CompletePreviewParams
        {
            public string TenantId;
            public long CampaignId;
            public long AgentId;
            public int ContactId;
            public int CallId;
            public string PhoneNumber;
            public bool IsRecording;
        }

        public void SetBehaviorForCompletePreview(Func<CompletePreviewParams, int> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.CompletePreview),
                args =>
                {
                    var parameters = new CompletePreviewParams()
                    {
                        TenantId = (string)args[0],
                        CampaignId = (long)args[1],
                        AgentId = long.Parse((string)args[2]),
                        ContactId = (int)args[3],
                        CallId = (int)args[4],
                        PhoneNumber = (string)args[5],
                        IsRecording = (bool)args[6],
                    };

                    return behavior(parameters);
                });
        }

        public void SetBehaviorForSendNumberToAgent(Func<SendNumberToAgentParams, CallOutcome> behavior)
        {
            SetBehaviorForSendNumberToAgent((args) =>
            {
                SendNotification(() => SendEventNotifyOutcome(args.CampaignId, args.AgentId, args.CallId, behavior(args)));
                return 0;
            });
        }

        public void SetAutoCallOutcomes(params CallOutcome[] outcomes)
        {
            int currentOutcomeIndex = 0;

            SetAutoCallOutcomesBehavior((personId, callId) => outcomes[currentOutcomeIndex++ % outcomes.Length]);
        }

        public void SetAutoCallOutcomesBehavior(Func<long, int, CallOutcome> generator)
        {
            SetBehaviorForSendNumberToAgent((args) => generator(args.AgentId, args.CallId));
        }

        public void SetAutoResponseOnFlushNumbers(Action<IEnumerable<CallInfo>> catcher)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.FlushNumbers),
            args =>
            {
                catcher((List<CallInfo>)(args[2]));
                return 0;
            });
        }

        public void AddRequestRedial()
        {
            FakeDialer.AddExpectedRequest(nameof(IDialerAPI.Redial));
        }

        public void SetBehaviorForGetLogFiles(Func<IEnumerable<LogFileInfo>> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GetLogFiles), args => behavior());
        }

        public void SetBehaviorForGetLogFileBodyZipped(Func<string, byte[]> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GetLogFileBodyZipped), args => behavior((string)args[0]));
        }

        public class EmptyParams
        {
        }

        public void SetBehaviorForGetDialerVersion(Func<EmptyParams, string> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GetDialerVersion),
                args =>
                {
                    var parameters = new EmptyParams()
                    {
                    };

                    return behavior(parameters);
                });
        }

        public class GetFeaturesParams
        {
            public string TenantId;
        }

        public void SetBehaviorForGetFeatures(Func<GetFeaturesParams, DialerFeatures> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GetFeatures),
                args =>
                {
                    var parameters = new GetFeaturesParams()
                    {
                        TenantId = (string)args[0],
                    };

                    return behavior(parameters);
                });
        }

        public class GetStateParams
        {
            public string TenantId;
        }

        public void SetBehaviorForGetState(Func<GetStateParams, DialerState> behavior)
        {
            FakeDialer.SetDefaultRequestBehavior(nameof(IDialerAPI.GetState),
                args =>
                {
                    var parameters = new GetStateParams
                    {
                        TenantId = (string)args[0],
                    };

                    return behavior(parameters);
                });
        }
    }
}