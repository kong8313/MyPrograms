using System;
using Confirmit.CATI.Telephony;

namespace Confirmit.CATI.Telephony.Fakes
{
    public class StubILoggerHealthChecker : ILoggerHealthChecker 
    {
        private ILoggerHealthChecker _inner;

        public StubILoggerHealthChecker()
        {
            _inner = null;
        }

        public ILoggerHealthChecker Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ResetDelegate();
        public ResetDelegate Reset;

        void ILoggerHealthChecker.Reset()
        {

            if (Reset != null)
            {
                Reset();
            } else if (_inner != null)
            {
                ((ILoggerHealthChecker)_inner).Reset();
            }
        }

        public delegate void CheckInt32Int32Delegate(int companyId, int dialerId);
        public CheckInt32Int32Delegate CheckInt32Int32;

        void ILoggerHealthChecker.Check(int companyId, int dialerId)
        {

            if (CheckInt32Int32 != null)
            {
                CheckInt32Int32(companyId, dialerId);
            } else if (_inner != null)
            {
                ((ILoggerHealthChecker)_inner).Check(companyId, dialerId);
            }
        }

        public delegate void ForcedCheckInt32Int32Delegate(int companyId, int dialerId);
        public ForcedCheckInt32Int32Delegate ForcedCheckInt32Int32;

        void ILoggerHealthChecker.ForcedCheck(int companyId, int dialerId)
        {

            if (ForcedCheckInt32Int32 != null)
            {
                ForcedCheckInt32Int32(companyId, dialerId);
            } else if (_inner != null)
            {
                ((ILoggerHealthChecker)_inner).ForcedCheck(companyId, dialerId);
            }
        }

    }
}