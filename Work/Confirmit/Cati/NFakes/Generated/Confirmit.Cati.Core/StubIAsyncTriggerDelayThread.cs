using System;
using Confirmit.CATI.Core.AsynchronousTrigger;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Fakes
{
    public class StubIAsyncTriggerDelayThread : IAsyncTriggerDelayThread 
    {
        private IAsyncTriggerDelayThread _inner;

        public StubIAsyncTriggerDelayThread()
        {
            _inner = null;
        }

        public IAsyncTriggerDelayThread Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SendDelayNotificationDelegate();
        public SendDelayNotificationDelegate SendDelayNotification;

        void IAsyncTriggerDelayThread.SendDelayNotification()
        {

            if (SendDelayNotification != null)
            {
                SendDelayNotification();
            } else if (_inner != null)
            {
                ((IAsyncTriggerDelayThread)_inner).SendDelayNotification();
            }
        }

    }
}