using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Core.Common;
namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class Button: System.Web.UI.WebControls.Button
    {
        private string m_sResName;
        private bool m_isSubmit = true;

        public Button()
        {
            UseSubmitBehavior = false;
            if (String.IsNullOrEmpty(CssClass))
                CssClass = "plain_button";
        }

        /// <summary>
        /// Resource identifier for Text property of the button
        /// </summary>
        public string ResName
        {
            get { return m_sResName; }
            set
            {
                m_sResName = value;
                Text = ResourceWrapper.Instance.GetString(value);
            }
        }

        /// <summary>
        /// Defines whever or not button generates postback to the server
        /// (default - true)
        /// </summary>
        public bool IsSubmit
        {
            get { return m_isSubmit; }
            set { m_isSubmit = value; }
        }

        /// <summary>
        /// Raises button click event
        /// </summary>
        /// <param name="e"></param>
        public void RaiseServerClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (!IsSubmit)
            {
                Attributes["onclick"] = string.Format("{0} return false;", EnsureEndWithSemiColon(Attributes["onclick"]));
            }

            base.AddAttributesToRender(writer);
        }

        private static string EnsureEndWithSemiColon(string value)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(value) == false)
            {
                result = value.Trim().EndsWith(";") ? value : value + ";";
            }

            return result;
        } 
    }
}
