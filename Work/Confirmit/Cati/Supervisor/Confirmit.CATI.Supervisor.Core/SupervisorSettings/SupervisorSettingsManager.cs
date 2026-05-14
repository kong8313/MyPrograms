using Confirmit.CATI.Common.ServiceLocation;

namespace Confirmit.CATI.Supervisor.Core.SupervisorSettings
{
    /// <summary>
    /// Class responsible for operations with supervisor settings
    /// </summary>
    public static class SupervisorSettingsManager
    {
        /// <summary>
        /// Returns tables density setting for modern supervisor style
        /// </summary>
        public static string GetTableDensity()
        {
            var supervisorSettingsRepository = ServiceLocator.Resolve<ISupervisorSettingsRepository>();
            return supervisorSettingsRepository.ReadTableDensity();
        }
    }
}
