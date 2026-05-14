using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Represents a control that allows the user to select a single item from a drop-down list
    /// </summary>
    public class DropDownList : System.Web.UI.WebControls.DropDownList
    {
        public string WrapperCssClass { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(
                $"<div " +
                $"class='dropdown-control {(string.IsNullOrEmpty(WrapperCssClass) ? "" : WrapperCssClass)}' " +
                $"style='{(!string.IsNullOrEmpty(Attributes["style"]) ? Attributes["style"] : "")}'>");
            base.Render(writer);
            writer.Write("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"><path d=\"M0 6l12 12L24 6z\"></path></svg>");
            writer.WriteEndTag("div");
        }

        public bool MaintainSelectedItemDuringDataBind
        {
            get { return (bool)(ViewState["MaintainSelectedItemDuringDataBind"] ?? false); }
            set { ViewState["MaintainSelectedItemDuringDataBind"] = value; }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            Attributes["style"] = "";
            base.AddAttributesToRender(writer);
        }

        public DropDownList()
        {
            if (String.IsNullOrEmpty(CssClass))
                CssClass = "plain_dropdown";
        }

        private string _selectedValue;

        protected override void OnDataBinding(EventArgs e)
        {
            _selectedValue = SelectedValue;

            base.OnDataBinding(e);
        }

        protected override void OnDataBound(EventArgs e)
        {
            base.OnDataBound(e);

            if (MaintainSelectedItemDuringDataBind)
            {
                ListItem selectedItem = Items.FindByValue(_selectedValue);
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;
                }
            }
        }
    }
}