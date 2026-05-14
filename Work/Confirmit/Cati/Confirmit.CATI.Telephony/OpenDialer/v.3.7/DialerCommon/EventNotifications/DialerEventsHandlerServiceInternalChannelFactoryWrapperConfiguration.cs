using System.Collections.Generic;
using System.ServiceModel.Description;

using Confirmit.CATI.Common.SideBySide;
using Confirmit.CATI.Common.WcfTools;

namespace Confirmit.CATI.Telephony.DialerCommon.EventNotifications
{
    /// <summary>
    /// ChannelFactoryWrapper configuration for DialerEventsHandler service.
    /// </summary>
    public class DialerEventsHandlerServiceInternalChannelFactoryWrapperConfiguration : IChannelFactoryWrapperConfiguration
    {
        private readonly int _companyId;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialerEventsHandlerServiceInternalChannelFactoryWrapperConfiguration"/> class.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        public DialerEventsHandlerServiceInternalChannelFactoryWrapperConfiguration(int companyId)
        {
            _companyId = companyId;
        }

        /// <summary>
        /// Gets the name of the endpoint configuration (in config file).
        /// </summary>
        public string EndpointConfigurationName
        {
            get
            {
                return "DialerEventsHandlerServiceInternal";
            }
        }

        /// <summary>
        /// Adjusts the endpoint URL.
        /// </summary>
        /// <param name="originalUrl">The original URL.</param>
        /// <returns>Adjusted URL.</returns>
        public string AdjustEndpointUri(string originalUrl)
        {
            originalUrl = originalUrl.Replace("_companyid_", _companyId.ToString());
            return new SideBySideManager().AddSideBySideNameToBackendWCFServiceUrl(originalUrl) + _companyId;
        }

        public bool UseLogicalAddressReplacementForHttps()
        {
            return true;
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
                return CachingStrategy.Factory;
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
                // Exceptions are logged in DialerEventNotificationsSender.SendEventNotificationSynchronously together with other information.
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