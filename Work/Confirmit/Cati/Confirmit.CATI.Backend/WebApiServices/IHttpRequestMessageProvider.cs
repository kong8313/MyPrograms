using System.Net.Http;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface IHttpRequestMessageProvider
    {
        HttpRequestMessage GetRequest();
    }
}