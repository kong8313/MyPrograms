using System.Threading;

namespace DialerCommon
{
    public class RequestId
    {
        private long id = 0;

        public long Next()
        {
            return Interlocked.Increment(ref id);
        }
    }
}
