using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

using ConfirmitDialerInterface;
using DialerCommon.Logging;

namespace Confirmit.CATI.Telephony.DialerCommon
{
    /// <summary>
    /// Sends the service started notification.
    /// </summary>
    public class ServiceStartedNotificationSender
    {
        private readonly ICommonLogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStartedNotificationSender"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ServiceStartedNotificationSender(ICommonLogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Sends the service started notification using dialer identity from file.
        /// </summary>
        /// <param name="dialerId">
        /// The dialer Id.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        public void SendServiceStartedNotification(int dialerId, int companyId)
        {
            var sender = new DialerEventNotificationsSender(logger, dialerId, companyId);
            var stateEvent = new DialerEventNotifyDialerState(DialerEventPriority.LowPriority, DialerState.DialerServiceStarted, companyId);
            sender.SendEventNotification(stateEvent);
        }
    }
}