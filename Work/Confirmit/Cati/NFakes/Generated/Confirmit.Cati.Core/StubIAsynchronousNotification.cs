using System;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.AsynchronousTrigger;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Fakes
{
    public class StubIAsynchronousNotification : IAsynchronousNotification 
    {
        private IAsynchronousNotification _inner;

        public StubIAsynchronousNotification()
        {
            _inner = null;
        }

        public IAsynchronousNotification Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnNotificationNotificationMessageDelegate(NotificationMessage message);
        public OnNotificationNotificationMessageDelegate OnNotificationNotificationMessage;

        void IAsynchronousNotification.OnNotification(NotificationMessage message)
        {

            if (OnNotificationNotificationMessage != null)
            {
                OnNotificationNotificationMessage(message);
            } else if (_inner != null)
            {
                ((IAsynchronousNotification)_inner).OnNotification(message);
            }
        }

    }
}