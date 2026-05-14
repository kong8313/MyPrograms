using System;
using System.Linq;
using System.Web;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class SupervisorToCallCenterAssignment : CallCenterAdminForm
    {
        public override string TopTitle
        {
            get { return Strings.AssignSupervisorToCallCenter; }
        }

        [StoreInViewState]
        protected string[] Supervisors;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                Supervisors = Request["ID"].Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        protected void SaveButtonClick(object sender, EventArgs e)
        {
            if (ValidateInput() == false)
            {
                return;
            }

            var callCenterId = _callCenters.SelectedCallCenterIds.Single();
            CallCenterService.AssignSupervisors(callCenterId, Supervisors);

            if (Supervisors.Contains(HttpContext.Current.User.Identity.Name))
            {
                RefreshUserSettings();
            }

            RegisterStartupScript("refreshCallCenterInfo();");

            CloseOverlay(true);
        }

        private bool ValidateInput()
        {
            if (_callCenters.SelectedCallCenterIds.Any() == false)
            {
                AddUserMessage(Strings.CallCenterSelectionForAssignWarning);
                return false;
            }

            return true;
        }
    }
}