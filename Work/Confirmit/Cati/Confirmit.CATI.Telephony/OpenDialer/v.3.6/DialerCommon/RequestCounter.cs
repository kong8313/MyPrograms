using System.Threading;

namespace DialerCommon
{
    public class RequestCounter
    {
        private int counter = 0;

        public int Increment()
        {
            return Interlocked.Increment(ref counter);
        }

        public int Decrement()
        {
            return Interlocked.Decrement(ref counter);
        }
    }
}
