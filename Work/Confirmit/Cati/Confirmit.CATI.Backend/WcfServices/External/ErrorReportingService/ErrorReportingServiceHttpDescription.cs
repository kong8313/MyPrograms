using System;

using Confirmit.CATI.Backend.Properties;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;

namespace Confirmit.CATI.Backend.WcfServices.External.ErrorReportingService
{
    /// <summary>
    /// HTTP endpoint description for Error Reporting Service (for internal communication in container environment)
    /// </summary>
    internal class ErrorReportingServiceHttpDescription : IWcfServiceDescription
    {
        public string ServiceName => "Error Reporting Service (HTTP)";

        public string Uri => Settings.Default.ErrorReportingInternalServiceBaseAddress;

        public Type ServiceType => typeof(ErrorReportingServiceHttp);
        
        public bool IsExternal => false;

        public bool RequireSchemaIndependentEndpointAddress => false;
        
        public bool IsInternalHttpOnly  => true;
    }
}
