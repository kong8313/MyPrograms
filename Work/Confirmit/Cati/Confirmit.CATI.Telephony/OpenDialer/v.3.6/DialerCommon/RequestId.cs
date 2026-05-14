using System;
using System.Threading;

namespace DialerCommon
{
    public class RequestId
    {
        private long id = 0;

        [ThreadStatic]
        private long _value;

        public long Value
        {
            get
            {
                return _value;
            }
        }

        public long Next()
        {
            _value = Interlocked.Increment(ref id);
            return Value;
        }
    }
}
