using System;
using Confirmit.CATI.Backend.WebApiServices.Authorization;

namespace Confirmit.CATI.Backend.WebApiServices.Authorization.Fakes
{
    public class StubIAuthorizer : IAuthorizer 
    {
        private IAuthorizer _inner;

        public StubIAuthorizer()
        {
            _inner = null;
        }

        public IAuthorizer Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void AuthorizeDelegate();
        public AuthorizeDelegate Authorize;

        void IAuthorizer.Authorize()
        {

            if (Authorize != null)
            {
                Authorize();
            } else if (_inner != null)
            {
                ((IAuthorizer)_inner).Authorize();
            }
        }

    }
}