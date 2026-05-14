using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public class SvgImage : WebControl
    {
        public string ImageName
        {
            get => (string)this.ViewState[nameof(ImageName)] ?? string.Empty;
            set => ViewState[nameof(ImageName)] = value;
        }

        public string Title { get; set; } = "";

        protected override void Render(HtmlTextWriter writer)
        {
            var renderWrapper = Style.Count > 0;
            if (renderWrapper)
            {
                RenderBeginTag(writer);
            }

            writer.Write(new ImageProvider().GetSvg(ImageName, Title));

            if (renderWrapper)
            {
                RenderEndTag(writer);
            }
        }
    }
}