using System;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes.CallCenters;
using Confirmit.CATI.Supervisor.Core.CallCenters;

namespace Confirmit.CATI.Supervisor.CallCenters
{
    public partial class SwitchCallCenter : CallCenterBaseForm
    {
        protected readonly IChangeCallCenter CallCenterChanger = ServiceLocator.Resolve<IChangeCallCenter>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                lblUserLogin.Text = User.Name;

                InitCallCentersList();                
            }
        }

        private void InitCallCentersList()
        {
            BindCallCentersList(ddlCallCenter);

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            ddlCallCenter.SelectedValue = callCenterId.ToString();
        }

        protected void Switch(object sender, EventArgs e)
        {            
            CallCenterChanger.Change(Int32.Parse(ddlCallCenter.SelectedValue));

            CloseOverlay(true);
        }
    }
}