using Confirmit.CATI.Core.Misc.CP;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public class HttpContextSupervisorNameProvider : ISupervisorNameProvider
    {
        public string Name => SupervisorPrincipal.Current.Name;
    }
}