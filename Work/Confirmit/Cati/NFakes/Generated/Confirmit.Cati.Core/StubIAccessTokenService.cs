using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
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

        public delegate string BuildUrlToGetAccessTokenStringDelegate(string returnUrl);
        public BuildUrlToGetAccessTokenStringDelegate BuildUrlToGetAccessTokenString;

        string IAccessTokenService.BuildUrlToGetAccessToken(string returnUrl)
        {


            if (BuildUrlToGetAccessTokenString != null)
            {
                return BuildUrlToGetAccessTokenString(returnUrl);
            } else if (_inner != null)
            {
                return ((IAccessTokenService)_inner).BuildUrlToGetAccessToken(returnUrl);
            }

            return default(string);
        }

    }
}