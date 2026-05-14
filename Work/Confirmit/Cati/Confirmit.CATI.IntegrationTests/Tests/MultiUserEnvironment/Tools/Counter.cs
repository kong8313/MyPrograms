using System.Threading;

namespace Confirmit.CATI.IntegrationTests.Tests.MultiUserEnvironment.Tools
{
    public class Counter
    {
        private int count = 0;

        public Counter() { }

        public Counter(int count)
        {
            this.count = count;
        }

        public static implicit operator int(Counter counter)
        {
            return counter.count;
        }

        public static Counter operator +(Counter counter, int value)
        {
            Interlocked.Add(ref counter.count, value);
            return counter;
        }
    }
}