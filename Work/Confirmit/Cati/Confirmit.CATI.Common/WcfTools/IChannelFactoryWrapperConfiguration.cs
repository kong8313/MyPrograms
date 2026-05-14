using System.Collections.Generic;
using System.ServiceModel.Description;

namespace Confirmit.CATI.Common.WcfTools
{
    /// <summary>
    /// Implement to provide configuration for ChannelFactoryWrapper.
    /// </summary>
    public interface IChannelFactoryWrapperConfiguration
    {
        /// <summary>
        /// Gets the name of the endpoint configuration (in config file).
        /// </summary>
        string EndpointConfigurationName { get; }

        /// <summary>
        /// Adjusts the endpoint URL.
        /// </summary>
        /// <param name="originalUrl">The original URL.</param>
        /// <returns>Adjusted URL.</returns>
        string AdjustEndpointUri(string originalUrl);

        /// <summary>
        /// If true, logical address will be replaced with something like new Uri("urn://" + listeningUri.Host + listeningUri.PathAndQuery)
        /// See ChannelFactoryWrapper.CreateNewChannelFactory() for details.
        /// </summary>
        /// <returns></returns>
        bool UseLogicalAddressReplacementForHttps();

        /// <summary>
        /// Gets the endpoint behaviors to add.
        /// <see langword="null"/> means do not add any behaviors.
        /// </summary>
        IEnumerable<IEndpointBehavior> EndpointBehaviors { get; }

        /// <summary>
        /// Gets the caching strategy used by the channel factory wrapper.
        /// </summary>
        CachingStrategy CachingStrategy { get; }

        /// <summary>
        /// Gets a value indicating whether all method calls and their timings should be logged.
        /// </summary>
        bool LogAllCalls { get; }

        /// <summary>
        /// Gets a value indicating whether all exceptions thrown during method calls execution should be logged.
        /// </summary>
        bool LogExceptions { get; }

        bool KeepAliveEnabled { get; }

        /// <summary>
        /// Initializes the client credentials (login / password).
        /// </summary>
        /// <param name="credentials">The client credentials object to set.</param>
        void InitializeClientCredentials(ClientCredentials credentials);
    }
}