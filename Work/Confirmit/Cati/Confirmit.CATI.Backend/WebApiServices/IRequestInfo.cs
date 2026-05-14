using System.Net.Http;
using Microsoft.Owin;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public interface IRequestInfo
    {
        IOwinRequest GetOwinRequest(HttpRequestMessage request);
        string GetRequestInfo(IOwinRequest request);
        bool IsKubeProbeOrMetricsRequest(IOwinRequest request);
    }
}