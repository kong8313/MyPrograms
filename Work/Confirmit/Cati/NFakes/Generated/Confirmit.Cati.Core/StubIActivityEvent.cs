using System;
using Confirmit.CATI.Core.ActivityLogging;

namespace Confirmit.CATI.Core.ActivityLogging.Fakes
{
    public class StubIActivityEvent : IActivityEvent 
    {
        private IActivityEvent _inner;

        public StubIActivityEvent()
        {
            _inner = null;
        }

        public IActivityEvent Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void FinishDelegate();
        public FinishDelegate Finish;

        void IActivityEvent.Finish()
        {

            if (Finish != null)
            {
                Finish();
            } else if (_inner != null)
            {
                ((IActivityEvent)_inner).Finish();
            }
        }

        public delegate void SaveDelegate();
        public SaveDelegate Save;

        void IActivityEvent.Save()
        {

            if (Save != null)
            {
                Save();
            } else if (_inner != null)
            {
                ((IActivityEvent)_inner).Save();
            }
        }

        public delegate bool IsRunningDelegate();
        public IsRunningDelegate IsRunning;

        bool IActivityEvent.IsRunning()
        {


            if (IsRunning != null)
            {
                return IsRunning();
            } else if (_inner != null)
            {
                return ((IActivityEvent)_inner).IsRunning();
            }

            return default(bool);
        }

    }
}