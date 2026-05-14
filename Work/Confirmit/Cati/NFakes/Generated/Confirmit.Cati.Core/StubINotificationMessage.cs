using System;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Messages.Fakes
{
    public class StubINotificationMessage : INotificationMessage 
    {
        private INotificationMessage _inner;

        public StubINotificationMessage()
        {
            _inner = null;
        }

        public INotificationMessage Inner
        {
            set {_inner = value;} get {return _inner;}
        }

    }
}