using System;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Messages.Fakes
{
    public class StubITriggerMessage : ITriggerMessage 
    {
        private ITriggerMessage _inner;

        public StubITriggerMessage()
        {
            _inner = null;
        }

        public ITriggerMessage Inner
        {
            set {_inner = value;} get {return _inner;}
        }

    }
}