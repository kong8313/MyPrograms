using System;
using Confirmit.Identity.Sdk.Configuration;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Threading.Tasks;
using Firmglobal.Framework.Security;
using Confirmit.CATI.Core.Misc.CP;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIIdentityService : IIdentityService 
    {
        private IIdentityService _inner;

        public StubIIdentityService()
        {
            _inner = null;
        }

        public IIdentityService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<ConfirmitIdentity> GetConfirmitIdentityStringIdentityEndpointProviderDelegate(string accessToken, IdentityEndpointProvider provider);
        public GetConfirmitIdentityStringIdentityEndpointProviderDelegate GetConfirmitIdentityStringIdentityEndpointProvider;

        Task<ConfirmitIdentity> IIdentityService.GetConfirmitIdentity(string accessToken, IdentityEndpointProvider provider)
        {


            if (GetConfirmitIdentityStringIdentityEndpointProvider != null)
            {
                return GetConfirmitIdentityStringIdentityEndpointProvider(accessToken, provider);
            } else if (_inner != null)
            {
                return ((IIdentityService)_inner).GetConfirmitIdentity(accessToken, provider);
            }

            return default(Task<ConfirmitIdentity>);
        }

        public delegate SupervisorPrincipal CreateSupervisorPrincipalByConfirmitIdentityConfirmitIdentityDelegate(ConfirmitIdentity identity);
        public CreateSupervisorPrincipalByConfirmitIdentityConfirmitIdentityDelegate CreateSupervisorPrincipalByConfirmitIdentityConfirmitIdentity;

        SupervisorPrincipal IIdentityService.CreateSupervisorPrincipalByConfirmitIdentity(ConfirmitIdentity identity)
        {


            if (CreateSupervisorPrincipalByConfirmitIdentityConfirmitIdentity != null)
            {
                return CreateSupervisorPrincipalByConfirmitIdentityConfirmitIdentity(identity);
            } else if (_inner != null)
            {
                return ((IIdentityService)_inner).CreateSupervisorPrincipalByConfirmitIdentity(identity);
            }

            return default(SupervisorPrincipal);
        }

        public delegate Task<string> GetAccessTokenForCatiSupervisorApiStringStringStringIdentityEndpointProviderDelegate(string accessTokenFromIdpCookie, string customGrantClientId, string customGrantClientSecret, IdentityEndpointProvider provider);
        public GetAccessTokenForCatiSupervisorApiStringStringStringIdentityEndpointProviderDelegate GetAccessTokenForCatiSupervisorApiStringStringStringIdentityEndpointProvider;

        Task<string> IIdentityService.GetAccessTokenForCatiSupervisorApi(string accessTokenFromIdpCookie, string customGrantClientId, string customGrantClientSecret, IdentityEndpointProvider provider)
        {


            if (GetAccessTokenForCatiSupervisorApiStringStringStringIdentityEndpointProvider != null)
            {
                return GetAccessTokenForCatiSupervisorApiStringStringStringIdentityEndpointProvider(accessTokenFromIdpCookie, customGrantClientId, customGrantClientSecret, provider);
            } else if (_inner != null)
            {
                return ((IIdentityService)_inner).GetAccessTokenForCatiSupervisorApi(accessTokenFromIdpCookie, customGrantClientId, customGrantClientSecret, provider);
            }

            return default(Task<string>);
        }

    }
}