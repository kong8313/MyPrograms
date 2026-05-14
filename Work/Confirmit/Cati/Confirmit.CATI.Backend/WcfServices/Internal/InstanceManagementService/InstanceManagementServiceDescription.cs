using System;

using Confirmit.CATI.Backend.Properties;
using Confirmit.CATI.Core.ManagementService;

namespace Confirmit.CATI.Backend.WcfServices.Internal.InstanceManagementService
{
    /// <summary>
    /// Provides description for the Instance Management Wcf Service.
    /// </summary>
    internal class InstanceManagementServiceDescription : IWcfServiceDescription
    {
        public string ServiceName => "Instance Management Service";

        public string Uri => Settings.Default.InstanceManagementServiceBaseAddress;

        public Type ServiceType => typeof(InstanceManagementService);

        public bool IsExternal => false;

        public bool RequireSchemaIndependentEndpointAddress => false;
        
        public bool IsInternalHttpOnly  => false;
    }
}
