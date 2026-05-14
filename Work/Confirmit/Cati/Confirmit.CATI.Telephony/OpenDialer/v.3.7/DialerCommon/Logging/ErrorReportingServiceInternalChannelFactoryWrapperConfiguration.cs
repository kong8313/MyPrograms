using System.Collections.Generic;
using System.ServiceModel.Description;
using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.WcfTools;

namespace DialerCommon.TraceListeners
{
    internal class ErrorReportingServiceInternalChannelFactoryWrapperConfiguration : IChannelFactoryWrapperConfiguration
    {                
        /// <summary>
        /// Gets the name of the endpoint configuration (in config file).
        /// </summary>
        public string EndpointConfigurationName
        {
            get
            {
                return "ErrorReportingServiceInternal";
            }
        }

        public string AdjustEndpointUri(string originalUrl)
        {
            return new SideBySideManager().AddSideBySideNameToBackendWCFServiceUrl(originalUrl);
        }

        public bool UseLogicalAddressReplacementForHttps()
        {
            return false;
        }

        /// <summary>
        /// Gets the endpoint behaviors to add.
        /// <see langword="null"/> means do not add any behaviors.
        /// </summary>
        public IEnumerable<IEndpointBehavior> EndpointBehaviors
        {
            get
            {
                return null;                    
            }
        }

        /// <summary>
        /// Gets the caching strategy used by the channel factory wrapper.
        /// </summary>
        public CachingStrategy CachingStrategy
        {
            get
            {
                return CachingStrategy.FactoryAndChannels;
            }
        }

        /// <summary>
        /// Gets a value indicating whether all method calls and their timings should be logged.
        /// </summary>
        public bool LogAllCalls
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether all exceptions thrown during method calls execution should be logged.
        /// </summary>
        public bool LogExceptions
        {
            get
            {
                return false;
            }
        }

        public bool KeepAliveEnabled => false;

        /// <summary>
        /// Initializes the client credentials (login / password).
        /// </summary>
        /// <param name="credentials">The client credentials object to set.</param>
        public void InitializeClientCredentials(ClientCredentials credentials)
        {
        }        
    }
}