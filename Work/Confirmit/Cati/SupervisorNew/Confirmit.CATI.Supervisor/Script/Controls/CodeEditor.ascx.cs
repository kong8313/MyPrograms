using System;
using System.Web;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Script.Controls
{
    public partial class CodeEditor : BaseWUC
    {
        protected string KeepSessionUrl => ConfigHelper.ConfirmitKeepSessionAspxUrl;

        public string Text
        {
            get => IsInternetExplorer() ? scriptEditor.Text : scriptText.Value;
            set
            {
                if (IsInternetExplorer())
                {
                    scriptEditor.Text = value;
                }
                else
                {
                    scriptText.Value = value;
                }
            }
        }

        [StoreInViewState] 
        public bool LargeScriptFeatures;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsInternetExplorer())
            {
                monacoContainer.Visible = false;
                scriptEditor.Visible = true;
            }
        }
        
        public bool IsInternetExplorer()
        {
            return IsInternetExplorer(Request.Headers["User-Agent"]);
        }

        private static bool IsInternetExplorer(string userAgent)
        {
            return userAgent.Contains("MSIE")
                || userAgent.Contains("Trident");
        }
    }
}