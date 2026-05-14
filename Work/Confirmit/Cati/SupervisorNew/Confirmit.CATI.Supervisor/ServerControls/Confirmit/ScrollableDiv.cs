using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Supervisor.ServerControls.Confirmit
{
    [ParseChildren(false), 
    PersistChildren(true)]
    public class ScrollableDiv : WebControl
    {
        private string ScrollPositionVariableName
        {
            get { return this.ClientID + "_scrollPosition"; }
        }
        
        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }
               
        protected override void OnPreRender(EventArgs e)
        {
            if (ScriptManager.GetCurrent(Page) == null)
            {
                throw new InternalErrorException("A ScriptManager control must exist on the current page.");
            }

            var script = @"var prm = Sys.WebForms.PageRequestManager.getInstance();
                 		   prm.add_pageLoaded(function(sender, args){
                           document.getElementById('" + this.ClientID + "').scrollTop = window['" + ScrollPositionVariableName + "']" +
                         "});";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "scrollableDiv", script, true);

            base.OnPreRender(e);
        }
        
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "activityscrollablediv");
            writer.AddAttribute("onscroll", "window['" + ScrollPositionVariableName + "'] = this.scrollTop;", false);
        }        
    }
}