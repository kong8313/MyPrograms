using System;
using System.Web;

using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Help
{
    public partial class HelpTextViewerPage : BaseForm
    {
        public override string Title
        {
            get
            {
                return HttpContext.Current.Server.UrlDecode(Request.QueryString["Title"]);
            }
        }

        public bool IsHelpTextStoredInSession
        {
            get
            {
                var useSession = Request.QueryString["useSession"];

                return  useSession!= null && bool.Parse(useSession);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string text = "Help text not specified";
            string textId = HttpContext.Current.Server.UrlDecode(Request.QueryString["HelpTextId"]);
            
            pagetitle.InnerHtml = HttpUtility.HtmlEncode(Title);

            if (textId != null)
            {
                text = GetText(textId);
            }

            helpSpan.InnerHtml = text;
        }

        private string GetText(string textId)
        {
            return IsHelpTextStoredInSession ? (string) Session[textId]: 
                                                Resources.Strings.ResourceManager.GetString(textId);
        }
    }
}
