namespace Confirmit.CATI.Core.Repositories.Interfaces
{
    public interface IStartedServicesRepository
    {
        /// <summary>
        /// Add information about started service to BvStartedServices table
        /// </summary>
        /// <param name="machineName">Machine name</param>
        /// <param name="serviceName">Service name</param>
        void AddStartedServiceInfo(string machineName, string serviceName);

        /// <summary>
        /// Remove information about started service from BvStartedServices table
        /// </summary>
        /// <param name="machineName">Machine name</param>
        /// <param name="serviceName">Service name</param>
        void RemoveStartedServiceInfo(string machineName, string serviceName);
    }
}