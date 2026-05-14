using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class MultilineTextBox : TextBox
    {
        public MultilineTextBox()
        {
            CssClass = "settings-value-multiline";
            TextMode = TextBoxMode.MultiLine;
            Wrap = true;
        }
    }
}
