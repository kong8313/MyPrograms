using System.Web.UI;

namespace Confirmit.CATI.Supervisor.Controls.Grid
{
    public class NotSubmitCheckBox : Control
    {
        public string Class { get; set; }

        public bool Checked { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(
                $"<div class =\"checkbox-selector-wrapper\"><INPUT id=\"{ClientID}\" type=\"checkbox\" class=\"{Class}\" {(Checked ? "checked=\"\"" : "")} /><span class=\"checkbox-prettier\"></span></div>");
            base.Render(writer);
        }
    }

    public class SelectionColumnTemplate : ITemplate
    {
        public void InstantiateIn(Control container)
        {
            var checkbox = new NotSubmitCheckBox { ID = "cbxSelection", Class = "Selection"};
            container.Controls.Add(checkbox);
        }
    }
}