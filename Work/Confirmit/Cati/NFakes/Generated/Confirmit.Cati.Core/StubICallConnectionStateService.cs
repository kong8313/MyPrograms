using System;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubICallConnectionStateService : ICallConnectionStateService 
    {
        private ICallConnectionStateService _inner;

        public StubICallConnectionStateService()
        {
            _inner = null;
        }

        public ICallConnectionStateService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void SetCallConnectionDisconnectedInt32Delegate(int personSid);
        public SetCallConnectionDisconnectedInt32Delegate SetCallConnectionDisconnectedInt32;

        void ICallConnectionStateService.SetCallConnectionDisconnected(int personSid)
        {

            if (SetCallConnectionDisconnectedInt32 != null)
            {
                SetCallConnectionDisconnectedInt32(personSid);
            } else if (_inner != null)
            {
                ((ICallConnectionStateService)_inner).SetCallConnectionDisconnected(personSid);
            }
        }

    }
}