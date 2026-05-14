using System;

using Confirmit.CATI.Backend.Properties;
using Confirmit.CATI.Telephony.DialerCommon;

namespace Confirmit.CATI.Backend.WcfServices.External.DialerEventsHandlerService
{
    /// <summary>
    /// HTTP endpoint description for Dialer Events Service (for internal communication in container environment)
    /// </summary>
    internal class DialerEventsServiceHttpDescription : IWcfServiceDescription
    {
        private readonly int _instanceId;

        public DialerEventsServiceHttpDescription(int instanceId)
        {
            _instanceId = instanceId;
        }

        public string ServiceName => "Dialer Events Service (HTTP)";

        public string Uri => Settings.Default.DialerEventsHandlerInternalServiceBaseAddress.Replace("{companyId}", _instanceId.ToString()) + _instanceId;

        public Type ServiceType => typeof(DialerEventsHandlerServiceHttp);
        
        public bool IsExternal => false;

        public bool RequireSchemaIndependentEndpointAddress => false;
        
        public bool IsInternalHttpOnly  => true;
    }
}
