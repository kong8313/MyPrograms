using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class ClickablePanel : Panel, IPostBackEventHandler
    {
        public string CommandName { get; set; }

        public string OnClientClick { get; set; }

        public event CommandEventHandler Command;
                

        public void RaisePostBackEvent(string eventArgument)
        {
            OnCommand(new CommandEventArgs(CommandName ?? String.Empty, eventArgument));
        }

        private void OnCommand(CommandEventArgs commandEventArgs)
        {
            if (Command != null)
            {
                Command(this, commandEventArgs);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);            

            if (String.IsNullOrEmpty(OnClientClick) == false)
            {
                Attributes["onclick"] = OnClientClick;
            }
        }
        
    }
}