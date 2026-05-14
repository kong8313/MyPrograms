using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    public class DialerEventNotificationSenders : IDisposable
    {
        private readonly Dictionary<int, DialerEventNotificationsSender> _companyIdToSenderDictionary =
            new Dictionary<int, DialerEventNotificationsSender>();

        private readonly NotificationsSenderInitializer _notificationsSenderInitializer;

        public DialerEventNotificationSenders(NotificationsSenderInitializer notificationsSenderInitializer)
        {
            _notificationsSenderInitializer = notificationsSenderInitializer;
        }

        public DialerEventNotificationsSender GetSender(int companyId, int dialerId)
        {
            if (companyId == 0)
            {
                throw new ArgumentException("Company Id is 0 (zero)");
            }

            DialerEventNotificationsSender notificationsSender;

            lock (_companyIdToSenderDictionary)
            {
                if (!_companyIdToSenderDictionary.TryGetValue(companyId, out notificationsSender))
                {
                    // If there is no such sender - create a new one and add to the dictionary
                    notificationsSender = _notificationsSenderInitializer.InitializeIdentity(dialerId, companyId);
                    _companyIdToSenderDictionary.Add(companyId, notificationsSender);
                }
            }
            
            return notificationsSender;
        }

        public void Dispose()
        {
            lock (_companyIdToSenderDictionary)
            {
                foreach (var key in _companyIdToSenderDictionary.Keys)
                {
                    _companyIdToSenderDictionary[key].Dispose();
                    _companyIdToSenderDictionary.Remove(key);
                }
            }
        }
    }
}
