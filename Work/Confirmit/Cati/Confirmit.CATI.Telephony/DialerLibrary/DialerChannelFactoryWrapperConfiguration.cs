using System.Collections.Generic;
using System.ServiceModel.Description;

using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;

namespace Confirmit.CATI.Telephony
{
    public class DialerChannelFactoryWrapperConfiguration : IChannelFactoryWrapperConfiguration
    {
        private readonly string _endpointConfigurationName;
        private readonly string _endpointUri;

        private readonly string _authorizationKeyForOutgoingRequests;

        public DialerChannelFactoryWrapperConfiguration(
            string endpointName,
            string endpointUri,
            string authorizationKeyForOutgoingRequests,
            bool keepAliveEnabled)
        {
            _endpointConfigurationName = endpointName;
            _endpointUri = endpointUri;
            _authorizationKeyForOutgoingRequests = authorizationKeyForOutgoingRequests;
            KeepAliveEnabled = keepAliveEnabled;
        }

        public string EndpointConfigurationName
        {
            get
            {
                return _endpointConfigurationName;
            }
        }

        public string AdjustEndpointUri(string originalUrl)
        {
            return _endpointUri;
        }

        public bool UseLogicalAddressReplacementForHttps()
        {
            return false;
        }

        public IEnumerable<IEndpointBehavior> EndpointBehaviors
        {
            get
            {
                return new[]
                {
                    new AuthorizationMessageHeaderBehavior("", _authorizationKeyForOutgoingRequests)
                };
            }
        }

        public CachingStrategy CachingStrategy
        {
            get
            {
                return CachingStrategy.Factory;
            }
        }

        public bool LogAllCalls
        {
            get
            {
                return false;
            }
        }

        public bool LogExceptions
        {
            get
            {
                return false;
            }
        }

        public bool KeepAliveEnabled { get; }

        public void InitializeClientCredentials(ClientCredentials credentials)
        {
        }
    }
}