using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Tasks;
using Confirmit.CATI.Core.Telephony.Inbound;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.Dial.Interfaces
{
    public interface IActiveDialService
    {
        BvActiveDialEntity CreateInboundCall(int dialerId, string inboundCallId, string ddiNumber, string telephoneNumber);
        BvActiveDialEntity CreateOutboundCall(int dialerId, long campaignId, long callId);


        InboundHandlerOperationType AcceptInboundCall(BvActiveDialEntity dial, BvSurveyEntity survey, BvInterviewEntity interview, BvCallEntity call);
        InboundHandlerOperationType DropInboundCall(BvActiveDialEntity dial, DropInboundCallReason dropInboundCallReason);

        DialerErrorCode Dial(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, string telephoneNumber);
        DialerErrorCode Redial(ref BvActiveDialEntity dial, BvTasksEntity task, BvSurveyEntity survey, BvInterviewEntity interview, string telephoneNumber);
        DialerErrorCode SetNextInterview(BvActiveDialEntity dial, InterviewStatus status, BvCallEntity call);
        DialerErrorCode CompleteCall(BvTasksEntity task, int dialerId, long campaignId, long agentId, int? callId, bool makeAgentReady, string breakName, InterviewStatus status, CallCompleteStatus callCompleteStatus);
        DialerErrorCode Hangup(BvTasksEntity task, BvSurveyEntity survey, int initiator);
        DialerErrorCode KillAgent(BvTasksEntity task, BvSurveyEntity survey);

        void OnDialNotifyOutcome(BvActiveDialEntity dial, BvTasksEntity task, CallOutcome callOutcome);
        void AttachDialToTaskContextIfNeed(BvActiveDialEntity dial, BvTasksEntity task);
        void DetachDialFromTaskContextIfNeed(TaskContext context);
        DialerErrorCode TransferStart(BvActiveDialEntity dial, TransferType transferType, ConsoleTransferState initialTransferState);
        DialerErrorCode TransferSetConnectionState(BvActiveDialEntity dial, ConnectionState connectionState);
        DialerErrorCode TransferSetTarget(BvActiveDialEntity dial, TargetType targetType, string resource, bool borrowAgentsFromAllCampaigns);
        DialerErrorCode TransferComplete(BvActiveDialEntity dial, BvTasksEntity task);
        DialerErrorCode TransferCancel(BvActiveDialEntity dial);
        void SetTransferState(BvActiveDialEntity dial, ConsoleTransferState transferState);
        ConnectionState GetInitialConnectionState(ConsoleTransferType transferType);
        DialerErrorCode TransferConfirm(BvActiveDialEntity dial, BvPersonEntity person);
        int CleanActiveDials(TimeSpan expirationPeriod);
    }
}
