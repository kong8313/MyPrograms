using System.Web;

namespace Confirmit.CATI.Supervisor.Classes
{
    public static class UrlHelper
    {
        /// <summary>
        /// Modifies the URL protocol according to current request protocol.
        /// </summary>
        /// <param name="url">The URL to modify.</param>
        public static string ModifyUrlProtocol(string url)
        {
            HttpRequest request = HttpContext.Current.Request;
            bool isSecure = ConfigHelper.IsConnectionSecure(request);

            return ModifyUrlProtocol(url, isSecure);
        }

        /// <summary>
        /// Modifies the URL protocol.
        /// </summary>
        /// <param name="url">The URL to modify.</param>
        /// <param name="useSSL">if set to <c>true</c> HTTPS is used.</param>
        public static string ModifyUrlProtocol(string url, bool useSSL)
        {
            string result = url;
            if (url.StartsWith("http://") && useSSL)
            {
                result = result.Replace("http://", "https://");
            }
            else if (url.StartsWith("https://") && !useSSL)
            {
                result = result.Replace("https://", "http://");
            }

            return result;
        }
    }
}
