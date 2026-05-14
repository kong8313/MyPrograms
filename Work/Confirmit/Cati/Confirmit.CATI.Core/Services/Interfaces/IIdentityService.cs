using System.Security.Claims;
using System.Threading.Tasks;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.Identity.Sdk.Configuration;
using Firmglobal.Framework.Security;

namespace Confirmit.CATI.Core.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<ConfirmitIdentity> GetConfirmitIdentity(string accessToken, IdentityEndpointProvider provider);
        SupervisorPrincipal CreateSupervisorPrincipalByConfirmitIdentity(ConfirmitIdentity identity);
        Task<string> GetAccessTokenForCatiSupervisorApi(string accessTokenFromIdpCookie, string customGrantClientId, string customGrantClientSecret, IdentityEndpointProvider provider);
    }
}
