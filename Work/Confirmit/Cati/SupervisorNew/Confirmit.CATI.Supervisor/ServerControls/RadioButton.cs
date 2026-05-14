using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class RadioButton : System.Web.UI.WebControls.RadioButton
    {
        protected override void OnInit(EventArgs e)
        {
            CssClass = "cati-radio";
            base.OnInit(e);
        }
    }
}