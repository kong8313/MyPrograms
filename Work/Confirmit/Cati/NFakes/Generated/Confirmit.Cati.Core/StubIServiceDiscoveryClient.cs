using System;
using Confirmit.Rest.Client;

namespace Confirmit.Rest.Client.Fakes
{
    public class StubIServiceDiscoveryClient : IServiceDiscoveryClient 
    {
        private IServiceDiscoveryClient _inner;

        public StubIServiceDiscoveryClient()
        {
            _inner = null;
        }

        public IServiceDiscoveryClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Uri IdentityServiceTokenEndpointUriDelegate();
        public IdentityServiceTokenEndpointUriDelegate IdentityServiceTokenEndpointUri;

        Uri IServiceDiscoveryClient.IdentityServiceTokenEndpointUri()
        {


            if (IdentityServiceTokenEndpointUri != null)
            {
                return IdentityServiceTokenEndpointUri();
            } else if (_inner != null)
            {
                return ((IServiceDiscoveryClient)_inner).IdentityServiceTokenEndpointUri();
            }

            return default(Uri);
        }

        public delegate Uri GetServiceStringDelegate(string serviceId);
        public GetServiceStringDelegate GetServiceString;

        Uri IServiceDiscoveryClient.GetService(string serviceId)
        {


            if (GetServiceString != null)
            {
                return GetServiceString(serviceId);
            } else if (_inner != null)
            {
                return ((IServiceDiscoveryClient)_inner).GetService(serviceId);
            }

            return default(Uri);
        }

    }
}