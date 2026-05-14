using Confirmit.CATI.Core.CallCenters;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;
using System;
using System.Globalization;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class DeleteCallCenter : CallCenterAdminForm
    {
        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly ICallCenterService _callCenterService = ServiceLocator.Resolve<ICallCenterService>();

        [StoreInViewState]
        protected int CallCenterId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                CallCenterId = Int32.Parse(Request["ID"]);
                BindCallCentersList(_callCenters);
            }
        }

        protected void CallCentersDataBound(object sender, EventArgs e)
        {
            var dropDown = (DropDownList)sender;

            if (CallCenterRepository.Default.ID != CallCenterId)
            {
                var currentCallCenter = dropDown.Items.FindByValue(CallCenterId.ToString(CultureInfo.InvariantCulture));
                if (currentCallCenter != null)
                {
                    //currentCallCenter.Selected = true;
                    dropDown.Items.Remove(currentCallCenter);
                }
            }

            dropDown.SelectedIndex = 0;
        }

        protected void Delete(object sender, EventArgs e)
        {
            var callCenterToMoveData = Int32.Parse(_callCenters.SelectedValue);
            var interviewerAction =
                (InterviewerActionOnCallCenterDelete)Enum.Parse(typeof (InterviewerActionOnCallCenterDelete), _interviewersAction.SelectedValue);

            if (ValidateCallCenterDeletion(callCenterToMoveData) == false)
            {
                return;
            }

            var currentCallCenterId = _callCenterProvider.GetCurrentId();

            CallCenterRepository.Delete(CallCenterId, callCenterToMoveData, interviewerAction);

            if (currentCallCenterId == CallCenterId)
            {
                RefreshUserSettings();
                RegisterStartupScript("refreshCallCenterInfo();");
            }
            
            CloseOverlay(true);
        }

        private bool ValidateCallCenterDeletion(int callCenterToMoveData)
        {
            if (_callCenterService.HasLoggedInPersons(CallCenterId, 0))
            {
                AddUserMessage(Strings.CallCenterDeletionHasLoggedPersonsWarning);
                return false;
            }

            if (CallCenterRepository.Default.ID == CallCenterId)
            {
                AddUserMessage(Strings.DefaultCallCenterDeletionWarning);
                return false;
            }

            if (callCenterToMoveData == CallCenterId)
            {
                AddUserMessage(Strings.CallCenterToMoveDataWarning);
                return false;
            }

            return true;
        }
    }
}
