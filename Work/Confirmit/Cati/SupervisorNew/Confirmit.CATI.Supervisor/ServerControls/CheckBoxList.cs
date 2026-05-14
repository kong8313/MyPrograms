using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Represents a control that allows the user to select a single item from a drop-down list
    /// </summary>
    public class CheckBoxList : System.Web.UI.WebControls.CheckBoxList
    {
        private readonly HiddenField checkedCountField = new HiddenField();

        public string FunctionName
        {
            get { return "validateAndRecalculateCheckedCount"; }
        }        

        public bool KeepOneChecked
        {
            get;
            set;
        }

        public string ErrorMessageOnLastUncheck
        {
            get;
            set;
        }        

        protected override void OnPreRender(EventArgs e)
        {
            CssClass = "cati-checkbox-list";

            base.OnPreRender(e);

            if (KeepOneChecked)
            {
                checkedCountField.ID = ClientID + "checkedCountField";

                var checkedCount = Items.Cast<ListItem>().Where(x => x.Selected).Count();

                if (checkedCount == 0)
                {
                    throw new InvalidOperationException("If property KeepOneChecked set to true at least one item must have checked state.");
                }

                checkedCountField.Value = checkedCount.ToString();

                foreach (ListItem item in Items)
                {
                    item.Attributes["onclick"] = String.Format("javascript:{0}(this,'{1}');", FunctionName, checkedCountField.ID);
                }

                RegisterScripts();
            }
        }

        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);
            checkedCountField.RenderControl(output);
        }

        private void RegisterScripts()
        {
            if (Page.ClientScript.IsClientScriptBlockRegistered(FunctionName) == false)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("function {0}(inputElement, checkedCountFieldId){{", FunctionName);
                sb.Append("var field = document.getElementById(checkedCountFieldId);");
                sb.Append("var checkedCount = parseInt(field.value);");
                sb.Append("if(inputElement.checked){");
                sb.Append("   field.value = (checkedCount+1).toString();");
                sb.Append(" }");
                sb.Append(" else{");
                sb.Append("   if(checkedCount == 1){");
                sb.Append("      inputElement.checked = true;");
                sb.AppendFormat("alert('{0}');", ErrorMessageOnLastUncheck);
                sb.Append("      return;");
                sb.Append("   }");
                sb.Append("   field.value = (checkedCount-1).toString();");
                sb.Append(" }");
                sb.Append("}");

                Page.ClientScript.RegisterClientScriptBlock(GetType(), FunctionName, sb.ToString(), true);
            }
        }
    }
}
