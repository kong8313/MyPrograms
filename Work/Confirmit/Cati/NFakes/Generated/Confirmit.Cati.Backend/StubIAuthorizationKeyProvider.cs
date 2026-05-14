using System;
using Confirmit.CATI.Backend.WebApiServices;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIAuthorizationKeyProvider : IAuthorizationKeyProvider 
    {
        private IAuthorizationKeyProvider _inner;

        public StubIAuthorizationKeyProvider()
        {
            _inner = null;
        }

        public IAuthorizationKeyProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetKeyDelegate();
        public GetKeyDelegate GetKey;

        string IAuthorizationKeyProvider.GetKey()
        {


            if (GetKey != null)
            {
                return GetKey();
            } else if (_inner != null)
            {
                return ((IAuthorizationKeyProvider)_inner).GetKey();
            }

            return default(string);
        }

    }
}