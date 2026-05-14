using System;

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
        private readonly int _dialerId;

        private readonly ChannelFactoryWrapper<IDialerEventsHandlerService> _channelFactoryWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialerEventsServiceClient"/> class.
        /// </summary>
        /// <param name="companyId">The company ID. Used to connect to sent events to specific backend instance.</param>
        /// <param name="dialerId">The dialer ID. </param>
        /// <param name="logger">The logger.</param>
        public DialerEventsServiceClient(int companyId, int dialerId, ILoggerCatiCommon logger)
        {
            _dialerId = dialerId;

            var configuration = new DialerEventsHandlerServiceChannelFactoryWrapperConfiguration(companyId);

            _channelFactoryWrapper = new ChannelFactoryWrapper<IDialerEventsHandlerService>(configuration, logger);
        }

        /// <summary>
        /// This function is being used to pass back information about the status of the User. 
        /// It is especially needed for asynchronous functions like login, logout, go ready, etc. 
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="userState"></param>
        public void NotifyUserState(
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            string userId,
            string userState)
        {
            this._channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyUserState(
                    this._dialerId,
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
            this._channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyOutcome(
                    this._dialerId,
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

        /// <summary>
        /// This method is called when dialer ready to call for specified interview.
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="userId"></param>
        /// <param name="contactId"></param>
        /// <param name="callId"></param>
        /// <param name="callDialingMode"></param>
        public void ScreenPop(
            string requestId, 
            string sessionId, 
            string companyId, 
            long surveyId, 
            string userId, 
            string contactId, 
            int callId, 
            DialingMode callDialingMode)
        {
            this._channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.ScreenPop(
                    this._dialerId,
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
        /// <param name="dialerState"></param>
        public void NotifyDialerState(string companyId, DialerState dialerState)
        {
            this._channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyDialerState(this._dialerId, companyId, dialerState));
        }

        /// <summary>
        /// Dialler requests for calls (predictive)
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sessionId"></param>
        /// <param name="companyId"></param>
        /// <param name="surveyId"></param>
        /// <param name="groupId"></param>
        /// <param name="callsSelectionAlgorithm"></param>
        /// <param name="callCount">amount of calls the dialer requests for</param>
        public void RequestCalls(
            string requestId,
            string sessionId,
            string companyId,
            long surveyId,
            int? groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int callCount)
        {
            this._channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.RequestCalls(
                    this._dialerId,
                    requestId,
                    sessionId,
                    companyId,
                    surveyId,
                    groupId,
                    callsSelectionAlgorithm,
                    callCount));
        }

        public void NotifyLicenseExpiration(DateTime licenseExpirationDateTime)
        {
            this._channelFactoryWrapper.Execute(
                dialerEventsHandler => dialerEventsHandler.NotifyLicenseExpiration(this._dialerId, licenseExpirationDateTime));
        }

        /// <summary>
        /// Releases WCF client proxy used by this class.
        /// </summary>
        public void Dispose()
        {
            if (this._channelFactoryWrapper != null)
            {
                this._channelFactoryWrapper.Release();
            }
        }
    }
}