using System;
using System.Web.UI;

namespace Confirmit.CATI.Supervisor.ServerControls
{
	/// <summary>
	/// Represents Apollo CP specific text box
	/// </summary>
	public class TextBox : System.Web.UI.WebControls.TextBox
	{
        /// <summary>
        /// If page contains only one input element, browser will automatically submit page on enter press. 
        /// If this behaviour is not desired set this property to true. For more info check the following
        /// http://stackoverflow.com/questions/864924/when-does-an-html-input-tag-post-back-on-enter
        /// </summary>
        public bool DisableSubmitOnEnter
        {
            get;
            set;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!CssClass.Contains("plain_textbox"))
            {
                CssClass += " plain_textbox";
            }

            base.Render(writer);
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (DisableSubmitOnEnter)
            {
                Attributes["onkeydown"] = "return (event.keyCode!=13);";
            }

            base.AddAttributesToRender(writer);
        }
	}
}
