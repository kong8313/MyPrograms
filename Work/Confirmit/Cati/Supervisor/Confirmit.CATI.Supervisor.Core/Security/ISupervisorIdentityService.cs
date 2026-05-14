using System.Threading.Tasks;
using Confirmit.Identity.Sdk.Configuration;
using Firmglobal.Framework.Security;

namespace Confirmit.CATI.Supervisor.Core.Security
{
    public interface ISupervisorIdentityService
    {
        string GetActualAccessToken();
        Task<IdentityEndpointProvider> GetActualIdentityEndpointProvider(string accessToken);
        Task<ConfirmitIdentity> GetConfirmitIdentity(string accessToken);
        Task<string> GetAccessTokenForCatiSupervisorApi(string customGrantClientId, string customGrantClientSecret);
    }
}