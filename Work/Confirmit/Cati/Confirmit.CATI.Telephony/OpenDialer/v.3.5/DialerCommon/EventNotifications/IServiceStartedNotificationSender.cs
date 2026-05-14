namespace Confirmit.CATI.Telephony.DialerCommon
{
    public interface IServiceStartedNotificationSender
    {
        /// <summary>
        /// Sends the service started notification using dialer identity from file.
        /// </summary>
        /// <param name="dialerId">
        /// The dialer Id.
        /// </param>
        /// <param name="companyId">
        /// The company Id.
        /// </param>
        void SendServiceStartedNotification(int dialerId, int companyId);
    }
}