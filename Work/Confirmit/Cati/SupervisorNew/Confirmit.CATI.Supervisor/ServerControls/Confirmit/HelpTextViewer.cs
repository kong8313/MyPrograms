using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ServerControls.Confirmit
{
    public class HelpTextViewer : WebControl
    {
        public string HelpTextId { get; set; }

        public string TitleTextId { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public bool UseSession { get; set; }

        public int CustomWidth { get; set; } = 300;

        public HelpTextViewer()
        {
            ImageUrl = string.Empty;
            Title = string.Empty;
            TitleTextId = string.Empty;
            HelpTextId = string.Empty;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            PageHelper.RegisterClientLibrary("Client/WindowManager.js");

            if (!string.IsNullOrEmpty(TitleTextId))
                Title = Strings.ResourceManager.GetString(TitleTextId);
        }

        protected override void Render(HtmlTextWriter w)
        {
            if (string.IsNullOrEmpty(HelpTextId)) return;

            var text = Strings.ResourceManager.GetString(HelpTextId);
            w.Write("<script>Y.on(\"load\", function () {" +
                    $"document.getElementById(\"{ClientID}\").onclick = function (e) {{ e.preventDefault(); window.showTooltip('{ClientID}', '{text?.Replace(Environment.NewLine,"").Replace("'", "&apos;")}', '{CustomWidth}'); }} }});</script>");

            w.Write(
                $"<div class='help-text-viewer' id=\"{ClientID}\">{new ImageProvider().GetSvg("help")}</div>");
        }
    }
}