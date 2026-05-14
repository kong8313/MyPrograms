using Confirmit.CATI.Core.WcfServices.MessageHeaders;

namespace Confirmit.CATI.Core.Misc.CP
{
    public class BackendSupervisorNameProvider : ISupervisorNameProvider
    {

        public string Name
        {
            get { return SupervisorMessageHeaderInspector.GetIncomingMessageSupervisor(); }
        }
    }
}