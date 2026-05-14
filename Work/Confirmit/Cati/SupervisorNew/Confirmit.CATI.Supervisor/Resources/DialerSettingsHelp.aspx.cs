using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.DialerSettings;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DialerSettingsHelp : BaseForm
    {
        /// <summary>
        /// Parameter identifier.
        /// </summary>
        private string  ParameterId
        {
            get { return (string)ViewState["ParameterId"]; }
            set { ViewState["ParameterId"] = value; }
        }

        /// <summary>
        /// Gets dialer type
        /// </summary>
        private static DiallerType DiallerType
        {
            get
            {
                return ServiceLocator.Resolve<IDialerSettings>().Dialer;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            toolBar.LeftLabel = Request.Params["ParameterName"]?? String.Empty;

            ParameterId = Request.Params["ParameterId"] ?? string.Empty;            
            pnlHelpText.Controls.Add(new Literal() { Text =  DialerSettingsParameterHelpManager.GetHelpStringKey(DiallerType, ParameterId)});
        }
    }    
}
