using System.Net.Http;

namespace Confirmit.CATI.Core.Misc
{
    /// <summary>
    /// See http://www.nimaara.com/2016/11/01/beware-of-the-net-httpclient/
    /// </summary>
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly HttpClientHandler _httpClientHandler = new HttpClientHandler();

        public HttpClientHandler GetClientHandler()
        {
            return _httpClientHandler;
        }

        public HttpClient Get()
        {
            return new HttpClient(_httpClientHandler, false);
        }
    }
}
