using System.Collections;

namespace Confirmit.CATI.Supervisor.Core.AccessToken
{
    public interface IAccessTokenService
    {
        string GetAccessToken();
        string GetAccessToken(IDictionary httpContextItems);
        void SetAccessToken(string accessToken);
        void SetAccessToken(IDictionary httpContextItems, string accessToken);
    }
}