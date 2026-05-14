using System;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerStateOperationsLock : IDialerStateOperationsLock 
    {
        private IDialerStateOperationsLock _inner;

        public StubIDialerStateOperationsLock()
        {
            _inner = null;
        }

        public IDialerStateOperationsLock Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Object GetLockObjectDelegate();
        public GetLockObjectDelegate GetLockObject;

        Object IDialerStateOperationsLock.GetLockObject()
        {


            if (GetLockObject != null)
            {
                return GetLockObject();
            } else if (_inner != null)
            {
                return ((IDialerStateOperationsLock)_inner).GetLockObject();
            }

            return default(Object);
        }

    }
}