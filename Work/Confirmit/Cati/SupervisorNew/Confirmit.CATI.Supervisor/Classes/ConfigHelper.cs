using System;
using System.Web;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.Configuration;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class ConfigHelper
    {
        /// <summary>
        /// Gets confirmit keepsession.aspx page url.
        /// </summary>
        public static string ConfirmitKeepSessionAspxUrl
        {
            get
            {
                return UrlHelper.ModifyUrlProtocol(Config.ConfirmitKeepSessionAspxUrl);
            }
        }

        public static bool IsConnectionSecure(HttpRequest request)
        {
            if (BootstrapConfig.IsContainerEnvironment)
            {
                var xForwardedProto = request.Headers["X-Forwarded-Proto"];
                bool forwardedHttps = xForwardedProto?.Equals("https", StringComparison.OrdinalIgnoreCase) ?? false;
                return request.IsSecureConnection || forwardedHttps;
            }

            if (ConfirmitConfiguration.SslAcceleratorMode && ConfirmitConfiguration.SslAcceleratorPort == 80)
            {
                return true;
            }

            return request.IsSecureConnection || request.Url.Port == Config.SSLPort;
        }
    }
}
