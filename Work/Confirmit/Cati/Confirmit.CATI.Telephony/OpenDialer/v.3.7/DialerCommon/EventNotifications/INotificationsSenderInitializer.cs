using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

namespace Confirmit.CATI.Telephony.DialerCommon
{
    public interface INotificationsSenderInitializer
    {
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
        IDialerEventNotificationsSender InitializeIdentity(int dialerId, int companyId);
    }
}