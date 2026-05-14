using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#pragma warning disable 0420

namespace Confirmit.CATI.Core.DAL.Framework
{
    public class CachePerformance
    {
        public static volatile int cacheHit = 0;
        public static volatile int cacheMiss = 0;
        public static volatile int cacheReRead = 0;

        public static void OnCacheHit()
        {
            Interlocked.Increment(ref cacheHit);
            CustomMetrics.OnCacheHit();
        }

        public static void OnCacheMiss()
        {
            Interlocked.Increment(ref cacheMiss);
            CustomMetrics.OnCacheMiss();
        }

        public static void OnCacheReRead()
        {
            Interlocked.Increment(ref cacheReRead);
            CustomMetrics.OnCacheReread();
        }
    }
}

#pragma warning restore 0420