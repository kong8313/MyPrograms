using System.Net.Http;

namespace Confirmit.CATI.Core.Misc
{
    public interface IHttpClientFactory
    {
        HttpClientHandler GetClientHandler();
        HttpClient Get();
    }
}