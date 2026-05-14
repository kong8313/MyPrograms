using System;

namespace Confirmit.CATI.Common.SideBySide
{
    public class SideBySideManager : ISideBySideManager
    {
        /// <summary>
        /// Instance name for the current instance
        /// </summary>
        public string SideBySideName
        {
            get
            {
                return SideBySide.SideBySideName;
            }

            set
            {
                SideBySide.SideBySideName = value;
            }
        }

        /// <summary>
        /// Service prefix
        /// </summary>
        public string ServicePrefix
        {
            get
            {
                return "Confirmit.CATI.Backend." + SideBySide.SideBySideName + "$";
            }
        }

        public string AddSideBySideNameToBackendWCFServiceUrl(string url)
        {
            var uriBuilder = new UriBuilder(url);

            uriBuilder.Path = "/" + SideBySide.SideBySideName + uriBuilder.Path;

            return uriBuilder.Uri.ToString();
        }

        public string AddSideBySideNameToServiceName(string serviceName)
        {
            int positionOfDollar = serviceName.IndexOf("$", StringComparison.Ordinal);

            string newServiceName = serviceName.Substring(0, positionOfDollar) + "." + SideBySide.SideBySideName + serviceName.Substring(positionOfDollar);

            return newServiceName;
        }

        public string RemoveSideBySideNameFromServiceName(string serviceName)
        {
            return serviceName.Replace("." + SideBySide.SideBySideName, "");
        }

        public string AddSideBySideNameToIISServiceUrl(string url)
        {
            int positionOfLastSlash = url.LastIndexOf("/", StringComparison.Ordinal);

            string newDialerServiceName = url.Substring(0, positionOfLastSlash) + "." + SideBySide.SideBySideName + url.Substring(positionOfLastSlash);

            return newDialerServiceName;
        }
    }
}
