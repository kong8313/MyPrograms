using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Classes.CallCenters
{
    public class CallCenterBaseForm : BaseForm
    {
        protected readonly ICallCenterRepository CallCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();
        protected readonly ICallCenterService CallCenterService = ServiceLocator.Resolve<ICallCenterService>();

        protected override void CheckSecurity()
        {
            base.CheckSecurity();

            if (BackendInstance.Current.HasCallCentersAddon == false)
            {
                throw new UserMessageException(Strings.PermissionDenied);
            }
        }

        protected void BindCallCentersList(ListControl control)
        {
            control.DataSource = CallCenterRepository.GetAllWithDialerIds();
            control.DataValueField = "Id";
            control.DataTextField = "Name";
            control.DataBind();
        }
    }
}