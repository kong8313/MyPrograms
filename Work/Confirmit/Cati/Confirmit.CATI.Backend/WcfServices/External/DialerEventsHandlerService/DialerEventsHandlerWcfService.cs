using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WcfServices.Tools.IPFilter;
using Confirmit.CATI.Common.WcfTools.ErrorContextHandler;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony.DialerCommon;
using ConfirmitDialerInterface;
using System;
using System.Diagnostics;
using System.ServiceModel;
using Confirmit.CATI.Backend.WcfServices.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.IpLockDown.IPFilterInspectors;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Backend.WcfServices.External.DialerEventsHandlerService
{
    [IpFilterBehavior(IpFilterMode = IpFilterMode.Dialer)]
    [ErrorContextHandler(WebServiceType.Internal)]
    [MetricsBehaviour(TrackMethodsSeparately = false)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DialerEventsHandlerServiceHttp : DialerEventsHandlerService
    {
    }
    
    [IpFilterBehavior(IpFilterMode = IpFilterMode.Dialer)]
    [ErrorContextHandler(WebServiceType.External)]
    [MetricsBehaviour(TrackMethodsSeparately = false)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class DialerEventsHandlerService : IDialerEventsHandlerService
    {
        private readonly Lazy<IAsyncManager> _asyncManager;
        private readonly Lazy<IDialerEventsHandler> _dialerEventsHandler;
        private readonly Lazy<IDialerEmailNotificationService> _dialerEmailNotificationService;
        private readonly Lazy<IDialerCollection> _dialerCollection;

        public DialerEventsHandlerService()
        {
            _asyncManager = new Lazy<IAsyncManager>(() => ServiceLocator.Resolve<IAsyncManager>());
            _dialerEventsHandler = new Lazy<IDialerEventsHandler>(() => ServiceLocator.Resolve<IDialerEventsHandler>());
            _dialerEmailNotificationService = new Lazy<IDialerEmailNotificationService>(() => ServiceLocator.Resolve<IDialerEmailNotificationService>());
            _dialerCollection = new Lazy<IDialerCollection>(() => ServiceLocator.Resolve<IDialerCollection>());
        }


        /// <summary>
        /// This function is used to pass back information about the status of the User. 
        /// It is especially needed for asynchronous functions like login, logout, go ready, etc. 
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="userState"></param>
        public void NotifyUserState(
            int dialerId,
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            string userId,
            string userState)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerNotifyAgentState(
                    dialerId,
                    companyId,
                    surveyId,
                    int.Parse(userId),
                    userState),

                () => string.Format("DialerEventsHandlerService.NotifyUserState. " +
                    "dialerId=[{0}], requestId=[{1}], sessionId=[{2}], " +
                    "companyId=[{3}], surveyId=[{4}], userId=[{5}], userState=[{6}]",
                    dialerId, requestId, sessionId,
                    companyId, surveyId, userId, userState));
        }

        /// <summary>
        /// This function is used to pass back call outcome information.
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="outcomeType"></param>
        /// <param name="outcomeCode"></param>
        /// <param name="callerId"></param>
        /// <param name="ringTime"></param>
        /// <param name="callOutcomeMetadata"></param>
        /// <param name="correlationId"></param>
        public void NotifyOutcome(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId,
            string contactId, int callId, string outcomeType, string outcomeCode,
            string callerId, TimeSpan ringTime, Dictionary<string, string> callOutcomeMetadata, string correlationId)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerNotifyOutcome(
                    dialerId,
                    companyId,
                    surveyId,
                    int.Parse(userId),
                    contactId,
                    callId,
                    int.Parse(outcomeCode),
                    callerId,
                    ringTime,
                    callOutcomeMetadata),

                () => string.Format("DialerEventsHandlerService.NotifyOutcome. " +
                                    "dialerId={0}, requestId={1}, sessionId={2}, companyId={3}, surveyId={4}, " +
                                    "userId={5}, contactId={6}, callId={7}, outcomeType={8}, outcomeCode={9}, " +
                                    "callerId={10}, ringTime={11}, callOutcomeMetadata={12}, correlationId={13}",
                    dialerId, requestId, sessionId, companyId, surveyId,
                    userId, contactId, callId, outcomeType, outcomeCode,
                    callId, ringTime, callOutcomeMetadata?.Stringify(), correlationId));
        }

        public void NotifyInboundCall(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerNotifyInboundCall(
                    dialerId,
                    companyId,
                    ddiNumber,
                    cliNumber,
                    inboundCallId),

                () => string.Format("DialerEventsHandlerService.NotifyInboundCall. " +
                    "dialerId={0}, companyId={1}, ddiNumber={2},  cliNumber={3}, inboundCallId={4}",
                    dialerId, companyId, ddiNumber, cliNumber, inboundCallId));
        }

        public void NotifyCallDroppedByRespondent(int dialerId, int companyId, long surveyId, int agentId, long callId)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerNotifyCallDroppedByRespondent(
                    dialerId,
                    companyId.ToString(),
                    surveyId,
                    agentId,
                    callId),

                () => string.Format("DialerEventsHandlerService.NotifyCallDroppedByRespondent. " +
                                    "dialerId={0}, companyId={1}, surveyId={2}, agentId={3}, callId={4}",
                    dialerId, companyId, surveyId, agentId, callId));
        }

        public void NotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerNotifyInboundCallDroppedByRespondent(
                    dialerId,
                    companyId,
                    inboundCallId),

                () => string.Format("DialerEventsHandlerService.OnDialerNotifyInboundCallDroppedByRespondent. " +
                    "dialerId={0}, companyId={1}, inboundCallId={2}",
                    dialerId, companyId, inboundCallId));
        }

        public void NotifyCustomIvrInterviewEnd(int dialerId, int companyId, long campaignId,
            int agentId, int interviewId, long callId, CallOutcome callOutcome)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerNotifyCustomIvrInterviewEnd(
                    dialerId,
                    companyId,
                    campaignId,
                    agentId,
                    interviewId,
                    callId,
                    callOutcome),

                () => string.Format("DialerEventsHandlerService.OnDialerNotifyCustomIvrInterviewEnd. " +
                                    "dialerId={0}, companyId={1}, campaignId={2}, agentId{3}, interviewId={4}, callId={5}, callOutcome={6}",
                    dialerId, companyId, campaignId, agentId, interviewId, callId, callOutcome));
        }
        
        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="callDialingMode"></param>
        public void ScreenPop(int dialerId, string requestId, string sessionId, string companyId, long surveyId, string userId, string contactId, int callId, DialingMode callDialingMode)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerScreenPop(
                    dialerId,
                    companyId,
                    surveyId,
                    int.Parse(userId),
                    contactId,
                    callId,
                    callDialingMode),

                () => string.Format("DialerEventsHandlerService.ScreenPop. " +
                    "dialerId=[{0}], requestId=[{1}], sessionId=[{2}], companyId=[{3}], surveyId=[{4}], userId=[{5}], " +
                    "contactId=[{6}], callId=[{7}], callDialingMode=[{8}]",
                    dialerId, requestId, sessionId, companyId, surveyId, userId,
                    contactId, callId, callDialingMode));
        }

        /// <summary>
        /// This function is used to pass back information about state of the Dialer. 
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="companyId"></param>
        /// <param name="dialerState"></param>
        public void NotifyDialerState(int dialerId, string companyId, DialerState dialerState)
        {
            _asyncManager.Value.QueueWorkItem(
                () =>
                {
                    Trace.TraceInformation(
                        "DialerEventsHandlerService: Dialer [id={0}] state [{1}] event received /// companyId=[{2}]",
                        dialerId, dialerState, companyId);

                    _dialerCollection.Value.GetDialerById(dialerId).OnDialerState(dialerState);
                },

                () => string.Format("DialerEventsHandlerService.NotifyDialerState. " +
                    "companyId=[{0}], dialerId=[{1}], dialerState=[{2}]",
                    companyId, dialerId, dialerState));
        }

        /// <summary>
        /// Dialler requests for calls (predictive)
        /// </summary>
        /// <param name="dialerId"></param>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="groupId"></param>
        /// <param name="callsSelectionAlgorithm"></param>
        /// <param name="callCount">amount of calls the dialer requests for</param>
        public void RequestCalls(int dialerId, string requestId, string sessionId, string companyId, long surveyId, int? groupId, CallsSelectionAlgorithm callsSelectionAlgorithm, int callCount)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerRequestCalls(
                    dialerId,
                    requestId,
                    companyId,
                    surveyId,
                    groupId,
                    callsSelectionAlgorithm,
                    callCount),

                () => string.Format("DialerEventsHandlerService.RequestCalls. " +
                    "dialerId=[{0}], requestId=[{1}], sessionId=[{2}], companyId=[{3}], " +
                    "surveyId=[{4}], groupId=[{5}], callsSelectionAlgorithm=[{6}], callCount=[{7}]",
                    dialerId, requestId, sessionId, companyId,
                    surveyId, groupId, callsSelectionAlgorithm, callCount));
        }

        public void NotifyLicenseExpiration(int dialerId, DateTime licenseExpirationDateTime)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEmailNotificationService.Value.SendDialerLicenseExpirationEmailNotification(
                    dialerId,
                    licenseExpirationDateTime.ToLongDateString()),

                () => string.Format("DialerEventsHandlerService.RequestCalls. " +
                    "dialerId=[{0}], licenseExpirationDateTime=[{1}]",
                    dialerId, licenseExpirationDateTime));
        }

        public void NotifyIvrSubmit(int dialerId, string companyId, long campaignId, string agentId, KeyValuePair<string, string>[] variables)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnDialerIvrSubmit(
                    dialerId,
                    companyId,
                    campaignId,
                    int.Parse(agentId),
                    variables),

                () => string.Format("DialerEventsHandlerService.NotifyDialerState. " +
                    "companyId={0}, dialerId={1}, campaignId={2}, agentId={3}" +
                    "variables={4}",
                    companyId, dialerId, campaignId, agentId,
                    string.Join(", ", variables.Select(x => x.Key + ": " + x.Value))));
        }

        public void NotifyTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {
            _asyncManager.Value.QueueWorkItem(
                () => _dialerEventsHandler.Value.OnTransferState(
                    dialerId,
                    companyId,
                    transferId,
                    transferState),

                () => string.Format("DialerEventsHandlerService.NotifyDialerState. " +
                                    "companyId={0}, dialerId={1}, transferId={2}, transferState={3}",
                    companyId, dialerId, transferId, transferState));
        }
    }
}
