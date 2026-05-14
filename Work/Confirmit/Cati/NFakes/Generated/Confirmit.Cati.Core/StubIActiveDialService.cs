using System;
using Confirmit.CATI.Core.Telephony.Dial.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Core.Telephony.Dial.Interfaces.Fakes
{
    public class StubIActiveDialService : IActiveDialService 
    {
        private IActiveDialService _inner;

        public StubIActiveDialService()
        {
            _inner = null;
        }

        public IActiveDialService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvActiveDialEntity CreateInboundCallInt32StringStringStringDelegate(int dialerId, string inboundCallId, string ddiNumber, string telephoneNumber);
        public CreateInboundCallInt32StringStringStringDelegate CreateInboundCallInt32StringStringString;

        BvActiveDialEntity IActiveDialService.CreateInboundCall(int dialerId, string inboundCallId, string ddiNumber, string telephoneNumber)
        {


            if (CreateInboundCallInt32StringStringString != null)
            {
                return CreateInboundCallInt32StringStringString(dialerId, inboundCallId, ddiNumber, telephoneNumber);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).CreateInboundCall(dialerId, inboundCallId, ddiNumber, telephoneNumber);
            }

            return default(BvActiveDialEntity);
        }

        public delegate BvActiveDialEntity CreateOutboundCallInt32Int64Int64Delegate(int dialerId, long campaignId, long callId);
        public CreateOutboundCallInt32Int64Int64Delegate CreateOutboundCallInt32Int64Int64;

        BvActiveDialEntity IActiveDialService.CreateOutboundCall(int dialerId, long campaignId, long callId)
        {


            if (CreateOutboundCallInt32Int64Int64 != null)
            {
                return CreateOutboundCallInt32Int64Int64(dialerId, campaignId, callId);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).CreateOutboundCall(dialerId, campaignId, callId);
            }

            return default(BvActiveDialEntity);
        }

        public delegate InboundHandlerOperationType AcceptInboundCallBvActiveDialEntityBvSurveyEntityBvInterviewEntityBvCallEntityDelegate(BvActiveDialEntity dial, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call);
        public AcceptInboundCallBvActiveDialEntityBvSurveyEntityBvInterviewEntityBvCallEntityDelegate AcceptInboundCallBvActiveDialEntityBvSurveyEntityBvInterviewEntityBvCallEntity;

        InboundHandlerOperationType IActiveDialService.AcceptInboundCall(BvActiveDialEntity dial, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call)
        {


            if (AcceptInboundCallBvActiveDialEntityBvSurveyEntityBvInterviewEntityBvCallEntity != null)
            {
                return AcceptInboundCallBvActiveDialEntityBvSurveyEntityBvInterviewEntityBvCallEntity(dial, survey, interview, call);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).AcceptInboundCall(dial, survey, interview, call);
            }

            return default(InboundHandlerOperationType);
        }

        public delegate InboundHandlerOperationType DropInboundCallBvActiveDialEntityDropInboundCallReasonDelegate(BvActiveDialEntity dial, DropInboundCallReason dropInboundCallReason);
        public DropInboundCallBvActiveDialEntityDropInboundCallReasonDelegate DropInboundCallBvActiveDialEntityDropInboundCallReason;

        InboundHandlerOperationType IActiveDialService.DropInboundCall(BvActiveDialEntity dial, DropInboundCallReason dropInboundCallReason)
        {


            if (DropInboundCallBvActiveDialEntityDropInboundCallReason != null)
            {
                return DropInboundCallBvActiveDialEntityDropInboundCallReason(dial, dropInboundCallReason);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).DropInboundCall(dial, dropInboundCallReason);
            }

            return default(InboundHandlerOperationType);
        }

        public delegate DialerErrorCode DialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityStringDelegate(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, string telephoneNumber);
        public DialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityStringDelegate DialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityString;

        DialerErrorCode IActiveDialService.Dial(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, string telephoneNumber)
        {


            if (DialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityString != null)
            {
                return DialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityString(ref dial, task, survey, interview, telephoneNumber);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).Dial(ref dial, task, survey, interview, telephoneNumber);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode RedialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityStringDelegate(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, string telephoneNumber);
        public RedialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityStringDelegate RedialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityString;

        DialerErrorCode IActiveDialService.Redial(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, string telephoneNumber)
        {


            if (RedialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityString != null)
            {
                return RedialBvActiveDialEntityRefBvTasksEntityBvSurveyEntityBvInterviewEntityString(ref dial, task, survey, interview, telephoneNumber);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).Redial(ref dial, task, survey, interview, telephoneNumber);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode SetNextInterviewBvActiveDialEntityInterviewStatusBvCallEntityDelegate(BvActiveDialEntity dial, InterviewStatus status, BvCallEntity call);
        public SetNextInterviewBvActiveDialEntityInterviewStatusBvCallEntityDelegate SetNextInterviewBvActiveDialEntityInterviewStatusBvCallEntity;

        DialerErrorCode IActiveDialService.SetNextInterview(BvActiveDialEntity dial, InterviewStatus status, BvCallEntity call)
        {


            if (SetNextInterviewBvActiveDialEntityInterviewStatusBvCallEntity != null)
            {
                return SetNextInterviewBvActiveDialEntityInterviewStatusBvCallEntity(dial, status, call);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).SetNextInterview(dial, status, call);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode CompleteCallBvTasksEntityInt32Int64Int64NullableOfInt32BooleanStringInterviewStatusCallCompleteStatusDelegate(BvTasksEntity task, int dialerId, long campaignId, long agentId, int? callId, bool makeAgentReady, string breakName, InterviewStatus status, CallCompleteStatus callCompleteStatus);
        public CompleteCallBvTasksEntityInt32Int64Int64NullableOfInt32BooleanStringInterviewStatusCallCompleteStatusDelegate CompleteCallBvTasksEntityInt32Int64Int64NullableOfInt32BooleanStringInterviewStatusCallCompleteStatus;

        DialerErrorCode IActiveDialService.CompleteCall(BvTasksEntity task, int dialerId, long campaignId, long agentId, int? callId, bool makeAgentReady, string breakName, InterviewStatus status, CallCompleteStatus callCompleteStatus)
        {


            if (CompleteCallBvTasksEntityInt32Int64Int64NullableOfInt32BooleanStringInterviewStatusCallCompleteStatus != null)
            {
                return CompleteCallBvTasksEntityInt32Int64Int64NullableOfInt32BooleanStringInterviewStatusCallCompleteStatus(task, dialerId, campaignId, agentId, callId, makeAgentReady, breakName, status, callCompleteStatus);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).CompleteCall(task, dialerId, campaignId, agentId, callId, makeAgentReady, breakName, status, callCompleteStatus);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode HangupBvTasksEntityBvSurveyEntityInt32Delegate(BvTasksEntity task, BvSurveyEntity survey, int initiator);
        public HangupBvTasksEntityBvSurveyEntityInt32Delegate HangupBvTasksEntityBvSurveyEntityInt32;

        DialerErrorCode IActiveDialService.Hangup(BvTasksEntity task, BvSurveyEntity survey, int initiator)
        {


            if (HangupBvTasksEntityBvSurveyEntityInt32 != null)
            {
                return HangupBvTasksEntityBvSurveyEntityInt32(task, survey, initiator);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).Hangup(task, survey, initiator);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode KillAgentBvTasksEntityBvSurveyEntityDelegate(BvTasksEntity task, BvSurveyEntity survey);
        public KillAgentBvTasksEntityBvSurveyEntityDelegate KillAgentBvTasksEntityBvSurveyEntity;

        DialerErrorCode IActiveDialService.KillAgent(BvTasksEntity task, BvSurveyEntity survey)
        {


            if (KillAgentBvTasksEntityBvSurveyEntity != null)
            {
                return KillAgentBvTasksEntityBvSurveyEntity(task, survey);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).KillAgent(task, survey);
            }

            return default(DialerErrorCode);
        }

        public delegate void OnDialNotifyOutcomeBvActiveDialEntityBvTasksEntityCallOutcomeDelegate(BvActiveDialEntity dial, BvTasksEntity task, CallOutcome callOutcome);
        public OnDialNotifyOutcomeBvActiveDialEntityBvTasksEntityCallOutcomeDelegate OnDialNotifyOutcomeBvActiveDialEntityBvTasksEntityCallOutcome;

        void IActiveDialService.OnDialNotifyOutcome(BvActiveDialEntity dial, BvTasksEntity task, CallOutcome callOutcome)
        {

            if (OnDialNotifyOutcomeBvActiveDialEntityBvTasksEntityCallOutcome != null)
            {
                OnDialNotifyOutcomeBvActiveDialEntityBvTasksEntityCallOutcome(dial, task, callOutcome);
            } else if (_inner != null)
            {
                ((IActiveDialService)_inner).OnDialNotifyOutcome(dial, task, callOutcome);
            }
        }

        public delegate void AttachDialToTaskContextIfNeedBvActiveDialEntityBvTasksEntityDelegate(BvActiveDialEntity dial, BvTasksEntity task);
        public AttachDialToTaskContextIfNeedBvActiveDialEntityBvTasksEntityDelegate AttachDialToTaskContextIfNeedBvActiveDialEntityBvTasksEntity;

        void IActiveDialService.AttachDialToTaskContextIfNeed(BvActiveDialEntity dial, BvTasksEntity task)
        {

            if (AttachDialToTaskContextIfNeedBvActiveDialEntityBvTasksEntity != null)
            {
                AttachDialToTaskContextIfNeedBvActiveDialEntityBvTasksEntity(dial, task);
            } else if (_inner != null)
            {
                ((IActiveDialService)_inner).AttachDialToTaskContextIfNeed(dial, task);
            }
        }

        public delegate void DetachDialFromTaskContextIfNeedTaskContextDelegate(TaskContext context);
        public DetachDialFromTaskContextIfNeedTaskContextDelegate DetachDialFromTaskContextIfNeedTaskContext;

        void IActiveDialService.DetachDialFromTaskContextIfNeed(TaskContext context)
        {

            if (DetachDialFromTaskContextIfNeedTaskContext != null)
            {
                DetachDialFromTaskContextIfNeedTaskContext(context);
            } else if (_inner != null)
            {
                ((IActiveDialService)_inner).DetachDialFromTaskContextIfNeed(context);
            }
        }

        public delegate DialerErrorCode TransferStartBvActiveDialEntityTransferTypeConsoleTransferStateDelegate(BvActiveDialEntity dial, TransferType transferType, ConsoleTransferState initialTransferState);
        public TransferStartBvActiveDialEntityTransferTypeConsoleTransferStateDelegate TransferStartBvActiveDialEntityTransferTypeConsoleTransferState;

        DialerErrorCode IActiveDialService.TransferStart(BvActiveDialEntity dial, TransferType transferType, ConsoleTransferState initialTransferState)
        {


            if (TransferStartBvActiveDialEntityTransferTypeConsoleTransferState != null)
            {
                return TransferStartBvActiveDialEntityTransferTypeConsoleTransferState(dial, transferType, initialTransferState);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).TransferStart(dial, transferType, initialTransferState);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferSetConnectionStateBvActiveDialEntityConnectionStateDelegate(BvActiveDialEntity dial, ConnectionState connectionState);
        public TransferSetConnectionStateBvActiveDialEntityConnectionStateDelegate TransferSetConnectionStateBvActiveDialEntityConnectionState;

        DialerErrorCode IActiveDialService.TransferSetConnectionState(BvActiveDialEntity dial, ConnectionState connectionState)
        {


            if (TransferSetConnectionStateBvActiveDialEntityConnectionState != null)
            {
                return TransferSetConnectionStateBvActiveDialEntityConnectionState(dial, connectionState);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).TransferSetConnectionState(dial, connectionState);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferSetTargetBvActiveDialEntityTargetTypeStringBooleanDelegate(BvActiveDialEntity dial, TargetType targetType, string resource, bool borrowAgentsFromAllCampaigns);
        public TransferSetTargetBvActiveDialEntityTargetTypeStringBooleanDelegate TransferSetTargetBvActiveDialEntityTargetTypeStringBoolean;

        DialerErrorCode IActiveDialService.TransferSetTarget(BvActiveDialEntity dial, TargetType targetType, string resource, bool borrowAgentsFromAllCampaigns)
        {


            if (TransferSetTargetBvActiveDialEntityTargetTypeStringBoolean != null)
            {
                return TransferSetTargetBvActiveDialEntityTargetTypeStringBoolean(dial, targetType, resource, borrowAgentsFromAllCampaigns);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).TransferSetTarget(dial, targetType, resource, borrowAgentsFromAllCampaigns);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferCompleteBvActiveDialEntityBvTasksEntityDelegate(BvActiveDialEntity dial, BvTasksEntity task);
        public TransferCompleteBvActiveDialEntityBvTasksEntityDelegate TransferCompleteBvActiveDialEntityBvTasksEntity;

        DialerErrorCode IActiveDialService.TransferComplete(BvActiveDialEntity dial, BvTasksEntity task)
        {


            if (TransferCompleteBvActiveDialEntityBvTasksEntity != null)
            {
                return TransferCompleteBvActiveDialEntityBvTasksEntity(dial, task);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).TransferComplete(dial, task);
            }

            return default(DialerErrorCode);
        }

        public delegate DialerErrorCode TransferCancelBvActiveDialEntityDelegate(BvActiveDialEntity dial);
        public TransferCancelBvActiveDialEntityDelegate TransferCancelBvActiveDialEntity;

        DialerErrorCode IActiveDialService.TransferCancel(BvActiveDialEntity dial)
        {


            if (TransferCancelBvActiveDialEntity != null)
            {
                return TransferCancelBvActiveDialEntity(dial);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).TransferCancel(dial);
            }

            return default(DialerErrorCode);
        }

        public delegate void SetTransferStateBvActiveDialEntityConsoleTransferStateDelegate(BvActiveDialEntity dial, ConsoleTransferState transferState);
        public SetTransferStateBvActiveDialEntityConsoleTransferStateDelegate SetTransferStateBvActiveDialEntityConsoleTransferState;

        void IActiveDialService.SetTransferState(BvActiveDialEntity dial, ConsoleTransferState transferState)
        {

            if (SetTransferStateBvActiveDialEntityConsoleTransferState != null)
            {
                SetTransferStateBvActiveDialEntityConsoleTransferState(dial, transferState);
            } else if (_inner != null)
            {
                ((IActiveDialService)_inner).SetTransferState(dial, transferState);
            }
        }

        public delegate ConnectionState GetInitialConnectionStateConsoleTransferTypeDelegate(ConsoleTransferType transferType);
        public GetInitialConnectionStateConsoleTransferTypeDelegate GetInitialConnectionStateConsoleTransferType;

        ConnectionState IActiveDialService.GetInitialConnectionState(ConsoleTransferType transferType)
        {


            if (GetInitialConnectionStateConsoleTransferType != null)
            {
                return GetInitialConnectionStateConsoleTransferType(transferType);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).GetInitialConnectionState(transferType);
            }

            return default(ConnectionState);
        }

        public delegate DialerErrorCode TransferConfirmBvActiveDialEntityBvPersonEntityDelegate(BvActiveDialEntity dial, BvPersonEntity person);
        public TransferConfirmBvActiveDialEntityBvPersonEntityDelegate TransferConfirmBvActiveDialEntityBvPersonEntity;

        DialerErrorCode IActiveDialService.TransferConfirm(BvActiveDialEntity dial, BvPersonEntity person)
        {


            if (TransferConfirmBvActiveDialEntityBvPersonEntity != null)
            {
                return TransferConfirmBvActiveDialEntityBvPersonEntity(dial, person);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).TransferConfirm(dial, person);
            }

            return default(DialerErrorCode);
        }

        public delegate int CleanActiveDialsTimeSpanDelegate(TimeSpan expirationPeriod);
        public CleanActiveDialsTimeSpanDelegate CleanActiveDialsTimeSpan;

        int IActiveDialService.CleanActiveDials(TimeSpan expirationPeriod)
        {


            if (CleanActiveDialsTimeSpan != null)
            {
                return CleanActiveDialsTimeSpan(expirationPeriod);
            } else if (_inner != null)
            {
                return ((IActiveDialService)_inner).CleanActiveDials(expirationPeriod);
            }

            return default(int);
        }

    }
}