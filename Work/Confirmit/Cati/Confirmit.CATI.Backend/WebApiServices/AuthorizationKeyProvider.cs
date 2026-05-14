using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Backend.WebApiServices.Authorization;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class AuthorizationKeyProvider : IAuthorizationKeyProvider
    {
        public const string XConfirmitApiKey = "X-Confirmit-ApiKey";

        private readonly IHttpRequestMessageProvider _requestMessageProvider;

        public AuthorizationKeyProvider(IHttpRequestMessageProvider requestMessageProvider)
        {
            _requestMessageProvider = requestMessageProvider;
        }

        public string GetKey()
        {
            IEnumerable<string> keys;

            var keyAvailable = _requestMessageProvider.GetRequest().Headers.TryGetValues(XConfirmitApiKey, out keys);

            if (keyAvailable)
            {
                var keysArray = keys.ToArray();

                if (keysArray.Length == 1)
                {
                    return keysArray[0];
                }
            }

            throw new AuthenticateException(string.Format("Header {0} must be set", XConfirmitApiKey));
        }
    }
}