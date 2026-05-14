using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class UpdatePanel : System.Web.UI.UpdatePanel
    {
        private class ProgressAnimationTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                var div = new Panel { CssClass = "updatePanelProgress" };
                var image = new HtmlGenericControl("div");
                image.InnerHtml =
                    "<div class=\"comd-busy-dots comd-busy-dots--large\"><div class=\"comd-busy-dots__dot\"></div><div class=\"comd-busy-dots__dot\"></div><div class=\"comd-busy-dots__dot\"></div></div>";
                div.Controls.Add(image);
                container.Controls.Add(div);
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            var updateProgress =
                new UpdateProgress
                    {
                        ID = ID + "_progress",                        
                        ProgressTemplate = new ProgressAnimationTemplate(),                        
                    };
            ContentTemplateContainer.Controls.Add(updateProgress);
        }
    }
}