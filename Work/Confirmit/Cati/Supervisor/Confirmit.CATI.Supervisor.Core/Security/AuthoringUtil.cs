using Azure.Core;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;

namespace Confirmit.CATI.Supervisor.Core.Security
{
    public static class AuthoringUtil
    {
        public static string GetRemoteIpAddress(this HttpRequest request)
        {
            if (request.IsLocal) return "127.0.0.1";

            string ip;
            // X-Forwarded-For is used by nginx reverse proxy
            ip = request.Headers["X-Forwarded-For"]?.Split(',').First().Trim();
            if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out _)) return ip;

            // HTTP_X_FORWARDED_FOR
            ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"]?.Split(',').First().Trim();
            if (!string.IsNullOrWhiteSpace(ip) && IPAddress.TryParse(ip, out _)) return ip;

            // REMOTE_ADDR
            ip = request.ServerVariables["REMOTE_ADDR"];
            if (!string.IsNullOrWhiteSpace(ip) && IPAddress.TryParse(ip, out _)) return ip;

            // RemoteAddress is used by IIS
            return request.UserHostAddress;
        }

        public static string GetRemoteIpAddress() => HttpContext.Current.Request.GetRemoteIpAddress();

        public static OidcManager GetOidcManager()
        {
            OidcManager oidcMan = new OidcManager(GetRemoteIpAddress());
            return oidcMan;
        }
    }
}