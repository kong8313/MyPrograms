using System;

using Confirmit.CATI.Backend.Properties;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Backend.WcfServices.Internal.ManagementService
{
    internal class ManagementServiceDescription : IWcfServiceDescription
    {
        private readonly int instanceId;

        public ManagementServiceDescription(int instanceId)
        {
            this.instanceId = instanceId;
        }

        public string ServiceName => "Management Service";

        public string Uri => Settings.Default.ManagementServiceBaseAddress + this.instanceId;

        public Type ServiceType => typeof(ManagementService);

        public bool IsExternal => false;

        public bool RequireSchemaIndependentEndpointAddress => false;
        
        public bool IsInternalHttpOnly  => false;
    }
}
