using System.Collections;
using System.Web;

namespace Confirmit.CATI.Supervisor.Core.AccessToken
{
    public class AccessTokenService : IAccessTokenService
    {
	    private const string AccessTokenKeyName = "AccessToken";

		public string GetAccessToken()
        {
            return HttpContext.Current.Items[AccessTokenKeyName] as string;
        }

		public string GetAccessToken(IDictionary httpContextItems)
        {
            return httpContextItems[AccessTokenKeyName] as string;
        }

        public void SetAccessToken(string accessToken)
        {
			HttpContext.Current.Items[AccessTokenKeyName] = accessToken;
        }

        public void SetAccessToken(IDictionary httpContextItems, string accessToken)
        {
            httpContextItems[AccessTokenKeyName] = accessToken;
        }
    }
}