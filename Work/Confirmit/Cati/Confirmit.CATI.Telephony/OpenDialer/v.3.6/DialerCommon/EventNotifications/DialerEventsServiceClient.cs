using System;
using System.Collections.Generic;
using Confirmit.CATI.Common.WcfTools;
using ConfirmitDialerInterface;

using ILoggerCatiCommon = Confirmit.CATI.Common.ILogger;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    /// <summary>
    /// Client wrapper for DialerEventsService.
    /// </summary>
    public class DialerEventsServiceClient : IDisposable
    {
        private readonly ChannelFactoryWrapper<IDialerEventsHandlerService> _channelFactoryWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialerEventsServiceClient"/> class.
        /// </summary>
        /// <param name="companyId">The company ID. Used to connect to sent events to specific backend instance.</param>
        /// <param name="logger">The logger.</param>
        public DialerEventsServiceClient(int companyId, ILoggerCatiCommon logger)
        {
            var configuration = new DialerEventsHandlerServiceChannelFactoryWrapperConfiguration(companyId);

            _channelFactoryWrapper = new ChannelFactoryWrapper<IDialerEventsHandlerService>(configuration, logger);
        }

        /// <summary>
        /// This function is being used to pass back information about the status of the User. 
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
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyUserState(
                    dialerId,
                    requestId,
                    sessionId,
                    companyId,
                    surveyId,
                    userId,
                    userState));
        }

        /// <summary>
        /// This function is being used to pass back the outcome for a call.
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
        /// <param name="dialerAccompanyingCallInfo">Some accompanying info received from dialer. PROTSInternalFlag in case of PROTS.</param>
        public void NotifyOutcome(
            int dialerId,
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            string userId,
            string contactId,
            int callId,
            string outcomeType,
            string outcomeCode,
            string dialerAccompanyingCallInfo)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyOutcome(
                    dialerId,
                    requestId,
                    sessionId,
                    companyId,
                    surveyId,
                    userId,
                    contactId,
                    callId,
                    outcomeType,
                    outcomeCode,
                    dialerAccompanyingCallInfo));
        }

        public void NotifyInboundCall(int dialerId, int companyId, string ddiNumber, string cliNumber, string inboundCallId)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyInboundCall(
                    dialerId,
                    companyId,
                    ddiNumber, 
                    cliNumber,
                    inboundCallId));
        }

        public void NotifyCallDroppedByRespondent(int dialerId, int companyId, long surveyId, int agentId, long callId)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyCallDroppedByRespondent(
                    dialerId,
                    companyId,
                    surveyId,
                    agentId,
                    callId));
        }

        public void NotifyInboundCallDroppedByRespondent(int dialerId, int companyId, string inboundCallId)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyInboundCallDroppedByRespondent(
                    dialerId,
                    companyId,
                    inboundCallId));
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
        public void ScreenPop(
            int dialerId,
            string requestId, 
            string sessionId, 
            string companyId, 
            long surveyId, 
            string userId, 
            string contactId, 
            int callId, 
            DialingMode callDialingMode)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.ScreenPop(
                    dialerId,
                    requestId,
                    sessionId,
                    companyId,
                    surveyId,
                    userId,
                    contactId,
                    callId,
                    callDialingMode));
        }

        /// <summary>
        /// This function is being used to pass back errors
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dialerId"></param>
        /// <param name="dialerState"></param>
        public void NotifyDialerState(int dialerId, string companyId, DialerState dialerState)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyDialerState(dialerId, companyId, dialerState));
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
        public void RequestCalls(
            int dialerId,
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            int? groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int callCount)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.RequestCalls(
                    dialerId,
                    requestId,
                    sessionId,
                    companyId,
                    surveyId,
                    groupId,
                    callsSelectionAlgorithm,
                    callCount));
        }

        public void NotifyLicenseExpiration(int dialerId, DateTime licenseExpirationDateTime)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyLicenseExpiration(dialerId, licenseExpirationDateTime));
        }

        public void NotifyIvrSubmit(int dialerId, string companyId, long surveyId, string agentId, KeyValuePair<string, string>[] variables)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyIvrSubmit(dialerId, companyId, surveyId, agentId, variables));
        }

        public void NotifyTransferState(int dialerId, int companyId, string transferId, TransferState transferState)
        {
            _channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyTransferState(dialerId, companyId, transferId, transferState));
        }

        /// <summary>
        /// Releases WCF client proxy used by this class.
        /// </summary>
        public void Dispose()
        {
            if (_channelFactoryWrapper != null)
            {
                _channelFactoryWrapper.Release();
            }
        }
    }
}