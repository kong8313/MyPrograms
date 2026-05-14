using System.Net.Http;

namespace Confirmit.CATI.Backend.WebApiServices
{
    // TODO: http://www.lybecker.com/blog/2013/06/26/accessing-http-request-from-asp-net-web-api/
    //       try to avoid using HttpRequestMessageProvider at all if possible
    public class HttpRequestMessageProvider : IHttpRequestMessageProvider
    {
        private readonly HttpRequestMessage _request;

        public HttpRequestMessageProvider(HttpRequestMessage request)
        {
            _request = request;
        }

        public HttpRequestMessage GetRequest()
        {
            return _request;
        }
    }
}