using System;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerType : IDialerType 
    {
        private IDialerType _inner;

        public StubIDialerType()
        {
            _inner = null;
        }

        public IDialerType Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        T IDialerType.CreateInstance<T>()
        {


            return default(T);
        }

    }
}