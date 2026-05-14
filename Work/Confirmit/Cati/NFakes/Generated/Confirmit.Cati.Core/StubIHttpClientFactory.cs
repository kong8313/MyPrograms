using System;
using Confirmit.CATI.Core.Misc;
using System.Net.Http;

namespace Confirmit.CATI.Core.Misc.Fakes
{
    public class StubIHttpClientFactory : IHttpClientFactory 
    {
        private IHttpClientFactory _inner;

        public StubIHttpClientFactory()
        {
            _inner = null;
        }

        public IHttpClientFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate HttpClientHandler GetClientHandlerDelegate();
        public GetClientHandlerDelegate GetClientHandler;

        HttpClientHandler IHttpClientFactory.GetClientHandler()
        {


            if (GetClientHandler != null)
            {
                return GetClientHandler();
            } else if (_inner != null)
            {
                return ((IHttpClientFactory)_inner).GetClientHandler();
            }

            return default(HttpClientHandler);
        }

        public delegate HttpClient GetDelegate();
        public GetDelegate Get;

        HttpClient IHttpClientFactory.Get()
        {


            if (Get != null)
            {
                return Get();
            } else if (_inner != null)
            {
                return ((IHttpClientFactory)_inner).Get();
            }

            return default(HttpClient);
        }

    }
}