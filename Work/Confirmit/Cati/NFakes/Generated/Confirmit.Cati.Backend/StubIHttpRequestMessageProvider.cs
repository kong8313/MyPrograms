using System;
using Confirmit.CATI.Backend.WebApiServices;
using System.Net.Http;

namespace Confirmit.CATI.Backend.WebApiServices.Fakes
{
    public class StubIHttpRequestMessageProvider : IHttpRequestMessageProvider 
    {
        private IHttpRequestMessageProvider _inner;

        public StubIHttpRequestMessageProvider()
        {
            _inner = null;
        }

        public IHttpRequestMessageProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate HttpRequestMessage GetRequestDelegate();
        public GetRequestDelegate GetRequest;

        HttpRequestMessage IHttpRequestMessageProvider.GetRequest()
        {


            if (GetRequest != null)
            {
                return GetRequest();
            } else if (_inner != null)
            {
                return ((IHttpRequestMessageProvider)_inner).GetRequest();
            }

            return default(HttpRequestMessage);
        }

    }
}