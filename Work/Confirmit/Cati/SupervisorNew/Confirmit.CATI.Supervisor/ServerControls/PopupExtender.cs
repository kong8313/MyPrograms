using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public enum Direction
    {
        Down = 1,
        Up = 2,
    }
    public class PopupExtender: BaseControl
    {
        private bool m_ApplyDefaultStyle = true;
        private string m_DefaultSlaveStyle =
             "background-color:White; position:absolute; display:none; visibility:hidden;";
        private Direction m_PopupDirection = Direction.Down;

        private bool m_initializeOnPostback = true;
        private bool m_autohide = true;

        /// <summary>
        /// Gets or sets serverid of master control.
        /// </summary>
        public string MasterID { get; set; }

        /// <summary>
        /// Gets or sets serverid of slavecontrol.
        /// </summary>
        public string SlaveID { get; set; }

        /// <summary>
        /// Apply default popup dialog style to master control
        /// </summary>
        public bool ApplyDefaultStyle
        {
            get { return m_ApplyDefaultStyle; }
            set { m_ApplyDefaultStyle = value; }
        }

        /// <summary>
        /// Defines popup dialog direction
        /// </summary>
        public Direction PopupDirection
        {
            get { return m_PopupDirection; }
            set { m_PopupDirection = value; }
        }

        /// <summary>
        /// Any valid javascript code that shoud run after popup is shown,
        /// no <script></script> tags should be placed
        /// </summary>
        public string OnPopupScript { get; set; }

        /// <summary>
        /// Returs javascript code that closes popup dialog on the client
        /// </summary>
        public string CloseSlaveScript
        {
            get
            {
                return "hidePopup();";
            }
        }

        /// <summary>
        /// Gets or sets if popup should be hidden automatically on mouse click in any area (not popup area).
        /// True by default.
        /// </summary>
        public bool AutoHide
        {
            get
            {
                return m_autohide;
            } 
            set
            {
                m_autohide = value;
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether javascript events subscription and 
        /// script registration should be done on each postback. Default value is true.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if javascript events subscription and 
        /// script registration should be done on each postback; otherwise, <c>false</c>.
        /// </value>
        public bool InitializeOnPostback
        {
            get
            {
                return m_initializeOnPostback;
            }
            set
            {
                m_initializeOnPostback = value;
            }
        }

        protected void InitializeClientBehaviour(object sender, EventArgs e)
        {
            Control master_control = Parent.FindControl(MasterID) ?? BaseForm.FindControlRecursive(Page, MasterID);
            if (master_control == null)
            {
                throw new ApplicationException(string.Format(GetResString("Can't find control with id = '{0}'"),
                                                             MasterID));
            }

            Control slave_control = Parent.FindControl(SlaveID) ?? BaseForm.FindControlRecursive(Page, SlaveID);
            if (slave_control == null)
            {
                throw new ApplicationException(string.Format(GetResString("Can't find control with id = '{0}'"), SlaveID));
            }

            if (!(slave_control is WebControl))
            {
                throw new ApplicationException(
                    string.Format(GetResString("Control with id = '{0}' should be WebControl"), SlaveID));
            }

            PageHelper.RegisterClientLibrary("Client/PopupExtender.js");

            string initScript = string.Format(
                "initBehaviour(\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\");",
                master_control.ClientID,
                slave_control.ClientID,
                PopupDirection,
                "onPopupScript_" + MasterID + SlaveID + "()",
                AutoHide);

            Page.RegisterStartupScript(initScript);

            Page.RegisterScriptBlock(
                "function onPopupScript_" + MasterID + SlaveID + "(){" + OnPopupScript + "}",
                "onPopupScript_" + MasterID + SlaveID,
                GetType());
            
            if (m_ApplyDefaultStyle)
            {
                ((WebControl)slave_control).Attributes.Add("Style", m_DefaultSlaveStyle);
            }

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack || InitializeOnPostback)
            {
                Page.PreRenderComplete += new EventHandler(InitializeClientBehaviour);
            }
        }
    }
}