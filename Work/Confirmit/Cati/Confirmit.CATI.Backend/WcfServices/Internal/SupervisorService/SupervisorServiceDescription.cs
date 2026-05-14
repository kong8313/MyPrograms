using System;

using Confirmit.CATI.Backend.Properties;
using Confirmit.CATI.Core.SupervisorService;

namespace Confirmit.CATI.Backend.WcfServices.Internal.SupervisorService
{
    internal class SupervisorServiceDescription : IWcfServiceDescription
    {
        private readonly int instanceId;

        public SupervisorServiceDescription(int instanceId)
        {
            this.instanceId = instanceId;
        }

        public string ServiceName => "Supervisor Service";

        public string Uri => Settings.Default.SupervisorServiceBaseAddress + this.instanceId;

        public Type ServiceType => typeof(SupervisorService);

        public bool IsExternal => false;

        public bool RequireSchemaIndependentEndpointAddress => false;
        
        public bool IsInternalHttpOnly  => false;
    }
}
