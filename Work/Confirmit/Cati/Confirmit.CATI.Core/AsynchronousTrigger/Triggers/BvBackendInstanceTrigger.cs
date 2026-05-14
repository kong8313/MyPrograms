using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.InstanceRegistrator;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Triggers
{
    /// <summary>
    /// Needed to create/remove NT service (aka instance) on "2nd" server when NT service (aka instance) registered on "1st" server.
    /// </summary>
    internal class BvBackendInstanceTrigger : IAsynchronousTrigger
    {
        public string TrigerName
        {
            get
            {
                return this.TableName + " Asynchronous Trigger";
            }
        }

        public string TableName
        {
            get
            {
                return "BvBackendInstance";
            }
        }

        public void Initialize()
        {
        }

        public void Uninitialize()
        {
        }

        public void OnTableChanged(TriggerMessage triggerMessage)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
            {
                BackendInstanceRegistrator.ResynchronizeLocalServicesWithDatabase();
            }
        }
    }
}
