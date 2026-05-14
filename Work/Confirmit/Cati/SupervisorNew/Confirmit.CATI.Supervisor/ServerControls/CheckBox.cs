using System;
using System.Net.Mime;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    /// <summary>
    /// Summary description for CheckBox.
    /// </summary>
    public class CheckBox : System.Web.UI.WebControls.CheckBox
    {
        private string m_sResName = null;
        protected IResourceWrapper m_RS = null;

        //---------------------------------------------------------------------------
        public CheckBox() : base()
        {
            m_RS = ResourceWrapper.Instance;

            if (string.IsNullOrEmpty(Text))
            {
                Text = " ";
            }
        }
        //---------------------------------------------------------------------------
        public string ResName
        {
            get { return m_sResName; }
            set { m_sResName = value; }
        }

        //---------------------------------------------------------------------------
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!CssClass.Contains("checkbox-selector-wrapper"))
            {
                CssClass += $" checkbox-selector-wrapper checkbox-selector-wrapper--{(TextAlign == TextAlign.Right ? "right" : "left")}";
            }

            TextAlign = TextAlign.Right;

            if (m_RS != null && m_sResName != null)
            {
                Text = m_RS.GetString(this.m_sResName);
            }

            base.Render(writer);
        }
    }
}
