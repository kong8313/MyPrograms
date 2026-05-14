using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

using ConfirmitDialerInterface;
using DialerCommon.Logging;

namespace Confirmit.CATI.Telephony.DialerCommon
{
    /// <summary>
    /// Sends the service started notification.
    /// </summary>
    public class ServiceStartedNotificationSender : IServiceStartedNotificationSender
    {
        private readonly ICommonLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStartedNotificationSender"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ServiceStartedNotificationSender(ICommonLogger logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
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
            var sender = new DialerEventNotificationsSender(_logger, dialerId, companyId);
            var stateEvent = new DialerEventNotifyDialerState(DialerEventPriority.LowPriority, companyId, dialerId, DialerState.DialerServiceStarted);
            sender.SendEventNotification(stateEvent);
        }
    }
}