using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;
using DialerCommon.EventNotifications;
using DialerCommon.Logging;

namespace Confirmit.CATI.Telephony.DialerCommon
{
    /// <summary>
    /// Creates and initializes the <see cref="DialerEventNotificationsSender"/> instances.
    /// </summary>
    public class NotificationsSenderInitializer
    {
        private readonly ICommonLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsSenderInitializer"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NotificationsSenderInitializer(ICommonLogger logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// Initializes the new instance of <see cref="DialerEventNotificationsSender"/> and without saving dialer identity info to file.
        /// </summary>
        /// <param name="dialerId">
        /// The dialer ID.
        /// </param>
        /// <param name="companyId">
        /// The company ID.
        /// </param>
        /// <returns>
        /// initialized instance of <see cref="DialerEventNotificationsSender"/>.
        /// </returns>
        public DialerEventNotificationsSender InitializeIdentity(int dialerId, int companyId)
        {
            DialerServicePerformanceCounters.Initialize();

            var sender = new DialerEventNotificationsSender(_logger, dialerId, companyId);

            return sender;
        }
    }
}
