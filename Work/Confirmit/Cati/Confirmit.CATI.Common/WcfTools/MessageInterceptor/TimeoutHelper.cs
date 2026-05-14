using System;

namespace Confirmit.CATI.Common.WcfTools.MessageInterceptor
{
    /// <summary>
    /// Helper class to calculate remaining time.
    /// </summary>
    /// <remarks>
    /// Implementation based on the MSDN sample Custom Message Interceptor:
    /// http://msdn.microsoft.com/en-us/library/ms751495.aspx
    /// </remarks>
    public class TimeoutHelper
    {
        private readonly DateTime deadline;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutHelper"/> class.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public TimeoutHelper(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }

            if (timeout == TimeSpan.MaxValue)
            {
                this.deadline = DateTime.MaxValue;
            }
            else
            {
                this.deadline = DateTime.UtcNow + timeout;
            }
        }

        /// <summary>
        /// Gets the time remainings to the deadline specified in the constructor.
        /// </summary>
        /// <returns>The time remainings to the deadline.</returns>
        public TimeSpan RemainingTime()
        {
            if (this.deadline == DateTime.MaxValue)
            {
                return TimeSpan.MaxValue;
            }

            TimeSpan remaining = this.deadline - DateTime.UtcNow;
            return remaining <= TimeSpan.Zero ? TimeSpan.Zero : remaining;
        }
    }
}