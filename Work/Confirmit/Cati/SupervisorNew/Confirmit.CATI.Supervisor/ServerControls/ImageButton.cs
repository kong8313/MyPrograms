using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Controls;
using Confirmit.CATI.Supervisor.Core.Common;
namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class ImageButton : System.Web.UI.WebControls.Button
    {
        private string _resName = "";

        public ImageButton()
        {
            UseSubmitBehavior = false;
        }

        /// <summary>
        /// Resource identifier for Text property of the button
        /// </summary>
        public string ResName
        {
            get => _resName;
            set
            {
                _resName = value;
                Text = ResourceWrapper.Instance.GetString(value);
            }
        }

        /// <summary>
        /// Defines whever or not button generates postback to the server
        /// (default - true)
        /// </summary>
        public bool IsSubmit { get; set; } = true;

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
            Attributes.Remove("type");
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!CssClass.Contains("comd-button comd-button--icon"))
            {
                CssClass += " comd-button comd-button--icon";
            }
            
            AddAttributesToRender(writer);

            writer.RenderBeginTag("button");
            var imageProvider = new ImageProvider();
            writer.Write(imageProvider.GetSvg(ImageName, string.IsNullOrWhiteSpace(ToolTip) ? Text : ToolTip));
            writer.RenderEndTag();
        }

        public string ImageName
        {
            get => (string)this.ViewState[nameof(ImageName)] ?? string.Empty;
            set => ViewState[nameof(ImageName)] = value;
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
