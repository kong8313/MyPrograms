using System;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Represents Apollo CP specific numeric text box
    /// </summary>
    public class ListBox: System.Web.UI.WebControls.ListBox
    {
        public ListBox()
        {
            if (String.IsNullOrEmpty(CssClass))
            {
                CssClass = "plain_listbox";
            }
        }
    }
}