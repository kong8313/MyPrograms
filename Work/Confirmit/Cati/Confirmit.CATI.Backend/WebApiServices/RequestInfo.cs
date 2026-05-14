using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Owin;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class RequestInfo : IRequestInfo
    {
        private string GetRequestRemoteIp(IOwinRequest owinRequest)
        {
            return owinRequest.RemoteIpAddress;
        }

        public string GetRequestInfo(IOwinRequest request)
        {
            string requestInfo =
                $"WebApi {request.Method} {request.Uri}\r\n\r\nRemote Ip: {GetRequestRemoteIp(request)}";

            return requestInfo;
        }

        public IOwinRequest GetOwinRequest(HttpRequestMessage request)
        {
            var context = (OwinContext)request.Properties["MS_OwinContext"];

            if (context != null)
            {
                return context.Request;
            }

            return null;
        }

        public bool IsKubeProbeOrMetricsRequest(IOwinRequest request)
        {
            return (request?.Headers["User-Agent"]?.StartsWith("kube-probe", StringComparison.OrdinalIgnoreCase) ?? false)
                   || (request?.Path.ToString().EndsWith("/metrics", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}
