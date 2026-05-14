using System;
using Confirmit.CATI.Supervisor.Core.AccessToken;
using System.Collections;

namespace Confirmit.CATI.Supervisor.Core.AccessToken.Fakes
{
    public class StubIAccessTokenService : IAccessTokenService 
    {
        private IAccessTokenService _inner;

        public StubIAccessTokenService()
        {
            _inner = null;
        }

        public IAccessTokenService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetAccessTokenDelegate();
        public GetAccessTokenDelegate GetAccessToken;

        string IAccessTokenService.GetAccessToken()
        {


            if (GetAccessToken != null)
            {
                return GetAccessToken();
            } else if (_inner != null)
            {
                return ((IAccessTokenService)_inner).GetAccessToken();
            }

            return default(string);
        }

        public delegate string GetAccessTokenIDictionaryDelegate(IDictionary httpContextItems);
        public GetAccessTokenIDictionaryDelegate GetAccessTokenIDictionary;

        string IAccessTokenService.GetAccessToken(IDictionary httpContextItems)
        {


            if (GetAccessTokenIDictionary != null)
            {
                return GetAccessTokenIDictionary(httpContextItems);
            } else if (_inner != null)
            {
                return ((IAccessTokenService)_inner).GetAccessToken(httpContextItems);
            }

            return default(string);
        }

        public delegate void SetAccessTokenStringDelegate(string accessToken);
        public SetAccessTokenStringDelegate SetAccessTokenString;

        void IAccessTokenService.SetAccessToken(string accessToken)
        {

            if (SetAccessTokenString != null)
            {
                SetAccessTokenString(accessToken);
            } else if (_inner != null)
            {
                ((IAccessTokenService)_inner).SetAccessToken(accessToken);
            }
        }

        public delegate void SetAccessTokenIDictionaryStringDelegate(IDictionary httpContextItems, string accessToken);
        public SetAccessTokenIDictionaryStringDelegate SetAccessTokenIDictionaryString;

        void IAccessTokenService.SetAccessToken(IDictionary httpContextItems, string accessToken)
        {

            if (SetAccessTokenIDictionaryString != null)
            {
                SetAccessTokenIDictionaryString(httpContextItems, accessToken);
            } else if (_inner != null)
            {
                ((IAccessTokenService)_inner).SetAccessToken(httpContextItems, accessToken);
            }
        }

    }
}