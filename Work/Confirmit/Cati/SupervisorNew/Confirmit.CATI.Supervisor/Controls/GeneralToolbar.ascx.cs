using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Commands;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;

namespace Confirmit.CATI.Supervisor.Controls
{
    [ParseChildren(true, "RightMenuItems")]
    public partial class GeneralToolbar : BaseWUC, IPostBackEventHandler
    {
        public GeneralToolbar()
        {
            ToolbarLayout = ToolbarLayout.LabelAndMenu;
        }

        private Dictionary<string, Command> _commands = new Dictionary<string, Command>();

        public List<Command> Commands
        {
            set
            {
                foreach (Command command in value)
                {
                    _commands[command.Key] = command;
                }
            }
            get { return _commands.Values.ToList(); }
        }

        public string LeftLabel
        {
            get { return HttpUtility.HtmlDecode(leftLabel.Text); } 
            set { leftLabel.Text = HttpUtility.HtmlEncode(value); }
        }

        public ToolbarLayout ToolbarLayout { get; set; }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public XpMenuItemCollection RightMenuItems
        {
            get { return rightMenu.MenuItems; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public XpMenuItemCollection LeftMenuItems
        {
            get { return leftMenu.MenuItems; }
        }

        public bool RightMenuVisible
        {
            get { return rightMenuDiv.Visible; }
            set { rightMenuDiv.Visible = value; }
        }

        public bool MakeMarginForExpanCollapseButton { get; set; } = false;

        public string MenuCssClass { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(MenuCssClass))
            {
                leftMenu.CssClass = rightMenu.CssClass = MenuCssClass;
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (ToolbarLayout == ToolbarLayout.DoubleMenu || String.IsNullOrWhiteSpace(LeftLabel))
            {
                labelDiv.Visible = false;
            }

            if (ToolbarLayout != ToolbarLayout.DoubleMenu || LeftMenuItems.Count == 0)
            {
                leftMenuDiv.Visible = false;
            }

            if (MakeMarginForExpanCollapseButton)
            {
                rightMenuDiv.Attributes["class"] = "toolbar__right-menu--with-margin-for-collapse";
            }

            base.Render(writer);
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("__command_", StringComparison.Ordinal))
            {
                EventHandler eh = _commands[eventArgument.Replace("__command_", "")].ServerClickEventHandler;
                if (eh != null)
                    eh(this, EventArgs.Empty);
            }
            else if (_commands.ContainsKey(eventArgument))
            {
                EventHandler eh = _commands[eventArgument].ServerClickEventHandler;
                if (eh != null)
                    eh(this, EventArgs.Empty);
            }
        }

        public void AddCommandButton(ToolbarCommandButton item, Command command, bool enabled, Control ctrl, XpMenuItemCollection itemCollection = null)
        {
            if (itemCollection == null)
                itemCollection = RightMenuItems;

            item.LinkedCommand = command;
            item.BaseControl = ctrl;
            item.Enabled = enabled;
            itemCollection.Add(item);
        }
    }

    public enum ToolbarLayout
    {
        LabelAndMenu,
        DoubleMenu
    }
}