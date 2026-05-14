using System;
using System.Linq;
using System.Web.UI;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;
using System.Text.RegularExpressions;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.ServerControls.Commands
{
    /// <summary>
    /// Defines base class for commands be may executed from context menu and toolbar of GeneralGrid
    /// </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public class Command
    {
        private string m_Caption;
        private string m_Key;
        private string m_Image;
        private string m_OnClientClick;
        private string m_confirmation = "";
        private Control m_owner;
        private string m_idColumnName = "ID";
        private bool m_enabled = true;

        public Command()
        { }

        public Command(string key, string caption, string image, EventHandler server_click)
        {
            m_Key = key;
            m_Caption = caption;
            m_Image = image;
            ServerClick += server_click;
        }

        public Command(string key, string caption, string image, string onClientClick)
        {
            m_Key = key;
            m_Caption = caption;
            m_Image = image;
            m_OnClientClick = onClientClick;
        }

        public Command(string key, string caption, string image, EventHandler server_click, string onClientClick)
        {
            m_Key = key;
            m_Caption = caption;
            m_Image = image;
            ServerClick += server_click;
            m_OnClientClick = onClientClick;
        }

        public string GetResString(string sId)
        {
            return (ResourceWrapper.Instance.GetString(sId));
        }

        public event EventHandler ServerClick;

        public EventHandler ServerClickEventHandler
        {
            get
            {
                return ServerClick != null ? (EventHandler)Delegate.Combine(ServerClick.GetInvocationList().Distinct().ToArray()) : null;
            }
        }

        /// <summary>
        /// Command's unique identificator
        /// </summary>
        public string Key
        {
            set { m_Key = value; }
            get { return m_Key; }
        }

        /// <summary>
        /// Command's image
        /// </summary>
        public string Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }

        /// <summary>
        /// Contains client script executed
        /// </summary>
        public virtual string OnClientClick
        {
            get { return m_OnClientClick; }
            set { m_OnClientClick = value; }
        }

        public string ValidateFunctionName { get; set; }

        /// <summary>
        /// Command's caption
        /// </summary>
        public string Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        /// <summary>
        /// Command's confirmation text
        /// </summary>
        public string Confirmation
        {
            get
            {
                return m_confirmation;
            }

            set
            {
                m_confirmation = value;
            }
        }

        public string PromptAcceptCode { get; set; } = "";

        /// <summary>
        /// Command's owner
        /// </summary>
        public virtual Control Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        /// <summary>
        /// Name of column in grid that contains ID to pass to underlying dialogs
        /// </summary>
        public string IDColumnName
        {
            get
            {
                return m_idColumnName;
            }
            set
            {
                m_idColumnName = value;
            }
        }

        /// <summary>
        /// Is Command enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }

        private CommandGridSelectMode m_selectMode = CommandGridSelectMode.No;
        /// <summary>
        /// Selection mode
        /// </summary>
        public CommandGridSelectMode SelectMode
        {
            get
            {
                return m_selectMode;
            }
            set
            {
                m_selectMode = value;
            }
        }

        public virtual string GetClientEventJavaScript(Page page, Control baseControl)
        {
            string command = string.Empty;
            string postbackReference = string.Empty;

            if (ServerClickEventHandler != null)
            {
                string strOnClientClick = OnClientClick;
                if (!string.IsNullOrEmpty(strOnClientClick))
                {
                    postbackReference = page.ClientScript.GetPostBackEventReference(new PostBackOptions(baseControl, Key, "", false, false, true, true, true, ""));
                    command = "if( " + strOnClientClick + " ) { " + postbackReference + "; }";
                }
                else
                {
                    postbackReference = page.ClientScript.GetPostBackEventReference(new PostBackOptions(baseControl, "__command_" + Key, "", false, false, true, true, true, ""));
                    command = postbackReference + ";";
                }
                //changes quotes to apostrophes to correct java-script
                command = Regex.Replace(command, "\"", "'");
            }
            else if (OnClientClick != null)
                command = OnClientClick + ";";

            if (m_confirmation != "")
            {
                if (string.IsNullOrEmpty(PromptAcceptCode))
                    command = $"if( confirm('{ResourceWrapper.Instance.GetString(m_confirmation)}') ) {{ {command} }};";
                else
                    command = $"if( prompt('{ResourceWrapper.Instance.GetString(m_confirmation)}') === '{PromptAcceptCode}') {{ {command} }};";
            }

            if (Owner is GeneralGrid)
            {
                var owner_grid = Owner as GeneralGrid;

                if (String.IsNullOrEmpty(ValidateFunctionName) == false)
                {
                    command = "if(!" + ValidateFunctionName + "(" + owner_grid.ClientControllerName + ")) { return; }" + command;
                }

                if (SelectMode == CommandGridSelectMode.SingleRow)
                {
                    command = "var row = " + owner_grid.ClientGetCurrentRow() + ";" +
                        "if( row == null )" +
                        "   { window.alert( \'" + Strings.NoRowchosen + "\' ); }" +
                        "else " +
                        "{" + command + "}";
                }
                else if (SelectMode == CommandGridSelectMode.MultiRow)
                {
                    command =
                        "var row = " + owner_grid.ClientGetCurrentRow() + ";" +
                        "if( row == null && !" + owner_grid.ClientGetIsRowsSelected() + "   )" +
                        "   { window.alert( \'" + Strings.NoRowsSelected + "\' ); }" +
                        "else " +
                        "{" + command + "}";
                }
            }
            else if (Owner is HierarchicalGridControl)
            {
                var owner_grid = Owner as HierarchicalGridControl;
                if (SelectMode == CommandGridSelectMode.SingleRow)
                {
                    command =
                        "var row = " + owner_grid.ClientGetCurrentRow() + ";" +
                        "if( row == null )" +
                        "   { window.alert( \'" + Strings.NoRowsSelected + "\' ); }" +
                        "else " +
                        "{" + command + "}";
                }
            }
            return command;
        }
    }
}