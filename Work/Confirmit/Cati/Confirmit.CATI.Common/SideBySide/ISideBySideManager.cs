namespace Confirmit.CATI.Common.SideBySide
{
    public interface ISideBySideManager
    {
        /// <summary>
        /// Instance name for the current instance
        /// Change in for tests only
        /// </summary>
        string SideBySideName { get; set; }

        /// <summary>
        /// Service prefix
        /// </summary>
        string ServicePrefix { get; }

        /// <summary>
        /// Add SideBySideName to url
        /// </summary>
        /// <param name="url">WCF url</param>
        /// <returns></returns>
        string AddSideBySideNameToBackendWCFServiceUrl(string url);

        /// <summary>
        /// Add SideBySideName to service name
        /// </summary>
        /// <param name="serviceName">Name of service</param>
        /// <returns></returns>
        string AddSideBySideNameToServiceName(string serviceName);

        /// <summary>
        /// Remove SideBySideName to service name
        /// </summary>
        /// <param name="serviceName">Name of service</param>
        /// <returns></returns>
        string RemoveSideBySideNameFromServiceName(string serviceName);

        /// <summary>
        /// Add SideBySideName to dialer service url
        /// ex.:
        /// before: http://localhost/GenericDialerService/DialerService.svc
        /// after:  http://localhost/GenericDialerService.Main/DialerService.svc
        /// </summary>
        /// <param name="url">WCF url</param>
        /// <returns></returns>
        string AddSideBySideNameToIISServiceUrl(string url);
    }
}