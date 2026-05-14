using System;
using Confirmit.CATI.Core.AsynchronousTrigger.Notifications;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Notifications.Fakes
{
    public class StubIAsyncTriggerNotificationSender : IAsyncTriggerNotificationSender 
    {
        private IAsyncTriggerNotificationSender _inner;

        public StubIAsyncTriggerNotificationSender()
        {
            _inner = null;
        }

        public IAsyncTriggerNotificationSender Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        void IAsyncTriggerNotificationSender.Send<T>()
        {

        }

        void IAsyncTriggerNotificationSender.Send<T>(string body)
        {

        }

    }
}