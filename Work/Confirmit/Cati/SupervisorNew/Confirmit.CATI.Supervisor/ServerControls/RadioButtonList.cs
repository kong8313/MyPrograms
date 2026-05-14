using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.Util;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class RadioButtonList : System.Web.UI.WebControls.RadioButtonList
    {
        protected override void OnInit(EventArgs e)
        {
            CssClass = "cati-radio-list";
            if (RepeatDirection == RepeatDirection.Horizontal)
            {
                CssClass += " cati-radio-list--horizontal";
            }

            base.OnInit(e);
        }

        protected override void RenderItem(
            ListItemType itemType,
            int repeatIndex,
            RepeatInfo repeatInfo,
            HtmlTextWriter writer)
        {
            writer.AddAttribute("class", "flex-panel");
            writer.RenderBeginTag("div");

            writer.AddAttribute("class", "cati-radio");
            writer.RenderBeginTag("div");
            var label = new HtmlGenericControl("label") { InnerText = Items[repeatIndex].Text };
            label.Attributes.Add("for", $"{ClientID}_{repeatIndex}");
            Items[repeatIndex].Text = " ";
            base.RenderItem(itemType, repeatIndex, repeatInfo, writer);
            writer.RenderEndTag();

            label.RenderControl(writer);
            writer.RenderEndTag();
        }
    }
}