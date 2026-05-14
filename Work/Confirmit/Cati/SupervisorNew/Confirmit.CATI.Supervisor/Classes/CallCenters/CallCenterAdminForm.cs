using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.CallCenters
{
    public class CallCenterAdminForm : CallCenterBaseForm
    {
        protected override void CheckSecurity()
        {
            base.CheckSecurity();

            if (!SupervisorPrincipal.Current.IsCatiAdministratorOrPros && !SupervisorPrincipal.Current.IsSystemProjectAdministrator)
            {
                throw new UserMessageException(Strings.PermissionDenied);
            }
        }
    }
}