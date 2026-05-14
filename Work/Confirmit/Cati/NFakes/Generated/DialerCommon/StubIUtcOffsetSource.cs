using System;
using DialerCommon.Logging;

namespace DialerCommon.Logging.Fakes
{
    public class StubIUtcOffsetSource : IUtcOffsetSource 
    {
        private IUtcOffsetSource _inner;

        public StubIUtcOffsetSource()
        {
            _inner = null;
        }

        public IUtcOffsetSource Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate TimeSpan GetDelegate();
        public GetDelegate Get;

        TimeSpan IUtcOffsetSource.Get()
        {


            if (Get != null)
            {
                return Get();
            } else if (_inner != null)
            {
                return ((IUtcOffsetSource)_inner).Get();
            }

            return default(TimeSpan);
        }

    }
}