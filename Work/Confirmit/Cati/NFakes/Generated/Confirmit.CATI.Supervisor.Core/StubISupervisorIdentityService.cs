using System;
using Confirmit.CATI.Supervisor.Core.Security;
using System.Threading.Tasks;
using Confirmit.Identity.Sdk.Configuration;
using Firmglobal.Framework.Security;

namespace Confirmit.CATI.Supervisor.Core.Security.Fakes
{
    public class StubISupervisorIdentityService : ISupervisorIdentityService 
    {
        private ISupervisorIdentityService _inner;

        public StubISupervisorIdentityService()
        {
            _inner = null;
        }

        public ISupervisorIdentityService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetActualAccessTokenDelegate();
        public GetActualAccessTokenDelegate GetActualAccessToken;

        string ISupervisorIdentityService.GetActualAccessToken()
        {


            if (GetActualAccessToken != null)
            {
                return GetActualAccessToken();
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityService)_inner).GetActualAccessToken();
            }

            return default(string);
        }

        public delegate Task<IdentityEndpointProvider> GetActualIdentityEndpointProviderStringDelegate(string accessToken);
        public GetActualIdentityEndpointProviderStringDelegate GetActualIdentityEndpointProviderString;

        Task<IdentityEndpointProvider> ISupervisorIdentityService.GetActualIdentityEndpointProvider(string accessToken)
        {


            if (GetActualIdentityEndpointProviderString != null)
            {
                return GetActualIdentityEndpointProviderString(accessToken);
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityService)_inner).GetActualIdentityEndpointProvider(accessToken);
            }

            return default(Task<IdentityEndpointProvider>);
        }

        public delegate Task<ConfirmitIdentity> GetConfirmitIdentityStringDelegate(string accessToken);
        public GetConfirmitIdentityStringDelegate GetConfirmitIdentityString;

        Task<ConfirmitIdentity> ISupervisorIdentityService.GetConfirmitIdentity(string accessToken)
        {


            if (GetConfirmitIdentityString != null)
            {
                return GetConfirmitIdentityString(accessToken);
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityService)_inner).GetConfirmitIdentity(accessToken);
            }

            return default(Task<ConfirmitIdentity>);
        }

        public delegate Task<string> GetAccessTokenForCatiSupervisorApiStringStringDelegate(string customGrantClientId, string customGrantClientSecret);
        public GetAccessTokenForCatiSupervisorApiStringStringDelegate GetAccessTokenForCatiSupervisorApiStringString;

        Task<string> ISupervisorIdentityService.GetAccessTokenForCatiSupervisorApi(string customGrantClientId, string customGrantClientSecret)
        {


            if (GetAccessTokenForCatiSupervisorApiStringString != null)
            {
                return GetAccessTokenForCatiSupervisorApiStringString(customGrantClientId, customGrantClientSecret);
            } else if (_inner != null)
            {
                return ((ISupervisorIdentityService)_inner).GetAccessTokenForCatiSupervisorApi(customGrantClientId, customGrantClientSecret);
            }

            return default(Task<string>);
        }

    }
}