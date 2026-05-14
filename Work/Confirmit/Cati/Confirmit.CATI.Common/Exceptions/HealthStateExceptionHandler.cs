using System;
using System.Diagnostics;
using Confirmit.CATI.Common.Health;
using Confirmit.CATI.Common.Types;

namespace Confirmit.CATI.Common.Exceptions
{
    public static class HealthStateExceptionHandler<T> where T : Exception
    {
        private const int Size = 3;
        private static readonly FixedSizedQueue<DateTime> _exceptionQueue = new FixedSizedQueue<DateTime>(Size);

        public static void OnException(Exception exception)
        {
            var innerException = exception;
            while (!(innerException is T))
            {
                innerException = innerException?.InnerException;
                if (innerException == null) return;
            }
            if (!HealthCheckHandler.IsHealthy()) return;

            var currentTime = DateTime.UtcNow;
            _exceptionQueue.Enqueue(currentTime);

            if (_exceptionQueue.Count() >= Size && currentTime.Subtract(_exceptionQueue.TryPeek()).TotalMinutes <= 5)
            {
                HealthCheckHandler.SetUnhealthy();
                Trace.TraceWarning($"Exception of type {typeof(T)} occurred. Setting the API as unhealthy");
            }
        }
    }
}