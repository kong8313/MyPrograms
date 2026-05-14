using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.Rest.Client;

namespace Confirmit.CATI.Core.Services
{
    public class ServiceDiscoveryClientProxy : IServiceDiscoveryClientProxy
    {
        private readonly IServiceDiscoveryClient _serviceDiscoveryClient;

        public ServiceDiscoveryClientProxy()
        {
            _serviceDiscoveryClient = new ServiceDiscoveryClientFactory().Create();
        }

        public Uri GetService(string microserviceId)
        {
            return _serviceDiscoveryClient.GetServiceUriAsync(microserviceId).Result;
        }
    }
}
