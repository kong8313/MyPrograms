using System;
using Confirmit.CATI.Supervisor.Core.Security;
using System.Threading.Tasks;
using Confirmit.Identity.Sdk.Configuration;

namespace Confirmit.CATI.Supervisor.Core.Security.Fakes
{
    public class StubISupervisorIdentityProviderService : ISupervisorIdentityProviderService 
    {
        private ISupervisorIdentityProviderService _inner;

        public StubISupervisorIdentityProviderService()
        {
            _inner = null;
        }

        public ISupervisorIdentityProviderService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<IdentityEndpointProvider> GetIdentityEndpointProviderDelegate();
        public GetIdentityEndpointProviderDelegate GetIdentityEndpointProvider;

        Task<IdentityEndpointProvider> ISupervisorIdentityProviderService.GetIdentityEndpointProvider()
        {


            if (GetIdentityEndpointProvider != null)
            {
                return GetIdentityEndpointProvider();
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityProviderService)_inner).GetIdentityEndpointProvider();
            }

            return default(Task<IdentityEndpointProvider>);
        }

        public delegate Task<IdentityEndpointProvider> GetIdentityEndpointProviderWithAddressStringDelegate(string ipAddress);
        public GetIdentityEndpointProviderWithAddressStringDelegate GetIdentityEndpointProviderWithAddressString;

        Task<IdentityEndpointProvider> ISupervisorIdentityProviderService.GetIdentityEndpointProviderWithAddress(string ipAddress)
        {


            if (GetIdentityEndpointProviderWithAddressString != null)
            {
                return GetIdentityEndpointProviderWithAddressString(ipAddress);
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityProviderService)_inner).GetIdentityEndpointProviderWithAddress(ipAddress);
            }

            return default(Task<IdentityEndpointProvider>);
        }

        public delegate IdentityEndpointProvider GetIdentityEndpointProviderIdentityVersionDelegate(IdentityVersion identityVersion);
        public GetIdentityEndpointProviderIdentityVersionDelegate GetIdentityEndpointProviderIdentityVersion;

        IdentityEndpointProvider ISupervisorIdentityProviderService.GetIdentityEndpointProvider(IdentityVersion identityVersion)
        {


            if (GetIdentityEndpointProviderIdentityVersion != null)
            {
                return GetIdentityEndpointProviderIdentityVersion(identityVersion);
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityProviderService)_inner).GetIdentityEndpointProvider(identityVersion);
            }

            return default(IdentityEndpointProvider);
        }

    }
}