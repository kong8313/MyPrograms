using System;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    public interface IDialerEventNotificationsSender: IDisposable
    {
        /// <summary>
        /// Sends the event notification to the backend asynchronously. 
        /// </summary>
        /// <param name="dialerEvent">The dialer event.</param>
        /// <remarks>This method should be used in most cases.</remarks>
        void SendEventNotification(IDialerEvent dialerEvent);

        /// <summary>
        /// Sends the event notification synchronously.
        /// </summary>
        /// <param name="dialerEvent">The dialer event.</param>
        /// <remarks>This method should be used only when this class is going
        /// to be disposed after the event sending. In this case asynchronous execution may fail if
        /// communication channel will be closed before event notification is sent.  </remarks>
        bool SendEventNotificationSynchronously(IDialerEvent dialerEvent);
    }
}