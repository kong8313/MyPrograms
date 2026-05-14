using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.IntegrationTests.Framework.Controllers.Consoles;
using Confirmit.CATI.IntegrationTests.Framework.Data;
using Confirmit.CATI.IntegrationTests.Framework.Dialer;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.IntegrationTests.Framework.Controllers
{
    public class DialerController : Ref<BvDialersEntity>
    {
        private readonly IDialersRepository _dialersRepository;
        
        public TestDialerHelper Helper => Behavior;

        public TestDialerHelper DialerHelper => Behavior;

        public DialerController(TestDataContext context, string tag, int id, DialerBehaviorController helper)
            : base(tag, id, context)
        {
            Behavior = helper;
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
        }
        
        public override BvDialersEntity Model => _dialersRepository.GetById(Id);

        public DialerBehaviorController Behavior { get; }

        public TestDialerHelper.SendNumbersParams RequestCalls(SurveyController survey, int count, CallsSelectionAlgorithm algorithm = CallsSelectionAlgorithm.ByPersonGroup, int groupId = 0)
        {

            var group = groupId == 0 ? survey.Id : groupId;

            TestDialerHelper.SendNumbersParams result = null;
            Behavior.SetBehaviorForSendNumbers((parameters) =>
            {
                result = parameters;
                return 0;
            });
            Behavior.SendEventRequestCalls(survey.Model.CampaignId, group, count, algorithm);

            return result;
        }

        public List<CallInfo> FlushedCalls = new List<CallInfo>();

        public void SetOutcomes(params CallOutcome[] outcomes)
        {
            Behavior.SetAutoCallOutcomes(outcomes);
        }

        public void SetOutcomeBehaviors(Func<CallRef, CallOutcome> behavior)
        {
            Behavior.SetAutoCallOutcomesBehavior((personId, callId) =>
            {
                var dbCall = CallQueueService.GetCallInfo(callId);
                var call = Context.Calls.SingleOrDefault(x => x.Interview.Survey.Id == dbCall.SurveySID && x.Id == dbCall.InterviewID);
                return behavior(call);
            });
        }

        public int ProcessAllPosponedNotification()
        {
            return Behavior.ProcessAllPosponedNotification();
        }

        [Obsolete("You should use DialerData.ReplyType field to setup behavior")]
        public void SetNotificationReply(ReplyType replyType)
        {
            Behavior.NotificationReplyType = replyType;
        }

        public void SendPredicitvePreviewCall(long campaignId, CallInfo callinfo, PersonController person)
        {
            Behavior.SendEventScreenPop(campaignId, person.Id, callinfo.interviewId, (int)callinfo.callId, DialingMode.Preview);
        }

        public void SendPredicitveConnectedCall(long campaignId, CallInfo callinfo, PersonController person)
        {
            Behavior.SendEventNotifyOutcome(campaignId, person.Id, (int)callinfo.callId, CallOutcome.Connected);
        }

        public void SendPredicitveNoConnectedCall(TestDialerHelper.SendNumbersParams requestedCalls, CallInfo callinfo, CallOutcome outcome = CallOutcome.Busy)
        {
            Behavior.SendEventNotifyOutcome(requestedCalls.CampaignId, 0, (int)callinfo.callId, outcome);
        }

        public void SendNotifyInboundCall(string inboundCallNumber, string callerPhoneNumber, string inboundCallId)
        {
            Behavior.SendEventNotifyInboundCall(inboundCallNumber, callerPhoneNumber, inboundCallId);
        }

        public void SendNotifyDropInboundCall(string inboundCallId)
        {
            Behavior.SendEventNotifyDropInboundCall(inboundCallId);
        }

        public void SendEventNotifyDropCallByRespondent(long campaignId, long agentId, long callId)
        {
            Behavior.SendEventNotifyDropCallByRespondent(campaignId, agentId, callId);
        }

        public PredictiveDialerController Predictive(string surveyTag)
        {
            return new PredictiveDialerController(this, Context.GetSurvey(surveyTag));
        }

        public AutomaticDialerController Automatic(string surveyTag)
        {
            return new AutomaticDialerController(this, Context.GetSurvey(surveyTag));
        }

        public void Connect(TestDialerHelper.ConnectInboundCallToAgentParams inbound)
        {
            DialerMethodBehaviors.SendOutcomeConnected(this.Behavior, inbound);
        }

        public void Connect(TestDialerHelper.SendNumberToAgentParams sendNumberToAgentParams)
        {
            DialerMethodBehaviors.SendOutcomeConnected(this.Behavior, sendNumberToAgentParams);
        }

        public void Preview(ConsoleController console, string callTag)
        {
            var call = Context.GetCall(callTag);
            DialerMethodBehaviors.SendEventScreenPop(console.Person, call);
        }

        public void Connect(ConsoleController console, string callTag)
        {
            var call = Context.GetCall(callTag);
            DialerMethodBehaviors.SendOutcomeConnected(this.Behavior, console, call);
        }

        public void Busy(TestDialerHelper.SendNumberToAgentParams sendNumberToAgentParams)
        {
            DialerMethodBehaviors.SendOutcomeNotConnected(this.Behavior, sendNumberToAgentParams, CallOutcome.Busy);
        }
    }
}