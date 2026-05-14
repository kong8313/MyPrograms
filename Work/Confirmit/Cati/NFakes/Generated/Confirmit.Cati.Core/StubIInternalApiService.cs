using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInternalApiService : IInternalApiService 
    {
        private IInternalApiService _inner;

        public StubIInternalApiService()
        {
            _inner = null;
        }

        public IInternalApiService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetTrustedTokenStringDelegate(string scopes);
        public GetTrustedTokenStringDelegate GetTrustedTokenString;

        string IInternalApiService.GetTrustedToken(string scopes)
        {


            if (GetTrustedTokenString != null)
            {
                return GetTrustedTokenString(scopes);
            } else if (_inner != null)
            {
                return ((IInternalApiService)_inner).GetTrustedToken(scopes);
            }

            return default(string);
        }

    }
}