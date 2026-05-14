using System.Threading;

namespace Confirmit.CATI.Common.Health
{
    public static class HealthCheckHandler
    {
        private static bool _isHealthy = true;

        /// <summary>
        /// Sets the unhealthy status of the system
        /// </summary>
        public static void SetUnhealthy()
        {
            _isHealthy = false;
        }

        /// <summary>
        /// Sets the healthy status of the system
        /// </summary>
        public static void SetHealthy()
        {
            _isHealthy = true;
        }

        /// <summary>
        /// Checks and returns the current health status of the system.
        /// </summary>
        /// <returns>
        /// A boolean value indicating the health status. True if the system is healthy, false otherwise.
        /// </returns>
        public static bool IsHealthy()
        {
            return _isHealthy;
        }
    }
}