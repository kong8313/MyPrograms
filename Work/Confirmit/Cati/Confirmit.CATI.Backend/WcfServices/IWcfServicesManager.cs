using System.Collections.Generic;

namespace Confirmit.CATI.Backend.WcfServices
{
    internal interface IWcfServicesManager
    {
        /// <summary>
        /// Creates, initializes and publishes ServiceHost for all Wcf services provided in the constructor.
        /// </summary>
        /// <param name="serviceDescriptions">
        /// The service descriptions to start.
        /// </param>
        void Start(IEnumerable<IWcfServiceDescription> serviceDescriptions);

        /// <summary>
        /// Closes all started wcf services.
        /// </summary>
        void Stop();
    }
}