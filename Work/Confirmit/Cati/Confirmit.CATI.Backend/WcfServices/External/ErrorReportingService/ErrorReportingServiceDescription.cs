using System;

using Confirmit.CATI.Backend.Properties;

namespace Confirmit.CATI.Backend.WcfServices.External.ErrorReportingService
{
    internal class ErrorReportingServiceDescription : IWcfServiceDescription
    {
        public string ServiceName => "Error Reporting Service";

        public string Uri => Settings.Default.ErrorReportingServiceBaseAddress;

        public Type ServiceType => typeof(ErrorReportingService);

        public bool IsExternal => true;

        public bool RequireSchemaIndependentEndpointAddress => true;
        
        public bool IsInternalHttpOnly  => false;
    }
}
