using System;

using Confirmit.CATI.Backend.Properties;
using Confirmit.CATI.Telephony.DialerCommon;

namespace Confirmit.CATI.Backend.WcfServices.External.DialerEventsHandlerService
{
    internal class DialerEventsServiceDescription : IWcfServiceDescription
    {
        private readonly int _instanceId;

        public DialerEventsServiceDescription(int instanceId)
        {
            _instanceId = instanceId;
        }

        public string ServiceName => "Dialer Events Service";

        public string Uri => Settings.Default.DialerEventsHandlerServiceBaseAddress + _instanceId;

        public Type ServiceType => typeof(DialerEventsHandlerService);

        public bool IsExternal => true;

        public bool RequireSchemaIndependentEndpointAddress => true;
        
        public bool IsInternalHttpOnly  => false;
    }
}
