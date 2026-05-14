using System;
using System.Web.UI;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using System.Web.Script.Serialization;

namespace Confirmit.CATI.Supervisor.Controls
{
    public partial class CallCenterSwitch : BaseWUC
    {
        protected readonly ICallCenterRepository CallCenterRepository = ServiceLocator.Resolve<ICallCenterRepository>();

        protected string ClientControllerName
        {
            get { return ClientID + "_controller"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BackendInstance.Current.HasCallCentersAddon == false)
            {
                Visible = false;
                return;
            }

            var callCenterId = ServiceLocator.Resolve<ICallCenterProvider>().GetCurrentId();

            var callCenterName = CallCenterRepository.Get(callCenterId).Name;

            lbUserName.Text = User.Name;
            lbUserCallCenter.Text = callCenterName;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (User.IsCatiAdministratorOrPros)
            {
                pnlInfo.Attributes["onclick"] = String.Format("{0}.switchCallCenter();", ClientControllerName);
                pnlInfo.Style.Add(HtmlTextWriterStyle.Cursor, "pointer");

                Page.RegisterStartupScript(String.Format("var {0} = new CallCenterSwitchController({1});", ClientControllerName, GetClientSettings()));
            }
        }

        private string GetClientSettings()
        {
            var settings = new
            {
                callCenterNameElementId = lbUserCallCenter.ClientID,
            };

            return new JavaScriptSerializer().Serialize(settings);
        }        
    }
}